using ArchonLightingSystem.Common;
using ArchonLightingSystem.Models;
using ArchonLightingSystem.OpenHardware;
using ArchonLightingSystem.UsbApplicationV2;
using System;

namespace ArchonLightingSystem.Services
{
    class TimeServiceController : ControllerServiceBase
    {
        /// <summary>
        /// Service which periodically updates time on controller units.
        /// </summary>
        /// <param name="taskPeriod">Task execution period</param>
        public TimeServiceController(int taskPeriod)
        {
            TaskPeriod = taskPeriod;
        }

        public override void ServiceTask(UsbControllerDevice usbControllerDevice, SensorMonitorManager hardwareManager)
        {
            try
            {
                var time = DateTime.Now;
                usbControllerDevice.AppData.TimeValue[0] = (byte)time.Hour;
                usbControllerDevice.AppData.TimeValue[1] = (byte)time.Minute;
                usbControllerDevice.AppData.TimeValue[2] = (byte)time.Second;

                usbControllerDevice.AppData.SendTimePending = true;
            }
            catch (Exception ex)
            {
                Logger.Write(Level.Error, $"LightServiceThread Error: {ex}");
            }
        }
    }
}
