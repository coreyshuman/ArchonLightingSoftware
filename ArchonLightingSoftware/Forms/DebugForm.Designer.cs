namespace ArchonLightingSystem
{
    partial class DebugForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DebugForm));
            this.btn_ReadDebug = new System.Windows.Forms.Button();
            this.txt_Debug = new System.Windows.Forms.TextBox();
            this.updateFormTimer = new System.Windows.Forms.Timer(this.components);
            this.btn_ClearScreen = new System.Windows.Forms.Button();
            this.cbo_DeviceAddress = new System.Windows.Forms.ComboBox();
            this.btn_Close = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btn_ReadDebug
            // 
            this.btn_ReadDebug.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.btn_ReadDebug.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btn_ReadDebug.Location = new System.Drawing.Point(200, 15);
            this.btn_ReadDebug.Margin = new System.Windows.Forms.Padding(4);
            this.btn_ReadDebug.Name = "btn_ReadDebug";
            this.btn_ReadDebug.Size = new System.Drawing.Size(109, 46);
            this.btn_ReadDebug.TabIndex = 0;
            this.btn_ReadDebug.Text = "Read Debug";
            this.btn_ReadDebug.UseVisualStyleBackColor = false;
            this.btn_ReadDebug.Click += new System.EventHandler(this.btn_ReadDebug_Click);
            // 
            // txt_Debug
            // 
            this.txt_Debug.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.txt_Debug.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.txt_Debug.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_Debug.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(233)))), ((int)(((byte)(243)))));
            this.txt_Debug.Location = new System.Drawing.Point(0, 91);
            this.txt_Debug.Margin = new System.Windows.Forms.Padding(4);
            this.txt_Debug.Multiline = true;
            this.txt_Debug.Name = "txt_Debug";
            this.txt_Debug.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txt_Debug.Size = new System.Drawing.Size(783, 405);
            this.txt_Debug.TabIndex = 1;
            // 
            // updateFormTimer
            // 
            this.updateFormTimer.Tick += new System.EventHandler(this.updateFormTimer_Tick);
            // 
            // btn_ClearScreen
            // 
            this.btn_ClearScreen.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.btn_ClearScreen.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btn_ClearScreen.Location = new System.Drawing.Point(317, 15);
            this.btn_ClearScreen.Margin = new System.Windows.Forms.Padding(4);
            this.btn_ClearScreen.Name = "btn_ClearScreen";
            this.btn_ClearScreen.Size = new System.Drawing.Size(109, 46);
            this.btn_ClearScreen.TabIndex = 2;
            this.btn_ClearScreen.Text = "Clear Screen";
            this.btn_ClearScreen.UseVisualStyleBackColor = false;
            this.btn_ClearScreen.Click += new System.EventHandler(this.btn_ClearScreen_Click);
            // 
            // cbo_DeviceAddress
            // 
            this.cbo_DeviceAddress.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.cbo_DeviceAddress.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.cbo_DeviceAddress.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(233)))), ((int)(((byte)(243)))));
            this.cbo_DeviceAddress.FormattingEnabled = true;
            this.cbo_DeviceAddress.Location = new System.Drawing.Point(16, 26);
            this.cbo_DeviceAddress.Margin = new System.Windows.Forms.Padding(4);
            this.cbo_DeviceAddress.Name = "cbo_DeviceAddress";
            this.cbo_DeviceAddress.Size = new System.Drawing.Size(160, 24);
            this.cbo_DeviceAddress.TabIndex = 3;
            this.cbo_DeviceAddress.SelectedIndexChanged += new System.EventHandler(this.cbo_DeviceAddress_SelectedIndexChanged);
            // 
            // btn_Close
            // 
            this.btn_Close.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.btn_Close.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btn_Close.Location = new System.Drawing.Point(434, 13);
            this.btn_Close.Margin = new System.Windows.Forms.Padding(4);
            this.btn_Close.Name = "btn_Close";
            this.btn_Close.Size = new System.Drawing.Size(109, 46);
            this.btn_Close.TabIndex = 4;
            this.btn_Close.Text = "Close";
            this.btn_Close.UseVisualStyleBackColor = false;
            this.btn_Close.Click += new System.EventHandler(this.btn_Close_Click);
            // 
            // DebugForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.ClientSize = new System.Drawing.Size(783, 496);
            this.Controls.Add(this.btn_Close);
            this.Controls.Add(this.cbo_DeviceAddress);
            this.Controls.Add(this.btn_ClearScreen);
            this.Controls.Add(this.txt_Debug);
            this.Controls.Add(this.btn_ReadDebug);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(233)))), ((int)(((byte)(243)))));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "DebugForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Debug Form";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btn_ReadDebug;
        private System.Windows.Forms.TextBox txt_Debug;
        private System.Windows.Forms.Timer updateFormTimer;
        private System.Windows.Forms.Button btn_ClearScreen;
        private System.Windows.Forms.ComboBox cbo_DeviceAddress;
        private System.Windows.Forms.Button btn_Close;
    }
}