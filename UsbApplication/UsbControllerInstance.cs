using ArchonLightingSystem.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ArchonLightingSystem.UsbApplication
{
    public class UsbControllerInstance
    {
        public ApplicationData AppData { get; set; }
        public bool IsPaused { get; set; }
        public bool AppIsInitialized { get; set; }
        public bool IsDisconnected { get; set; }
        public SemaphoreSlim semaphore { get; set; }
        public UsbDevice UsbDevice {get; internal set; }
        private BackgroundWorker ReadWriteThread;

        public UsbControllerInstance(UsbDevice usbDevice)
        {
            semaphore = new SemaphoreSlim(1, 1);
            UsbDevice = usbDevice;
            //Recommend performing USB read/write operations in a separate thread.  Otherwise,
            //the Read/Write operations are effectively blocking functions and can lock up the
            //user interface if the I/O operations take a long time to complete.
            Trace.WriteLine("Initialize UsbApp Thread.");
            ReadWriteThread = new BackgroundWorker();
            ReadWriteThread.WorkerReportsProgress = true;
            ReadWriteThread.DoWork += new DoWorkEventHandler(ReadWriteThread_DoWork);
            ReadWriteThread.RunWorkerAsync();
        }

        ~UsbControllerInstance()
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
            while (true)
            {
                try
                {
                    if (!IsDisconnected && !IsPaused)
                    {
                        await UsbApp.DeviceDoWork(this);

                        Thread.Sleep(1);    //Add a small delay.  Otherwise, this while(true) loop can execute very fast and cause 
                                            //high CPU utilization, with no particular benefit to the application.

                    }
                    else
                    {
                        // device disconnected or paused
                        Thread.Sleep(1000);
                    }

                }
                catch (Exception exc)
                {
                    //Exceptions can occur during the read or write operations.  For example,
                    //exceptions may occur if for instance the USB device is physically unplugged
                    //from the host while the above read/write functions are executing.

                    //Don't need to do anything special in this case.  The application will automatically
                    //re-establish communications based on the global IsAttached boolean variable used
                    //in conjunction with the WM_DEVICECHANGE messages to dyanmically respond to Plug and Play
                    //USB connection events.
                    Trace.WriteLine($"ReadWriteThread Error: {exc.ToString()}");
                    Thread.Sleep(3000);
                }
            }
        }
    }
}
