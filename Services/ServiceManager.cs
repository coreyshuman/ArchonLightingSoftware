﻿using ArchonLightingSystem.Models;
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
        }

        public void StartServices(UserSettings userSettings, UsbApp usbApp, HardwareManager hardwareManager)
        {
            services.ForEach(service =>
            {
                service.BeginService(userSettings, usbApp, hardwareManager);
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
