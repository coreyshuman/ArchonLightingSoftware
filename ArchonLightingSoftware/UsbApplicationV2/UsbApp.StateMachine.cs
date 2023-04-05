using System;
using System.Threading;
using System.ComponentModel;
using ArchonLightingSystem.Models;
using ArchonLightingSystem.Common;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;

namespace ArchonLightingSystem.UsbApplicationV2
{
    public static partial class UsbApp
    {
        /*
        public static async Task DeviceDoWork(UsbControllerDevice controllerInstance)
        {
            Byte[] rxtxBuffer = new Byte[UsbAppCommon.USB_BUFFER_SIZE];
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

                        for (i = 0; i < UsbAppCommon.USB_BUFFER_SIZE; i++)
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
                            controllerInstance.AppData.DeviceControllerData.UpdateEepromData(response.Data);
                            controllerInstance.AppData.EepromReadDone = true;
                        }

                        if (controllerInstance.AppData.EepromWritePending)
                        {
                            controllerInstance.AppData.EepromWritePending = false;
                            for (i = 0; i < UsbAppCommon.USB_BUFFER_SIZE; i++)
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
        */
    }
}
