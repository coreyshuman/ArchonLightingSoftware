using ArchonLightingSystem.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ArchonLightingSystem.Interfaces
{
    public interface IBackgroundTask
    {
        Task StartTask(Func<CancellationToken, Task<BackgroundTaskResponse>> recurringAction);
        void Cancel();
    }
}
