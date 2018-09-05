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
            this.btn_readDebug = new System.Windows.Forms.Button();
            this.ANxVoltage_lbl = new System.Windows.Forms.Label();
            this.FormUpdateTimer = new System.Windows.Forms.Timer(this.components);
            this.ANxVoltageToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.ToggleLEDToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.PushbuttonStateTooltip = new System.Windows.Forms.ToolTip(this.components);
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.toolTip2 = new System.Windows.Forms.ToolTip(this.components);
            this.lbl_fanSpeed = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txt_Data_1 = new System.Windows.Forms.TextBox();
            this.btn_Read = new System.Windows.Forms.Button();
            this.btn_Write = new System.Windows.Forms.Button();
            this.txt_Data_2 = new System.Windows.Forms.TextBox();
            this.txt_Data_3 = new System.Windows.Forms.TextBox();
            this.txt_Data_4 = new System.Windows.Forms.TextBox();
            this.txt_Data_5 = new System.Windows.Forms.TextBox();
            this.txt_Data_6 = new System.Windows.Forms.TextBox();
            this.txt_Data_7 = new System.Windows.Forms.TextBox();
            this.txt_Data_8 = new System.Windows.Forms.TextBox();
            this.num_Addr = new System.Windows.Forms.NumericUpDown();
            this.num_Len = new System.Windows.Forms.NumericUpDown();
            this.txt_Data_16 = new System.Windows.Forms.TextBox();
            this.txt_Data_15 = new System.Windows.Forms.TextBox();
            this.txt_Data_14 = new System.Windows.Forms.TextBox();
            this.txt_Data_13 = new System.Windows.Forms.TextBox();
            this.txt_Data_12 = new System.Windows.Forms.TextBox();
            this.txt_Data_11 = new System.Windows.Forms.TextBox();
            this.txt_Data_10 = new System.Windows.Forms.TextBox();
            this.txt_Data_9 = new System.Windows.Forms.TextBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editConfigToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fanPanel1 = new System.Windows.Forms.Panel();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            this.button1 = new System.Windows.Forms.Button();
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
            this.trackBar1 = new System.Windows.Forms.TrackBar();
            this.cbo_LightingMode = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.comboBox2 = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.lbl_Address = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.num_Addr)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_Len)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // btn_readDebug
            // 
            this.btn_readDebug.Enabled = false;
            this.btn_readDebug.Location = new System.Drawing.Point(840, 383);
            this.btn_readDebug.Name = "btn_readDebug";
            this.btn_readDebug.Size = new System.Drawing.Size(96, 24);
            this.btn_readDebug.TabIndex = 24;
            this.btn_readDebug.Text = "Read Debug";
            this.btn_readDebug.UseVisualStyleBackColor = true;
            this.btn_readDebug.Click += new System.EventHandler(this.ToggleLEDs_btn_Click);
            // 
            // ANxVoltage_lbl
            // 
            this.ANxVoltage_lbl.AutoSize = true;
            this.ANxVoltage_lbl.Enabled = false;
            this.ANxVoltage_lbl.Location = new System.Drawing.Point(751, 254);
            this.ANxVoltage_lbl.Name = "ANxVoltage_lbl";
            this.ANxVoltage_lbl.Size = new System.Drawing.Size(62, 13);
            this.ANxVoltage_lbl.TabIndex = 23;
            this.ANxVoltage_lbl.Text = "Fan Speed:";
            // 
            // FormUpdateTimer
            // 
            this.FormUpdateTimer.Enabled = true;
            this.FormUpdateTimer.Interval = 6;
            this.FormUpdateTimer.Tick += new System.EventHandler(this.FormUpdateTimer_Tick);
            // 
            // ANxVoltageToolTip
            // 
            this.ANxVoltageToolTip.AutomaticDelay = 20;
            this.ANxVoltageToolTip.AutoPopDelay = 20000;
            this.ANxVoltageToolTip.InitialDelay = 15;
            this.ANxVoltageToolTip.ReshowDelay = 15;
            // 
            // ToggleLEDToolTip
            // 
            this.ToggleLEDToolTip.AutomaticDelay = 2000;
            this.ToggleLEDToolTip.AutoPopDelay = 20000;
            this.ToggleLEDToolTip.InitialDelay = 15;
            this.ToggleLEDToolTip.ReshowDelay = 15;
            // 
            // PushbuttonStateTooltip
            // 
            this.PushbuttonStateTooltip.AutomaticDelay = 20;
            this.PushbuttonStateTooltip.AutoPopDelay = 20000;
            this.PushbuttonStateTooltip.InitialDelay = 15;
            this.PushbuttonStateTooltip.ReshowDelay = 15;
            // 
            // toolTip1
            // 
            this.toolTip1.AutomaticDelay = 2000;
            this.toolTip1.AutoPopDelay = 20000;
            this.toolTip1.InitialDelay = 15;
            this.toolTip1.ReshowDelay = 15;
            // 
            // toolTip2
            // 
            this.toolTip2.AutomaticDelay = 20;
            this.toolTip2.AutoPopDelay = 20000;
            this.toolTip2.InitialDelay = 15;
            this.toolTip2.ReshowDelay = 15;
            // 
            // lbl_fanSpeed
            // 
            this.lbl_fanSpeed.AutoSize = true;
            this.lbl_fanSpeed.Location = new System.Drawing.Point(820, 254);
            this.lbl_fanSpeed.Name = "lbl_fanSpeed";
            this.lbl_fanSpeed.Size = new System.Drawing.Size(13, 13);
            this.lbl_fanSpeed.TabIndex = 26;
            this.lbl_fanSpeed.Text = "0";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(569, 438);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 13);
            this.label1.TabIndex = 29;
            this.label1.Text = "Addr";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(641, 438);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(25, 13);
            this.label2.TabIndex = 30;
            this.label2.Text = "Len";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(569, 491);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(30, 13);
            this.label3.TabIndex = 32;
            this.label3.Text = "Data";
            // 
            // txt_Data_1
            // 
            this.txt_Data_1.Location = new System.Drawing.Point(566, 507);
            this.txt_Data_1.Name = "txt_Data_1";
            this.txt_Data_1.Size = new System.Drawing.Size(41, 20);
            this.txt_Data_1.TabIndex = 31;
            // 
            // btn_Read
            // 
            this.btn_Read.Location = new System.Drawing.Point(716, 453);
            this.btn_Read.Name = "btn_Read";
            this.btn_Read.Size = new System.Drawing.Size(61, 24);
            this.btn_Read.TabIndex = 33;
            this.btn_Read.Text = "Read";
            this.btn_Read.UseVisualStyleBackColor = true;
            this.btn_Read.Click += new System.EventHandler(this.btn_Read_Click);
            // 
            // btn_Write
            // 
            this.btn_Write.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_Write.ForeColor = System.Drawing.Color.OrangeRed;
            this.btn_Write.Location = new System.Drawing.Point(789, 453);
            this.btn_Write.Name = "btn_Write";
            this.btn_Write.Size = new System.Drawing.Size(53, 24);
            this.btn_Write.TabIndex = 34;
            this.btn_Write.Text = "Write";
            this.btn_Write.UseVisualStyleBackColor = true;
            this.btn_Write.Click += new System.EventHandler(this.btn_Write_Click);
            // 
            // txt_Data_2
            // 
            this.txt_Data_2.Enabled = false;
            this.txt_Data_2.Location = new System.Drawing.Point(613, 507);
            this.txt_Data_2.Name = "txt_Data_2";
            this.txt_Data_2.Size = new System.Drawing.Size(41, 20);
            this.txt_Data_2.TabIndex = 35;
            // 
            // txt_Data_3
            // 
            this.txt_Data_3.Enabled = false;
            this.txt_Data_3.Location = new System.Drawing.Point(660, 507);
            this.txt_Data_3.Name = "txt_Data_3";
            this.txt_Data_3.Size = new System.Drawing.Size(41, 20);
            this.txt_Data_3.TabIndex = 36;
            // 
            // txt_Data_4
            // 
            this.txt_Data_4.Enabled = false;
            this.txt_Data_4.Location = new System.Drawing.Point(707, 507);
            this.txt_Data_4.Name = "txt_Data_4";
            this.txt_Data_4.Size = new System.Drawing.Size(41, 20);
            this.txt_Data_4.TabIndex = 37;
            // 
            // txt_Data_5
            // 
            this.txt_Data_5.Enabled = false;
            this.txt_Data_5.Location = new System.Drawing.Point(754, 507);
            this.txt_Data_5.Name = "txt_Data_5";
            this.txt_Data_5.Size = new System.Drawing.Size(41, 20);
            this.txt_Data_5.TabIndex = 38;
            // 
            // txt_Data_6
            // 
            this.txt_Data_6.Enabled = false;
            this.txt_Data_6.Location = new System.Drawing.Point(801, 507);
            this.txt_Data_6.Name = "txt_Data_6";
            this.txt_Data_6.Size = new System.Drawing.Size(41, 20);
            this.txt_Data_6.TabIndex = 39;
            // 
            // txt_Data_7
            // 
            this.txt_Data_7.Enabled = false;
            this.txt_Data_7.Location = new System.Drawing.Point(848, 507);
            this.txt_Data_7.Name = "txt_Data_7";
            this.txt_Data_7.Size = new System.Drawing.Size(41, 20);
            this.txt_Data_7.TabIndex = 40;
            // 
            // txt_Data_8
            // 
            this.txt_Data_8.Enabled = false;
            this.txt_Data_8.Location = new System.Drawing.Point(895, 507);
            this.txt_Data_8.Name = "txt_Data_8";
            this.txt_Data_8.Size = new System.Drawing.Size(41, 20);
            this.txt_Data_8.TabIndex = 41;
            // 
            // num_Addr
            // 
            this.num_Addr.Increment = new decimal(new int[] {
            8,
            0,
            0,
            0});
            this.num_Addr.Location = new System.Drawing.Point(566, 456);
            this.num_Addr.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.num_Addr.Name = "num_Addr";
            this.num_Addr.Size = new System.Drawing.Size(62, 20);
            this.num_Addr.TabIndex = 42;
            // 
            // num_Len
            // 
            this.num_Len.Location = new System.Drawing.Point(644, 456);
            this.num_Len.Maximum = new decimal(new int[] {
            16,
            0,
            0,
            0});
            this.num_Len.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.num_Len.Name = "num_Len";
            this.num_Len.Size = new System.Drawing.Size(62, 20);
            this.num_Len.TabIndex = 43;
            this.num_Len.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.num_Len.ValueChanged += new System.EventHandler(this.numericUpDown2_ValueChanged);
            // 
            // txt_Data_16
            // 
            this.txt_Data_16.Enabled = false;
            this.txt_Data_16.Location = new System.Drawing.Point(895, 533);
            this.txt_Data_16.Name = "txt_Data_16";
            this.txt_Data_16.Size = new System.Drawing.Size(41, 20);
            this.txt_Data_16.TabIndex = 52;
            // 
            // txt_Data_15
            // 
            this.txt_Data_15.Enabled = false;
            this.txt_Data_15.Location = new System.Drawing.Point(848, 533);
            this.txt_Data_15.Name = "txt_Data_15";
            this.txt_Data_15.Size = new System.Drawing.Size(41, 20);
            this.txt_Data_15.TabIndex = 51;
            // 
            // txt_Data_14
            // 
            this.txt_Data_14.Enabled = false;
            this.txt_Data_14.Location = new System.Drawing.Point(801, 533);
            this.txt_Data_14.Name = "txt_Data_14";
            this.txt_Data_14.Size = new System.Drawing.Size(41, 20);
            this.txt_Data_14.TabIndex = 50;
            // 
            // txt_Data_13
            // 
            this.txt_Data_13.Enabled = false;
            this.txt_Data_13.Location = new System.Drawing.Point(754, 533);
            this.txt_Data_13.Name = "txt_Data_13";
            this.txt_Data_13.Size = new System.Drawing.Size(41, 20);
            this.txt_Data_13.TabIndex = 49;
            // 
            // txt_Data_12
            // 
            this.txt_Data_12.Enabled = false;
            this.txt_Data_12.Location = new System.Drawing.Point(707, 533);
            this.txt_Data_12.Name = "txt_Data_12";
            this.txt_Data_12.Size = new System.Drawing.Size(41, 20);
            this.txt_Data_12.TabIndex = 48;
            // 
            // txt_Data_11
            // 
            this.txt_Data_11.Enabled = false;
            this.txt_Data_11.Location = new System.Drawing.Point(660, 533);
            this.txt_Data_11.Name = "txt_Data_11";
            this.txt_Data_11.Size = new System.Drawing.Size(41, 20);
            this.txt_Data_11.TabIndex = 47;
            // 
            // txt_Data_10
            // 
            this.txt_Data_10.Enabled = false;
            this.txt_Data_10.Location = new System.Drawing.Point(613, 533);
            this.txt_Data_10.Name = "txt_Data_10";
            this.txt_Data_10.Size = new System.Drawing.Size(41, 20);
            this.txt_Data_10.TabIndex = 46;
            // 
            // txt_Data_9
            // 
            this.txt_Data_9.Enabled = false;
            this.txt_Data_9.Location = new System.Drawing.Point(566, 533);
            this.txt_Data_9.Name = "txt_Data_9";
            this.txt_Data_9.Size = new System.Drawing.Size(41, 20);
            this.txt_Data_9.TabIndex = 45;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.optionsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(965, 24);
            this.menuStrip1.TabIndex = 54;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.editConfigToolStripMenuItem});
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.optionsToolStripMenuItem.Text = "Options";
            // 
            // editConfigToolStripMenuItem
            // 
            this.editConfigToolStripMenuItem.Name = "editConfigToolStripMenuItem";
            this.editConfigToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
            this.editConfigToolStripMenuItem.Text = "Edit Config";
            this.editConfigToolStripMenuItem.Click += new System.EventHandler(this.editConfigToolStripMenuItem_Click);
            // 
            // fanPanel1
            // 
            this.fanPanel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.fanPanel1.Location = new System.Drawing.Point(6, 59);
            this.fanPanel1.Name = "fanPanel1";
            this.fanPanel1.Size = new System.Drawing.Size(45, 290);
            this.fanPanel1.TabIndex = 55;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusLabel});
            this.statusStrip1.Location = new System.Drawing.Point(0, 616);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(965, 22);
            this.statusStrip1.TabIndex = 56;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // statusLabel
            // 
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(59, 17);
            this.statusLabel.Text = "Loading...";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(16, 33);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(36, 37);
            this.button1.TabIndex = 57;
            this.button1.Text = "L1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(16, 75);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(36, 37);
            this.button2.TabIndex = 58;
            this.button2.Text = "L2";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(16, 117);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(36, 37);
            this.button3.TabIndex = 59;
            this.button3.Text = "L3";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(16, 159);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(36, 37);
            this.button4.TabIndex = 60;
            this.button4.Text = "L4";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
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
            // trackBar1
            // 
            this.trackBar1.LargeChange = 10;
            this.trackBar1.Location = new System.Drawing.Point(74, 59);
            this.trackBar1.Maximum = 100;
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.trackBar1.Size = new System.Drawing.Size(45, 290);
            this.trackBar1.SmallChange = 5;
            this.trackBar1.TabIndex = 69;
            this.trackBar1.TickFrequency = 10;
            this.trackBar1.Scroll += new System.EventHandler(this.trackBar1_Scroll);
            // 
            // cbo_LightingMode
            // 
            this.cbo_LightingMode.FormattingEnabled = true;
            this.cbo_LightingMode.Location = new System.Drawing.Point(70, 87);
            this.cbo_LightingMode.Name = "cbo_LightingMode";
            this.cbo_LightingMode.Size = new System.Drawing.Size(121, 28);
            this.cbo_LightingMode.TabIndex = 70;
            this.cbo_LightingMode.SelectedIndexChanged += new System.EventHandler(this.cbo_LightingMode_SelectedIndexChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lbl_Address);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.groupBox2);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.comboBox2);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.button1);
            this.groupBox1.Controls.Add(this.cbo_LightingMode);
            this.groupBox1.Controls.Add(this.button2);
            this.groupBox1.Controls.Add(this.button9);
            this.groupBox1.Controls.Add(this.button3);
            this.groupBox1.Controls.Add(this.button10);
            this.groupBox1.Controls.Add(this.button4);
            this.groupBox1.Controls.Add(this.button11);
            this.groupBox1.Controls.Add(this.button8);
            this.groupBox1.Controls.Add(this.button12);
            this.groupBox1.Controls.Add(this.button7);
            this.groupBox1.Controls.Add(this.button5);
            this.groupBox1.Controls.Add(this.button6);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(29, 27);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(251, 554);
            this.groupBox1.TabIndex = 71;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Device 1";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.fanPanel1);
            this.groupBox2.Controls.Add(this.trackBar1);
            this.groupBox2.Location = new System.Drawing.Point(78, 193);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(120, 355);
            this.groupBox2.TabIndex = 72;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Fan Speed";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(74, 132);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(116, 20);
            this.label5.TabIndex = 74;
            this.label5.Text = "Lighting Speed";
            // 
            // comboBox2
            // 
            this.comboBox2.FormattingEnabled = true;
            this.comboBox2.Location = new System.Drawing.Point(70, 155);
            this.comboBox2.Name = "comboBox2";
            this.comboBox2.Size = new System.Drawing.Size(121, 28);
            this.comboBox2.TabIndex = 73;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(74, 64);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(109, 20);
            this.label4.TabIndex = 72;
            this.label4.Text = "Lighting Mode";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(74, 33);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(72, 20);
            this.label6.TabIndex = 75;
            this.label6.Text = "Address:";
            // 
            // lbl_Address
            // 
            this.lbl_Address.AutoSize = true;
            this.lbl_Address.Location = new System.Drawing.Point(152, 33);
            this.lbl_Address.Name = "lbl_Address";
            this.lbl_Address.Size = new System.Drawing.Size(19, 20);
            this.lbl_Address.TabIndex = 76;
            this.lbl_Address.Text = "--";
            // 
            // AppForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(965, 638);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.txt_Data_16);
            this.Controls.Add(this.txt_Data_15);
            this.Controls.Add(this.txt_Data_14);
            this.Controls.Add(this.txt_Data_13);
            this.Controls.Add(this.txt_Data_12);
            this.Controls.Add(this.txt_Data_11);
            this.Controls.Add(this.txt_Data_10);
            this.Controls.Add(this.txt_Data_9);
            this.Controls.Add(this.num_Len);
            this.Controls.Add(this.num_Addr);
            this.Controls.Add(this.txt_Data_8);
            this.Controls.Add(this.txt_Data_7);
            this.Controls.Add(this.txt_Data_6);
            this.Controls.Add(this.txt_Data_5);
            this.Controls.Add(this.txt_Data_4);
            this.Controls.Add(this.txt_Data_3);
            this.Controls.Add(this.txt_Data_2);
            this.Controls.Add(this.btn_Write);
            this.Controls.Add(this.btn_Read);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txt_Data_1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lbl_fanSpeed);
            this.Controls.Add(this.btn_readDebug);
            this.Controls.Add(this.ANxVoltage_lbl);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "AppForm";
            this.Text = "Archon Lighting System";
            ((System.ComponentModel.ISupportInitialize)(this.num_Addr)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_Len)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btn_readDebug;
        private System.Windows.Forms.Label ANxVoltage_lbl;
        private System.Windows.Forms.Timer FormUpdateTimer;
        private System.Windows.Forms.ToolTip ANxVoltageToolTip;
        private System.Windows.Forms.ToolTip ToggleLEDToolTip;
        private System.Windows.Forms.ToolTip PushbuttonStateTooltip;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ToolTip toolTip2;
        private System.Windows.Forms.Label lbl_fanSpeed;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txt_Data_1;
        private System.Windows.Forms.Button btn_Read;
        private System.Windows.Forms.Button btn_Write;
        private System.Windows.Forms.TextBox txt_Data_2;
        private System.Windows.Forms.TextBox txt_Data_3;
        private System.Windows.Forms.TextBox txt_Data_4;
        private System.Windows.Forms.TextBox txt_Data_5;
        private System.Windows.Forms.TextBox txt_Data_6;
        private System.Windows.Forms.TextBox txt_Data_7;
        private System.Windows.Forms.TextBox txt_Data_8;
        private System.Windows.Forms.NumericUpDown num_Addr;
        private System.Windows.Forms.NumericUpDown num_Len;
        private System.Windows.Forms.TextBox txt_Data_16;
        private System.Windows.Forms.TextBox txt_Data_15;
        private System.Windows.Forms.TextBox txt_Data_14;
        private System.Windows.Forms.TextBox txt_Data_13;
        private System.Windows.Forms.TextBox txt_Data_12;
        private System.Windows.Forms.TextBox txt_Data_11;
        private System.Windows.Forms.TextBox txt_Data_10;
        private System.Windows.Forms.TextBox txt_Data_9;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editConfigToolStripMenuItem;
        private System.Windows.Forms.Panel fanPanel1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel statusLabel;
        private System.Windows.Forms.ColorDialog colorDialog1;
        private System.Windows.Forms.Button button1;
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
        private System.Windows.Forms.TrackBar trackBar1;
        private System.Windows.Forms.ComboBox cbo_LightingMode;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox comboBox2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lbl_Address;
        private System.Windows.Forms.Label label6;
    }
}

