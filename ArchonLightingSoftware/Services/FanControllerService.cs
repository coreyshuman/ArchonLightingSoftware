using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ArchonLightingSystem.Models;
using ArchonLightingSystem.OpenHardware;

namespace ArchonLightingSystem.Services
{
    class FanControllerService : ControllerServiceBase
    {
        /// <summary>
        /// Service which periodically updates fan speeds based on temperature sensor and fan curve settings.
        /// </summary>
        /// <param name="taskFrequency">Task execution frequency in Hz. One controller updated per execution.</param>
        public FanControllerService(int taskFrequency)
        {
            TaskFrequency = taskFrequency;
        }

        public override void ServiceTask(ApplicationData applicationData, ControllerSettings controllerSettings, SensorMonitorManager hardwareManager)
        {
            try
            {
                Tuple<byte[], byte[]> values = CalculateFanSpeeds(applicationData, controllerSettings, hardwareManager);
                SendFanSpeeds(applicationData, values);
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"FanServiceThread Error: {ex.ToString()}");
            }
        }

        /// <summary>
        /// Map fan temperature to fan speed based on speed curve
        /// </summary>
        /// <param name="applicationData"></param>
        /// <param name="controllerSettings"></param>
        /// <param name="hardwareManager"></param>
        /// <returns>Array of bytes containing speed values</returns>
        private Tuple<byte[], byte[]> CalculateFanSpeeds(ApplicationData applicationData, ControllerSettings controllerSettings, SensorMonitorManager hardwareManager)
        {
            byte[] speedValues = new byte[DeviceControllerDefinitions.DevicePerController];
            byte[] temperatureValues = new byte[DeviceControllerDefinitions.DevicePerController];

            for (int devIdx = 0; devIdx < DeviceControllerDefinitions.DevicePerController; devIdx++)
            {
                speedValues[devIdx] = 0xFF;
                temperatureValues[devIdx] = 0xFF;

                var deviceSetting = controllerSettings.Devices.Where(d => d.Index == devIdx).FirstOrDefault();
                if (deviceSetting != null)
                {
                    var sensor = hardwareManager.GetSensorByIdentifier(deviceSetting.Sensor);
                    if (sensor == null || !sensor.Value.HasValue)
                    {
                        continue;
                    }

                    int temperature = (int)Math.Round(sensor.Value.Value);

                    if (temperature < 0) temperature = 0;
                    if (temperature > 100) temperature = 100;

                    temperatureValues[devIdx] = (byte)ScaleValue(100f, 132f, 0f, 0f, temperature);

                    if (deviceSetting != null && deviceSetting.UseFanCurve)
                    {
                        int lowerBoundValue = 0;
                        int calculatedFanSpeed = 0;
                        List<int> fanCurveValues = deviceSetting.FanCurveValues;
                        for (int i = 0; i < fanCurveValues.Count; i++)
                        {
                            int comparedTemperature = i * 10;
                            if (temperature > comparedTemperature)
                            {
                                lowerBoundValue = fanCurveValues[i];
                            }
                            else
                            {
                                if (temperature != comparedTemperature)
                                {
                                    calculatedFanSpeed = ((fanCurveValues[i] - lowerBoundValue) / 10) * (temperature - (comparedTemperature - 10)) + lowerBoundValue; // y = (y2-y1)/(x2-x1)*(x-x1) + y1
                                }
                                else
                                {
                                    calculatedFanSpeed = fanCurveValues[i];
                                }
                                break; // found our value, break from loop
                            }
                        }
                        speedValues[devIdx] = (byte)calculatedFanSpeed;
                    }
                }
            }
            return new Tuple<byte[], byte[]>(speedValues, temperatureValues);
        }

        /// <summary>
        /// Send fan speed control values to the fan controller. Accepts array of 5 values for each fan output.
        /// 0-100 for override control, or 255 to use interval config speed.
        /// </summary>
        private void SendFanSpeeds(ApplicationData applicationData, Tuple<byte[], byte[]> values)
        {
            for(int i = 0; i < DeviceControllerDefinitions.DevicePerController; i++)
            {
                applicationData.DeviceControllerData.AutoFanSpeedValue[i] = values.Item1[i];
                applicationData.DeviceControllerData.TemperatureValue[i] = values.Item2[i];
            }
            applicationData.UpdateFanSpeedPending = true;
        }

        private float ScaleValue(float fromHigher, float toHigher, float fromLower, float toLower, float value)
        {
            if (value <= fromLower)
                return toLower;
            else if (value >= fromHigher)
                return toHigher;
            if (fromHigher - fromLower == 0)
            {
                return (toHigher - toLower)  * value;
            } else
            {
                return ((toHigher - toLower) / (fromHigher - fromLower)) * (value - fromLower) + toLower;
            }
        }
    }
}
