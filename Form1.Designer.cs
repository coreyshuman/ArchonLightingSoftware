namespace ArchonLightingSystem
{
    partial class Form1
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
            this.PushbuttonState_lbl = new System.Windows.Forms.Label();
            this.ToggleLEDs_btn = new System.Windows.Forms.Button();
            this.ANxVoltage_lbl = new System.Windows.Forms.Label();
            this.StatusBox_lbl = new System.Windows.Forms.Label();
            this.StatusBox_txtbx = new System.Windows.Forms.TextBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
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
            this.btn_ReadConfig = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.num_Addr)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_Len)).BeginInit();
            this.SuspendLayout();
            // 
            // PushbuttonState_lbl
            // 
            this.PushbuttonState_lbl.AutoSize = true;
            this.PushbuttonState_lbl.Enabled = false;
            this.PushbuttonState_lbl.Location = new System.Drawing.Point(131, 65);
            this.PushbuttonState_lbl.Name = "PushbuttonState_lbl";
            this.PushbuttonState_lbl.Size = new System.Drawing.Size(141, 13);
            this.PushbuttonState_lbl.TabIndex = 25;
            this.PushbuttonState_lbl.Text = "Pushbutton State: Unknown";
            // 
            // ToggleLEDs_btn
            // 
            this.ToggleLEDs_btn.Enabled = false;
            this.ToggleLEDs_btn.Location = new System.Drawing.Point(12, 60);
            this.ToggleLEDs_btn.Name = "ToggleLEDs_btn";
            this.ToggleLEDs_btn.Size = new System.Drawing.Size(96, 23);
            this.ToggleLEDs_btn.TabIndex = 24;
            this.ToggleLEDs_btn.Text = "ToggleLED(s)";
            this.ToggleLEDs_btn.UseVisualStyleBackColor = true;
            this.ToggleLEDs_btn.Click += new System.EventHandler(this.ToggleLEDs_btn_Click);
            // 
            // ANxVoltage_lbl
            // 
            this.ANxVoltage_lbl.AutoSize = true;
            this.ANxVoltage_lbl.Enabled = false;
            this.ANxVoltage_lbl.Location = new System.Drawing.Point(12, 105);
            this.ANxVoltage_lbl.Name = "ANxVoltage_lbl";
            this.ANxVoltage_lbl.Size = new System.Drawing.Size(62, 13);
            this.ANxVoltage_lbl.TabIndex = 23;
            this.ANxVoltage_lbl.Text = "Fan Speed:";
            // 
            // StatusBox_lbl
            // 
            this.StatusBox_lbl.AutoSize = true;
            this.StatusBox_lbl.Location = new System.Drawing.Point(316, 15);
            this.StatusBox_lbl.Name = "StatusBox_lbl";
            this.StatusBox_lbl.Size = new System.Drawing.Size(37, 13);
            this.StatusBox_lbl.TabIndex = 22;
            this.StatusBox_lbl.Text = "Status";
            // 
            // StatusBox_txtbx
            // 
            this.StatusBox_txtbx.BackColor = System.Drawing.SystemColors.Window;
            this.StatusBox_txtbx.Location = new System.Drawing.Point(12, 12);
            this.StatusBox_txtbx.Name = "StatusBox_txtbx";
            this.StatusBox_txtbx.ReadOnly = true;
            this.StatusBox_txtbx.Size = new System.Drawing.Size(298, 20);
            this.StatusBox_txtbx.TabIndex = 21;
            // 
            // progressBar1
            // 
            this.progressBar1.BackColor = System.Drawing.Color.White;
            this.progressBar1.ForeColor = System.Drawing.Color.Red;
            this.progressBar1.Location = new System.Drawing.Point(12, 121);
            this.progressBar1.Maximum = 4000;
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(298, 23);
            this.progressBar1.Step = 1;
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBar1.TabIndex = 20;
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
            this.lbl_fanSpeed.Location = new System.Drawing.Point(81, 105);
            this.lbl_fanSpeed.Name = "lbl_fanSpeed";
            this.lbl_fanSpeed.Size = new System.Drawing.Size(13, 13);
            this.lbl_fanSpeed.TabIndex = 26;
            this.lbl_fanSpeed.Text = "0";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 165);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 13);
            this.label1.TabIndex = 29;
            this.label1.Text = "Addr";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(87, 165);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(25, 13);
            this.label2.TabIndex = 30;
            this.label2.Text = "Len";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(15, 218);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(30, 13);
            this.label3.TabIndex = 32;
            this.label3.Text = "Data";
            // 
            // txt_Data_1
            // 
            this.txt_Data_1.Location = new System.Drawing.Point(12, 234);
            this.txt_Data_1.Name = "txt_Data_1";
            this.txt_Data_1.Size = new System.Drawing.Size(41, 20);
            this.txt_Data_1.TabIndex = 31;
            // 
            // btn_Read
            // 
            this.btn_Read.Location = new System.Drawing.Point(162, 180);
            this.btn_Read.Name = "btn_Read";
            this.btn_Read.Size = new System.Drawing.Size(61, 23);
            this.btn_Read.TabIndex = 33;
            this.btn_Read.Text = "Read";
            this.btn_Read.UseVisualStyleBackColor = true;
            this.btn_Read.Click += new System.EventHandler(this.btn_Read_Click);
            // 
            // btn_Write
            // 
            this.btn_Write.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_Write.ForeColor = System.Drawing.Color.OrangeRed;
            this.btn_Write.Location = new System.Drawing.Point(235, 180);
            this.btn_Write.Name = "btn_Write";
            this.btn_Write.Size = new System.Drawing.Size(53, 23);
            this.btn_Write.TabIndex = 34;
            this.btn_Write.Text = "Write";
            this.btn_Write.UseVisualStyleBackColor = true;
            this.btn_Write.Click += new System.EventHandler(this.btn_Write_Click);
            // 
            // txt_Data_2
            // 
            this.txt_Data_2.Enabled = false;
            this.txt_Data_2.Location = new System.Drawing.Point(59, 234);
            this.txt_Data_2.Name = "txt_Data_2";
            this.txt_Data_2.Size = new System.Drawing.Size(41, 20);
            this.txt_Data_2.TabIndex = 35;
            // 
            // txt_Data_3
            // 
            this.txt_Data_3.Enabled = false;
            this.txt_Data_3.Location = new System.Drawing.Point(106, 234);
            this.txt_Data_3.Name = "txt_Data_3";
            this.txt_Data_3.Size = new System.Drawing.Size(41, 20);
            this.txt_Data_3.TabIndex = 36;
            // 
            // txt_Data_4
            // 
            this.txt_Data_4.Enabled = false;
            this.txt_Data_4.Location = new System.Drawing.Point(153, 234);
            this.txt_Data_4.Name = "txt_Data_4";
            this.txt_Data_4.Size = new System.Drawing.Size(41, 20);
            this.txt_Data_4.TabIndex = 37;
            // 
            // txt_Data_5
            // 
            this.txt_Data_5.Enabled = false;
            this.txt_Data_5.Location = new System.Drawing.Point(200, 234);
            this.txt_Data_5.Name = "txt_Data_5";
            this.txt_Data_5.Size = new System.Drawing.Size(41, 20);
            this.txt_Data_5.TabIndex = 38;
            // 
            // txt_Data_6
            // 
            this.txt_Data_6.Enabled = false;
            this.txt_Data_6.Location = new System.Drawing.Point(247, 234);
            this.txt_Data_6.Name = "txt_Data_6";
            this.txt_Data_6.Size = new System.Drawing.Size(41, 20);
            this.txt_Data_6.TabIndex = 39;
            // 
            // txt_Data_7
            // 
            this.txt_Data_7.Enabled = false;
            this.txt_Data_7.Location = new System.Drawing.Point(294, 234);
            this.txt_Data_7.Name = "txt_Data_7";
            this.txt_Data_7.Size = new System.Drawing.Size(41, 20);
            this.txt_Data_7.TabIndex = 40;
            // 
            // txt_Data_8
            // 
            this.txt_Data_8.Enabled = false;
            this.txt_Data_8.Location = new System.Drawing.Point(341, 234);
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
            this.num_Addr.Location = new System.Drawing.Point(12, 183);
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
            this.num_Len.Location = new System.Drawing.Point(90, 183);
            this.num_Len.Maximum = new decimal(new int[] {
            8,
            0,
            0,
            0});
            this.num_Len.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.num_Len.Name = "num_Len";
            this.num_Len.Size = new System.Drawing.Size(66, 20);
            this.num_Len.TabIndex = 43;
            this.num_Len.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.num_Len.ValueChanged += new System.EventHandler(this.numericUpDown2_ValueChanged);
            // 
            // btn_ReadConfig
            // 
            this.btn_ReadConfig.Location = new System.Drawing.Point(321, 183);
            this.btn_ReadConfig.Name = "btn_ReadConfig";
            this.btn_ReadConfig.Size = new System.Drawing.Size(61, 23);
            this.btn_ReadConfig.TabIndex = 44;
            this.btn_ReadConfig.Text = "Read Config";
            this.btn_ReadConfig.UseVisualStyleBackColor = true;
            this.btn_ReadConfig.Click += new System.EventHandler(this.btn_ReadConfig_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(408, 287);
            this.Controls.Add(this.btn_ReadConfig);
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
            this.Controls.Add(this.PushbuttonState_lbl);
            this.Controls.Add(this.ToggleLEDs_btn);
            this.Controls.Add(this.ANxVoltage_lbl);
            this.Controls.Add(this.StatusBox_lbl);
            this.Controls.Add(this.StatusBox_txtbx);
            this.Controls.Add(this.progressBar1);
            this.Name = "Form1";
            this.Text = "HID PnP Demo";
            ((System.ComponentModel.ISupportInitialize)(this.num_Addr)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_Len)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label PushbuttonState_lbl;
        private System.Windows.Forms.Button ToggleLEDs_btn;
        private System.Windows.Forms.Label ANxVoltage_lbl;
        private System.Windows.Forms.Label StatusBox_lbl;
        private System.Windows.Forms.TextBox StatusBox_txtbx;
        private System.Windows.Forms.ProgressBar progressBar1;
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
        private System.Windows.Forms.Button btn_ReadConfig;
    }
}

