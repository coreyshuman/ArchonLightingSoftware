using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ArchonLightingSystem.Models;
using ArchonLightingSystem.Common;

namespace ArchonLightingSystem.Components
{
    class ControllerComponent
    {
        private ApplicationData applicationData;
        private Control parentControl;
        private GroupBox grpDev;
        private GroupBox grpFan;
        private FanSpeedBar fanBar;
        private FanSpeedBar tempBar;
        private TrackBar fanCtrl;
        private ComboBox lightingMode;
        private ComboBox lightingSpeed;
        private IList<Button> buttons;
        private Timer fanUpdateTimer;
        private bool isInitialized = false;

        public bool UpdateReady { get; set; }
        public ApplicationData AppData
        {
            get
            {
                return applicationData;
            }
            set
            {
                fanUpdateTimer.Enabled = false;
                isInitialized = false;
                ControlsEnabled(false);
                applicationData = value;
                fanUpdateTimer.Enabled = true;
            }
        }

        public ControllerComponent()
        {
            grpDev = new GroupBox();
            grpFan = new GroupBox();
            fanBar = new FanSpeedBar();
            tempBar = new FanSpeedBar();
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
                if(!isInitialized && applicationData?.DeviceControllerData?.IsInitialized == true)
                {
                    isInitialized = true;
                    UpdatePeripheralSettings(deviceIdx);
                    ControlsEnabled(true);
                }
                if(isInitialized)
                {
                    fanBar.Value = applicationData.DeviceControllerData.MeasuredFanRpm[deviceIdx];
                }
            };
        }

        public void InitializeComponent(Control parent, int index)
        {
            GroupBox deviceTemplate = (GroupBox)parent.Controls["grp_Device1"];
            parentControl = deviceTemplate;
            Button buttonTemplate = (Button)deviceTemplate.Controls["btn_1_1"];
            Label lblModeTemplate = (Label)deviceTemplate.Controls["lbl_LightingMode"];
            Label lblSpeedTemplate = (Label)deviceTemplate.Controls["lbl_LightingSpeed"];
            ComboBox cboModeTemplate = (ComboBox)deviceTemplate.Controls["cbo_LightMode"];
            ComboBox cboSpeedTemplate = (ComboBox)deviceTemplate.Controls["cbo_LightSpeed"];
            GroupBox fanTemplate = (GroupBox)deviceTemplate.Controls["grp_FanSpeed1"];
            Panel fanMarker = (Panel)fanTemplate.Controls["pnl_FanMarker"];
            Panel fanTemperature = (Panel)fanTemplate.Controls["pnl_TemperatureMarker"];
            TrackBar fanCtrlTemplate = (TrackBar)fanTemplate.Controls["trk_FanSpeed1"];
            Button btnFanConfigTemplate = (Button)fanTemplate.Controls["btn_FanConfig"];
            Label lblFanControlTemplate = (Label)fanTemplate.Controls["lbl_FanControls"];
            Label lblFanUnitsTemplate = (Label)fanTemplate.Controls["lbl_FanUnits"];

            Util.CopyObjectProperties<GroupBox>(grpDev, deviceTemplate, new string[] { "ForeColor", "BackColor", "Width", "Top", "Height" });
            grpDev.Left = deviceTemplate.Left + (deviceTemplate.Width + 10) * (index - 1);
            grpDev.Parent = parent;
            grpDev.Text = $"Device {index}";
            grpDev.Font = deviceTemplate.Font;
            grpDev.Show();

            for(int i = 0; i < DeviceControllerDefinitions.LedCountPerDevice; i++)
            {
                Button btn = new Button();
                Util.CopyObjectProperties<Button>(btn, buttonTemplate, new string[] { "FlatStyle", "ForeColor", "BackColor", "Width", "Left", "Height" });
                btn.Top = buttonTemplate.Top + (buttonTemplate.Height + 6) * i;
                btn.Text = $"L{i + 1}";
                btn.Parent = grpDev;
                btn.Show();
                btn.Click += ColorUpdateClickEventHandler(index - 1, i);
                buttons.Add(btn);
            }

            Label lblMode = new Label();
            Util.CopyObjectProperties<Label>(lblMode, lblModeTemplate, new string[] { "Left", "Width", "Top", "Height" });
            lblMode.Parent = grpDev;
            lblMode.Show();

            Label lblSpeed = new Label();
            Util.CopyObjectProperties<Label>(lblSpeed, lblSpeedTemplate, new string[] { "Left", "Width", "Top", "Height" });
            lblSpeed.Parent = grpDev;
            lblSpeed.Show();

            Util.CopyObjectProperties<ComboBox>(lightingMode, cboModeTemplate, new string[] { "FlatStyle", "ForeColor", "BackColor", "Top", "Width", "Left", "Height" });
            lightingMode.Parent = grpDev;
            lightingMode.Show();
            lightingMode.Items.AddRange(LightingModes.GetLightingModes());
            lightingMode.SelectedIndexChanged += LightingModeChangedEventHandler(index - 1);


            Util.CopyObjectProperties<ComboBox>(lightingSpeed, cboSpeedTemplate, new string[] { "FlatStyle", "ForeColor", "BackColor", "Top", "Width", "Left", "Height" });
            lightingSpeed.Parent = grpDev;
            lightingSpeed.Show();
            lightingSpeed.Items.AddRange(LightingModes.GetLightingSpeeds());
            lightingSpeed.SelectedIndexChanged += LightingSpeedChangedEventHandler(index - 1);


            Util.CopyObjectProperties<GroupBox>(grpFan, fanTemplate, new string[] { "ForeColor", "BackColor", "Width", "Top", "Left", "Height", "Text" });
            grpFan.Parent = grpDev;
            grpFan.Name = $"grp_FanSpeed{index}";
            grpFan.Show();

            Label lblFanControls = new Label();
            Util.CopyObjectProperties<Label>(lblFanControls, lblFanControlTemplate, new string[] { "ForeColor", "BackColor", "Left", "Text", "Width", "Top", "Height" });
            lblFanControls.Parent = grpFan;
            lblFanControls.Show();

            Label lblFanUnits = new Label();
            Util.CopyObjectProperties<Label>(lblFanUnits, lblFanUnitsTemplate, new string[] { "ForeColor", "BackColor", "Font", "Left", "Text", "Width", "Top", "Height" });
            lblFanUnits.Parent = grpFan;
            lblFanUnits.Show();

            Button btnFanConfig = new Button();
            Util.CopyObjectProperties<Button>(btnFanConfig, btnFanConfigTemplate, new string[] { "FlatStyle", "ForeColor", "Text", "BackColor", "Width", "Top", "Left", "Height" });
            btnFanConfig.Parent = grpFan;
            btnFanConfig.Show();


            fanBar.Left = fanMarker.Left;
            fanBar.Top = fanMarker.Top;
            fanBar.Width = fanMarker.Width;
            fanBar.Height = fanMarker.Height;
            fanBar.BorderStyle = fanMarker.BorderStyle;
            fanBar.BarColor = System.Drawing.Color.FromArgb(53, 114, 102);
            fanBar.ForeColor = grpFan.ForeColor;
            fanBar.UseAverage = true;
            fanBar.Minimum = 0;
            fanBar.Maximum = 2500;
            fanBar.Value = 0;
            fanBar.Parent = grpFan;
            fanBar.Show();

            tempBar.Left = fanTemperature.Left;
            tempBar.Top = fanTemperature.Top;
            tempBar.Width = fanTemperature.Width;
            tempBar.Height = fanTemperature.Height;
            tempBar.BorderStyle = fanTemperature.BorderStyle;
            tempBar.BarColor = System.Drawing.Color.FromArgb(185, 49, 79);
            tempBar.ForeColor = grpFan.ForeColor;
            tempBar.Minimum = 0;
            tempBar.Maximum = 120;
            tempBar.Value = 0;
            tempBar.Parent = grpFan;
            tempBar.Show();

            Util.CopyObjectProperties<TrackBar>(fanCtrl, fanCtrlTemplate, 
                new string[] { "BackColor", "Width", "Top", "Left", "Height", "Minimum", "Maximum", "TickFrequency", "SmallChange", "LargeChange", "Orientation" });

            fanCtrl.Parent = grpFan;
            fanCtrl.Show();
            fanCtrl.Scroll += TrackScrollEvent(index - 1);
            fanUpdateTimer.Tick += FanUpdateTimerTickHandler(index - 1);

            ControlsEnabled(false);
        }

        public int Temperature
        {
            get
            {
                return tempBar.Value;
            }
            set
            {
                tempBar.Value = value;
            }
        }

        private void ControlsEnabled(bool enable)
        {
            foreach(Control cont in parentControl.Controls)
            {
                cont.Enabled = enable;
            }
        }

        private void UpdatePeripheralSettings(int deviceIdx)
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
            byte ledMode = applicationData.DeviceControllerData.DeviceConfig.LedMode[deviceIdx];
            byte ledSpeed = applicationData.DeviceControllerData.DeviceConfig.LedSpeed[deviceIdx];
            lightingMode.SelectedIndex = ledMode < LightingModes.GetLightingModes().Length ? ledMode : -1;
            lightingSpeed.SelectedIndex = ledSpeed > 0 && ledSpeed < LightingModes.GetLightingSpeeds().Length ? ledSpeed - 1 : -1;
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
                applicationData.DeviceControllerData.DeviceConfig.LedSpeed[deviceIdx] = (byte)(cbox.SelectedIndex + 1);
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
