
using ArchonLightingSystem.Models;
using ArchonLightingSystem.Properties;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ArchonLightingSystem.OpenHardware;
using System.Collections.ObjectModel;
using ArchonLightingSystem.Components;
using System.Windows.Forms.DataVisualization.Charting;
using LibreHardwareMonitor.Hardware;

namespace ArchonLightingSystem.Forms
{
    public partial class FanConfigurationForm : Form
    {
        private const string fanCurveCopyPasteKey = "fanCurve";
        private SensorMonitorManager hardwareManager;
        private DragWindowSupport dragSupport = new DragWindowSupport();
        private Node currentNode = null;
        private string identifier = "";
        private DataPoint selectedDataPoint = null;
        private DataPoint lastSelectedDataPoint = null;
        private DeviceSettings deviceSettings = null;

        public string Identifier
        {
            get
            {
                return identifier;
            }
        }

        #region initializers
        public FanConfigurationForm()
        {
            InitializeComponent();
            dragSupport.Initialize(this);
        }

        public void InitializeForm(SensorMonitorManager hm, DeviceSettings settings)
        {
            hardwareManager = hm;
            deviceSettings = settings;

            chk_UseFan.Checked = settings.UseFanCurve;
            chk_AlertFanStopped.Checked = settings.AlertOnFanStopped;
            identifier = settings.Sensor;
            if(identifier != "")
            {
                currentNode = hardwareManager.GetParentNodeByIdentifier(identifier);
            }

            num_hysteresisDecrease.Value = settings.DecreaseHysteresis;
            num_hysteresisIncrease.Value = settings.IncreaseHysteresis;
            num_stepDecrease.Value = settings.DecreaseStep;
            num_stepIncrease.Value = settings.IncreaseStep;

            InitializeGrid();
            InitializeFanCurve();

            AppTheme.ApplyThemeToForm(this);
        }

        void InitializeGrid()
        {
            ImageList imageList = new ImageList { ImageSize = new Size(16, 16) };
            ResourceManager resourceManager = new ResourceManager(typeof(Resources));

            ResourceSet resourceSet = resourceManager.GetResourceSet(CultureInfo.CurrentUICulture, true, true);
            foreach (DictionaryEntry entry in resourceSet)
            {
                string resourceKey = entry.Key.ToString();
                object resource = entry.Value;
                if(resource.GetType().BaseType == typeof(Image))
                    imageList.Images.Add(resourceKey, (Image)resource);
            }
            
            listView1.View = View.Details;
            listView1.FullRowSelect = true;
            listView1.SmallImageList = imageList;

            listView1.Columns.Add("Type", 150, HorizontalAlignment.Center);
            listView1.Columns.Add("Name", 240, HorizontalAlignment.Left);
            listView1.Columns.Add("Children", 120, HorizontalAlignment.Left);

            LoadGridForNode(currentNode);
        }

        void InitializeFanCurve()
        {
            var lineSeries = fanCurveChart.Series[0];
            var pointSeries = fanCurveChart.Series[1];
            var chart = fanCurveChart.ChartAreas[0];
            fanCurveChart.Titles.Add("Fan Curve");
            fanCurveChart.Titles[0].ForeColor = AppTheme.PrimaryText;
            lineSeries.Color = AppTheme.PrimaryHighlight;
            pointSeries.Color = AppTheme.SecondaryHighlight;
            chart.AxisX.LineColor = AppTheme.PrimaryText;
            chart.AxisX.TitleForeColor = AppTheme.PrimaryText;
            chart.AxisX.LabelStyle.ForeColor = AppTheme.PrimaryText;
            chart.AxisX.MajorGrid.LineColor = AppTheme.PrimaryText;
            chart.AxisX.MajorTickMark.LineColor = AppTheme.PrimaryText;
            chart.AxisY.LineColor = AppTheme.PrimaryText;
            chart.AxisY.TitleForeColor = AppTheme.PrimaryText;
            chart.AxisY.LabelStyle.ForeColor = AppTheme.PrimaryText;
            chart.AxisY.MajorGrid.LineColor = AppTheme.PrimaryText;
            chart.AxisY.MajorTickMark.LineColor = AppTheme.PrimaryText;

            var sensor = hardwareManager.GetSensorByIdentifier(Identifier);
            SetFanCurveRange(sensor);
            UpdateFanCurveChart();
        }

        void SetFanCurveRange(ISensor sensor)
        {
            string label = SensorUnits.GetLabel(sensor);
            double[] range = SensorUnits.GetRange(sensor);
            int pointCount = range.Length;
            double min = SensorUnits.GetMin(sensor);
            double interval = SensorUnits.GetInterval(sensor);

            var chart = fanCurveChart.ChartAreas[0];
            var lineSeries = fanCurveChart.Series[0];
            var linePoints = lineSeries.Points;
            var pointSeries = fanCurveChart.Series[1];
            var pointPoints = pointSeries.Points;

            linePoints.Clear();
            pointPoints.Clear();

            chart.AxisY.Title = "Fan Speed %";
            chart.AxisX.Title = label;

            chart.AxisY.Maximum = 100;
            chart.AxisY.Minimum = 0;
            chart.AxisY.Interval = 10;
            chart.AxisX.Maximum = range[range.Length - 1];
            chart.AxisX.Minimum = min;
            chart.AxisX.Interval = interval;

            for (int i = 0; i < range.Length; i++)
            {
                int defaultFanCurveVal = 30;

                if(i > pointCount * 0.75)
                {
                    defaultFanCurveVal = 100;
                }
                else if (i >= pointCount / 2)
                {
                    defaultFanCurveVal = (int)Math.Round(40 + 60 * (i - pointCount / 2) * (1 / (pointCount * 0.3)));
                }

                linePoints.AddXY(range[i], defaultFanCurveVal);
                pointPoints.AddXY(range[i], defaultFanCurveVal);
            }
        }
        #endregion

        void StoreChartPointLocation(ChartArea ca, Series s, DataPoint dp)
        {
            float mh = dp.MarkerSize / 2f;
            float px = (float)ca.AxisX.ValueToPixelPosition(dp.XValue);
            float py = (float)ca.AxisY.ValueToPixelPosition(dp.YValues[0]);
            dp.Tag = (new RectangleF(px - mh, py - mh, dp.MarkerSize, dp.MarkerSize));
        }

        void UpdateFanCurveChart()
        {
            var lineSeries = fanCurveChart.Series[0];
            var linePoints = lineSeries.Points;
            var pointSeries = fanCurveChart.Series[1];
            var pointPoints = pointSeries.Points;
            var chart = fanCurveChart.ChartAreas[0];

            for (int i = 0; i < pointPoints.Count; i++)
            {
                linePoints[i].YValues[0] = deviceSettings.FanCurveValues[i];
                pointPoints[i].YValues[0] = deviceSettings.FanCurveValues[i];
            }
            fanCurveChart.Invalidate();
        }

        private void LoadGridForNode(Node topNode = null)
        {
            listView1.Items.Clear();
            Collection<Node> nodes;

            if (topNode == null)
            {
                nodes = hardwareManager.Nodes;
                currentNode = hardwareManager.Root;
            }
            else
            {
                nodes = topNode.Nodes;
                currentNode = topNode;
            }

            foreach (Node node in nodes)
            {
                var item = new ListViewItem(node.Text, node.ImageKey);
                item.SubItems.Add(node.Text);

                var nodeType = node.GetType();
                string colName = "Children";
                string value = node.Nodes.Count.ToString();
                if (nodeType == typeof(SensorNode))
                {
                    colName = "Value";
                    var sensor = (SensorNode)node;
                    value = sensor.Value;
                    item.Text = ((TypeNode)node.Parent).Text;
                    item.ImageKey = ((TypeNode)node.Parent).ImageKey;
                    if(sensor.Sensor.Identifier.ToString() == identifier)
                    {
                        item.Selected = true;
                        lbl_Selected.Text = sensor.Text;
                    }
                }
                else if(nodeType == typeof(HardwareNode))
                {
                    item.Text = ((HardwareNode)node).Hardware.HardwareType.ToString();
                }

                listView1.Columns[2].Text = colName;
                item.SubItems.Add(value);

                item.Tag = node;
                listView1.Items.Add(item);
            }

            string path = string.Empty;
            var tempNode = currentNode;
            while(tempNode != null)
            {
                path = tempNode.Text + " > " + path;
                tempNode = tempNode.Parent;
            }

            lbl_Path.Text = path;
        }

        private void SelectSensor(SensorNode node)
        {
            lbl_Selected.Text = node.Text;
            identifier = node.Sensor.Identifier.ToString();
            var sensor = hardwareManager.GetSensorByIdentifier(Identifier);
            SetFanCurveRange(sensor);
            deviceSettings.Sensor = null;
            deviceSettings.SensorName = null;
            if (sensor != null)
            {
                deviceSettings.Sensor = Identifier;
                deviceSettings.SensorName = sensor.Name.Trim();
                deviceSettings.SensorType = sensor.SensorType;
            }
        }

        private void clearSelectedPoint()
        {
            if (lastSelectedDataPoint != null)
            {
                lastSelectedDataPoint.Color = AppTheme.SecondaryHighlight;
            }
            selectedDataPoint = null;
        }

        private void selectPoint(DataPoint dp)
        {
            dp.Color = AppTheme.PrimaryLowLight;
            selectedDataPoint = dp;
            lastSelectedDataPoint = dp;
        } 

        private void incrementPointValue(DataPoint dp, double value)
        {
            var pointSeries = fanCurveChart.Series[1];
            var pointPoints = pointSeries.Points;

            value += dp.YValues[0];
            if (value > 100) value = 100f;
            else if (value < 0) value = 0f;

            int pointIndex = pointPoints.IndexOf(dp);
            deviceSettings.FanCurveValues[pointIndex] = (int)value;
            UpdateFanCurveChart();
            fanCurveChart.Invalidate();
        }

        /// <summary>
        /// Select a point based on the positive or negative index offset from the current point.
        /// If the offset goes out of bounds, the first or last point in the series is selected depending on offset direction.
        /// </summary>
        /// <param name="currentPoint"></param>
        /// <param name="offsetIndex"></param>
        private void selectAdjacentPoint(DataPoint currentPoint, int offsetIndex)
        {
            var pointSeries = fanCurveChart.Series[1];
            var pointPoints = pointSeries.Points;
            int pointIndex = pointPoints.IndexOf(currentPoint);

            pointIndex += offsetIndex;
            if (pointIndex > pointPoints.Count - 1) pointIndex = pointPoints.Count - 1;
            else if (pointIndex < 0) pointIndex = 0;

            clearSelectedPoint();
            selectPoint(pointPoints[pointIndex]);
        }

        #region form_events
        private void listView1_KeyDown(object sender, KeyEventArgs e)
        {
            HandleKeyPress(sender, e);
        }

        private void FanConfigurationForm_KeyDown(object sender, KeyEventArgs e)
        {
            HandleKeyPress(sender, e);
        }

        private void FanCurveChart_KeyDown(object sender, KeyEventArgs e)
        {
            HandleKeyPress(sender, e);
        }

        private void HandleKeyPress(object sender, KeyEventArgs e)
        {
            var pointSeries = fanCurveChart.Series[1];
            var pointPoints = pointSeries.Points;
            var chart = fanCurveChart.ChartAreas[0];
            var keyCode = e.KeyCode;

            if (lastSelectedDataPoint != null)
            {
                if (keyCode == Keys.W)
                {
                    incrementPointValue(lastSelectedDataPoint, 10);
                }
                else if (keyCode == Keys.S)
                {
                    incrementPointValue(lastSelectedDataPoint, -10);
                }
                else if (keyCode == Keys.D)
                {
                    selectAdjacentPoint(lastSelectedDataPoint, 1);
                }
                else if (keyCode == Keys.A)
                {
                    selectAdjacentPoint(lastSelectedDataPoint, -1);
                }
            }
            e.SuppressKeyPress = true;
        }

        private void chk_AlertFanStopped_CheckedChanged(object sender, EventArgs e)
        {
            deviceSettings.AlertOnFanStopped = chk_AlertFanStopped.Checked;
        }

        private void useFanCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            deviceSettings.UseFanCurve = chk_UseFan.Checked;
        }

        private void listView1_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
        }

        private void fanCurveChart_MouseUp(object sender, MouseEventArgs e)
        {
            var pointSeries = fanCurveChart.Series[1];
            var pointPoints = pointSeries.Points;

            if (selectedDataPoint != null)
            {
                int pointIndex = pointPoints.IndexOf(selectedDataPoint);
                deviceSettings.FanCurveValues[pointIndex] = (int)selectedDataPoint.YValues[0];
                selectedDataPoint = null;
                UpdateFanCurveChart();
            }
        }

        private void fanCurveChart_MouseDown(object sender, MouseEventArgs e)
        {
            var pointSeries = fanCurveChart.Series[1];
            var pointPoints = pointSeries.Points;

            clearSelectedPoint();

            foreach (DataPoint dp in pointPoints)
            {
                if (((RectangleF)dp.Tag).Contains(e.Location))
                {
                    Cursor = Cursors.HSplit;
                    selectPoint(dp);
                    break;
                }
            }
        }
        private void fanCurveChart_MouseMove(object sender, MouseEventArgs e)
        {
            var pointSeries = fanCurveChart.Series[1];
            var pointPoints = pointSeries.Points;
            var chart = fanCurveChart.ChartAreas[0];

            if (e.Button.HasFlag(MouseButtons.Left) && selectedDataPoint != null)
            {
                float mh = selectedDataPoint.MarkerSize / 2f;
                //double vx = chart.AxisX.PixelPositionToValue(e.Location.X);
                double vy = selectedDataPoint.YValues[0];
                try
                {
                    vy = chart.AxisY.PixelPositionToValue(e.Location.Y);
                }
                finally
                {
                    if (vy < chart.AxisY.Minimum) vy = 0;
                    if (vy > chart.AxisY.Maximum) vy = 100;
                }

                selectedDataPoint.SetValueXY(selectedDataPoint.XValue, vy);
                StoreChartPointLocation(chart, pointSeries, selectedDataPoint);
                fanCurveChart.Invalidate();
            }
            else
            {
                Cursor = Cursors.Default;
                foreach (DataPoint dp in pointPoints)
                {
                    if (dp.Tag == null) continue;
                    if (((RectangleF)dp.Tag).Contains(e.Location))
                    {
                        Cursor = Cursors.Hand; break;
                    }
                }
            }
        }

        private void btnCopyFanCurve_Click(object sender, EventArgs e)
        {
            CopyPasteDictionary.Copy(fanCurveCopyPasteKey, deviceSettings.FanCurveValues);
        }

        private void btnPasteFanCurve_Click(object sender, EventArgs e)
        {
            deviceSettings.FanCurveValues = (List<int>)CopyPasteDictionary.Paste(fanCurveCopyPasteKey);
            UpdateFanCurveChart();
        }

        private void btn_ClearSelection_Click(object sender, EventArgs e)
        {
            identifier = string.Empty;
            lbl_Selected.Text = "...";
            SetFanCurveRange(null);
            LoadGridForNode(null);
            deviceSettings.Sensor = Identifier;
            deviceSettings.SensorName = string.Empty;
            deviceSettings.SensorType = SensorType.Temperature;
        }

        private void btn_Back_Click(object sender, EventArgs e)
        {
            if (currentNode?.Parent != null)
            {
                LoadGridForNode(currentNode.Parent);
            }
        }

        private void btn_Close_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btn_OK_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void listView1_Click(object sender, EventArgs e)
        {
            Node node = (Node)((ListView)sender).FocusedItem.Tag;

            if (node.GetType() == typeof(SensorNode))
            {
                SelectSensor((SensorNode)node);
            }
        }

        private void fanCurveChart_PrePaint(object sender, ChartPaintEventArgs e)
        {
            var pointSeries = fanCurveChart.Series[1];
            var pointPoints = pointSeries.Points;
            var chart = fanCurveChart.ChartAreas[0];

            foreach (DataPoint dp in pointPoints)
            {
                StoreChartPointLocation(chart, pointSeries, dp);
            }
        }

        private void listView1_ItemActivate(object sender, EventArgs e)
        {
            int selectedIndex = ((ListView)sender).SelectedIndices[0];
            Node node = (Node)listView1.Items[selectedIndex].Tag;
            if (node?.Nodes.Count > 0)
            {
                LoadGridForNode(node);
            }
            else if (node.GetType() == typeof(SensorNode))
            {
                SelectSensor((SensorNode)node);
            }
        }

        private void num_hysteresisIncrease_ValueChanged(object sender, EventArgs e)
        {
            deviceSettings.IncreaseHysteresis = (int)num_hysteresisIncrease.Value;
        }

        private void num_hysteresisDecrease_ValueChanged(object sender, EventArgs e)
        {
            deviceSettings.DecreaseHysteresis = (int)num_hysteresisDecrease.Value;
        }

        private void num_stepIncrease_ValueChanged(object sender, EventArgs e)
        {
            deviceSettings.IncreaseStep = (int)num_stepIncrease.Value;
        }

        private void num_stepDecrease_ValueChanged(object sender, EventArgs e)
        {
            deviceSettings.DecreaseStep = (int)num_stepDecrease.Value;
        }
        #endregion


    }
}
