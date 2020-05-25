using ArchonLightingSystem.Models;
using ArchonLightingSystem.UsbApplication;
using ArchonLightingSystem.OpenHardware;
using System.Collections.Generic;

namespace ArchonLightingSystem.Services
{
    class ServiceManager
    {
        private readonly List<ControllerServiceBase> services = new List<ControllerServiceBase>();

        public ServiceManager()
        {
            services.Add(new HardwareUpdateService(4));
            services.Add(new FanControllerService(2));
            services.Add(new LightControllerService(1));
            services.Add(new TimeServiceController(2000));
        }

        public void StartServices(UserSettings userSettings, UsbDeviceManager usbDeviceManager, HardwareManager hardwareManager)
        {
            services.ForEach(service =>
            {
                service.BeginService(userSettings, usbDeviceManager, hardwareManager);
            });
        }

        public void StopServices()
        {
            services.ForEach(service =>
            {
                service.CloseService();
            });
        }
    }
}
