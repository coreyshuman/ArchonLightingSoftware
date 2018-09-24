namespace ArchonLightingSystem
{
    using Components;
    partial class AppForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AppForm));
            this.FormUpdateTimer = new System.Windows.Forms.Timer(this.components);
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editConfigToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editEEPROMToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.debugToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.updateFirmwareToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pnl_FanMarker = new System.Windows.Forms.Panel();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            this.btn_1_1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.button7 = new System.Windows.Forms.Button();
            this.button8 = new System.Windows.Forms.Button();
            this.button9 = new System.Windows.Forms.Button();
            this.button10 = new System.Windows.Forms.Button();
            this.button11 = new System.Windows.Forms.Button();
            this.button12 = new System.Windows.Forms.Button();
            this.trk_FanSpeed1 = new System.Windows.Forms.TrackBar();
            this.cbo_LightMode = new System.Windows.Forms.ComboBox();
            this.grp_Device1 = new System.Windows.Forms.GroupBox();
            this.grp_FanSpeed1 = new System.Windows.Forms.GroupBox();
            this.lbl_LightingSpeed = new System.Windows.Forms.Label();
            this.cbo_LightSpeed = new System.Windows.Forms.ComboBox();
            this.lbl_LightingMode = new System.Windows.Forms.Label();
            this.lbl_Address = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.btn_SaveConfig = new System.Windows.Forms.Button();
            this.cbo_DeviceAddress = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.menuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trk_FanSpeed1)).BeginInit();
            this.grp_Device1.SuspendLayout();
            this.grp_FanSpeed1.SuspendLayout();
            this.SuspendLayout();
            // 
            // FormUpdateTimer
            // 
            this.FormUpdateTimer.Enabled = true;
            this.FormUpdateTimer.Interval = 6;
            this.FormUpdateTimer.Tick += new System.EventHandler(this.FormUpdateTimer_Tick);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.optionsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1331, 24);
            this.menuStrip1.TabIndex = 54;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.editConfigToolStripMenuItem,
            this.editEEPROMToolStripMenuItem,
            this.debugToolStripMenuItem,
            this.updateFirmwareToolStripMenuItem});
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.optionsToolStripMenuItem.Text = "Options";
            // 
            // editConfigToolStripMenuItem
            // 
            this.editConfigToolStripMenuItem.Name = "editConfigToolStripMenuItem";
            this.editConfigToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.editConfigToolStripMenuItem.Text = "Edit Config";
            this.editConfigToolStripMenuItem.Click += new System.EventHandler(this.editConfigToolStripMenuItem_Click);
            // 
            // editEEPROMToolStripMenuItem
            // 
            this.editEEPROMToolStripMenuItem.Name = "editEEPROMToolStripMenuItem";
            this.editEEPROMToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.editEEPROMToolStripMenuItem.Text = "Edit EEPROM";
            this.editEEPROMToolStripMenuItem.Click += new System.EventHandler(this.editEEPROMToolStripMenuItem_Click);
            // 
            // debugToolStripMenuItem
            // 
            this.debugToolStripMenuItem.Name = "debugToolStripMenuItem";
            this.debugToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.debugToolStripMenuItem.Text = "Debug";
            this.debugToolStripMenuItem.Click += new System.EventHandler(this.debugToolStripMenuItem_Click);
            // 
            // updateFirmwareToolStripMenuItem
            // 
            this.updateFirmwareToolStripMenuItem.Name = "updateFirmwareToolStripMenuItem";
            this.updateFirmwareToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.updateFirmwareToolStripMenuItem.Text = "Update Firmware";
            this.updateFirmwareToolStripMenuItem.Click += new System.EventHandler(this.updateFirmwareToolStripMenuItem_Click);
            // 
            // pnl_FanMarker
            // 
            this.pnl_FanMarker.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnl_FanMarker.Location = new System.Drawing.Point(6, 59);
            this.pnl_FanMarker.Name = "pnl_FanMarker";
            this.pnl_FanMarker.Size = new System.Drawing.Size(45, 290);
            this.pnl_FanMarker.TabIndex = 55;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusLabel});
            this.statusStrip1.Location = new System.Drawing.Point(0, 644);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1331, 22);
            this.statusStrip1.TabIndex = 56;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // statusLabel
            // 
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(59, 17);
            this.statusLabel.Text = "Loading...";
            // 
            // btn_1_1
            // 
            this.btn_1_1.Location = new System.Drawing.Point(16, 33);
            this.btn_1_1.Name = "btn_1_1";
            this.btn_1_1.Size = new System.Drawing.Size(44, 37);
            this.btn_1_1.TabIndex = 57;
            this.btn_1_1.Text = "L12";
            this.btn_1_1.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(16, 75);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(36, 37);
            this.button2.TabIndex = 58;
            this.button2.Text = "L2";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(16, 117);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(36, 37);
            this.button3.TabIndex = 59;
            this.button3.Text = "L3";
            this.button3.UseVisualStyleBackColor = true;
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(16, 159);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(36, 37);
            this.button4.TabIndex = 60;
            this.button4.Text = "L4";
            this.button4.UseVisualStyleBackColor = true;
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(16, 330);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(36, 37);
            this.button5.TabIndex = 64;
            this.button5.Text = "L1";
            this.button5.UseVisualStyleBackColor = true;
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(16, 285);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(36, 37);
            this.button6.TabIndex = 63;
            this.button6.Text = "L1";
            this.button6.UseVisualStyleBackColor = true;
            // 
            // button7
            // 
            this.button7.Location = new System.Drawing.Point(16, 243);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(36, 37);
            this.button7.TabIndex = 62;
            this.button7.Text = "L1";
            this.button7.UseVisualStyleBackColor = true;
            // 
            // button8
            // 
            this.button8.Location = new System.Drawing.Point(16, 201);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(36, 37);
            this.button8.TabIndex = 61;
            this.button8.Text = "L1";
            this.button8.UseVisualStyleBackColor = true;
            // 
            // button9
            // 
            this.button9.Location = new System.Drawing.Point(16, 501);
            this.button9.Name = "button9";
            this.button9.Size = new System.Drawing.Size(36, 37);
            this.button9.TabIndex = 68;
            this.button9.Text = "L1";
            this.button9.UseVisualStyleBackColor = true;
            // 
            // button10
            // 
            this.button10.Location = new System.Drawing.Point(16, 456);
            this.button10.Name = "button10";
            this.button10.Size = new System.Drawing.Size(36, 37);
            this.button10.TabIndex = 67;
            this.button10.Text = "L1";
            this.button10.UseVisualStyleBackColor = true;
            // 
            // button11
            // 
            this.button11.Location = new System.Drawing.Point(16, 414);
            this.button11.Name = "button11";
            this.button11.Size = new System.Drawing.Size(36, 37);
            this.button11.TabIndex = 66;
            this.button11.Text = "L1";
            this.button11.UseVisualStyleBackColor = true;
            // 
            // button12
            // 
            this.button12.Location = new System.Drawing.Point(16, 372);
            this.button12.Name = "button12";
            this.button12.Size = new System.Drawing.Size(36, 37);
            this.button12.TabIndex = 65;
            this.button12.Text = "L1";
            this.button12.UseVisualStyleBackColor = true;
            // 
            // trk_FanSpeed1
            // 
            this.trk_FanSpeed1.LargeChange = 10;
            this.trk_FanSpeed1.Location = new System.Drawing.Point(74, 59);
            this.trk_FanSpeed1.Maximum = 100;
            this.trk_FanSpeed1.Name = "trk_FanSpeed1";
            this.trk_FanSpeed1.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.trk_FanSpeed1.Size = new System.Drawing.Size(45, 290);
            this.trk_FanSpeed1.SmallChange = 5;
            this.trk_FanSpeed1.TabIndex = 69;
            this.trk_FanSpeed1.TickFrequency = 10;
            // 
            // cbo_LightMode
            // 
            this.cbo_LightMode.FormattingEnabled = true;
            this.cbo_LightMode.Location = new System.Drawing.Point(70, 87);
            this.cbo_LightMode.Name = "cbo_LightMode";
            this.cbo_LightMode.Size = new System.Drawing.Size(121, 28);
            this.cbo_LightMode.TabIndex = 70;
            // 
            // grp_Device1
            // 
            this.grp_Device1.Controls.Add(this.grp_FanSpeed1);
            this.grp_Device1.Controls.Add(this.lbl_LightingSpeed);
            this.grp_Device1.Controls.Add(this.cbo_LightSpeed);
            this.grp_Device1.Controls.Add(this.lbl_LightingMode);
            this.grp_Device1.Controls.Add(this.btn_1_1);
            this.grp_Device1.Controls.Add(this.cbo_LightMode);
            this.grp_Device1.Controls.Add(this.button2);
            this.grp_Device1.Controls.Add(this.button9);
            this.grp_Device1.Controls.Add(this.button3);
            this.grp_Device1.Controls.Add(this.button10);
            this.grp_Device1.Controls.Add(this.button4);
            this.grp_Device1.Controls.Add(this.button11);
            this.grp_Device1.Controls.Add(this.button8);
            this.grp_Device1.Controls.Add(this.button12);
            this.grp_Device1.Controls.Add(this.button7);
            this.grp_Device1.Controls.Add(this.button5);
            this.grp_Device1.Controls.Add(this.button6);
            this.grp_Device1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grp_Device1.Location = new System.Drawing.Point(29, 74);
            this.grp_Device1.Name = "grp_Device1";
            this.grp_Device1.Size = new System.Drawing.Size(251, 554);
            this.grp_Device1.TabIndex = 71;
            this.grp_Device1.TabStop = false;
            this.grp_Device1.Text = "Device 1";
            this.grp_Device1.Visible = false;
            // 
            // grp_FanSpeed1
            // 
            this.grp_FanSpeed1.Controls.Add(this.pnl_FanMarker);
            this.grp_FanSpeed1.Controls.Add(this.trk_FanSpeed1);
            this.grp_FanSpeed1.Location = new System.Drawing.Point(78, 193);
            this.grp_FanSpeed1.Name = "grp_FanSpeed1";
            this.grp_FanSpeed1.Size = new System.Drawing.Size(120, 355);
            this.grp_FanSpeed1.TabIndex = 72;
            this.grp_FanSpeed1.TabStop = false;
            this.grp_FanSpeed1.Text = "Fan Speed";
            // 
            // lbl_LightingSpeed
            // 
            this.lbl_LightingSpeed.AutoSize = true;
            this.lbl_LightingSpeed.Location = new System.Drawing.Point(74, 132);
            this.lbl_LightingSpeed.Name = "lbl_LightingSpeed";
            this.lbl_LightingSpeed.Size = new System.Drawing.Size(116, 20);
            this.lbl_LightingSpeed.TabIndex = 74;
            this.lbl_LightingSpeed.Text = "Lighting Speed";
            // 
            // cbo_LightSpeed
            // 
            this.cbo_LightSpeed.FormattingEnabled = true;
            this.cbo_LightSpeed.Location = new System.Drawing.Point(70, 155);
            this.cbo_LightSpeed.Name = "cbo_LightSpeed";
            this.cbo_LightSpeed.Size = new System.Drawing.Size(121, 28);
            this.cbo_LightSpeed.TabIndex = 73;
            // 
            // lbl_LightingMode
            // 
            this.lbl_LightingMode.AutoSize = true;
            this.lbl_LightingMode.Location = new System.Drawing.Point(74, 64);
            this.lbl_LightingMode.Name = "lbl_LightingMode";
            this.lbl_LightingMode.Size = new System.Drawing.Size(109, 20);
            this.lbl_LightingMode.TabIndex = 72;
            this.lbl_LightingMode.Text = "Lighting Mode";
            // 
            // lbl_Address
            // 
            this.lbl_Address.AutoSize = true;
            this.lbl_Address.Location = new System.Drawing.Point(540, 39);
            this.lbl_Address.Name = "lbl_Address";
            this.lbl_Address.Size = new System.Drawing.Size(13, 13);
            this.lbl_Address.TabIndex = 76;
            this.lbl_Address.Text = "--";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(462, 39);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(48, 13);
            this.label6.TabIndex = 75;
            this.label6.Text = "Address:";
            // 
            // btn_SaveConfig
            // 
            this.btn_SaveConfig.Location = new System.Drawing.Point(310, 27);
            this.btn_SaveConfig.Name = "btn_SaveConfig";
            this.btn_SaveConfig.Size = new System.Drawing.Size(105, 37);
            this.btn_SaveConfig.TabIndex = 75;
            this.btn_SaveConfig.Text = "Save Configuration";
            this.btn_SaveConfig.UseVisualStyleBackColor = true;
            this.btn_SaveConfig.Click += new System.EventHandler(this.btn_SaveConfig_Click);
            // 
            // cbo_DeviceAddress
            // 
            this.cbo_DeviceAddress.FormattingEnabled = true;
            this.cbo_DeviceAddress.Location = new System.Drawing.Point(159, 36);
            this.cbo_DeviceAddress.Name = "cbo_DeviceAddress";
            this.cbo_DeviceAddress.Size = new System.Drawing.Size(121, 21);
            this.cbo_DeviceAddress.TabIndex = 77;
            this.cbo_DeviceAddress.SelectedIndexChanged += new System.EventHandler(this.cbo_DeviceAddress_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(26, 37);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(127, 16);
            this.label1.TabIndex = 78;
            this.label1.Text = "Controller (Address)";
            // 
            // AppForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1331, 666);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cbo_DeviceAddress);
            this.Controls.Add(this.btn_SaveConfig);
            this.Controls.Add(this.lbl_Address);
            this.Controls.Add(this.grp_Device1);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "AppForm";
            this.Text = "Archon Lighting System";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trk_FanSpeed1)).EndInit();
            this.grp_Device1.ResumeLayout(false);
            this.grp_Device1.PerformLayout();
            this.grp_FanSpeed1.ResumeLayout(false);
            this.grp_FanSpeed1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Timer FormUpdateTimer;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editConfigToolStripMenuItem;
        private System.Windows.Forms.Panel pnl_FanMarker;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel statusLabel;
        private System.Windows.Forms.ColorDialog colorDialog1;
        private System.Windows.Forms.Button btn_1_1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.Button button8;
        private System.Windows.Forms.Button button9;
        private System.Windows.Forms.Button button10;
        private System.Windows.Forms.Button button11;
        private System.Windows.Forms.Button button12;
        private System.Windows.Forms.TrackBar trk_FanSpeed1;
        private System.Windows.Forms.ComboBox cbo_LightMode;
        private System.Windows.Forms.GroupBox grp_Device1;
        private System.Windows.Forms.GroupBox grp_FanSpeed1;
        private System.Windows.Forms.Label lbl_LightingSpeed;
        private System.Windows.Forms.ComboBox cbo_LightSpeed;
        private System.Windows.Forms.Label lbl_LightingMode;
        private System.Windows.Forms.Label lbl_Address;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ToolStripMenuItem editEEPROMToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem debugToolStripMenuItem;
        private System.Windows.Forms.Button btn_SaveConfig;
        private System.Windows.Forms.ToolStripMenuItem updateFirmwareToolStripMenuItem;
        private System.Windows.Forms.ComboBox cbo_DeviceAddress;
        private System.Windows.Forms.Label label1;
    }
}

