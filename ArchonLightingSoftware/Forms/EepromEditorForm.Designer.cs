namespace ArchonLightingSystem
{
    partial class EepromEditorForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EepromEditorForm));
            this.eepromGridView = new System.Windows.Forms.DataGridView();
            this.btn_ReadEeprom = new System.Windows.Forms.Button();
            this.btn_WriteEeprom = new System.Windows.Forms.Button();
            this.formUpdateTimer = new System.Windows.Forms.Timer(this.components);
            this.cbo_DeviceAddress = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lbl_Disconnected = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.eepromGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // eepromGridView
            // 
            this.eepromGridView.AllowUserToAddRows = false;
            this.eepromGridView.AllowUserToDeleteRows = false;
            this.eepromGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.eepromGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.eepromGridView.Location = new System.Drawing.Point(0, 71);
            this.eepromGridView.Name = "eepromGridView";
            this.eepromGridView.Size = new System.Drawing.Size(841, 381);
            this.eepromGridView.TabIndex = 0;
            this.eepromGridView.DataBindingComplete += new System.Windows.Forms.DataGridViewBindingCompleteEventHandler(this.eepromGridView_DataBindingComplete);
            // 
            // btn_ReadEeprom
            // 
            this.btn_ReadEeprom.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_ReadEeprom.BackColor = System.Drawing.SystemColors.Control;
            this.btn_ReadEeprom.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btn_ReadEeprom.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.btn_ReadEeprom.Location = new System.Drawing.Point(583, 13);
            this.btn_ReadEeprom.Name = "btn_ReadEeprom";
            this.btn_ReadEeprom.Size = new System.Drawing.Size(120, 35);
            this.btn_ReadEeprom.TabIndex = 1;
            this.btn_ReadEeprom.Text = "Read EEPROM";
            this.btn_ReadEeprom.UseVisualStyleBackColor = false;
            this.btn_ReadEeprom.Click += new System.EventHandler(this.btn_ReadEeprom_Click);
            // 
            // btn_WriteEeprom
            // 
            this.btn_WriteEeprom.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_WriteEeprom.BackColor = System.Drawing.SystemColors.Control;
            this.btn_WriteEeprom.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btn_WriteEeprom.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.btn_WriteEeprom.Location = new System.Drawing.Point(709, 13);
            this.btn_WriteEeprom.Name = "btn_WriteEeprom";
            this.btn_WriteEeprom.Size = new System.Drawing.Size(120, 35);
            this.btn_WriteEeprom.TabIndex = 2;
            this.btn_WriteEeprom.Text = "Write EEPROM";
            this.btn_WriteEeprom.UseVisualStyleBackColor = false;
            this.btn_WriteEeprom.Click += new System.EventHandler(this.btn_WriteEeprom_Click);
            // 
            // formUpdateTimer
            // 
            this.formUpdateTimer.Tick += new System.EventHandler(this.formUpdateTimer_Tick);
            // 
            // cbo_DeviceAddress
            // 
            this.cbo_DeviceAddress.BackColor = System.Drawing.SystemColors.Control;
            this.cbo_DeviceAddress.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.cbo_DeviceAddress.ForeColor = System.Drawing.SystemColors.ControlText;
            this.cbo_DeviceAddress.FormattingEnabled = true;
            this.cbo_DeviceAddress.Location = new System.Drawing.Point(76, 21);
            this.cbo_DeviceAddress.Margin = new System.Windows.Forms.Padding(4);
            this.cbo_DeviceAddress.Name = "cbo_DeviceAddress";
            this.cbo_DeviceAddress.Size = new System.Drawing.Size(160, 21);
            this.cbo_DeviceAddress.TabIndex = 4;
            this.cbo_DeviceAddress.SelectedIndexChanged += new System.EventHandler(this.cbo_DeviceAddress_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(51, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Device:";
            // 
            // lbl_Disconnected
            // 
            this.lbl_Disconnected.AutoSize = true;
            this.lbl_Disconnected.Location = new System.Drawing.Point(253, 24);
            this.lbl_Disconnected.Name = "lbl_Disconnected";
            this.lbl_Disconnected.Size = new System.Drawing.Size(73, 13);
            this.lbl_Disconnected.TabIndex = 6;
            this.lbl_Disconnected.Text = "Disconnected";
            // 
            // EepromEditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(841, 452);
            this.Controls.Add(this.lbl_Disconnected);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cbo_DeviceAddress);
            this.Controls.Add(this.btn_WriteEeprom);
            this.Controls.Add(this.btn_ReadEeprom);
            this.Controls.Add(this.eepromGridView);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EepromEditorForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Eeprom Editor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.EepromEditorForm_FormClosing);
            this.Load += new System.EventHandler(this.EepromEditorForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.eepromGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView eepromGridView;
        private System.Windows.Forms.Button btn_ReadEeprom;
        private System.Windows.Forms.Button btn_WriteEeprom;
        private System.Windows.Forms.Timer formUpdateTimer;
        private System.Windows.Forms.ComboBox cbo_DeviceAddress;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lbl_Disconnected;
    }
}