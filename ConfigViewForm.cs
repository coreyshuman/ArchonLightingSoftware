using System;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;

namespace ArchonLightingSystem
{
    public partial class ConfigViewForm : Form
    {
        private DataGridView[] topGrids;
        private AppData appData = new AppData();
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

        #region Initializers
        public ConfigViewForm()
        {
            InitializeComponent();
            topGrids = new DataGridView[] { fanSpeedGridView, ledModeGridView, ledSpeedGridView };

            InitializeGrid();
            InitializeNavigator();
        }

        private void InitializeNavigator()
        {
            int i;
            var bindingSource = new BindingSource();
            ledColorNavigator.BindingSource = bindingSource;

            for (i = 0; i < DeviceController.DeviceCount; i++)
            {
                bindingSource.Add(i);
            }

            bindingSource.PositionChanged += BindingSource_PositionChanged;
        }

        private void InitializeGrid()
        {
            int i, gridNum;
            DataGridView gView;

            int[] cellRangeMax = new int[] { 100, 10, 10 };

            topGrids.Select(grid =>
            {
                grid.ColumnCount = (int)DeviceController.DeviceCount;
                grid.RowHeadersVisible = true;
                grid.AllowUserToAddRows = false;
                grid.AllowUserToDeleteRows = false;
                grid.CellValidating += DataGridValidateNumericRangeHandler(GetNumericValidationRangeForGrid(grid));
                grid.CellEndEdit += CellEndHandler(GetDataPropertyForGrid(grid));
                for (i = 1; i <= DeviceController.DeviceCount; i++)
                {
                    grid.Columns[i - 1].Name = $"D{i}";
                }
                return grid;
            }).ToArray();

            for (i = 1; i <= DeviceController.DeviceCount; i++)
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
                gView.ColumnCount = (int)(DeviceController.LedBytesPerDevice / numberOfGrids);
                gView.CellValidating += DataGridValidateNumericRangeHandler(GetNumericValidationRangeForGrid(gView));
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
        }
        #endregion

        #region Form Update
        public void UpdateFormData()
        {
            int i;

            topGrids.Select(grid =>
            {
                grid.Rows.Clear();
                grid.Rows.Add(((byte[])GetDataPropertyForGrid(grid).GetValue(appData.DeviceConfig)).ToStringArray());
                return grid;
            }).ToArray();

            int dataPerGrid = (int)(DeviceController.LedBytesPerDevice / numberOfGrids);
            for (i=1; i<=numberOfGrids; i++)
            {
                DataGridView gView = ((DataGridView)this.splitContainer1.Panel2.Controls["colorsGridView" + i]);
                gView.Rows.Clear();
                gView.Rows.Add(appData.DeviceConfig.Colors.SliceRow(currentDevice).Select(d => ((int)d).ToString()).Skip(dataPerGrid * (i - 1)).Take(dataPerGrid).ToArray());
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
                byte[] configArray = (byte[])property.GetValue(appData.DeviceConfig, null);
                configArray[e.ColumnIndex] = (byte)numVal;
            };
        }

        private void ColorGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            var gView = ((DataGridView)sender);
            var gridNumber = int.Parse(gView.Name.Substring(14));
            int dataPerGrid = (int)(DeviceController.LedBytesPerDevice / numberOfGrids);
            gView.Rows[e.RowIndex].ErrorText = String.Empty;
            var value = gView.Rows[e.RowIndex].Cells[e.ColumnIndex].FormattedValue.ToString();
            var numVal = int.Parse(value);
            appData.DeviceConfig.Colors[currentDevice, e.ColumnIndex + (gridNumber - 1) * dataPerGrid] = (byte)numVal;
        }

        private DataGridViewCellValidatingEventHandler DataGridValidateNumericRangeHandler(Tuple<int, int> minMax)
        {
            return (object sender, DataGridViewCellValidatingEventArgs e) =>
            {
                if (ValidateCellNumericRange(sender, e.FormattedValue, minMax.Item1, minMax.Item2))
                {
                    e.Cancel = true;
                }
            };
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

            return appData.DeviceConfig.GetType().GetProperty(propName);
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

            if (number < min || number > max)
            {
                gView.Rows[0].ErrorText = $"Value must be between {min} and {max}";
                return true;
            }

            return false;
        }
        #endregion
    }
}
