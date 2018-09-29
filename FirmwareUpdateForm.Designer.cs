namespace ArchonLightingSystem
{
    partial class FirmwareUpdateForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FirmwareUpdateForm));
            this.listView1 = new System.Windows.Forms.ListView();
            this.label1 = new System.Windows.Forms.Label();
            this.lbl_CurrentVersion = new System.Windows.Forms.Label();
            this.btn_UpdateAll = new System.Windows.Forms.Button();
            this.lbl_Status = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.btn_Cancel = new System.Windows.Forms.Button();
            this.formUpdateTimer = new System.Windows.Forms.Timer(this.components);
            this.btn_OpenHexFile = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.btn_StartApp = new System.Windows.Forms.Button();
            this.timer_ResetHardware = new System.Windows.Forms.Timer(this.components);
            this.timer_EnableUsb = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // listView1
            // 
            this.listView1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.listView1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(233)))), ((int)(((byte)(243)))));
            this.listView1.GridLines = true;
            this.listView1.Location = new System.Drawing.Point(35, 87);
            this.listView1.Margin = new System.Windows.Forms.Padding(4);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(637, 571);
            this.listView1.TabIndex = 0;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(32, 15);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(118, 16);
            this.label1.TabIndex = 1;
            this.label1.Text = "Current Version:";
            // 
            // lbl_CurrentVersion
            // 
            this.lbl_CurrentVersion.AutoSize = true;
            this.lbl_CurrentVersion.Location = new System.Drawing.Point(158, 15);
            this.lbl_CurrentVersion.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbl_CurrentVersion.Name = "lbl_CurrentVersion";
            this.lbl_CurrentVersion.Size = new System.Drawing.Size(32, 16);
            this.lbl_CurrentVersion.TabIndex = 2;
            this.lbl_CurrentVersion.Text = "v?.?";
            // 
            // btn_UpdateAll
            // 
            this.btn_UpdateAll.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.btn_UpdateAll.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btn_UpdateAll.Location = new System.Drawing.Point(355, 14);
            this.btn_UpdateAll.Name = "btn_UpdateAll";
            this.btn_UpdateAll.Size = new System.Drawing.Size(86, 33);
            this.btn_UpdateAll.TabIndex = 3;
            this.btn_UpdateAll.Text = "Update All";
            this.btn_UpdateAll.UseVisualStyleBackColor = false;
            this.btn_UpdateAll.Click += new System.EventHandler(this.btn_UpdateAll_Click);
            // 
            // lbl_Status
            // 
            this.lbl_Status.AutoSize = true;
            this.lbl_Status.Location = new System.Drawing.Point(95, 37);
            this.lbl_Status.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbl_Status.Name = "lbl_Status";
            this.lbl_Status.Size = new System.Drawing.Size(66, 16);
            this.lbl_Status.TabIndex = 5;
            this.lbl_Status.Text = "Loading...";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(32, 37);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(55, 16);
            this.label3.TabIndex = 4;
            this.label3.Text = "Status:";
            // 
            // btn_Cancel
            // 
            this.btn_Cancel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.btn_Cancel.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btn_Cancel.Location = new System.Drawing.Point(457, 14);
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.Size = new System.Drawing.Size(86, 33);
            this.btn_Cancel.TabIndex = 6;
            this.btn_Cancel.Text = "Cancel";
            this.btn_Cancel.UseVisualStyleBackColor = false;
            this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
            // 
            // btn_OpenHexFile
            // 
            this.btn_OpenHexFile.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.btn_OpenHexFile.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btn_OpenHexFile.Location = new System.Drawing.Point(253, 14);
            this.btn_OpenHexFile.Name = "btn_OpenHexFile";
            this.btn_OpenHexFile.Size = new System.Drawing.Size(86, 33);
            this.btn_OpenHexFile.TabIndex = 7;
            this.btn_OpenHexFile.Text = "Open Hex";
            this.btn_OpenHexFile.UseVisualStyleBackColor = false;
            this.btn_OpenHexFile.Click += new System.EventHandler(this.btn_OpenHexFile_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(35, 57);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(508, 23);
            this.progressBar1.TabIndex = 8;
            // 
            // btn_StartApp
            // 
            this.btn_StartApp.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.btn_StartApp.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btn_StartApp.Location = new System.Drawing.Point(558, 14);
            this.btn_StartApp.Name = "btn_StartApp";
            this.btn_StartApp.Size = new System.Drawing.Size(86, 33);
            this.btn_StartApp.TabIndex = 9;
            this.btn_StartApp.Text = "Start App";
            this.btn_StartApp.UseVisualStyleBackColor = false;
            this.btn_StartApp.Click += new System.EventHandler(this.btn_StartApp_Click);
            // 
            // timer_ResetHardware
            // 
            this.timer_ResetHardware.Interval = 500;
            this.timer_ResetHardware.Tick += new System.EventHandler(this.timer_ResetHardware_Tick);
            // 
            // timer_EnableUsb
            // 
            this.timer_EnableUsb.Interval = 2000;
            this.timer_EnableUsb.Tick += new System.EventHandler(this.timer_EnableUsb_Tick);
            // 
            // FirmwareUpdateForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.ClientSize = new System.Drawing.Size(685, 662);
            this.Controls.Add(this.btn_StartApp);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.btn_OpenHexFile);
            this.Controls.Add(this.btn_Cancel);
            this.Controls.Add(this.lbl_Status);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btn_UpdateAll);
            this.Controls.Add(this.lbl_CurrentVersion);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.listView1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(233)))), ((int)(((byte)(243)))));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.Name = "FirmwareUpdateForm";
            this.Text = "Firmware Updater";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FirmwareUpdateForm_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lbl_CurrentVersion;
        private System.Windows.Forms.Button btn_UpdateAll;
        private System.Windows.Forms.Label lbl_Status;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btn_Cancel;
        private System.Windows.Forms.Timer formUpdateTimer;
        private System.Windows.Forms.Button btn_OpenHexFile;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Button btn_StartApp;
        private System.Windows.Forms.Timer timer_ResetHardware;
        private System.Windows.Forms.Timer timer_EnableUsb;
    }
}