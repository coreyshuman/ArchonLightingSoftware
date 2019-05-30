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
            this.grpSeq.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numBpm)).BeginInit();
            this.grpLights.SuspendLayout();
            this.grpOut.SuspendLayout();
            this.SuspendLayout();
            // 
            // tick_1
            // 
            this.tick_1.Location = new System.Drawing.Point(6, 19);
            this.tick_1.Name = "tick_1";
            this.tick_1.Size = new System.Drawing.Size(40, 40);
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
            this.grpSeq.Controls.Add(this.label1);
            this.grpSeq.Controls.Add(this.btnPlay);
            this.grpSeq.Controls.Add(this.numBpm);
            this.grpSeq.Controls.Add(this.tick_1);
            this.grpSeq.Location = new System.Drawing.Point(12, 578);
            this.grpSeq.Name = "grpSeq";
            this.grpSeq.Size = new System.Drawing.Size(776, 109);
            this.grpSeq.TabIndex = 16;
            this.grpSeq.TabStop = false;
            this.grpSeq.Text = "Sequence Step";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(96, 81);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(30, 13);
            this.label1.TabIndex = 15;
            this.label1.Text = "BPM";
            // 
            // btnPlay
            // 
            this.btnPlay.Location = new System.Drawing.Point(6, 77);
            this.btnPlay.Name = "btnPlay";
            this.btnPlay.Size = new System.Drawing.Size(75, 23);
            this.btnPlay.TabIndex = 14;
            this.btnPlay.Text = "Pause";
            this.btnPlay.UseVisualStyleBackColor = true;
            this.btnPlay.Click += new System.EventHandler(this.btnPlay_Click);
            // 
            // numBpm
            // 
            this.numBpm.Location = new System.Drawing.Point(132, 78);
            this.numBpm.Maximum = new decimal(new int[] {
            300,
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
            this.grpLights.Location = new System.Drawing.Point(18, 8);
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
            this.grpOut.Location = new System.Drawing.Point(794, 12);
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
            // SequencerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(876, 700);
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
    }
}