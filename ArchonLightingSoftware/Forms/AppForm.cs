using System;
using System.Windows.Forms;
using ArchonLightingSystem.Components;
using ArchonLightingSystem.Models;
using ArchonLightingSystem.Common;
using System.Collections.Generic;
using System.Linq;
using ArchonLightingSystem.Properties;
using ArchonLightingSystem.OpenHardware;
using ArchonLightingSystem.Services;
using ArchonLightingSystem.Forms;
using ArchonLightingSystem.WindowsSystem.Startup;
using ArchonLightingSystem.UsbApplicationV2;
using System.Threading;

namespace ArchonLightingSystem
{
    public partial class AppForm : Form
    {
        public bool FormIsInitialized { get; set; } = false;

        private ConfigEditorForm configForm = null;
        private EepromEditorForm eepromForm = null;
        private DebugForm debugForm = null;
        private FirmwareUpdateForm firmwareForm = null;
        private SensorMonitorManager hardwareManager = null;
        private SequencerForm sequencerForm = null;
        private LogForm logForm = null;
        private StatsForm statsForm = null;
        private StatsConfigForm statsConfigForm = null;
        private StartupManager startupManager = new StartupManager();
        private ServiceManager serviceManager = new ServiceManager();

        UsbApplicationV2.UsbControllerManager usbControllerManager;

        private List<ControllerComponent> deviceComponents = new List<ControllerComponent>();
        private DragWindowSupport dragSupport = new DragWindowSupport();
        private UserSettingsManager settingsManager = new UserSettingsManager();
        private UserSettings userSettings = null;
        private bool isHidden = false;
        private bool allowVisible = false;
        private bool allowClose = false;
        private bool formIsBusy = false;
        private string lastNotification = "";
        private bool disableNotification = false;
        private SemaphoreSlim formUpdateSemaphore = new SemaphoreSlim(1,1);

        private UsbControllerDevice usbControllerDevice = null;

        #region initializers
        public unsafe AppForm(bool startInBackground)
        {
            InitializeComponent();

            Logger.LatestLogEvent += HandleLogEvent;

            Logger.Write(Level.Information, $"Loading ArchonLightingSystem {DateTime.Now}");

            dragSupport.Initialize(this, menuStrip1);
            dragSupport.DragWindowEvent += new DragWindowEventDelegate(DragWindowEventHandler);

            userSettings = settingsManager.GetSettings();
            hardwareManager = new SensorMonitorManager();
            if (Settings.Default.MainWindowLocation.X >= 0)
            {
                this.Location = Settings.Default.MainWindowLocation;
                Logger.Write(Level.Trace, $"Window location setting: {this.Location}");
            }

            // debug testing new manager
            usbControllerManager = new UsbControllerManager();
            usbControllerManager.Register(Handle, Definitions.ApplicationVid, Definitions.ApplicationPid);
            usbControllerManager.UsbControllerEvent += UsbControllerManger_UsbControllerEvent;

            startWithWindowsToolStripMenuItem.Enabled = startupManager.IsAvailable;
            startWithWindowsToolStripMenuItem.Checked = startupManager.Startup;

            // Handle cleanup when the user logs off
            Microsoft.Win32.SystemEvents.SessionEnded += delegate {
                serviceManager.StopServices();
                hardwareManager.Close();
            };

            serviceManager.StartServices(usbControllerManager, hardwareManager);

            allowVisible = !startInBackground;
            isHidden = startInBackground;

            InitializeForm();
            UpdateFormState(null, false);
        }

        void InitializeForm()
        {
            cbo_DeviceAddress.DisplayMember = "Text";
            cbo_DeviceAddress.ValueMember = "Value";

            for (int i = 1; i <= DeviceControllerDefinitions.DevicePerController; i++)
            {
                ControllerComponent component = new ControllerComponent();
                deviceComponents.Add(component);
                component.InitializeComponent(this, hardwareManager, i);
            }

            UpdateDeviceComboBox();

            AppTheme.ApplyThemeToForm(this);

            lbl_Disconnected.ForeColor = AppTheme.PrimaryLowLight;
        }
        #endregion

        #region update_form_methods

        void UpdateStatusBar(string msg) 
        {
            if (InvokeRequired)
            {
                this.BeginInvoke(new Action<string>(UpdateStatusBar), new object[] { msg });
                return;
            }
            statusLabel.Text = msg; 
        }

        public void UpdateFormState(UsbControllerDevice device, bool? busy = null)
        {
            if (InvokeRequired)
            {
                this.BeginInvoke(new Action<UsbControllerDevice, bool?>(UpdateFormState), new object[] { usbControllerDevice, null });
                return;
            }

            formUpdateSemaphore.Wait();

            if (busy.HasValue)
            {
                this.formIsBusy = busy.Value;
            }

            try
            {
                if (device == null)
                {
                    lbl_Disconnected.Visible = true;
                    cbo_DeviceAddress.Enabled = !formIsBusy;
                    txt_ControllerName.Enabled = false;
                    btn_SaveConfig.Enabled = false;
                    chk_AlertOnDisconnect.Enabled = false;
                    return;
                }

                lbl_Disconnected.Visible = device.IsDisconnected;
                cbo_DeviceAddress.Enabled = !formIsBusy;
                txt_ControllerName.Enabled = !formIsBusy;
                btn_SaveConfig.Enabled = device.IsConnected && !formIsBusy;
                chk_AlertOnDisconnect.Enabled = !formIsBusy;
            }
            finally
            {
                formUpdateSemaphore.Release();
            }
        }

        void UpdateFormData(UsbControllerDevice controller)
        {
            if (InvokeRequired)
            {
                this.BeginInvoke(new Action<UsbControllerDevice>(UpdateFormData), new object[] { usbControllerDevice });
                return;
            }

            if(controller == null)
            {
                return; // todo disable form components
            }

            txt_ControllerName.Text = controller.Settings.Name;
            chk_AlertOnDisconnect.Checked = controller.Settings.AlertOnDisconnect;

            for (int i = 0; i < DeviceControllerDefinitions.DevicePerController; i++)
            {
                deviceComponents[i].UpdateComponentData(controller);
            }
        }

        private void UpdateDeviceComboBox()
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action(UpdateDeviceComboBox));
                return;
            }

            var lastSelected = (ComboBoxItem)cbo_DeviceAddress.SelectedItem;
            cbo_DeviceAddress.Items.Clear();
            cbo_DeviceAddress.Items.AddRange(
                usbControllerManager.ActiveControllers.Select(c =>
                    new ComboBoxItem() { 
                        Text = $"{c.Address} {c.Settings.Name}",
                        Value = c.Address
                    }
                ).ToArray()
            );
            if(lastSelected != null)
            {
                var selectedController = usbControllerManager.ActiveControllers.Where(c => c.Address == lastSelected.Value).FirstOrDefault();
                cbo_DeviceAddress.SelectedIndex = usbControllerManager.ActiveControllers.IndexOf(selectedController);
            }

            if (cbo_DeviceAddress.Items.Count > 0 && cbo_DeviceAddress.SelectedItem == null)
            {
                cbo_DeviceAddress.SelectedIndex = 0;
            }
        }
        
        public void HideForm()
        {
            Hide();
            isHidden = true;
            foreach (var deviceComponent in deviceComponents)
            {
                deviceComponent.Hide();
            }
        }

        public void ShowForm()
        {
            if (isHidden)
            {
                allowVisible = true;
                Show();
                isHidden = false;
                foreach (var deviceComponent in deviceComponents)
                {
                    deviceComponent.Show();
                }
            }
        }
        #endregion

        #region notify_icon
        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            SetErrorIcon(false);
            ShowForm();
        }

        private void ShowTaskbarNotification(string title, string content, int duration = 5000)
        {
            // simple skip to ignore repeating notifications (failed device retrying, for example)
            //if (content == lastNotification) return;

            if (disableNotification) return;

           // if (notifyIcon1.Visible) return;

            if (InvokeRequired)
            {
                this.Invoke(new Action<string, string, int>(ShowTaskbarNotification), new object[] { title, content, duration });
                return;
            }

            notifyIcon1.BalloonTipTitle = title;
            notifyIcon1.BalloonTipText = content;
            notifyIcon1.BalloonTipIcon = ToolTipIcon.Error;
            notifyIcon1.Icon = Resources.darkarchon;
            notifyIcon1.ShowBalloonTip(duration);
            lastNotification = content;
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
        #endregion

        #region event_handlers
        //This is a callback function that gets called when a Windows message is received by the form.
        //We will receive various different types of messages, but the ones we really want to use are the WM_DEVICECHANGE messages.
        protected override void WndProc(ref Message m)
        {
            usbControllerManager?.HandleWindowEvent(ref m);
            base.WndProc(ref m);
        }

        private void UsbControllerManger_UsbControllerEvent(object sender, UsbApplicationV2.UsbControllerEventArgs e)
        {
            UpdateDeviceComboBox();
            UpdateFormState(usbControllerDevice, null);
            UpdateFormData(usbControllerDevice);
        }

        void DragWindowEventHandler(object sender, DragWindowEventArgs args)
        {
            Settings.Default.MainWindowLocation = args.Location;
            Settings.Default.Save();
        }

        void HandleLogEvent(object sender, LogEventArgs eventArgs)
        {
            UpdateStatusBar(eventArgs.Log.Message);
            if (eventArgs.Log.Level == Level.Error)
            {
                ShowTaskbarNotification("Archon Error", eventArgs.Log.Message);
                SetErrorIcon(true);
            }
        }
        #endregion

        #region form_events
        private void txt_ControllerName_Leave(object sender, EventArgs e)
        {
            usbControllerDevice.Settings.Name = txt_ControllerName.Text;
            usbControllerDevice.Settings.Save();
        }

        private void AppForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            serviceManager.StopServices();
            hardwareManager.Close();
        }

        private void chk_AlertOnDisconnect_CheckedChanged(object sender, EventArgs e)
        {
            var check = (CheckBox)sender;

            usbControllerDevice.Settings.AlertOnDisconnect = check.Checked;
            usbControllerDevice.Settings.Save();
        }

        private void btn_SaveConfig_Click(object sender, EventArgs e)
        {
            usbControllerDevice.AppData.WriteConfigPending = true;
        }

        private void cbo_DeviceAddress_SelectedIndexChanged(object sender, EventArgs e)
        {
            var cbo = (ComboBox)sender;

            if (cbo.SelectedIndex == -1)
            {
                return;
            }

            var item = (ComboBoxItem)(cbo.SelectedItem);

            usbControllerDevice = usbControllerManager.GetControllerByAddress(item.Value);
            UpdateFormState(usbControllerDevice, null);
            UpdateFormData(usbControllerDevice);
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

        // Override visibility behavior
        protected override void SetVisibleCore(bool value)
        {
            if (!allowVisible)
            {
                value = false;
                if (!this.IsHandleCreated) CreateHandle();
            }
            base.SetVisibleCore(value);
        }
        #endregion

        #region toolstrip
        private void toolStripMenuItem_ViewLog_Click(object sender, EventArgs e)
        {
            if (logForm == null || logForm.IsDisposed)
            {
                logForm = new LogForm();
                logForm.Location = this.Location;
                logForm.Show();
            }
            if (logForm.WindowState == FormWindowState.Minimized)
            {
                logForm.WindowState = FormWindowState.Normal;
            }
            logForm.Focus();
        }

        private void startWithWindowsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!startupManager.IsAvailable)
            {
                MessageBox.Show("The startup service is not available.");
                return;
            }

            startupManager.Startup = !startupManager.Startup;
            startWithWindowsToolStripMenuItem.Checked = startupManager.Startup;
        }

        private void sequencerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (sequencerForm == null || sequencerForm.IsDisposed)
            {
                sequencerForm = new SequencerForm();
                sequencerForm.InitializeForm(usbControllerManager);
                sequencerForm.Location = this.Location;
                sequencerForm.Show();
            }
            if (sequencerForm.WindowState == FormWindowState.Minimized)
            {
                sequencerForm.WindowState = FormWindowState.Normal;
            }
            sequencerForm.Focus();
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HideForm();
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
                configForm.InitializeForm(usbControllerManager);
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
                eepromForm.InitializeForm(usbControllerManager);
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
                debugForm.InitializeForm(usbControllerManager);
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
            if (firmwareForm == null || firmwareForm.IsDisposed)
            {
                firmwareForm = new FirmwareUpdateForm();
            }

            firmwareForm.Location = this.Location;
            usbControllerManager.SuppressErrors(true);
            serviceManager.PauseServices(true);

            foreach (var controller in usbControllerManager.ActiveControllers)
            {
                if (controller.IsConnected)
                    controller.AppData.ResetToBootloaderPending = true;
            }

            firmwareForm.ShowDialog(this);
            serviceManager.PauseServices(false);
            usbControllerManager.SuppressErrors(false);
            firmwareForm.Dispose();
        }

        private void closeToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            allowClose = true;
            this.Close();
        }

        private void disableNotificationToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            var notifMenuItem = (ToolStripMenuItem)sender;

            disableNotification = notifMenuItem.Checked;
        }

        private void showStatsToolStripMenuItem_ShowStats_Click(object sender, EventArgs e)
        {
            if (statsForm == null || statsForm.IsDisposed)
            {
                statsForm = new StatsForm();
                statsForm.InitializeForm(hardwareManager);
                
                statsForm.Show();
            }
            if (statsForm.WindowState == FormWindowState.Minimized)
            {
                statsForm.WindowState = FormWindowState.Normal;
            }
            statsForm.Location = this.Location;
            statsForm.Focus();
        }

        private void configToolStripMenuItem_StatsConfigure_Click(object sender, EventArgs e)
        {
            if (statsConfigForm == null || statsConfigForm.IsDisposed)
            {
                statsConfigForm = new StatsConfigForm();
                statsConfigForm.InitializeForm(statsForm);
                statsConfigForm.Location = this.Location;
                statsConfigForm.ShowDialog();
            }
            if (statsConfigForm.WindowState == FormWindowState.Minimized)
            {
                statsConfigForm.WindowState = FormWindowState.Normal;
            }
            statsConfigForm.Focus();
        }

        #endregion


    }
} 