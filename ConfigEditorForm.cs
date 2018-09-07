using System;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using ArchonLightingSystem.Common;
using ArchonLightingSystem.Models;
using ArchonLightingSystem.Interfaces;

namespace ArchonLightingSystem
{
    public partial class ConfigEditorForm : SubformBase
    {
        private DataGridView[] topGrids;
        private ApplicationData appData = new ApplicationData();
        private int numberOfGrids = 3;
        private int currentDevice = 0;

        #region Initializers
        public ConfigEditorForm()
        {
            InitializeComponent();
            topGrids = new DataGridView[] { fanSpeedGridView, ledModeGridView, ledSpeedGridView };
        }

        public void InitializeForm(ApplicationData applicationData)
        {
            appData = applicationData;
            InitializeGrid();
            InitializeNavigator();
            UpdateFormData();
        }

        private void InitializeNavigator()
        {
            int i;
            var bindingSource = new BindingSource();
            ledColorNavigator.BindingSource = bindingSource;

            for (i = 0; i < DeviceControllerDefinitions.DeviceCount; i++)
            {
                bindingSource.Add(i);
            }

            bindingSource.PositionChanged += BindingSource_PositionChanged;
        }

        private void InitializeGrid()
        {
            int i, gridNum;
            DataGridView gView;

            topGrids.Select(grid =>
            {
                grid.ColumnCount = (int)DeviceControllerDefinitions.DeviceCount;
                grid.RowHeadersVisible = true;
                grid.AllowUserToAddRows = false;
                grid.AllowUserToDeleteRows = false;
                grid.CellValidating += DataGridViewHandlers.DataGridValidateNumericRangeHandler(GetNumericValidationRangeForGrid(grid));
                grid.CellEndEdit += CellEndHandler(GetDataPropertyForGrid(grid));
                for (i = 1; i <= DeviceControllerDefinitions.DeviceCount; i++)
                {
                    grid.Columns[i - 1].Name = $"D{i}";
                }
                return grid;
            }).ToArray();

            for (i = 1; i <= DeviceControllerDefinitions.DeviceCount; i++)
            {
                string name = $"D{i}";
                fanSpeedGridView.Columns[i - 1].Name = name;
                ledModeGridView.Columns[i - 1].Name = name;
            }

            for (gridNum = 1; gridNum <= numberOfGrids; gridNum++)
            {

                if (gridNum == 1)
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
                gView.ColumnCount = (int)(DeviceControllerDefinitions.LedBytesPerDevice / numberOfGrids);
                gView.CellValidating += DataGridViewHandlers.DataGridValidateNumericRangeHandler(GetNumericValidationRangeForGrid(gView));
                gView.CellEndEdit += ColorGridView_CellEndEdit;

                int offsetStart = (int)(DeviceControllerDefinitions.LedBytesPerDevice / numberOfGrids * (gridNum - 1));
                for (i = 1 + offsetStart; i <= (int)(DeviceControllerDefinitions.LedBytesPerDevice / numberOfGrids * gridNum); i++)
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
        }
        #endregion

        #region Form Update
        public void UpdateFormData()
        {
            int i;

            topGrids.Select(grid =>
            {
                grid.Rows.Clear();
                grid.Rows.Add(((byte[])GetDataPropertyForGrid(grid).GetValue(appData.DeviceControllerData.DeviceConfig)).ToStringArray());
                return grid;
            }).ToArray();

            int dataPerGrid = (int)(DeviceControllerDefinitions.LedBytesPerDevice / numberOfGrids);
            for (i=1; i<=numberOfGrids; i++)
            {
                DataGridView gView = ((DataGridView)this.splitContainer1.Panel2.Controls["colorsGridView" + i]);
                gView.Rows.Clear();
                gView.Rows.Add(appData.DeviceControllerData.DeviceConfig.Colors.SliceRow(currentDevice).Select(d => ((int)d).ToString()).Skip(dataPerGrid * (i - 1)).Take(dataPerGrid).ToArray());
            }
            

        }
        #endregion

        #region EventHandlers
        private DataGridViewCellEventHandler CellEndHandler(System.Reflection.PropertyInfo property)
        {
            return (object sender, DataGridViewCellEventArgs e) =>
            {
                var gView = ((DataGridView)sender);
                gView.Rows[e.RowIndex].ErrorText = String.Empty;
                var value = gView.Rows[e.RowIndex].Cells[e.ColumnIndex].FormattedValue.ToString();
                var numVal = int.Parse(value);
                byte[] configArray = (byte[])property.GetValue(appData.DeviceControllerData.DeviceConfig, null);
                configArray[e.ColumnIndex] = (byte)numVal;
            };
        }

        private void ColorGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            var gView = ((DataGridView)sender);
            var gridNumber = int.Parse(gView.Name.Substring(14));
            int dataPerGrid = (int)(DeviceControllerDefinitions.LedBytesPerDevice / numberOfGrids);
            gView.Rows[e.RowIndex].ErrorText = String.Empty;
            var value = gView.Rows[e.RowIndex].Cells[e.ColumnIndex].FormattedValue.ToString();
            var numVal = int.Parse(value);
            appData.DeviceControllerData.DeviceConfig.Colors[currentDevice, e.ColumnIndex + (gridNumber - 1) * dataPerGrid] = (byte)numVal;
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

        private void btn_ResetConfig_Click(object sender, EventArgs e)
        {
            appData.DefaultConfigPending = true;
            updateTimer.Enabled = true;
        }
        #endregion

        #region Helpers
        private System.Reflection.PropertyInfo GetDataPropertyForGrid(DataGridView grid)
        {
            string propName = "";

            switch (grid.Name)
            {
                case "fanSpeedGridView": propName = "FanSpeed"; break;
                case "ledModeGridView": propName = "LedMode"; break;
                case "ledSpeedGridView": propName = "LedSpeed"; break;
                default: propName = "Colors"; break;
            }

            return appData.DeviceControllerData.DeviceConfig.GetType().GetProperty(propName);
        }

        private Tuple<int, int> GetNumericValidationRangeForGrid(DataGridView grid)
        {
            Tuple<int, int> minMax = null;

            switch (grid.Name)
            {
                case "fanSpeedGridView": minMax = new Tuple<int, int>(0, 100); break;
                case "ledModeGridView": minMax = new Tuple<int, int>(0, 16); break;
                case "ledSpeedGridView": minMax = new Tuple<int, int>(0, 10); break;
                default: minMax = new Tuple<int, int>(0, 255); break;
            }

            return minMax;
        }

        
        #endregion

        private void btn_UpdateConfig_Click(object sender, EventArgs e)
        {
            appData.UpdateConfigPending = true;
        }
    }
}
