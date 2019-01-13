using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using ArchonLightingSystem.Models;
using ArchonLightingSystem.UsbApplication;
using ArchonLightingSystem.OpenHardware;

namespace ArchonLightingSystem.Services
{
    class FanControllerService
    {
        private UserSettings userSettings = null;
        private UsbApp usbApplication = null;
        private HardwareManager hardwareManager = null;
        private BackgroundWorker serviceThread;
        private int controllerAddress = 0;
        private int controllerIdx = 0;

        public FanControllerService()
        {
            
        }

        public void BeginService(UserSettings us, UsbApp ap, HardwareManager hw)
        {
            userSettings = us;
            usbApplication = ap;
            hardwareManager = hw;
            serviceThread = new BackgroundWorker();
            serviceThread.WorkerReportsProgress = false;
            serviceThread.DoWork += new DoWorkEventHandler(ServiceThread_DoWork);
            serviceThread.RunWorkerAsync();
        }

        public void CloseService()
        {
            serviceThread.CancelAsync();
        }

        private void ServiceThread_DoWork(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                try
                {
                    hardwareManager.UpdateReadings();
                    var controller = GetNextController();
                    if (controller != null)
                    {
                        var appData = controller.Item1;
                        var controllerSettings = controller.Item2;

                        byte[] speedValues = CalculateFanSpeeds(appData, controllerSettings);
                        SendFanSpeeds(appData, speedValues);
                    }
                    
                }
                catch (Exception ex)
                {
                    Trace.WriteLine($"FanServiceThread Error: {ex.ToString()}");
                }
                Thread.Sleep(500);
            }
        }

        private Tuple<ApplicationData, ControllerSettings> GetNextController()
        {
            try
            {
                if (++controllerIdx > usbApplication.DeviceCount)
                {
                    controllerIdx = 0;
                }

                var usbDevice = usbApplication.GetDevice(controllerIdx);
                var appData = usbDevice?.AppData;
                if (appData == null)
                {
                    return null;
                }
                var controllerSettings = GetControllerSettings(appData.DeviceControllerData.DeviceAddress);
                if(controllerSettings == null)
                {
                    return null;
                }
                return new Tuple<ApplicationData, ControllerSettings>(appData, controllerSettings);
            }
            catch
            {
                Trace.TraceError("Couldn't locate next controller.");
                return null;
            }
        }

        private ControllerSettings GetControllerSettings(int controllerAddress)
        {
            return userSettings.Controllers.Where(c => c.Address == controllerAddress).FirstOrDefault();
        }

        private DeviceSettings GetDeviceSettings(int devIdx)
        {
            return userSettings.Controllers.Where(c => c.Address == controllerAddress).FirstOrDefault().Devices.Where(d => d.Index == devIdx).FirstOrDefault();
        }

        /// <summary>
        /// Map fan temperature to fan speed based on speed curve
        /// </summary>
        /// <param name="temperature"></param>
        /// <param name="deviceIndex"></param>
        private byte[] CalculateFanSpeeds(ApplicationData applicationData, ControllerSettings controllerSettings)
        {
            byte[] speedValues = new byte[DeviceControllerDefinitions.DevicePerController];

            for (int devIdx = 0; devIdx < DeviceControllerDefinitions.DevicePerController; devIdx++)
            {
                speedValues[devIdx] = 0xFF;

                var deviceSetting = controllerSettings.Devices.Where(d => d.Index == devIdx).FirstOrDefault();
                if (deviceSetting != null && deviceSetting.UseFanCurve)
                {
                    var sensor = hardwareManager.GetSensorByIdentifier(deviceSetting.Sensor);
                    if (sensor == null || !sensor.Value.HasValue)
                    {
                        continue;
                    }

                    int temperature = (int)Math.Round(sensor.Value.Value);

                    if (temperature < 0) temperature = 0;
                    if (temperature > 100) temperature = 100;

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
            return speedValues;
        }

        /// <summary>
        /// Send fan speed control values to the fan controller. Accepts array of 5 values for each fan output.
        /// 0-100 for override control, or 255 to use interval config speed.
        /// </summary>
        private void SendFanSpeeds(ApplicationData applicationData, byte[] speedValues)
        {
            for(int i = 0; i < DeviceControllerDefinitions.DevicePerController; i++)
            {
                applicationData.DeviceControllerData.AutoFanSpeedValue[i] = speedValues[i];
            }
            applicationData.UpdateFanSpeedPending = true;
        }
    }
}
