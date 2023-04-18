using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using ArchonLightingSystem.Models;
using ArchonLightingSystem.UsbApplicationV2;
using ArchonLightingSystem.OpenHardware;
using ArchonLightingSystem.Common;

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

        public void Pause(bool pause)
        {
            this.pause = pause;
        }

        private int frequency = 2;
        private int period = 500;
        private bool pause = false;
        private UsbControllerManager usbControllerManager = null;
        private SensorMonitorManager hardwareManager = null;
        private BackgroundWorker serviceThread;
        private int controllerIdx = 0;

        public void BeginService(UsbControllerManager cm, SensorMonitorManager hw)
        {
            usbControllerManager = cm;
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
                    if (pause) continue;

                    var controllerContext = GetNextControllerContext();
                    if (controllerContext == null) continue;
                    ServiceTask(controllerContext.Item1, controllerContext.Item2);
                }
                catch (Exception ex)
                {
                    Logger.Write(Level.Error, $"ServiceThread Error: {ex}");
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
        /// <param name="usbControllerDevice"></param>
        /// <param name="hardwareManager"></param>
        public abstract void ServiceTask(UsbControllerDevice usbControllerDevice, SensorMonitorManager hardwareManager);

        public Tuple<UsbControllerDevice, SensorMonitorManager> GetNextControllerContext()
        {
            try
            {
                if (++controllerIdx >= usbControllerManager.ActiveControllers.Count)
                {
                    controllerIdx = 0;
                }

                var usbControllerDevice = usbControllerManager.ActiveControllers[controllerIdx];

                if (usbControllerDevice == null)
                {
                    return null;
                }

                return new Tuple<UsbControllerDevice, SensorMonitorManager>(usbControllerDevice, hardwareManager);
            }
            catch(Exception ex)
            {
                Logger.Write(Level.Error, "Couldn't locate next controller: " + ex.Message);
                return null;
            }
        }    
    }
}
