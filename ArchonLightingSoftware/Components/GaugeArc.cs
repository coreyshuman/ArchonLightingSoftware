using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ArchonLightingSystem.Components
{
    public partial class GaugeArc : UserControl
    {
        private double val = 0;
        private double scale = 100;
        private double min = 0;
        private double max = 100;
        private double overload = 100;
        private bool flipped = false;
        private string unitLabel = "%";

        public GaugeArc()
        {
            InitializeComponent();

            Initialize();
        }

        public GaugeArc(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
            Initialize();
        }

        private void Initialize()
        {
            this.SetAutoSizeMode(AutoSizeMode.GrowOnly);
            base.Width = 160;
            base.Height = 140;
            base.MinimumSize = new Size(this.Width, this.Height);
            this.BackColor = Color.LightGray;
            this.DoubleBuffered = true;
        }

        /// <summary>
        /// Set the value of the gauge and redraw.
        /// </summary>
        public double Value
        {
            get
            {
                return val;
            }
            set
            {
                if(value < min)
                {
                    val = min;
                }
                else if(value > max)
                {
                    val = max;
                }
                else
                {
                    val = value;
                }
                Invalidate();
            }
        }

        /// <summary>
        /// Set the minimum allowed value for the gauge and redraw.
        /// </summary>
        public double MinimumValue
        {
            get
            {
                return min;
            }
            set
            {
                if (val < value)
                {
                    val = value;
                }
                if(max < value)
                {
                    max = value + 1;
                }
                min = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Set the maximum allowed value for the gauge and redraw.
        /// </summary>
        public double MaximumValue
        {
            get
            {
                return max;
            }
            set
            {
                if (val > value)
                {
                    val = value;
                }
                if (min > value)
                {
                    min = value - 1;
                }
                if(overload == max)
                {
                    overload = value;
                }
                max = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Set the warning range at the top of the gauge arc.
        /// </summary>
        public double OverValue
        {
            get
            {
                return overload;
            }
            set
            {
                if (min > value)
                {
                    overload = min;
                }
                if (max < value)
                {
                    overload = max;
                }
                else
                {
                    overload = value;
                }
                Invalidate();
            }
        }

        /// <summary>
        /// Get or set the Width of the Control. Note this will also update the Height to retain a correct aspect ratio.
        /// </summary>
        new public int Width
        {
            get
            {
                return base.Width;
            }
            set 
            {
                if(value < 1)
                {
                    throw new ArgumentException("Width must be greater than zero.");
                }
                scale = (double)value * (100d / 160d);
                base.Width = value;
                base.Height = (int)(value * 140d / 160d);
                Invalidate();
            }
        }

        /// <summary>
        /// Get or set the Height of the Control. Note this will also update the Width to retain a correct aspect ratio.
        /// </summary>
        new public int Height
        {
            get
            {
                return base.Height;
            }
            set
            {
                if (value < 1)
                {
                    throw new ArgumentException("Width must be greater than zero.");
                }
                scale = (double)value * (100d / 140d);
                base.Height = value;
                base.Width = (int)(value * 160d / 140d);
                Invalidate();
            }
        }

        /// <summary>
        /// Get the size of the control.
        /// </summary>
        new public Size Size { get { return base.Size; } }

        /// <summary>
        /// Get the minimum size allowd for the control.
        /// </summary>
        new public Size MinimumSize { get { return base.MinimumSize; } }

        /// <summary>
        /// Get or Set the unit symbol shown on the gauge.
        /// </summary>
        [DefaultValue("%")]
        public String UnitSymbol
        {
            get
            {
                return unitLabel;
            }
            set
            {
                unitLabel = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Flip the gauge to the right when true.
        /// </summary>
        public bool Flipped
        {
            get
            {
                return flipped;
            }
            set
            {
                flipped = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Primary color of the gauge's drawn arc.
        /// </summary>
        public Color LineColor { get; set; }

        /// <summary>
        /// Warning color of the gauge's drawn arc in the overload region.
        /// </summary>
        public Color OverLineColor { get; set; }

        public String Label { get; set; }

        private void GaugeArc_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            Pen pen = new Pen(this.ForeColor, (float)(scale / 55d));
            Pen penCircle = new Pen(this.ForeColor, (float)(scale / 35d));
            Pen penLine = new Pen(this.LineColor, (float)(scale / 35d));
            Pen overloadLine = new Pen(this.OverLineColor, (float)(scale / 35d));
            Font font = new Font("monospace", (float)(scale / 10d), FontStyle.Bold);
            SolidBrush brush = new SolidBrush(this.ForeColor);
            SolidBrush brushBack = new SolidBrush(this.BackColor);


            if (overload < max)
            {
                double primaryLineMax = overload / max;
                double primaryToMaxOffset = max - overload;

                g.DrawBezier(penLine,
                    new Point((int)(scale * primaryLineMax * (flipped ? 0.4 : 0.2)), (int)CalcArcCurve(0, scale, flipped) + (int)(scale * 0.2)),
                    new Point((int)(scale * primaryLineMax * (flipped ? 0.7 : 0.5)), (int)CalcArcCurve((scale * 0.3 * primaryLineMax), scale, flipped) + (int)(scale * 0.2)),
                    new Point((int)(scale * primaryLineMax * (flipped ? 1.1 : 0.9)), (int)CalcArcCurve((scale * 0.7 * primaryLineMax), scale, flipped) + (int)(scale * 0.2)),
                    new Point((int)(scale * primaryLineMax * (flipped ? 1.4 : 1.2)), (int)CalcArcCurve((scale * primaryLineMax), scale, flipped) + (int)(scale * 0.2)));

                g.DrawBezier(overloadLine,
                    new Point((int)(scale * (flipped ? 0.4 : 0.2)), (int)CalcArcCurve(primaryLineMax, scale, flipped) + (int)(scale * 0.2)),
                    new Point((int)(scale * (flipped ? 0.7 : 0.5)), (int)CalcArcCurve((scale * (primaryLineMax + 0.3 * (1 - primaryLineMax))), scale, flipped) + (int)(scale * 0.2)),
                    new Point((int)(scale * (flipped ? 1.1 : 0.9)), (int)CalcArcCurve((scale * (primaryLineMax + 0.7 * (1 - primaryLineMax))), scale, flipped) + (int)(scale * 0.2)),
                    new Point((int)(scale * (flipped ? 1.4 : 1.2)), (int)CalcArcCurve((scale * 1.0), scale, flipped) + (int)(scale * 0.2)));

            }
            else
            {
                g.DrawBezier(penLine,
                    new Point((int)(scale * (flipped ? 0.4 : 0.2)), (int)CalcArcCurve(0, scale, flipped) + (int)(scale * 0.2)),
                    new Point((int)(scale * (flipped ? 0.7 : 0.5)), (int)CalcArcCurve((scale * 0.3), scale, flipped) + (int)(scale * 0.2)),
                    new Point((int)(scale * (flipped ? 1.1 : 0.9)), (int)CalcArcCurve((scale * 0.7), scale, flipped) + (int)(scale * 0.2)),
                    new Point((int)(scale * (flipped ? 1.4 : 1.2)), (int)CalcArcCurve((scale * 1.0), scale, flipped) + (int)(scale * 0.2)));
            }
            
            // this throws a generic gdi+ error occassionally
            g.DrawIcon(Icon.FromHandle(Properties.Resources.temperature.GetHicon()), new Rectangle { 
                X = (int)(scale * (flipped ? 0.1 : 1.2)),
                Y = (int)(scale * 0.1),
                Width = (int)(scale * 0.24),
                Height = (int)(scale * 0.24)
            });


            string tickLabel = min.ToString("0.");
            float tickLabelOffset = (flipped ? g.MeasureString(tickLabel, font).Width : 0);
            g.DrawString(tickLabel, font, brush, new Point(
                (int)(scale * (flipped ? 0.3 : 1.34) - tickLabelOffset), 
                (int)CalcArcCurve((scale * (flipped ? 0.0 : 1.0)), scale, flipped) + (int)(scale * 0.1)));

            tickLabel = ((max - min) / 2d).ToString("0.");
            tickLabelOffset = (flipped ? g.MeasureString(tickLabel, font).Width : 0);
            g.DrawString(tickLabel, font, brush, new Point(
                (int)(scale * (flipped ? 0.62 : 0.99) - tickLabelOffset),
                (int)CalcArcCurve((scale * (flipped ? 0.30 : 0.65)), scale, flipped) + (int)(scale * 0.1)));

            tickLabel = max.ToString("0.") + unitLabel;
            tickLabelOffset = (flipped ? g.MeasureString(tickLabel, font).Width : 0);
            g.DrawString(tickLabel, font, brush, new Point(
                (int)(scale * (flipped ? 1.2 : 0.25) - tickLabelOffset),
                (int)CalcArcCurve((scale * (flipped ? 1.0 : 0.0)), scale, flipped) + (int)(scale * 0.1)));

            var labelString = Label.Substring(0, (Label.Length > 10 ? 10 : Label.Length));
            var stringLen = g.MeasureString(labelString, font);
            g.DrawString(labelString, font, brush,
                new Point((int)((scale * (flipped ? 1.5 : 0.1)) - (flipped ? stringLen.Width : 0)), (int)(scale * 1.1)));


            // temp
            g.DrawString(val.ToString("0."), font, brush,
                new Point((int)((scale * (flipped ? 1.5 : 0.1)) - (flipped ? stringLen.Width : 0)), (int)(scale * 0.5)));


            // linear
            double scaledX = (val - min) / (max - min);
            // piecemeal scaling for non-linear gauge movement
            if(scaledX < 0.5)
            {
                // 0 - 38
                scaledX = scaledX * 38d / 50d;
            }
            else if(scaledX < 0.94 )
            {
                // 39 - 88
                scaledX = scaledX * 50d / 44d - 0.19;
            }
            else
            {
                // 89 - 100
                scaledX = scaledX * 1.8d - 0.8;
            }

            scaledX *= scale;

            // exponential
            //var scaledX = (scale/100) * (24 * Math.Pow(1.0163, 100 * (val - min) / (max - min)) - 21);

            float indicatorX = (float)(flipped ? scaledX : scale - scaledX);
            float indicatorY = (float)CalcArcCurve(indicatorX, scale, flipped);

            g.DrawLine(pen,
                indicatorX + (float)(scale * (flipped ? 0.31 : 0.01)),
                indicatorY + (float)(scale * 0.22),
                indicatorX + (float)(scale * (flipped ? 0.61 : 0.31)),
                indicatorY + (float)(scale * 0.22)
                );

            g.FillEllipse(brushBack,
                indicatorX + (float)(scale * (flipped ? 0.4 : 0.1)),
                indicatorY + (float)(scale * 0.15),
                (float)(scale * 0.12),
                (float)(scale * 0.12));

            g.DrawEllipse(penCircle,
                indicatorX + (float)(scale * (flipped ? 0.4 : 0.1)),
                indicatorY + (float)(scale * 0.15), 
                (float)(scale * 0.12), 
                (float)(scale * 0.12));

            

        }

        private double CalcArcCurve(double x, double scale, bool flip)
        {
            double xOffset = flip ? -1.4 : 0.4;
            double yOffset = 0.4;
            double radius = 2.12;

            xOffset *= scale;
            yOffset *= scale;
            radius = radius * Math.Pow(scale, 2);

            double y = Math.Sqrt(radius - Math.Pow(x + xOffset, 2)) - yOffset;

            return scale - y;
        }
    }
}
