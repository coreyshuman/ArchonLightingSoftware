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
        private ConfigViewForm configView = null;
        private bool configUpdateReady = false;
        private bool formIsInitialized = false;
        private UInt16[] fanBuffer;
        Panel fanSpeedBar1;

        public unsafe AppForm()
        {
            InitializeComponent();

            usbApp = new UsbApp();
            usbApp.RegisterEventHandler(this.Handle);
            usbApp.InitializeDevice("04D8", "0033");

            if (usbApp.IsAttached == true)
            {
                statusLabel.Text = "Device Found, AttachedState = TRUE";
            }
            else
            {
                statusLabel.Text = "Device not found, verify connect/correct firmware";
            }

            fanSpeedBar1 = new Panel();
            fanSpeedBar1.Left = 0;
            fanSpeedBar1.Top = 0;
            fanSpeedBar1.Height = fanPanel1.Height;
            fanSpeedBar1.Width = fanPanel1.Width;
            fanSpeedBar1.Parent = fanPanel1;
            fanSpeedBar1.BackColor = System.Drawing.Color.Blue;
            fanSpeedBar1.Show();

            fanBuffer = new UInt16[10];

            cbo_LightingMode.Items.Add("Off");
            cbo_LightingMode.Items.Add("Steady");
            cbo_LightingMode.Items.Add("Rotate");
        }

        void UpdateFormSettings(DeviceControllerData controller)
        {
            lbl_Address.Text = controller.DeviceAddress.ToString();
            trackBar1.Value = controller.DeviceConfig.FanSpeed[0];
            cbo_LightingMode.SelectedIndex = controller.DeviceConfig.LedMode[0];
            button1.BackColor = GetLedColor(0, 0);
            button2.BackColor = GetLedColor(0, 1);
            button3.BackColor = GetLedColor(0, 2);
            button4.BackColor = GetLedColor(0, 3);
        }

        void SetFanSpeedValue(int value)
        {
            if (value > 3000) value = 3000;
            int i;
            int total = 0;
            for(i=9; i>0; i--)
            {
                fanBuffer[i] = fanBuffer[i - 1]; 
            }
            fanBuffer[0] = (UInt16)value;
            for(i=0; i<10; i++)
            {
                total += fanBuffer[i];
            }
            value = total / 10;

            int size = (int)(fanPanel1.Height * value / 3000);
            int top = fanPanel1.Height - size;
            fanSpeedBar1.Height = size;
            fanSpeedBar1.Top = top;
        }

        //This is a callback function that gets called when a Windows message is received by the form.
        //We will receive various different types of messages, but the ones we really want to use are the WM_DEVICECHANGE messages.
        protected override void WndProc(ref Message m)
        {
            usbApp.HandleWindowEvent(ref m);
            base.WndProc(ref m);
        } 

        // read debug
        private void ToggleLEDs_btn_Click(object sender, EventArgs e)
        {
            usbApp.AppData.ReadDebug = true;
        }

        private void FormUpdateTimer_Tick(object sender, EventArgs e)
        {
            //This timer tick event handler function is used to update the user interface on the form, based on data
            //obtained asynchronously by the ReadWriteThread and the WM_DEVICECHANGE event handler functions.

            //Check if user interface on the form should be enabled or not, based on the attachment state of the USB device.
            if (usbApp.IsAttached == true)
            {
                //Device is connected and ready to communicate, enable user interface on the form 
                statusLabel.Text = "Device Found: AttachedState = TRUE";
                ANxVoltage_lbl.Enabled = true;
                btn_readDebug.Enabled = true;
            }
            if ((usbApp.IsAttached == false) || (usbApp.IsAttachedButBroken == true))
            {
                //Device not available to communicate. Disable user interface on the form.
                statusLabel.Text = "Device Not Detected: Verify Connection/Correct Firmware";
                ANxVoltage_lbl.Enabled = false;
                btn_readDebug.Enabled = false;

                SetFanSpeedValue(0);
            }

            //Update the various status indicators on the form with the latest info obtained from the ReadWriteThread()
            if (usbApp.IsAttached == true)
            {
                if (!formIsInitialized && usbApp.AppData.DeviceControllerData != null && usbApp.AppData.DeviceControllerData.IsInitialized)
                {
                    UpdateFormSettings(usbApp.AppData.DeviceControllerData);
                    formIsInitialized = true;
                }

                SetFanSpeedValue((int)usbApp.AppData.DeviceControllerData.MeasuredFanRpm[0]);
                //lbl_fanSpeed.Text = usbApp.AppData.FanSpeed[0].ToString();
                if(usbApp.AppData.EepromReadDone)
                {
                    for (int i = 0; i < usbApp.AppData.EepromLength; i++)
                    {
                        Control ctn = this.Controls["txt_Data_" + (i + 1)];
                        //ctn.Text = usbApp.AppData.EepromData[i].ToString();
                    }
                    usbApp.AppData.EepromReadDone = false;
                }

                if(configUpdateReady)
                {
                    configUpdateReady = false;
                    usbApp.AppData.DeviceControllerData.DeviceConfig.FanSpeed[0] = (byte)trackBar1.Value;
                    usbApp.AppData.UpdateConfigPending = true;
                }
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {
            
        }

        private void btn_Read_Click(object sender, EventArgs e)
        {
            usbApp.AppData.EepromAddress = (uint)Int32.Parse(num_Addr.Text);
            usbApp.AppData.EepromLength = (uint)Int32.Parse(num_Len.Text);
            usbApp.AppData.EepromReadPending = true;
        }

        private void btn_Write_Click(object sender, EventArgs e)
        {
            usbApp.AppData.EepromAddress = (uint)Int32.Parse(num_Addr.Text);
            usbApp.AppData.EepromLength = (uint)Int32.Parse(num_Len.Text);
            //usbApp.AppData.EepromData[0] = (byte)Int32.Parse(txt_Data_1.Text);
            for (int i = 0; i < usbApp.AppData.EepromLength; i++)
            {
                Control ctn = this.Controls["txt_Data_" + (i + 1)];
                //usbApp.AppData.EepromData[i] = (byte)Int32.Parse(ctn.Text);
            }
            usbApp.AppData.EepromWritePending = true;
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                int len = (int)num_Len.Value;
                if (len < 1 || len > 16) throw new Exception();
                int i = 0;
                for (i = 0; i < len; i++)
                {
                    Control ctn = this.Controls["txt_Data_" + (i + 1)];
                    ctn.Enabled = true;
                }
                for (; i < 16; i++)
                {
                    Control ctn = this.Controls["txt_Data_" + (i + 1)];
                    ctn.Enabled = false;
                }
            }
            catch
            {
                num_Len.Text = "1";
                for (int i = 1; i < 16; i++)
                {
                    Control ctn = this.Controls["txt_Data_" + (i + 1)];
                    ctn.Enabled = false;
                }
            }
        }

        private void btn_ReadConfig_Click(object sender, EventArgs e)
        {
            usbApp.AppData.ReadConfigPending = true;
        }

        private void btn_WriteConfig_Click(object sender, EventArgs e)
        {
            usbApp.AppData.WriteConfigPending = true;
        }

        private void editConfigToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(configView == null || configView.IsDisposed)
            {
                configView = new ConfigViewForm();
                configView.InitializeForm(usbApp.AppData);
                configView.UpdateFormData();
                configView.Show();
            } 
            if(configView.WindowState == FormWindowState.Minimized)
            {
                configView.WindowState = FormWindowState.Normal;
            }
            configView.Focus();
        }

        private void SetLedColor(System.Drawing.Color color, int deviceIdx, int ledIdx)
        {
            ledIdx *= 3;
            usbApp.AppData.DeviceControllerData.DeviceConfig.Colors[deviceIdx, ledIdx++] = color.G;
            usbApp.AppData.DeviceControllerData.DeviceConfig.Colors[deviceIdx, ledIdx++] = color.R;
            usbApp.AppData.DeviceControllerData.DeviceConfig.Colors[deviceIdx, ledIdx++] = color.B;
        }
        
        private System.Drawing.Color GetLedColor(int deviceIdx, int ledIdx)
        {
            ledIdx *= 3;
            byte G = usbApp.AppData.DeviceControllerData.DeviceConfig.Colors[deviceIdx, ledIdx++];
            byte R = usbApp.AppData.DeviceControllerData.DeviceConfig.Colors[deviceIdx, ledIdx++];
            byte B = usbApp.AppData.DeviceControllerData.DeviceConfig.Colors[deviceIdx, ledIdx++];
            return System.Drawing.Color.FromArgb(R, G, B);
        }
        

        private void UpdateColorClickHandler(object sender, int ledIdx)
        {
            colorDialog1.ShowDialog();
            var color = colorDialog1.Color;
            SetLedColor(color, 0, ledIdx);
            ((Button)sender).BackColor = color;
            configUpdateReady = true;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            UpdateColorClickHandler(sender, 0);
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            usbApp.AppData.DeviceControllerData.DeviceConfig.FanSpeed[0] = (byte)((TrackBar)sender).Value;
            configUpdateReady = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            UpdateColorClickHandler(sender, 1);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            UpdateColorClickHandler(sender, 2);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            UpdateColorClickHandler(sender, 3);
        }

        private void cbo_LightingMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            usbApp.AppData.DeviceControllerData.DeviceConfig.LedMode[0] = (byte)cbo_LightingMode.SelectedIndex;
            configUpdateReady = true;
        }
    } 
} 