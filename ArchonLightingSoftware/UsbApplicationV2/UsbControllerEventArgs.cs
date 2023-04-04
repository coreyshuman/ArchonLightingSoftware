using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArchonLightingSystem.UsbApplicationV2
{
    public class UsbControllerEventArgs : EventArgs
    {
        public UsbControllerDevice ControllerInstance { get; internal set; }
    }

    public class UsbControllerErrorEventArgs : UsbControllerEventArgs
    {
        public int ControllerAddress
        {
            get
            {
                return ControllerInstance?.AppData?.DeviceControllerData?.DeviceAddress ?? -1;
            }
        }
        public string Message { get; set; }
    }

    public class UsbControllerAddedEventArgs : UsbControllerEventArgs
    {

    }

    public class UsbControllerRemovedEventArgs : UsbControllerEventArgs
    {

    }
}
