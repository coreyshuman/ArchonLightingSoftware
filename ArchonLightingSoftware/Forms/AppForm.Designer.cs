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
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.startWithWindowsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sequencerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pnl_TemperatureMarker = new System.Windows.Forms.Panel();
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
            this.txt_Name = new System.Windows.Forms.TextBox();
            this.lbl_Name = new System.Windows.Forms.Label();
            this.grp_FanSpeed1 = new System.Windows.Forms.GroupBox();
            this.lbl_FanUnits = new System.Windows.Forms.Label();
            this.btn_FanConfig = new System.Windows.Forms.Button();
            this.lbl_FanControls = new System.Windows.Forms.Label();
            this.pnl_FanMarker = new System.Windows.Forms.Panel();
            this.lbl_LightingSpeed = new System.Windows.Forms.Label();
            this.cbo_LightSpeed = new System.Windows.Forms.ComboBox();
            this.lbl_LightingMode = new System.Windows.Forms.Label();
            this.btn_SaveConfig = new System.Windows.Forms.Button();
            this.cbo_DeviceAddress = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.notifiyIconContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.closeToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.label2 = new System.Windows.Forms.Label();
            this.txt_ControllerName = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.chk_Enabled = new System.Windows.Forms.CheckBox();
            this.menuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trk_FanSpeed1)).BeginInit();
            this.grp_Device1.SuspendLayout();
            this.grp_FanSpeed1.SuspendLayout();
            this.notifiyIconContextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // FormUpdateTimer
            // 
            this.FormUpdateTimer.Interval = 50;
            this.FormUpdateTimer.Tick += new System.EventHandler(this.FormUpdateTimer_Tick);
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.optionsToolStripMenuItem,
            this.closeToolStripMenuItem});
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
            this.updateFirmwareToolStripMenuItem,
            this.settingsToolStripMenuItem,
            this.sequencerToolStripMenuItem});
            this.optionsToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(233)))), ((int)(((byte)(243)))));
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.optionsToolStripMenuItem.Text = "Options";
            // 
            // editConfigToolStripMenuItem
            // 
            this.editConfigToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.editConfigToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(233)))), ((int)(((byte)(243)))));
            this.editConfigToolStripMenuItem.Name = "editConfigToolStripMenuItem";
            this.editConfigToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.editConfigToolStripMenuItem.Text = "Edit Config";
            this.editConfigToolStripMenuItem.Click += new System.EventHandler(this.editConfigToolStripMenuItem_Click);
            // 
            // editEEPROMToolStripMenuItem
            // 
            this.editEEPROMToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.editEEPROMToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(233)))), ((int)(((byte)(243)))));
            this.editEEPROMToolStripMenuItem.Name = "editEEPROMToolStripMenuItem";
            this.editEEPROMToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.editEEPROMToolStripMenuItem.Text = "Edit EEPROM";
            this.editEEPROMToolStripMenuItem.Click += new System.EventHandler(this.editEEPROMToolStripMenuItem_Click);
            // 
            // debugToolStripMenuItem
            // 
            this.debugToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.debugToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(233)))), ((int)(((byte)(243)))));
            this.debugToolStripMenuItem.Name = "debugToolStripMenuItem";
            this.debugToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.debugToolStripMenuItem.Text = "Debug";
            this.debugToolStripMenuItem.Click += new System.EventHandler(this.debugToolStripMenuItem_Click);
            // 
            // updateFirmwareToolStripMenuItem
            // 
            this.updateFirmwareToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.updateFirmwareToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(233)))), ((int)(((byte)(243)))));
            this.updateFirmwareToolStripMenuItem.Name = "updateFirmwareToolStripMenuItem";
            this.updateFirmwareToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.updateFirmwareToolStripMenuItem.Text = "Update Firmware";
            this.updateFirmwareToolStripMenuItem.Click += new System.EventHandler(this.updateFirmwareToolStripMenuItem_Click);
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.settingsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.startWithWindowsToolStripMenuItem});
            this.settingsToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(233)))), ((int)(((byte)(243)))));
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.settingsToolStripMenuItem.Text = "Settings";
            // 
            // startWithWindowsToolStripMenuItem
            // 
            this.startWithWindowsToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.startWithWindowsToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(233)))), ((int)(((byte)(243)))));
            this.startWithWindowsToolStripMenuItem.Name = "startWithWindowsToolStripMenuItem";
            this.startWithWindowsToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
            this.startWithWindowsToolStripMenuItem.Text = "Start With Windows";
            this.startWithWindowsToolStripMenuItem.Click += new System.EventHandler(this.startWithWindowsToolStripMenuItem_Click);
            // 
            // sequencerToolStripMenuItem
            // 
            this.sequencerToolStripMenuItem.Name = "sequencerToolStripMenuItem";
            this.sequencerToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.sequencerToolStripMenuItem.Text = "Sequencer";
            this.sequencerToolStripMenuItem.Click += new System.EventHandler(this.sequencerToolStripMenuItem_Click);
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.closeToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(233)))), ((int)(((byte)(243)))));
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
            this.closeToolStripMenuItem.Text = "Close";
            this.closeToolStripMenuItem.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
            // 
            // pnl_TemperatureMarker
            // 
            this.pnl_TemperatureMarker.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnl_TemperatureMarker.Location = new System.Drawing.Point(12, 125);
            this.pnl_TemperatureMarker.Name = "pnl_TemperatureMarker";
            this.pnl_TemperatureMarker.Size = new System.Drawing.Size(40, 245);
            this.pnl_TemperatureMarker.TabIndex = 55;
            // 
            // statusStrip1
            // 
            this.statusStrip1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusLabel});
            this.statusStrip1.Location = new System.Drawing.Point(0, 714);
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
            this.btn_1_1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.btn_1_1.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btn_1_1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btn_1_1.Location = new System.Drawing.Point(16, 33);
            this.btn_1_1.Name = "btn_1_1";
            this.btn_1_1.Size = new System.Drawing.Size(44, 37);
            this.btn_1_1.TabIndex = 57;
            this.btn_1_1.Text = "L12";
            this.btn_1_1.UseVisualStyleBackColor = false;
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
            this.trk_FanSpeed1.Location = new System.Drawing.Point(103, 125);
            this.trk_FanSpeed1.Maximum = 100;
            this.trk_FanSpeed1.Name = "trk_FanSpeed1";
            this.trk_FanSpeed1.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.trk_FanSpeed1.Size = new System.Drawing.Size(45, 245);
            this.trk_FanSpeed1.SmallChange = 5;
            this.trk_FanSpeed1.TabIndex = 69;
            this.trk_FanSpeed1.TickFrequency = 10;
            // 
            // cbo_LightMode
            // 
            this.cbo_LightMode.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.cbo_LightMode.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.cbo_LightMode.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(233)))), ((int)(((byte)(243)))));
            this.cbo_LightMode.FormattingEnabled = true;
            this.cbo_LightMode.Location = new System.Drawing.Point(78, 93);
            this.cbo_LightMode.MaxDropDownItems = 30;
            this.cbo_LightMode.Name = "cbo_LightMode";
            this.cbo_LightMode.Size = new System.Drawing.Size(155, 28);
            this.cbo_LightMode.TabIndex = 70;
            // 
            // grp_Device1
            // 
            this.grp_Device1.Controls.Add(this.txt_Name);
            this.grp_Device1.Controls.Add(this.lbl_Name);
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
            this.grp_Device1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(233)))), ((int)(((byte)(243)))));
            this.grp_Device1.Location = new System.Drawing.Point(29, 117);
            this.grp_Device1.Name = "grp_Device1";
            this.grp_Device1.Size = new System.Drawing.Size(251, 577);
            this.grp_Device1.TabIndex = 71;
            this.grp_Device1.TabStop = false;
            this.grp_Device1.Text = "Device 1";
            this.grp_Device1.Visible = false;
            // 
            // txt_Name
            // 
            this.txt_Name.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.txt_Name.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_Name.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(233)))), ((int)(((byte)(243)))));
            this.txt_Name.Location = new System.Drawing.Point(78, 41);
            this.txt_Name.Name = "txt_Name";
            this.txt_Name.Size = new System.Drawing.Size(155, 26);
            this.txt_Name.TabIndex = 83;
            // 
            // lbl_Name
            // 
            this.lbl_Name.AutoSize = true;
            this.lbl_Name.Location = new System.Drawing.Point(101, 20);
            this.lbl_Name.Name = "lbl_Name";
            this.lbl_Name.Size = new System.Drawing.Size(103, 20);
            this.lbl_Name.TabIndex = 75;
            this.lbl_Name.Text = "Device Name";
            // 
            // grp_FanSpeed1
            // 
            this.grp_FanSpeed1.Controls.Add(this.lbl_FanUnits);
            this.grp_FanSpeed1.Controls.Add(this.btn_FanConfig);
            this.grp_FanSpeed1.Controls.Add(this.lbl_FanControls);
            this.grp_FanSpeed1.Controls.Add(this.pnl_FanMarker);
            this.grp_FanSpeed1.Controls.Add(this.pnl_TemperatureMarker);
            this.grp_FanSpeed1.Controls.Add(this.trk_FanSpeed1);
            this.grp_FanSpeed1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(233)))), ((int)(((byte)(243)))));
            this.grp_FanSpeed1.Location = new System.Drawing.Point(78, 186);
            this.grp_FanSpeed1.Name = "grp_FanSpeed1";
            this.grp_FanSpeed1.Size = new System.Drawing.Size(155, 385);
            this.grp_FanSpeed1.TabIndex = 72;
            this.grp_FanSpeed1.TabStop = false;
            this.grp_FanSpeed1.Text = "Fan Speed";
            // 
            // lbl_FanUnits
            // 
            this.lbl_FanUnits.AutoSize = true;
            this.lbl_FanUnits.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_FanUnits.Location = new System.Drawing.Point(24, 106);
            this.lbl_FanUnits.Name = "lbl_FanUnits";
            this.lbl_FanUnits.Size = new System.Drawing.Size(75, 16);
            this.lbl_FanUnits.TabIndex = 76;
            this.lbl_FanUnits.Text = "°C        RPM";
            // 
            // btn_FanConfig
            // 
            this.btn_FanConfig.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.btn_FanConfig.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btn_FanConfig.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btn_FanConfig.Location = new System.Drawing.Point(9, 25);
            this.btn_FanConfig.Name = "btn_FanConfig";
            this.btn_FanConfig.Size = new System.Drawing.Size(136, 37);
            this.btn_FanConfig.TabIndex = 75;
            this.btn_FanConfig.Text = "Configure";
            this.btn_FanConfig.UseVisualStyleBackColor = false;
            // 
            // lbl_FanControls
            // 
            this.lbl_FanControls.AutoSize = true;
            this.lbl_FanControls.Location = new System.Drawing.Point(4, 80);
            this.lbl_FanControls.Name = "lbl_FanControls";
            this.lbl_FanControls.Size = new System.Drawing.Size(146, 20);
            this.lbl_FanControls.TabIndex = 70;
            this.lbl_FanControls.Text = "Temp / Fan / Adjust";
            // 
            // pnl_FanMarker
            // 
            this.pnl_FanMarker.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnl_FanMarker.Location = new System.Drawing.Point(58, 125);
            this.pnl_FanMarker.Name = "pnl_FanMarker";
            this.pnl_FanMarker.Size = new System.Drawing.Size(39, 245);
            this.pnl_FanMarker.TabIndex = 56;
            // 
            // lbl_LightingSpeed
            // 
            this.lbl_LightingSpeed.AutoSize = true;
            this.lbl_LightingSpeed.Location = new System.Drawing.Point(101, 128);
            this.lbl_LightingSpeed.Name = "lbl_LightingSpeed";
            this.lbl_LightingSpeed.Size = new System.Drawing.Size(116, 20);
            this.lbl_LightingSpeed.TabIndex = 74;
            this.lbl_LightingSpeed.Text = "Lighting Speed";
            // 
            // cbo_LightSpeed
            // 
            this.cbo_LightSpeed.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.cbo_LightSpeed.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.cbo_LightSpeed.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(233)))), ((int)(((byte)(243)))));
            this.cbo_LightSpeed.FormattingEnabled = true;
            this.cbo_LightSpeed.Location = new System.Drawing.Point(78, 151);
            this.cbo_LightSpeed.MaxDropDownItems = 30;
            this.cbo_LightSpeed.Name = "cbo_LightSpeed";
            this.cbo_LightSpeed.Size = new System.Drawing.Size(155, 28);
            this.cbo_LightSpeed.TabIndex = 73;
            // 
            // lbl_LightingMode
            // 
            this.lbl_LightingMode.AutoSize = true;
            this.lbl_LightingMode.Location = new System.Drawing.Point(101, 70);
            this.lbl_LightingMode.Name = "lbl_LightingMode";
            this.lbl_LightingMode.Size = new System.Drawing.Size(109, 20);
            this.lbl_LightingMode.TabIndex = 72;
            this.lbl_LightingMode.Text = "Lighting Mode";
            // 
            // btn_SaveConfig
            // 
            this.btn_SaveConfig.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.btn_SaveConfig.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btn_SaveConfig.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_SaveConfig.Location = new System.Drawing.Point(1173, 39);
            this.btn_SaveConfig.Name = "btn_SaveConfig";
            this.btn_SaveConfig.Size = new System.Drawing.Size(146, 37);
            this.btn_SaveConfig.TabIndex = 75;
            this.btn_SaveConfig.Text = "Save Configuration";
            this.btn_SaveConfig.UseVisualStyleBackColor = false;
            this.btn_SaveConfig.Click += new System.EventHandler(this.btn_SaveConfig_Click);
            // 
            // cbo_DeviceAddress
            // 
            this.cbo_DeviceAddress.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.cbo_DeviceAddress.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbo_DeviceAddress.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.cbo_DeviceAddress.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbo_DeviceAddress.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(233)))), ((int)(((byte)(243)))));
            this.cbo_DeviceAddress.FormattingEnabled = true;
            this.cbo_DeviceAddress.Location = new System.Drawing.Point(29, 68);
            this.cbo_DeviceAddress.MaxDropDownItems = 30;
            this.cbo_DeviceAddress.Name = "cbo_DeviceAddress";
            this.cbo_DeviceAddress.Size = new System.Drawing.Size(322, 28);
            this.cbo_DeviceAddress.TabIndex = 77;
            this.cbo_DeviceAddress.SelectedIndexChanged += new System.EventHandler(this.cbo_DeviceAddress_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(25, 39);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(126, 20);
            this.label1.TabIndex = 78;
            this.label1.Text = "Select Controller";
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.ContextMenuStrip = this.notifiyIconContextMenu;
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "Archon Lighting Controller";
            this.notifyIcon1.Visible = true;
            this.notifyIcon1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon1_MouseDoubleClick);
            // 
            // notifiyIconContextMenu
            // 
            this.notifiyIconContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.closeToolStripMenuItem1});
            this.notifiyIconContextMenu.Name = "notifiyIconContextMenu";
            this.notifiyIconContextMenu.Size = new System.Drawing.Size(104, 26);
            // 
            // closeToolStripMenuItem1
            // 
            this.closeToolStripMenuItem1.Name = "closeToolStripMenuItem1";
            this.closeToolStripMenuItem1.Size = new System.Drawing.Size(103, 22);
            this.closeToolStripMenuItem1.Text = "Close";
            this.closeToolStripMenuItem1.Click += new System.EventHandler(this.closeToolStripMenuItem1_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(535, 39);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(123, 20);
            this.label2.TabIndex = 79;
            this.label2.Text = "Controller Name";
            // 
            // txt_ControllerName
            // 
            this.txt_ControllerName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.txt_ControllerName.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_ControllerName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(233)))), ((int)(((byte)(243)))));
            this.txt_ControllerName.Location = new System.Drawing.Point(539, 68);
            this.txt_ControllerName.Name = "txt_ControllerName";
            this.txt_ControllerName.Size = new System.Drawing.Size(365, 26);
            this.txt_ControllerName.TabIndex = 80;
            this.txt_ControllerName.Leave += new System.EventHandler(this.txt_ControllerName_Leave);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(372, 39);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(140, 20);
            this.label3.TabIndex = 81;
            this.label3.Text = "Controller Enabled";
            // 
            // chk_Enabled
            // 
            this.chk_Enabled.AutoSize = true;
            this.chk_Enabled.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chk_Enabled.Location = new System.Drawing.Point(427, 75);
            this.chk_Enabled.Name = "chk_Enabled";
            this.chk_Enabled.Size = new System.Drawing.Size(15, 14);
            this.chk_Enabled.TabIndex = 82;
            this.chk_Enabled.UseVisualStyleBackColor = true;
            // 
            // AppForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.ClientSize = new System.Drawing.Size(1331, 736);
            this.Controls.Add(this.chk_Enabled);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txt_ControllerName);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cbo_DeviceAddress);
            this.Controls.Add(this.btn_SaveConfig);
            this.Controls.Add(this.grp_Device1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(233)))), ((int)(((byte)(243)))));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "AppForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Archon Lighting System";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.AppForm_FormClosed);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trk_FanSpeed1)).EndInit();
            this.grp_Device1.ResumeLayout(false);
            this.grp_Device1.PerformLayout();
            this.grp_FanSpeed1.ResumeLayout(false);
            this.grp_FanSpeed1.PerformLayout();
            this.notifiyIconContextMenu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Timer FormUpdateTimer;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editConfigToolStripMenuItem;
        private System.Windows.Forms.Panel pnl_TemperatureMarker;
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
        private System.Windows.Forms.ToolStripMenuItem editEEPROMToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem debugToolStripMenuItem;
        private System.Windows.Forms.Button btn_SaveConfig;
        private System.Windows.Forms.ToolStripMenuItem updateFirmwareToolStripMenuItem;
        private System.Windows.Forms.ComboBox cbo_DeviceAddress;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.ContextMenuStrip notifiyIconContextMenu;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem1;
        private System.Windows.Forms.Panel pnl_FanMarker;
        private System.Windows.Forms.Button btn_FanConfig;
        private System.Windows.Forms.Label lbl_FanControls;
        private System.Windows.Forms.Label lbl_FanUnits;
        private System.Windows.Forms.TextBox txt_Name;
        private System.Windows.Forms.Label lbl_Name;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txt_ControllerName;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox chk_Enabled;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem startWithWindowsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sequencerToolStripMenuItem;
    }
}

