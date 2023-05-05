using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using ArchonLightingSystem.Models;
using ArchonLightingSystem.UsbApplicationV2;
using ArchonLightingSystem.OpenHardware;
using ArchonLightingSystem.Common;


namespace ArchonLightingSystem.Services
{
    class SensorUpdateService : ServiceBase
    {
        /// <summary>
        /// Service which periodically reads internal temperature sensors.
        /// </summary>
        /// <param name="taskPeriod">Task execution period</param>
        public SensorUpdateService(int taskPeriod)
        {
            TaskPeriod = taskPeriod;
        }

        public override void ServiceTask(UsbControllerManager controllerManager, SensorMonitorManager hardwareManager)
        {
            try
            {
                hardwareManager.UpdateReadings();
            }
            catch (Exception ex)
            {
                Logger.Write(Level.Error, $"SensorUpdateService Error: {ex}");
            }
        }
    }
}
