using ArchonLightingSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArchonLightingSystem.UsbApplication
{
    public class UsbDeviceEventArgs : EventArgs
    {
        public bool DeviceAdded { get; set; }
        public bool DeviceRemoved { get; set; }
        public UsbControllerInstance ControllerInstance { get; internal set; }
    }

    public class UsbDeviceManager
    {
        public event EventHandler<UsbDeviceEventArgs> UsbControllerEvent;

        private List<UsbControllerInstance> usbControllerInstances = new List<UsbControllerInstance>();

        public UsbDeviceManager(IntPtr handle, string vid, string pid)
        {
            UsbApp.usbDriver.UsbDriverEvent += HandleUsbDriverEvent;
            UsbApp.Register(handle, vid, pid);
        }

        public int DeviceCount
        {
            get
            {
                return usbControllerInstances.Count;
            }
        }

        public IList<UsbControllerInstance> UsbDevices
        {
            get
            {
                return usbControllerInstances;
            }
        }

        public UsbControllerInstance GetDevice(int deviceIdx)
        {
            if (deviceIdx >= usbControllerInstances.Count)
            {
                return null;
            }
            return usbControllerInstances[deviceIdx];
        }

        public void HandleWindowEvent(ref System.Windows.Forms.Message m)
        {
            UsbApp.HandleWindowEvent(ref m);
        }

        protected virtual void OnUsbDeviceEvent(UsbDeviceEventArgs e)
        {
            UsbControllerEvent?.Invoke(this, e);
        }

        private void HandleUsbDriverEvent(object sender, UsbDriverEventArgs e)
        {
            e.Devices.ForEach(dev =>
            {
                var controller = usbControllerInstances.Where(ctrl => ctrl.UsbDevice == dev).FirstOrDefault();
                if(controller != null)
                {
                    if(!dev.IsAttached)
                    {
                        usbControllerInstances.Remove(controller);
                        OnUsbDeviceEvent(new UsbDeviceEventArgs() { ControllerInstance = controller, DeviceAdded = false, DeviceRemoved = true });
                    }
                }
                else
                {
                    var newController = new UsbControllerInstance(dev);
                    usbControllerInstances.Add(newController);
                    OnUsbDeviceEvent(new UsbDeviceEventArgs() { ControllerInstance = newController, DeviceAdded = true, DeviceRemoved = false });
                }
            });
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
