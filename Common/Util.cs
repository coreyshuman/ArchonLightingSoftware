using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace ArchonLightingSystem.Common
{
    public static class Util
    {
        private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

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

        static public void CopyObjectProperties<T>(T destination, T source, string[] propertyNames = null)
        {
            PropertyInfo[] properties = typeof(T).GetProperties();
            var propertiesToUpdate = properties.Where(p => p.CanWrite && (propertyNames == null || propertyNames.Contains(p.Name)));
            foreach (PropertyInfo property in propertiesToUpdate)
            {
                property.SetValue(destination, property.GetValue(source));
            }
        }

        // <summary>
        // Get the name of a static or instance property from a property access lambda.
        // </summary>
        // <typeparam name="T">Type of the property</typeparam>
        // <param name="propertyLambda">lambda expression of the form: '() => Class.Property' or '() => object.Property'</param>
        // <returns>The name of the property</returns>
        static public string GetPropertyName<T>(Expression<Func<T>> propertyLambda)
        {
            var me = propertyLambda.Body as MemberExpression;

            if (me == null)
            {
                throw new ArgumentException("You must pass a lambda of the form: '() => Class.Property' or '() => object.Property'");
            }

            return me.Member.Name;
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

            // allow arithmetic overflow
            unchecked
            {
                while (len-- > 0)
                {
                    i = (UInt16)((crc >> 12) ^ (data[idx] >> 4));
                    crc = (UInt16)(crc_table[i & 0x0F] ^ (crc << 4));
                    i = (UInt16)((crc >> 12) ^ (data[idx] >> 0));
                    crc = (UInt16)(crc_table[i & 0x0F] ^ (crc << 4));
                    idx++;
                }
            }
            

            return (UInt16)(crc & 0xFFFF);
        }

        public static void CopyArray(ref byte[] dest, uint destStart, ref byte[] src, uint srcStart, int length)
        {
            for (int i = 0; i < length; i++)
            {
                dest[i + destStart] = src[i + srcStart];
            }
        }

        public static long GetCurrentUnixTimestampMillis()
        {
            DateTime localDateTime, univDateTime;
            localDateTime = DateTime.Now;
            univDateTime = localDateTime.ToUniversalTime();
            return (long)(univDateTime - UnixEpoch).TotalMilliseconds;
        }

        public static Task WhenAllTasks(IEnumerable<Task> tasks)
        {
            Task allTasks = Task.WhenAll(tasks);
            try
            {
                allTasks.Wait();
            }
            catch (Exception ex)
            {
                Trace.TraceError($"WhenAllTasks Exception: {ex.ToString()}");
            }

            if (allTasks.Exception != null)
            {
                throw allTasks.Exception;
            }
            return allTasks;
        }
    }
}
