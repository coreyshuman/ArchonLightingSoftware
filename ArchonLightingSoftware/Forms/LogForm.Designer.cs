namespace ArchonLightingSystem.Forms
{
    partial class LogForm
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
            this.txt_log = new System.Windows.Forms.RichTextBox();
            this.lvlLabel = new System.Windows.Forms.Label();
            this.cb_Level = new System.Windows.Forms.ComboBox();
            this.btn_CopyToClipboard = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txt_log
            // 
            this.txt_log.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txt_log.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(16)))), ((int)(((byte)(16)))));
            this.txt_log.Font = new System.Drawing.Font("Lucida Console", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_log.Location = new System.Drawing.Point(12, 57);
            this.txt_log.Name = "txt_log";
            this.txt_log.ReadOnly = true;
            this.txt_log.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.txt_log.Size = new System.Drawing.Size(776, 381);
            this.txt_log.TabIndex = 0;
            this.txt_log.Text = "";
            // 
            // lvlLabel
            // 
            this.lvlLabel.AutoSize = true;
            this.lvlLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold);
            this.lvlLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(233)))), ((int)(((byte)(243)))));
            this.lvlLabel.Location = new System.Drawing.Point(612, 17);
            this.lvlLabel.Name = "lvlLabel";
            this.lvlLabel.Size = new System.Drawing.Size(49, 16);
            this.lvlLabel.TabIndex = 1;
            this.lvlLabel.Text = "Level:";
            // 
            // cb_Level
            // 
            this.cb_Level.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.cb_Level.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
            this.cb_Level.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(233)))), ((int)(((byte)(243)))));
            this.cb_Level.FormattingEnabled = true;
            this.cb_Level.Location = new System.Drawing.Point(667, 14);
            this.cb_Level.Name = "cb_Level";
            this.cb_Level.Size = new System.Drawing.Size(121, 24);
            this.cb_Level.TabIndex = 2;
            this.cb_Level.SelectedIndexChanged += new System.EventHandler(this.comboBoxLevel_SelectedIndexChanged);
            // 
            // btn_CopyToClipboard
            // 
            this.btn_CopyToClipboard.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.btn_CopyToClipboard.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btn_CopyToClipboard.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
            this.btn_CopyToClipboard.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(233)))), ((int)(((byte)(243)))));
            this.btn_CopyToClipboard.Location = new System.Drawing.Point(12, 9);
            this.btn_CopyToClipboard.Name = "btn_CopyToClipboard";
            this.btn_CopyToClipboard.Size = new System.Drawing.Size(128, 33);
            this.btn_CopyToClipboard.TabIndex = 8;
            this.btn_CopyToClipboard.Text = "Copy to Clipboard";
            this.btn_CopyToClipboard.UseVisualStyleBackColor = false;
            this.btn_CopyToClipboard.Click += new System.EventHandler(this.btn_CopyToClipboard_Click);
            // 
            // LogForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.btn_CopyToClipboard);
            this.Controls.Add(this.cb_Level);
            this.Controls.Add(this.lvlLabel);
            this.Controls.Add(this.txt_log);
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(233)))), ((int)(((byte)(243)))));
            this.Name = "LogForm";
            this.Text = "Application Log";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.LogForm_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox txt_log;
        private System.Windows.Forms.Label lvlLabel;
        private System.Windows.Forms.ComboBox cb_Level;
        private System.Windows.Forms.Button btn_CopyToClipboard;
    }
}