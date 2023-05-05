using System;
using System.Diagnostics;
using System.Globalization;
using ArchonLightingSystem.ArchonLightingSDKIntegration;
using ArchonLightingSystem.Common;
using ArchonLightingSystem.Models;
using ArchonLightingSystem.OpenHardware;
using ArchonLightingSystem.UsbApplicationV2;

namespace ArchonLightingSystem.Services
{
    class LightControllerService : ControllerServiceBase
    {
        private MappedFileManager mappedFileManager = MappedFileManager.Instance;
        private byte[,] ledFrame = new byte[DeviceControllerDefinitions.DevicePerController, DeviceControllerDefinitions.LedBytesPerDevice];

        /// <summary>
        /// Service which periodically updates leds based on dynamic lighting sources.
        /// </summary>
        /// <param name="taskPeriod">Task execution period</param>
        public LightControllerService(int taskPeriod)
        {
            TaskPeriod = taskPeriod;
        }

        public override void ServiceTask(UsbControllerDevice usbControllerDevice, SensorMonitorManager hardwareManager)
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

                    usbControllerDevice.AppData.LedFrameData = ledFrame;
                    usbControllerDevice.AppData.WriteLedFrame = true;
                }
            }
            catch (Exception ex)
            {
                Logger.Write(Level.Error, $"LightServiceThread Error: {ex}");
            }
        }
    }
}
