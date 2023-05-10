using ArchonLightingSystem.Components;
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
        private int value = 0;
        private int offset = 1;
        private GaugeArc gauge1;
        private GaugeArc gauge2;


        public StatsForm()
        {
            InitializeComponent();

            container = panel_Container;

            gauge1 = new GaugeArc();
            gauge1.BackColor = Color.Black;
            gauge1.ForeColor = Color.White;
            gauge1.LineColor= Color.LightGreen;
            gauge1.OverLineColor = Color.Red;
            gauge1.OverValue = 80;
            gauge1.Label = "CPU";
            container.Controls.Add(gauge1);
            gauge1.Show();

            gauge2 = new GaugeArc();
            gauge2.BackColor = Color.Black;
            gauge2.ForeColor = Color.White;
            gauge2.LineColor = Color.Aqua;
            gauge2.OverLineColor = Color.Red;
            gauge2.Left = 300;
            gauge2.Width = 300;
            gauge2.Label = "GRAPHICS";
            gauge2.Flipped= true;
            container.Controls.Add(gauge2);
            gauge2.Show();
            gauge2.MinimumValue = 20;
            gauge2.MaximumValue = 140;
            gauge2.OverValue = 60;
            gauge2.Value = 90;

            Timer timer = new Timer();
            timer.Tick += Timer_Tick;
            timer.Interval= 70;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if(value == 0) offset = 1;
            if(value == 100) offset = -1;

            value += offset;
            gauge1.Value = value;
            gauge2.Value = value;
        }
    }
}
