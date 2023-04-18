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
        public bool IsActive {
            get { return Settings.AlertOnDisconnect || hasConnected; } 
        }
        public bool IsConnected { get; private set; }
        public int FailedHealthCheckCount { get; set; }


        // old flags
        public bool IsPaused { get; set; } = false;
        public bool AppIsInitialized { get; set; } = false;
        public bool IsDisconnected { get; set; } = false;
        public bool IsReady { get; set; } = false;
        public SemaphoreSlim semaphore { get; set; }


        public UsbDevice UsbDevice {get; internal set; }
        public ControllerSettings Settings { get; internal set; }

        private bool hasConnected = false;
        private BackgroundWorker ReadWriteThread;
        private int consecutiveErrors = 0;

        public UsbControllerDevice(int address, ControllerSettings settings)
        {
            semaphore = new SemaphoreSlim(1, 1);

            Trace.WriteLine("Initialize UsbApp Thread.");
            ReadWriteThread = new BackgroundWorker();
            ReadWriteThread.WorkerReportsProgress = true;
            ReadWriteThread.DoWork += new DoWorkEventHandler(ReadWriteThread_DoWork);
            ReadWriteThread.RunWorkerAsync();

            IsDisconnected = true;
            IsConnected = false;

            AppData = new ApplicationData();
            ControllerData = new DeviceControllerData();

            Address = address;
            Settings = settings;
        }

        ~UsbControllerDevice()
        {
            ReadWriteThread.Dispose();
        }

        public void Disconnect()
        {
            UsbDevice.Cancel();
            UsbDevice = null;
            IsDisconnected = true;
            IsConnected = false;
            ControllerData.Reset();
        }

        public void Connect(UsbDevice usbDevice, DeviceControllerData controllerData)
        {
            UsbDevice = usbDevice;
            ControllerData = controllerData;
            IsDisconnected = false;
            hasConnected = true;
            IsConnected = true;
        }

        private async void ReadWriteThread_DoWork(object sender, DoWorkEventArgs e)
        {
            do
            {
                try
                {
                    if (!IsDisconnected && !IsPaused)
                    {
                        await UsbApp.DeviceDoWork(this);

                        consecutiveErrors = 0;

                        Thread.Sleep(100);

                    }
                    else
                    {
                        Thread.Sleep(1000);
                    }

                }
                catch (Exception exc)
                {
                    /*
                    ControllerErrorEvent?.Invoke(this, new UsbControllerErrorEventArgs
                    {
                        Message = exc.Message
                    });
                    */
                    Trace.WriteLine($"ReadWriteThread Error: {exc.ToString()}");

                    // exponential retry backoff on consecutive errors
                    consecutiveErrors++;
                    Thread.Sleep(1000 * consecutiveErrors * consecutiveErrors);
                }
            } while (true);
        }
    }
}
