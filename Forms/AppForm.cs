using System;
using System.Windows.Forms;
using ArchonLightingSystem.Components;
using ArchonLightingSystem.Models;
using ArchonLightingSystem.UsbApplication;
using ArchonLightingSystem.Common;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using ArchonLightingSystem.Properties;
using ArchonLightingSystem.OpenHardware;
using ArchonLightingSystem.Services;
using ArchonLightingSystem.Forms;
using ArchonLightingSystem.OpenHardware.Startup;

namespace ArchonLightingSystem
{
    public partial class AppForm : Form
    {
        public bool FormIsInitialized { get; set; } = false;

        private UsbDeviceManager usbDeviceManager = null;
        private ConfigEditorForm configForm = null;
        private EepromEditorForm eepromForm = null;
        private DebugForm debugForm = null;
        private FirmwareUpdateForm firmwareForm = null;
        private HardwareManager hardwareManager = null;
        private SequencerForm sequencerForm = null;
        private StartupManager startupManager = new StartupManager();
        private ServiceManager serviceManager = new ServiceManager();
        
        private List<ComboBoxItem> deviceAddressList = new List<ComboBoxItem>();
        private int selectedAddressIdx = 0;
        private int selectedAddress = 0;
        private List<ControllerComponent> deviceComponents = new List<ControllerComponent>();
        private string temperatureString = "";
        private DragWindowSupport dragSupport = new DragWindowSupport();
        private UserSettingsManager settingsManager = new UserSettingsManager();
        private UserSettings userSettings = null;
        private bool isHidden = false;
        private bool allowVisible = false;
        private bool allowClose = false;
        private string lastNotification = "";


        public unsafe AppForm(bool startInBackground)
        {
            InitializeComponent();     

            dragSupport.Initialize(this, menuStrip1);
            dragSupport.DragWindowEvent += new DragWindowEventDelegate(DragWindowEventHandler);

            userSettings = settingsManager.GetSettings();
            hardwareManager = new HardwareManager();
            if (Settings.Default.MainWindowLocation.X >= 0)
            {
                this.Location = Settings.Default.MainWindowLocation;
            }

            InitializeForm();

            usbDeviceManager = new UsbDeviceManager();
            usbDeviceManager.UsbControllerEvent += UsbDeviceManager_UsbControllerEvent;
            usbDeviceManager.Connect(Handle, Consts.ApplicationVid, Consts.ApplicationPid);

            FormUpdateTimer.Enabled = true;

            startWithWindowsToolStripMenuItem.Enabled = startupManager.IsAvailable;
            startWithWindowsToolStripMenuItem.Checked = startupManager.Startup;

            // Handle cleanup when the user logs off
            Microsoft.Win32.SystemEvents.SessionEnded += delegate {
                serviceManager.StopServices();
                hardwareManager.Close();
            };

            serviceManager.StartServices(userSettings, usbDeviceManager, hardwareManager);

            allowVisible = !startInBackground;
            isHidden = startInBackground;
        }

        protected override void SetVisibleCore(bool value)
        {
            if (!allowVisible)
            {
                value = false;
                if (!this.IsHandleCreated) CreateHandle();
            }
            base.SetVisibleCore(value);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (!allowClose)
            {
                HideForm();
                e.Cancel = true;
            }
            base.OnFormClosing(e);
        }

        void DragWindowEventHandler(object sender, DragWindowEventArgs args)
        {
            Settings.Default.MainWindowLocation = args.Location;
            Settings.Default.Save();
        }

        void InitializeForm()
        {
            cbo_DeviceAddress.DisplayMember = "Text";
            cbo_DeviceAddress.ValueMember = "Value";

            for(int i = 1; i <= DeviceControllerDefinitions.DevicePerController; i++)
            {
                ControllerComponent device = new ControllerComponent();
                deviceComponents.Add(device);
                device.InitializeComponent(this, hardwareManager, i);
                device.UserSettings = userSettings;
            }
        }

        void UpdateFormSettings(DeviceControllerData controller)
        {
            FormUpdateTimer.Enabled = false;

            txt_ControllerName.Text = userSettings.Controllers.Where(c => c.Address == selectedAddress).FirstOrDefault()?.Name;


            for (int i = 1; i <= DeviceControllerDefinitions.DevicePerController; i++)
            {
                deviceComponents[i - 1].ControllerAddress = selectedAddress;
                deviceComponents[i - 1].AppData = usbDeviceManager.GetAppData(selectedAddressIdx);
            }
            FormUpdateTimer.Enabled = true;
        }

        //This is a callback function that gets called when a Windows message is received by the form.
        //We will receive various different types of messages, but the ones we really want to use are the WM_DEVICECHANGE messages.
        protected override void WndProc(ref Message m)
        {
            usbDeviceManager?.HandleWindowEvent(ref m);
            base.WndProc(ref m);
        }

        private void UsbDeviceManager_UsbControllerEvent(object sender, UsbControllerEventArgs e)
        {
            
            if (e is UsbControllerAddedEventArgs)
            {
                var controllerInstance = e.ControllerInstance;
                var comboBoxItem = new ComboBoxItem
                {
                    Text = controllerInstance.AppData.DeviceControllerData.DeviceAddress.ToString() +
                                $" ({userSettings.Controllers.Where(c => c.Address == controllerInstance.AppData.DeviceControllerData.DeviceAddress).FirstOrDefault()?.Name ?? ""})",
                    Value = usbDeviceManager.UsbDevices.IndexOf(controllerInstance)
                };

                deviceAddressList.Add(comboBoxItem);
            }
            else if (e is UsbControllerRemovedEventArgs)
            {

            }
            else if (e is UsbControllerErrorEventArgs)
            {
                UsbControllerErrorEventArgs errorArgs = e as UsbControllerErrorEventArgs;
                statusLabel.Text = $"Error on Device {errorArgs.ControllerAddress}: {errorArgs.Message}";
                SetErrorIcon(true);
                ShowTaskbarNotification("Device Error", statusLabel.Text);
            }
        }

        private async void FormUpdateTimer_Tick(object sender, EventArgs e)
        {
            // Update dropdown UI if new controllers have been connected
            if(deviceAddressList.Count > cbo_DeviceAddress.Items.Count)
            {
                var lastSelected = cbo_DeviceAddress.SelectedValue;
                cbo_DeviceAddress.Items.Clear();
                cbo_DeviceAddress.Items.AddRange(deviceAddressList.OrderBy(d => d.Text).ToArray());
                cbo_DeviceAddress.SelectedValue = lastSelected;

                if (cbo_DeviceAddress.Items.Count > 0 && cbo_DeviceAddress.SelectedItem == null)
                {
                    cbo_DeviceAddress.SelectedIndex = 0;
                }
            }

            var usbDevice = usbDeviceManager.GetDevice(selectedAddressIdx);
            if(usbDevice == null)
            {
                return;
            }

            if (await usbDevice.semaphore.WaitAsync(200))
            {
                try
                {
                    //Check if user interface on the form should be enabled or not, based on the attachment state of the USB device.
                    //Update the various status indicators on the form with the latest info obtained from the ReadWriteThread()
                    if (usbDevice.UsbDevice.IsAttached == true && usbDevice.AppIsInitialized)
                    {
                        if (!FormIsInitialized && usbDeviceManager.GetAppData(selectedAddressIdx)?.DeviceControllerData?.IsInitialized == true)
                        {
                            UpdateFormSettings(usbDeviceManager.GetAppData(selectedAddressIdx).DeviceControllerData);
                            FormIsInitialized = true;
                        }
                    }
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

        private void ShowTaskbarNotification(string title, string content, int duration = 5000)
        {
            // simple skip to ignore repeating notifications (failed device retrying, for example)
            if (title == lastNotification) return;
            
            if (InvokeRequired)
            {
                this.Invoke(new Action<string, string, int>(ShowTaskbarNotification), new object[] { title, content, duration });
                return;
            }
            notifyIcon1.BalloonTipTitle = "Archon Lighting: " + title;
            notifyIcon1.BalloonTipText = content;
            notifyIcon1.ShowBalloonTip(duration);
            lastNotification = title;
        }

        private void SetErrorIcon(bool error)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action<bool>(SetErrorIcon), new object[] { error });
                return;
            }
            notifyIcon1.Icon = error ? Resources.darkarchon : Resources.archon;
            this.Icon = notifyIcon1.Icon;
        }

        /*
        private void OpenSubform<T>(SubformBase subform) where T : new()
        {
            if (subform == null || configForm.IsDisposed)
            {
                subform = new T() as SubformBase;
                subform.InitializeForm(usbDeviceManager.AppData);
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
                configForm.InitializeForm(usbDeviceManager.GetAppData(selectedAddressIdx));
                configForm.Show();
            }
            if (configForm.WindowState == FormWindowState.Minimized)
            {
                configForm.WindowState = FormWindowState.Normal;
            }
            configForm.Location = this.Location;
            configForm.Focus();
            
            //OpenSubform<ConfigEditorForm>(configForm);
        }

        private void editEEPROMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (eepromForm == null || eepromForm.IsDisposed)
            {
                eepromForm = new EepromEditorForm();
                eepromForm.InitializeForm(usbDeviceManager.GetAppData(selectedAddressIdx));
                eepromForm.Show();
            }
            if (eepromForm.WindowState == FormWindowState.Minimized)
            {
                eepromForm.WindowState = FormWindowState.Normal;
            }
            eepromForm.Location = this.Location;
            eepromForm.Focus();
        }

        private void debugToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (debugForm == null || debugForm.IsDisposed)
            {
                debugForm = new DebugForm();
                debugForm.InitializeForm(usbDeviceManager);
                debugForm.Show();
            }
            if (debugForm.WindowState == FormWindowState.Minimized)
            {
                debugForm.WindowState = FormWindowState.Normal;
            }
            debugForm.Location = this.Location;
            debugForm.Focus();
        }

        private void updateFirmwareToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for(int i = 0; i < usbDeviceManager.DeviceCount; i++)
            {
                // deinitialize devices to switch to bootloader mode.
                // prepare form for reinitialization
                // usbDeviceManager.GetDevice(i).PauseUsb = true;
                // formIsInitialized = false;
            }
            if (firmwareForm == null || firmwareForm.IsDisposed)
            {
                firmwareForm = new FirmwareUpdateForm();
                firmwareForm.InitializeForm(this, usbDeviceManager);
                firmwareForm.Show();
            }
            if (firmwareForm.WindowState == FormWindowState.Minimized)
            {
                firmwareForm.WindowState = FormWindowState.Normal;
            }
            firmwareForm.Location = this.Location;
            firmwareForm.Focus();
        }

        private void btn_SaveConfig_Click(object sender, EventArgs e)
        {
            usbDeviceManager.GetAppData(selectedAddressIdx).WriteConfigPending = true;
        }

        private void btn_ResetToBoot_Click(object sender, EventArgs e)
        {
            usbDeviceManager.GetAppData(selectedAddressIdx).ResetToBootloaderPending = true;
        }

        private void cbo_DeviceAddress_SelectedIndexChanged(object sender, EventArgs e)
        {
            var item = ((ComboBoxItem)((ComboBox)sender).SelectedItem);
            selectedAddressIdx = item.Value;
            selectedAddress = (int)usbDeviceManager.GetAppData(selectedAddressIdx)?.DeviceControllerData?.DeviceAddress;
            UpdateFormSettings(usbDeviceManager.GetAppData(selectedAddressIdx).DeviceControllerData);
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HideForm();
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ShowForm();
        }

        public void HideForm()
        {
            Hide();
            FormUpdateTimer.Enabled = false;
            isHidden = true;
        }

        public void ShowForm()
        {
            if (isHidden)
            {
                allowVisible = true;
                Show();
                FormUpdateTimer.Enabled = true;
                isHidden = false;
            }
        }

        private void closeToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            allowClose = true;
            this.Close();
        }

        private void txt_ControllerName_Leave(object sender, EventArgs e)
        {
            userSettings.Controllers.Where(c => c.Address == selectedAddress).FirstOrDefault().Name = txt_ControllerName.Text;
        }

        private void startWithWindowsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(!startupManager.IsAvailable)
            {
                MessageBox.Show("The startup service is not available.");
                return;
            }

            startupManager.Startup = !startupManager.Startup;
            startWithWindowsToolStripMenuItem.Checked = startupManager.Startup;
        }

        private void sequencerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (debugForm == null || debugForm.IsDisposed)
            {
                sequencerForm = new SequencerForm();
                sequencerForm.InitializeForm(usbDeviceManager, userSettings, deviceAddressList, selectedAddressIdx);
                sequencerForm.Location = this.Location;
                sequencerForm.Show();
            }
            if (sequencerForm.WindowState == FormWindowState.Minimized)
            {
                sequencerForm.WindowState = FormWindowState.Normal;
            }
            sequencerForm.Focus();
        }
    }
} 