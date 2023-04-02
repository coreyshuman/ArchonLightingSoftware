using System;
using System.Diagnostics;
using System.Globalization;
using ArchonLightingSystem.ArchonLightingSDKIntegration;
using ArchonLightingSystem.Common;
using ArchonLightingSystem.Models;
using ArchonLightingSystem.OpenHardware;


namespace ArchonLightingSystem.Services
{
    class LightControllerService : ControllerServiceBase
    {
        private MappedFileManager mappedFileManager = MappedFileManager.Instance;
        private byte[,] ledFrame = new byte[DeviceControllerDefinitions.DevicePerController, DeviceControllerDefinitions.LedBytesPerDevice];
        private DateTime tick = DateTime.Now;

        /// <summary>
        /// Service which periodically updates leds based on dynamic lighting options.
        /// </summary>
        /// <param name="taskFrequency">Task execution frequency in Hz. One controller updated per execution.</param>
        public LightControllerService(int taskFrequency)
        {
            TaskFrequency = taskFrequency;
        }


        public override void ServiceTask(ApplicationData applicationData, ControllerSettings controllerSettings, SensorMonitorManager hardwareManager)
        {
            try
            {
                if(mappedFileManager.IsUpdateReceived)
                {
                    for(int devIdx = 0; devIdx < DeviceControllerDefinitions.DevicePerController; devIdx ++)
                    {
                        for(int ledIdx = 0; ledIdx < DeviceControllerDefinitions.LedCountPerDevice; ledIdx++)
                        {
                            ledFrame[devIdx, ledIdx * 3] = mappedFileManager.Lights[ledIdx].Green;
                            ledFrame[devIdx, ledIdx * 3 + 1] = mappedFileManager.Lights[ledIdx].Red;
                            ledFrame[devIdx, ledIdx * 3 + 2] = mappedFileManager.Lights[ledIdx].Blue;
                        }
                    }

                    applicationData.LedFrameData = ledFrame;
                    applicationData.WriteLedFrame = true;
                }
            }
            catch (Exception ex)
            {
                Logger.Write(Level.Error, $"LightServiceThread Error: {ex}");
            }

            if(applicationData.DeviceControllerData.DeviceAddress == 1)
            {
                //Console.WriteLine((DateTime.Now - tick).TotalMilliseconds);
                tick = DateTime.Now;
            }
        }
    }
}
