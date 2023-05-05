using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using ArchonLightingSystem.Models;
using ArchonLightingSystem.UsbApplicationV2;
using ArchonLightingSystem.OpenHardware;
using ArchonLightingSystem.Common;
using System.Threading.Tasks;
using ArchonLightingSystem.Interfaces;

namespace ArchonLightingSystem.Services
{
    abstract class ServiceBase : IService
    {
        /// <summary>
        /// Amount of delay in milliseconds between each run of the task for each controller
        /// </summary>
        public int TaskPeriod
        {
            get
            {
                return period;
            }
            set
            {
                period = value;
            }
        }

        /// <summary>
        /// Pause or resume service activity
        /// </summary>
        /// <param name="pause"></param>
        public void Pause(bool pause)
        {
            this.pause = pause;
        }

        private bool isStarted = false;
        private int period = 500;
        private bool pause = false;
        private UsbControllerManager usbControllerManager = null;
        private SensorMonitorManager hardwareManager = null;

        private CancellationTokenSource cancellationTokenSource;
        private Task task;

        /// <summary>
        /// Begin service worker
        /// </summary>
        /// <param name="cm"></param>
        /// <param name="hw"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public void BeginService(UsbControllerManager cm, SensorMonitorManager hw)
        {
            if (isStarted) throw new InvalidOperationException("Service already started.");
            isStarted = true;

            usbControllerManager = cm;
            hardwareManager = hw;
            cancellationTokenSource = new CancellationTokenSource();

            task = Task.Run(() =>
            {
                while (!cancellationTokenSource.IsCancellationRequested)
                {
                    try
                    {
                        if (pause) continue;

                        ServiceTask(cm, hw);
                    }
                    catch (Exception ex)
                    {
                        Logger.Write(Level.Error, $"ServiceThread Exception: {ex.Message}");
                        throw ex;
                    }
                    finally
                    {
                        Thread.Sleep(period);
                    }
                }
            });
        }

        /// <summary>
        /// Stop all service work and close worker thread
        /// </summary>
        public void CloseService()
        {
            cancellationTokenSource.Cancel();
        }

        /// <summary>
        /// Override this function and do your service tasks here.
        /// </summary>
        /// <param name="controllerManager"></param>
        /// <param name="hardwareManager"></param>
        public abstract void ServiceTask(UsbControllerManager controllerManager, SensorMonitorManager hardwareManager);
    }
}
