using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using ArchonLightingSystem.Models;
using ArchonLightingSystem.UsbApplication;
using ArchonLightingSystem.OpenHardware;


namespace ArchonLightingSystem.Services
{
    abstract class ControllerServiceBase
    {
        /// <summary>
        /// Freqency in Hz that the task will run. Valid values 1 - 1000;
        /// </summary>
        public int TaskFrequency
        {
            get
            {
                return frequency;
            }
            set
            {
                frequency = value > 1000 ? 1000 : value;
                if (frequency < 1) frequency = 1;

                period = (int)Math.Round(1000 / (decimal)frequency);
            }
        }

        /// <summary>
        /// Amount of delay in milliseconds between each run of the task
        /// </summary>
        public int TaskPeriod
        {
            get
            {
                return period;
            }
            set
            {
                frequency = 0;  // todo make this better?
                period = value;
            }
        }

        private int frequency = 2;
        private int period = 500;
        private UserSettings userSettings = null;
        private UsbDeviceManager usbDeviceManager = null;
        private SensorMonitorManager hardwareManager = null;
        private BackgroundWorker serviceThread;
        private int controllerIdx = 0;

        public void BeginService(UserSettings us, UsbDeviceManager ap, SensorMonitorManager hw)
        {
            userSettings = us;
            usbDeviceManager = ap;
            hardwareManager = hw;
            serviceThread = new BackgroundWorker();
            serviceThread.WorkerSupportsCancellation = true;
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
            while (!serviceThread.CancellationPending)
            {
                try
                {
                    var controllerContext = GetNextControllerContext();
                    if (controllerContext == null) return;
                    ServiceTask(controllerContext.Item1, controllerContext.Item2, controllerContext.Item3);
                }
                catch (Exception ex)
                {
                    Trace.TraceError($"ServiceThread Error: {ex.ToString()}");
                }
                finally
                {
                    Thread.Sleep(period);
                }
                
            }
        }

        /// <summary>
        /// Override this function and do your service tasks here.
        /// </summary>
        /// <param name="applicationData"></param>
        /// <param name="controllerSettings"></param>
        /// <param name="hardwareManager"></param>
        public abstract void ServiceTask(ApplicationData applicationData, ControllerSettings controllerSettings, SensorMonitorManager hardwareManager);

        public Tuple<ApplicationData, ControllerSettings, SensorMonitorManager> GetNextControllerContext()
        {
            try
            {
                if (++controllerIdx >= usbDeviceManager.DeviceCount)
                {
                    controllerIdx = 0;
                }

                var usbDevice = usbDeviceManager.GetDevice(controllerIdx);
                var appData = usbDevice?.AppData;
                if (appData == null)
                {
                    return null;
                }
                var controllerSettings = userSettings.GetControllerByAddress(appData.DeviceControllerData.DeviceAddress);
                if (controllerSettings == null)
                {
                    return null;
                }

                return new Tuple<ApplicationData, ControllerSettings, SensorMonitorManager>(appData, controllerSettings, hardwareManager);
            }
            catch(Exception ex)
            {
                Trace.TraceError("Couldn't locate next controller: " + ex.Message);
                return null;
            }
        }    
    }
}
