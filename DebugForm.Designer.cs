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
            this.SuspendLayout();
            // 
            // btn_ReadDebug
            // 
            this.btn_ReadDebug.Location = new System.Drawing.Point(12, 12);
            this.btn_ReadDebug.Name = "btn_ReadDebug";
            this.btn_ReadDebug.Size = new System.Drawing.Size(82, 37);
            this.btn_ReadDebug.TabIndex = 0;
            this.btn_ReadDebug.Text = "Read Debug";
            this.btn_ReadDebug.UseVisualStyleBackColor = true;
            this.btn_ReadDebug.Click += new System.EventHandler(this.btn_ReadDebug_Click);
            // 
            // txt_Debug
            // 
            this.txt_Debug.Location = new System.Drawing.Point(1, 55);
            this.txt_Debug.Multiline = true;
            this.txt_Debug.Name = "txt_Debug";
            this.txt_Debug.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txt_Debug.Size = new System.Drawing.Size(735, 330);
            this.txt_Debug.TabIndex = 1;
            // 
            // updateFormTimer
            // 
            this.updateFormTimer.Tick += new System.EventHandler(this.updateFormTimer_Tick);
            // 
            // btn_ClearScreen
            // 
            this.btn_ClearScreen.Location = new System.Drawing.Point(100, 12);
            this.btn_ClearScreen.Name = "btn_ClearScreen";
            this.btn_ClearScreen.Size = new System.Drawing.Size(82, 37);
            this.btn_ClearScreen.TabIndex = 2;
            this.btn_ClearScreen.Text = "Clear Screen";
            this.btn_ClearScreen.UseVisualStyleBackColor = true;
            this.btn_ClearScreen.Click += new System.EventHandler(this.btn_ClearScreen_Click);
            // 
            // DebugForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(737, 385);
            this.Controls.Add(this.btn_ClearScreen);
            this.Controls.Add(this.txt_Debug);
            this.Controls.Add(this.btn_ReadDebug);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "DebugForm";
            this.Text = "Debug Form";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btn_ReadDebug;
        private System.Windows.Forms.TextBox txt_Debug;
        private System.Windows.Forms.Timer updateFormTimer;
        private System.Windows.Forms.Button btn_ClearScreen;
    }
}