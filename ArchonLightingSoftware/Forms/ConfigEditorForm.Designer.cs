namespace ArchonLightingSystem
{
    partial class ConfigEditorForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigEditorForm));
            this.colorsGridView1 = new System.Windows.Forms.DataGridView();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.lbl_Disconnected = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.cbo_DeviceAddress = new System.Windows.Forms.ComboBox();
            this.btn_WriteLedFrame = new System.Windows.Forms.Button();
            this.btn_UpdateConfig = new System.Windows.Forms.Button();
            this.btn_ResetConfig = new System.Windows.Forms.Button();
            this.ledSpeedGridView = new System.Windows.Forms.DataGridView();
            this.lbl_LedSpeed = new System.Windows.Forms.Label();
            this.btn_WriteConfig = new System.Windows.Forms.Button();
            this.btn_ReadConfig = new System.Windows.Forms.Button();
            this.fanSpeedGridView = new System.Windows.Forms.DataGridView();
            this.ledModeGridView = new System.Windows.Forms.DataGridView();
            this.lbl_LedMode = new System.Windows.Forms.Label();
            this.lbl_FanSpeed = new System.Windows.Forms.Label();
            this.lbl_LedColors = new System.Windows.Forms.Label();
            this.ledColorNavigator = new System.Windows.Forms.BindingNavigator(this.components);
            this.bindingNavigatorCountItem = new System.Windows.Forms.ToolStripLabel();
            this.bindingNavigatorMoveFirstItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorMovePreviousItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.bindingNavigatorPositionItem = new System.Windows.Forms.ToolStripTextBox();
            this.bindingNavigatorSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.bindingNavigatorMoveNextItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorMoveLastItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.updateTimer = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.colorsGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ledSpeedGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fanSpeedGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ledModeGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ledColorNavigator)).BeginInit();
            this.ledColorNavigator.SuspendLayout();
            this.SuspendLayout();
            // 
            // colorsGridView1
            // 
            this.colorsGridView1.AllowUserToAddRows = false;
            this.colorsGridView1.AllowUserToDeleteRows = false;
            this.colorsGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.colorsGridView1.Location = new System.Drawing.Point(-1, 34);
            this.colorsGridView1.Name = "colorsGridView1";
            this.colorsGridView1.Size = new System.Drawing.Size(1255, 55);
            this.colorsGridView1.TabIndex = 1;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.lbl_Disconnected);
            this.splitContainer1.Panel1.Controls.Add(this.label5);
            this.splitContainer1.Panel1.Controls.Add(this.cbo_DeviceAddress);
            this.splitContainer1.Panel1.Controls.Add(this.btn_WriteLedFrame);
            this.splitContainer1.Panel1.Controls.Add(this.btn_UpdateConfig);
            this.splitContainer1.Panel1.Controls.Add(this.btn_ResetConfig);
            this.splitContainer1.Panel1.Controls.Add(this.ledSpeedGridView);
            this.splitContainer1.Panel1.Controls.Add(this.lbl_LedSpeed);
            this.splitContainer1.Panel1.Controls.Add(this.btn_WriteConfig);
            this.splitContainer1.Panel1.Controls.Add(this.btn_ReadConfig);
            this.splitContainer1.Panel1.Controls.Add(this.fanSpeedGridView);
            this.splitContainer1.Panel1.Controls.Add(this.ledModeGridView);
            this.splitContainer1.Panel1.Controls.Add(this.lbl_LedMode);
            this.splitContainer1.Panel1.Controls.Add(this.lbl_FanSpeed);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.lbl_LedColors);
            this.splitContainer1.Panel2.Controls.Add(this.ledColorNavigator);
            this.splitContainer1.Panel2.Controls.Add(this.colorsGridView1);
            this.splitContainer1.Size = new System.Drawing.Size(1254, 687);
            this.splitContainer1.SplitterDistance = 304;
            this.splitContainer1.TabIndex = 2;
            // 
            // lbl_Disconnected
            // 
            this.lbl_Disconnected.AutoSize = true;
            this.lbl_Disconnected.Location = new System.Drawing.Point(243, 23);
            this.lbl_Disconnected.Name = "lbl_Disconnected";
            this.lbl_Disconnected.Size = new System.Drawing.Size(73, 13);
            this.lbl_Disconnected.TabIndex = 13;
            this.lbl_Disconnected.Text = "Disconnected";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(12, 23);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(51, 13);
            this.label5.TabIndex = 12;
            this.label5.Text = "Device:";
            // 
            // cbo_DeviceAddress
            // 
            this.cbo_DeviceAddress.BackColor = System.Drawing.SystemColors.Control;
            this.cbo_DeviceAddress.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.cbo_DeviceAddress.ForeColor = System.Drawing.SystemColors.ControlText;
            this.cbo_DeviceAddress.FormattingEnabled = true;
            this.cbo_DeviceAddress.Location = new System.Drawing.Point(76, 20);
            this.cbo_DeviceAddress.Margin = new System.Windows.Forms.Padding(4);
            this.cbo_DeviceAddress.Name = "cbo_DeviceAddress";
            this.cbo_DeviceAddress.Size = new System.Drawing.Size(160, 21);
            this.cbo_DeviceAddress.TabIndex = 11;
            this.cbo_DeviceAddress.SelectedIndexChanged += new System.EventHandler(this.cbo_DeviceAddress_SelectedIndexChanged);
            // 
            // btn_WriteLedFrame
            // 
            this.btn_WriteLedFrame.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btn_WriteLedFrame.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_WriteLedFrame.Location = new System.Drawing.Point(1123, 12);
            this.btn_WriteLedFrame.Name = "btn_WriteLedFrame";
            this.btn_WriteLedFrame.Size = new System.Drawing.Size(119, 35);
            this.btn_WriteLedFrame.TabIndex = 10;
            this.btn_WriteLedFrame.Text = "Write Led Frame";
            this.btn_WriteLedFrame.UseVisualStyleBackColor = true;
            this.btn_WriteLedFrame.Click += new System.EventHandler(this.btn_WriteLedFrame_Click);
            // 
            // btn_UpdateConfig
            // 
            this.btn_UpdateConfig.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btn_UpdateConfig.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_UpdateConfig.Location = new System.Drawing.Point(806, 12);
            this.btn_UpdateConfig.Name = "btn_UpdateConfig";
            this.btn_UpdateConfig.Size = new System.Drawing.Size(103, 35);
            this.btn_UpdateConfig.TabIndex = 9;
            this.btn_UpdateConfig.Text = "Update Config";
            this.btn_UpdateConfig.UseVisualStyleBackColor = true;
            this.btn_UpdateConfig.Click += new System.EventHandler(this.btn_UpdateConfig_Click);
            // 
            // btn_ResetConfig
            // 
            this.btn_ResetConfig.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btn_ResetConfig.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_ResetConfig.Location = new System.Drawing.Point(1019, 12);
            this.btn_ResetConfig.Name = "btn_ResetConfig";
            this.btn_ResetConfig.Size = new System.Drawing.Size(98, 35);
            this.btn_ResetConfig.TabIndex = 8;
            this.btn_ResetConfig.Text = "Reset Config";
            this.btn_ResetConfig.UseVisualStyleBackColor = true;
            this.btn_ResetConfig.Click += new System.EventHandler(this.btn_ResetConfig_Click);
            // 
            // ledSpeedGridView
            // 
            this.ledSpeedGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.ledSpeedGridView.Location = new System.Drawing.Point(-1, 247);
            this.ledSpeedGridView.Name = "ledSpeedGridView";
            this.ledSpeedGridView.Size = new System.Drawing.Size(1255, 55);
            this.ledSpeedGridView.TabIndex = 7;
            // 
            // lbl_LedSpeed
            // 
            this.lbl_LedSpeed.AutoSize = true;
            this.lbl_LedSpeed.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_LedSpeed.Location = new System.Drawing.Point(3, 224);
            this.lbl_LedSpeed.Name = "lbl_LedSpeed";
            this.lbl_LedSpeed.Size = new System.Drawing.Size(96, 20);
            this.lbl_LedSpeed.TabIndex = 6;
            this.lbl_LedSpeed.Text = "Led Speed";
            // 
            // btn_WriteConfig
            // 
            this.btn_WriteConfig.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btn_WriteConfig.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_WriteConfig.Location = new System.Drawing.Point(915, 12);
            this.btn_WriteConfig.Name = "btn_WriteConfig";
            this.btn_WriteConfig.Size = new System.Drawing.Size(98, 35);
            this.btn_WriteConfig.TabIndex = 5;
            this.btn_WriteConfig.Text = "Write Config";
            this.btn_WriteConfig.UseVisualStyleBackColor = true;
            this.btn_WriteConfig.Click += new System.EventHandler(this.btn_WriteConfig_Click);
            // 
            // btn_ReadConfig
            // 
            this.btn_ReadConfig.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btn_ReadConfig.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_ReadConfig.Location = new System.Drawing.Point(705, 12);
            this.btn_ReadConfig.Name = "btn_ReadConfig";
            this.btn_ReadConfig.Size = new System.Drawing.Size(95, 35);
            this.btn_ReadConfig.TabIndex = 4;
            this.btn_ReadConfig.Text = "Read Config";
            this.btn_ReadConfig.UseVisualStyleBackColor = true;
            this.btn_ReadConfig.Click += new System.EventHandler(this.btn_ReadConfig_Click);
            // 
            // fanSpeedGridView
            // 
            this.fanSpeedGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.fanSpeedGridView.Location = new System.Drawing.Point(0, 85);
            this.fanSpeedGridView.Name = "fanSpeedGridView";
            this.fanSpeedGridView.Size = new System.Drawing.Size(1255, 55);
            this.fanSpeedGridView.TabIndex = 3;
            // 
            // ledModeGridView
            // 
            this.ledModeGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.ledModeGridView.Location = new System.Drawing.Point(0, 166);
            this.ledModeGridView.Name = "ledModeGridView";
            this.ledModeGridView.Size = new System.Drawing.Size(1255, 55);
            this.ledModeGridView.TabIndex = 2;
            // 
            // lbl_LedMode
            // 
            this.lbl_LedMode.AutoSize = true;
            this.lbl_LedMode.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_LedMode.Location = new System.Drawing.Point(4, 143);
            this.lbl_LedMode.Name = "lbl_LedMode";
            this.lbl_LedMode.Size = new System.Drawing.Size(88, 20);
            this.lbl_LedMode.TabIndex = 1;
            this.lbl_LedMode.Text = "Led Mode";
            // 
            // lbl_FanSpeed
            // 
            this.lbl_FanSpeed.AutoSize = true;
            this.lbl_FanSpeed.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_FanSpeed.Location = new System.Drawing.Point(3, 62);
            this.lbl_FanSpeed.Name = "lbl_FanSpeed";
            this.lbl_FanSpeed.Size = new System.Drawing.Size(160, 20);
            this.lbl_FanSpeed.TabIndex = 0;
            this.lbl_FanSpeed.Text = "Fan Speed (0-100)";
            // 
            // lbl_LedColors
            // 
            this.lbl_LedColors.AutoSize = true;
            this.lbl_LedColors.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_LedColors.Location = new System.Drawing.Point(4, 8);
            this.lbl_LedColors.Name = "lbl_LedColors";
            this.lbl_LedColors.Size = new System.Drawing.Size(202, 20);
            this.lbl_LedColors.TabIndex = 3;
            this.lbl_LedColors.Text = "Led Colors (GRB) 1 - 12";
            // 
            // ledColorNavigator
            // 
            this.ledColorNavigator.AddNewItem = null;
            this.ledColorNavigator.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ledColorNavigator.CountItem = this.bindingNavigatorCountItem;
            this.ledColorNavigator.DeleteItem = null;
            this.ledColorNavigator.Dock = System.Windows.Forms.DockStyle.None;
            this.ledColorNavigator.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.bindingNavigatorMoveFirstItem,
            this.bindingNavigatorMovePreviousItem,
            this.bindingNavigatorSeparator,
            this.bindingNavigatorPositionItem,
            this.bindingNavigatorCountItem,
            this.bindingNavigatorSeparator1,
            this.bindingNavigatorMoveNextItem,
            this.bindingNavigatorMoveLastItem,
            this.bindingNavigatorSeparator2});
            this.ledColorNavigator.Location = new System.Drawing.Point(515, 300);
            this.ledColorNavigator.MoveFirstItem = this.bindingNavigatorMoveFirstItem;
            this.ledColorNavigator.MoveLastItem = this.bindingNavigatorMoveLastItem;
            this.ledColorNavigator.MoveNextItem = this.bindingNavigatorMoveNextItem;
            this.ledColorNavigator.MovePreviousItem = this.bindingNavigatorMovePreviousItem;
            this.ledColorNavigator.Name = "ledColorNavigator";
            this.ledColorNavigator.PositionItem = this.bindingNavigatorPositionItem;
            this.ledColorNavigator.Size = new System.Drawing.Size(209, 25);
            this.ledColorNavigator.TabIndex = 2;
            this.ledColorNavigator.Text = "bindingNavigator1";
            // 
            // bindingNavigatorCountItem
            // 
            this.bindingNavigatorCountItem.Name = "bindingNavigatorCountItem";
            this.bindingNavigatorCountItem.Size = new System.Drawing.Size(35, 22);
            this.bindingNavigatorCountItem.Text = "of {0}";
            this.bindingNavigatorCountItem.ToolTipText = "Total number of items";
            // 
            // bindingNavigatorMoveFirstItem
            // 
            this.bindingNavigatorMoveFirstItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorMoveFirstItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorMoveFirstItem.Image")));
            this.bindingNavigatorMoveFirstItem.Name = "bindingNavigatorMoveFirstItem";
            this.bindingNavigatorMoveFirstItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorMoveFirstItem.Size = new System.Drawing.Size(23, 22);
            this.bindingNavigatorMoveFirstItem.Text = "Move first";
            // 
            // bindingNavigatorMovePreviousItem
            // 
            this.bindingNavigatorMovePreviousItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorMovePreviousItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorMovePreviousItem.Image")));
            this.bindingNavigatorMovePreviousItem.Name = "bindingNavigatorMovePreviousItem";
            this.bindingNavigatorMovePreviousItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorMovePreviousItem.Size = new System.Drawing.Size(23, 22);
            this.bindingNavigatorMovePreviousItem.Text = "Move previous";
            // 
            // bindingNavigatorSeparator
            // 
            this.bindingNavigatorSeparator.Name = "bindingNavigatorSeparator";
            this.bindingNavigatorSeparator.Size = new System.Drawing.Size(6, 25);
            // 
            // bindingNavigatorPositionItem
            // 
            this.bindingNavigatorPositionItem.AccessibleName = "Position";
            this.bindingNavigatorPositionItem.AutoSize = false;
            this.bindingNavigatorPositionItem.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.bindingNavigatorPositionItem.Name = "bindingNavigatorPositionItem";
            this.bindingNavigatorPositionItem.Size = new System.Drawing.Size(50, 23);
            this.bindingNavigatorPositionItem.Text = "0";
            this.bindingNavigatorPositionItem.ToolTipText = "Current position";
            // 
            // bindingNavigatorSeparator1
            // 
            this.bindingNavigatorSeparator1.Name = "bindingNavigatorSeparator1";
            this.bindingNavigatorSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // bindingNavigatorMoveNextItem
            // 
            this.bindingNavigatorMoveNextItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorMoveNextItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorMoveNextItem.Image")));
            this.bindingNavigatorMoveNextItem.Name = "bindingNavigatorMoveNextItem";
            this.bindingNavigatorMoveNextItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorMoveNextItem.Size = new System.Drawing.Size(23, 22);
            this.bindingNavigatorMoveNextItem.Text = "Move next";
            // 
            // bindingNavigatorMoveLastItem
            // 
            this.bindingNavigatorMoveLastItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorMoveLastItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorMoveLastItem.Image")));
            this.bindingNavigatorMoveLastItem.Name = "bindingNavigatorMoveLastItem";
            this.bindingNavigatorMoveLastItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorMoveLastItem.Size = new System.Drawing.Size(23, 22);
            this.bindingNavigatorMoveLastItem.Text = "Move last";
            // 
            // bindingNavigatorSeparator2
            // 
            this.bindingNavigatorSeparator2.Name = "bindingNavigatorSeparator2";
            this.bindingNavigatorSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // updateTimer
            // 
            this.updateTimer.Tick += new System.EventHandler(this.updateTimer_Tick);
            // 
            // ConfigEditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(1254, 687);
            this.Controls.Add(this.splitContainer1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "ConfigEditorForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Device Config Editor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ConfigEditorForm_FormClosing);
            this.Load += new System.EventHandler(this.ConfigEditorForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.colorsGridView1)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ledSpeedGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fanSpeedGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ledModeGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ledColorNavigator)).EndInit();
            this.ledColorNavigator.ResumeLayout(false);
            this.ledColorNavigator.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.DataGridView colorsGridView1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Label lbl_LedMode;
        private System.Windows.Forms.Label lbl_FanSpeed;
        private System.Windows.Forms.Label lbl_LedColors;
        private System.Windows.Forms.BindingNavigator ledColorNavigator;
        private System.Windows.Forms.ToolStripLabel bindingNavigatorCountItem;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMoveFirstItem;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMovePreviousItem;
        private System.Windows.Forms.ToolStripSeparator bindingNavigatorSeparator;
        private System.Windows.Forms.ToolStripTextBox bindingNavigatorPositionItem;
        private System.Windows.Forms.ToolStripSeparator bindingNavigatorSeparator1;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMoveNextItem;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMoveLastItem;
        private System.Windows.Forms.ToolStripSeparator bindingNavigatorSeparator2;
        private System.Windows.Forms.DataGridView fanSpeedGridView;
        private System.Windows.Forms.DataGridView ledModeGridView;
        private System.Windows.Forms.Button btn_WriteConfig;
        private System.Windows.Forms.Button btn_ReadConfig;
        private System.Windows.Forms.Timer updateTimer;
        private System.Windows.Forms.Button btn_ResetConfig;
        private System.Windows.Forms.DataGridView ledSpeedGridView;
        private System.Windows.Forms.Label lbl_LedSpeed;
        private System.Windows.Forms.Button btn_UpdateConfig;
        private System.Windows.Forms.Button btn_WriteLedFrame;
        private System.Windows.Forms.Label lbl_Disconnected;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cbo_DeviceAddress;
    }
}