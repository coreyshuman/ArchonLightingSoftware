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
        public static bool GetDeviceInitialization(IUsbDevice usbDevice)
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
                usbDevice.Wait(cancelToken);

                bootResponse = ReadBootloaderInfo(usbDevice, cancelToken);
                if (bootResponse == null) throw new Exception("Couldn't read Bootloader info.");
                if (cancelToken.IsCancellationRequested) return false;
                Logger.Write(Level.Trace, $"DeviceInit {usbDevice.ShortName} Boot: {bootResponse.Data[0]}.{bootResponse.Data[1]}");

                appResponse = ReadApplicationInfo(usbDevice, cancelToken);
                if (appResponse == null) throw new Exception("Couldn't read Application info.");
                if (cancelToken.IsCancellationRequested) return false;
                Logger.Write(Level.Trace, $"DeviceInit {usbDevice.ShortName} App: {appResponse.Data[0]}.{appResponse.Data[1]}");

                usbDeviceAddressResponse = ReadControllerAddress(usbDevice, cancelToken);
                if (usbDeviceAddressResponse == null) throw new Exception("Couldn't read Address.");
                if (cancelToken.IsCancellationRequested) return false;
                Logger.Write(Level.Trace, $"DeviceInit {usbDevice.ShortName} Addr: {usbDeviceAddressResponse.Data[0]}");

                bootStatusResponse = ReadBootStatus(usbDevice, cancelToken);
                if (bootStatusResponse == null) throw new Exception("Couldn't read boot status.");
                if (cancelToken.IsCancellationRequested) return false;
                Logger.Write(Level.Trace, $"DeviceInit {usbDevice.ShortName} Status: {bootStatusResponse.Data[0]}");

                eepromResponse = ReadEeprom(usbDevice, 0, (UInt16)DeviceControllerDefinitions.EepromSize, cancelToken);
                if (eepromResponse == null) throw new Exception("Couldn't read EEPROM.");
                if (cancelToken.IsCancellationRequested) return false;
                Logger.Write(Level.Trace, $"DeviceInit {usbDevice.ShortName} EEPROM: {eepromResponse.Len} bytes");

                usbDeviceConfigResponse = ReadConfig(usbDevice, cancelToken);
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

        public static Task<bool> GetDeviceInitializationAsync(IUsbDevice usbDevice)
        {
            return Task.Run(() =>
            {
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
                                    response = ReadBootloaderInfo(usbDevice, cancelToken);
                                    break;
                                case DeviceInitializationState.ReadApplication:
                                    response = ReadApplicationInfo(usbDevice, cancelToken);
                                    break;
                                case DeviceInitializationState.ReadAddress:
                                    response = ReadControllerAddress(usbDevice, cancelToken);
                                    break;
                                case DeviceInitializationState.ReadBootStatus:
                                    response = ReadBootStatus(usbDevice, cancelToken);
                                    break;
                                case DeviceInitializationState.ReadEeprom:
                                    response = ReadEeprom(usbDevice, 0, (UInt16)DeviceControllerDefinitions.EepromSize, cancelToken);
                                    break;
                                case DeviceInitializationState.ReadConfig:
                                    response = ReadConfig(usbDevice, cancelToken);
                                    break;
                            }
                        }
                        catch(Exception ex)
                        {
                            Logger.Write(Level.Warning, $"Initialize UsbDevice {usbDevice.ShortName} Warning: {ex.Message}");
                            response = null;
                        }

                        if(response == null)
                        {
                            if(retryCount >= 3)
                            {
                                switch(initState)
                                {
                                    case DeviceInitializationState.ReadBootloader:
                                        throw new Exception("Couldn't read Bootloader info.");
                                    case DeviceInitializationState.ReadApplication:
                                        throw new Exception("Couldn't read Application info.");
                                    case DeviceInitializationState.ReadAddress:
                                        throw new Exception("Couldn't read Address.");
                                    case DeviceInitializationState.ReadBootStatus:
                                        throw new Exception("Couldn't read boot status.");
                                    case DeviceInitializationState.ReadEeprom:
                                        throw new Exception("Couldn't read Eeprom.");
                                    case DeviceInitializationState.ReadConfig:
                                        throw new Exception("Couldn't read Config.");
                                }
                            }

                            retryCount++;
                            Logger.Write(Level.Trace, "_retry");
                            continue;
                        }

                        retryCount = 0;

                        switch(initState)
                        {
                            case DeviceInitializationState.ReadBootloader:
                                Logger.Write(Level.Trace, $"DeviceInit {usbDevice.ShortName} Boot: {response.Data[0]}.{response.Data[1]}");
                                break;
                            case DeviceInitializationState.ReadApplication:
                                Logger.Write(Level.Trace, $"DeviceInit {usbDevice.ShortName} App: {response.Data[0]}.{response.Data[1]}");
                                break;
                            case DeviceInitializationState.ReadAddress:
                                Logger.Write(Level.Trace, $"DeviceInit {usbDevice.ShortName} Addr: {response.Data[0]}");
                                break;
                            case DeviceInitializationState.ReadBootStatus:
                                Logger.Write(Level.Trace, $"DeviceInit {usbDevice.ShortName} Status: {response.Data[0]}");
                                break;
                            case DeviceInitializationState.ReadEeprom:
                                Logger.Write(Level.Trace, $"DeviceInit {usbDevice.ShortName} EEPROM: {response.Len} bytes");
                                break;
                            case DeviceInitializationState.ReadConfig:
                                Logger.Write(Level.Trace, $"DeviceInit {usbDevice.ShortName} Config: {response.Len} bytes");
                                break;
                        }

                        initState++;

                        if(initState == DeviceInitializationState.Done)
                        {
                            return true;
                        }
                    }

                    return false;
                }
                catch (Exception ex)
                {
                    Logger.Write(Level.Warning, $"Initialize UsbDevice {usbDevice.ShortName} Error: {ex.Message}");
                    return false;
                }
                finally
                {
                    usbDevice.Release(cancelToken);
                }
            });
        }
    }
}
