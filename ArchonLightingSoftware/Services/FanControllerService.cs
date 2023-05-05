using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ArchonLightingSystem.Common;
using ArchonLightingSystem.Models;
using ArchonLightingSystem.OpenHardware;
using ArchonLightingSystem.UsbApplicationV2;
using LibreHardwareMonitor.Hardware;

namespace ArchonLightingSystem.Services
{
    class FanControllerService : ControllerServiceBase
    {
        /// <summary>
        /// Service which periodically updates fan speeds based on temperature sensor and fan curve settings.
        /// </summary>
        /// <param name="taskPeriod">Task execution period</param>
        public FanControllerService(int taskPeriod)
        {
            TaskPeriod = taskPeriod;
        }

        public override void ServiceTask(UsbControllerDevice usbControllerDevice, SensorMonitorManager hardwareManager)
        {
            try
            {
                Tuple<byte[], byte[]> values = CalculateFanSpeeds(usbControllerDevice, hardwareManager);
                SendFanSpeeds(usbControllerDevice, values);
            }
            catch (Exception ex)
            {
                Logger.Write(Level.Error, $"FanServiceThread Error: {ex.Message}");
            }
        }

        /// <summary>
        /// Map fan temperature to fan speed based on speed curve
        /// </summary>
        /// <param name="usbControllerDevice"></param>
        /// <param name="hardwareManager"></param>
        /// <returns>A tuple of array of bytes, the first containing speed values and the second containing sensor values for animations</returns>
        private Tuple<byte[], byte[]> CalculateFanSpeeds(UsbControllerDevice usbControllerDevice, SensorMonitorManager hardwareManager)
        {
            byte[] speedValues = new byte[DeviceControllerDefinitions.DevicePerController];
            byte[] scaledSensorValuesForAnimation = new byte[DeviceControllerDefinitions.DevicePerController];

            for (int devIdx = 0; devIdx < DeviceControllerDefinitions.DevicePerController; devIdx++)
            {
                // Default / disabled value for controller API is 0xFF
                speedValues[devIdx] = 0xFF;
                scaledSensorValuesForAnimation[devIdx] = 0xFF;

                var deviceSetting = usbControllerDevice.Settings.GetDeviceByIndex(devIdx);
                if (deviceSetting != null)
                {
                    lock (deviceSetting)
                    {
                        List<double> values = new List<double>();
                        ISensor primarySensor = null;

                        foreach(var identifier in deviceSetting.IdentifierList)
                        {
                            var sensor = hardwareManager.GetSensorByIdentifier(identifier);
                            if(primarySensor == null) primarySensor = sensor;
                            if(sensor?.Value.HasValue == true) values.Add(sensor.Value.Value);
                        }

                        
                        if (primarySensor == null)
                        {
                            continue;
                        }

                        double min = SensorUnits.GetMin(primarySensor);
                        double max = SensorUnits.GetMax(primarySensor);
                        double interval = SensorUnits.GetInterval(primarySensor);

                        double rawSensorValue = primarySensor.Value ?? 0;

                        // metric-based lighting animations use a range from 0 to 132
                        scaledSensorValuesForAnimation[devIdx] = (byte)ScaleValue(max, 132f, min, 0f, rawSensorValue);

                        if (deviceSetting != null && deviceSetting.UseFanCurve)
                        {
                            List<int> fanCurveValues = deviceSetting.FanCurveValues;

                            switch (deviceSetting.CalculationMethod)
                            {
                                case CalculationMethods.Methods.Max:
                                    rawSensorValue = values.Max();
                                    break;
                                case CalculationMethods.Methods.Min:
                                    rawSensorValue = values.Min();
                                    break;
                                case CalculationMethods.Methods.Average:
                                    rawSensorValue = values.Average();
                                    break;
                                case CalculationMethods.Methods.Sum:
                                    rawSensorValue = values.Sum();
                                    break;
                            }

                            double fanLowerBound = fanCurveValues[0];
                            double sensorLowerBound = min;
                            double calculatedFanSpeed = 0;

                            // Find the location on the curve for the given sensor value.
                            // Use slope equation to find value if between two curve points.
                            bool found = false;
                            for (int i = 1; i < fanCurveValues.Count; i++)
                            {
                                double sensorUpperBound = i * interval;
                                double fanUpperBound = fanCurveValues[i];
                                // if sensor value above upper bound, move to next interval
                                if (rawSensorValue > sensorUpperBound)
                                {
                                    fanLowerBound = fanUpperBound;
                                    sensorLowerBound = sensorUpperBound;
                                }
                                // sensor value is within current interval range, calculate fan speed
                                else
                                {
                                    calculatedFanSpeed = ScaleValue(sensorUpperBound, fanUpperBound, sensorLowerBound, fanLowerBound, rawSensorValue);
                                    found = true;
                                    break; // found our value, break from loop
                                }
                            }

                            // value is out of bounds, use highest fan curve value
                            if (!found)
                            {
                                calculatedFanSpeed = fanCurveValues.Last();
                            }

                            // compare target speed to newly calculated speed and apply hysteresis
                            int targetFanSpeed = usbControllerDevice.AppData.FanSpeedTargetValue[devIdx];
                            if (calculatedFanSpeed < targetFanSpeed)
                            {
                                int fanSpeedDecreaseAmount = (int)(targetFanSpeed - calculatedFanSpeed);
                                if (fanSpeedDecreaseAmount > deviceSetting.DecreaseHysteresis)
                                {
                                    usbControllerDevice.AppData.FanSpeedTargetValue[devIdx] = (byte)calculatedFanSpeed;
                                }
                            }
                            else
                            {
                                int fanSpeedIncreaseAmount = (int)(calculatedFanSpeed - targetFanSpeed);
                                if (fanSpeedIncreaseAmount > deviceSetting.IncreaseHysteresis)
                                {
                                    usbControllerDevice.AppData.FanSpeedTargetValue[devIdx] = (byte)calculatedFanSpeed;
                                }
                            }

                            // update actual fan speed using step configuration
                            targetFanSpeed = usbControllerDevice.AppData.FanSpeedTargetValue[devIdx];
                            int actualFanSpeed = usbControllerDevice.ControllerData.AutoFanSpeedValue[devIdx];
                            // switching from manual to auto mode
                            if(actualFanSpeed == 0xFF)
                            {
                                actualFanSpeed = 50;
                            }
                            int fanAdjustAmount = targetFanSpeed - actualFanSpeed;
                            if (targetFanSpeed < actualFanSpeed)
                            {
                                if (fanAdjustAmount < -deviceSetting.DecreaseStep)
                                {
                                    fanAdjustAmount = -deviceSetting.DecreaseStep;
                                }
                            }
                            else if (fanAdjustAmount > deviceSetting.IncreaseStep)
                            {
                                fanAdjustAmount = deviceSetting.IncreaseStep;
                            }

                            speedValues[devIdx] = (byte)(actualFanSpeed + fanAdjustAmount);
                        }
                    }
                }
            }
            return new Tuple<byte[], byte[]>(speedValues, scaledSensorValuesForAnimation);
        }

        /// <summary>
        /// Send fan speed control values to the fan controller. Accepts array of 5 values for each fan output.
        /// 0-100 for override control, or 255 to use interval config speed.
        /// </summary>
        private void SendFanSpeeds(UsbControllerDevice usbControllerDevice, Tuple<byte[], byte[]> values)
        {
            for(int i = 0; i < DeviceControllerDefinitions.DevicePerController; i++)
            {
                usbControllerDevice.ControllerData.AutoFanSpeedValue[i] = values.Item1[i];
                usbControllerDevice.ControllerData.TemperatureValue[i] = values.Item2[i];
            }
            usbControllerDevice.AppData.UpdateFanSpeedPending = true;
        }

        private double ScaleValue(double fromHigher, double toHigher, double fromLower, double toLower, double value)
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
