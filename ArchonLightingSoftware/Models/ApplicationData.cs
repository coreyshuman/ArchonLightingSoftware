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
        public bool EepromReadPending { get; set; }
        public bool EepromReadDone { get; set; }
        public bool EepromWritePending { get; set; }
        public bool ReadConfigPending { get; set; }
        public bool DefaultConfigPending { get; set; }
        public bool UpdateConfigPending { get; set; }
        public bool WriteConfigPending { get; set; }
        public bool ReadDebugPending { get; set; }
        public bool ResetToBootloaderPending { get; set; }
        public bool UpdateFanSpeedPending { get; set; }
        public bool SendTimePending { get; set; }
        public bool WriteLedFrame { get; set; }
        public uint EepromAddress { get; set; }
        public uint EepromLength { get; set; }
        public string Debug { get; set; }
        public Byte[] FanSpeedTargetValue { get; set; }

        public Byte[] TimeValue { get; set; }           // 3  bytes: hour, minute, second

        public byte[,] LedFrameData;

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
            WriteLedFrame = false;
            EepromAddress = 1;
            EepromLength = 1;
            Debug = "";
            TimeValue = new byte[3];
            FanSpeedTargetValue = new Byte[DeviceControllerDefinitions.DevicePerController];
        }
    }
}
