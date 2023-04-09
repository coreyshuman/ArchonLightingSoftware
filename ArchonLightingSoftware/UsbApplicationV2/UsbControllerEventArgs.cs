using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArchonLightingSystem.UsbApplicationV2
{
    public class UsbControllerEventArgs : EventArgs
    {
        public ReadOnlyCollection<UsbControllerDevice> Controllers { get; private set; }

        public UsbControllerEventArgs(ReadOnlyCollection<UsbControllerDevice> controllers)
        {
            this.Controllers = controllers;
        }
    }

    public class UsbControllerErrorEventArgs : UsbControllerEventArgs
    {
        public int ControllerAddress { get; set; }
        public string Message { get; set; }

        public UsbControllerErrorEventArgs(
            ReadOnlyCollection<UsbControllerDevice> controllers,
            int controllerAddress,
            string message) : base(controllers)
        {
            this.Message = message;
            this.ControllerAddress = controllerAddress;
        }
    }

    public class UsbControllersChangedEventArgs : UsbControllerEventArgs
    {
        public UsbControllersChangedEventArgs(
            ReadOnlyCollection<UsbControllerDevice> controllers) : base(controllers)
        {

        }
    }
}
