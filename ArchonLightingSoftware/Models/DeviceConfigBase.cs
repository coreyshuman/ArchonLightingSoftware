using System;

namespace ArchonLightingSystem.Models
{
    public abstract class DeviceConfigBase
    {
        public abstract uint ToBuffer(ref byte[] buffer);
        public abstract void FromBuffer(byte[] buffer);
    }
}
