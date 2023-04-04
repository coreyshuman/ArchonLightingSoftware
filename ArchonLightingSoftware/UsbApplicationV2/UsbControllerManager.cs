using ArchonLightingSystem.Common;
using ArchonLightingSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArchonLightingSystem.Common;
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

            usbDeviceManager.UsbDriverEvent += HandleUsbDriverEvent;
            usbDeviceManager.RegisterEventHandler(handle);
            usbDeviceManager.RegisterUsbDevice(vid, pid);
            
            Logger.Write(Level.Debug, $"Register UsbControllerManager vid={vid} pid={pid}");
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

        private void HandleUsbDriverEvent(object sender, UsbDeviceEventArgs e)
        {
            if(e.EventCount > 0)
            {
                e.ConnectedDevices.ForEach(device =>
                {
                    Logger.Write(Level.Debug, $"Connect device {device.DevicePath}");
                });

                e.DisconnectedDevices.ForEach(device =>
                {
                    Logger.Write(Level.Debug, $"Disconnect device {device.DevicePath}");
                });
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
