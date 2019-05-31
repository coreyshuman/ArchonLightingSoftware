using System;
using System.Diagnostics;
using ArchonLightingSystem.Models;
using ArchonLightingSystem.OpenHardware;


namespace ArchonLightingSystem.Services
{
    class LightControllerService : ControllerServiceBase
    {

        /// <summary>
        /// Service which periodically updates leds based on dynamic lighting options.
        /// </summary>
        /// <param name="taskFrequency">Task execution frequency in Hz. One controller updated per execution.</param>
        public LightControllerService(int taskFrequency)
        {
            TaskFrequency = taskFrequency;
        }


        public override void ServiceTask(ApplicationData applicationData, ControllerSettings controllerSettings, HardwareManager hardwareManager)
        {
            try
            {
                Console.WriteLine("light controller service tick");
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"FanServiceThread Error: {ex.ToString()}");
            }
        }
    }
}
