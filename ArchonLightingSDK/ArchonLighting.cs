using System;
using System.Diagnostics;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using System.Threading;
using ArchonLightingSDKCommon;
using ArchonLightingSDKCommon.Models;

namespace ArchonLightingSDK
{
    public class ArchonLighting
    {

        public static void SendLights(SdkLightData _lightData)
        {
            try
            {
                using (MemoryMappedFile mmf = MemoryMappedFile.OpenExisting(Common.MappedFileName))
                {
                    Mutex mutex = Mutex.OpenExisting(Common.MutexName);
                    mutex.WaitOne();
                    EventWaitHandle eventWaitHandle = EventWaitHandle.OpenExisting(Common.EventName);
                    try
                    {
                        using (var accessor = mmf.CreateViewAccessor())
                        {
                            // Get size of struct
                            int size = Marshal.SizeOf(typeof(SdkLightData));
                            byte[] data = new byte[size];

                            // Initialize unmanaged memory.
                            IntPtr p = Marshal.AllocHGlobal(size);

                            // Copy struct to unmanaged memory.
                            Marshal.StructureToPtr(_lightData, p, false);

                            // Copy from unmanaged memory to byte array.
                            Marshal.Copy(p, data, 0, size);

                            // Write to memory mapped file.
                            accessor.WriteArray<byte>(0, data, 0, data.Length);
                            eventWaitHandle.Set();

                            // Free unmanaged memory.
                            Marshal.FreeHGlobal(p);
                            p = IntPtr.Zero;
                        }
                    }
                    finally
                    {
                        mutex.ReleaseMutex();
                    }
                }
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
