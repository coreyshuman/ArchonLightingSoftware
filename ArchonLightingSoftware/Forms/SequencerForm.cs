using ArchonLightingSystem.Common;
using ArchonLightingSystem.Components;
using ArchonLightingSystem.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ArchonLightingSystem.UsbApplicationV2;

namespace ArchonLightingSystem.Forms
{
    public partial class SequencerForm : Form
    {
        private UsbControllerManager usbControllerManager;
        private UsbControllerDevice usbController;
        private static Color lastColor;
        private static List<Color> lastColorsAll = new List<Color>();
        private static ColorDialog colorDialog = new ColorDialog();

        private Color[,] colorStorage = new Color[16, 12];

        private int MaxStepCount = 16;
        private int CurrentStep = 1;
        private int SelectedDevice = 1;

        public SequencerForm()
        {
            InitializeComponent();

            cboController.DisplayMember = "Text";
            cboController.ValueMember = "Value";
            cboDevice.DisplayMember = "Text";
            cboDevice.ValueMember = "Value";

            AppTheme.ApplyThemeToForm(this);
        }

        public void InitializeForm(UsbControllerManager cm)
        {
            usbControllerManager = cm;

            cboController.Items.Clear();
            cboController.Items.AddRange(
                usbControllerManager
                .Controllers
                .Select(controller =>
                    new ComboBoxItem { Text = $"Address {controller.Address}", Value = controller.Address }
                ).ToArray());

            for (int i = 1; i <= DeviceControllerDefinitions.DevicePerController; i++)
            {
                cboDevice.Items.Add(new ComboBoxItem { Text = i.ToString(), Value = i });
            }

            cboController.SelectedIndex = 0;  
            cboDevice.SelectedIndex = 0;
        }

        private void SequencerForm_Load(object sender, EventArgs e)
        {
            light_1_1.Click += ColorUpdateClickEventHandler(0, 0);
            for (int j = 1; j <= 16; j++)
            {
                for (int i = (j == 1) ? 2 : 1; i <= DeviceControllerDefinitions.LedCountPerDevice; i++)
                {
                    Button btn = new Button();
                    Util.CopyObjectProperties<Button>(btn, light_1_1, new string[] { "FlatStyle", "ForeColor", "BackColor", "Width", "Left", "Height" });
                    btn.Top = light_1_1.Top + (light_1_1.Height + 4) * (i - 1);
                    btn.Left = light_1_1.Left + (light_1_1.Width + 4) * (j - 1);
                    //btn.Text = $"L{i + 1}";
                    btn.Parent = grpLights;
                    btn.Name = $"light_{j}_{i}";
                    btn.Show();
                    btn.Click += ColorUpdateClickEventHandler(j-1, i-1);
                    //buttons.Add(btn);
                }

                if (j > 1)
                {
                    Button btn = new Button();
                    Util.CopyObjectProperties<Button>(btn, tick_1, new string[] { "FlatStyle", "ForeColor", "BackColor", "Width", "Left", "Height" });
                    btn.Top = tick_1.Top;
                    btn.Left = (tick_1.Width + 4) * (j - 1);
                    //btn.Text = $"L{i + 1}";
                    btn.Parent = grpSeq;
                    btn.Name = $"tick_{j}";
                    btn.Show();
                }
            }

            for (int i = 2; i <= DeviceControllerDefinitions.LedCountPerDevice; i++)
            {
                Button btn = new Button();
                Util.CopyObjectProperties<Button>(btn, out_1, new string[] { "FlatStyle", "ForeColor", "BackColor", "Width", "Left", "Height" });
                btn.Top = out_1.Top + (light_1_1.Height + 4) * (i - 1);
                //btn.Text = $"L{i + 1}";
                btn.Parent = grpOut;
                btn.Name = $"out_{i}";
                btn.Show();
                //btn.Click += ColorUpdateClickEventHandler(compNum - 1, i);
                //buttons.Add(btn);
            }

            for (int j = 0; j < 16; j++)
            {
                for (int i = 0; i < DeviceControllerDefinitions.LedCountPerDevice; i++)
                {
                    UpdateLedColor(Color.Black, j, i);
                }
            }

            UpdateSequencerStep(CurrentStep);
            timer1.Interval = (1000 * 60) / 120;
            timer1.Enabled = true;
        }

        private void btnPlay_Click(object sender, EventArgs e)
        {
            timer1.Enabled = !timer1.Enabled;
            btnPlay.Text = (timer1.Enabled) ? "Pause" : "Play";
        }

        private void numBpm_ValueChanged(object sender, EventArgs e)
        {
            timer1.Interval = (int)Math.Round((1000 * 60) / numBpm.Value);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if(++CurrentStep > MaxStepCount)
            {
                CurrentStep = 1;
            }
            UpdateSequencerStep(CurrentStep);
            SendLedFrameToDevice(SelectedDevice, CurrentStep);
        }

        private void UpdateSequencerStep(int step)
        {
            for(int i = 1; i <= 16; i++)
            {
                var btn = grpSeq.Controls[$"tick_{i}"];
                btn.BackColor = (i == step) ? Color.Red : Color.Black;
            }

            for(int i = 1; i<= DeviceControllerDefinitions.LedCountPerDevice; i++)
            {
                grpOut.Controls[$"out_{i}"].BackColor = grpLights.Controls[$"light_{step}_{i}"].BackColor;
            }
        }

        private void SendLedFrameToDevice(int device, int step)
        {
            byte[,] ledFrame = new byte[DeviceControllerDefinitions.DevicePerController, DeviceControllerDefinitions.LedBytesPerDevice];

            for (int i = 1; i <= DeviceControllerDefinitions.LedCountPerDevice; i++)
            {
                ledFrame[device - 1, (i - 1) * 3 + 0] = colorStorage[step-1, i-1].G;
                ledFrame[device - 1, (i - 1) * 3 + 1] = colorStorage[step - 1, i - 1].R;
                ledFrame[device - 1, (i - 1) * 3 + 2] = colorStorage[step - 1, i - 1].B;
            }
            usbController.AppData.LedFrameData = ledFrame;
            usbController.AppData.WriteLedFrame = true;
        }

        private EventHandler ColorUpdateClickEventHandler(int stepIdx, int ledIdx)
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
                        UpdateLedColor(color, stepIdx, i);
                    }
                    //applicationData.UpdateConfigPending = true;
                }
                else if ((Control.ModifierKeys & (Keys.Shift | Keys.Alt)) == (Keys.Shift | Keys.Alt))
                {
                    if (lastColorsAll.Count == DeviceControllerDefinitions.LedCountPerDevice)
                    {
                        // update all leds for device copied from other device
                        for (int i = 0; i < DeviceControllerDefinitions.LedCountPerDevice; i++)
                        {
                            UpdateLedColor(lastColorsAll[i], stepIdx, i);
                        }
                        //applicationData.UpdateConfigPending = true;
                    }
                }
                else if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift)
                {
                    // copy color from button
                    lastColor = GetLedColor(stepIdx, ledIdx);
                    lastColorsAll.Clear();
                    for (int i = 0; i < DeviceControllerDefinitions.LedCountPerDevice; i++)
                    {
                        lastColorsAll.Add(GetLedColor(stepIdx, i));
                    }
                }
                else if ((Control.ModifierKeys & Keys.Control) == Keys.Control)
                {
                    // paste last color to button
                    UpdateLedColor(color, stepIdx, ledIdx);
                    //applicationData.UpdateConfigPending = true;
                }
                else
                {
                    color = GetColorFromDialog(GetLedColor(stepIdx, ledIdx));
                    UpdateLedColor(color, stepIdx, ledIdx);
                    //applicationData.UpdateConfigPending = true;
                }
            };
        }

        private void UpdateLedColor(Color color, int stepIdx, int ledIdx)
        {
            SetLedColor(color, stepIdx, ledIdx);
            color = GetLedColor(stepIdx, ledIdx, true);
            Button btn = (Button)grpLights.Controls[$"light_{stepIdx+1}_{ledIdx+1}"];
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
            if (currentColor.HasValue)
            {
                colorDialog.Color = currentColor.Value;
            }
            colorDialog.ShowDialog();
            lastColor = colorDialog.Color;
            return lastColor;
        }

        private void SetLedColor(System.Drawing.Color color, int stepIdx, int ledIdx)
        {
            colorStorage[stepIdx, ledIdx] = color;
        }


        private System.Drawing.Color GetLedColor(int stepIdx, int ledIdx, bool lighterColorScale = false)
        {
            byte G = colorStorage[stepIdx, ledIdx].G;
            byte R = colorStorage[stepIdx, ledIdx].R;
            byte B = colorStorage[stepIdx, ledIdx].B;

            if (lighterColorScale)
            {
                if (G > 0)
                {
                    G = (byte)(G / 1.5 + 85);
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

        private void CboController_SelectedIndexChanged(object sender, EventArgs e)
        {
            var item = ((ComboBoxItem)((ComboBox)sender).SelectedItem);
            usbController = usbControllerManager.GetControllerByAddress(item.Value);
            lblName.Text = usbController.Settings.Name;
        }

        private void CboDevice_SelectedIndexChanged(object sender, EventArgs e)
        {
            var item = ((ComboBoxItem)((ComboBox)sender).SelectedItem);
            SelectedDevice = item.Value;
        }

        private void NumSteps_ValueChanged(object sender, EventArgs e)
        {
            CurrentStep = 1;
            MaxStepCount = (int)(((NumericUpDown)sender).Value);
        }
    }
}
