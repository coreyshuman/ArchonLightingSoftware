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
        public static async Task<bool> GetDeviceInitialization(IUsbDevice usbDevice)
        {
            ControlPacket bootResponse;
            ControlPacket appResponse;
            ControlPacket bootStatusResponse;
            ControlPacket eepromResponse;
            ControlPacket usbDeviceAddressResponse;
            ControlPacket usbDeviceConfigResponse;

            var cancelToken = usbDevice.GetCancellationToken();

            try
            {
                await usbDevice.WaitAsync(cancelToken);

                bootResponse = await ReadBootloaderInfo(usbDevice, cancelToken);
                if (bootResponse == null) throw new Exception("Couldn't read Bootloader info.");
                if (cancelToken.IsCancellationRequested) return false;
                Logger.Write(Level.Trace, $"DeviceInit {usbDevice.ShortName} Boot: {bootResponse.Data[0]}.{bootResponse.Data[1]}");

                appResponse = await ReadApplicationInfo(usbDevice, cancelToken);
                if (appResponse == null) throw new Exception("Couldn't read Application info.");
                if (cancelToken.IsCancellationRequested) return false;
                Logger.Write(Level.Trace, $"DeviceInit {usbDevice.ShortName} App: {appResponse.Data[0]}.{appResponse.Data[1]}");

                usbDeviceAddressResponse = await SendReadAddressCmd(usbDevice, cancelToken);
                if (usbDeviceAddressResponse == null) throw new Exception("Couldn't read Address.");
                if (cancelToken.IsCancellationRequested) return false;
                Logger.Write(Level.Trace, $"DeviceInit {usbDevice.ShortName} Addr: {usbDeviceAddressResponse.Data[0]}");

                bootStatusResponse = await ReadBootStatus(usbDevice, cancelToken);
                if (bootStatusResponse == null) throw new Exception("Couldn't read boot status.");
                if (cancelToken.IsCancellationRequested) return false;
                Logger.Write(Level.Trace, $"DeviceInit {usbDevice.ShortName} Status: {bootStatusResponse.Data[0]}");

                eepromResponse = await ReadEeprom(usbDevice, 0, (UInt16)DeviceControllerDefinitions.EepromSize, cancelToken);
                if (eepromResponse == null) throw new Exception("Couldn't read EEPROM.");
                if (cancelToken.IsCancellationRequested) return false;
                Logger.Write(Level.Trace, $"DeviceInit {usbDevice.ShortName} EEPROM: {eepromResponse.Len} bytes");

                usbDeviceConfigResponse = await SendReadConfigCmd(usbDevice, cancelToken);
                if (usbDeviceConfigResponse == null) throw new Exception("Couldn't read Config.");
                if (cancelToken.IsCancellationRequested) return false;
                Logger.Write(Level.Trace, $"DeviceInit {usbDevice.ShortName} Config: {usbDeviceConfigResponse.Len} bytes");

                //usbDevice.AppData.DeviceControllerData.InitializeDevice(usbDeviceAddressResponse.Data[0], eepromResponse.Data, usbDeviceConfigResponse.Data, bootResponse.Data, appResponse.Data, bootStatusResponse.Data);
                return true;
            }
            catch(Exception ex)
            {
                Logger.Write(Level.Warning, $"Initialize UsbDevice {usbDevice.ShortName} Error: {ex.Message}");
                return false;
            }
            finally
            {
                usbDevice.Release(cancelToken);
            }
        }

        private enum DeviceInitializationState
        {
            ReserveDevice = 0,
            ReadBootloader,
            ReadApplication,
            ReadAddress,
            ReadBootStatus,
            ReadEeprom,
            ReadConfig,
            Done
        }

        public static Task<DeviceControllerData> GetDeviceInitializationAsync(IUsbDevice usbDevice)
        {
            return Task.Run(async () =>
            {
                DeviceControllerData deviceControllerData = new DeviceControllerData();

                ControlPacket response = null;
                int retryCount = 0;

                DeviceInitializationState initState = DeviceInitializationState.ReserveDevice;

                var cancelToken = usbDevice.GetCancellationToken();

                try
                {
                    while (!cancelToken.IsCancellationRequested)
                    {
                        retryCount++;

                        try
                        {
                            switch (initState)
                            {
                                case DeviceInitializationState.ReserveDevice:
                                    usbDevice.Wait(cancelToken);
                                    initState++;
                                    retryCount = 0;
                                    continue;
                                case DeviceInitializationState.ReadBootloader:
                                    response = await ReadBootloaderInfo(usbDevice, cancelToken);
                                    break;
                                case DeviceInitializationState.ReadApplication:
                                    response = await ReadApplicationInfo(usbDevice, cancelToken);
                                    break;
                                case DeviceInitializationState.ReadAddress:
                                    response = await SendReadAddressCmd(usbDevice, cancelToken);
                                    break;
                                case DeviceInitializationState.ReadBootStatus:
                                    response = await ReadBootStatus(usbDevice, cancelToken);
                                    break;
                                case DeviceInitializationState.ReadEeprom:
                                    response = await ReadEeprom(usbDevice, 0, (UInt16)DeviceControllerDefinitions.EepromSize, cancelToken);
                                    break;
                                case DeviceInitializationState.ReadConfig:
                                    response = await SendReadConfigCmd(usbDevice, cancelToken);
                                    break;
                            }

                            if(response == null)
                            {
                                throw new Exception("No Response.");
                            }
                        }
                        catch(Exception ex)
                        {
                            Logger.Write(Level.Trace, $"Initialize UsbDevice Read {usbDevice.ShortName} {Enum.GetName(typeof(DeviceInitializationState), initState)} Warning: {ex.Message}");
                            if (retryCount >= 3)
                            {
                                throw new Exception($"Failed on {Enum.GetName(typeof(DeviceInitializationState), initState)}", ex);
                            }

                            Logger.Write(Level.Trace, "_retryA");
                            continue;
                        }

                        try
                        {
                            switch (initState)
                            {
                                case DeviceInitializationState.ReadBootloader:
                                    deviceControllerData.UpdateBootloaderVersion(response.Data, response.Len);
                                    Logger.Write(Level.Trace, $"DeviceInit {usbDevice.ShortName} Boot: {response.Data[0]}.{response.Data[1]}");
                                    break;
                                case DeviceInitializationState.ReadApplication:
                                    deviceControllerData.UpdateApplicationVersion(response.Data, response.Len);
                                    Logger.Write(Level.Trace, $"DeviceInit {usbDevice.ShortName} App: {response.Data[0]}.{response.Data[1]}");
                                    break;
                                case DeviceInitializationState.ReadAddress:
                                    deviceControllerData.UpdateAddress(response.Data, response.Len);
                                    Logger.Write(Level.Trace, $"DeviceInit {usbDevice.ShortName} Addr: {response.Data[0]}");
                                    break;
                                case DeviceInitializationState.ReadBootStatus:
                                    deviceControllerData.UpdateBootStatusFlag(response.Data, response.Len);
                                    Logger.Write(Level.Trace, $"DeviceInit {usbDevice.ShortName} Status: {deviceControllerData.BootStatusFlag}");
                                    break;
                                case DeviceInitializationState.ReadEeprom:
                                    deviceControllerData.UpdateEepromData(response.Data, response.Len);
                                    Logger.Write(Level.Trace, $"DeviceInit {usbDevice.ShortName} EEPROM: {response.Len} bytes");
                                    break;
                                case DeviceInitializationState.ReadConfig:
                                    deviceControllerData.UpdateDeviceConfig(response.Data, response.Len);
                                    Logger.Write(Level.Trace, $"DeviceInit {usbDevice.ShortName} Config: {response.Len} bytes");
                                    break;
                            }

                            initState++;
                            retryCount = 0;

                            if (initState == DeviceInitializationState.Done)
                            {
                                return deviceControllerData;
                            }
                        }
                        catch (Exception ex)
                        {
                            if (retryCount >= 3)
                            {
                                throw new Exception($"Failed on {Enum.GetName(typeof(DeviceInitializationState), initState)}", ex);
                            }

                            Logger.Write(Level.Trace, "_retryB");
                            continue;
                        }
                    }

                    throw new Exception("Initialization was cancelled.");
                }
                catch (Exception ex)
                {
                    Logger.Write(Level.Warning, $"Initialize UsbDevice {usbDevice.ShortName} Error: {ex.Message} {ex.InnerException?.Message}");
                    return null;
                }
                finally
                {
                    usbDevice.Release(cancelToken);
                }
            });
        }
    }
}
