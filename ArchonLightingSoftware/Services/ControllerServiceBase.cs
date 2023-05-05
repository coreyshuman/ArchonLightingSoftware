﻿using System;
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
    /// <summary>
    /// Special service handler base which will split the period time up between active controllers and
    /// alternate between active controllers in the service handler callback.
    /// </summary>
    abstract class ControllerServiceBase : IService
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
        private bool delayRoundUpDown = false;
        private bool pause = false;
        private UsbControllerManager usbControllerManager = null;
        private SensorMonitorManager hardwareManager = null;
        private int controllerIdx = 0;

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

                        var controllerContext = GetNextControllerContext();
                        if (controllerContext == null) continue;
                        ServiceTask(controllerContext.Item1, controllerContext.Item2);
                    }
                    catch (Exception ex)
                    {
                        Logger.Write(Level.Error, $"ServiceThread Exception: {ex.Message}");
                        throw ex;
                    }
                    finally
                    {
                        Thread.Sleep(GetDelay());
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
        /// <param name="usbControllerDevice"></param>
        /// <param name="hardwareManager"></param>
        public abstract void ServiceTask(UsbControllerDevice usbControllerDevice, SensorMonitorManager hardwareManager);

        public Tuple<UsbControllerDevice, SensorMonitorManager> GetNextControllerContext()
        {
            try
            {
                if (usbControllerManager.ActiveControllers.Count == 0) return null;

                if (++controllerIdx >= usbControllerManager.ActiveControllers.Count)
                {
                    controllerIdx = 0;
                }

                var usbControllerDevice = usbControllerManager.ActiveControllers[controllerIdx];

                if (usbControllerDevice == null)
                {
                    return null;
                }

                if(!usbControllerDevice.IsConnected)
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

        // Calculate delay so that each controller is serviced at the prescribed period
        public int GetDelay()
        {
            double actualPeriod = period / usbControllerManager.ActiveControllers.Count;

            // alternate rounding up and down to retain better accuracy
            if(delayRoundUpDown)
            {
                actualPeriod = Math.Ceiling(actualPeriod);
            }
            else
            {
                actualPeriod = Math.Floor(actualPeriod);
            }
            delayRoundUpDown = !delayRoundUpDown;

            if(actualPeriod < 1)
            {
                actualPeriod = 1;
            }

            return (int)actualPeriod;
        }
    }
}
