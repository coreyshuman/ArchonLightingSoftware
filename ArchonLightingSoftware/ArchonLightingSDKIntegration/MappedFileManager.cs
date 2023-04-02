using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using ArchonLightingSDKCommon;
using ArchonLightingSDKCommon.Models;
using System.Security.AccessControl;
using System.Threading;
using System.ComponentModel;
using ArchonLightingSystem.Common;

namespace ArchonLightingSystem.ArchonLightingSDKIntegration
{
    public sealed class MappedFileManager
    {
        private static MemoryMappedFile lightingMappedFile;
        private static EventWaitHandle eventWaitHandle;
        private static Mutex mutex;
        private static SdkLightData lightData;

        public static MappedFileManager Instance { get; } = new MappedFileManager();

        private BackgroundWorker eventWatcherTaskWorker;
        private bool isUpdateReceived = false;

        static MappedFileManager()
        {

        }

        private MappedFileManager()
        {
            MemoryMappedFileSecurity customSecurity = new MemoryMappedFileSecurity();
            customSecurity.AddAccessRule(new AccessRule<MemoryMappedFileRights>("everyone", MemoryMappedFileRights.FullControl, AccessControlType.Allow));

            lightingMappedFile = MemoryMappedFile.CreateOrOpen(
                ArchonLightingSDKCommon.Common.MappedFileName,
                ArchonLightingSDKCommon.Common.MappedFileSize,
                MemoryMappedFileAccess.ReadWriteExecute,
                MemoryMappedFileOptions.None, customSecurity,
                System.IO.HandleInheritability.Inheritable
           );

            eventWaitHandle = new EventWaitHandle(false, EventResetMode.ManualReset, ArchonLightingSDKCommon.Common.EventName);
            mutex = new Mutex(false, ArchonLightingSDKCommon.Common.MutexName);

            lightData = new SdkLightData();
            lightData.Lights = new SdkLightColor[12];

            for (int i = 0; i < 12; i++)
            {
                lightData.Lights[i] = new SdkLightColor();
            }

            eventWatcherTaskWorker = new BackgroundWorker();
            eventWatcherTaskWorker.WorkerReportsProgress = false;
            eventWatcherTaskWorker.WorkerSupportsCancellation = true;
            eventWatcherTaskWorker.DoWork += new DoWorkEventHandler(EventWatcherTask);
            eventWatcherTaskWorker.RunWorkerAsync();
        }

        ~MappedFileManager()
        {
            if (lightingMappedFile != null)
            {
                lightingMappedFile.Dispose();
            }
            eventWatcherTaskWorker.CancelAsync();
        }

        /// <summary>
        /// Returns true if an updated has been received since the last time this flag was checked.
        /// Automatically clears flag on read.
        /// </summary>
        public bool IsUpdateReceived
        {
            get
            {
                bool retVal = isUpdateReceived;
                isUpdateReceived = false;
                return retVal;
            }
        }

        public SdkLightColor[] Lights
        {
            get
            {
                return lightData.Lights;
            }
        }

        public void ReadFile()
        {
            mutex.WaitOne();

            try
            {
                using (var accessor = lightingMappedFile.CreateViewAccessor())
                {
                    //accessor.Read<SdkLightData>(0, out data);

                    int size = Marshal.SizeOf(typeof(SdkLightData));
                    IntPtr p = Marshal.AllocHGlobal(size);
                    byte[] data = new byte[size];

                    // Read from memory mapped file.
                    accessor.ReadArray<byte>(0, data, 0, data.Length);

                    // Copy from byte array to unmanaged memory.
                    Marshal.Copy(data, 0, p, size);

                    // Copy unmanaged memory to struct.
                    lightData = (SdkLightData)Marshal.PtrToStructure(p, typeof(SdkLightData));

                    // Free unmanaged memory.
                    Marshal.FreeHGlobal(p);
                    p = IntPtr.Zero;

                    for (int i = 0; i < lightData.Lights.Count(); i++)
                    {
                        var light = lightData.Lights[i];
                    }
                }
            }
            finally
            {
                mutex.ReleaseMutex();
            }
        }

        private void EventWatcherTask(object sender, DoWorkEventArgs e)
        {
            while (!eventWatcherTaskWorker.CancellationPending)
            {
                try
                {
                    eventWaitHandle.WaitOne();
                    ReadFile();
                    isUpdateReceived = true;
                    eventWaitHandle.Reset();
                }
                catch (Exception ex)
                {
                    Logger.Write(Level.Error, $"SDK error: {ex.Message}");
                    Thread.Sleep(1000);
                }
            }
        }
    }
}
