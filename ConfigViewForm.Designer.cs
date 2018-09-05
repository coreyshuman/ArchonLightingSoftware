namespace ArchonLightingSystem
{
    partial class ConfigViewForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigViewForm));
            this.colorsGridView1 = new System.Windows.Forms.DataGridView();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.btn_ResetConfig = new System.Windows.Forms.Button();
            this.ledSpeedGridView = new System.Windows.Forms.DataGridView();
            this.label4 = new System.Windows.Forms.Label();
            this.btn_WriteConfig = new System.Windows.Forms.Button();
            this.btn_ReadConfig = new System.Windows.Forms.Button();
            this.fanSpeedGridView = new System.Windows.Forms.DataGridView();
            this.ledModeGridView = new System.Windows.Forms.DataGridView();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
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
            this.colorsGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.colorsGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.colorsGridView1.Location = new System.Drawing.Point(-1, 70);
            this.colorsGridView1.Name = "colorsGridView1";
            this.colorsGridView1.Size = new System.Drawing.Size(1277, 59);
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
            this.splitContainer1.Panel1.Controls.Add(this.btn_ResetConfig);
            this.splitContainer1.Panel1.Controls.Add(this.ledSpeedGridView);
            this.splitContainer1.Panel1.Controls.Add(this.label4);
            this.splitContainer1.Panel1.Controls.Add(this.btn_WriteConfig);
            this.splitContainer1.Panel1.Controls.Add(this.btn_ReadConfig);
            this.splitContainer1.Panel1.Controls.Add(this.fanSpeedGridView);
            this.splitContainer1.Panel1.Controls.Add(this.ledModeGridView);
            this.splitContainer1.Panel1.Controls.Add(this.label3);
            this.splitContainer1.Panel1.Controls.Add(this.label2);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.label1);
            this.splitContainer1.Panel2.Controls.Add(this.ledColorNavigator);
            this.splitContainer1.Panel2.Controls.Add(this.colorsGridView1);
            this.splitContainer1.Size = new System.Drawing.Size(1277, 740);
            this.splitContainer1.SplitterDistance = 328;
            this.splitContainer1.TabIndex = 2;
            // 
            // btn_ResetConfig
            // 
            this.btn_ResetConfig.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_ResetConfig.Location = new System.Drawing.Point(409, 12);
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
            this.ledSpeedGridView.Location = new System.Drawing.Point(-1, 257);
            this.ledSpeedGridView.Name = "ledSpeedGridView";
            this.ledSpeedGridView.Size = new System.Drawing.Size(1277, 69);
            this.ledSpeedGridView.TabIndex = 7;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(3, 234);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(96, 20);
            this.label4.TabIndex = 6;
            this.label4.Text = "Led Speed";
            // 
            // btn_WriteConfig
            // 
            this.btn_WriteConfig.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_WriteConfig.Location = new System.Drawing.Point(305, 12);
            this.btn_WriteConfig.Name = "btn_WriteConfig";
            this.btn_WriteConfig.Size = new System.Drawing.Size(98, 35);
            this.btn_WriteConfig.TabIndex = 5;
            this.btn_WriteConfig.Text = "Write Config";
            this.btn_WriteConfig.UseVisualStyleBackColor = true;
            this.btn_WriteConfig.Click += new System.EventHandler(this.btn_WriteConfig_Click);
            // 
            // btn_ReadConfig
            // 
            this.btn_ReadConfig.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_ReadConfig.Location = new System.Drawing.Point(204, 13);
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
            this.fanSpeedGridView.Location = new System.Drawing.Point(0, 68);
            this.fanSpeedGridView.Name = "fanSpeedGridView";
            this.fanSpeedGridView.Size = new System.Drawing.Size(1277, 69);
            this.fanSpeedGridView.TabIndex = 3;
            // 
            // ledModeGridView
            // 
            this.ledModeGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.ledModeGridView.Location = new System.Drawing.Point(0, 163);
            this.ledModeGridView.Name = "ledModeGridView";
            this.ledModeGridView.Size = new System.Drawing.Size(1277, 69);
            this.ledModeGridView.TabIndex = 2;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(4, 140);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(88, 20);
            this.label3.TabIndex = 1;
            this.label3.Text = "Led Mode";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(3, 45);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(160, 20);
            this.label2.TabIndex = 0;
            this.label2.Text = "Fan Speed (0-100)";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(4, 47);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(95, 20);
            this.label1.TabIndex = 3;
            this.label1.Text = "Led Colors";
            // 
            // ledColorNavigator
            // 
            this.ledColorNavigator.AddNewItem = null;
            this.ledColorNavigator.CountItem = this.bindingNavigatorCountItem;
            this.ledColorNavigator.DeleteItem = null;
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
            this.ledColorNavigator.Location = new System.Drawing.Point(0, 0);
            this.ledColorNavigator.MoveFirstItem = this.bindingNavigatorMoveFirstItem;
            this.ledColorNavigator.MoveLastItem = this.bindingNavigatorMoveLastItem;
            this.ledColorNavigator.MoveNextItem = this.bindingNavigatorMoveNextItem;
            this.ledColorNavigator.MovePreviousItem = this.bindingNavigatorMovePreviousItem;
            this.ledColorNavigator.Name = "ledColorNavigator";
            this.ledColorNavigator.PositionItem = this.bindingNavigatorPositionItem;
            this.ledColorNavigator.Size = new System.Drawing.Size(1277, 25);
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
            // ConfigViewForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1277, 740);
            this.Controls.Add(this.splitContainer1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "ConfigViewForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Device Config Editor";
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
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
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
        private System.Windows.Forms.Label label4;
    }
}