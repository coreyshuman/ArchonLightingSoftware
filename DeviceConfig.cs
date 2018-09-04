using System;

namespace ArchonLightingSystem
{
    public class DeviceConfig : DeviceObjectBase
    {
        public const uint FanSpeedOffset = 4;
        public const uint LedModeOffset = 9;
        public const uint LedSpeedOffset = 14;
        public const uint LedColorOffset = 20;
        public UInt16 Crc { get; set; }
        public UInt16 Length { get; set; }
        public Byte[] FanSpeed { get; set; }
        public Byte[] LedMode { get; set; }
        public Byte[] LedSpeed { get; set; }
        public Byte Unused1 { get; set; }
        public Byte[,] Colors { get; set; }

        public DeviceConfig()
        {
            FanSpeed = new byte[DeviceController.DeviceCount];
            LedMode = new byte[DeviceController.DeviceCount];
            LedSpeed = new byte[DeviceController.DeviceCount];
            Unused1 = 0;
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
                LedSpeed[i] = buffer[i + LedSpeedOffset];
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
                buffer[i + LedSpeedOffset] = LedSpeed[i];
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
