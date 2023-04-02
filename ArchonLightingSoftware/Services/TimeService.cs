using ArchonLightingSystem.Common;
using ArchonLightingSystem.Models;
using ArchonLightingSystem.OpenHardware;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArchonLightingSystem.Services
{
    class TimeServiceController : ControllerServiceBase
    {
        /// <summary>
        /// Service which periodically updates time on controller units.
        /// </summary>
        /// <param name="taskFrequency">Task execution period in milliseconds. One controller updated per execution.</param>
        public TimeServiceController(int taskPeriod)
        {
            // todo - improve this, make it update all controllers per execution
            TaskPeriod = taskPeriod;
        }


        public override void ServiceTask(ApplicationData applicationData, ControllerSettings controllerSettings, SensorMonitorManager hardwareManager)
        {
            try
            {
                var time = DateTime.Now;
                applicationData.TimeValue[0] = (byte)time.Hour;
                applicationData.TimeValue[1] = (byte)time.Minute;
                applicationData.TimeValue[2] = (byte)time.Second;

                applicationData.SendTimePending = true;
                Logger.Write(Level.Trace, "time controller service tick");
            }
            catch (Exception ex)
            {
                Logger.Write(Level.Error, $"LightServiceThread Error: {ex}");
            }
        }
    }
}
