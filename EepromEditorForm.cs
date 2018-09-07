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


namespace ArchonLightingSystem
{
    public partial class EepromEditorForm : SubformBase
    {
        private ApplicationData applicationData;
        private string[] colNames = new string[16];
        private DataSet eepromData;

        public EepromEditorForm()
        {
            InitializeComponent();
            eepromData = new DataSet();
            colNames = colNames.Select((item, index) => ((int)index).ToString("X")).ToArray();
        }

        public void InitializeForm(ApplicationData appData)
        {
            applicationData = appData;
            DataTable eeprom = eepromData.Tables.Add("Eeprom");

            colNames.Select(col => eeprom.Columns.Add(col)).ToArray();

            UpdateFormData();

            eepromGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            eepromGridView.AutoGenerateColumns = true;
            eepromGridView.DataSource = eepromData;
            eepromGridView.DataMember = "Eeprom";
            eepromGridView.RowHeadersWidth = 50;
            eepromGridView.CellValidating += DataGridViewHandlers.DataGridValidateNumericRangeHandler(new Tuple<int, int>(0, 255));
        }

        public void UpdateFormData()
        {
            eepromData.Tables["Eeprom"].Rows.Clear();
            for (int rows = 0; rows < DeviceControllerDefinitions.EepromSize / 16; rows++)
            {
                DataRow row = eepromData.Tables["Eeprom"].NewRow();
                for (int i = 0; i < 16; i++)
                {
                    row[colNames[i]] = applicationData.DeviceControllerData.EepromData[i + DeviceControllerDefinitions.EepromSize / 16 * rows];
                }
                eepromData.Tables["Eeprom"].Rows.Add(row);
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
                UpdateFormData();
                SetFormBusy(false);
            }
            
        }
    }
}
