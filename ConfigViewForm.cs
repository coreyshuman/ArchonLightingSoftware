using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Linq;

namespace ArchonLightingSystem
{
    public partial class ConfigViewForm : Form
    {
        private AppData appData;
        private int numberOfGrids = 3;
        private int currentDevice = 0;
        public AppData AppDataRef
        {
            get
            {
                return appData;
            }
            set
            {
                appData = value;
            }
        }

        public ConfigViewForm()
        {
            InitializeComponent();
            InitializeGrid();
            InitializeNavigator();
        }

        public void UpdateFormData()
        {
            int i;

            fanModeGridView.Rows.Clear();
            fanModeGridView.Rows.Add(appData.deviceConfig.FanSpeed.Select(d => ((int)d).ToString()).ToArray());
            ledModeGridView.Rows.Clear();
            ledModeGridView.Rows.Add(appData.deviceConfig.LedMode.Select(d => ((int)d).ToString()).ToArray());

            int dataPerGrid = (int)(DeviceController.LedBytesPerDevice / numberOfGrids);
            for (i=1; i<=numberOfGrids; i++)
            {
                DataGridView gView = ((DataGridView)this.splitContainer1.Panel2.Controls["colorsGridView" + i]);
                gView.Rows.Clear();
                gView.Rows.Add(appData.deviceConfig.Colors.SliceRow(currentDevice).Select(d => ((int)d).ToString()).Skip(dataPerGrid * (i - 1)).Take(dataPerGrid).ToArray());
            }
            

        }

        private void InitializeGrid()
        {
            int i, gridNum;

            fanModeGridView.ColumnCount = (int)DeviceController.DeviceCount;
            ledModeGridView.ColumnCount = (int)DeviceController.DeviceCount;
            fanModeGridView.RowHeadersVisible = true;
            ledModeGridView.RowHeadersVisible = true;
            fanModeGridView.AllowUserToAddRows = false;
            ledModeGridView.AllowUserToAddRows = false;
            fanModeGridView.AllowUserToDeleteRows = false;
            ledModeGridView.AllowUserToDeleteRows = false;
            fanModeGridView.CellValidating += FanModeGridView_CellValidating;
            fanModeGridView.CellEndEdit += FanModeGridView_CellEndEdit;
            ledModeGridView.CellValidating += LedModeGridView_CellValidating;
            ledModeGridView.CellEndEdit += LedModeGridView_CellEndEdit;
            for (i = 1; i <= DeviceController.DeviceCount; i++)
            {
                string name = $"D{i}";
                fanModeGridView.Columns[i - 1].Name = name;
                ledModeGridView.Columns[i - 1].Name = name;
            }
                       
            for(gridNum = 1; gridNum <= numberOfGrids; gridNum++)
            {
                DataGridView gView;
                if(gridNum == 1)
                {
                    gView = colorsGridView1;
                }
                else
                {
                    gView = new DataGridView();
                    gView.Parent = splitContainer1.Panel2;
                    gView.Name = "colorsGridView" + gridNum;
                    gView.Left = colorsGridView1.Left;
                    gView.Width = colorsGridView1.Width;
                    gView.Height = colorsGridView1.Height;
                    gView.Top = colorsGridView1.Top + (colorsGridView1.Height + 20) * (gridNum - 1);
                    gView.Show();
                }

                gView.RowHeadersVisible = true;
                gView.AllowUserToAddRows = false;
                gView.AllowUserToDeleteRows = false;
                gView.GridColor = Color.Black;
                gView.ColumnCount = (int)(DeviceController.LedBytesPerDevice / numberOfGrids);
                gView.CellValidating += ColorGridView_CellValidating;
                gView.CellEndEdit += ColorGridView_CellEndEdit;

                int offsetStart = (int)(DeviceController.LedBytesPerDevice / numberOfGrids * (gridNum - 1));
                for (i = 1 + offsetStart; i <= (int)(DeviceController.LedBytesPerDevice / numberOfGrids * gridNum); i++)
                {
                    string color = "";
                    string number = (((byte)(i - 1) / 3) + 1).ToString("D2");
                    switch (i % 3)
                    {
                        case 1: color = "G"; break;
                        case 2: color = "R"; break;
                        case 0: color = "B"; break;
                    }
                    string name = $"L{number} {color}";
                    gView.Columns[i - 1 - offsetStart].Name = name;
                }
            }


            string[] row0 = { "0", "128", "57" };

            //colorsGridView.Rows.Add(row0);

            //colorsGridView.Update();
        }

        private void ColorGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            var gView = ((DataGridView)sender);
            var gridNumber = int.Parse(gView.Name.Substring(14));
            int dataPerGrid = (int)(DeviceController.LedBytesPerDevice / numberOfGrids);
            gView.Rows[e.RowIndex].ErrorText = String.Empty;
            var value = gView.Rows[e.RowIndex].Cells[e.ColumnIndex].FormattedValue.ToString();
            var numVal = int.Parse(value);
            appData.deviceConfig.Colors[currentDevice, e.ColumnIndex + (gridNumber - 1) * dataPerGrid] = (byte)numVal;
        }

        private void ColorGridView_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (ValidateCellNumericRange(sender, e.FormattedValue, 0, 255))
            {
                e.Cancel = true;
            }
        }

        private void LedModeGridView_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (ValidateCellNumericRange(sender, e.FormattedValue, 0, 10))
            {
                e.Cancel = true;
            }
        }

        private void LedModeGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            var gView = ((DataGridView)sender);
            gView.Rows[e.RowIndex].ErrorText = String.Empty;
            var value = gView.Rows[e.RowIndex].Cells[e.ColumnIndex].FormattedValue.ToString();
            var numVal = int.Parse(value);
            appData.deviceConfig.LedMode[e.ColumnIndex] = (byte)numVal;
        }

        private void FanModeGridView_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (ValidateCellNumericRange(sender, e.FormattedValue, 0, 100))
            {
                e.Cancel = true;
            }
        }

        private void FanModeGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            var gView = ((DataGridView)sender);
            gView.Rows[e.RowIndex].ErrorText = String.Empty;
            var value = gView.Rows[e.RowIndex].Cells[e.ColumnIndex].FormattedValue.ToString();
            var numVal = int.Parse(value);
            appData.deviceConfig.FanSpeed[e.ColumnIndex] = (byte)numVal;
        }

        private bool ValidateCellNumericRange(object sender, object value, int min, int max)
        {
            var gView = ((DataGridView)sender);
            string valStr = value.ToString();
            int number = 0;
            // Confirm that the cell is not empty.
            if (string.IsNullOrEmpty(valStr))
            {
                gView.Rows[0].ErrorText = "Value can not be empty";
                return true;
            }

            if (!int.TryParse(valStr, out number))
            {
                gView.Rows[0].ErrorText = "Value must be a number";
                return true;
            }

            if(number < min || number > max)
            {
                gView.Rows[0].ErrorText = $"Value must be between {min} and {max}";
                return true;
            }

            return false;
        }

       

        private void InitializeNavigator()
        {
            int i;
            var bindingSource = new BindingSource();
            ledColorNavigator.BindingSource = bindingSource;

            for(i=0; i<DeviceController.DeviceCount; i++)
            {
                bindingSource.Add(i);
            }

            bindingSource.PositionChanged += BindingSource_PositionChanged;
        }

        private void BindingSource_PositionChanged(object sender, EventArgs e)
        {
            currentDevice = ((BindingSource)sender).Position;
            UpdateFormData();
        }

        private void btn_ReadConfig_Click(object sender, EventArgs e)
        {
            appData.ReadConfigPending = true;
            updateTimer.Enabled = true;
        }

        private void btn_WriteConfig_Click(object sender, EventArgs e)
        {
            appData.WriteConfigPending = true;
            updateTimer.Enabled = true;
        }

        private void updateTimer_Tick(object sender, EventArgs e)
        {
            updateTimer.Enabled = false;
            UpdateFormData();
        }
    }
}
