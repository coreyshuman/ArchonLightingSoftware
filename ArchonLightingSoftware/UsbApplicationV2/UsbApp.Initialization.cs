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
                Logger.Write(Level.Trace, $"DeviceInit Boot: {bootResponse.Data[0]}.{bootResponse.Data[1]}");

                appResponse = ReadApplicationInfo(usbDevice, cancelToken);
                if (appResponse == null) throw new Exception("Couldn't read Application info.");
                if (cancelToken.IsCancellationRequested) return false;
                Logger.Write(Level.Trace, $"DeviceInit App: {appResponse.Data[0]}.{appResponse.Data[1]}");

                usbDeviceAddressResponse = ReadControllerAddress(usbDevice, cancelToken);
                if (usbDeviceAddressResponse == null) throw new Exception("Couldn't read Address.");
                if (cancelToken.IsCancellationRequested) return false;
                Logger.Write(Level.Trace, $"DeviceInit Addr: {usbDeviceAddressResponse.Data[0]}");

                bootStatusResponse = ReadBootStatus(usbDevice, cancelToken);
                if (bootStatusResponse == null) throw new Exception("Couldn't read boot status.");
                if (cancelToken.IsCancellationRequested) return false;

                eepromResponse = ReadEeprom(usbDevice, 0, (UInt16)DeviceControllerDefinitions.EepromSize, cancelToken);
                if (eepromResponse == null) throw new Exception("Couldn't read EEPROM.");
                if (cancelToken.IsCancellationRequested) return false;

                usbDeviceConfigResponse = ReadConfig(usbDevice, cancelToken);
                if (usbDeviceConfigResponse == null) throw new Exception("Couldn't read Config.");
                if (cancelToken.IsCancellationRequested) return false;

                //usbDevice.AppData.DeviceControllerData.InitializeDevice(usbDeviceAddressResponse.Data[0], eepromResponse.Data, usbDeviceConfigResponse.Data, bootResponse.Data, appResponse.Data, bootStatusResponse.Data);
                return true;
            }
            catch(Exception ex)
            {
                Logger.Write(Level.Warning, $"Initialize UsbDevice Error: {ex.Message}");
                return false;
            }
            finally
            {
                usbDevice.Release(cancelToken);
            }
        }
    }
}
