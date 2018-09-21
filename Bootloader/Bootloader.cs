using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArchonLightingSystem.Common;
using System.ComponentModel;
using System.Threading;
using ArchonLightingSystem.UsbApplication;

namespace ArchonLightingSystem.Bootloader
{
    enum SpecialByte
    {
        SOH = 1,
        EOT = 4,
        DLE = 16
    }

    public enum BootloaderCmd
    {
        READ_BOOT_INFO = 1,
        ERASE_FLASH,
        PROGRAM_FLASH,
        READ_CRC,
        JMP_TO_APP
    }

    enum TxStates
    {
        IDLE = 0,
        FIRST_TRY,
        RE_TRY
    }

    public class BootloaderState
    {
        public bool NoResponseFromDevice { get; set; }
        public bool Success { get; set; }
        public byte[] Data;
        public uint Length { get; set; }

        public BootloaderState()
        {
            Data = new byte[1000];
        }
    }

    public class Bootloader
    {
        const uint TxPacketMaxLength = 1000;
        const uint RxPacketMaxLength = 255;

        private UsbDriver usbDriver = null;
        private HexManager hexManager = null;
        private BootloaderState bootloaderState = new BootloaderState();
        private UInt32 NextRetryTimeInMs = 0;
        private BackgroundWorker bootloaderTaskWorker = new BackgroundWorker();
        private bool threadKilled = false;
        private BootloaderCmd lastCommandSent;
        private bool resetHexFilePtr = false;
        private bool fileLoaded = false;

        private byte[] TxPacket = new byte[TxPacketMaxLength];
        private UInt16 TxPacketLen = 0;
        private byte[] RxPacket = new byte[RxPacketMaxLength];
        private UInt16 RxPacketLen = 0;
        private UInt16 RetryCount = 0;
        private bool RxFrameValid = false;
        private volatile TxStates txState = TxStates.IDLE;
        private volatile UInt16 MaxRetry = 0;
        private volatile UInt16 TxRetryDelay = 0;

        public Bootloader()
        {
            resetHexFilePtr = true;
        }

        public void InitializeBootloader(UsbDriver usb, ProgressChangedEventHandler progressEventHandler)
        {
            usbDriver = usb;
            
            hexManager = new HexManager();
            CreateBootloaderThread(progressEventHandler);
        }

        ~Bootloader()
        {
            if (bootloaderTaskWorker.IsBusy)
            {
                bootloaderTaskWorker.CancelAsync();
            }
            bootloaderTaskWorker.Dispose();
        }


        /// <summary>
        /// This worker calls receive and transmit tasks for bootloader usb operation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BootloaderTask_DoWork(object sender, DoWorkEventArgs e)
        {
            while (!this.bootloaderTaskWorker.CancellationPending)
            {
                try
                {
                    ReceiveTask();
                    TransmitTask();
                    Thread.Sleep(10);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            threadKilled = true;
        }

        /// <summary>
        /// Stop the bootloader operation in progress.
        /// </summary>
        public void ShutdownThread()
        {
            bootloaderTaskWorker.CancelAsync();
        }

        /// <summary>
        /// Creates background task which does bootloader loading task and reports progress.
        /// </summary>
        private void CreateBootloaderThread(ProgressChangedEventHandler progressEventHandler)
        {
            bootloaderTaskWorker = new BackgroundWorker();
            bootloaderTaskWorker.WorkerReportsProgress = true;
            bootloaderTaskWorker.WorkerSupportsCancellation = true;
            bootloaderTaskWorker.ProgressChanged += progressEventHandler;
            bootloaderTaskWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BootloaderTask_RunWorkerCompleted);
            bootloaderTaskWorker.DoWork += new DoWorkEventHandler(BootloaderTask_DoWork);
            bootloaderTaskWorker.RunWorkerAsync();

        }

        private void BootloaderTask_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {

        }

        private void BootloaderTask_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

        }

        /// <summary>
        /// Process received USB responses from bootloader firmware.
        /// </summary>
        private void ReceiveTask()
        {
            UInt32 buffLen;
            uint buffSize = 255;
            byte[] buff = new byte[buffSize];

            buffLen = ReadPort(ref buff, (buffSize - 10));
            BuildRxFrame(buff, buffLen);
            if (RxFrameValid)
            {
                // Valid frame is received.
                // Disable further retries.
                StopTxRetries();
                RxFrameValid = false;
                // Handle Response
                HandleResponse();
            }
            else
            {
                // Retries exceeded. There is no reponse from the device.
                if (bootloaderState.NoResponseFromDevice)
                {
                    // Reset flags
                    bootloaderState.NoResponseFromDevice = false;
                    RxFrameValid = false;
                    // Handle no response situation.
                    HandleNoResponse();
                }
            }
        }


        /// <summary>
        /// Handle situation where no response is received.
        /// </summary>
        private void HandleNoResponse()
        {
            // Handle no response situation depending on the last sent command.
            switch (lastCommandSent)
            {
                case BootloaderCmd.READ_BOOT_INFO:
                case BootloaderCmd.ERASE_FLASH:
                case BootloaderCmd.PROGRAM_FLASH:
                case BootloaderCmd.READ_CRC:
                    // Notify main window that there was no reponse.
                    bootloaderTaskWorker.ReportProgress(0, bootloaderState);
                    break;

            }
        }

        /// <summary>
        /// Handle responses packets from bootloader firmware.
        /// </summary>
        private void HandleResponse()
        {
            BootloaderCmd cmd = (BootloaderCmd)RxPacket[0];
            bootloaderState.Length = 0;


            switch (cmd)
            {
                case BootloaderCmd.READ_BOOT_INFO:
                case BootloaderCmd.ERASE_FLASH:
                case BootloaderCmd.READ_CRC:
                    // Notify main window that command received successfully.
                    Util.CopyArray(ref bootloaderState.Data, 0, ref RxPacket, 0, RxPacketLen);
                    bootloaderState.Length = RxPacketLen;
                    bootloaderState.Success = true;
                    bootloaderTaskWorker.ReportProgress(0, bootloaderState);
                    break;

                case BootloaderCmd.PROGRAM_FLASH:

                    // If there is a hex record, send next hex record.
                    resetHexFilePtr = false; // No need to reset hex file pointer.
                    if (!SendCommand(BootloaderCmd.PROGRAM_FLASH, MaxRetry, TxRetryDelay))
                    {
                        // Notify main window that programming operation completed.
                        Util.CopyArray(ref bootloaderState.Data, 0, ref RxPacket, 0, RxPacketLen);
                        bootloaderState.Length = RxPacketLen;
                        bootloaderState.Success = true;
                        bootloaderTaskWorker.ReportProgress(0, bootloaderState);
                    }
                    resetHexFilePtr = true;
                    break;
            }
            bootloaderState.Success = false;
        }

         /// <summary>
         /// Build the response frame
         /// </summary>
         /// <param name="buff">Data buffer</param>
         /// <param name="buffLen">Buffer length</param>
        private void BuildRxFrame(byte[] buff, UInt32 buffLen)
        {
            bool Escape = false;
            UInt16 crc;
            uint bufferIdx = 0;


            while ((buffLen > 0) && (RxFrameValid == false))
            {
                buffLen--;

                if (RxPacketLen >= (RxPacketMaxLength - 2))
                {
                    RxPacketLen = 0;
                }

                switch ((SpecialByte)buff[bufferIdx])
                {
                    case SpecialByte.SOH: //Start of header
                        if (Escape)
                        {
                            // Received byte is not SOH, but data.
                            RxPacket[RxPacketLen++] = buff[bufferIdx];
                            // Reset Escape Flag.
                            Escape = false;
                        }
                        else
                        {
                            // Received byte is indeed a SOH which indicates start of new frame.
                            RxPacketLen = 0;
                        }
                        break;

                    case SpecialByte.EOT: // End of transmission
                        if (Escape)
                        {
                            // Received byte is not EOT, but data.
                            RxPacket[RxPacketLen++] = buff[bufferIdx];
                            // Reset Escape Flag.
                            Escape = false;
                        }
                        else
                        {
                            // Received byte is indeed a EOT which indicates end of frame.
                            // Calculate CRC to check the validity of the frame.
                            if (RxPacketLen > 1)
                            {
                                crc = (UInt16)((RxPacket[RxPacketLen - 2]) & 0x00ff);
                                crc = (UInt16)(crc | (UInt16)((RxPacket[RxPacketLen - 1] << 8) & 0xFF00));
                                if (( Util.CalculateCrc(RxPacket, 0, (UInt32)(RxPacketLen - 2)) == crc) && (RxPacketLen > 2))
                                {
                                    // CRC matches and frame received is valid.
                                    RxFrameValid = true;
                                }
                            }
                        }
                        break;


                    case SpecialByte.DLE: // Escape character received.
                        if (Escape)
                        {
                            // Received byte is not ESC but data.
                            RxPacket[RxPacketLen++] = buff[bufferIdx];
                            // Reset Escape Flag.
                            Escape = false;
                        }
                        else
                        {
                            // Received byte is an escape character. Set Escape flag to escape next byte.
                            Escape = true;
                        }
                        break;

                    default: // Data field.
                        RxPacket[RxPacketLen++] = buff[bufferIdx];
                        // Reset Escape Flag.
                        Escape = false;
                        break;

                }
                // Increment the buffer index.
                bufferIdx++;
            }
        }


        /// <summary>
        /// Transmit frame if there is data to send.
        /// </summary>
        private void TransmitTask()
        {

            switch (txState)
            {
                case TxStates.FIRST_TRY:
                    if (RetryCount > 0)
                    {
                        // There is something to send.
                        WritePort(TxPacket, TxPacketLen);
                        RetryCount--;
                        // If there is no response to "first try", the command will be retried.
                        txState = TxStates.RE_TRY;
                        // Next retry should be attempted only after a delay.
                        NextRetryTimeInMs = (UInt32)DateTime.Now.Millisecond + TxRetryDelay;
                    }
                    break;

                case TxStates.RE_TRY:
                    if (RetryCount > 0)
                    {
                        if (NextRetryTimeInMs < (UInt32)Util.GetCurrentUnixTimestampMillis())
                        {
                            // Delay elapsed. Its time to retry.
                            NextRetryTimeInMs = (UInt32)Util.GetCurrentUnixTimestampMillis() + TxRetryDelay;
                            WritePort(TxPacket, TxPacketLen);
                            // Decrement retry count.
                            RetryCount--;

                        }
                    }
                    else
                    {
                        // Retries Exceeded
                        bootloaderState.NoResponseFromDevice = true;
                        // Reset the state
                        txState = TxStates.IDLE;
                    }
                    break;
            }
        }

        /// <summary>
        /// Stop transmission retries.
        /// </summary>
        private void StopTxRetries()
        {
            // Reset state.
            txState = TxStates.IDLE;
            RetryCount = 0;
        }

        /// <summary>
        /// Send command to the bootloader firmware.
        /// </summary>
        /// <param name="cmd">Command code</param>
        /// <param name="retries">Number of retries allowed</param>
        /// <param name="delayInMs">Delay between retries in milliseconds</param>
        /// <returns></returns>
        public bool SendCommand(BootloaderCmd cmd, UInt16 retries, UInt16 delayInMs)
        {
            UInt32 buffSize = 1000;
            byte[] buff = new byte[buffSize];
            UInt16 crc = 0;
            UInt32 StartAddress = 0, Len = 0;
            UInt16 buffLen = 0;
            UInt16 HexRecLen = 0;
            UInt32 totalRecords = 10;

            if(txState > TxStates.IDLE)
            {
                // transmission in progress
                return false;
            }

            TxPacketLen = 0;
            lastCommandSent = cmd;

            switch ((BootloaderCmd)cmd)
	        {
	            case BootloaderCmd.READ_BOOT_INFO:
                    buff[buffLen++] = (byte)cmd;
                    MaxRetry = RetryCount = retries;
                    TxRetryDelay = delayInMs; // in ms
                    break;

	            case BootloaderCmd.ERASE_FLASH:
                    buff[buffLen++] = (byte)cmd;
                    MaxRetry = RetryCount = retries;
                    TxRetryDelay = delayInMs; // in ms
                    break;

	            case BootloaderCmd.JMP_TO_APP:
                    buff[buffLen++] = (byte)cmd;
                    MaxRetry = RetryCount = 1;
                    TxRetryDelay = 10; // in ms
                    break;
	
	            case BootloaderCmd.PROGRAM_FLASH:
                    buff[buffLen++] = (byte)cmd;
                    if (resetHexFilePtr)
                    {
                        if (!hexManager.ResetHexFilePointer())
                        {
                            // Error in resetting the file pointer
                            return false;
                        }
                    }
                    HexRecLen = hexManager.GetNextHexRecord(ref buff, buffLen, (buffSize - 5));
                    if (HexRecLen == 0)
                    {
                        // no more records
                        return false;
                    }

                    buffLen = (UInt16)(buffLen + HexRecLen);
                    while (totalRecords > 0)
                    {
                        HexRecLen = hexManager.GetNextHexRecord(ref buff, buffLen, (buffSize - 5));
                        buffLen = (UInt16)(buffLen + HexRecLen);
                        totalRecords--;
                    }
                    MaxRetry = RetryCount = retries;
                    TxRetryDelay = delayInMs; // in ms
                    UpdateProgress();
                    break;

	            case BootloaderCmd.READ_CRC:
                    buff[buffLen++] = (byte)cmd;
                    hexManager.VerifyFlash(ref StartAddress, ref Len, ref crc);
                    buff[buffLen++] = (byte)(StartAddress);
                    buff[buffLen++] = (byte)(StartAddress >> 8);
                    buff[buffLen++] = (byte)(StartAddress >> 16);
                    buff[buffLen++] = (byte)(StartAddress >> 24);
                    buff[buffLen++] = (byte)(Len);
                    buff[buffLen++] = (byte)(Len >> 8);
                    buff[buffLen++] = (byte)(Len >> 16);
                    buff[buffLen++] = (byte)(Len >> 24);
                    buff[buffLen++] = (byte)crc;
                    buff[buffLen++] = (byte)(crc >> 8);
                    MaxRetry = RetryCount = retries;
                    TxRetryDelay = delayInMs; // in ms
                    break;

                default:
		            return false;

            }

            // Calculate CRC for the frame.
            crc = Util.CalculateCrc(buff, 0, buffLen);
            buff[buffLen++] = (byte)crc;
            buff[buffLen++] = (byte)(crc >> 8);

            // SOH: Start of header
            TxPacket[TxPacketLen++] = (byte)SpecialByte.SOH;

            // Form TxPacket. Insert DLE in the data field whereever SOH and EOT are present.
            for (int i = 0; i < buffLen; i++)
            {
                if ((buff[i] == (byte)SpecialByte.EOT) || (buff[i] == (byte)SpecialByte.SOH)
                        || (buff[i] == (byte)SpecialByte.DLE))
                {
                    TxPacket[TxPacketLen++] = (byte)SpecialByte.DLE;
                }
                TxPacket[TxPacketLen++] = buff[i];
            }

            // EOT: End of transmission
            TxPacket[TxPacketLen++] = (byte)SpecialByte.EOT;
            txState = TxStates.FIRST_TRY;

            return true;

        }

        /// <summary>
        /// Gets the progress of each command. This function can be used for progress bar.
        /// </summary>
        /// <param name="currentProgress">Current value of progress.</param>
        /// <param name="maxProgress">Progress value for completed job.</param>
        void UpdateProgress()
        {
            bootloaderState.Length = 0;
            switch (lastCommandSent)
            {
                case BootloaderCmd.READ_BOOT_INFO:
                case BootloaderCmd.ERASE_FLASH:
                case BootloaderCmd.READ_CRC:
                case BootloaderCmd.JMP_TO_APP:
                    // Progress with respect to retry count.
                    bootloaderTaskWorker.ReportProgress((int)((1 - RetryCount / MaxRetry) * 100), bootloaderState);
                    break;

                case BootloaderCmd.PROGRAM_FLASH:
                    // Progress with respect to line counts in hex file.
                    bootloaderTaskWorker.ReportProgress((int)(((float)hexManager.HexCurrLineNo / (float)hexManager.HexTotalLines) * 100f), bootloaderState);
                    break;
            }
        }

        /// <summary>
        /// Gets the locally calculated CRC
        /// </summary>
        /// <returns></returns>
        UInt16 CalculateFlashCRC()
        {
            UInt32 StartAddress = 0, Len = 0;
            UInt16 crc = 0;
            hexManager.VerifyFlash(ref StartAddress, ref Len, ref crc);
            return crc;
        }

        /// <summary>
        /// Loads the HEX file for programming.
        /// </summary>
        /// <param name="filepath">HEX file path.</param>
        /// <returns>True if file loaded successfully</returns>
        public bool LoadHexFile(string filepath)
        {
            return hexManager.LoadHexFile(filepath);
        }

        /****************************************************************************
         *  Write communication port (USB/COM/ETH)
         *
         * \param Buffer, Len
         * \return 
         *****************************************************************************/
        void WritePort(byte[] buffer, UInt32 bufflen)
        {
            uint index = 0;
            while (bufflen - index > 0)
            {
                byte[] usbBuff = new byte[64];
                uint sendLen = (bufflen - index) > 64 ? 64 : bufflen - index;
                Util.CopyArray(ref usbBuff, 0, ref buffer, index, (int)sendLen);
                usbDriver.WriteUSBDevice(usbBuff, sendLen);
                index += sendLen;
                if(bufflen - index > 0)
                {
                    Thread.Sleep(5);
                }
            }
        }


        /****************************************************************************
         *  Read communication port (USB/COM/ETH)
         *
         * \param Buffer, Len
         * \return 
         *****************************************************************************/
        UInt32 ReadPort(ref byte[] buffer, UInt32 bufflen)
        {
            return usbDriver.ReadUSBDevice(ref buffer, bufflen);
        }

    }
}
