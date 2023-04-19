using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ArchonLightingSystem.Interfaces;

namespace ArchonLightingSystem.Common
{
    public class EventDrivenBackgroundTask : IBackgroundTask
    {
        readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        readonly AutoResetEvent autoResetEvent = new AutoResetEvent(false);
        private bool disposed = false;

        public Task StartTask(Func<CancellationToken, Task<BackgroundTaskResponse>> recurringAction)
        {
            if (recurringAction == null) throw new ArgumentException(nameof(recurringAction));

            return Task.Run( async () =>
            {
                try
                {
                    while(!cancellationTokenSource.IsCancellationRequested)
                    {
                        await recurringAction(cancellationTokenSource.Token);
                        autoResetEvent.WaitOne();
                    }
                }
                catch(Exception ex)
                {
                    Logger.Write(Level.Error, $"EventDrivenBackgroundTask Exception: {ex.Message}");
                    throw ex;
                }
                finally
                {
                    Logger.Write(Level.Trace, "EventDrivenBackgroundTask Closed");
                    disposed = true;
                    cancellationTokenSource.Dispose();
                    autoResetEvent.Dispose();
                }

                return Task.CompletedTask;
            });
        }

        public void NextStep()
        {
            if(disposed)
            {
                throw new ObjectDisposedException("EventDrivenBackgroundTask disposed.");
            }

            autoResetEvent.Set();
        }

        public void Cancel() 
        {
            if (disposed)
            {
                throw new ObjectDisposedException("EventDrivenBackgroundTask disposed.");
            }

            Logger.Write(Level.Trace, "EventDrivenBackgroundTask Closing");
            cancellationTokenSource.Cancel();
            autoResetEvent.Set();
        }
    }
}
