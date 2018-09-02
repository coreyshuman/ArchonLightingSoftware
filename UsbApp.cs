using System;
using System.Collections.Generic;
using System.Threading;
using System.ComponentModel;
using System.Text;

namespace ArchonLightingSystem
{
    class AppData
    {
        public uint FanSpeed;
        public bool EepromReadPending;
        public bool EepromReadDone;
        public bool EepromWritePending;
        public bool ReadConfigPending;
        public bool WriteConfigPending;
        public bool ReadDebug;
        public uint EepromAddress;
        public uint EepromLength;
        public Byte[] EepromData;
        public DeviceConfig deviceConfig;

        public AppData()
        {
            FanSpeed = 0;
            EepromReadPending = false;
            EepromReadDone = false;
            EepromWritePending = false;
            ReadDebug = false;
            EepromAddress = 1;
            EepromLength = 1;
            EepromData = new byte[256];
            deviceConfig = new DeviceConfig();
        }
    }

    class ControlPacket
    {
        public UsbApp.CONTROL_CMD Cmd;
        public UInt16 Len;
        public Byte[] Data;

        public ControlPacket()
        {
            Cmd = UsbApp.CONTROL_CMD.CMD_ERROR_OCCURED;
            Len = 0;
            Data = new byte[UsbApp.CONTROL_BUFFER_SIZE];
        }
    }

    class FrameInfo
    {
        public Byte[] FrameData;
        public uint FrameLen;
        public uint OutBufferMaxLen;
        public bool Multiframe;
        public bool FrameValid;

        public FrameInfo()
        {
            FrameData = new byte[UsbApp.USB_PACKET_SIZE];
            FrameLen = 0;
            OutBufferMaxLen = 512;
            Multiframe = false;
            FrameValid = false;
        }
    }

    partial class UsbApp : UsbDriver
    {
        public AppData Data;
        //Variables used by the application/form updates.
        public bool PushbuttonPressed = false;     //Updated by ReadWriteThread, read by FormUpdateTimer tick handler (needs to be atomic)
        public bool ToggleLEDsPending = false;     //Updated by ToggleLED(s) button click event handler, used by ReadWriteThread (needs to be atomic)
        //public uint FanSpeed = 0;			//Updated by ReadWriteThread, read by FormUpdateTimer tick handler (needs to be atomic)
        private BackgroundWorker ReadWriteThread;

        public UsbApp()
        {
            Data = new AppData();
            //Recommend performing USB read/write operations in a separate thread.  Otherwise,
            //the Read/Write operations are effectively blocking functions and can lock up the
            //user interface if the I/O operations take a long time to complete.
            ReadWriteThread = new BackgroundWorker();
            ReadWriteThread.WorkerReportsProgress = true;
            ReadWriteThread.DoWork += new DoWorkEventHandler(ReadWriteThread_DoWork);
            ReadWriteThread.RunWorkerAsync();
        }

        private void ReadWriteThread_DoWork(object sender, DoWorkEventArgs e)
        {
            Byte[] rxtxBuffer = new Byte[CONTROL_BUFFER_SIZE];
            uint byteCnt = 0;
            int i = 0;

            while (true)
            {
                
                try
                {
                    if (IsAttached == true)	//Do not try to use the read/write handles unless the USB device is attached and ready
                    {
                        for(i = 0; i < CONTROL_BUFFER_SIZE; i++)
                        {
                            rxtxBuffer[i] = 0;
                        }
                        /*
                        if(GenerateAndSendFrames(CONTROL_CMD.CMD_READ_FANSPEED, rxtxBuffer, 0) > 0)
                        {
                            ControlPacket response = GetDeviceResponse(CONTROL_CMD.CMD_READ_FANSPEED);
                            if(response != null)
                            {
                                Data.FanSpeed = (uint)(response.Data[1] << 8) + response.Data[0];	//Need to reformat the data from two unsigned chars into one unsigned int.
                            }
                        }
                        */
                        if (Data.EepromReadPending)
                        {
                            Data.EepromReadPending = false;
                            for (i = 0; i < 512; i++)
                            {
                                rxtxBuffer[i] = 0;
                            }
                            rxtxBuffer[0] = (byte)Data.EepromAddress;
                            rxtxBuffer[1] = (byte)Data.EepromLength;

                            if (GenerateAndSendFrames(CONTROL_CMD.CMD_READ_EEPROM, rxtxBuffer, 2) > 0)
                            {
                                ControlPacket response = GetDeviceResponse(CONTROL_CMD.CMD_READ_EEPROM);
                                if (response != null)
                                {
                                    for (i = 0; i < response.Len; i++)
                                    {
                                        Data.EepromData[i] = response.Data[i];
                                    }
                                    Data.EepromReadDone = true;
                                }
                            }
                        }

                        if (Data.EepromWritePending)
                        {
                            Data.EepromWritePending = false;
                            for (i = 0; i < CONTROL_BUFFER_SIZE; i++)
                            {
                                rxtxBuffer[i] = 0;
                            }
                            rxtxBuffer[0] = (byte)Data.EepromAddress;
                            rxtxBuffer[1] = (byte)Data.EepromLength;
                            for (i = 0; i < rxtxBuffer[1]; i++)
                            {
                                rxtxBuffer[i + 2] = Data.EepromData[i];
                            }

                            if (GenerateAndSendFrames(CONTROL_CMD.CMD_WRITE_EEPROM, rxtxBuffer, 2 + Data.EepromLength) > 0)
                            {
                                ControlPacket response = GetDeviceResponse(CONTROL_CMD.CMD_WRITE_EEPROM);
                                if (response != null)
                                {
                                    Console.WriteLine(response.Data[0]);
                                }
                            }
                        }

                        if (Data.ReadConfigPending)
                        {
                            Data.ReadConfigPending = false;
                            for (i = 0; i < CONTROL_BUFFER_SIZE; i++)
                            {
                                rxtxBuffer[i] = 0;
                            }

                            if (GenerateAndSendFrames(CONTROL_CMD.CMD_READ_CONFIG, rxtxBuffer, 0) > 0)
                            {
                                ControlPacket response = GetDeviceResponse(CONTROL_CMD.CMD_READ_CONFIG);
                                if (response != null)
                                {
                                    Data.deviceConfig.FromBuffer(response.Data);
                                    Data.EepromReadDone = true;
                                }
                            }
                        }

                        if (Data.WriteConfigPending)
                        {
                            Data.WriteConfigPending = false;
                            byteCnt = Data.deviceConfig.ToBuffer(rxtxBuffer);
                            if (GenerateAndSendFrames(CONTROL_CMD.CMD_WRITE_CONFIG, rxtxBuffer, byteCnt) > 0)
                            {
                                ControlPacket response = GetDeviceResponse(CONTROL_CMD.CMD_WRITE_CONFIG);
                                if (response != null)
                                {
                                    
                                }
                            }
                        }

                        if (Data.ReadDebug)
                        {
                            Data.ReadDebug = false;
                            for (i = 0; i < CONTROL_BUFFER_SIZE; i++)
                            {
                                rxtxBuffer[i] = 0;
                            }

                            if (GenerateAndSendFrames(CONTROL_CMD.CMD_READ_EE_DEBUG, rxtxBuffer, 0) > 0)
                            {
                                ControlPacket response = GetDeviceResponse(CONTROL_CMD.CMD_READ_EE_DEBUG);
                                if (response != null)
                                {
                                    System.Diagnostics.Debug.WriteLine($"Debug Len: {response.Len}");
                                    for (i = 0; i < response.Len; )
                                    {
                                        var dat = response.Data[i];
                                        System.Diagnostics.Debug.Write($"{dat.ToString("X2")} ");
                                        if(++i % 20 == 0)
                                        {
                                            System.Diagnostics.Debug.Write("\r\n");
                                        }
                                    }
                                    System.Diagnostics.Debug.WriteLine("\r\n_________________________");
                                }
                            }
                        }


                    } 
                    else
                    {
                        Thread.Sleep(5);    //Add a small delay.  Otherwise, this while(true) loop can execute very fast and cause 
                                            //high CPU utilization, with no particular benefit to the application.
                    }
                }
                catch(Exception exc)
                {
                    //Exceptions can occur during the read or write operations.  For example,
                    //exceptions may occur if for instance the USB device is physically unplugged
                    //from the host while the above read/write functions are executing.

                    //Don't need to do anything special in this case.  The application will automatically
                    //re-establish communications based on the global IsAttached boolean variable used
                    //in conjunction with the WM_DEVICECHANGE messages to dyanmically respond to Plug and Play
                    //USB connection events.
                    exc = exc;
                }

            }

        }

        private ControlPacket GetDeviceResponse(CONTROL_CMD cmd)
        {
            uint byteCnt;
            FrameInfo frameInfo = new FrameInfo();
            ControlPacket controlPacket = new ControlPacket();
            frameInfo.OutBufferMaxLen = 512;
            while (!frameInfo.FrameValid)
            {
                byteCnt = ReadUSBDevice(ref frameInfo.FrameData, USB_PACKET_SIZE);
                if (byteCnt > 0)
                {
                    frameInfo.FrameLen = byteCnt;
                    byteCnt = ValidateFrame(frameInfo, controlPacket);
                    if (byteCnt == 0)
                    {
                        return null;
                    }
                    else if (frameInfo.FrameValid)
                    {
                        if(controlPacket.Cmd == CONTROL_CMD.CMD_ERROR_OCCURED)
                        {
                            // do error handling here
                            throw new Exception("Error Occured");
                        }
                        return controlPacket.Cmd == cmd ? controlPacket : null;
                    }
                }
                else
                {
                    return null;
                }
            }
            return null;
        }

        private uint GenerateAndSendFrames(CONTROL_CMD cmd, Byte[] frameData, uint frameLen)
        {
            uint byteCnt;
            uint bytesSent = 0;
            Byte[] usbBuffer = new byte[USB_PACKET_SIZE];
            Byte[] packetsBuffer = new byte[USB_BUFFER_SIZE];

            byteCnt = GenerateFrames(cmd, frameData, frameLen, ref packetsBuffer, USB_BUFFER_SIZE);
            while (byteCnt > 0)
            {
                uint sendLen = byteCnt > 64 ? 64 : byteCnt;
                for (int i = 0; i < sendLen; i++)
                {
                    usbBuffer[i] = packetsBuffer[i + bytesSent];
                }
                if (WriteUSBDevice(usbBuffer, sendLen) == 0)
                {
                    return 0;
                }
                byteCnt -= sendLen;
                bytesSent += sendLen;
            }

            return bytesSent;
        }

        private uint GenerateFrames(CONTROL_CMD cmd, Byte[] frameData, uint dataLen, ref Byte[] outBuffer, uint outBufferMaxLen)
        {
            uint outBufferLen = 0;
            uint packetDataCount = 0;
            uint frameDataCount = 0;
            UInt16 crc = 0;

            do
            {
                if (outBufferLen >= outBufferMaxLen)
                {
                    throw new Exception("Frame Data Overflow.");
                }
                outBuffer[outBufferLen++] = (byte)cmd;
                // if greater than 60 bytes, flag for multibyte
                outBuffer[outBufferLen++] = (byte)((dataLen > 60 ? (60 | 0x80) : dataLen) + 2);
                packetDataCount = 0;
                while (dataLen > 0 && packetDataCount < 60)
                {
                    outBuffer[outBufferLen++] = frameData[frameDataCount++];
                    dataLen--;
                    packetDataCount++;
                }

                // Add CRC
                crc = CalculateCrc(outBuffer, outBufferLen - packetDataCount - 2, (uint)(outBuffer[outBufferLen - packetDataCount - 1] & 0x3F));
                outBuffer[outBufferLen++] = (byte)(crc & 0xFF);
                outBuffer[outBufferLen++] = (byte)((crc >> 8) & 0xFF);
            }
            while (dataLen > 0);

            // pad 0xFF to 64 byte boundary since we always send 64 bytes per packet
            packetDataCount = outBufferLen % 64;
            while (packetDataCount-- > 0)
            {
                outBuffer[outBufferLen++] = 0xFF;
            }

            return (outBufferLen); // Return buffer length.
        }

        

        private uint ValidateFrame(FrameInfo frameInfo, ControlPacket controlPacket)
        {
            UInt16 crc;
            uint dataLen = 0;
            uint frameIdx = 0;

            bool multipacket = (frameInfo.FrameData[2] & 0x80) == 0x80; // check multipacket flag
            
            dataLen = (uint)(frameInfo.FrameData[2] & 0x3F);

            if(dataLen > 62)
            {
                throw new Exception("Invalid packet length.");
            }

            if (frameInfo.Multiframe) {
                if(controlPacket.Cmd != (CONTROL_CMD)frameInfo.FrameData[1])
                {
                    throw new Exception("Invalid multipacket.");
                }
            }
            else
            {
                controlPacket.Cmd = (CONTROL_CMD)frameInfo.FrameData[1];
            }

            frameInfo.Multiframe |= multipacket;
            crc = CalculateCrc(frameInfo.FrameData, 1, dataLen);

            if (crc != ((UInt16)frameInfo.FrameData[dataLen+1] | (UInt16)(frameInfo.FrameData[dataLen+2] << 8)))
            {
                return 0;
            }

            // skip Report ID, CMD and LEN
            frameIdx = 3;

            while (dataLen > 2)
            {
                dataLen--;

                controlPacket.Data[controlPacket.Len++] = frameInfo.FrameData[frameIdx++];
            }

            if (!multipacket)
            {
                frameInfo.FrameValid = true;
            }
           
            return controlPacket.Len;
        }

        private static UInt16[] crc_table = new UInt16[]
        {
            0x0000, 0x1021, 0x2042, 0x3063, 0x4084, 0x50a5, 0x60c6, 0x70e7,
            0x8108, 0x9129, 0xa14a, 0xb16b, 0xc18c, 0xd1ad, 0xe1ce, 0xf1ef
        };

        /// <summary>
        /// Calculate CRC for the given data and len
        /// </summary>
        /// <param name="data">data pointer</param>
        /// <param name="start">start location in array</param>
        /// <param name="len">data length</param>
        /// <returns></returns>
        UInt16 CalculateCrc(Byte[] data, uint start, uint len)
        {
            UInt16 i;
            UInt16 crc = 0;
            uint idx = start;

            while (len-- > 0)
            {
                i = (UInt16)((crc >> 12) ^ (data[idx] >> 4));
                crc = (UInt16)(crc_table[i & 0x0F] ^ (crc << 4));
                i = (UInt16)((crc >> 12) ^ (data[idx] >> 0));
                crc = (UInt16)(crc_table[i & 0x0F] ^ (crc << 4));
                idx++;
            }

            return (UInt16)(crc & 0xFFFF);
        }

    }
}
