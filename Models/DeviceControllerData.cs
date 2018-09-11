using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArchonLightingSystem.Models;


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

        private bool isInitialized;

        public DeviceControllerData()
        {
            DeviceAddress = 0;
            MeasuredFanRpm = new UInt16[DeviceControllerDefinitions.DeviceCount];
            EepromData = new byte[DeviceControllerDefinitions.EepromSize];
            DeviceConfig = new DeviceControllerConfig();
            isInitialized = false;
        }

        public void InitializeDevice(Byte deviceAddress, Byte[] eepromData, Byte[] deviceConfig, Byte[] bootVer, Byte[] appVer)
        {
            BootloaderVersion = new Version(bootVer[0], bootVer[1]);
            ApplicationVersion = new Version(appVer[0], appVer[1]);
            DeviceAddress = deviceAddress;
            UpdateEepromData(eepromData);
            DeviceConfig.FromBuffer(deviceConfig);
            isInitialized = true;
        }

        public void UpdateEepromData(Byte[] eeprom)
        {
            for (int i = 0; i < DeviceControllerDefinitions.EepromSize; i++)
            {
                EepromData[i] = eeprom[i];
            }
        }
    }

    
}
