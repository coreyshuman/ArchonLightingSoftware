using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ArchonLightingSystem.Models;
using ArchonLightingSystem.Interfaces;
using ArchonLightingSystem.Components;
using ArchonLightingSystem.UsbApplicationV2;
using ArchonLightingSystem.ArchonLightingSDKIntegration;

namespace ArchonLightingSystem
{
    public partial class DebugForm : Form
    {
        private UsbControllerManager usbControllerManager;
        private DragWindowSupport dragSupport = new DragWindowSupport();
        private int selectedAddress = -1;
        public DebugForm()
        {
            InitializeComponent();
            cbo_DeviceAddress.DisplayMember = "Text";
            cbo_DeviceAddress.ValueMember = "Value";
            dragSupport.Initialize(this);
        }

        public void InitializeForm(UsbControllerManager cm)
        {
            usbControllerManager = cm;
            cbo_DeviceAddress.Items.Clear();
            cbo_DeviceAddress.Items.AddRange(
                usbControllerManager
                .ActiveControllers
                .Select(controller =>
                    new ComboBoxItem { Text = $"Address {controller.Address}", Value = controller.Address }
                ).ToArray());

            AppTheme.ApplyThemeToForm(this);
        }

        public void UpdateFormData()
        {

        }

        private void btn_ReadDebug_Click(object sender, EventArgs e)
        {
            if(selectedAddress >= 0)
            {
                usbControllerManager.GetControllerByAddress(selectedAddress).AppData.ReadDebugPending = true;
                updateFormTimer.Enabled = true;
            }
        }

        private void updateFormTimer_Tick(object sender, EventArgs e)
        {
            txt_Debug.Text += usbControllerManager.GetControllerByAddress(selectedAddress).AppData.Debug;
            updateFormTimer.Enabled = false;
        }

        private void btn_ClearScreen_Click(object sender, EventArgs e)
        {
            txt_Debug.Text = "";
        }

        private void cbo_DeviceAddress_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedAddress = ((ComboBoxItem)((ComboBox)sender).SelectedItem).Value;
        }

        private void btn_Close_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var mappedFileManager = MappedFileManager.Instance;
            mappedFileManager.ReadFile();
        }
    }
}
