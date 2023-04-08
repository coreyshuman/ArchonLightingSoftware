using ArchonLightingSystem.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ArchonLightingSystem.UsbApplicationV2
{
    public class UsbControllerDevice
    {
        public ApplicationData AppData { get; set; }
        public DeviceControllerData ControllerData { get; set; }
        public int Address { get; set; }
        public bool IsPaused { get; set; } = false;
        public bool AppIsInitialized { get; set; } = false;
        public bool IsDisconnected { get; set; } = false;
        public bool IsReady { get; set; } = false;
        public SemaphoreSlim semaphore { get; set; }
        public UsbDevice UsbDevice {get; internal set; }
        private BackgroundWorker ReadWriteThread;
        private int consecutiveErrors = 0;

        public event EventHandler<EventArgs> ControllerReadyEvent;
        public event EventHandler<UsbControllerErrorEventArgs> ControllerErrorEvent;

        public UsbControllerDevice()
        {
            semaphore = new SemaphoreSlim(1, 1);
            //Recommend performing USB read/write operations in a separate thread.  Otherwise,
            //the Read/Write operations are effectively blocking functions and can lock up the
            //user interface if the I/O operations take a long time to complete.
            Trace.WriteLine("Initialize UsbApp Thread.");
            ReadWriteThread = new BackgroundWorker();
            ReadWriteThread.WorkerReportsProgress = true;
            ReadWriteThread.DoWork += new DoWorkEventHandler(ReadWriteThread_DoWork);
            ReadWriteThread.RunWorkerAsync();

            IsDisconnected = true;
        }

        ~UsbControllerDevice()
        {
            ReadWriteThread.Dispose();
        }

        public void Disconnect()
        {
            IsPaused = true;
            IsDisconnected = true;
        }

        public void Reinitialize()
        {
            IsPaused = false;
            IsDisconnected = false;
        }

        private async void ReadWriteThread_DoWork(object sender, DoWorkEventArgs e)
        {
            do
            {
                try
                {
                    if (!IsDisconnected && !IsPaused)
                    {
                        //await UsbApp.DeviceDoWork(this);

                        if (!IsReady && (this?.AppData?.DeviceControllerData?.IsInitialized ?? false))
                        {
                            IsReady = true;
                            ControllerReadyEvent?.Invoke(this, null);
                        }
                        consecutiveErrors = 0;

                        Thread.Sleep(1);    //Add a small delay.  Otherwise, this while(true) loop can execute very fast and cause 
                                            //high CPU utilization, with no particular benefit to the application.

                    }
                    else
                    {
                        // device disconnected or paused
                        Thread.Sleep(1000);
                    }

                }
                // todo - use different exception types to handle various errors
                catch (Exception exc)
                {
                    ControllerErrorEvent?.Invoke(this, new UsbControllerErrorEventArgs
                    {
                        Message = exc.Message
                    });
                    Trace.WriteLine($"ReadWriteThread Error: {exc.ToString()}");

                    // exponential retry backoff on consecutive errors
                    consecutiveErrors++;
                    Thread.Sleep(1000 * consecutiveErrors * consecutiveErrors);
                }
            } while (true);
        }
    }
}
