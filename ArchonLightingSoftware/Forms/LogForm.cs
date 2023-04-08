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
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace ArchonLightingSystem.Forms
{
    public partial class LogForm : Form
    {
        private uint lineNumber = 0;
        private object eventLock = new object();
        private SemaphoreSlim updateSemaphore = new SemaphoreSlim(1,1);

        public LogForm()
        {
            InitializeComponent();

            cb_Level.Items.Clear();
            cb_Level.Items.AddRange(Enum.GetNames(typeof(Level)));
            cb_Level.SelectedIndex = (int)Logger.GetLevel();
        }

        private void GetLogs()
        {
            updateSemaphore.Wait();
            try
            {
                lineNumber = 0;
                txt_log.Clear();
                Logger.GetLogs().ForEach(log => WriteLine(log));
            }
            finally
            {
                updateSemaphore.Release();
            }
        }

        void HandleLogEvent(object sender, LogEventArgs eventArgs)
        {
            updateSemaphore.Wait();
            try
            {
                WriteLine(eventArgs.Log);
            }
            finally
            {
                updateSemaphore.Release();
            }
        }

        void WriteLine(Log log)
        {
            if (InvokeRequired)
            {
                this.BeginInvoke(new Action<Log>(WriteLine), new object[] { log });
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

            if(lev != Logger.GetLevel())
            {
                Logger.SetLevel(lev);
                GetLogs();
            }        
        }

        private void btn_CopyToClipboard_Click(object sender, EventArgs e)
        {
            Clipboard.SetData(DataFormats.Text, (Object)txt_log.Text);
        }

        private void LogForm_Load(object sender, EventArgs e)
        {
            Logger.LatestLogEvent += HandleLogEvent;
            GetLogs();
        }
    }
}
