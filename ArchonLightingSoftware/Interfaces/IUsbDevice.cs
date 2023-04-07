using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ArchonLightingSystem.Interfaces
{
    public interface IUsbDevice
    {
        string ShortName { get; }
        bool IsAttached { get; }
        SafeFileHandle WriteHandleToUSBDevice { get; }
        SafeFileHandle ReadHandleToUSBDevice { get; }

        void Wait();
        void Wait(CancellationTokenSource cancellationTokenSource);
        Task WaitAsync();
        Task WaitAsync(CancellationTokenSource cancellationTokenSource);
        void Release();
        void Release(CancellationTokenSource cancellationTokenSource);
        CancellationTokenSource GetCancellationToken();
        void Cancel();
    }
}
