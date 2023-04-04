using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArchonLightingSystem.UsbApplicationV2
{
    public partial class UsbDevice
    {
        public bool IsAttached { get; set; }
        public bool IsAttachedButBroken { get; set; }
        public bool IsFound { get; set; } // used to verify device is attached when system event occurs
        public SafeFileHandle WriteHandleToUSBDevice { get; set; }
        public SafeFileHandle ReadHandleToUSBDevice { get; set; }
        public string DevicePath { get; set; }
    }
}
