using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ArchonLightingSystem.Interfaces;
using System.Diagnostics;

namespace ArchonLightingSystem.UsbApplicationV2
{
    public partial class UsbDevice : IUsbDevice
    {
        public bool IsAttached { get; set; }
        public bool IsAttachedButBroken { get; set; }
        public bool IsFound { get; set; } // used to verify device is attached when system event occurs
        public SafeFileHandle WriteHandleToUSBDevice { get; set; }
        public SafeFileHandle ReadHandleToUSBDevice { get; set; }
        public string DevicePath { get; set; }
        public string ShortName
        {
            get
            {
                return DevicePath.Substring(26, 10);
            }

        }

        private SemaphoreSlim semaphore;
        private object listLock;
        private List<CancellationTokenSource> cancellationTokenSources;

        public UsbDevice()
        {
            semaphore = new SemaphoreSlim(1, 1);
            listLock = new object();
            cancellationTokenSources = new List<CancellationTokenSource>();
        }

        public void Wait()
        {
            semaphore.Wait();
        }

        public void Wait(CancellationTokenSource cancellationTokenSource)
        {
            semaphore.Wait(cancellationTokenSource.Token);
        }

        public Task WaitAsync()
        {
            return semaphore.WaitAsync();
        }

        public Task WaitAsync(CancellationTokenSource cancellationTokenSource)
        {
            return semaphore.WaitAsync(cancellationTokenSource.Token);
        }

        public void Release()
        {
            semaphore.Release();
        }

        public void Release(CancellationTokenSource cancellationTokenSource)
        {
            try
            {
                semaphore.Release();
            }
            finally
            {
                cancellationTokenSource.Dispose();
                lock (listLock)
                {
                    cancellationTokenSources.Remove(cancellationTokenSource);
                }
            }
        }

        public CancellationTokenSource GetCancellationToken()
        {
            CancellationTokenSource tokenSource = new CancellationTokenSource();
            lock (listLock)
            {
                cancellationTokenSources.Add(tokenSource);
            }
            return tokenSource;
        }

        public void Cancel()
        {
            lock (listLock)
            {
                cancellationTokenSources.ForEach(cts => cts.Cancel());
                cancellationTokenSources.ForEach(cts => cts.Dispose());
                cancellationTokenSources.Clear();
            }

        }
    }
}
