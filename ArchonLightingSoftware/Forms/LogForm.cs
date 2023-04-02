using ArchonLightingSystem.Common;
using ArchonLightingSystem.Components;
using ArchonLightingSystem.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace ArchonLightingSystem.Forms
{
    public partial class LogForm : Form
    {
        private uint lineNumber = 0;

        public LogForm()
        {
            InitializeComponent();

            cb_Level.Items.Clear();
            cb_Level.Items.AddRange(Enum.GetNames(typeof(Level)));
            cb_Level.SelectedIndex = (int)Logger.GetLevel();

            Logger.LatestLogEvent += HandleLogEvent;
            GetLogs();
        }

        private void GetLogs()
        {
            Logger.LatestLogEvent -= HandleLogEvent;
            lineNumber = 0;
            txt_log.Clear();
            Logger.GetLogs().ForEach(log => WriteLine(log));
            Logger.LatestLogEvent += HandleLogEvent;
        }

        void HandleLogEvent(object sender, LogEventArgs eventArgs)
        {
            WriteLine(eventArgs.Log);
        }

        void WriteLine(Log log)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action<Log>(WriteLine), new object[] { log });
                return;
            }

            var textColor = Color.White;
            string msg = $"{lineNumber,4} ";
            switch (log.Level)
            {
                case Level.Error:
                    textColor = Color.PaleVioletRed;
                    msg += "[ERROR]";
                    break;
                case Level.Warning:
                    textColor = Color.Goldenrod;
                    msg += "[WARN ]";
                    break;
                case Level.Debug:
                    textColor = Color.MediumOrchid;
                    msg += "[DEBUG]";
                    break;
                case Level.Trace:
                    textColor = Color.LightSkyBlue;
                    msg += "[TRACE]";
                    break;
                case Level.Information:
                    msg += "[INFO ]";
                    break;
            }

            msg += $" {log.Message}" + Environment.NewLine;

            lineNumber++;
            try 
            { 
                txt_log.SelectionColor = textColor;
                txt_log.SelectionBackColor = (lineNumber % 2 == 0) ? txt_log.BackColor : AppColors.Background;
                txt_log.AppendText(msg);
            }
            catch(ObjectDisposedException) 
            { 
            
            }
        }

        private void LogForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Logger.LatestLogEvent -= HandleLogEvent;
        }

        private void comboBoxLevel_SelectedIndexChanged(object sender, EventArgs e)
        {
            Level lev = (Level)((ComboBox)sender).SelectedIndex;
            Logger.SetLevel(lev);
            GetLogs();
        }
    }
}
