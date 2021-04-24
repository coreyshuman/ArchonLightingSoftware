using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using ArchonLightingSystem.Models;
using ArchonLightingSystem.UsbApplication;
using ArchonLightingSystem.OpenHardware;


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

        public override void ServiceTask(ApplicationData applicationData, ControllerSettings controllerSettings, SensorMonitorManager hardwareManager)
        {
            try
            {
                //var test = ArchonLightingSDKIntegration.AIDA64Integration.ReadData();
                hardwareManager.UpdateReadings();
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"HardwareUpdateService Error: {ex.ToString()}");
            }
        }
    }
}
