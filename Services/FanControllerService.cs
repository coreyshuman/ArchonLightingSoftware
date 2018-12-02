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
                hardwareManager.UpdateReadings();
                var controller = GetNextController();
                if(controller != null)
                {
                    var appData = controller.Item1;
                    var settings = controller.Item2;

                    for(int i = 0; i < DeviceControllerDefinitions.DevicePerController; i++)
                    {
                        var deviceSetting = settings.Devices.Where(d => d.Index == i).FirstOrDefault();
                        if (deviceSetting != null && deviceSetting.UseFanCurve) {
                            UpdateFanSpeed(appData, deviceSetting, i);
                        }
                    }
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
        /// convert temperature 0 - 100 to fan speed 0 - 100
        /// </summary>
        /// <param name="temperature"></param>
        /// <param name="deviceIndex"></param>
        private void UpdateFanSpeed(ApplicationData applicationData, DeviceSettings deviceSettings, int deviceIndex)
        {
            var sensor = hardwareManager.GetSensorByIdentifier(deviceSettings.Sensor);
            if(sensor == null || !sensor.Value.HasValue)
            {
                return;
            }
            
            int temperature = (int)Math.Round(sensor.Value.Value);

            if (temperature < 0) temperature = 0;
            if (temperature > 100) temperature = 100;

            int lowerBoundValue = 0;
            int calculatedFanSpeed = 0;
            List<int> fanSpeedValues = deviceSettings.FanCurveValues;
            for (int i = 0; i < fanSpeedValues.Count; i++)
            {
                int comparedTemperature = i * 10;
                if (temperature > comparedTemperature)
                {
                    lowerBoundValue = fanSpeedValues[i];
                }
                else
                {
                    if (temperature != comparedTemperature)
                    {
                        calculatedFanSpeed = ((fanSpeedValues[i] - lowerBoundValue) / 10) * (temperature - (comparedTemperature - 10)) + lowerBoundValue; // y = (y2-y1)/(x2-x1)*(x-x1) + y1
                    }
                    else
                    {
                        calculatedFanSpeed = fanSpeedValues[i];
                    }
                    break;
                }
            }

            applicationData.DeviceControllerData.DeviceConfig.FanSpeed[deviceIndex] = (byte)calculatedFanSpeed;
            applicationData.UpdateConfigPending = true;
        }
    }
}
