namespace ArchonLightingSystem.Forms
{
    partial class StatsConfigForm
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
            this.cbo_Screens = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // cbo_Screens
            // 
            this.cbo_Screens.FormattingEnabled = true;
            this.cbo_Screens.Location = new System.Drawing.Point(286, 72);
            this.cbo_Screens.Name = "cbo_Screens";
            this.cbo_Screens.Size = new System.Drawing.Size(208, 28);
            this.cbo_Screens.TabIndex = 0;
            this.cbo_Screens.SelectedIndexChanged += new System.EventHandler(this.cbo_Screens_SelectedIndexChanged);
            // 
            // StatsConfigForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.cbo_Screens);
            this.Name = "StatsConfigForm";
            this.Text = "StatsConfigForm";
            this.TopMost = true;
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox cbo_Screens;
    }
}