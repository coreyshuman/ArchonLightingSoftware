using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ArchonLightingSystem.Models;

namespace ArchonLightingSystem.Components
{
    class DeviceComponent
    {
        ApplicationData applicationData;
        GroupBox grpDev;
        GroupBox grpFan;
        FanSpeedBar fanBar;
        TrackBar fanCtrl;
        ComboBox lightingMode;
        ComboBox lightingSpeed;
        IList<Button> buttons;
        Timer fanUpdateTimer;
        bool isInitialized = false;

        public bool UpdateReady {get; set;}

        public DeviceComponent()
        {
            grpDev = new GroupBox();
            grpFan = new GroupBox();
            fanBar = new FanSpeedBar();
            fanCtrl = new TrackBar();
            buttons = new List<Button>();
            lightingMode = new ComboBox();
            lightingSpeed = new ComboBox();
            fanUpdateTimer = new Timer();
            fanUpdateTimer.Interval = 100;
            
        }

        private EventHandler FanUpdateTimerTickHandler(int deviceIdx)
        {
            return (object sender, EventArgs e) =>
            {
                if(!isInitialized && applicationData.DeviceControllerData.IsInitialized)
                {
                    isInitialized = true;
                    UpdateDeviceSettings(deviceIdx);
                }
                fanBar.Value = applicationData.DeviceControllerData.MeasuredFanRpm[deviceIdx];
            };
        }

        public void InitializeComponent(Control parent, int index, ApplicationData appData)
        {
            applicationData = appData;
            GroupBox deviceTemplate = (GroupBox)parent.Controls["grp_Device1"];
            Button buttonTemplate = (Button)deviceTemplate.Controls["btn_1_1"];
            Label lblModeTemplate = (Label)deviceTemplate.Controls["lbl_LightingMode"];
            Label lblSpeedTemplate = (Label)deviceTemplate.Controls["lbl_LightingSpeed"];
            ComboBox cboModeTemplate = (ComboBox)deviceTemplate.Controls["cbo_LightMode"];
            ComboBox cboSpeedTemplate = (ComboBox)deviceTemplate.Controls["cbo_LightSpeed"];
            GroupBox fanTemplate = (GroupBox)deviceTemplate.Controls["grp_FanSpeed1"];
            Panel fanMarker = (Panel)fanTemplate.Controls["pnl_FanMarker"];
            TrackBar fanCtrlTemplate = (TrackBar)fanTemplate.Controls["trk_FanSpeed1"];

            grpDev.Top = deviceTemplate.Top;
            grpDev.Left = deviceTemplate.Left + (deviceTemplate.Width + 10) * (index - 1);
            grpDev.Width = deviceTemplate.Width;
            grpDev.Height = deviceTemplate.Height;
            grpDev.Parent = parent;
            grpDev.Text = $"Device {index}";
            grpDev.Font = deviceTemplate.Font;
            grpDev.Show();

            for(int i = 0; i < DeviceControllerDefinitions.LedCountPerDevice; i++)
            {
                Button btn = new Button();
                btn.Top = buttonTemplate.Top + (buttonTemplate.Height + 6) * i;
                btn.Width = buttonTemplate.Width;
                btn.Left = buttonTemplate.Left;
                btn.Height = buttonTemplate.Height;
                btn.Text = $"L{i + 1}";
                btn.Parent = grpDev;
                btn.Show();
                btn.Click += ColorUpdateClickEventHandler(index - 1, i);
                buttons.Add(btn);
            }

            Label lblMode = new Label();
            lblMode.Top = lblModeTemplate.Top;
            lblMode.Left = lblModeTemplate.Left;
            lblMode.Text = lblModeTemplate.Text;
            lblMode.Width = lblModeTemplate.Width;
            lblMode.Parent = grpDev;
            lblMode.Show();

            Label lblSpeed = new Label();
            lblSpeed.Top = lblSpeedTemplate.Top;
            lblSpeed.Left = lblSpeedTemplate.Left;
            lblSpeed.Text = lblSpeedTemplate.Text;
            lblSpeed.Width = lblSpeedTemplate.Width;
            lblSpeed.Parent = grpDev;
            lblSpeed.Show();

            lightingMode.Top = cboModeTemplate.Top;
            lightingMode.Left = cboModeTemplate.Left;
            lightingMode.Width = cboModeTemplate.Width;
            lightingMode.Height = cboModeTemplate.Height;
            //cboMode.Text = lblSpeedTemplate.Text;
            lightingMode.Parent = grpDev;
            lightingMode.Show();
            lightingMode.Items.AddRange(LightingModes.GetLightingModes());
            lightingMode.SelectedIndexChanged += LightingModeChangedEventHandler(index - 1);

            lightingSpeed.Top = cboSpeedTemplate.Top;
            lightingSpeed.Left = cboSpeedTemplate.Left;
            lightingSpeed.Width = cboSpeedTemplate.Width;
            lightingSpeed.Height = cboSpeedTemplate.Height;
            //cboMode.Text = lblSpeedTemplate.Text;
            lightingSpeed.Parent = grpDev;
            lightingSpeed.Show();
            lightingSpeed.Items.AddRange(LightingModes.GetLightingSpeeds());
            lightingSpeed.SelectedIndexChanged += LightingSpeedChangedEventHandler(index - 1);


            grpFan.Top = fanTemplate.Top;
            grpFan.Left = fanTemplate.Left;
            grpFan.Width = fanTemplate.Width;
            grpFan.Height = fanTemplate.Height;
            grpFan.Parent = grpDev;
            grpFan.Text = "Fan Speed";
            grpFan.Name = $"grp_FanSpeed{index}";
            grpFan.Show();


            fanBar.Left = fanMarker.Left;
            fanBar.Top = fanMarker.Top;
            fanBar.Width = fanMarker.Width;
            fanBar.Height = fanMarker.Height;
            fanBar.BorderStyle = fanMarker.BorderStyle;
            fanBar.Minimum = 0;
            fanBar.Maximum = 3000;
            fanBar.Value = 3000;
            fanBar.Parent = grpFan;
            fanBar.Show();


            fanCtrl.Top = fanCtrlTemplate.Top;
            fanCtrl.Left = fanCtrlTemplate.Left;
            fanCtrl.Width = fanCtrlTemplate.Width;
            fanCtrl.Height = fanCtrlTemplate.Height;
            fanCtrl.Minimum = fanCtrlTemplate.Minimum;
            fanCtrl.Maximum = fanCtrlTemplate.Maximum;
            fanCtrl.TickFrequency = fanCtrlTemplate.TickFrequency;
            fanCtrl.SmallChange = fanCtrlTemplate.SmallChange;
            fanCtrl.LargeChange = fanCtrlTemplate.LargeChange;
            fanCtrl.Orientation = fanCtrlTemplate.Orientation;
            fanCtrl.Parent = grpFan;
            fanCtrl.Show();
            fanCtrl.Scroll += TrackScrollEvent(index - 1);
            fanUpdateTimer.Tick += FanUpdateTimerTickHandler(index - 1);
            fanUpdateTimer.Enabled = true;
        }

        private void UpdateDeviceSettings(int deviceIdx)
        {
            fanCtrl.Value = applicationData.DeviceControllerData.DeviceConfig.FanSpeed[deviceIdx];
            buttons.Select((Button btn, int index) => {
                btn.BackColor = GetLedColor(deviceIdx, index);
                if (btn.BackColor.G == 0 || btn.BackColor.R == 0)
                {
                    btn.ForeColor = System.Drawing.Color.White;
                }
                else
                {
                    btn.ForeColor = System.Drawing.Color.Black;
                }
                return btn;
            }).ToArray();
            lightingMode.SelectedIndex = applicationData.DeviceControllerData.DeviceConfig.LedMode[deviceIdx];
            lightingSpeed.SelectedIndex = applicationData.DeviceControllerData.DeviceConfig.LedSpeed[deviceIdx];
        }

        private EventHandler TrackScrollEvent(int deviceIdx)
        {
            return (object sender, EventArgs e) =>
            {
                applicationData.DeviceControllerData.DeviceConfig.FanSpeed[deviceIdx] = (byte)((TrackBar)sender).Value;
                applicationData.UpdateConfigPending = true;
            };
        }

        private EventHandler LightingModeChangedEventHandler(int deviceIdx)
        {
            return (object sender, EventArgs e) =>
            {
                ComboBox cbox = (ComboBox)sender;
                applicationData.DeviceControllerData.DeviceConfig.LedMode[deviceIdx] = (byte)cbox.SelectedIndex;
                applicationData.UpdateConfigPending = true;
            };
        }

        private EventHandler LightingSpeedChangedEventHandler(int deviceIdx)
        {
            return (object sender, EventArgs e) =>
            {
                ComboBox cbox = (ComboBox)sender;
                applicationData.DeviceControllerData.DeviceConfig.LedSpeed[deviceIdx] = (byte)cbox.SelectedIndex;
                applicationData.UpdateConfigPending = true;
            };
        }

        private EventHandler ColorUpdateClickEventHandler(int deviceIdx, int ledIdx)
        {
            return (object sender, EventArgs e) =>
            {
                Button btn = (Button)sender;
                ColorDialog colorDialog = new ColorDialog();
                colorDialog.ShowDialog();
                var color = colorDialog.Color;
                SetLedColor(color, deviceIdx, ledIdx);
                color = GetLedColor(deviceIdx, ledIdx);
                btn.BackColor = color;
                if(color.G == 0 || color.R == 0)
                {
                    btn.ForeColor = System.Drawing.Color.White;
                }
                else
                {
                    btn.ForeColor = System.Drawing.Color.Black;
                }
                applicationData.UpdateConfigPending = true;
            };
        }


        private void SetLedColor(System.Drawing.Color color, int deviceIdx, int ledIdx)
        {
            ledIdx *= 3;
            applicationData.DeviceControllerData.DeviceConfig.Colors[deviceIdx, ledIdx++] = color.G;
            applicationData.DeviceControllerData.DeviceConfig.Colors[deviceIdx, ledIdx++] = color.R;
            applicationData.DeviceControllerData.DeviceConfig.Colors[deviceIdx, ledIdx++] = color.B;
        }

        private System.Drawing.Color GetLedColor(int deviceIdx, int ledIdx)
        {
            ledIdx *= 3;
            byte G = applicationData.DeviceControllerData.DeviceConfig.Colors[deviceIdx, ledIdx++];
            byte R = applicationData.DeviceControllerData.DeviceConfig.Colors[deviceIdx, ledIdx++];
            byte B = applicationData.DeviceControllerData.DeviceConfig.Colors[deviceIdx, ledIdx++];

            if(G > 0)
            {
                G = (byte)(G / 2 + 128);
            }
            if (R > 0)
            {
                R = (byte)(R / 2 + 128);
            }
            if (B > 0)
            {
                B = (byte)(B / 3 + 170);
            }
            return System.Drawing.Color.FromArgb(R, G, B);
        }

       
    }
}
