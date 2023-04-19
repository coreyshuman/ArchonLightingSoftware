using ArchonLightingSystem.Models;
using ArchonLightingSystem.OpenHardware;
using System.Collections.Generic;
using ArchonLightingSystem.UsbApplicationV2;
using ArchonLightingSystem.Common;

namespace ArchonLightingSystem.Services
{
    class ServiceManager
    {
        private readonly List<ControllerServiceBase> services = new List<ControllerServiceBase>();

        public ServiceManager()
        {
            //services.Add(new SensorUpdateService(5));
            services.Add(new FanControllerService(2));
            services.Add(new LightControllerService(1000));
            services.Add(new TimeServiceController(30000));
        }

        public void StartServices(UsbControllerManager usbControllerManager, SensorMonitorManager hardwareManager)
        {
            services.ForEach(service =>
            {
                service.BeginService(usbControllerManager, hardwareManager);
            });
        }

        public void StopServices()
        {
            services.ForEach(service =>
            {
                service.CloseService();
            });
        }

        public void PauseServices(bool pause)
        {
            Logger.Write(Level.Trace, $"{(pause ? "Pause" : "Unpause")} services");
            services.ForEach(service =>
            {
                service.Pause(pause);
            });
        }
    }
}
