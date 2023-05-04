﻿using System;
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
using LibreHardwareMonitor.Hardware;
using ArchonLightingSystem.UsbApplicationV2;

namespace ArchonLightingSystem.Components
{
    public class ControllerComponent
    {
        private static Color lastColor;
        private static List<Color> lastColorsAll = new List<Color>();
        private static ColorDialog colorDialog = new ColorDialog();

        private UsbControllerDevice usbController = null;
        private DeviceSettings deviceSettings = null;
        private DeviceControllerData deviceControllerData = null;
        private ApplicationData applicationData;

        private SensorMonitorManager hardwareManager;
        private Form parentForm;
        private Control parentControl;
        private GroupBox grpDev;
        private GroupBox grpFan;
        private FanSpeedBar fanBar;
        private FanSpeedBar sensorBar;
        private TrackBar fanCtrl;
        private ComboBox lightingMode;
        private ComboBox lightingSpeed;
        private TextBox txtDeviceName;
        private Button btnFanConfig;
        private IList<Button> buttons;
        private Timer fanUpdateTimer;
        Label lblSensorUnits;

        private int devIdx = 0;
        private ISensor sensor = null;

        public bool UpdateReady { get; set; }

        public void UpdateComponentData(UsbControllerDevice controller)
        {
            fanUpdateTimer.Enabled = false;
            ControlsEnabled(false);
            usbController = controller;
            deviceControllerData = usbController.ControllerData;
            applicationData = usbController.AppData;
            LoadConfig();
            UpdateUI(devIdx);
            ControlsEnabled(controller.IsConnected);
            fanUpdateTimer.Enabled = true;
        }

        public ControllerComponent()
        {
            grpDev = new GroupBox();
            grpFan = new GroupBox();
            fanBar = new FanSpeedBar();
            sensorBar = new FanSpeedBar();
            fanCtrl = new TrackBar();
            buttons = new List<Button>();
            lightingMode = new ComboBox();
            lightingSpeed = new ComboBox();
            txtDeviceName = new TextBox();
            fanUpdateTimer = new Timer();
            fanUpdateTimer.Interval = 50;
        }

        private EventHandler FanUpdateTimerTickHandler(int deviceIdx)
        {
            return (object sender, EventArgs e) =>
            {
                if (usbController == null || !usbController.IsConnected)
                {
                    return;
                }

                fanBar.Value = deviceControllerData.MeasuredFanRpm[deviceIdx];
                sensorBar.Value = GetSensorBarValue();

                if (deviceSettings.UseFanCurve)
                {
                    var calculatedFanSpeed = deviceControllerData.AutoFanSpeedValue[deviceIdx];
                    fanCtrl.Value = calculatedFanSpeed == 0xFF ? fanCtrl.Value : calculatedFanSpeed;
                }

            };
        }

        public void InitializeComponent(Form parent, SensorMonitorManager hm, int compNum)
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
            Label lblSensorUnitsTemplate = (Label)fanTemplate.Controls["lbl_SensorUnits"];

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

            Util.CopyObjectProperties<ComboBox>(lightingMode, cboModeTemplate, new string[] { "FlatStyle", "ForeColor", "BackColor", "Top", "Width", "Left", "Height", "MaxDropDownItems" });
            lightingMode.Parent = grpDev;
            lightingMode.Show();
            lightingMode.Items.AddRange(LightingModes.GetLightingModes());
            lightingMode.SelectedIndexChanged += LightingModeChangedEventHandler(compNum - 1);


            Util.CopyObjectProperties<ComboBox>(lightingSpeed, cboSpeedTemplate, new string[] { "FlatStyle", "ForeColor", "BackColor", "Top", "Width", "Left", "Height", "MaxDropDownItems" });
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
            Util.CopyObjectProperties<Label>(lblFanUnits, lblFanUnitsTemplate, new string[] { "ForeColor", "BackColor", "Font", "Left", "Text", "Width", "Top", "Height", "TextAlign" });
            lblFanUnits.Parent = grpFan;
            lblFanUnits.Tag = new ThemeTag() { small = true };
            lblFanUnits.Show();

            lblSensorUnits = new Label();
            Util.CopyObjectProperties<Label>(lblSensorUnits, lblSensorUnitsTemplate, new string[] { "ForeColor", "BackColor", "Font", "Left", "Text", "Width", "Top", "Height", "TextAlign" });
            lblSensorUnits.Parent = grpFan;
            lblSensorUnits.Tag = new ThemeTag() { small = true };
            lblSensorUnits.Show();

            btnFanConfig = new Button();
            Util.CopyObjectProperties<Button>(btnFanConfig, btnFanConfigTemplate, new string[] { "FlatStyle", "ForeColor", "Text", "BackColor", "Width", "Top", "Left", "Height" });
            btnFanConfig.Parent = grpFan;
            btnFanConfig.Click += OpenConfigFormEventHandler;
            btnFanConfig.Show();


            fanBar.Left = fanMarker.Left;
            fanBar.Top = fanMarker.Top;
            fanBar.Width = fanMarker.Width;
            fanBar.Height = fanMarker.Height;
            fanBar.BorderStyle = fanMarker.BorderStyle;
            fanBar.BarColor = AppTheme.PrimaryHighlight;
            fanBar.ForeColor = grpFan.ForeColor;
            fanBar.UseAverage = true;
            fanBar.Minimum = 0;
            fanBar.Maximum = 3000;
            fanBar.Value = 0;
            fanBar.Parent = grpFan;
            fanBar.Show();

            sensorBar.Left = fanTemperature.Left;
            sensorBar.Top = fanTemperature.Top;
            sensorBar.Width = fanTemperature.Width;
            sensorBar.Height = fanTemperature.Height;
            sensorBar.BorderStyle = fanTemperature.BorderStyle;
            sensorBar.BarColor = System.Drawing.Color.FromArgb(185, 49, 79);
            sensorBar.ForeColor = grpFan.ForeColor;
            sensorBar.UseAverage = true;
            sensorBar.Minimum = 0;
            sensorBar.Maximum = 120;
            sensorBar.Value = 0;
            sensorBar.Parent = grpFan;
            sensorBar.Show();

            Util.CopyObjectProperties<TrackBar>(fanCtrl, fanCtrlTemplate, 
                new string[] { "BackColor", "Width", "Top", "Left", "Height", "Minimum", "Maximum", "TickFrequency", "SmallChange", "LargeChange", "Orientation" });

            fanCtrl.Parent = grpFan;
            fanCtrl.Show();
            fanCtrl.Scroll += TrackScrollEvent(compNum - 1);
            fanUpdateTimer.Tick += FanUpdateTimerTickHandler(compNum - 1);

            ControlsEnabled(false);
        }

        public void Show()
        {
            fanBar.Show();
            sensorBar.Show();
        }

        public void Hide()
        {
            fanBar.Hide();
            sensorBar.Hide();
        }

        private void OpenConfigFormEventHandler(object sender, EventArgs e)
        {
            if (usbController == null || !usbController.IsConnected)
            {
                return;
            }

            ArchonLightingSystem.Forms.FanConfigurationForm form = new ArchonLightingSystem.Forms.FanConfigurationForm();
            form.InitializeForm(hardwareManager, deviceSettings);
            form.Location = parentForm.Location;
            DialogResult res = form.ShowDialog(parentForm);
            if(res == DialogResult.OK)
            {
                usbController.Settings.Save();
                LoadConfig();
            }
            else
            {
                usbController.Settings.RevertChanges();
                LoadConfig();
            }
        }

        public double SensorValue
        {
            get
            {
                return sensorBar.Value;
            }
            set
            {
                sensorBar.Value = value;
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
            if (usbController == null) return;
            deviceSettings = usbController.Settings.GetDeviceByIndex(devIdx);

            txtDeviceName.Text = deviceSettings.Name;
            fanCtrl.Enabled = !deviceSettings.UseFanCurve;
            fanCtrl.BackColor = fanCtrl.Enabled ? grpFan.BackColor : Color.FromArgb(unchecked((int)0xFF661111));
            btnFanConfig.Text = deviceSettings.SensorName.IsNotNullOrEmpty() ? deviceSettings.SensorName : "Configure...";
            sensor = hardwareManager.GetSensorByIdentifier(deviceSettings.Sensor);

            LoadSensorBarConfig();
        }

        private void LoadSensorBarConfig()
        {
            lblSensorUnits.Text = SensorUnits.GetLabel(sensor);
            sensorBar.Maximum = SensorUnits.GetMax(sensor);
        }

        private double GetSensorBarValue()
        {
            float? value = sensor?.Value;

            if(!value.HasValue) return 0;

            if (sensor.SensorType == SensorType.Throughput)
            {
                return (int)(value / 1048576); // MB/s
            }

            return (value.Value);
        }

        private void UpdateUI(int deviceIdx)
        {
            fanCtrl.Value = deviceControllerData.DeviceConfig.FanSpeed[deviceIdx];
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
            byte ledMode = deviceControllerData.DeviceConfig.LedMode[deviceIdx];
            byte ledSpeed = deviceControllerData.DeviceConfig.LedSpeed[deviceIdx];
            lightingMode.SelectedIndex = ledMode < LightingModes.GetLightingModes().Length ? ledMode : -1;
            lightingSpeed.SelectedIndex = ledSpeed > 0 && ledSpeed < LightingModes.GetLightingSpeeds().Length ? ledSpeed - 1 : -1;
        }

        private EventHandler TrackScrollEvent(int deviceIdx)
        {
            return (object sender, EventArgs e) =>
            {
                if (usbController == null || !usbController.IsConnected)
                {
                    return;
                }

                deviceControllerData.DeviceConfig.FanSpeed[deviceIdx] = (byte)((TrackBar)sender).Value;
                applicationData.UpdateConfigPending = true;
            };
        }

        private EventHandler TextNameChangedEventHandler(int deviceIdx)
        {
            return (object sender, EventArgs e) =>
            {
                if (deviceSettings == null)
                {
                    return;
                }

                try
                {
                    TextBox txt = (TextBox)sender;
                    deviceSettings.Name = txt.Text;
                    usbController.Settings.Save();

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
                if (usbController == null || !usbController.IsConnected)
                {
                    return;
                }

                ComboBox cbox = (ComboBox)sender;

                if (cbox.SelectedIndex == -1)
                {
                    return;
                }

                deviceControllerData.DeviceConfig.LedMode[deviceIdx] = (byte)cbox.SelectedIndex;
                applicationData.UpdateConfigPending = true;
            };
        }

        private EventHandler LightingSpeedChangedEventHandler(int deviceIdx)
        {
            return (object sender, EventArgs e) =>
            {
                if (usbController == null || !usbController.IsConnected)
                {
                    return;
                }

                ComboBox cbox = (ComboBox)sender;
                deviceControllerData.DeviceConfig.LedSpeed[deviceIdx] = (byte)(cbox.SelectedIndex + 1);
                applicationData.UpdateConfigPending = true;
            };
        }

        private EventHandler ColorUpdateClickEventHandler(int deviceIdx, int ledIdx)
        {
            return (object sender, EventArgs e) =>
            {
                if (usbController == null || !usbController.IsConnected)
                {
                    return;
                }

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
            deviceControllerData.DeviceConfig.Colors[deviceIdx, ledIdx++] = color.G;
            deviceControllerData.DeviceConfig.Colors[deviceIdx, ledIdx++] = color.R;
            deviceControllerData.DeviceConfig.Colors[deviceIdx, ledIdx++] = color.B;
        }

        private System.Drawing.Color GetLedColor(int deviceIdx, int ledIdx, bool lighterColorScale = false)
        {
            ledIdx *= 3;
            byte G = deviceControllerData.DeviceConfig.Colors[deviceIdx, ledIdx++];
            byte R = deviceControllerData.DeviceConfig.Colors[deviceIdx, ledIdx++];
            byte B = deviceControllerData.DeviceConfig.Colors[deviceIdx, ledIdx++];

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
