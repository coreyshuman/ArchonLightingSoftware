using ArchonLightingSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArchonLightingSystem.UsbApplication
{


    public class UsbDeviceManager
    {
        public event EventHandler<UsbControllerEventArgs> UsbControllerEvent;

        private List<UsbControllerInstance> usbControllerInstances = new List<UsbControllerInstance>();
        private bool connected = false;

        public UsbDeviceManager()
        {
            UsbApp.usbDriver.UsbDriverEvent += HandleUsbDriverEvent;
        }

        public void Connect(IntPtr handle, string vid, string pid)
        {
            if(connected)
            {
                throw new Exception("Connect() can only be called once.");
            }

            connected = true;
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

        protected virtual void OnUsbDeviceEvent(UsbControllerEventArgs e)
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
                        OnUsbDeviceEvent(new UsbControllerRemovedEventArgs() { ControllerInstance = controller});
                    }
                }
                else
                {
                    var newController = new UsbControllerInstance(dev);
                    newController.ControllerReadyEvent += ControllerReadyEventHandler;
                    newController.ControllerErrorEvent += ControllerErrorEventHandler;
                    usbControllerInstances.Add(newController);
                }
            });
        }

        private void ControllerReadyEventHandler(object sender, EventArgs e)
        {
            OnUsbDeviceEvent(new UsbControllerAddedEventArgs() { ControllerInstance = (UsbControllerInstance)sender});
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
