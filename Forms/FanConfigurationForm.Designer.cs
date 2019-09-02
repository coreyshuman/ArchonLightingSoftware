namespace ArchonLightingSystem.Forms
{
    partial class FanConfigurationForm
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.listView1 = new System.Windows.Forms.ListView();
            this.btn_Back = new System.Windows.Forms.Button();
            this.btn_Close = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.lbl_Selected = new System.Windows.Forms.Label();
            this.btn_OK = new System.Windows.Forms.Button();
            this.fanCurveChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.useFanCheckbox = new System.Windows.Forms.CheckBox();
            this.btnCopyFanCurve = new System.Windows.Forms.Button();
            this.btnPasteFanCurve = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.fanCurveChart)).BeginInit();
            this.SuspendLayout();
            // 
            // listView1
            // 
            this.listView1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.listView1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(233)))), ((int)(((byte)(243)))));
            this.listView1.HideSelection = false;
            this.listView1.Location = new System.Drawing.Point(56, 59);
            this.listView1.Margin = new System.Windows.Forms.Padding(4);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(642, 243);
            this.listView1.TabIndex = 0;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.ItemActivate += new System.EventHandler(this.listView1_ItemActivate);
            this.listView1.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.listView1_ItemChecked);
            this.listView1.Click += new System.EventHandler(this.listView1_Click);
            this.listView1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listView1_KeyDown);
            // 
            // btn_Back
            // 
            this.btn_Back.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.btn_Back.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btn_Back.Location = new System.Drawing.Point(56, 23);
            this.btn_Back.Margin = new System.Windows.Forms.Padding(4);
            this.btn_Back.Name = "btn_Back";
            this.btn_Back.Size = new System.Drawing.Size(100, 28);
            this.btn_Back.TabIndex = 1;
            this.btn_Back.Text = "Back";
            this.btn_Back.UseVisualStyleBackColor = false;
            this.btn_Back.Click += new System.EventHandler(this.btn_Back_Click);
            // 
            // btn_Close
            // 
            this.btn_Close.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.btn_Close.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btn_Close.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btn_Close.Location = new System.Drawing.Point(954, 510);
            this.btn_Close.Margin = new System.Windows.Forms.Padding(4);
            this.btn_Close.Name = "btn_Close";
            this.btn_Close.Size = new System.Drawing.Size(100, 28);
            this.btn_Close.TabIndex = 2;
            this.btn_Close.Text = "Cancel";
            this.btn_Close.UseVisualStyleBackColor = false;
            this.btn_Close.Click += new System.EventHandler(this.btn_Close_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(706, 59);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 16);
            this.label1.TabIndex = 3;
            this.label1.Text = "Selected:";
            // 
            // lbl_Selected
            // 
            this.lbl_Selected.AutoSize = true;
            this.lbl_Selected.Location = new System.Drawing.Point(732, 84);
            this.lbl_Selected.Name = "lbl_Selected";
            this.lbl_Selected.Size = new System.Drawing.Size(17, 16);
            this.lbl_Selected.TabIndex = 4;
            this.lbl_Selected.Text = "...";
            // 
            // btn_OK
            // 
            this.btn_OK.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.btn_OK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btn_OK.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btn_OK.Location = new System.Drawing.Point(846, 510);
            this.btn_OK.Margin = new System.Windows.Forms.Padding(4);
            this.btn_OK.Name = "btn_OK";
            this.btn_OK.Size = new System.Drawing.Size(100, 28);
            this.btn_OK.TabIndex = 5;
            this.btn_OK.Text = "OK";
            this.btn_OK.UseVisualStyleBackColor = false;
            this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
            // 
            // fanCurveChart
            // 
            this.fanCurveChart.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            chartArea1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            chartArea1.BorderColor = System.Drawing.Color.White;
            chartArea1.Name = "ChartArea1";
            this.fanCurveChart.ChartAreas.Add(chartArea1);
            this.fanCurveChart.Location = new System.Drawing.Point(56, 309);
            this.fanCurveChart.Name = "fanCurveChart";
            this.fanCurveChart.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.None;
            series1.BorderWidth = 3;
            series1.ChartArea = "ChartArea1";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series1.Name = "FanCurveSeries";
            series2.ChartArea = "ChartArea1";
            series2.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Point;
            series2.MarkerSize = 7;
            series2.Name = "FanCurvePoints";
            this.fanCurveChart.Series.Add(series1);
            this.fanCurveChart.Series.Add(series2);
            this.fanCurveChart.Size = new System.Drawing.Size(642, 229);
            this.fanCurveChart.TabIndex = 6;
            this.fanCurveChart.Text = "fanCurveChart";
            this.fanCurveChart.PrePaint += new System.EventHandler<System.Windows.Forms.DataVisualization.Charting.ChartPaintEventArgs>(this.fanCurveChart_PrePaint);
            this.fanCurveChart.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FanCurveChart_KeyDown);
            this.fanCurveChart.MouseDown += new System.Windows.Forms.MouseEventHandler(this.fanCurveChart_MouseDown);
            this.fanCurveChart.MouseMove += new System.Windows.Forms.MouseEventHandler(this.fanCurveChart_MouseMove);
            this.fanCurveChart.MouseUp += new System.Windows.Forms.MouseEventHandler(this.fanCurveChart_MouseUp);
            // 
            // useFanCheckbox
            // 
            this.useFanCheckbox.AutoSize = true;
            this.useFanCheckbox.Location = new System.Drawing.Point(709, 309);
            this.useFanCheckbox.Name = "useFanCheckbox";
            this.useFanCheckbox.Size = new System.Drawing.Size(116, 20);
            this.useFanCheckbox.TabIndex = 7;
            this.useFanCheckbox.Text = "Use Fan Curve";
            this.useFanCheckbox.UseVisualStyleBackColor = true;
            this.useFanCheckbox.CheckedChanged += new System.EventHandler(this.useFanCheckbox_CheckedChanged);
            // 
            // btnCopyFanCurve
            // 
            this.btnCopyFanCurve.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.btnCopyFanCurve.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnCopyFanCurve.Location = new System.Drawing.Point(709, 335);
            this.btnCopyFanCurve.Name = "btnCopyFanCurve";
            this.btnCopyFanCurve.Size = new System.Drawing.Size(116, 31);
            this.btnCopyFanCurve.TabIndex = 8;
            this.btnCopyFanCurve.Text = "Copy Fan Curve";
            this.btnCopyFanCurve.UseVisualStyleBackColor = false;
            this.btnCopyFanCurve.Click += new System.EventHandler(this.btnCopyFanCurve_Click);
            // 
            // btnPasteFanCurve
            // 
            this.btnPasteFanCurve.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.btnPasteFanCurve.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnPasteFanCurve.Location = new System.Drawing.Point(831, 335);
            this.btnPasteFanCurve.Name = "btnPasteFanCurve";
            this.btnPasteFanCurve.Size = new System.Drawing.Size(116, 31);
            this.btnPasteFanCurve.TabIndex = 9;
            this.btnPasteFanCurve.Text = "Paste Fan Curve";
            this.btnPasteFanCurve.UseVisualStyleBackColor = false;
            this.btnPasteFanCurve.Click += new System.EventHandler(this.btnPasteFanCurve_Click);
            // 
            // FanConfigurationForm
            // 
            this.AcceptButton = this.btn_OK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.CancelButton = this.btn_Close;
            this.ClientSize = new System.Drawing.Size(1067, 554);
            this.Controls.Add(this.btnPasteFanCurve);
            this.Controls.Add(this.btnCopyFanCurve);
            this.Controls.Add(this.useFanCheckbox);
            this.Controls.Add(this.fanCurveChart);
            this.Controls.Add(this.btn_OK);
            this.Controls.Add(this.lbl_Selected);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btn_Close);
            this.Controls.Add(this.btn_Back);
            this.Controls.Add(this.listView1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(233)))), ((int)(((byte)(243)))));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FanConfigurationForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "FanConfigurationForm";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FanConfigurationForm_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.fanCurveChart)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.Button btn_Back;
        private System.Windows.Forms.Button btn_Close;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lbl_Selected;
        private System.Windows.Forms.Button btn_OK;
        private System.Windows.Forms.DataVisualization.Charting.Chart fanCurveChart;
        private System.Windows.Forms.CheckBox useFanCheckbox;
        private System.Windows.Forms.Button btnCopyFanCurve;
        private System.Windows.Forms.Button btnPasteFanCurve;
    }
}