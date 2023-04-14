using ArchonLightingSystem.Common;
using ArchonLightingSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArchonLightingSystem.UsbApplication;
using System.Threading;
using System.Security.Cryptography;
using System.Timers;
using System.Collections.ObjectModel;

namespace ArchonLightingSystem.UsbApplicationV2
{
    public class UsbControllerManager
    {
        public event EventHandler<UsbControllerEventArgs> UsbControllerEvent;

        private UsbDeviceManager usbDeviceManager = new UsbDeviceManager();
        private ReadOnlyCollection<UsbControllerDevice> usbControllerInstances;
        private bool isRegistered = false;

        private EventDrivenBackgroundTask usbEventBackgroundTask = null;
        private PeriodicBackgroundTask periodicBackgroundTask = null;
        private List<UsbDevice> pendingConnectedDevices = new List<UsbDevice>();
        private object connectedDevicesLock = new object();
        private List<UsbDevice> pendingDisconnectedDevices = new List<UsbDevice>();
        private object disconnectedDevicesLock = new object();
        private List<UsbDevice> pendingRetryDevices = new List<UsbDevice>();
        private SemaphoreSlim pendingSemaphore = new SemaphoreSlim(1, 1);

        private async Task<BackgroundTaskResponse> processPendingDevices(CancellationToken token)
        {
            List<Task> deviceTasks = new List<Task>();

            if(!await pendingSemaphore.WaitAsync(200))
            {
                return BackgroundTaskResponse.Continue;
            }

            try
            {
                lock (connectedDevicesLock)
                {
                    lock (disconnectedDevicesLock)
                    {
                        pendingDisconnectedDevices.ForEach(device =>
                        {
                            pendingConnectedDevices.Remove(device);

                            pendingRetryDevices.Remove(device);

                            deviceTasks.Add(DisconnectUsbControllerAsync(device));
                        });
                        pendingDisconnectedDevices.Clear();
                    }
                    pendingConnectedDevices.ForEach(device =>
                    {

                        pendingRetryDevices.Remove(device);

                        deviceTasks.Add(ConnectUsbControllerAsync(device));
                    });
                    pendingConnectedDevices.Clear();
                }

                pendingRetryDevices.ForEach(device =>
                {
                    Logger.Write(Level.Debug, $"Do retry device {device.ShortName}");
                    deviceTasks.Add(ConnectUsbControllerAsync(device));
                });

                pendingRetryDevices.Clear();
            }
            finally 
            { 
                pendingSemaphore.Release(); 
            }

            try
            {
                await Task.WhenAll(deviceTasks);
            }
            finally
            {
                OnUsbDeviceEvent(new UsbControllersChangedEventArgs(usbControllerInstances));
            }

            return BackgroundTaskResponse.Continue;
        }

        public UsbControllerManager()
        {
            var controllers = new List<UsbControllerDevice>();

            for(int i = 0; i < DeviceControllerDefinitions.MaxControllers; i++)
            {
                controllers.Add(new UsbControllerDevice() { Address = i });
            }

            usbControllerInstances = controllers.AsReadOnly();
        }

        public void Register(IntPtr handle, string vid, string pid)
        {
            if (isRegistered)
            {
                throw new Exception("Register() can only be called once.");
            }

            isRegistered = true;

            Logger.Write(Level.Debug, $"Register UsbControllerManager vid={vid} pid={pid}");

            usbEventBackgroundTask = new EventDrivenBackgroundTask();
            periodicBackgroundTask = new PeriodicBackgroundTask(5000);

            usbDeviceManager.UsbDriverEvent += HandleUsbDriverEvent;
            usbDeviceManager.RegisterEventHandler(handle);
            usbDeviceManager.RegisterUsbDevice(vid, pid);

            usbEventBackgroundTask.StartTask(processPendingDevices);
            periodicBackgroundTask.StartTask(processPeriodicTasks);
            
            /*
            // start controller period check thread
            Task.Factory.StartNew(async () =>
            {
                while (true)
                {
                    usbEventBackgroundTask.NextStep();
                    await Task.Delay(5000);
                }

            }, new CancellationToken(), TaskCreationOptions.LongRunning, TaskScheduler.Default);
            */
        }

        private Task<BackgroundTaskResponse> processPeriodicTasks(CancellationToken cancellationToken)
        {
            // trigger usb event task to perform connection retries
            usbEventBackgroundTask.NextStep();

            return Task.FromResult(BackgroundTaskResponse.Continue);
        }

        public int ControllerCount
        {
            get
            {
                return usbControllerInstances.Count;
            }
        }

        public ReadOnlyCollection<UsbControllerDevice> Controllers
        {
            get
            {
                return usbControllerInstances;
            }
        }

        public UsbControllerDevice GetDevice(int deviceIdx)
        {
            if (deviceIdx >= usbControllerInstances.Count)
            {
                return null;
            }
            return usbControllerInstances[deviceIdx];
        }

        public void HandleWindowEvent(ref System.Windows.Forms.Message m)
        {
            usbDeviceManager.HandleWindowEvent(ref m);
        }

        protected virtual void OnUsbDeviceEvent(UsbControllerEventArgs e)
        {
            UsbControllerEvent?.Invoke(this, e);
        }

        private async Task ConnectUsbControllerAsync(UsbDevice device)
        {
            Logger.Write(Level.Debug, $"Connecting device {device.ShortName}");

            var deviceControllerConfig = await UsbApp.GetDeviceInitializationAsync(device);

            if (deviceControllerConfig == null)
            {
                Logger.Write(Level.Error, $"Failed to initialize {device.ShortName}");
                if(await pendingSemaphore.WaitAsync(300))
                { 
                    try
                    {
                        Logger.Write(Level.Debug, $"Pending retry device {device.ShortName}");
                        pendingRetryDevices.Add(device);
                    }
                    finally
                    {
                        pendingSemaphore.Release();
                    }
                }
                return;
            }

            var deviceInstance = usbControllerInstances.Where(d => d.Address == deviceControllerConfig.DeviceAddress).FirstOrDefault();
            if (deviceInstance == null)
            {
                Logger.Write(Level.Error, $"Cannot connect device, invalid address: {deviceControllerConfig.DeviceAddress}");
                return;
            }
            
            if(!deviceInstance.IsDisconnected)
            {
                Logger.Write(Level.Error, $"Cannot connect device, already initialized: {deviceControllerConfig.DeviceAddress}");
                return;
            }

            deviceInstance.ControllerData = deviceControllerConfig;
            deviceInstance.UsbDevice = device;
            deviceInstance.IsDisconnected = false;

            var d = deviceControllerConfig;
            Logger.Write(Level.Information, $"Connected device {device.ShortName} address: {d.DeviceAddress} firmware: {d.ApplicationVersion} bootloader: {d.BootloaderVersion}");
        }

        private async Task DisconnectUsbControllerAsync(UsbDevice device)
        {
            Logger.Write(Level.Debug, $"Disconnect device {device.ShortName}");
            device.Cancel();
            await device.WaitAsync();
            device.Release();

            foreach(var deviceInstance in usbControllerInstances)
            {
                if (deviceInstance.UsbDevice == device)
                {
                    deviceInstance.UsbDevice = null;
                    deviceInstance.IsDisconnected = true;
                    Logger.Write(Level.Information, $"Disconnected device {device.ShortName} address: {deviceInstance.Address}");
                }
            }
        }

        private void HandleUsbDriverEvent(object sender, UsbDeviceEventArgs e)
        {
            if(e.EventCount > 0)
            {
                Logger.Write(Level.Trace, $"UsbControllerManager UsbEvent");
                lock (connectedDevicesLock)
                {
                    pendingConnectedDevices.AddRange(e.ConnectedDevices);
                }

                lock(disconnectedDevicesLock)
                {
                    pendingDisconnectedDevices.AddRange(e.DisconnectedDevices);
                }

                usbEventBackgroundTask?.NextStep();
                Logger.Write(Level.Trace, $"UsbControllerManager UsbEvent Done");
            }
        }

        private void ControllerReadyEventHandler(object sender, EventArgs e)
        {
            //OnUsbDeviceEvent(new UsbControllerAddedEventArgs() { ControllerInstance = (UsbControllerDevice)sender});
        }

        private void ControllerErrorEventHandler(object sender, UsbControllerErrorEventArgs e)
        {
            OnUsbDeviceEvent(e);
        }

        public ApplicationData GetAppData(int deviceIdx)
        {
            var device = GetDevice(deviceIdx);
            return device?.AppData;
        }

        public void ClearDevices()
        {
            foreach(var dev in usbControllerInstances)
            {
                try
                {
                    dev.semaphore.Wait(1000);
                    dev.AppIsInitialized = false;
                    dev.AppData = null;
                    // todo, how should this be handled?
                    //DetachDevice(dev);
                    dev.Reinitialize();
                }
                finally
                {
                    dev.semaphore.Release();
                }
            }
        }
    }
}
