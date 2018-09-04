using System;

namespace ArchonLightingSystem
{
    public abstract class DeviceObjectBase
    {
        public abstract uint ToBuffer(ref byte[] buffer);
        public abstract void FromBuffer(byte[] buffer);

        static public UInt16 UInt16FromBytes(byte low, byte high)
        {
            return (UInt16)((UInt16)low | ((UInt16)high << 8));
        }
    }
}
