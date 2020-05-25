namespace ArchonLightingSystem.Forms
{
    partial class SequencerForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SequencerForm));
            this.tick_1 = new System.Windows.Forms.Button();
            this.light_1_1 = new System.Windows.Forms.Button();
            this.grpSeq = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnPlay = new System.Windows.Forms.Button();
            this.numBpm = new System.Windows.Forms.NumericUpDown();
            this.grpLights = new System.Windows.Forms.GroupBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.grpOut = new System.Windows.Forms.GroupBox();
            this.out_1 = new System.Windows.Forms.Button();
            this.cboController = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblName = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.cboDevice = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.numSteps = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.grpSeq.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numBpm)).BeginInit();
            this.grpLights.SuspendLayout();
            this.grpOut.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numSteps)).BeginInit();
            this.SuspendLayout();
            // 
            // tick_1
            // 
            this.tick_1.Location = new System.Drawing.Point(6, 19);
            this.tick_1.Name = "tick_1";
            this.tick_1.Size = new System.Drawing.Size(40, 14);
            this.tick_1.TabIndex = 0;
            this.tick_1.UseVisualStyleBackColor = true;
            // 
            // light_1_1
            // 
            this.light_1_1.Location = new System.Drawing.Point(6, 19);
            this.light_1_1.Name = "light_1_1";
            this.light_1_1.Size = new System.Drawing.Size(40, 40);
            this.light_1_1.TabIndex = 13;
            this.light_1_1.UseVisualStyleBackColor = true;
            // 
            // grpSeq
            // 
            this.grpSeq.Controls.Add(this.label5);
            this.grpSeq.Controls.Add(this.numSteps);
            this.grpSeq.Controls.Add(this.label1);
            this.grpSeq.Controls.Add(this.btnPlay);
            this.grpSeq.Controls.Add(this.numBpm);
            this.grpSeq.Controls.Add(this.tick_1);
            this.grpSeq.Location = new System.Drawing.Point(12, 628);
            this.grpSeq.Name = "grpSeq";
            this.grpSeq.Size = new System.Drawing.Size(776, 83);
            this.grpSeq.TabIndex = 16;
            this.grpSeq.TabStop = false;
            this.grpSeq.Text = "Sequence Step";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(96, 56);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(30, 13);
            this.label1.TabIndex = 15;
            this.label1.Text = "BPM";
            // 
            // btnPlay
            // 
            this.btnPlay.Location = new System.Drawing.Point(6, 52);
            this.btnPlay.Name = "btnPlay";
            this.btnPlay.Size = new System.Drawing.Size(75, 23);
            this.btnPlay.TabIndex = 14;
            this.btnPlay.Text = "Pause";
            this.btnPlay.UseVisualStyleBackColor = true;
            this.btnPlay.Click += new System.EventHandler(this.btnPlay_Click);
            // 
            // numBpm
            // 
            this.numBpm.Location = new System.Drawing.Point(132, 53);
            this.numBpm.Maximum = new decimal(new int[] {
            600,
            0,
            0,
            0});
            this.numBpm.Minimum = new decimal(new int[] {
            30,
            0,
            0,
            0});
            this.numBpm.Name = "numBpm";
            this.numBpm.Size = new System.Drawing.Size(68, 20);
            this.numBpm.TabIndex = 1;
            this.numBpm.Value = new decimal(new int[] {
            120,
            0,
            0,
            0});
            this.numBpm.ValueChanged += new System.EventHandler(this.numBpm_ValueChanged);
            // 
            // grpLights
            // 
            this.grpLights.Controls.Add(this.light_1_1);
            this.grpLights.Location = new System.Drawing.Point(12, 58);
            this.grpLights.Name = "grpLights";
            this.grpLights.Size = new System.Drawing.Size(770, 564);
            this.grpLights.TabIndex = 17;
            this.grpLights.TabStop = false;
            this.grpLights.Text = "Light Controls";
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // grpOut
            // 
            this.grpOut.Controls.Add(this.out_1);
            this.grpOut.Location = new System.Drawing.Point(788, 62);
            this.grpOut.Name = "grpOut";
            this.grpOut.Size = new System.Drawing.Size(72, 560);
            this.grpOut.TabIndex = 18;
            this.grpOut.TabStop = false;
            this.grpOut.Text = "Output";
            // 
            // out_1
            // 
            this.out_1.Location = new System.Drawing.Point(6, 19);
            this.out_1.Name = "out_1";
            this.out_1.Size = new System.Drawing.Size(40, 40);
            this.out_1.TabIndex = 14;
            this.out_1.UseVisualStyleBackColor = true;
            // 
            // cboController
            // 
            this.cboController.FormattingEnabled = true;
            this.cboController.Location = new System.Drawing.Point(73, 14);
            this.cboController.Name = "cboController";
            this.cboController.Size = new System.Drawing.Size(121, 21);
            this.cboController.TabIndex = 19;
            this.cboController.SelectedIndexChanged += new System.EventHandler(this.CboController_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 21);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(51, 13);
            this.label2.TabIndex = 20;
            this.label2.Text = "Controller";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lblName);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.cboDevice);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.cboController);
            this.groupBox1.Location = new System.Drawing.Point(18, 11);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(764, 41);
            this.groupBox1.TabIndex = 21;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Live Preview";
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Location = new System.Drawing.Point(444, 22);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(10, 13);
            this.lblName.TabIndex = 24;
            this.lblName.Text = "-";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(400, 22);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(38, 13);
            this.label4.TabIndex = 23;
            this.label4.Text = "Name:";
            // 
            // cboDevice
            // 
            this.cboDevice.FormattingEnabled = true;
            this.cboDevice.Location = new System.Drawing.Point(262, 14);
            this.cboDevice.Name = "cboDevice";
            this.cboDevice.Size = new System.Drawing.Size(121, 21);
            this.cboDevice.TabIndex = 22;
            this.cboDevice.SelectedIndexChanged += new System.EventHandler(this.CboDevice_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(215, 21);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 13);
            this.label3.TabIndex = 21;
            this.label3.Text = "Device";
            // 
            // numSteps
            // 
            this.numSteps.Location = new System.Drawing.Point(326, 52);
            this.numSteps.Maximum = new decimal(new int[] {
            16,
            0,
            0,
            0});
            this.numSteps.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.numSteps.Name = "numSteps";
            this.numSteps.Size = new System.Drawing.Size(63, 20);
            this.numSteps.TabIndex = 16;
            this.numSteps.Value = new decimal(new int[] {
            16,
            0,
            0,
            0});
            this.numSteps.ValueChanged += new System.EventHandler(this.NumSteps_ValueChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(233, 57);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(87, 13);
            this.label5.TabIndex = 17;
            this.label5.Text = "Sequence Count";
            // 
            // SequencerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(876, 723);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.grpOut);
            this.Controls.Add(this.grpLights);
            this.Controls.Add(this.grpSeq);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SequencerForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "SequencerForm";
            this.Load += new System.EventHandler(this.SequencerForm_Load);
            this.grpSeq.ResumeLayout(false);
            this.grpSeq.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numBpm)).EndInit();
            this.grpLights.ResumeLayout(false);
            this.grpOut.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numSteps)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button tick_1;
        private System.Windows.Forms.Button light_1_1;
        private System.Windows.Forms.GroupBox grpSeq;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnPlay;
        private System.Windows.Forms.NumericUpDown numBpm;
        private System.Windows.Forms.GroupBox grpLights;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.GroupBox grpOut;
        private System.Windows.Forms.Button out_1;
        private System.Windows.Forms.ComboBox cboController;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox cboDevice;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown numSteps;
    }
}