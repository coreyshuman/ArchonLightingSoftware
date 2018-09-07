using System;
using ArchonLightingSystem.Common;
using ArchonLightingSystem.Models;

namespace ArchonLightingSystem.Models
{
    public class DeviceControllerConfig : DeviceConfigBase
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

        // non-saved data
        

        public DeviceControllerConfig()
        {
            FanSpeed = new byte[DeviceControllerDefinitions.DeviceCount];
            LedMode = new byte[DeviceControllerDefinitions.DeviceCount];
            LedSpeed = new byte[DeviceControllerDefinitions.DeviceCount];
            Unused1 = 0;
            Colors = new byte[DeviceControllerDefinitions.DeviceCount, DeviceControllerDefinitions.LedBytesPerDevice];
            
        }



        public override void FromBuffer(byte[] buffer)
        {
            int i, j;
            Crc = Util.UInt16FromBytes(buffer[0], buffer[1]);
            Length = Util.UInt16FromBytes(buffer[2], buffer[3]);
            for(i=0; i<DeviceControllerDefinitions.DeviceCount; i++)
            {
                FanSpeed[i] = buffer[i + FanSpeedOffset];
                LedMode[i] = buffer[i + LedModeOffset];
                LedSpeed[i] = buffer[i + LedSpeedOffset];
            }
            for (i = 0; i < DeviceControllerDefinitions.DeviceCount; i++)
            {
                for(j=0; j < DeviceControllerDefinitions.LedBytesPerDevice; j++)
                {
                    Colors[i, j] = buffer[LedColorOffset + i * DeviceControllerDefinitions.LedBytesPerDevice + j];
                }
            }
        }

        public override uint ToBuffer(ref byte[] buffer)
        {
            int i, j;

            // crc ignored for now

            // update length
            var lengthBytes = Util.BytesFromUInt16(this.Length);
            buffer[2] = lengthBytes.Item1;
            buffer[3] = lengthBytes.Item2;

            for (i = 0; i < DeviceControllerDefinitions.DeviceCount; i++)
            {
                buffer[i + FanSpeedOffset] = FanSpeed[i];
                buffer[i + LedModeOffset] = LedMode[i];
                buffer[i + LedSpeedOffset] = LedSpeed[i];
            }
            for (i = 0; i < DeviceControllerDefinitions.DeviceCount; i++)
            {
                for (j = 0; j < DeviceControllerDefinitions.LedBytesPerDevice; j++)
                {
                    buffer[LedColorOffset + i * DeviceControllerDefinitions.LedBytesPerDevice + j] = Colors[i, j];
                }
            }

            return Length;
        }
    }
}
