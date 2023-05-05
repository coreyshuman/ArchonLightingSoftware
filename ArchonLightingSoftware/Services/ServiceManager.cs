using ArchonLightingSystem.Models;
using ArchonLightingSystem.OpenHardware;
using System.Collections.Generic;
using ArchonLightingSystem.UsbApplicationV2;
using ArchonLightingSystem.Common;
using ArchonLightingSystem.Interfaces;

namespace ArchonLightingSystem.Services
{
    class ServiceManager
    {
        private readonly List<IService> services = new List<IService>();

        public ServiceManager()
        {
            services.Add(new SensorUpdateService(200));
            services.Add(new FanControllerService(500));
            services.Add(new LightControllerService(33));
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
