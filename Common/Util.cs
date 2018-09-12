using System;
using System.Collections.Generic;
using System.Linq;

namespace ArchonLightingSystem.Common
{
    public static class Util
    {
        public static IEnumerable<T> SliceRow<T>(this T[,] array, int row)
        {
            for (var i = array.GetLowerBound(1); i <= array.GetUpperBound(1); i++)
            {
                yield return array[row, i];
            }
        }

        public static IEnumerable<T> SliceColumn<T>(this T[,] array, int column)
        {
            for (var i = array.GetLowerBound(0); i <= array.GetUpperBound(0); i++)
            {
                yield return array[i, column];
            }
        }

        public static string[] ToStringArray(this byte[] array)
        {
            return array.Select(d => ((int)d).ToString()).ToArray();
        }

        static public UInt16 UInt16FromBytes(byte low, byte high)
        {
            return (UInt16)((UInt16)low | ((UInt16)high << 8));
        }

        static public Tuple<byte, byte> BytesFromUInt16(UInt16 value)
        {
            return new Tuple<byte, byte>((byte)(value & 0xFF), (byte)((value >> 8) & 0xFF));
        }

        static public UInt32 UInt32FromBytes(byte low, byte midlow, byte midhigh, byte high)
        {
            return (UInt32)((UInt32)low | ((UInt32)midlow << 8) | ((UInt32)midhigh << 16) | ((UInt32)high << 24));
        }

        private static UInt16[] crc_table = new UInt16[]
        {
            0x0000, 0x1021, 0x2042, 0x3063, 0x4084, 0x50a5, 0x60c6, 0x70e7,
            0x8108, 0x9129, 0xa14a, 0xb16b, 0xc18c, 0xd1ad, 0xe1ce, 0xf1ef
        };

        /// <summary>
        /// Calculate CRC for the given data and len
        /// </summary>
        /// <param name="data">data pointer</param>
        /// <param name="start">start location in array</param>
        /// <param name="len">data length</param>
        /// <returns></returns>
        public static UInt16 CalculateCrc(Byte[] data, uint start, uint len)
        {
            UInt16 i;
            UInt16 crc = 0;
            uint idx = start;

            while (len-- > 0)
            {
                i = (UInt16)((crc >> 12) ^ (data[idx] >> 4));
                crc = (UInt16)(crc_table[i & 0x0F] ^ (crc << 4));
                i = (UInt16)((crc >> 12) ^ (data[idx] >> 0));
                crc = (UInt16)(crc_table[i & 0x0F] ^ (crc << 4));
                idx++;
            }

            return (UInt16)(crc & 0xFFFF);
        }
    }
}
