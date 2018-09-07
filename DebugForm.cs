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
using ArchonLightingSystem.Interfaces;

namespace ArchonLightingSystem
{
    public partial class DebugForm : SubformBase
    {
        ApplicationData applicationData;
        public DebugForm()
        {
            InitializeComponent();
        }

        public void InitializeForm(ApplicationData appData)
        {
            applicationData = appData;
        }

        public void UpdateFormData()
        {

        }

        private void btn_ReadDebug_Click(object sender, EventArgs e)
        {
            applicationData.ReadDebugPending = true;
            updateFormTimer.Enabled = true;
        }

        private void updateFormTimer_Tick(object sender, EventArgs e)
        {
            txt_Debug.Text += applicationData.Debug;
            updateFormTimer.Enabled = false;
        }

        private void btn_ClearScreen_Click(object sender, EventArgs e)
        {
            txt_Debug.Text = "";
        }
    }
}
