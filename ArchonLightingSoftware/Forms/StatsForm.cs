using ArchonLightingSystem.Components;
using ArchonLightingSystem.Models;
using ArchonLightingSystem.OpenHardware;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ArchonLightingSystem.Forms
{
    public partial class StatsForm : Form
    {
        private Panel container;
        private SensorMonitorManager hardwareManager = null;

        private int value = 0;
        private int offset = 1;
        private GaugeArc gauge1;
        private GaugeArc gauge2;
        private GaugeArc gauge3;
        private GaugeArc gauge4;

        public StatsForm()
        {
            InitializeComponent();

            container = panel_Container;

            gauge1 = new GaugeArc();
            gauge1.BackColor = Color.Black;
            gauge1.ForeColor = Color.White;
            gauge1.LineColor= Color.LightGreen;
            gauge1.OverLineColor = Color.Red;
            gauge1.Left = 0;
            gauge1.Width = 300;
            gauge1.OverValue = 80;
            gauge1.Label = "CPU";
            gauge1.UnitSymbol= "C";
            gauge1.Image = Icon.FromHandle(Properties.Resources.cpu.GetHicon());
            container.Controls.Add(gauge1);
            gauge1.Show();

            gauge2 = new GaugeArc();
            gauge2.BackColor = Color.Black;
            gauge2.ForeColor = Color.White;
            gauge2.LineColor = Color.Aqua;
            gauge2.OverLineColor = Color.Red;
            gauge2.Left = 400;
            gauge2.Width = 300;
            gauge2.Label = "LOAD";
            gauge2.UnitSymbol= "%";
            gauge2.Image = Icon.FromHandle(Properties.Resources.load.GetHicon());
            gauge2.Flipped= true;
            container.Controls.Add(gauge2);
            gauge2.Show();

            gauge3 = new GaugeArc();
            gauge3.BackColor = Color.Black;
            gauge3.ForeColor = Color.White;
            gauge3.LineColor = Color.LightGreen;
            gauge3.OverLineColor = Color.Red;
            gauge3.Top = 400;
            gauge3.Left = 0;
            gauge3.Width = 300;
            gauge3.MaximumValue = 6000;
            gauge3.OverValue = 4000;
            gauge3.Label = "CLOCK";
            gauge3.UnitSymbol= "MHz";
            gauge3.Image = Icon.FromHandle(Properties.Resources.clock.GetHicon());
            container.Controls.Add(gauge3);
            gauge3.Show();

            gauge4 = new GaugeArc();
            gauge4.BackColor = Color.Black;
            gauge4.ForeColor = Color.White;
            gauge4.LineColor = Color.Aqua;
            gauge4.OverLineColor = Color.Red;
            gauge4.Top = 400;
            gauge4.Left = 400;
            gauge4.Width = 300;
            gauge4.Label = "PUMP";
            gauge4.UnitSymbol = "RPM";
            gauge4.Image = Icon.FromHandle(Properties.Resources.fan.GetHicon());
            gauge4.Flipped = true;
            container.Controls.Add(gauge4);
            gauge4.Show();
            gauge4.MinimumValue = 800;
            gauge4.MaximumValue = 5000;

            

            AppTheme.ApplyThemeToForm(this);

            var gaugeTacho = new GaugeTacho();
            gaugeTacho.Top = 0;
            gaugeTacho.Left = 800;
            gaugeTacho.Width = 600;
            container.Controls.Add(gaugeTacho);
            gaugeTacho.Show();
        }

        public void InitializeForm(SensorMonitorManager sensorMonitor)
        {
            hardwareManager= sensorMonitor;
            Timer timer = new Timer();
            timer.Tick += Timer_Tick;
            timer.Interval = 33;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if(value == 0) offset = 1;
            if(value == 100) offset = -1;

            value += offset;
            gauge1.Value = (double)hardwareManager.GetSensorByIdentifier("/intelcpu/0/temperature/50")?.Value;
            gauge2.Value = (double)hardwareManager.GetSensorByIdentifier("/intelcpu/0/load/1")?.Value;
            gauge3.Value = (double)hardwareManager.GetSensorByIdentifier("/intelcpu/0/clock/1")?.Value;
            gauge4.Value = (double)hardwareManager.GetSensorByIdentifier("/lpc/nct6687d/fan/6")?.Value;
        }
    }
}
