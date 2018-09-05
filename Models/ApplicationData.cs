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
        public bool UpdateConfigPending;
        public bool WriteConfigPending;
        public bool ReadDebug;
        public uint EepromAddress;
        public uint EepromLength;

        public DeviceControllerData DeviceControllerData { get; set; }
        

        public ApplicationData()
        {
            EepromReadPending = false;
            EepromReadDone = false;
            EepromWritePending = false;
            ReadDebug = false;
            EepromAddress = 1;
            EepromLength = 1;
            DeviceControllerData = null;
        }
    }
}
