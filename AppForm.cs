using System;
using System.Windows.Forms;
using ArchonLightingSystem.Components;
using ArchonLightingSystem.Models;
using ArchonLightingSystem.UsbApplication;



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

        public unsafe AppForm()
        {
            InitializeComponent();

            usbApp = new UsbApp();
            InitializeForm();

            usbApp.RegisterEventHandler(this.Handle);
            usbApp.InitializeDevice("04D8", "0033");
        }

        void InitializeForm()
        {
            for(int i = 1; i <= DeviceControllerDefinitions.DeviceCount; i++)
            {
                DeviceComponent device = new DeviceComponent();
                device.InitializeComponent(this, i, usbApp.AppData);

            }
        }

        void UpdateFormSettings(DeviceControllerData controller)
        {
            FormUpdateTimer.Enabled = false;
            lbl_Address.Text = controller.DeviceAddress.ToString();
            FormUpdateTimer.Enabled = true;
        }

        

        //This is a callback function that gets called when a Windows message is received by the form.
        //We will receive various different types of messages, but the ones we really want to use are the WM_DEVICECHANGE messages.
        protected override void WndProc(ref Message m)
        {
            usbApp.HandleWindowEvent(ref m);
            base.WndProc(ref m);
        } 

        private void FormUpdateTimer_Tick(object sender, EventArgs e)
        {
            //This timer tick event handler function is used to update the user interface on the form, based on data
            //obtained asynchronously by the ReadWriteThread and the WM_DEVICECHANGE event handler functions.

            //Check if user interface on the form should be enabled or not, based on the attachment state of the USB device.
            if (usbApp.IsAttached == true)
            {
                //Device is connected and ready to communicate, enable user interface on the form 
                string bootVer = usbApp.AppData.DeviceControllerData.BootloaderVersion?.ToString();
                string AppVer = usbApp.AppData.DeviceControllerData.ApplicationVersion?.ToString();
                statusLabel.Text = $"Device Found: AttachedState = TRUE   BootVer: {bootVer}   AppVer: {AppVer}";
            }
            if ((usbApp.IsAttached == false) || (usbApp.IsAttachedButBroken == true))
            {
                //Device not available to communicate. Disable user interface on the form.
                statusLabel.Text = "Device Not Detected: Verify Connection/Correct Firmware";

                //SetFanSpeedValue(0);
            }

            //Update the various status indicators on the form with the latest info obtained from the ReadWriteThread()
            if (usbApp.IsAttached == true)
            {
                if (!formIsInitialized && usbApp.AppData.DeviceControllerData != null && usbApp.AppData.DeviceControllerData.IsInitialized)
                {
                    UpdateFormSettings(usbApp.AppData.DeviceControllerData);
                    formIsInitialized = true;
                }

                //SetFanSpeedValue((int)usbApp.AppData.DeviceControllerData.MeasuredFanRpm[0]);

            }
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
                configForm.InitializeForm(usbApp.AppData);
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
                eepromForm.InitializeForm(usbApp.AppData);
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
                debugForm.InitializeForm(usbApp.AppData);
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
            usbApp.AppData.WriteConfigPending = true;
        }

        private void btn_ResetToBoot_Click(object sender, EventArgs e)
        {
            usbApp.AppData.ResetToBootloaderPending = true;
        }

        
    } 
} 