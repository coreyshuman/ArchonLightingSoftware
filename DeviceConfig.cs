using System;
using System.Collections.Generic;
using System.Text;

namespace ArchonLightingSystem
{
    public class DeviceConfig : DeviceObjectBase
    {
        public const uint FanSpeedOffset = 4;
        public const uint LedModeOffset = 9;
        public const uint LedColorOffset = 14;
        public UInt16 Crc;
        public UInt16 Length;
        public Byte[] FanSpeed;
        public Byte[] LedMode;
        public Byte[,] Colors;

        public DeviceConfig()
        {
            FanSpeed = new byte[DeviceController.DeviceCount];
            LedMode = new byte[DeviceController.DeviceCount];
            Colors = new byte[DeviceController.DeviceCount, DeviceController.LedBytesPerDevice];
        }

        public override void FromBuffer(byte[] buffer)
        {
            int i, j;
            Crc = UInt16FromBytes(buffer[0], buffer[1]);
            Length = UInt16FromBytes(buffer[2], buffer[3]);
            for(i=0; i<DeviceController.DeviceCount; i++)
            {
                FanSpeed[i] = buffer[i + FanSpeedOffset];
                LedMode[i] = buffer[i + LedModeOffset];
            }
            for (i = 0; i < DeviceController.DeviceCount; i++)
            {
                for(j=0; j < DeviceController.LedBytesPerDevice; j++)
                {
                    Colors[i, j] = buffer[LedColorOffset + i * DeviceController.DeviceCount + j];
                }
            }
        }

        public override uint ToBuffer(ref byte[] buffer)
        {
            int i, j;
            // todo - length and crc. for now discarded by firmware.

            for (i = 0; i < DeviceController.DeviceCount; i++)
            {
                buffer[i + FanSpeedOffset] = FanSpeed[i];
                buffer[i + LedModeOffset] = LedMode[i];
            }
            for (i = 0; i < DeviceController.DeviceCount; i++)
            {
                for (j = 0; j < DeviceController.LedBytesPerDevice; j++)
                {
                    buffer[LedColorOffset + i * DeviceController.LedBytesPerDevice + j] = Colors[i, j];
                }
            }

            return Length;
        }
    }
}
