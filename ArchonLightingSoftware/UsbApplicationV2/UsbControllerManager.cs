using ArchonLightingSystem.Common;
using ArchonLightingSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArchonLightingSystem.UsbApplication;

namespace ArchonLightingSystem.UsbApplicationV2
{


    public class UsbControllerManager
    {
        public event EventHandler<UsbControllerEventArgs> UsbControllerEvent;

        private UsbDeviceManager usbDeviceManager = new UsbDeviceManager();
        private List<UsbControllerDevice> usbControllerInstances = new List<UsbControllerDevice>();
        private bool isRegistered = false;

        public UsbControllerManager()
        {
            for(int i = 0; i < DeviceControllerDefinitions.MaxControllers; i++)
            {
                usbControllerInstances.Add(new UsbControllerDevice() { Address = i });
            }
        }

        public void Register(IntPtr handle, string vid, string pid)
        {
            if (isRegistered)
            {
                throw new Exception("Register() can only be called once.");
            }

            isRegistered = true;

            Logger.Write(Level.Debug, $"Register UsbControllerManager vid={vid} pid={pid}");

            usbDeviceManager.UsbDriverEvent += HandleUsbDriverEvent;
            usbDeviceManager.RegisterEventHandler(handle);
            usbDeviceManager.RegisterUsbDevice(vid, pid);
            
            
        }

        public int DeviceCount
        {
            get
            {
                return usbControllerInstances.Count;
            }
        }

        public IList<UsbControllerDevice> UsbDevices
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

        private async Task<bool> ConnectUsbControllerAsync(UsbDevice device)
        {
            
            Logger.Write(Level.Debug, $"Connect device {device.ShortName}");
            //if (device.ShortName != "b&172fe167") return false;
            if(await UsbApp.GetDeviceInitializationAsync(device))
            {
                Logger.Write(Level.Debug, $"Can initialize {device.ShortName}");
            }


            
            return true;
        }

        private async Task DisconnectUsbControllerAsync(UsbDevice device)
        {
            Logger.Write(Level.Debug, $"Disconnect device {device.ShortName}");
            device.Cancel();
            await device.WaitAsync();
            device.Release();
            Logger.Write(Level.Debug, $"Disconnected {device.ShortName}");
        }

        private async void HandleUsbDriverEvent(object sender, UsbDeviceEventArgs e)
        {
            if(e.EventCount > 0)
            {
                var connectTasks = e.ConnectedDevices.Select(device => ConnectUsbControllerAsync(device));

                //e.ConnectedDevices.ForEach(device => ConnectUsbControllerAsync(device));

                var disconnectTasks = e.DisconnectedDevices.Select(device => DisconnectUsbControllerAsync(device));

                //e.DisconnectedDevices.ForEach(device => DisconnectUsbControllerAsync(device));
                await Task.WhenAll(connectTasks);
                await Task.WhenAll(disconnectTasks);
            }



            /*
            e.Devices.ForEach(dev =>
            {
                var controller = usbControllerInstances.Where(ctrl => ctrl.UsbDevice == dev).FirstOrDefault();
                if(controller != null)
                {
                    if(!dev.IsAttached)
                    {
                        usbControllerInstances.Remove(controller);
                        OnUsbDeviceEvent(new UsbControllerRemovedEventArgs() { ControllerInstance = controller});
                    }
                }
                else
                {
                    var newController = new UsbControllerDevice(dev);
                    newController.ControllerReadyEvent += ControllerReadyEventHandler;
                    newController.ControllerErrorEvent += ControllerErrorEventHandler;
                    usbControllerInstances.Add(newController);
                }
            });
            */
        }

        private void ControllerReadyEventHandler(object sender, EventArgs e)
        {
            OnUsbDeviceEvent(new UsbControllerAddedEventArgs() { ControllerInstance = (UsbControllerDevice)sender});
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
            usbControllerInstances.ForEach((dev) =>
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
            });
        }
    }
}
