namespace ArchonLightingSystem.Components
{
    partial class GaugeArc
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

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panel_Container = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // panel_Container
            // 
            this.panel_Container.BackColor = System.Drawing.Color.Bisque;
            this.panel_Container.Location = new System.Drawing.Point(0, 0);
            this.panel_Container.Name = "panel_Container";
            this.panel_Container.Size = new System.Drawing.Size(100, 100);
            this.panel_Container.TabIndex = 0;
            // 
            // GaugeArc
            // 
            this.Name = "GaugeArc";
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.GaugeArc_Paint);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel_Container;
    }
}
