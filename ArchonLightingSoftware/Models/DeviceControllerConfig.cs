using System;
using System.Drawing;
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
        public int ConfigLength { get; private set; }
        public UInt16 Crc { get; set; }
        public UInt16 Length { get; set; }
        public Byte[] FanSpeed { get; set; }
        public Byte[] LedMode { get; set; }
        public Byte[] LedSpeed { get; set; }
        public Byte Unused1 { get; set; }
        public Byte[,] Colors { get; set; }

        public DeviceControllerConfig()
        {
            FanSpeed = new byte[DeviceControllerDefinitions.DevicePerController];
            LedMode = new byte[DeviceControllerDefinitions.DevicePerController];
            LedSpeed = new byte[DeviceControllerDefinitions.DevicePerController];
            Unused1 = 0;
            Colors = new byte[DeviceControllerDefinitions.DevicePerController, DeviceControllerDefinitions.LedBytesPerDevice];
            
            ConfigLength = 2 + 2 + FanSpeed.Length + LedMode.Length + LedSpeed.Length + 1 + Colors.Length;
        }

        public override void FromBuffer(byte[] buffer)
        {
            int i, j;
            Crc = Util.UInt16FromBytes(buffer[0], buffer[1]);
            Length = Util.UInt16FromBytes(buffer[2], buffer[3]);

            if(Length > ConfigLength)
            {
                Logger.Write(Level.Warning, $"Invalid device config length, expect={ConfigLength} received={Length}");
                Length = (ushort)ConfigLength;
                return;
            }

            for(i=0; i<DeviceControllerDefinitions.DevicePerController; i++)
            {
                FanSpeed[i] = buffer[i + FanSpeedOffset];
                LedMode[i] = buffer[i + LedModeOffset];
                LedSpeed[i] = buffer[i + LedSpeedOffset];
            }
            for (i = 0; i < DeviceControllerDefinitions.DevicePerController; i++)
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

            for (i = 0; i < DeviceControllerDefinitions.DevicePerController; i++)
            {
                buffer[i + FanSpeedOffset] = FanSpeed[i];
                buffer[i + LedModeOffset] = LedMode[i];
                buffer[i + LedSpeedOffset] = LedSpeed[i];
            }
            for (i = 0; i < DeviceControllerDefinitions.DevicePerController; i++)
            {
                for (j = 0; j < DeviceControllerDefinitions.LedBytesPerDevice; j++)
                {
                    buffer[LedColorOffset + i * DeviceControllerDefinitions.LedBytesPerDevice + j] = Colors[i, j];
                }
            }

            return Length;
        }

        public Color GetColor(int deviceIndex, int lightIndex)
        {
            return Color.FromArgb(Colors[deviceIndex, lightIndex * 3 + 1], Colors[deviceIndex, lightIndex * 3 + 0], Colors[deviceIndex, lightIndex * 3 + 2]);
        }

        public void SetColor(int deviceIndex, int lightIndex, Color color)
        {
            Colors[deviceIndex, lightIndex * 3 + 0] = color.G; 
            Colors[deviceIndex, lightIndex * 3 + 1] = color.R;
            Colors[deviceIndex, lightIndex * 3 + 2] = color.B;
        }
    }
}
