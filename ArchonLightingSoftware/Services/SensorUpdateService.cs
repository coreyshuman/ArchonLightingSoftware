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
    class SensorUpdateService : ControllerServiceBase
    {
        /// <summary>
        /// Service which periodically reads internal temperature sensors.
        /// </summary>
        /// <param name="taskFrequency">Task execution frequency in Hz. One controller updated per execution.</param>
        public SensorUpdateService(int taskFrequency)
        {
            TaskFrequency = taskFrequency;
        }

        public override void ServiceTask(UsbControllerDevice usbControllerDevice, SensorMonitorManager hardwareManager)
        {
            try
            {
                hardwareManager.UpdateReadings();
            }
            catch (Exception ex)
            {
                Logger.Write(Level.Error, $"HardwareUpdateService Error: {ex}");
            }
        }
    }
}
