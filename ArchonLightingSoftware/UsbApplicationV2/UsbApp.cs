using System;
using System.Threading;
using System.ComponentModel;
using ArchonLightingSystem.Models;
using ArchonLightingSystem.Common;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;
using ArchonLightingSystem.Interfaces;

namespace ArchonLightingSystem.UsbApplicationV2
{
    public static partial class UsbApp
    {
        private static UsbReadWrite usbIO = new UsbReadWrite();
        private static uint defaultReadTimeout = 20; //ms
        private static uint eepromReadTimeout = 400; //ms

        private static ControlPacket ReadBootloaderInfo(IUsbDevice usbDevice, CancellationTokenSource cancelToken = null)
        {
            if (GenerateAndSendFrames(usbDevice, UsbAppCommon.CONTROL_CMD.CMD_READ_BOOTLOADER_INFO, null, 0, cancelToken) > 0)
            {
                ControlPacket response = GetDeviceResponse(usbDevice, UsbAppCommon.CONTROL_CMD.CMD_READ_BOOTLOADER_INFO, defaultReadTimeout, cancelToken);
                return response;
            }
            return null;
        }

        private static ControlPacket ReadApplicationInfo(IUsbDevice usbDevice, CancellationTokenSource cancelToken = null)
        {
            if (GenerateAndSendFrames(usbDevice, UsbAppCommon.CONTROL_CMD.CMD_READ_FIRMWARE_INFO, null, 0, cancelToken) > 0)
            {
                ControlPacket response = GetDeviceResponse(usbDevice, UsbAppCommon.CONTROL_CMD.CMD_READ_FIRMWARE_INFO, defaultReadTimeout, cancelToken);
                return response;
            }
            return null;
        }

        private static ControlPacket WriteFanSpeed(IUsbDevice usbDevice, byte[] speedValues, CancellationTokenSource cancelToken = null)
        {
            if (GenerateAndSendFrames(usbDevice, UsbAppCommon.CONTROL_CMD.CMD_WRITE_FANSPEED, speedValues, DeviceControllerDefinitions.DevicePerController, cancelToken) > 0)
            {
                ControlPacket response = GetDeviceResponse(usbDevice, UsbAppCommon.CONTROL_CMD.CMD_WRITE_FANSPEED, defaultReadTimeout, cancelToken);
                return response;
            }
            return null;
        }

        private static bool ResetDeviceToBootloader(IUsbDevice usbDevice, CancellationTokenSource cancelToken = null)
        {
            return GenerateAndSendFrames(usbDevice, UsbAppCommon.CONTROL_CMD.CMD_RESET_TO_BOOTLOADER, null, 0, cancelToken) > 0;
        }

        private static ControlPacket ReadBootStatus(IUsbDevice usbDevice, CancellationTokenSource cancelToken = null)
        {
            if (GenerateAndSendFrames(usbDevice, UsbAppCommon.CONTROL_CMD.CMD_READ_BOOT_STATUS, null, 0, cancelToken) > 0)
            {
                ControlPacket response = GetDeviceResponse(usbDevice, UsbAppCommon.CONTROL_CMD.CMD_READ_BOOT_STATUS, defaultReadTimeout, cancelToken);
                return response;
            }
            return null;
        }


        private static ControlPacket ReadEeprom(IUsbDevice usbDevice, UInt16 address, UInt16 length, CancellationTokenSource cancelToken = null)
        {
            byte[] request = new byte[] { (byte)address, (byte)(length & 0xFF), (byte)((length >> 8) & 0xFF) };
            if (GenerateAndSendFrames(usbDevice, UsbAppCommon.CONTROL_CMD.CMD_READ_EEPROM, request, 3, cancelToken) > 0)
            {
                ControlPacket response = GetDeviceResponse(usbDevice, UsbAppCommon.CONTROL_CMD.CMD_READ_EEPROM, eepromReadTimeout, cancelToken);
                return response;
            }
            return null;
        }

        private static ControlPacket DefaultConfig(IUsbDevice usbDevice, CancellationTokenSource cancelToken = null)
        {
            if (GenerateAndSendFrames(usbDevice, UsbAppCommon.CONTROL_CMD.CMD_DEFAULT_CONFIG, null, 0, cancelToken) > 0)
            {
                ControlPacket response = GetDeviceResponse(usbDevice, UsbAppCommon.CONTROL_CMD.CMD_DEFAULT_CONFIG, defaultReadTimeout, cancelToken);
                return response;
            }
            return null;
        }

        private static ControlPacket ReadConfig(IUsbDevice usbDevice, CancellationTokenSource cancelToken = null)
        {
            if (GenerateAndSendFrames(usbDevice, UsbAppCommon.CONTROL_CMD.CMD_READ_CONFIG, null, 0, cancelToken) > 0)
            {
                ControlPacket response = GetDeviceResponse(usbDevice, UsbAppCommon.CONTROL_CMD.CMD_READ_CONFIG, eepromReadTimeout, cancelToken);
                return response;
            }
            return null;
        }

        private static ControlPacket WriteConfig(IUsbDevice usbDevice, CancellationTokenSource cancelToken = null)
        {
            if (GenerateAndSendFrames(usbDevice, UsbAppCommon.CONTROL_CMD.CMD_WRITE_CONFIG, null, 0, cancelToken) > 0)
            {
                ControlPacket response = GetDeviceResponse(usbDevice, UsbAppCommon.CONTROL_CMD.CMD_WRITE_CONFIG, eepromReadTimeout, cancelToken);
                return response;
            }
            return null;
        }

        private static ControlPacket UpdateConfig(IUsbDevice usbDevice, DeviceControllerConfig config, CancellationTokenSource cancelToken = null)
        {
            Byte[] buffer = new byte[DeviceControllerDefinitions.EepromSize];
            uint length = config.ToBuffer(ref buffer);
            if (GenerateAndSendFrames(usbDevice, UsbAppCommon.CONTROL_CMD.CMD_UPDATE_CONFIG, buffer, length, cancelToken) > 0)
            {
                ControlPacket response = GetDeviceResponse(usbDevice, UsbAppCommon.CONTROL_CMD.CMD_UPDATE_CONFIG, 50, cancelToken);
                return response;
            }
            return null;
        }

        private static ControlPacket WriteLedFrame(IUsbDevice usbDevice, byte[,] ledFrame, CancellationTokenSource cancelToken = null)
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
            if (GenerateAndSendFrames(usbDevice, UsbAppCommon.CONTROL_CMD.CMD_WRITE_LED_FRAME, buffer, length, cancelToken) > 0)
            {
                ControlPacket response = GetDeviceResponse(usbDevice, UsbAppCommon.CONTROL_CMD.CMD_WRITE_LED_FRAME, defaultReadTimeout, cancelToken);
                return response;
            }
            return null;
        }

        private static ControlPacket ReadControllerAddress(IUsbDevice usbDevice, CancellationTokenSource cancelToken = null)
        {
            if (GenerateAndSendFrames(usbDevice, UsbAppCommon.CONTROL_CMD.CMD_READ_CONTROLLER_ADDRESS, null, 0, cancelToken) > 0)
            {
                ControlPacket response = GetDeviceResponse(usbDevice, UsbAppCommon.CONTROL_CMD.CMD_READ_CONTROLLER_ADDRESS, defaultReadTimeout, cancelToken);
                return response;
            }
            return null;
        }

        private static ControlPacket ReadDebug(IUsbDevice usbDevice, CancellationTokenSource cancelToken = null)
        {
            if (GenerateAndSendFrames(usbDevice, UsbAppCommon.CONTROL_CMD.CMD_READ_EE_DEBUG, null, 0, cancelToken) > 0)
            {
                ControlPacket response = GetDeviceResponse(usbDevice, UsbAppCommon.CONTROL_CMD.CMD_READ_EE_DEBUG, eepromReadTimeout, cancelToken);
                if (response != null)
                {
                    //usbDevice.AppData.Debug = ($"Debug Len: {response.Len}"+Environment.NewLine);
                    for (int i = 0; i < response.Len;)
                    {
                        var dat = response.Data[i];
                        //usbDevice.AppData.Debug += ($"{dat.ToString("X2")} ");
                        if (++i % 20 == 0)
                        {
                            //usbDevice.AppData.Debug += Environment.NewLine;
                        }
                    }
                    //usbDevice.AppData.Debug += (Environment.NewLine + "_________________________" + Environment.NewLine);
                }
            }
            return null;
        }

        private static ControlPacket SetTime(IUsbDevice usbDevice, byte[] timeValue, CancellationTokenSource cancelToken = null)
        {
            if (GenerateAndSendFrames(usbDevice, UsbAppCommon.CONTROL_CMD.CMD_SET_TIME, timeValue, 3, cancelToken) > 0)
            {
                ControlPacket response = GetDeviceResponse(usbDevice, UsbAppCommon.CONTROL_CMD.CMD_SET_TIME, defaultReadTimeout, cancelToken);
                return response;
            }
            return null;
        }
        
        private static ControlPacket GetDeviceResponse(IUsbDevice usbDevice, UsbAppCommon.CONTROL_CMD cmd, uint readTimeout = 200, CancellationTokenSource cancelToken = null)
        {
            uint byteCnt;
            uint frameCnt = 0;
            FrameInfo frameInfo = new FrameInfo();
            ControlPacket controlPacket = new ControlPacket();
            frameInfo.OutBufferMaxLen = 512;

            while (!frameInfo.FrameValid)
            {
                byteCnt = usbIO.Read(usbDevice, ref frameInfo.FrameData, UsbDeviceManager.USB_PACKET_SIZE, readTimeout, cancelToken);
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

        private static uint GenerateAndSendFrames(IUsbDevice usbDevice, UsbAppCommon.CONTROL_CMD cmd, Byte[] frameData, uint frameLen, CancellationTokenSource cancelToken = null)
        {
            uint byteCnt;
            uint bytesSent = 0;
            Byte[] usbBuffer = new byte[UsbDeviceManager.USB_PACKET_SIZE];
            Byte[] packetsBuffer = new byte[UsbAppCommon.USB_BUFFER_SIZE];

            byteCnt = GenerateFrames(cmd, frameData, frameLen, ref packetsBuffer, UsbAppCommon.USB_BUFFER_SIZE);
            while (byteCnt > 0)
            {
                uint sendLen = byteCnt > 64 ? 64 : byteCnt;
                for (int i = 0; i < sendLen; i++)
                {
                    usbBuffer[i] = packetsBuffer[i + bytesSent];
                }
                if (usbIO.Write(usbDevice, usbBuffer, sendLen, cancelToken) == 0)
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
                    throw new Exception("Software frame buffer overflow.");
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
                throw new Exception("Software invalid packet length.");
            }

            if (frameInfo.Multiframe) {
                if(controlPacket.Cmd != (UsbAppCommon.CONTROL_CMD)frameInfo.FrameData[1])
                {
                    throw new Exception("Software invalid multipacket.");
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
