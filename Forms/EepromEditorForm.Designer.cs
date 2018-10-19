namespace ArchonLightingSystem
{
    partial class EepromEditorForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EepromEditorForm));
            this.eepromGridView = new System.Windows.Forms.DataGridView();
            this.btn_ReadEeprom = new System.Windows.Forms.Button();
            this.btn_WriteEeprom = new System.Windows.Forms.Button();
            this.formUpdateTimer = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.eepromGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // eepromGridView
            // 
            this.eepromGridView.AllowUserToAddRows = false;
            this.eepromGridView.AllowUserToDeleteRows = false;
            this.eepromGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.eepromGridView.Location = new System.Drawing.Point(0, 49);
            this.eepromGridView.Name = "eepromGridView";
            this.eepromGridView.Size = new System.Drawing.Size(841, 386);
            this.eepromGridView.TabIndex = 0;
            this.eepromGridView.DataBindingComplete += new System.Windows.Forms.DataGridViewBindingCompleteEventHandler(this.eepromGridView_DataBindingComplete);
            // 
            // btn_ReadEeprom
            // 
            this.btn_ReadEeprom.Location = new System.Drawing.Point(26, 12);
            this.btn_ReadEeprom.Name = "btn_ReadEeprom";
            this.btn_ReadEeprom.Size = new System.Drawing.Size(93, 31);
            this.btn_ReadEeprom.TabIndex = 1;
            this.btn_ReadEeprom.Text = "Read EEPROM";
            this.btn_ReadEeprom.UseVisualStyleBackColor = true;
            this.btn_ReadEeprom.Click += new System.EventHandler(this.btn_ReadEeprom_Click);
            // 
            // btn_WriteEeprom
            // 
            this.btn_WriteEeprom.Location = new System.Drawing.Point(125, 12);
            this.btn_WriteEeprom.Name = "btn_WriteEeprom";
            this.btn_WriteEeprom.Size = new System.Drawing.Size(93, 31);
            this.btn_WriteEeprom.TabIndex = 2;
            this.btn_WriteEeprom.Text = "Write EEPROM";
            this.btn_WriteEeprom.UseVisualStyleBackColor = true;
            this.btn_WriteEeprom.Click += new System.EventHandler(this.btn_WriteEeprom_Click);
            // 
            // formUpdateTimer
            // 
            this.formUpdateTimer.Tick += new System.EventHandler(this.formUpdateTimer_Tick);
            // 
            // EepromEditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(841, 435);
            this.Controls.Add(this.btn_WriteEeprom);
            this.Controls.Add(this.btn_ReadEeprom);
            this.Controls.Add(this.eepromGridView);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EepromEditorForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Eeprom Editor";
            ((System.ComponentModel.ISupportInitialize)(this.eepromGridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView eepromGridView;
        private System.Windows.Forms.Button btn_ReadEeprom;
        private System.Windows.Forms.Button btn_WriteEeprom;
        private System.Windows.Forms.Timer formUpdateTimer;
    }
}