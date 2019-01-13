using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArchonLightingSystem.Models;

namespace ArchonLightingSystem.Models
{
    public class ApplicationData
    {
        public bool EepromReadPending;
        public bool EepromReadDone;
        public bool EepromWritePending;
        public bool ReadConfigPending;
        public bool DefaultConfigPending;
        public bool UpdateConfigPending;
        public bool WriteConfigPending;
        public bool ReadDebugPending;
        public bool ResetToBootloaderPending;
        public bool UpdateFanSpeedPending;
        public uint EepromAddress;
        public uint EepromLength;
        public string Debug;

        public DeviceControllerData DeviceControllerData { get; set; }
        

        public ApplicationData()
        {
            EepromReadPending = false;
            EepromReadDone = false;
            EepromWritePending = false;
            ReadDebugPending = false;
            DefaultConfigPending = false;
            UpdateConfigPending = false;
            WriteConfigPending = false;
            ResetToBootloaderPending = false;
            UpdateFanSpeedPending = false;
            EepromAddress = 1;
            EepromLength = 1;
            Debug = "";
            DeviceControllerData = new DeviceControllerData();
        }
    }
}
