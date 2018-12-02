using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ArchonLightingSystem.Models;
using ArchonLightingSystem.Common;
using System.Drawing;
using System.Diagnostics;
using ArchonLightingSystem.OpenHardware;
using OpenHardwareMonitor.Hardware;

namespace ArchonLightingSystem.Components
{
    public class ControllerComponent
    {
        private static Color lastColor;
        private static List<Color> lastColorsAll = new List<Color>();
        private static ColorDialog colorDialog = new ColorDialog();

        private ApplicationData applicationData;
        private HardwareManager hardwareManager;
        private Form parentForm;
        private Control parentControl;
        private GroupBox grpDev;
        private GroupBox grpFan;
        private FanSpeedBar fanBar;
        private FanSpeedBar tempBar;
        private TrackBar fanCtrl;
        private ComboBox lightingMode;
        private ComboBox lightingSpeed;
        private TextBox txtDeviceName;
        private Button btnFanConfig;
        private IList<Button> buttons;
        private Timer fanUpdateTimer;
        private bool isInitialized = false;
        private UserSettings userSettings = null;
        private int devIdx = 0;
        private ISensor sensor = null;
        private int controllerAddress = 0;

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
                LoadConfig();
            }
        }

        public UserSettings UserSettings
        {
            get
            {
                return userSettings;
            }
            set
            {
                userSettings = value;
                LoadConfig();
            }
        }

        

        public int ControllerAddress
        {
            get
            {
                return controllerAddress;
            }
            set
            {
                controllerAddress = value;
                LoadConfig();
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
            txtDeviceName = new TextBox();
            fanUpdateTimer = new Timer();
            fanUpdateTimer.Interval = 200;
        }

        private DeviceSettings GetDeviceSettings()
        {
            return userSettings.Controllers.Where(c => c.Address == controllerAddress).FirstOrDefault().Devices.Where(d => d.Index == devIdx).FirstOrDefault();
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
                    if(sensor != null && sensor.Value.HasValue)
                    {
                        tempBar.Value = (int)Math.Round(sensor.Value.Value);
                    }
                    else
                    {
                        tempBar.Value = 0;
                    }
                }
            };
        }

        public void InitializeComponent(Form parent, HardwareManager hm, int compNum)
        {
            parentForm = parent;
            hardwareManager = hm;
            devIdx = compNum - 1;
            GroupBox deviceTemplate = (GroupBox)parent.Controls["grp_Device1"];
            parentControl = deviceTemplate;
            Button buttonTemplate = (Button)deviceTemplate.Controls["btn_1_1"];
            Label lblModeTemplate = (Label)deviceTemplate.Controls["lbl_LightingMode"];
            Label lblSpeedTemplate = (Label)deviceTemplate.Controls["lbl_LightingSpeed"];
            Label lblNameTemplate = (Label)deviceTemplate.Controls["lbl_Name"];
            TextBox txtNameTemplate = (TextBox)deviceTemplate.Controls["txt_Name"];
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
            grpDev.Left = deviceTemplate.Left + (deviceTemplate.Width + 10) * (compNum - 1);
            grpDev.Parent = parent;
            grpDev.Text = $"Device {compNum}";
            grpDev.Font = deviceTemplate.Font;
            grpDev.Name = $"deviceGroup_{compNum}";
            grpDev.Show();

            for(int i = 0; i < DeviceControllerDefinitions.LedCountPerDevice; i++)
            {
                Button btn = new Button();
                Util.CopyObjectProperties<Button>(btn, buttonTemplate, new string[] { "FlatStyle", "ForeColor", "BackColor", "Width", "Left", "Height" });
                btn.Top = buttonTemplate.Top + (buttonTemplate.Height + 8) * i;
                btn.Text = $"L{i + 1}";
                btn.Parent = grpDev;
                btn.Name = $"ledBtn_{i}";
                btn.Show();
                btn.Click += ColorUpdateClickEventHandler(compNum - 1, i);
                buttons.Add(btn);
            }

            Label lblMode = new Label();
            Util.CopyObjectProperties<Label>(lblMode, lblModeTemplate, new string[] { "ForeColor", "BackColor", "Left", "Text", "Width", "Top", "Height" });
            lblMode.Parent = grpDev;
            lblMode.Show();

            Label lblSpeed = new Label();
            Util.CopyObjectProperties<Label>(lblSpeed, lblSpeedTemplate, new string[] { "ForeColor", "BackColor", "Left", "Text", "Width", "Top", "Height" });
            lblSpeed.Parent = grpDev;
            lblSpeed.Show();

            Label lblName = new Label();
            Util.CopyObjectProperties<Label>(lblName, lblNameTemplate, new string[] { "ForeColor", "BackColor", "Left", "Text", "Width", "Top", "Height" });
            lblName.Parent = grpDev;
            lblName.Show();

            Util.CopyObjectProperties<TextBox>(txtDeviceName, txtNameTemplate, new string[] { "FlatStyle", "ForeColor", "BackColor", "Top", "Width", "Left", "Height" });
            txtDeviceName.Parent = grpDev;
            txtDeviceName.Show();
            txtDeviceName.Leave += TextNameChangedEventHandler(compNum - 1);

            Util.CopyObjectProperties<ComboBox>(lightingMode, cboModeTemplate, new string[] { "FlatStyle", "ForeColor", "BackColor", "Top", "Width", "Left", "Height" });
            lightingMode.Parent = grpDev;
            lightingMode.Show();
            lightingMode.Items.AddRange(LightingModes.GetLightingModes());
            lightingMode.SelectedIndexChanged += LightingModeChangedEventHandler(compNum - 1);


            Util.CopyObjectProperties<ComboBox>(lightingSpeed, cboSpeedTemplate, new string[] { "FlatStyle", "ForeColor", "BackColor", "Top", "Width", "Left", "Height" });
            lightingSpeed.Parent = grpDev;
            lightingSpeed.Show();
            lightingSpeed.Items.AddRange(LightingModes.GetLightingSpeeds());
            lightingSpeed.SelectedIndexChanged += LightingSpeedChangedEventHandler(compNum - 1);


            Util.CopyObjectProperties<GroupBox>(grpFan, fanTemplate, new string[] { "ForeColor", "BackColor", "Width", "Top", "Left", "Height", "Text" });
            grpFan.Parent = grpDev;
            grpFan.Name = $"grp_FanSpeed{compNum}";
            grpFan.Show();

            Label lblFanControls = new Label();
            Util.CopyObjectProperties<Label>(lblFanControls, lblFanControlTemplate, new string[] { "ForeColor", "BackColor", "Left", "Text", "Width", "Top", "Height" });
            lblFanControls.Parent = grpFan;
            lblFanControls.Show();

            Label lblFanUnits = new Label();
            Util.CopyObjectProperties<Label>(lblFanUnits, lblFanUnitsTemplate, new string[] { "ForeColor", "BackColor", "Font", "Left", "Text", "Width", "Top", "Height" });
            lblFanUnits.Parent = grpFan;
            lblFanUnits.Show();

            btnFanConfig = new Button();
            Util.CopyObjectProperties<Button>(btnFanConfig, btnFanConfigTemplate, new string[] { "FlatStyle", "ForeColor", "Text", "BackColor", "Width", "Top", "Left", "Height" });
            btnFanConfig.Parent = grpFan;
            btnFanConfig.Click += OpenConfigFormEventHandler(devIdx);
            btnFanConfig.Show();


            fanBar.Left = fanMarker.Left;
            fanBar.Top = fanMarker.Top;
            fanBar.Width = fanMarker.Width;
            fanBar.Height = fanMarker.Height;
            fanBar.BorderStyle = fanMarker.BorderStyle;
            fanBar.BarColor = AppColors.PrimaryHighlight;
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
            tempBar.UseAverage = true;
            tempBar.Minimum = 0;
            tempBar.Maximum = 120;
            tempBar.Value = 0;
            tempBar.Parent = grpFan;
            tempBar.Show();

            Util.CopyObjectProperties<TrackBar>(fanCtrl, fanCtrlTemplate, 
                new string[] { "BackColor", "Width", "Top", "Left", "Height", "Minimum", "Maximum", "TickFrequency", "SmallChange", "LargeChange", "Orientation" });

            fanCtrl.Parent = grpFan;
            fanCtrl.Show();
            fanCtrl.Scroll += TrackScrollEvent(compNum - 1);
            fanUpdateTimer.Tick += FanUpdateTimerTickHandler(compNum - 1);

            ControlsEnabled(false);
        }

        private EventHandler OpenConfigFormEventHandler(int deviceIdx)
        {
            return (object sender, EventArgs e) =>
            {
                ArchonLightingSystem.Forms.FanConfigurationForm form = new ArchonLightingSystem.Forms.FanConfigurationForm();
                form.InitializeForm(applicationData, hardwareManager, GetDeviceSettings());
                form.Location = parentForm.Location;
                DialogResult res = form.ShowDialog(parentForm);
                if(res == DialogResult.OK)
                {
                    userSettings.Save();
                    UpdateControlsUsingSettings();
                }
                else
                {
                    userSettings.RevertChanges();
                }
            };
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

        private void LoadConfig()
        {
            if (userSettings == null) return;

            txtDeviceName.Text = GetDeviceSettings().Name;

            UpdateControlsUsingSettings();
        }

        private void UpdateControlsUsingSettings()
        {
            var deviceSettings = GetDeviceSettings();

            fanCtrl.Enabled = !deviceSettings.UseFanCurve;
            btnFanConfig.Text = deviceSettings.SensorName.IsNotNullOrEmpty() ? deviceSettings.SensorName : "Configure";
            sensor = hardwareManager.GetSensorByIdentifier(deviceSettings.Sensor);
        }

        private void UpdatePeripheralSettings(int deviceIdx)
        {
            fanCtrl.Value = applicationData.DeviceControllerData.DeviceConfig.FanSpeed[deviceIdx];
            buttons.Select((Button btn, int index) => {
                btn.BackColor = GetLedColor(deviceIdx, index, true);
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

        private EventHandler TextNameChangedEventHandler(int deviceIdx)
        {
            return (object sender, EventArgs e) =>
            {
                try
                {
                    TextBox txt = (TextBox)sender;
                    if (userSettings != null)
                    {
                        GetDeviceSettings().Name = txt.Text;
                        userSettings.Save();
                    }
                }
                catch(Exception ex)
                {
                    Trace.WriteLine(ex.Message);
                }
                
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
                var color = lastColor;
                Button btn = (Button)sender;

                if ((Control.ModifierKeys & (Keys.Shift | Keys.Control)) == (Keys.Shift | Keys.Control))
                {
                    // update all leds for device from single color
                    for (int i = 0; i < DeviceControllerDefinitions.LedCountPerDevice; i++)
                    {
                        UpdateLedColor(color, deviceIdx, i);
                    }
                    applicationData.UpdateConfigPending = true;
                }
                else if ((Control.ModifierKeys & (Keys.Shift | Keys.Alt)) == (Keys.Shift | Keys.Alt))
                {
                    if (lastColorsAll.Count == DeviceControllerDefinitions.LedCountPerDevice)
                    {
                        // update all leds for device copied from other device
                        for (int i = 0; i < DeviceControllerDefinitions.LedCountPerDevice; i++)
                        {
                            UpdateLedColor(lastColorsAll[i], deviceIdx, i);
                        }
                        applicationData.UpdateConfigPending = true;
                    }
                }
                else if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift)
                {
                    // copy color from button
                    lastColor = GetLedColor(deviceIdx, ledIdx);
                    lastColorsAll.Clear();
                    for (int i = 0; i < DeviceControllerDefinitions.LedCountPerDevice; i++)
                    {
                        lastColorsAll.Add(GetLedColor(deviceIdx, i));
                    }
                }
                else if ((Control.ModifierKeys & Keys.Control) == Keys.Control)
                {
                    // paste last color to button
                    UpdateLedColor(color, deviceIdx, ledIdx);
                    applicationData.UpdateConfigPending = true;
                }
                else
                {
                    color = GetColorFromDialog(GetLedColor(deviceIdx, ledIdx));
                    UpdateLedColor(color, deviceIdx, ledIdx);
                    applicationData.UpdateConfigPending = true;
                }
            };
        }

        private void UpdateLedColor(Color color, int deviceIdx, int ledIdx)
        {
            SetLedColor(color, deviceIdx, ledIdx);
            color = GetLedColor(deviceIdx, ledIdx, true);
            Button btn = (Button)grpDev.Controls[$"ledBtn_{ledIdx}"];
            if (btn != null)
            {
                btn.BackColor = color;
                if (color.G == 0 || color.R == 0)
                {
                    btn.ForeColor = System.Drawing.Color.White;
                }
                else
                {
                    btn.ForeColor = System.Drawing.Color.Black;
                }
            }
        }

        private static Color GetColorFromDialog(Color? currentColor)
        {
            if(currentColor.HasValue)
            {
                colorDialog.Color = currentColor.Value;
            }
            colorDialog.ShowDialog();
            lastColor = colorDialog.Color;
            return lastColor;
        }

        private void SetLedColor(System.Drawing.Color color, int deviceIdx, int ledIdx)
        {
            ledIdx *= 3;
            applicationData.DeviceControllerData.DeviceConfig.Colors[deviceIdx, ledIdx++] = color.G;
            applicationData.DeviceControllerData.DeviceConfig.Colors[deviceIdx, ledIdx++] = color.R;
            applicationData.DeviceControllerData.DeviceConfig.Colors[deviceIdx, ledIdx++] = color.B;
        }

        private System.Drawing.Color GetLedColor(int deviceIdx, int ledIdx, bool lighterColorScale = false)
        {
            ledIdx *= 3;
            byte G = applicationData.DeviceControllerData.DeviceConfig.Colors[deviceIdx, ledIdx++];
            byte R = applicationData.DeviceControllerData.DeviceConfig.Colors[deviceIdx, ledIdx++];
            byte B = applicationData.DeviceControllerData.DeviceConfig.Colors[deviceIdx, ledIdx++];

            if (lighterColorScale)
            {
                if (G > 0)
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
            }
            return System.Drawing.Color.FromArgb(R, G, B);
        }

       
    }
}
