using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ArchonLightingSystem.Common;
using ArchonLightingSystem.Interfaces;

namespace ArchonLightingSystem.Common
{
    public class PeriodicBackgroundTask : IBackgroundTask
    {
        readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private bool disposed = false;
        private int periodMilliseconds = 0;

        public int PeriodMilliseconds
        {
            get
            {
                return periodMilliseconds;
            }
            set
            {
                SetPeriod(value);
            }
        }

        public PeriodicBackgroundTask(int periodMilliseconds = 1000) 
        {
            SetPeriod(periodMilliseconds);
        }

        public Task StartTask(Func<CancellationToken, Task<BackgroundTaskResponse>> recurringAction)
        {
            if (recurringAction == null) throw new ArgumentNullException(nameof(recurringAction));

            cancellationTokenSource.Token.ThrowIfCancellationRequested();

            return Task.Run(async () =>
            {
                try
                {
                    while (!cancellationTokenSource.IsCancellationRequested)
                    {
                        await recurringAction(cancellationTokenSource.Token);

                        await Task.Delay(periodMilliseconds);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Write(Level.Error, $"PeriodicBackgroundTask Exception: {ex.Message}");
                    throw ex;
                }
                finally
                {
                    Logger.Write(Level.Trace, "PeriodicBackgroundTask Closed");
                    disposed = true;
                    cancellationTokenSource.Dispose();
                }

                return Task.CompletedTask;
            });
        }

        public void SetPeriod(int milliSeconds)
        {
            if (milliSeconds < 0) throw new ArgumentOutOfRangeException(nameof(milliSeconds));

            periodMilliseconds= milliSeconds;
        }

        public void Cancel()
        {
            if (disposed)
            {
                throw new ObjectDisposedException("PeriodicBackgroundTask disposed.");
            }

            Logger.Write(Level.Trace, "PeriodicBackgroundTask Closing");
            cancellationTokenSource.Cancel();
        }
    }
}
