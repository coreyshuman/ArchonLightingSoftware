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
using System.Diagnostics;

namespace ArchonLightingSystem
{
    public partial class EepromEditorForm : Form
    {
        private readonly string[] colNames = new string[16];
        private readonly DataSet eepromData;
        private int lastSelectedAddress = -1;
        private bool formBusy = false;
        
        private UsbControllerManager manager = null;
        private UsbControllerDevice usbControllerDevice;
        private ApplicationData applicationData;

        private SemaphoreSlim formUpdateSemaphore = new SemaphoreSlim(1,1);

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
            formUpdateSemaphore.Wait();

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
                formUpdateSemaphore.Release(); 
            }
        }

        public void UpdateFormState(UsbControllerDevice device, bool? busy = null) 
        {
            formUpdateSemaphore.Wait();

            if(busy.HasValue)
            {
                this.formBusy = busy.Value;
            }

            try
            {
                lbl_Disconnected.Visible = device.IsDisconnected;
                btn_ReadEeprom.Enabled = !device.IsDisconnected && !formBusy;
                btn_WriteEeprom.Enabled = !device.IsDisconnected && !formBusy;
                eepromGridView.Enabled = !device.IsDisconnected && !formBusy;
                cbo_DeviceAddress.Enabled = !formBusy;
            }
            finally
            {
                formUpdateSemaphore.Release();
            }
        }

        private void CommitLocalFormData()
        {
            eepromData.AcceptChanges();

            int dataIdx = 0;
            for (int rows = 0; rows < eepromData.Tables["Eeprom"].Rows.Count; rows++)
            {
                DataRow row = eepromData.Tables["Eeprom"].Rows[rows];
                for (int i = 0; i < DeviceControllerDefinitions.EepromSize / 16; i++)
                {
                    usbControllerDevice.ControllerData.EepromData[dataIdx++] = (byte)int.Parse((string)row[colNames[i]]);
                }
            }
        }

        private void eepromGridView_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            DataGridView grid = (DataGridView)sender;
            for (int i = 0; i < grid.Rows.Count; i++)
            {
                grid.Rows[i].HeaderCell.Value = colNames[i];
            }
        }

        private async void btn_ReadEeprom_Click(object sender, EventArgs e)
        {
            if (eepromData.HasChanges())
            {
                if (DiscardChangesMessageBox() != DialogResult.Yes)
                {
                    return;
                }
            }

            try
            {
                UpdateFormState(usbControllerDevice, true);
                if (false == await UsbApp.ReadAndUpdateEepromAsync(usbControllerDevice))
                {
                    throw new Exception("Transaction failed.");
                }

                UpdateFormData(usbControllerDevice);
            }
            catch(Exception ex)
            {
                Logger.Write(Level.Error, ex.Message);
                ErrorMessageBox(ex.Message);
            }
            finally
            {
                UpdateFormState(usbControllerDevice, false);
            }
        }

        private async void btn_WriteEeprom_Click(object sender, EventArgs e)
        {
            try
            {
                UpdateFormState(usbControllerDevice, true);
                CommitLocalFormData();

                if (false == await UsbApp.WriteAndUpdateEepromAsync(usbControllerDevice))
                {
                    throw new Exception("Transaction failed.");
                }

                UpdateFormData(usbControllerDevice);
            }
            catch (Exception ex)
            {
                Logger.Write(Level.Error, ex.Message);
                ErrorMessageBox(ex.Message);
            }
            finally
            {
                UpdateFormState(usbControllerDevice, false);
            }
        }

        private void formUpdateTimer_Tick(object sender, EventArgs e)
        {
            if(applicationData.EepromReadDone)
            {
                applicationData.EepromReadDone = false;
                //formUpdateTimer.Enabled = false;


                UpdateFormState(usbControllerDevice);
                UpdateFormData(usbControllerDevice);

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
            usbControllerDevice = manager.GetController(combo.SelectedIndex);
            applicationData = usbControllerDevice.AppData;
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

        private void ErrorMessageBox(string message)
        {
            MessageBox.Show(
                    $"The transaction failed: {message}",
                    "Error Occurred.",
                    MessageBoxButtons.OK);
        }

        private void EepromEditorForm_Load(object sender, EventArgs e)
        {
            cbo_DeviceAddress.SelectedIndex = 0;
            manager.UsbControllerEvent += HandleControllerEvent;
        }

        private void HandleControllerEvent(object sender, UsbControllerEventArgs e)
        {
            this.BeginInvoke(new Action<UsbControllerDevice, bool?>(UpdateFormState), new object[] { usbControllerDevice, null });
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
