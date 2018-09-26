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

    public enum TxStates
    {
        IDLE = 0,
        FIRST_TRY,
        RE_TRY
    }

    public class BootloaderStatus
    {
        public int DeviceIndex { get; set; }
        public Version BootloaderVersion { get; set; }
        public Version ApplicationVersion { get; set; }
        public UInt32 BootStatus { get; set; }
        public byte Address { get; set; }
        public bool IsEnabled { get; set; }
        public bool IsInitialized { get; set; }
        public bool NoResponseFromDevice { get; set; }
        public bool Success { get; set; }
        public byte[] Data;
        public uint Length { get; set; }

        public BootloaderStatus(int index)
        {
            Data = new byte[1000];
            BootloaderVersion = new Version();
            ApplicationVersion = new Version();
            DeviceIndex = index;
        }
    }

    public class BootloaderState
    {
        public byte[] TxPacket = null;
        public UInt16 TxPacketLen = 0;
        public byte[] RxPacket = null;
        public UInt16 RxPacketLen = 0;
        public UInt16 RetryCount = 0;
        public bool RxFrameValid = false;
        public volatile TxStates txState = TxStates.IDLE;
        public volatile UInt16 MaxRetry = 0;
        public volatile UInt16 TxRetryDelay = 0;
        public UInt64 NextRetryTimeInMs = 0;
        public BootloaderCmd lastCommandSent;
        public bool resetHexFilePtr = true;
        public BootloaderStatus bootloaderStatus = null;

        public BootloaderState(uint txMaxLen, uint rxMaxLen)
        {
            TxPacket = new byte[txMaxLen];
            RxPacket = new byte[rxMaxLen];
        }
    }

    public class Bootloader
    {
        const uint TxPacketMaxLength = 1000;
        const uint RxPacketMaxLength = 255;
        const uint DeviceCount = 16;

        private UsbDriver usbDriver = null;
        private HexManager hexManager = null;
        private BackgroundWorker bootloaderTaskWorker = new BackgroundWorker();
        private BootloaderState[] bootState = new BootloaderState[DeviceCount];
        

        public Bootloader()
        {
            for(int i = 0; i < DeviceCount; i++)
            {
                bootState[i] = new BootloaderState(TxPacketMaxLength, RxPacketMaxLength);
                bootState[i].bootloaderStatus = new BootloaderStatus(i);
            }
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
                    for(int i = 0; i < DeviceCount; i++)
                    {
                        if (bootState[i].bootloaderStatus.IsEnabled)
                        {
                            ReceiveTask(i);
                            TransmitTask(i);
                        }
                    }
                    Thread.Sleep(10);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
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
        private void ReceiveTask(int deviceIdx)
        {
            UInt32 buffLen;
            uint buffSize = 255;
            byte[] buff = new byte[buffSize];

            buffLen = ReadPort(deviceIdx, ref buff, (buffSize - 10));
            BuildRxFrame(deviceIdx, buff, buffLen);
            if (bootState[deviceIdx].RxFrameValid)
            {
                // Valid frame is received.
                // Disable further retries.
                StopTxRetries(deviceIdx);
                bootState[deviceIdx].RxFrameValid = false;
                // Handle Response
                HandleResponse(deviceIdx);
            }
            else
            {
                // Retries exceeded. There is no reponse from the device.
                if (bootState[deviceIdx].bootloaderStatus.NoResponseFromDevice)
                {
                    // Reset flags
                    bootState[deviceIdx].bootloaderStatus.NoResponseFromDevice = false;
                    bootState[deviceIdx].RxFrameValid = false;
                    // Handle no response situation.
                    HandleNoResponse(deviceIdx);
                }
            }
        }


        /// <summary>
        /// Handle situation where no response is received.
        /// </summary>
        private void HandleNoResponse(int deviceIdx)
        {
            // Handle no response situation depending on the last sent command.
            switch (bootState[deviceIdx].lastCommandSent)
            {
                case BootloaderCmd.READ_BOOT_INFO:
                case BootloaderCmd.ERASE_FLASH:
                case BootloaderCmd.PROGRAM_FLASH:
                case BootloaderCmd.READ_CRC:
                    // Notify main window that there was no reponse.
                    bootloaderTaskWorker.ReportProgress(0, bootState[deviceIdx].bootloaderStatus);
                    break;

            }
        }

        /// <summary>
        /// Handle responses packets from bootloader firmware.
        /// </summary>
        private void HandleResponse(int deviceIdx)
        {
            BootloaderCmd cmd = (BootloaderCmd)bootState[deviceIdx].RxPacket[0];
            bootState[deviceIdx].bootloaderStatus.Length = 0;


            switch (cmd)
            {
                case BootloaderCmd.READ_BOOT_INFO:
                case BootloaderCmd.ERASE_FLASH:
                case BootloaderCmd.READ_CRC:
                    // Notify main window that command received successfully.
                    Util.CopyArray(ref bootState[deviceIdx].bootloaderStatus.Data, 0, ref bootState[deviceIdx].RxPacket, 0, bootState[deviceIdx].RxPacketLen);
                    bootState[deviceIdx].bootloaderStatus.Length = bootState[deviceIdx].RxPacketLen;
                    bootState[deviceIdx].bootloaderStatus.Success = true;
                    bootloaderTaskWorker.ReportProgress(0, bootState[deviceIdx].bootloaderStatus);
                    break;

                case BootloaderCmd.PROGRAM_FLASH:

                    // If there is a hex record, send next hex record.
                    bootState[deviceIdx].resetHexFilePtr = false; // No need to reset hex file pointer.
                    if (!SendCommand(deviceIdx, BootloaderCmd.PROGRAM_FLASH, bootState[deviceIdx].MaxRetry, bootState[deviceIdx].TxRetryDelay))
                    {
                        // Notify main window that programming operation completed.
                        Util.CopyArray(ref bootState[deviceIdx].bootloaderStatus.Data, 0, ref bootState[deviceIdx].RxPacket, 0, bootState[deviceIdx].RxPacketLen);
                        bootState[deviceIdx].bootloaderStatus.Length = bootState[deviceIdx].RxPacketLen;
                        bootState[deviceIdx].bootloaderStatus.Success = true;
                        bootloaderTaskWorker.ReportProgress(0, bootState[deviceIdx].bootloaderStatus);
                    }
                    bootState[deviceIdx].resetHexFilePtr = true;
                    break;
            }
            bootState[deviceIdx].bootloaderStatus.Success = false;
        }

         /// <summary>
         /// Build the response frame
         /// </summary>
         /// <param name="buff">Data buffer</param>
         /// <param name="buffLen">Buffer length</param>
        private void BuildRxFrame(int deviceIdx, byte[] buff, UInt32 buffLen)
        {
            bool Escape = false;
            UInt16 crc;
            uint bufferIdx = 0;


            while ((buffLen > 0) && (bootState[deviceIdx].RxFrameValid == false))
            {
                buffLen--;

                if (bootState[deviceIdx].RxPacketLen >= (RxPacketMaxLength - 2))
                {
                    bootState[deviceIdx].RxPacketLen = 0;
                }

                switch ((SpecialByte)buff[bufferIdx])
                {
                    case SpecialByte.SOH: //Start of header
                        if (Escape)
                        {
                            // Received byte is not SOH, but data.
                            bootState[deviceIdx].RxPacket[bootState[deviceIdx].RxPacketLen++] = buff[bufferIdx];
                            // Reset Escape Flag.
                            Escape = false;
                        }
                        else
                        {
                            // Received byte is indeed a SOH which indicates start of new frame.
                            bootState[deviceIdx].RxPacketLen = 0;
                        }
                        break;

                    case SpecialByte.EOT: // End of transmission
                        if (Escape)
                        {
                            // Received byte is not EOT, but data.
                            bootState[deviceIdx].RxPacket[bootState[deviceIdx].RxPacketLen++] = buff[bufferIdx];
                            // Reset Escape Flag.
                            Escape = false;
                        }
                        else
                        {
                            // Received byte is indeed a EOT which indicates end of frame.
                            // Calculate CRC to check the validity of the frame.
                            if (bootState[deviceIdx].RxPacketLen > 1)
                            {
                                crc = (UInt16)((bootState[deviceIdx].RxPacket[bootState[deviceIdx].RxPacketLen - 2]) & 0x00ff);
                                crc = (UInt16)(crc | (UInt16)((bootState[deviceIdx].RxPacket[bootState[deviceIdx].RxPacketLen - 1] << 8) & 0xFF00));
                                if (( Util.CalculateCrc(bootState[deviceIdx].RxPacket, 0, (UInt32)(bootState[deviceIdx].RxPacketLen - 2)) == crc) && (bootState[deviceIdx].RxPacketLen > 2))
                                {
                                    // CRC matches and frame received is valid.
                                    bootState[deviceIdx].RxFrameValid = true;
                                }
                            }
                        }
                        break;


                    case SpecialByte.DLE: // Escape character received.
                        if (Escape)
                        {
                            // Received byte is not ESC but data.
                            bootState[deviceIdx].RxPacket[bootState[deviceIdx].RxPacketLen++] = buff[bufferIdx];
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
                        bootState[deviceIdx].RxPacket[bootState[deviceIdx].RxPacketLen++] = buff[bufferIdx];
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
        private void TransmitTask(int deviceIdx)
        {

            switch (bootState[deviceIdx].txState)
            {
                case TxStates.FIRST_TRY:
                    if (bootState[deviceIdx].RetryCount > 0)
                    {
                        // There is something to send.
                        WritePort(deviceIdx, bootState[deviceIdx].TxPacket, bootState[deviceIdx].TxPacketLen);
                        bootState[deviceIdx].RetryCount--;
                        // If there is no response to "first try", the command will be retried.
                        bootState[deviceIdx].txState = TxStates.RE_TRY;
                        // Next retry should be attempted only after a delay.
                        bootState[deviceIdx].NextRetryTimeInMs = (UInt64)Util.GetCurrentUnixTimestampMillis() + bootState[deviceIdx].TxRetryDelay;
                    }
                    break;

                case TxStates.RE_TRY:
                    if (bootState[deviceIdx].RetryCount > 0)
                    {
                        if (bootState[deviceIdx].NextRetryTimeInMs < (UInt64)Util.GetCurrentUnixTimestampMillis())
                        {
                            // Delay elapsed. Its time to retry.
                            bootState[deviceIdx].NextRetryTimeInMs = (UInt64)Util.GetCurrentUnixTimestampMillis() + bootState[deviceIdx].TxRetryDelay;
                            WritePort(deviceIdx, bootState[deviceIdx].TxPacket, bootState[deviceIdx].TxPacketLen);
                            // Decrement retry count.
                            bootState[deviceIdx].RetryCount--;

                        }
                    }
                    else
                    {
                        // Retries Exceeded
                        bootState[deviceIdx].bootloaderStatus.NoResponseFromDevice = true;
                        // Reset the state
                        bootState[deviceIdx].txState = TxStates.IDLE;
                    }
                    break;
            }
        }

        /// <summary>
        /// Stop transmission retries.
        /// </summary>
        private void StopTxRetries(int deviceIdx)
        {
            // Reset state.
            bootState[deviceIdx].txState = TxStates.IDLE;
            bootState[deviceIdx].RetryCount = 0;
        }

        /// <summary>
        /// Send command to the bootloader firmware.
        /// </summary>
        /// <param name="cmd">Command code</param>
        /// <param name="retries">Number of retries allowed</param>
        /// <param name="delayInMs">Delay between retries in milliseconds</param>
        /// <returns></returns>
        public bool SendCommand(int deviceIdx, BootloaderCmd cmd, UInt16 retries, UInt16 delayInMs)
        {
            UInt32 buffSize = 1000;
            byte[] buff = new byte[buffSize];
            UInt16 crc = 0;
            UInt32 StartAddress = 0, Len = 0;
            UInt16 buffLen = 0;
            UInt16 HexRecLen = 0;
            UInt32 totalRecords = 10;

            bootState[deviceIdx].bootloaderStatus.IsEnabled = true;

            if(bootState[deviceIdx].txState > TxStates.IDLE)
            {
                // transmission in progress
                return false;
            }

            bootState[deviceIdx].TxPacketLen = 0;
            bootState[deviceIdx].lastCommandSent = cmd;

            switch ((BootloaderCmd)cmd)
	        {
	            case BootloaderCmd.READ_BOOT_INFO:
                    buff[buffLen++] = (byte)cmd;
                    bootState[deviceIdx].MaxRetry = bootState[deviceIdx].RetryCount = retries;
                    bootState[deviceIdx].TxRetryDelay = delayInMs; // in ms
                    break;

	            case BootloaderCmd.ERASE_FLASH:
                    buff[buffLen++] = (byte)cmd;
                    bootState[deviceIdx].MaxRetry = bootState[deviceIdx].RetryCount = retries;
                    bootState[deviceIdx].TxRetryDelay = delayInMs; // in ms
                    break;

	            case BootloaderCmd.JMP_TO_APP:
                    buff[buffLen++] = (byte)cmd;
                    bootState[deviceIdx].MaxRetry = bootState[deviceIdx].RetryCount = 1;
                    bootState[deviceIdx].TxRetryDelay = 10; // in ms
                    break;
	
	            case BootloaderCmd.PROGRAM_FLASH:
                    buff[buffLen++] = (byte)cmd;
                    if (bootState[deviceIdx].resetHexFilePtr)
                    {
                        if (!hexManager.ResetHexFilePointer(deviceIdx))
                        {
                            // Error in resetting the file pointer
                            return false;
                        }
                    }
                    HexRecLen = hexManager.GetNextHexRecord(deviceIdx, ref buff, buffLen, (buffSize - 5));
                    if (HexRecLen == 0)
                    {
                        // no more records
                        return false;
                    }

                    buffLen = (UInt16)(buffLen + HexRecLen);
                    while (totalRecords > 0)
                    {
                        HexRecLen = hexManager.GetNextHexRecord(deviceIdx, ref buff, buffLen, (buffSize - 5));
                        buffLen = (UInt16)(buffLen + HexRecLen);
                        totalRecords--;
                    }
                    bootState[deviceIdx].MaxRetry = bootState[deviceIdx].RetryCount = retries;
                    bootState[deviceIdx].TxRetryDelay = delayInMs; // in ms
                    UpdateProgress(deviceIdx);
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
                    bootState[deviceIdx].MaxRetry = bootState[deviceIdx].RetryCount = retries;
                    bootState[deviceIdx].TxRetryDelay = delayInMs; // in ms
                    break;

                default:
		            return false;

            }

            // Calculate CRC for the frame.
            unchecked
            {
                crc = Util.CalculateCrc(buff, 0, buffLen);
                buff[buffLen++] = (byte)crc;
                buff[buffLen++] = (byte)(crc >> 8);
            }

            // SOH: Start of header
            bootState[deviceIdx].TxPacket[bootState[deviceIdx].TxPacketLen++] = (byte)SpecialByte.SOH;

            // Form TxPacket. Insert DLE in the data field whereever SOH and EOT are present.
            for (int i = 0; i < buffLen; i++)
            {
                if ((buff[i] == (byte)SpecialByte.EOT) || (buff[i] == (byte)SpecialByte.SOH)
                        || (buff[i] == (byte)SpecialByte.DLE))
                {
                    bootState[deviceIdx].TxPacket[bootState[deviceIdx].TxPacketLen++] = (byte)SpecialByte.DLE;
                }
                bootState[deviceIdx].TxPacket[bootState[deviceIdx].TxPacketLen++] = buff[i];
            }

            // EOT: End of transmission
            bootState[deviceIdx].TxPacket[bootState[deviceIdx].TxPacketLen++] = (byte)SpecialByte.EOT;
            bootState[deviceIdx].txState = TxStates.FIRST_TRY;

            return true;

        }

        /// <summary>
        /// Gets the progress of each command. This function can be used for progress bar.
        /// </summary>
        /// <param name="currentProgress">Current value of progress.</param>
        /// <param name="maxProgress">Progress value for completed job.</param>
        void UpdateProgress(int deviceIdx)
        {
            bootState[deviceIdx].bootloaderStatus.Length = 0;
            switch (bootState[deviceIdx].lastCommandSent)
            {
                case BootloaderCmd.READ_BOOT_INFO:
                case BootloaderCmd.ERASE_FLASH:
                case BootloaderCmd.READ_CRC:
                case BootloaderCmd.JMP_TO_APP:
                    // Progress with respect to retry count.
                    bootloaderTaskWorker.ReportProgress((int)((1 - bootState[deviceIdx].RetryCount / bootState[deviceIdx].MaxRetry) * 100), bootState[deviceIdx].bootloaderStatus);
                    break;

                case BootloaderCmd.PROGRAM_FLASH:
                    // Progress with respect to line counts in hex file.
                    bootloaderTaskWorker.ReportProgress((int)(((float)hexManager.HexCurrLineNo(deviceIdx) / (float)hexManager.HexTotalLines) * 100f), bootState[deviceIdx].bootloaderStatus);
                    break;
            }
        }

        /// <summary>
        /// Gets the locally calculated CRC
        /// </summary>
        /// <returns></returns>
        UInt16 CalculateFlashCRC(int deviceIdx)
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
        void WritePort(int deviceIdx, byte[] buffer, UInt32 bufflen)
        {
            uint index = 0;
            while (bufflen - index > 0)
            {
                byte[] usbBuff = new byte[64];
                uint sendLen = (bufflen - index) > 64 ? 64 : bufflen - index;
                Util.CopyArray(ref usbBuff, 0, ref buffer, index, (int)sendLen);
                usbDriver.WriteUSBDevice(usbDriver.GetDevice(deviceIdx), usbBuff, sendLen); // TODO - multi device support
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
        UInt32 ReadPort(int deviceIdx, ref byte[] buffer, UInt32 bufflen)
        {
            return usbDriver.ReadUSBDevice(usbDriver.GetDevice(deviceIdx), ref buffer, bufflen); // TODO - multi device support
        }

    }
}
