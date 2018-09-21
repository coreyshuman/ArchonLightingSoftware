using System;
using System.Threading;
using System.ComponentModel;
using ArchonLightingSystem.Models;
using ArchonLightingSystem.Common;

namespace ArchonLightingSystem.UsbApplication
{
    public partial class UsbApp : UsbDriver
    {
        public ApplicationData AppData { get; set; }
        public bool PauseUsb { get; set; }
        private BackgroundWorker ReadWriteThread;


        public UsbApp()
        {
            AppData = new ApplicationData();
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
                    if (IsAttached == true && !PauseUsb)	//Do not try to use the read/write handles unless the USB device is attached and ready
                    {
                        if(AppData.DeviceControllerData == null)
                        {
                            AppData.DeviceControllerData = new DeviceControllerData();
                        }
                        if(!AppData.DeviceControllerData.IsInitialized)
                        {
                            GetDeviceInitialization(AppData.DeviceControllerData);
                        }

                        for(i = 0; i < CONTROL_BUFFER_SIZE; i++)
                        {
                            rxtxBuffer[i] = 0;
                        }
                        
                        
                        if(GenerateAndSendFrames(CONTROL_CMD.CMD_READ_FANSPEED, rxtxBuffer, 0) > 0)
                        {
                            Thread.Sleep(2);
                            ControlPacket response = GetDeviceResponse(CONTROL_CMD.CMD_READ_FANSPEED);
                            if(response != null)
                            {
                                for (i = 0; i < DeviceControllerDefinitions.DeviceCount; i++)
                                {
                                    //AppData.FanSpeed[i] = (uint)(response.Data[0 + i*2] + (response.Data[1 + i * 2] << 8));
                                    AppData.DeviceControllerData.MeasuredFanRpm[i] = (UInt16)(response.Data[0 + i * 2] + (response.Data[1 + i * 2] << 8));
                                }
                            }
                        }
                        
                        

                        if(AppData.ResetToBootloaderPending)
                        {
                            AppData.ResetToBootloaderPending = false;
                            ResetDeviceToBootloader();
                            PauseUsb = true;
                        }
                        
                        if (AppData.EepromReadPending)
                        {
                            AppData.EepromReadPending = false;

                            ControlPacket response = ReadEeprom((byte)AppData.EepromAddress, (byte)AppData.EepromLength);
                            AppData.DeviceControllerData.UpdateEepromData(response.Data);
                            AppData.EepromReadDone = true;
                        }

                        if (AppData.EepromWritePending)
                        {
                            AppData.EepromWritePending = false;
                            for (i = 0; i < CONTROL_BUFFER_SIZE; i++)
                            {
                                rxtxBuffer[i] = 0;
                            }
                            rxtxBuffer[0] = (byte)AppData.EepromAddress;
                            rxtxBuffer[1] = (byte)AppData.EepromLength;
                            for (i = 0; i < AppData.EepromLength; i++)
                            {
                                rxtxBuffer[i + 2] = AppData.DeviceControllerData.EepromData[i];
                            }

                            if (GenerateAndSendFrames(CONTROL_CMD.CMD_WRITE_EEPROM, rxtxBuffer, 2 + AppData.EepromLength) > 0)
                            {
                                ControlPacket response = GetDeviceResponse(CONTROL_CMD.CMD_WRITE_EEPROM);
                                if (response != null)
                                {
                                    Console.WriteLine(response.Data[0]);
                                }
                            }
                        }

                        if (AppData.ReadConfigPending)
                        {
                            AppData.ReadConfigPending = false;

                             ControlPacket response = ReadConfig();
                            if (response != null)
                            {
                                AppData.DeviceControllerData.DeviceConfig.FromBuffer(response.Data);
                                AppData.EepromReadDone = true;
                            }
                        }

                        if (AppData.DefaultConfigPending)
                        {
                            AppData.DefaultConfigPending = false;

                            ControlPacket response = DefaultConfig();
                            if (response != null)
                            {
                                AppData.ReadConfigPending = true;
                            }
                        }

                        if (AppData.UpdateConfigPending)
                        {
                            AppData.UpdateConfigPending = false;
                            UpdateConfig(AppData.DeviceControllerData.DeviceConfig);
                        }

                        if (AppData.WriteConfigPending)
                        {
                            AppData.WriteConfigPending = false;
                            byteCnt = 1;// AppData.DeviceConfig.ToBuffer(ref rxtxBuffer);
                            if (GenerateAndSendFrames(CONTROL_CMD.CMD_WRITE_CONFIG, rxtxBuffer, byteCnt) > 0)
                            {
                                ControlPacket response = GetDeviceResponse(CONTROL_CMD.CMD_WRITE_CONFIG);
                                if (response != null)
                                {
                                    
                                }
                            }
                        }

                        if (AppData.ReadDebugPending)
                        {
                            AppData.ReadDebugPending = false;
                            for (i = 0; i < CONTROL_BUFFER_SIZE; i++)
                            {
                                rxtxBuffer[i] = 0;
                            }

                            if (GenerateAndSendFrames(CONTROL_CMD.CMD_READ_EE_DEBUG, rxtxBuffer, 0) > 0)
                            {
                                ControlPacket response = GetDeviceResponse(CONTROL_CMD.CMD_READ_EE_DEBUG);
                                if (response != null)
                                {
                                    AppData.Debug = ($"Debug Len: {response.Len}\r\n");
                                    for (i = 0; i < response.Len; )
                                    {
                                        var dat = response.Data[i];
                                        AppData.Debug += ($"{dat.ToString("X2")} ");
                                        if(++i % 20 == 0)
                                        {
                                            AppData.Debug += ("\r\n");
                                        }
                                    }
                                    AppData.Debug += ("\r\n_________________________");
                                }
                            }
                        }


                    } 
                    else
                    {
                        Thread.Sleep(10);    //Add a small delay.  Otherwise, this while(true) loop can execute very fast and cause 
                                            //high CPU utilization, with no particular benefit to the application.
                    }
                    if(!IsAttached && AppData.DeviceControllerData.IsInitialized)
                    {
                        AppData.DeviceControllerData = new DeviceControllerData();
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

        private void GetDeviceInitialization(DeviceControllerData deviceData)
        {
            int i;
            ControlPacket bootResponse = new ControlPacket();
            ControlPacket appResponse = new ControlPacket();
            ControlPacket bootStatusResponse;
            ControlPacket eepromResponse;
            ControlPacket deviceAddressResponse;
            ControlPacket deviceConfigResponse;

            
            bootResponse = ReadBootloaderInfo();
            if (bootResponse == null) throw new Exception("Couldn't read Bootloader info.");
            appResponse = ReadApplicationInfo();
            if (appResponse == null) throw new Exception("Couldn't read Application info.");
            
            deviceAddressResponse = ReadControllerAddress();
            if (deviceAddressResponse == null) throw new Exception("Couldn't read Address.");
            bootStatusResponse = ReadBootloaderInfo();
            if (bootStatusResponse == null) throw new Exception("Couldn't read boot status.");
            eepromResponse = ReadEeprom(0, (UInt16)DeviceControllerDefinitions.EepromSize-1); // cts debug, see todo below
            if (eepromResponse == null) throw new Exception("Couldn't read EEPROM.");
            deviceConfigResponse = ReadConfig();
            if (deviceConfigResponse == null) throw new Exception("Couldn't read Config.");

            deviceData.InitializeDevice(deviceAddressResponse.Data[0], eepromResponse.Data, deviceConfigResponse.Data, bootResponse.Data, appResponse.Data, bootStatusResponse.Data);
        }

        private ControlPacket ReadBootloaderInfo()
        {
            if (GenerateAndSendFrames(CONTROL_CMD.CMD_READ_BOOTLOADER_INFO, null, 0) > 0)
            {
                Thread.Sleep(2);
                ControlPacket response = GetDeviceResponse(CONTROL_CMD.CMD_READ_BOOTLOADER_INFO);
                return response;
            }
            return null;
        }

        private ControlPacket ReadApplicationInfo()
        {
            if (GenerateAndSendFrames(CONTROL_CMD.CMD_READ_FIRMWARE_INFO, null, 0) > 0)
            {
                Thread.Sleep(2);
                ControlPacket response = GetDeviceResponse(CONTROL_CMD.CMD_READ_FIRMWARE_INFO);
                return response;
            }
            return null;
        }

        private bool ResetDeviceToBootloader()
        {
            return GenerateAndSendFrames(CONTROL_CMD.CMD_RESET_TO_BOOTLOADER, null, 0) > 0;
        }

        private ControlPacket ReadBootStatus()
        {
            if (GenerateAndSendFrames(CONTROL_CMD.CMD_READ_BOOT_STATUS, null, 0) > 0)
            {
                Thread.Sleep(2);
                ControlPacket response = GetDeviceResponse(CONTROL_CMD.CMD_READ_BOOT_STATUS);
                return response;
            }
            return null;
        }


        private ControlPacket ReadEeprom(UInt16 address, UInt16 length)
        {
            // todo - need to make length a word to read all 256 bytes of the eeprom
            byte[] request = new byte[] { (byte)address, (byte)length };
            if (GenerateAndSendFrames(CONTROL_CMD.CMD_READ_EEPROM, request, 2) > 0)
            {
                Thread.Sleep(100); // give controller time to read EEPROM
                ControlPacket response = GetDeviceResponse(CONTROL_CMD.CMD_READ_EEPROM, 2000);
                return response;
            }
            return null;
        }

        private ControlPacket DefaultConfig()
        {
            if (GenerateAndSendFrames(CONTROL_CMD.CMD_DEFAULT_CONFIG, null, 0) > 0)
            {
                Thread.Sleep(2);
                ControlPacket response = GetDeviceResponse(CONTROL_CMD.CMD_DEFAULT_CONFIG);
                return response;
            }
            return null;
        }

        private ControlPacket ReadConfig()
        {
            if (GenerateAndSendFrames(CONTROL_CMD.CMD_READ_CONFIG, null, 0) > 0)
            {
                Thread.Sleep(50); // larger packet
                ControlPacket response = GetDeviceResponse(CONTROL_CMD.CMD_READ_CONFIG);
                return response;
            }
            return null;
        }

        private ControlPacket WriteConfig()
        {
            if (GenerateAndSendFrames(CONTROL_CMD.CMD_WRITE_CONFIG, null, 0) > 0)
            {
                Thread.Sleep(100); // writing to eeprom
                ControlPacket response = GetDeviceResponse(CONTROL_CMD.CMD_WRITE_CONFIG);
                return response;
            }
            return null;
        }

        private ControlPacket UpdateConfig(DeviceControllerConfig config)
        {
            Byte[] buffer = new byte[DeviceControllerDefinitions.EepromSize];
            uint length = config.ToBuffer(ref buffer);
            if (GenerateAndSendFrames(CONTROL_CMD.CMD_UPDATE_CONFIG, buffer, length) > 0)
            {
                ControlPacket response = GetDeviceResponse(CONTROL_CMD.CMD_UPDATE_CONFIG);
                return response;
            }
            return null;
        }

        private ControlPacket ReadControllerAddress()
        {
            if (GenerateAndSendFrames(CONTROL_CMD.CMD_READ_CONTROLLER_ADDRESS, null, 0) > 0)
            {
                ControlPacket response = GetDeviceResponse(CONTROL_CMD.CMD_READ_CONTROLLER_ADDRESS);
                return response;
            }
            return null;
        }

        private ControlPacket GetDeviceResponse(CONTROL_CMD cmd, uint readTimeout = 200)
        {
            uint byteCnt;
            uint frameCnt = 0;
            FrameInfo frameInfo = new FrameInfo();
            ControlPacket controlPacket = new ControlPacket();
            frameInfo.OutBufferMaxLen = 512;

            while (!frameInfo.FrameValid)
            {
                byteCnt = ReadUSBDevice(ref frameInfo.FrameData, USB_PACKET_SIZE, readTimeout);
                if (byteCnt > 0)
                {
                    frameCnt++;
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
                crc = Util.CalculateCrc(outBuffer, outBufferLen - packetDataCount - 2, (uint)(outBuffer[outBufferLen - packetDataCount - 1] & 0x3F));
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
            crc = Util.CalculateCrc(frameInfo.FrameData, 1, dataLen);

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

        

    }
}
