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
using ArchonLightingSystem.Common;
using ArchonLightingSystem.Interfaces;
using ArchonLightingSystem.UsbApplicationV2;
using ArchonLightingSystem.Components;
using System.Web.UI.Design;
using System.Threading;
using System.Windows.Forms.DataVisualization.Charting;
using ArchonLightingSDKCommon;

namespace ArchonLightingSystem
{
    public partial class EepromEditorForm : Form
    {
        private readonly string[] colNames = new string[16];
        private readonly DataSet eepromData;
        private int lastSelectedAddress = -1;
        
        private UsbControllerManager manager = null;
        private UsbControllerDevice usbControllerDevice;
        private readonly ApplicationData applicationData;

        private SemaphoreSlim formUpdateSemiphore = new SemaphoreSlim(1,1);

        public EepromEditorForm()
        {
            InitializeComponent();

            cbo_DeviceAddress.DisplayMember = "Text";
            cbo_DeviceAddress.ValueMember = "Value";

            eepromData = new DataSet();
            colNames = colNames.Select((item, index) => ((int)index).ToString("X")).ToArray();

            DataTable eeprom = eepromData.Tables.Add("Eeprom");

            foreach(var col in colNames)
            {
                eeprom.Columns.Add(col);
            }

            eepromGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            eepromGridView.AutoGenerateColumns = true;
            eepromGridView.DataSource = eepromData;
            eepromGridView.DataMember = "Eeprom";
            eepromGridView.RowHeadersWidth = 50;
            eepromGridView.CellValidating += DataGridViewHandlers.DataGridValidateNumericRangeHandler(new Tuple<int, int>(0, 255));

            AppTheme.ApplyThemeToForm(this);

            lbl_Disconnected.ForeColor = AppTheme.PrimaryLowLight;
        }

        public void InitializeForm(UsbControllerManager manager)
        {
            this.manager = manager;

            cbo_DeviceAddress.Items.Clear();
            cbo_DeviceAddress.Items.AddRange(
                manager
                .Controllers
                .Select(controller => 
                    new ComboBoxItem { Text = $"Address {controller.Address}", Value = controller.Address}
                ).ToArray());
        }

        public void UpdateFormData(UsbControllerDevice device)
        {
            formUpdateSemiphore.Wait();

            try
            {
                eepromData.Tables["Eeprom"].Rows.Clear();
                for (int rows = 0; rows < DeviceControllerDefinitions.EepromSize / 16; rows++)
                {
                    DataRow row = eepromData.Tables["Eeprom"].NewRow();
                    for (int i = 0; i < 16; i++)
                    {
                        row[colNames[i]] = device.ControllerData.EepromData[i + DeviceControllerDefinitions.EepromSize / 16 * rows];
                    }
                    eepromData.Tables["Eeprom"].Rows.Add(row);
                }

                eepromData.AcceptChanges();

                foreach (DataGridViewColumn col in eepromGridView.Columns)
                {
                    col.SortMode = DataGridViewColumnSortMode.NotSortable;
                }
            }
            finally 
            { 
                formUpdateSemiphore.Release(); 
            }
        }

        public void UpdateFormState(UsbControllerDevice device) 
        {
            formUpdateSemiphore.Wait();

            try
            {
                lbl_Disconnected.Visible = device.IsDisconnected;
                btn_ReadEeprom.Enabled = !device.IsDisconnected;
                btn_WriteEeprom.Enabled = !device.IsDisconnected;
                eepromGridView.Enabled = !device.IsDisconnected;
            }
            finally
            {
                formUpdateSemiphore.Release();
            }
        }

        private void SetFormBusy(bool busy)
        {
            foreach(Control con in this.Controls)
            {
                con.Enabled = !busy;
            }
        }

        private void WriteFormData()
        {
            eepromData.AcceptChanges();

            int dataIdx = 0;
            for (int rows = 0; rows < eepromData.Tables["Eeprom"].Rows.Count; rows++)
            {
                DataRow row = eepromData.Tables["Eeprom"].Rows[rows];
                for (int i = 0; i < DeviceControllerDefinitions.EepromSize / 16; i++)
                {
                    applicationData.DeviceControllerData.EepromData[dataIdx++] = (byte)int.Parse((string)row[colNames[i]]);
                }
            }
            applicationData.EepromAddress = 0;
            applicationData.EepromLength = 255;
            applicationData.EepromWritePending = true;
        }

        private void eepromGridView_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            DataGridView grid = (DataGridView)sender;
            for (int i = 0; i < grid.Rows.Count; i++)
            {
                grid.Rows[i].HeaderCell.Value = colNames[i];
            }
        }

        private void btn_ReadEeprom_Click(object sender, EventArgs e)
        {
            applicationData.EepromAddress = 0;
            applicationData.EepromLength = 255;
            applicationData.EepromReadDone = false;
            applicationData.EepromReadPending = true;
            StartFormUpdateEvent();
        }

        private void btn_WriteEeprom_Click(object sender, EventArgs e)
        {
            WriteFormData();
            
        }

        private void StartFormUpdateEvent()
        {
            SetFormBusy(true);
            formUpdateTimer.Enabled = true;
        }

        private void formUpdateTimer_Tick(object sender, EventArgs e)
        {
            if(applicationData.EepromReadDone)
            {
                applicationData.EepromReadDone = false;
                formUpdateTimer.Enabled = false;
                UpdateFormData(usbControllerDevice);
                SetFormBusy(false);
            }
        }

        private void cbo_DeviceAddress_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox combo = (ComboBox)sender;
            
            if (combo.SelectedIndex == lastSelectedAddress) return;

            if (eepromData.HasChanges())
            {
                if (DiscardChangesMessageBox() != DialogResult.Yes)
                {
                    combo.SelectedIndex = lastSelectedAddress;
                    return;
                }
            }

            lastSelectedAddress = combo.SelectedIndex;
            usbControllerDevice = manager.GetDevice(combo.SelectedIndex);
            UpdateFormState(usbControllerDevice);
            UpdateFormData(usbControllerDevice);
        }

        private DialogResult DiscardChangesMessageBox()
        {
            return MessageBox.Show(
                    "The form has uncommitted changes. Do you want to discard those changes?",
                    "Uncommitted Changes.",
                    MessageBoxButtons.YesNo);
        }

        private void EepromEditorForm_Load(object sender, EventArgs e)
        {
            cbo_DeviceAddress.SelectedIndex = 0;
            manager.UsbControllerEvent += HandleControllerEvent;
        }

        private void HandleControllerEvent(object sender, UsbControllerEventArgs e)
        {
            this.BeginInvoke(new Action<UsbControllerDevice>(UpdateFormState), new object[] { usbControllerDevice });
        }

        private void EepromEditorForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (eepromData.HasChanges())
            {
                if (DiscardChangesMessageBox() != DialogResult.Yes)
                {
                    e.Cancel = true;
                    return;
                }
            }

            manager.UsbControllerEvent -= HandleControllerEvent;
        }
    }
}
