using System;
using System.Threading;
using System.ComponentModel;
using ArchonLightingSystem.Models;
using ArchonLightingSystem.Common;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;

namespace ArchonLightingSystem.UsbApplication
{
    public static partial class UsbApp
    {
        public static UsbDriver usbDriver { get; } = new UsbDriver();


        public static void Register(IntPtr handle, string vid, string pid)
        {
            usbDriver.RegisterEventHandler(handle);
            usbDriver.InitializeDevice(vid, pid);
        }

        public static void HandleWindowEvent(ref System.Windows.Forms.Message m)
        {
            usbDriver.HandleWindowEvent(ref m);
        }

        public static async Task DeviceDoWork(UsbControllerInstance controllerInstance)
        {
            Byte[] rxtxBuffer = new Byte[UsbAppCommon.CONTROL_BUFFER_SIZE];
            uint byteCnt = 0;
            int i = 0;

            if (await controllerInstance.semaphore.WaitAsync(200))
            {
                try
                {
                    if(controllerInstance.IsDisconnected)
                    {
                        return;
                    }

                    if (!controllerInstance.AppIsInitialized)
                    {
                        Logger.Write(Level.Trace, "Initialize ApplicationData");
                        controllerInstance.AppData = new ApplicationData();
                        controllerInstance.AppIsInitialized = true;
                    }

                    if (!controllerInstance.UsbDevice.IsAttached && controllerInstance.AppData.DeviceControllerData.IsInitialized)
                    {
                        Logger.Write(Level.Trace, "Initialize DeviceControllerData");
                        controllerInstance.AppData.DeviceControllerData = new DeviceControllerData();
                    }

                    if (controllerInstance.UsbDevice.IsAttached == true)    //Do not try to use the read/write handles unless the USB controllerInstance is attached and ready
                    {
                        if (controllerInstance.AppData.DeviceControllerData == null)
                        {
                            Logger.Write(Level.Trace, "Reinitialize ApplicationData");
                            controllerInstance.AppData.DeviceControllerData = new DeviceControllerData();
                        }
                        if (!controllerInstance.AppData.DeviceControllerData.IsInitialized)
                        {
                            Logger.Write(Level.Trace, "Get Device Initialization");
                            await GetDeviceInitialization(controllerInstance);
                        }

                        for (i = 0; i < UsbAppCommon.CONTROL_BUFFER_SIZE; i++)
                        {
                            rxtxBuffer[i] = 0;
                        }

                        // stop general tasks when paused
                        if (!controllerInstance.IsPaused)
                        {
                            for (i = 0; i < DeviceControllerDefinitions.DevicePerController; i++)
                            {
                                rxtxBuffer[i] = controllerInstance.AppData.DeviceControllerData.TemperatureValue[i];
                            }
                            if (GenerateAndSendFrames(controllerInstance, UsbAppCommon.CONTROL_CMD.CMD_READ_FANSPEED, rxtxBuffer, DeviceControllerDefinitions.DevicePerController) > 0)
                            {
                                //await Task.Delay(2);
                                ControlPacket response = GetDeviceResponse(controllerInstance, UsbAppCommon.CONTROL_CMD.CMD_READ_FANSPEED);
                                if (response != null)
                                {
                                    for (i = 0; i < DeviceControllerDefinitions.DevicePerController; i++)
                                    {
                                        //AppData.FanSpeed[i] = (uint)(response.Data[0 + i*2] + (response.Data[1 + i * 2] << 8));
                                        controllerInstance.AppData.DeviceControllerData.MeasuredFanRpm[i] = (UInt16)(response.Data[0 + i * 2] + (response.Data[1 + i * 2] << 8));
                                    }
                                }
                            }

                            if(controllerInstance.AppData.UpdateFanSpeedPending)
                            {
                                await WriteFanSpeed(controllerInstance, controllerInstance.AppData.DeviceControllerData.AutoFanSpeedValue);
                                controllerInstance.AppData.UpdateFanSpeedPending = false;
                            }
                        }

                        if (controllerInstance.AppData.ResetToBootloaderPending)
                        {
                            controllerInstance.AppData.ResetToBootloaderPending = false;
                            ResetDeviceToBootloader(controllerInstance);
                            controllerInstance.Disconnect();
                        }

                        if (controllerInstance.AppData.EepromReadPending)
                        {
                            controllerInstance.AppData.EepromReadPending = false;

                            ControlPacket response = await ReadEeprom(controllerInstance, (byte)controllerInstance.AppData.EepromAddress, (byte)controllerInstance.AppData.EepromLength);
                            controllerInstance.AppData.DeviceControllerData.UpdateEepromData(response.Data, response.Len);
                            controllerInstance.AppData.EepromReadDone = true;
                        }

                        if (controllerInstance.AppData.EepromWritePending)
                        {
                            controllerInstance.AppData.EepromWritePending = false;
                            for (i = 0; i < UsbAppCommon.CONTROL_BUFFER_SIZE; i++)
                            {
                                rxtxBuffer[i] = 0;
                            }
                            rxtxBuffer[0] = (byte)controllerInstance.AppData.EepromAddress;
                            rxtxBuffer[1] = (byte)controllerInstance.AppData.EepromLength;
                            for (i = 0; i < controllerInstance.AppData.EepromLength; i++)
                            {
                                rxtxBuffer[i + 2] = controllerInstance.AppData.DeviceControllerData.EepromData[i];
                            }

                            if (GenerateAndSendFrames(controllerInstance, UsbAppCommon.CONTROL_CMD.CMD_WRITE_EEPROM, rxtxBuffer, 2 + controllerInstance.AppData.EepromLength) > 0)
                            {
                                ControlPacket response = GetDeviceResponse(controllerInstance, UsbAppCommon.CONTROL_CMD.CMD_WRITE_EEPROM);
                                if (response != null)
                                {
                                    Console.WriteLine(response.Data[0]);
                                }
                            }
                        }

                        if (controllerInstance.AppData.ReadConfigPending)
                        {
                            controllerInstance.AppData.ReadConfigPending = false;

                            ControlPacket response = await ReadConfig(controllerInstance);
                            if (response != null)
                            {
                                controllerInstance.AppData.DeviceControllerData.DeviceConfig.FromBuffer(response.Data);
                                controllerInstance.AppData.EepromReadDone = true;
                            }
                        }

                        if (controllerInstance.AppData.DefaultConfigPending)
                        {
                            controllerInstance.AppData.DefaultConfigPending = false;

                            ControlPacket response = await DefaultConfig(controllerInstance);
                            if (response != null)
                            {
                                controllerInstance.AppData.ReadConfigPending = true;
                            }
                        }

                        if (controllerInstance.AppData.UpdateConfigPending)
                        {
                            controllerInstance.AppData.UpdateConfigPending = false;
                            await UpdateConfig(controllerInstance, controllerInstance.AppData.DeviceControllerData.DeviceConfig);
                        }

                        if (controllerInstance.AppData.WriteConfigPending)
                        {
                            controllerInstance.AppData.WriteConfigPending = false;
                            byteCnt = 1;// AppData.DeviceConfig.ToBuffer(ref rxtxBuffer);
                            if (GenerateAndSendFrames(controllerInstance, UsbAppCommon.CONTROL_CMD.CMD_WRITE_CONFIG, rxtxBuffer, byteCnt) > 0)
                            {
                                ControlPacket response = GetDeviceResponse(controllerInstance, UsbAppCommon.CONTROL_CMD.CMD_WRITE_CONFIG);
                                if (response != null)
                                {

                                }
                            }
                        }

                        if(controllerInstance.AppData.WriteLedFrame)
                        {
                            controllerInstance.AppData.WriteLedFrame = false;
                            await WriteLedFrame(controllerInstance, controllerInstance.AppData.LedFrameData);
                        }

                        if (controllerInstance.AppData.ReadDebugPending)
                        {
                            controllerInstance.AppData.ReadDebugPending = false;
                            await ReadDebug(controllerInstance);
                        }

                        if (controllerInstance.AppData.SendTimePending)
                        {
                            controllerInstance.AppData.SendTimePending = false;
                            await SetTime(controllerInstance, controllerInstance.AppData.TimeValue);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Write(Level.Error, $"DeviceDoWork Error: {ex.ToString()}");
                    throw ex;
                }
                finally
                {
                    controllerInstance.semaphore.Release();
                }
            }
        }

        private static async Task GetDeviceInitialization(UsbControllerInstance controllerInstance)
        {
            ControlPacket bootResponse = new ControlPacket();
            ControlPacket appResponse = new ControlPacket();
            ControlPacket bootStatusResponse;
            ControlPacket eepromResponse;
            ControlPacket controllerInstanceAddressResponse;
            ControlPacket controllerInstanceConfigResponse;

            bootResponse = await ReadBootloaderInfo(controllerInstance);
            if (bootResponse == null) throw new Exception("Couldn't read Bootloader info.");
            Logger.Write(Level.Trace, $"DeviceInit Boot: {bootResponse.Data[0].ToString()}.{bootResponse.Data[1]}");
            appResponse = await ReadApplicationInfo(controllerInstance);
            if (appResponse == null) throw new Exception("Couldn't read Application info.");
            Logger.Write(Level.Trace, $"DeviceInit App: {appResponse.Data[0].ToString()}.{appResponse.Data[1]}");
            controllerInstanceAddressResponse = await ReadControllerAddress(controllerInstance);
            if (controllerInstanceAddressResponse == null) throw new Exception("Couldn't read Address.");
            Logger.Write(Level.Trace, $"DeviceInit Addr: {controllerInstanceAddressResponse.Data[0]}");
            bootStatusResponse = await ReadBootStatus(controllerInstance);
            if (bootStatusResponse == null) throw new Exception("Couldn't read boot status.");
            eepromResponse = await ReadEeprom(controllerInstance, 0, (UInt16)DeviceControllerDefinitions.EepromSize);
            if (eepromResponse == null) throw new Exception("Couldn't read EEPROM.");
            controllerInstanceConfigResponse = await ReadConfig(controllerInstance);
            if (controllerInstanceConfigResponse == null) throw new Exception("Couldn't read Config.");

            controllerInstance.AppData.DeviceControllerData.InitializeDevice(controllerInstanceAddressResponse.Data[0], eepromResponse.Data, controllerInstanceConfigResponse.Data, bootResponse.Data, appResponse.Data, bootStatusResponse.Data);
        }

        private static async Task<ControlPacket> ReadBootloaderInfo(UsbControllerInstance controllerInstance)
        {
            if (GenerateAndSendFrames(controllerInstance, UsbAppCommon.CONTROL_CMD.CMD_READ_BOOTLOADER_INFO, null, 0) > 0)
            {
                //await Task.Delay(2);
                ControlPacket response = GetDeviceResponse(controllerInstance, UsbAppCommon.CONTROL_CMD.CMD_READ_BOOTLOADER_INFO);
                return response;
            }
            return null;
        }

        private static async Task<ControlPacket> ReadApplicationInfo(UsbControllerInstance controllerInstance)
        {
            if (GenerateAndSendFrames(controllerInstance, UsbAppCommon.CONTROL_CMD.CMD_READ_FIRMWARE_INFO, null, 0) > 0)
            {
                //await Task.Delay(2);
                ControlPacket response = GetDeviceResponse(controllerInstance, UsbAppCommon.CONTROL_CMD.CMD_READ_FIRMWARE_INFO);
                return response;
            }
            return null;
        }

        private static async Task<ControlPacket> WriteFanSpeed(UsbControllerInstance controllerInstance, byte[] speedValues)
        {
            if (GenerateAndSendFrames(controllerInstance, UsbAppCommon.CONTROL_CMD.CMD_WRITE_FANSPEED, speedValues, DeviceControllerDefinitions.DevicePerController) > 0)
            {
                ControlPacket response = GetDeviceResponse(controllerInstance, UsbAppCommon.CONTROL_CMD.CMD_WRITE_FANSPEED, 20);
                return response;
            }
            return null;
        }

        private static bool ResetDeviceToBootloader(UsbControllerInstance controllerInstance)
        {
            return GenerateAndSendFrames(controllerInstance, UsbAppCommon.CONTROL_CMD.CMD_RESET_TO_BOOTLOADER, null, 0) > 0;
        }

        private static async Task<ControlPacket> ReadBootStatus(UsbControllerInstance controllerInstance)
        {
            if (GenerateAndSendFrames(controllerInstance, UsbAppCommon.CONTROL_CMD.CMD_READ_BOOT_STATUS, null, 0) > 0)
            {
                //await Task.Delay(2);
                ControlPacket response = GetDeviceResponse(controllerInstance, UsbAppCommon.CONTROL_CMD.CMD_READ_BOOT_STATUS);
                return response;
            }
            return null;
        }


        private static async Task<ControlPacket> ReadEeprom(UsbControllerInstance controllerInstance, UInt16 address, UInt16 length)
        {
            byte[] request = new byte[] { (byte)address, (byte)(length & 0xFF), (byte)((length >> 8) & 0xFF) };
            if (GenerateAndSendFrames(controllerInstance, UsbAppCommon.CONTROL_CMD.CMD_READ_EEPROM, request, 3) > 0)
            {
                //await Task.Delay(100); // give controller time to read EEPROM
                ControlPacket response = GetDeviceResponse(controllerInstance, UsbAppCommon.CONTROL_CMD.CMD_READ_EEPROM, 2000);
                return response;
            }
            return null;
        }

        private static async Task<ControlPacket> DefaultConfig(UsbControllerInstance controllerInstance)
        {
            if (GenerateAndSendFrames(controllerInstance, UsbAppCommon.CONTROL_CMD.CMD_DEFAULT_CONFIG, null, 0) > 0)
            {
                //await Task.Delay(2);
                ControlPacket response = GetDeviceResponse(controllerInstance, UsbAppCommon.CONTROL_CMD.CMD_DEFAULT_CONFIG);
                return response;
            }
            return null;
        }

        private static async Task<ControlPacket> ReadConfig(UsbControllerInstance controllerInstance)
        {
            if (GenerateAndSendFrames(controllerInstance, UsbAppCommon.CONTROL_CMD.CMD_READ_CONFIG, null, 0) > 0)
            {
                //await Task.Delay(50); // larger packet
                ControlPacket response = GetDeviceResponse(controllerInstance, UsbAppCommon.CONTROL_CMD.CMD_READ_CONFIG);
                return response;
            }
            return null;
        }

        private static async Task<ControlPacket> WriteConfig(UsbControllerInstance controllerInstance)
        {
            if (GenerateAndSendFrames(controllerInstance, UsbAppCommon.CONTROL_CMD.CMD_WRITE_CONFIG, null, 0) > 0)
            {
                //await Task.Delay(100); // writing to eeprom
                ControlPacket response = GetDeviceResponse(controllerInstance, UsbAppCommon.CONTROL_CMD.CMD_WRITE_CONFIG);
                return response;
            }
            return null;
        }

        private static async Task<ControlPacket> UpdateConfig(UsbControllerInstance controllerInstance, DeviceControllerConfig config)
        {
            Byte[] buffer = new byte[DeviceControllerDefinitions.EepromSize];
            uint length = config.ToBuffer(ref buffer);
            if (GenerateAndSendFrames(controllerInstance, UsbAppCommon.CONTROL_CMD.CMD_UPDATE_CONFIG, buffer, length) > 0)
            {
                //await Task.Delay(50); // larger packet
                ControlPacket response = GetDeviceResponse(controllerInstance, UsbAppCommon.CONTROL_CMD.CMD_UPDATE_CONFIG);
                return response;
            }
            return null;
        }

        private static async Task<ControlPacket> WriteLedFrame(UsbControllerInstance controllerInstance, byte[,] ledFrame)
        {
            Byte[] buffer = new byte[DeviceControllerDefinitions.DevicePerController * DeviceControllerDefinitions.LedBytesPerDevice];
            uint length = DeviceControllerDefinitions.DevicePerController * DeviceControllerDefinitions.LedBytesPerDevice;
            for(int devi = 0; devi < DeviceControllerDefinitions.DevicePerController; devi++)
            {
                for(int ledi = 0; ledi < DeviceControllerDefinitions.LedBytesPerDevice; ledi++)
                {
                    buffer[devi * DeviceControllerDefinitions.LedBytesPerDevice + ledi] = ledFrame[devi, ledi];
                }
            }
            if (GenerateAndSendFrames(controllerInstance, UsbAppCommon.CONTROL_CMD.CMD_WRITE_LED_FRAME, buffer, length) > 0)
            {
                ControlPacket response = GetDeviceResponse(controllerInstance, UsbAppCommon.CONTROL_CMD.CMD_WRITE_LED_FRAME);
                return response;
            }
            return null;
        }

        private static async Task<ControlPacket> ReadControllerAddress(UsbControllerInstance controllerInstance)
        {
            if (GenerateAndSendFrames(controllerInstance, UsbAppCommon.CONTROL_CMD.CMD_READ_CONTROLLER_ADDRESS, null, 0) > 0)
            {
                //await Task.Delay(2); 
                ControlPacket response = GetDeviceResponse(controllerInstance, UsbAppCommon.CONTROL_CMD.CMD_READ_CONTROLLER_ADDRESS);
                return response;
            }
            return null;
        }

        private static async Task<ControlPacket> ReadDebug(UsbControllerInstance controllerInstance)
        {
            if (GenerateAndSendFrames(controllerInstance, UsbAppCommon.CONTROL_CMD.CMD_READ_EE_DEBUG, null, 0) > 0)
            {
                //await Task.Delay(2);
                ControlPacket response = GetDeviceResponse(controllerInstance, UsbAppCommon.CONTROL_CMD.CMD_READ_EE_DEBUG);
                if (response != null)
                {
                    controllerInstance.AppData.Debug = ($"Debug Len: {response.Len}"+Environment.NewLine);
                    for (int i = 0; i < response.Len;)
                    {
                        var dat = response.Data[i];
                        controllerInstance.AppData.Debug += ($"{dat.ToString("X2")} ");
                        if (++i % 20 == 0)
                        {
                            controllerInstance.AppData.Debug += Environment.NewLine;
                        }
                    }
                    controllerInstance.AppData.Debug += (Environment.NewLine + "_________________________" + Environment.NewLine);
                }
            }
            return null;
        }

        private static async Task<ControlPacket> SetTime(UsbControllerInstance controllerInstance, byte[] timeValue)
        {
            if (GenerateAndSendFrames(controllerInstance, UsbAppCommon.CONTROL_CMD.CMD_SET_TIME, timeValue, 3) > 0)
            {
                ControlPacket response = GetDeviceResponse(controllerInstance, UsbAppCommon.CONTROL_CMD.CMD_SET_TIME);
                return response;
            }
            return null;
        }

        private static ControlPacket GetDeviceResponse(UsbControllerInstance controllerInstance, UsbAppCommon.CONTROL_CMD cmd, uint readTimeout = 200)
        {
            uint byteCnt;
            uint frameCnt = 0;
            FrameInfo frameInfo = new FrameInfo();
            ControlPacket controlPacket = new ControlPacket();
            frameInfo.OutBufferMaxLen = 512;

            while (!frameInfo.FrameValid)
            {
                byteCnt = usbDriver.ReadUSBDevice(controllerInstance.UsbDevice, ref frameInfo.FrameData, UsbDriver.USB_PACKET_SIZE, readTimeout);
                if (byteCnt > 0)
                {
                    frameCnt++;
                    frameInfo.FrameLen = byteCnt;
                    // validate frame and load data into controlPacket
                    byteCnt = ValidateFrame(frameInfo, controlPacket);
                    if (byteCnt == 0)
                    {
                        return null;
                    }
                    else if (frameInfo.FrameValid)
                    {
                        if(controlPacket.Cmd == UsbAppCommon.CONTROL_CMD.CMD_ERROR_OCCURRED)
                        {
                            // handle error codes from the Controller
                            byte errorCode = controlPacket.Data[0];
                            string err = $"USB Controller Error. Error code = [{errorCode.ToString("X2")}]. Error message: {GetErrorMessage(errorCode)}";
                            Trace.WriteLine(err);
                            throw new Exception(err);
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

        private static uint GenerateAndSendFrames(UsbControllerInstance controllerInstance, UsbAppCommon.CONTROL_CMD cmd, Byte[] frameData, uint frameLen)
        {
            uint byteCnt;
            uint bytesSent = 0;
            Byte[] usbBuffer = new byte[UsbDriver.USB_PACKET_SIZE];
            Byte[] packetsBuffer = new byte[UsbAppCommon.USB_BUFFER_SIZE];
            byteCnt = GenerateFrames(cmd, frameData, frameLen, ref packetsBuffer, UsbAppCommon.USB_BUFFER_SIZE);
            while (byteCnt > 0)
            {
                uint sendLen = byteCnt > 64 ? 64 : byteCnt;
                for (int i = 0; i < sendLen; i++)
                {
                    usbBuffer[i] = packetsBuffer[i + bytesSent];
                }
                if (usbDriver.WriteUSBDevice(controllerInstance.UsbDevice, usbBuffer, sendLen) == 0)
                {
                    return 0;
                }
                byteCnt -= sendLen;
                bytesSent += sendLen;
            }
            return bytesSent;
        }

        private static uint GenerateFrames(UsbAppCommon.CONTROL_CMD cmd, Byte[] frameData, uint dataLen, ref Byte[] outBuffer, uint outBufferMaxLen)
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
            while (packetDataCount > 0)
            {
                outBuffer[outBufferLen++] = 0xFF;
                packetDataCount--;
            }

            return (outBufferLen); // Return buffer length.
        }

        

        private static uint ValidateFrame(FrameInfo frameInfo, ControlPacket controlPacket)
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
                if(controlPacket.Cmd != (UsbAppCommon.CONTROL_CMD)frameInfo.FrameData[1])
                {
                    throw new Exception("Invalid multipacket.");
                }
            }
            else
            {
                controlPacket.Cmd = (UsbAppCommon.CONTROL_CMD)frameInfo.FrameData[1];
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

        
        private static string GetErrorMessage(byte errorCode)
        {
            string ret = "Unknown error code.";
            switch(errorCode)
            {
                case 0x01: ret = "Controller transmit buffer overflow."; break;
                case 0x02: ret = "Controller receiver buffer overflow."; break;
                case 0x03: ret = "Controller EEPROM failure."; break;
                case 0x04: ret = "Controller invalid payload length."; break;
                case 0xF0: ret = "Controller USB overflow (still processing last request)."; break;
                case 0xF1: ret = "Controller invalid USB multipacket frame."; break;
                case 0xF2: ret = "Controller invalid CRC."; break;
                case 0xFF: ret = "Controller unknown command."; break;
            }
            return ret;
        }
    }
}
