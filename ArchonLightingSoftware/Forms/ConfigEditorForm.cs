﻿using System;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using ArchonLightingSystem.Common;
using ArchonLightingSystem.Models;
using ArchonLightingSystem.Interfaces;
using ArchonLightingSystem.UsbApplicationV2;
using System.Threading;
using System.Data;
using System.Collections.Generic;
using System.Windows.Forms.DataVisualization.Charting;
using ArchonLightingSystem.Components;

namespace ArchonLightingSystem
{
    public partial class ConfigEditorForm : Form
    {
        private DataGridView[] topGrids;
        private DataSet dataSet;
        private ApplicationData appData = new ApplicationData();
        DeviceControllerConfig dummyConfig = new DeviceControllerConfig();
        private int numberOfGrids = 3;
        private int currentDevice = 0;
        private bool formBusy = false;

        private int lastSelectedAddress = -1;

        private UsbControllerManager manager = null;
        private UsbControllerDevice usbControllerDevice;
        private readonly ApplicationData applicationData;

        private SemaphoreSlim formUpdateSemaphore = new SemaphoreSlim(1, 1);

        #region Initializers
        public ConfigEditorForm()
        {
            InitializeComponent();
            topGrids = new DataGridView[] { fanSpeedGridView, ledModeGridView, ledSpeedGridView };
            dataSet = new DataSet();
        }

        public void InitializeForm(UsbControllerManager manager)
        {
            this.manager = manager;

            cbo_DeviceAddress.DisplayMember = "Text";
            cbo_DeviceAddress.ValueMember = "Value";

            cbo_DeviceAddress.Items.Clear();
            cbo_DeviceAddress.Items.AddRange(
                manager
                .Controllers
                .Select(controller =>
                    new ComboBoxItem { Text = $"Address {controller.Address}", Value = controller.Address }
                ).ToArray());

            InitializeGrid();
            InitializeNavigator();

            AppTheme.ApplyThemeToForm(this);
            lbl_FanSpeed.Font = AppTheme.ComponentFontLarge;
            lbl_LedColors.Font = AppTheme.ComponentFontLarge;
            lbl_LedMode.Font = AppTheme.ComponentFontLarge;
            lbl_LedSpeed.Font = AppTheme.ComponentFontLarge;
            lbl_CRC.Font = AppTheme.ComponentFont;
            lbl_Disconnected.ForeColor = AppTheme.PrimaryLowLight;
        }

        private void InitializeNavigator()
        {
            int i;
            var bindingSource = new BindingSource();
            ledColorNavigator.BindingSource = bindingSource;

            for (i = 0; i < DeviceControllerDefinitions.DevicePerController; i++)
            {
                bindingSource.Add(i);
            }

            bindingSource.PositionChanged += BindingSource_PositionChanged;
        }

        private void InitializeGrid()
        {
            int i, j, gridNum, tableNum;
            DataGridView gView;

            tableNum = 0;
            foreach(var grid in topGrids)
            {
                DataTable table = dataSet.Tables.Add($"table{tableNum}");

                for (i = 1; i <= DeviceControllerDefinitions.DevicePerController; i++)
                {
                    table.Columns.Add($"D{i}");
                }

                grid.DataSource = dataSet;
                grid.DataMember = $"table{tableNum}";
                grid.RowHeadersVisible = true;
                grid.AllowUserToAddRows = false;
                grid.AllowUserToDeleteRows = false;
                grid.CellValidating += DataGridViewHandlers.DataGridValidateNumericRangeHandler(GetNumericValidationRangeForGrid(grid));
                grid.CellEndEdit += CellEndHandler(GetDataPropertyForGrid(grid));
  
                tableNum ++;
            }

            for (gridNum = 1; gridNum <= numberOfGrids; gridNum++)
            {
                int offsetStart = (int)(DeviceControllerDefinitions.LedBytesPerDevice / numberOfGrids * (gridNum - 1));

                // generate table entry and row for each device 
                // 3 tables per device * 5 devices
                for(j = 0; j < DeviceControllerDefinitions.DevicePerController; j++)
                {
                    DataTable table = dataSet.Tables.Add($"table{tableNum}_device{j}");
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
                        table.Columns.Add(name);
                    }
                }

                if (gridNum == 1)
                {
                    gView = colorsGridView1;
                }
                else
                {
                    gView = new DataGridView();
                    gView.Name = "colorsGridView" + gridNum;
                    gView.Left = colorsGridView1.Left;
                    gView.Width = colorsGridView1.Width;
                    gView.Height = colorsGridView1.Height;
                    gView.Top = lbl_LedColors.Top + lbl_LedColors.Height + (colorsGridView1.Height + lbl_LedColors.Height) * (gridNum - 1);
                    gView.Show();
                }

                gView.Parent = splitContainer1.Panel2;
                gView.RowHeadersVisible = true;
                gView.AllowUserToAddRows = false;
                gView.AllowUserToDeleteRows = false;
                gView.GridColor = Color.Black;
                gView.DataSource = dataSet;
                gView.AutoGenerateColumns = true;
                gView.DataMember = $"table{tableNum}_device{currentDevice}";
                gView.CellValidating += DataGridViewHandlers.DataGridValidateNumericRangeHandler(GetNumericValidationRangeForGrid(gView));
                gView.CellEndEdit += ColorGridView_CellEndEdit;

                tableNum++;
            }
        }
        #endregion

        #region Form Update
        public void UpdateFormState(UsbControllerDevice device, bool? busy = null)
        {
            formUpdateSemaphore.Wait();

            if (busy.HasValue)
            {
                this.formBusy = busy.Value;
            }

            try
            {
                lbl_Disconnected.Visible = device.IsDisconnected;
                btn_ReadConfig.Enabled = !device.IsDisconnected && !formBusy;
                btn_ResetConfig.Enabled = !device.IsDisconnected && !formBusy;
                btn_WriteConfig.Enabled = !device.IsDisconnected && !formBusy;
                btn_CommitConfig.Enabled = !device.IsDisconnected && !formBusy;
                ledColorNavigator.Enabled = !device.IsDisconnected && !formBusy;

                foreach (var con in splitContainer1.Panel1.Controls)
                {
                    if(con is DataGridView grid)
                    {
                        grid.Enabled = !device.IsDisconnected && !formBusy;
                    }
                }

                foreach (var con in splitContainer1.Panel2.Controls)
                {
                    if (con is DataGridView grid)
                    {
                        grid.Enabled = !device.IsDisconnected && !formBusy;
                    }
                }

                cbo_DeviceAddress.Enabled = !formBusy;
            }
            finally
            {
                formUpdateSemaphore.Release();
            }
        }

        public void UpdateFormData(UsbControllerDevice device)
        {
            int i, j;

            formUpdateSemaphore.Wait();

            try
            {
                var deviceConfig = device.ControllerData.DeviceConfig;
                lbl_CRC.Text = "--";

                foreach (var grid in topGrids)
                {
                    var table = dataSet.Tables[grid.DataMember];
                    table.Rows.Clear();
                    table.Rows.Add(((byte[])GetDataPropertyForGrid(grid).GetValue(deviceConfig)).ToStringArray());
                }

                int dataPerGrid = (int)(DeviceControllerDefinitions.LedBytesPerDevice / numberOfGrids);
                for (i = 1; i <= numberOfGrids; i++)
                {
                    DataGridView gView = ((DataGridView)this.splitContainer1.Panel2.Controls["colorsGridView" + i]);
                    for (j = 0; j < DeviceControllerDefinitions.DevicePerController; j++)
                    {
                        
                        var table = dataSet.Tables[GetGridDataMemberForDeviceIndex(gView, j)];
                        table.Rows.Clear();
                        table.Rows.Add(deviceConfig.Colors.SliceRow(j).Select(d => ((int)d).ToString()).Skip(dataPerGrid * (i - 1)).Take(dataPerGrid).ToArray());
                    }
                }

                lbl_CRC.Text = $"0x{deviceConfig.Crc:X4}";

                dataSet.AcceptChanges();
            }
            finally
            { 
                formUpdateSemaphore.Release(); 
            }
        }

        private void CommitLocalFormData()
        {
            int i, j, cellIdx;

            dataSet.AcceptChanges();

            var deviceConfig = usbControllerDevice.ControllerData.DeviceConfig;

            foreach (var grid in topGrids)
            {
                var table = dataSet.Tables[grid.DataMember];

                var propertyData = (byte[])GetDataPropertyForGrid(grid).GetValue(deviceConfig);

                for(i = 0; i < DeviceControllerDefinitions.DevicePerController; i++)
                {
                    propertyData[i] = (byte)int.Parse((string)table.Rows[0][i]);
                }
            }

            int dataPerGrid = (int)(DeviceControllerDefinitions.LedBytesPerDevice / numberOfGrids);
            for (i = 1; i <= numberOfGrids; i++)
            {
                DataGridView gView = ((DataGridView)this.splitContainer1.Panel2.Controls["colorsGridView" + i]);
                for (j = 0; j < DeviceControllerDefinitions.DevicePerController; j++)
                {
                    var table = dataSet.Tables[GetGridDataMemberForDeviceIndex(gView, j)];
                    for (cellIdx = 0; cellIdx < dataPerGrid; cellIdx++)
                    {
                        deviceConfig.Colors[j, cellIdx + (i - 1) * dataPerGrid] = (byte)int.Parse((string)table.Rows[0][cellIdx]);
                    }
                    

                    deviceConfig.Colors.SliceRow(currentDevice).Select(d => ((int)d).ToString()).Skip(dataPerGrid * (i - 1)).Take(dataPerGrid).ToArray();
                }
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
            };
        }

        private void ColorGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            var gView = ((DataGridView)sender);
            var gridNumber = int.Parse(gView.Name.Substring(14));
            int dataPerGrid = (int)(DeviceControllerDefinitions.LedBytesPerDevice / numberOfGrids);
            gView.Rows[e.RowIndex].ErrorText = String.Empty;
        }

        private void BindingSource_PositionChanged(object sender, EventArgs e)
        {
            currentDevice = ((BindingSource)sender).Position;

            for (int i = 1; i <= numberOfGrids; i++)
            {
                DataGridView gView = ((DataGridView)this.splitContainer1.Panel2.Controls["colorsGridView" + i]);
                // update dataMember name (last character is `currentDevice`)
                gView.DataMember = GetGridDataMemberForDeviceIndex(gView, currentDevice);
            }
        }

        private async void btn_ReadConfig_Click(object sender, EventArgs e)
        {
            if (dataSet.HasChanges())
            {
                if (DiscardChangesMessageBox() != DialogResult.Yes)
                {
                    return;
                }
            }

            try
            {
                UpdateFormState(usbControllerDevice, true);
                if (false == await UsbApp.ReadAndUpdateConfigAsync(usbControllerDevice))
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

        private async void btn_WriteConfig_Click(object sender, EventArgs e)
        {
            try
            {
                UpdateFormState(usbControllerDevice, true);
                CommitLocalFormData();
                if (false == await UsbApp.WriteAndUpdateConfigAsync(usbControllerDevice))
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

        private async void btn_CommitConfig_Click(object sender, EventArgs e)
        {
            if (dataSet.HasChanges())
            {
                if (DiscardChangesMessageBox() != DialogResult.Yes)
                {
                    return;
                }
            }

            try
            {
                UpdateFormState(usbControllerDevice, true);
                if (false == await UsbApp.CommitConfigToEepromAsync(usbControllerDevice))
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

        private async void btn_ResetConfig_Click(object sender, EventArgs e)
        {
            if (dataSet.HasChanges())
            {
                if (DiscardChangesMessageBox() != DialogResult.Yes)
                {
                    return;
                }
            }

            if(DialogResult.Yes != MessageBox.Show(
                    $"Are you sure you want to reset all settings to their default value?",
                    "Reset Config To Default",
                    MessageBoxButtons.YesNo)
                )
            {
                return;
            }

            try
            {
                UpdateFormState(usbControllerDevice, true);
                if (false == await UsbApp.ResetConfigToDefaultAsync(usbControllerDevice))
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

        private void ConfigEditorForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (dataSet.HasChanges())
            {
                if (DiscardChangesMessageBox() != DialogResult.Yes)
                {
                    e.Cancel = true;
                    return;
                }
            }

            manager.UsbControllerEvent -= HandleControllerEvent;
        }

        private void ConfigEditorForm_Load(object sender, EventArgs e)
        {
            cbo_DeviceAddress.SelectedIndex = 0;
            manager.UsbControllerEvent += HandleControllerEvent;
        }

        private void HandleControllerEvent(object sender, UsbControllerEventArgs e)
        {
            this.BeginInvoke(new Action<UsbControllerDevice, bool?>(UpdateFormState), new object[] { usbControllerDevice, null });
        }
        #endregion

        #region Helpers
        private System.Reflection.PropertyInfo GetDataPropertyForGrid(DataGridView grid)
        {
            string propName;
            switch (grid.Name)
            {
                case "fanSpeedGridView": propName = "FanSpeed"; break;
                case "ledModeGridView": propName = "LedMode"; break;
                case "ledSpeedGridView": propName = "LedSpeed"; break;
                default: propName = "Colors"; break;
            }

            return dummyConfig.GetType().GetProperty(propName);
        }

        private Tuple<int, int> GetNumericValidationRangeForGrid(DataGridView grid)
        {
            Tuple<int, int> minMax;
            switch (grid.Name)
            {
                case "fanSpeedGridView": minMax = new Tuple<int, int>(0, 100); break;
                case "ledModeGridView": minMax = new Tuple<int, int>(0, 16); break;
                case "ledSpeedGridView": minMax = new Tuple<int, int>(0, 10); break;
                default: minMax = new Tuple<int, int>(0, 255); break;
            }

            return minMax;
        }

        private string GetGridDataMemberForDeviceIndex(DataGridView grid, int deviceIndex)
        {
            return grid.DataMember.Substring(0, grid.DataMember.Length - 1) + deviceIndex.ToString();
        }

        private DialogResult DiscardChangesMessageBox()
        {
            return MessageBox.Show(
                    "The form has uncommitted changes. Do you want to discard those changes?",
                    "Uncommitted Changes.",
                    MessageBoxButtons.YesNo);
        }
        #endregion



        private void cbo_DeviceAddress_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox combo = (ComboBox)sender;

            if (combo.SelectedIndex == lastSelectedAddress) return;

            if (dataSet.HasChanges())
            {
                if (DiscardChangesMessageBox() != DialogResult.Yes)
                {
                    combo.SelectedIndex = lastSelectedAddress;
                    return;
                }
            }

            ledColorNavigator.BindingSource.Position = 0;

            lastSelectedAddress = combo.SelectedIndex;
            usbControllerDevice = manager.GetController(combo.SelectedIndex);
            UpdateFormState(usbControllerDevice);
            UpdateFormData(usbControllerDevice);
        }

        #region Forms
        private void ErrorMessageBox(string message)
        {
            MessageBox.Show(
                    $"The transaction failed: {message}",
                    "Error Occurred.",
                    MessageBoxButtons.OK);
        }
        #endregion


    }
}
