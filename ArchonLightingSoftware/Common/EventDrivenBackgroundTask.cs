using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ArchonLightingSystem.Common
{
    public class EventDrivenBackgroundTask
    {
        readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        readonly AutoResetEvent autoResetEvent = new AutoResetEvent(false);
        private bool disposed = false;

        public void StartTask(Func<CancellationToken, Task> recurringAction)
        {
            if (recurringAction == null) throw new ArgumentException(nameof(recurringAction));

            Task.Factory.StartNew(async () =>
            {
                try
                {
                    while(!cancellationTokenSource.IsCancellationRequested)
                    {
                        Logger.Write(Level.Trace, "EventDrivenBackgroundTask Execute");
                        await recurringAction(cancellationTokenSource.Token);
                        Logger.Write(Level.Trace, "EventDrivenBackgroundTask Wait");
                        autoResetEvent.WaitOne();

                    }
                }
                catch(Exception ex)
                {
                    Logger.Write(Level.Error, ex.Message);
                }
                finally
                {
                    Logger.Write(Level.Trace, "EventDrivenBackgroundTask Closed");
                    disposed = true;
                    cancellationTokenSource.Dispose();
                    autoResetEvent.Dispose();
                }
            }, 
            cancellationTokenSource.Token, 
            TaskCreationOptions.LongRunning, 
            TaskScheduler.Default
            );
        }

        public void NextStep()
        {
            if(disposed)
            {
                throw new ObjectDisposedException("EventDrivenBackgroundTask disposed.");
            }

            Logger.Write(Level.Trace, "EventDrivenBackgroundTask Next");
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
