using System;
using ArchonLightingSystem.Common;


namespace ArchonLightingSystem.Models
{
    public class DeviceControllerData
    {
        public Byte DeviceAddress { get; set; }
        public UInt16[] MeasuredFanRpm { get; set; }
        public Byte[] EepromData { get; set; }
        public DeviceControllerConfig DeviceConfig { get; set; }
        public bool IsInitialized { get { return isInitialized; } }
        public Version BootloaderVersion { get; set; }
        public Version ApplicationVersion { get; set; }
        public UInt32 BootStatusFlag { get; set; }
        public Byte[] AutoFanSpeedValue { get; set; }
        public Byte[] TemperatureValue { get; set; }
        public Byte[] FanFailedHealthCheckCount { get; set; }

        private bool isInitialized;

        public DeviceControllerData()
        {
            FanFailedHealthCheckCount = new byte[DeviceControllerDefinitions.DevicePerController];
            Reset();
        }

        public void InitializeDevice(Byte deviceAddress, Byte[] eepromData, Byte[] deviceConfig, Byte[] bootVer, Byte[] appVer, Byte[] bootStatus)
        {
            BootloaderVersion = new Version(bootVer[0], bootVer[1]);
            ApplicationVersion = new Version(appVer[0], appVer[1]);
            DeviceAddress = deviceAddress;
            //UpdateEepromData(eepromData);
            DeviceConfig.FromBuffer(deviceConfig);
            BootStatusFlag = Util.UInt32FromBytes(bootStatus[0], bootStatus[1], bootStatus[2], bootStatus[4]);
            isInitialized = true;
        }

        public void Reset()
        {
            DeviceAddress = 0;
            MeasuredFanRpm = new UInt16[DeviceControllerDefinitions.DevicePerController];
            EepromData = new byte[DeviceControllerDefinitions.EepromSize];
            DeviceConfig = new DeviceControllerConfig();
            BootloaderVersion = new Version();
            ApplicationVersion = new Version();
            AutoFanSpeedValue = new byte[DeviceControllerDefinitions.DevicePerController];
            TemperatureValue = new byte[DeviceControllerDefinitions.DevicePerController];
            isInitialized = false;
        }

        public void UpdateBootloaderVersion(Byte[] data, uint len)
        {
            if(data == null || data.Length < len || len != 2) 
            {
                throw new ArgumentException("Invalid response length.");
            }
            BootloaderVersion = new Version(data[0], data[1]);
        }

        public void UpdateApplicationVersion(Byte[] data, uint len)
        {
            if (data == null || data.Length < len || len != 2)
            {
                throw new ArgumentException("Invalid response length.");
            }
            ApplicationVersion = new Version(data[0], data[1]);
        }

        public void UpdateAddress(Byte[] data, uint len)
        {
            if (data == null || data.Length < len || len != 1)
            {
                throw new ArgumentException("Invalid response length.");
            }
            DeviceAddress = data[0];
        }

        public void UpdateBootStatusFlag(Byte[] data, uint len)
        {
            if (data == null || data.Length < len || len != 4)
            {
                throw new ArgumentException("Invalid response length.");
            }
            BootStatusFlag = Util.UInt32FromBytes(data[0], data[1], data[2], data[3]);
        }

        public void UpdateDeviceConfig(Byte[] data, uint len)
        {
            if (data == null || data.Length < len || len != DeviceConfig.ConfigLength)
            {
                throw new ArgumentException("Invalid response length.");
            }
            DeviceConfig.FromBuffer(data);
        }

        public void UpdateEepromData(Byte[] data, uint len)
        {
            if (data == null || data.Length < len || len != DeviceControllerDefinitions.EepromSize)
            {
                throw new ArgumentException("Invalid response length.");
            }

            for (int i = 0; i < DeviceControllerDefinitions.EepromSize; i++)
            {
                EepromData[i] = data[i];
            }
        }
    }

    
}
