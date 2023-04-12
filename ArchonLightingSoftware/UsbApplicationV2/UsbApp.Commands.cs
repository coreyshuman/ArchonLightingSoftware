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
        public static async Task<bool> ReadAndUpdateEepromAsync(UsbControllerDevice controllerInstance)
        {
            ControlPacket response =
                await WithDeviceLock(controllerInstance, (token) =>
                {
                    return ReadEeprom(
                        controllerInstance.UsbDevice,
                        0,
                        (ushort)DeviceControllerDefinitions.EepromSize,
                        token
                    );
                });

            if (response != null)
            {
                controllerInstance.ControllerData.UpdateEepromData(response.Data, response.Len);
                return true;
            }

            return false;
        }

        public static async Task<bool> WriteAndUpdateEepromAsync(UsbControllerDevice controllerInstance)
        {
            ControlPacket response =
                await WithDeviceLock(controllerInstance, async (token) =>
                {
                    try
                    {
                        var res = await WriteEeprom(
                            controllerInstance.UsbDevice,
                            0,
                            (ushort)DeviceControllerDefinitions.EepromSize,
                            controllerInstance.ControllerData.EepromData,
                            token
                            );
                        if(res == null )
                        {
                            throw new Exception("write failed");
                        }
                        return await ReadEeprom(
                            controllerInstance.UsbDevice,
                            0,
                            (ushort)DeviceControllerDefinitions.EepromSize,
                            token
                        );
                    }
                    catch( Exception ex )
                    {
                        Logger.Write(Level.Error, ex.Message);
                        throw ex;
                    }
                });

            if (response != null)
            {
                controllerInstance.ControllerData.UpdateEepromData(response.Data, response.Len);
                return true;
            }


            return false;
        }

        public static async Task<bool> ReadAndUpdateConfigAsync(UsbControllerDevice controllerInstance)
        {
            ControlPacket response =
                await WithDeviceLock(controllerInstance, (token) =>
                {
                    return SendReadConfigCmd( controllerInstance.UsbDevice, token );
                });

            if (response != null)
            {
                controllerInstance.ControllerData.UpdateDeviceConfig(response.Data, response.Len);
                return true;
            }

            return false;
        }

        public static async Task<bool> WriteAndUpdateConfigAsync(UsbControllerDevice controllerInstance)
        {
            ControlPacket response =
                await WithDeviceLock(controllerInstance, async (token) =>
                {
                    try
                    {
                        var res = await SendWriteConfigCmd(
                            controllerInstance.UsbDevice,
                            controllerInstance.ControllerData.DeviceConfig,
                            token
                            );
                        if (res == null)
                        {
                            throw new Exception("write failed");
                        }
                        return await SendReadConfigCmd(controllerInstance.UsbDevice, token);
                    }
                    catch (Exception ex)
                    {
                        Logger.Write(Level.Error, ex.Message);
                        throw ex;
                    }
                });

            if (response != null)
            {
                controllerInstance.ControllerData.UpdateDeviceConfig(response.Data, response.Len);
                return true;
            }

            return false;
        }

        public static async Task<bool> CommitConfigToEepromAsync(UsbControllerDevice controllerInstance)
        {
            ControlPacket response =
                await WithDeviceLock(controllerInstance, async (token) =>
                {
                    try
                    {
                        var res = await SendCommitConfigCmd(controllerInstance.UsbDevice, token);
                        if (res == null)
                        {
                            throw new Exception("write failed");
                        }
                        return await SendReadConfigCmd(controllerInstance.UsbDevice, token);
                    }
                    catch (Exception ex)
                    {
                        Logger.Write(Level.Error, ex.Message);
                        throw ex;
                    }
                });

            if (response != null)
            {
                controllerInstance.ControllerData.UpdateDeviceConfig(response.Data, response.Len);
                return true;
            }

            return false;
        }

        public static async Task<bool> ResetConfigToDefaultAsync(UsbControllerDevice controllerInstance)
        {
            ControlPacket response =
                await WithDeviceLock(controllerInstance, async (token) =>
                {
                    try
                    {
                        var res = await SendDefaultConfigCmd(controllerInstance.UsbDevice, token);
                        if (res == null)
                        {
                            throw new Exception("write failed");
                        }
                        return await SendReadConfigCmd(controllerInstance.UsbDevice, token);
                    }
                    catch (Exception ex)
                    {
                        Logger.Write(Level.Error, ex.Message);
                        throw ex;
                    }
                });

            if (response != null)
            {
                controllerInstance.ControllerData.UpdateDeviceConfig(response.Data, response.Len);
                return true;
            }

            return false;
        }

        private static async Task<T> WithDeviceLock<T>(UsbControllerDevice ci, Func<CancellationTokenSource, Task<T>> action)
        {
            CancellationTokenSource cancelToken = new CancellationTokenSource();
            await ci.UsbDevice.WaitAsync(cancelToken);

            try
            {
                return await action(cancelToken);
            }
            finally
            {
                ci.UsbDevice.Release(cancelToken);
            }
        }
    }
}
