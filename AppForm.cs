using System;
using System.Windows.Forms;
using ArchonLightingSystem.Components;
using ArchonLightingSystem.Models;
using ArchonLightingSystem.UsbApplication;
using ArchonLightingSystem.Common;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace ArchonLightingSystem
{
    public partial class AppForm : Form
    {
        private UsbApp usbApp = null;
        private ConfigEditorForm configForm = null;
        private EepromEditorForm eepromForm = null;
        private DebugForm debugForm = null;
        private FirmwareUpdateForm firmwareForm = null;
        private bool formIsInitialized = false;
        private List<string> deviceAddressList = new List<string>();
        private int selectedAddressIdx = 0;
        private List<ControllerComponent> deviceComponents = new List<ControllerComponent>();
        private OpenHardwareMonitor.Hardware.IHardware motherboard;
        private string temperatureString = "";
        private DragWindowSupport dragSupport = new DragWindowSupport();

        public unsafe AppForm()
        {
            InitializeComponent();
            dragSupport.Initialize(this, menuStrip1);

            usbApp = new UsbApp();
            InitializeForm();

            usbApp.RegisterEventHandler(this.Handle);
            usbApp.InitializeDevice(Consts.ApplicationVid, Consts.ApplicationPid);
            //InitializeHardwareMonitor();
            FormUpdateTimer.Enabled = true;
        }

        void InitializeForm()
        {
            cbo_DeviceAddress.DisplayMember = "Text";
            cbo_DeviceAddress.ValueMember = "Value";

            for(int i = 1; i <= DeviceControllerDefinitions.DeviceCount; i++)
            {
                ControllerComponent device = new ControllerComponent();
                deviceComponents.Add(device);
                device.InitializeComponent(this, i);
            }
        }

        void UpdateFormSettings(DeviceControllerData controller)
        {
            FormUpdateTimer.Enabled = false;
            FormUpdateTimer.Enabled = true;
            for (int i = 1; i <= DeviceControllerDefinitions.DeviceCount; i++)
            {
                deviceComponents[i - 1].AppData = usbApp.GetAppData(selectedAddressIdx);
            }
        }

        //This is a callback function that gets called when a Windows message is received by the form.
        //We will receive various different types of messages, but the ones we really want to use are the WM_DEVICECHANGE messages.
        protected override void WndProc(ref Message m)
        {
            usbApp.HandleWindowEvent(ref m);
            base.WndProc(ref m);
        } 

        private async void FormUpdateTimer_Tick(object sender, EventArgs e)
        {
            //This timer tick event handler function is used to update the user interface on the form, based on data
            //obtained asynchronously by the ReadWriteThread and the WM_DEVICECHANGE event handler functions.

            if(usbApp.DeviceCount > cbo_DeviceAddress.Items.Count)
            {
                var activeDevices = usbApp.UsbDevices.Where(dev => dev.IsAttached && dev.AppIsInitialized && dev.AppData.DeviceControllerData?.IsInitialized == true);
                var newDevices = activeDevices.Where(dev => !deviceAddressList.Contains(dev.AppData.DeviceControllerData.DeviceAddress.ToString()))
                    .Select((dev) => new ComboBoxItem { Text = dev.AppData.DeviceControllerData.DeviceAddress.ToString(), Value = usbApp.UsbDevices.IndexOf(dev)})
                    .ToList();
                newDevices.ForEach(dev =>
                {
                    deviceAddressList.Add(dev.Text);
                    cbo_DeviceAddress.Items.Add(dev);
                });
                if(cbo_DeviceAddress.Items.Count > 0 && cbo_DeviceAddress.SelectedItem == null)
                {
                    cbo_DeviceAddress.SelectedIndex = 0;
                }
            }

            var usbDevice = usbApp.GetDevice(selectedAddressIdx);
            if(usbDevice == null)
            {
                return;
            }

            if (await usbDevice.semaphore.WaitAsync(200))
            {
                try
                {
                    //Check if user interface on the form should be enabled or not, based on the attachment state of the USB device.
                    if (usbApp.GetDevice(selectedAddressIdx).IsAttached == true)
                    {
                        //Device is connected and ready to communicate, enable user interface on the form 
                        string addr = usbApp.GetAppData(selectedAddressIdx)?.DeviceControllerData?.DeviceAddress.ToString();
                        string bootVer = usbApp.GetAppData(selectedAddressIdx)?.DeviceControllerData?.BootloaderVersion?.ToString();
                        string AppVer = usbApp.GetAppData(selectedAddressIdx)?.DeviceControllerData?.ApplicationVersion?.ToString();
                        statusLabel.Text = $"Device {addr} Found. BootVer: {bootVer}   AppVer: {AppVer}  Temp: {temperatureString}";
                    }
                    if ((usbApp.GetDevice(selectedAddressIdx).IsAttached == false) || (usbApp.GetDevice(selectedAddressIdx).IsAttachedButBroken == true))
                    {
                        //Device not available to communicate. Disable user interface on the form.
                        statusLabel.Text = $"Device Not Detected: Verify Connection/Correct Firmware.  Temp: {temperatureString}";
                    }

                    //Update the various status indicators on the form with the latest info obtained from the ReadWriteThread()
                    if (usbApp.GetDevice(selectedAddressIdx).IsAttached == true)
                    {
                        if (!formIsInitialized && usbApp.GetAppData(selectedAddressIdx)?.DeviceControllerData?.IsInitialized == true)
                        {
                            UpdateFormSettings(usbApp.GetAppData(selectedAddressIdx).DeviceControllerData);
                            formIsInitialized = true;
                        }
                    }

                    UpdateHardwareTemperature();
                }
                catch(Exception ex)
                {
                    Trace.WriteLine($"ForUpdateTick Error: {ex.ToString()}");
                }
                finally
                {
                    usbDevice.semaphore.Release();
                }
            }
        }

        private void InitializeHardwareMonitor()
        {
            try
            {
                var cp = new OpenHardwareMonitor.Hardware.Computer();
                cp.Open();
                //cp.HDDEnabled = true;
                //cp.RAMEnabled = true;
                //cp.GPUEnabled = true;
                cp.MainboardEnabled = true;
                //cp.CPUEnabled = true;

                motherboard = cp.Hardware.Where(hw => hw.HardwareType == OpenHardwareMonitor.Hardware.HardwareType.Mainboard).FirstOrDefault();
            }
            catch(Exception ex)
            {
                Trace.WriteLine($"HardwareMonitor Init Error: {ex.ToString()}");
            }
        }

        private void UpdateHardwareTemperature()
        {
            if (motherboard != null)
            {
                try
                {
                    motherboard.Update();
                    if (motherboard.SubHardware.Count() > 0)
                    {
                        var superio = motherboard.SubHardware.ElementAt(0);
                        superio.Update();
                        var sensors = superio.Sensors.Where(sens => sens.SensorType == OpenHardwareMonitor.Hardware.SensorType.Temperature);
                        temperatureString = "";
                        foreach (var sensor in sensors)
                        {
                            temperatureString += sensor.Name + ": " + sensor.Value + "C ";
                        }
                        for (int i = 1; i <= DeviceControllerDefinitions.DeviceCount; i++)
                        {
                            deviceComponents[i - 1].Temperature = (int)sensors.Where(s => s.Name == "CPU").FirstOrDefault()?.Value;
                        }
                    }
                } 
                catch(Exception ex)
                {
                    Trace.WriteLine($"HardwareMonitor Update Error: {ex.ToString()}");
                }
            }
        }

        private void ShowTaskbarNotification(string title, string content, int duration = 5000)
        {
            notifyIcon1.BalloonTipTitle = title;
            notifyIcon1.BalloonTipText = content;
            notifyIcon1.ShowBalloonTip(duration);
        }

        /*
        private void OpenSubform<T>(SubformBase subform) where T : new()
        {
            if (subform == null || configForm.IsDisposed)
            {
                subform = new T() as SubformBase;
                subform.InitializeForm(usbApp.AppData);
                subform.Show();
            }
            if (subform.WindowState == FormWindowState.Minimized)
            {
                subform.WindowState = FormWindowState.Normal;
            }
            subform.Focus();
        }
        */

        private void editConfigToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (configForm == null || configForm.IsDisposed)
            {
                configForm = new ConfigEditorForm();
                configForm.InitializeForm(usbApp.GetAppData(selectedAddressIdx));
                configForm.Show();
            }
            if (configForm.WindowState == FormWindowState.Minimized)
            {
                configForm.WindowState = FormWindowState.Normal;
            }
            configForm.Focus();
            
            //OpenSubform<ConfigEditorForm>(configForm);
        }

        private void editEEPROMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (eepromForm == null || eepromForm.IsDisposed)
            {
                eepromForm = new EepromEditorForm();
                eepromForm.InitializeForm(usbApp.GetAppData(selectedAddressIdx));
                eepromForm.Show();
            }
            if (eepromForm.WindowState == FormWindowState.Minimized)
            {
                eepromForm.WindowState = FormWindowState.Normal;
            }
            eepromForm.Focus();
        }

        private void debugToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (debugForm == null || debugForm.IsDisposed)
            {
                debugForm = new DebugForm();
                debugForm.InitializeForm(usbApp);
                debugForm.Show();
            }
            if (debugForm.WindowState == FormWindowState.Minimized)
            {
                debugForm.WindowState = FormWindowState.Normal;
            }
            debugForm.Focus();
        }

        private void updateFirmwareToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for(int i = 0; i < usbApp.DeviceCount; i++)
            {
                usbApp.GetDevice(i).PauseUsb = true;
            }
            if (firmwareForm == null || firmwareForm.IsDisposed)
            {
                firmwareForm = new FirmwareUpdateForm();
                firmwareForm.InitializeForm(usbApp);
                firmwareForm.Show();
            }
            if (firmwareForm.WindowState == FormWindowState.Minimized)
            {
                firmwareForm.WindowState = FormWindowState.Normal;
            }
            firmwareForm.Focus();
        }

        private void btn_SaveConfig_Click(object sender, EventArgs e)
        {
            usbApp.GetAppData(selectedAddressIdx).WriteConfigPending = true;
        }

        private void btn_ResetToBoot_Click(object sender, EventArgs e)
        {
            usbApp.GetAppData(selectedAddressIdx).ResetToBootloaderPending = true;
        }

        private void cbo_DeviceAddress_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedAddressIdx = ((ComboBoxItem)((ComboBox)sender).SelectedItem).Value;
            UpdateFormSettings(usbApp.GetAppData(selectedAddressIdx).DeviceControllerData);
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Show();
        }

        private void closeToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
} 