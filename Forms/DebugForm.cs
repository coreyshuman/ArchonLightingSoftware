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
using ArchonLightingSystem.UsbApplication;

namespace ArchonLightingSystem
{
    public partial class DebugForm : Form
    {
        private UsbDeviceManager usbDeviceManager;
        private DragWindowSupport dragSupport = new DragWindowSupport();
        private int selectedAddressIdx = -1;
        public DebugForm()
        {
            InitializeComponent();
            cbo_DeviceAddress.DisplayMember = "Text";
            cbo_DeviceAddress.ValueMember = "Value";
            dragSupport.Initialize(this);
        }

        public void InitializeForm(UsbDeviceManager usbApplication)
        {
            usbDeviceManager = usbApplication;
            var activeDevices = usbDeviceManager.UsbDevices
                .Where(dev => dev.UsbDevice.IsAttached && dev.AppIsInitialized && dev.AppData.DeviceControllerData?.IsInitialized == true)
                .Select((dev) => new ComboBoxItem { Text = dev.AppData.DeviceControllerData.DeviceAddress.ToString(), Value = usbDeviceManager.UsbDevices.IndexOf(dev) })
                .ToList();
            activeDevices.ForEach(dev =>
            {
                cbo_DeviceAddress.Items.Add(dev);
            });
            if (cbo_DeviceAddress.Items.Count > 0)
            {
                cbo_DeviceAddress.SelectedIndex = 0;
            }
        }

        public void UpdateFormData()
        {

        }

        private void btn_ReadDebug_Click(object sender, EventArgs e)
        {
            if(selectedAddressIdx >= 0)
            {
                usbDeviceManager.GetDevice(selectedAddressIdx).AppData.ReadDebugPending = true;
                updateFormTimer.Enabled = true;
            }
        }

        private void updateFormTimer_Tick(object sender, EventArgs e)
        {
            txt_Debug.Text += usbDeviceManager.GetDevice(selectedAddressIdx).AppData.Debug;
            updateFormTimer.Enabled = false;
        }

        private void btn_ClearScreen_Click(object sender, EventArgs e)
        {
            txt_Debug.Text = "";
        }

        private void cbo_DeviceAddress_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedAddressIdx = ((ComboBoxItem)((ComboBox)sender).SelectedItem).Value;
        }

        private void btn_Close_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
