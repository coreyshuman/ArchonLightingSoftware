using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ArchonLightingSystem.Components;
using ArchonLightingSystem.Models;
using Microsoft.Win32;

namespace ArchonLightingSystem.Forms
{
    public partial class StatsConfigForm : Form
    {
        private Form statsForm = null;

        public StatsConfigForm()
        {
            InitializeComponent();

            cbo_Screens.DisplayMember = "DeviceName";
            cbo_Screens.ValueMember = "DeviceName";

            UpdateScreenList();

            SystemEvents.DisplaySettingsChanged += SystemEvents_DisplaySettingsChanged;

            AppTheme.ApplyThemeToForm(this);
            lbl_GaugeList.Font = AppTheme.ComponentFontLarge;
        }

        ~StatsConfigForm()
        {
            SystemEvents.DisplaySettingsChanged -= SystemEvents_DisplaySettingsChanged;
        }

        private void SystemEvents_DisplaySettingsChanged(object sender, EventArgs e)
        {
            UpdateScreenList();
        }

        public void InitializeForm(Form form)
        {
            this.statsForm = form;
        }

        private void UpdateScreenList()
        {
            cbo_Screens.Items.Clear();
            int i = 0;
            foreach(var screen in Screen.AllScreens)
            {
                cbo_Screens.Items.Add(screen);
                if(screen.Primary)
                {
                    cbo_Screens.SelectedIndex = i;
                }
                i++; 
            }
        }

        private void cbo_Screens_SelectedIndexChanged(object sender, EventArgs e)
        {
            var bounds = ((Screen)cbo_Screens.SelectedItem).Bounds;
            this.statsForm?.SetBounds(bounds.X, bounds.Y, bounds.Width, bounds.Height);
        }
    }
}
