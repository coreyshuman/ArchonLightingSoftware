using Microsoft.VisualBasic.Logging;
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
    public enum GaugeTachoGear
    {
        Neutral = 0,
        One = 1,
        Two = 2,
        Three = 3,
        Four = 4,
        Five = 5,
        Six = 6
    }

    public partial class GaugeTacho : UserControl
    {
        const int defaultWidth = 480;
        const int defaultHeight = 420;

        private double val = 0;
        private GaugeTachoGear gear = GaugeTachoGear.Neutral;
        private double[] valBuf;
        private float scaleX = defaultWidth;
        private float scaleY = defaultHeight;
        private double min = 0;
        private double max = 100;
        private double overload = 100;
        private string unitLabel;
        private Color lineColor;
        private Color overloadColor;
        private Color tickColor;
        private Color frameColor;
        private Color pointerColor;
        private Color selectorColor;
        private String label;
        private Icon icon = null;

        public GaugeTacho()
        {
            InitializeComponent();

            Initialize();
        }

        public GaugeTacho(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
            Initialize();
        }

        private void Initialize()
        {
            this.SetAutoSizeMode(AutoSizeMode.GrowOnly);
            base.Width = defaultWidth;
            base.Height = defaultHeight;
            base.MinimumSize = new Size(this.Width, this.Height);
            this.BackColor = Color.Black;
            this.lineColor = Color.White;
            this.overloadColor = Color.Red;
            this.tickColor = Color.LightGreen;
            this.frameColor = Color.OrangeRed;
            this.pointerColor = Color.Cornsilk;
            this.selectorColor = Color.DarkGray;
            this.DoubleBuffered = true;
            valBuf = new double[20];
        }

        /// <summary>
        /// Set the value of the gauge.
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
                    SetValue(min);
                }
                else if(value > max)
                {
                    SetValue(max);
                }
                else
                {
                    SetValue(value);
                }
                Invalidate();
            }
        }

        /// <summary>
        /// Set the value of the gauge.
        /// </summary>
        public GaugeTachoGear Gear
        {
            get
            {
                return gear;
            }
            set
            {
                gear = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Set the minimum allowed value for the gauge.
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
                if (max < value)
                {
                    if (overload == max) overload = value + 1;
                    max = value + 1;
                }
                if (overload <= value || overload == min)
                {
                    overload = value;
                }

                min = value;
                FillValueBuffer(min);
                Invalidate();
            }
        }

        /// <summary>
        /// Set the maximum allowed value for the gauge.
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
                if (overload >= value || overload == max)
                {
                    overload = value;
                }

                max = value;
                FillValueBuffer(min);
                Invalidate();
            }
        }

        /// <summary>
        /// Set the warning range at the top of the gauge.
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
                
                base.Width = value;
                base.Height = (int)(value * defaultHeight / defaultWidth);
                scaleX = (float)base.Width / (float)defaultWidth * defaultWidth;
                scaleY = (float)base.Height / (float)defaultHeight * defaultHeight;
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

                base.Height = value;
                base.Width = (int)(value * defaultWidth / defaultHeight);
                scaleX = (float)base.Width / (float)defaultWidth * defaultWidth;
                scaleY = (float)base.Height / (float)defaultHeight * defaultHeight;
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
        /// The image shown in the corner of the gauge.
        /// </summary>
        public Icon Image
        {
            get
            {
                return icon;
            }
            set
            {
                icon = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Primary color of the gauge's drawn arc.
        /// </summary>
        public Color LineColor
        {
            get
            {
                return lineColor;
            }
            set
            {
                lineColor = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Warning color of the gauge's drawn arc in the overload region.
        /// </summary>
        public Color OverLineColor
        {
            get
            {
                return overloadColor;
            }
            set
            {
                overloadColor = value;
                Invalidate();
            }
        }

        public String Label
        {
            get
            {
                return label;
            }
            set
            {
                label = value;
                Invalidate();
            }
        }

        private void GaugeTacho_Paint(object sender, PaintEventArgs e)
        {
            int i;
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            //Pen pen = new Pen(this.ForeColor, (scaleX / 100f));
            Pen penFrame = new Pen(this.frameColor, (scaleX / 150f));
            Pen penDash = new Pen(this.tickColor, (scaleX / 200f));
            Pen penDashOverload = new Pen(this.overloadColor, (scaleX / 200f));
            Pen penTach = new Pen(this.tickColor, (scaleX / 80f));
            Pen penTachOverload = new Pen(this.overloadColor, (scaleX / 80f));
            Pen penPointer = new Pen(this.pointerColor, (scaleX / 125f));
            Pen penPointerCircle = new Pen(this.pointerColor, (float)(scaleX / 300f));
            Pen penSelectorCircle = new Pen(this.selectorColor, (float)(scaleX / 250f));
            Pen penSelectorCircleEnable = new Pen(this.tickColor, (float)(scaleX / 250f));
            Pen penLine = new Pen(this.LineColor, (float)(scaleX / 35d));
            Font font = new Font("monospace", (scaleX / 22f), FontStyle.Bold);
            Font fontSelected = new Font("monospace", (scaleX / 20f), FontStyle.Bold);
            SolidBrush brush = new SolidBrush(this.ForeColor);
            SolidBrush brushSelector = new SolidBrush(this.selectorColor);
            SolidBrush brushOverload = new SolidBrush(this.overloadColor);
            SolidBrush brushBack = new SolidBrush(this.BackColor);


            float frameTopX = 0.32f;
            float frameTopY = 0.9f;
            float frameBottomX = 0.1f;
            float frameBottomY = 0.1f;
            float tachBarOffsetX = 0.05f;
            float tachPointerSize = 0.05f;
            float tachPointerCircleSize = 0.012f;
            float numeralOffsetX = -0.003f;
            float numeralOffsetY = 0.05f;
            float dashOffsetX = 0.01f;
            float dashOffsetY = 0.0f;
            float dashLength = 0.02f;

            float selectorBottomMargin = 0.01f;
            float selectorTopMargin = 0.005f;
            float selectorCircleSize = 0.08f;
            float selectorEnabledCircleSize = 0.1f;

            // draw outer frame
            g.DrawLine(penFrame, scaleX * frameBottomX, scaleY * (1-frameBottomY), scaleX * frameTopX, scaleY * (1-frameTopY));
            g.DrawLine(penFrame, scaleX * (1-frameBottomX), scaleY * (1 - frameBottomY), scaleX * (1-frameTopX), scaleY * (1 - frameTopY));

            int numDivisions = 10;
            // draw numerals
            for(i = 1; i <= numDivisions; i++)
            {
                float numX = (i / (float)numDivisions) * (frameTopX - frameBottomX) + frameBottomX;
                float numY = (i / (float)numDivisions) * (frameTopY - frameBottomY) + frameBottomY;
                Brush textBrush = (i < 9 ? brush : brushOverload);

                string digitStr = i.ToString();
                float digitLen = g.MeasureString(digitStr, font).Width;
                g.DrawString(digitStr, font, textBrush, scaleX * (numX + numeralOffsetX) - digitLen, scaleY * (1 - numY - numeralOffsetY));
                g.DrawString(digitStr, font, textBrush, scaleX * (1 - numX - numeralOffsetX), scaleY * (1 - numY - numeralOffsetY));
            }

            // draw numeral ticks
            for (i = 0; i <= numDivisions; i++)
            {
                float numX = (i / (float)numDivisions) * (frameTopX - frameBottomX) + frameBottomX;
                float numY = (i / (float)numDivisions) * (frameTopY - frameBottomY) + frameBottomY;
                Pen pen = (i < 9 ? penDash : penDashOverload);

                g.DrawLine(pen,
                    scaleX * (numX + dashOffsetX),
                    scaleY * (1 - numY - dashOffsetY),
                    scaleX * (numX + dashOffsetX + dashLength),
                    scaleY * (1 - numY - dashOffsetY)
                );

                g.DrawLine(pen,
                    scaleX * (1 - numX - dashOffsetX),
                    scaleY * (1 - numY - dashOffsetY),
                    scaleX * (1 - numX - dashOffsetX - dashLength),
                    scaleY * (1 - numY - dashOffsetY)
                );
            }

            // draw tach value bar
            var penTachVal = (penTach);
            float valueY = 0.5f * (frameTopY - frameBottomY) + frameBottomY;
            float valueX = 0.5f * (frameTopX - frameBottomX) + frameBottomX;
            float valueCircleX1 = valueX + tachBarOffsetX + tachPointerSize;
            float valueCircleX2 = 1 - valueX - tachBarOffsetX - tachPointerSize - tachPointerCircleSize;
            float valueCircleY = (1 - valueY - 0.5f * tachPointerCircleSize);
            g.DrawLine(penTachVal, scaleX * (frameBottomX + tachBarOffsetX), scaleY * (1 - frameBottomY), scaleX * (valueX + tachBarOffsetX), scaleY * (1 - valueY + 0.8f * tachPointerCircleSize));
            g.DrawLine(penTachVal, scaleX * (1 - frameBottomX - tachBarOffsetX), scaleY * (1 - frameBottomY), scaleX * (1 - valueX - tachBarOffsetX), scaleY * (1 - valueY + 0.8f * tachPointerCircleSize));

            // draw tach value pointer
            g.DrawLine(penPointer,
                scaleX * (valueX + tachBarOffsetX),
                scaleY * (1 - valueY),
                scaleX * (valueX + tachBarOffsetX + tachPointerSize),
                scaleY * (1 - valueY)
            );

            g.DrawEllipse(penPointerCircle,
                scaleX * valueCircleX1,
                scaleY * valueCircleY,
                scaleX * tachPointerCircleSize,
                scaleY * tachPointerCircleSize
            );

            g.DrawLine(penPointer,
                scaleX * (1 - valueX - tachBarOffsetX),
                scaleY * (1 - valueY),
                scaleX * (1 - valueX - tachBarOffsetX - tachPointerSize),
                scaleY * (1 - valueY)
            );

            g.DrawEllipse(penPointerCircle,
                scaleX * valueCircleX2,
                scaleY * valueCircleY,
                scaleX * tachPointerCircleSize,
                scaleY * tachPointerCircleSize
            );

            // draw selector circles and text
            int selectorCount = 7;
            int selected = 1;
            for(i = 0; i < selectorCount; i++)
            {
                float circleSize = (selected == i ? selectorEnabledCircleSize : selectorCircleSize);
                float sdy = (1 - selectorBottomMargin - selectorTopMargin) / selectorCount;
                float sY = 1 - (i * sdy) - selectorEnabledCircleSize + selectorTopMargin - selectorBottomMargin - 0.5f * (selected == i ? selectorEnabledCircleSize - selectorCircleSize : 0);
                float sX = 0.5f - 0.5f * circleSize;

                var digitFont = (i == selected ? fontSelected : font);
                string digitStr = (i < 1 ? "N" : i.ToString());
                var digitSize = g.MeasureString(digitStr, digitFont);
                Brush selectorBrush = (i == selected ? brush : brushSelector);
                Pen penSelCir = (i == selected ? penSelectorCircleEnable : penSelectorCircle);

                g.DrawEllipse(penSelCir,
                    scaleX * sX,
                    scaleY * sY,
                    scaleX * circleSize,
                    scaleY * circleSize
                );

                g.DrawString(digitStr, digitFont, selectorBrush, 
                    scaleX * sX + 0.5f * (circleSize * scaleX - digitSize.Width), 
                    scaleY * sY + 0.5f * (circleSize * scaleY - digitSize.Height)
                );

                // draw pointer
                if(i == selected)
                {
                    g.DrawLine(penPointer,
                        scaleX * (sX),
                        scaleY * (sY + 0.5f * circleSize),
                        scaleX * (sX - tachPointerSize),
                        scaleY * (sY + 0.5f * circleSize)
                    );

                    g.DrawEllipse(penPointerCircle,
                        scaleX * (sX - tachPointerSize - tachPointerCircleSize),
                        scaleY * (sY + 0.5f * circleSize - 0.5f * tachPointerCircleSize),
                        scaleX * tachPointerCircleSize,
                        scaleY * tachPointerCircleSize
                    );

                    g.DrawLine(penPointer,
                        scaleX * (sX + circleSize),
                        scaleY * (sY + 0.5f * circleSize),
                        scaleX * (sX + circleSize + tachPointerSize),
                        scaleY * (sY + 0.5f * circleSize)
                    );

                    g.DrawEllipse(penPointerCircle,
                        scaleX * (sX + circleSize + tachPointerSize),
                        scaleY * (sY + 0.5f * circleSize - 0.5f * tachPointerCircleSize),
                        scaleX * tachPointerCircleSize,
                        scaleY * tachPointerCircleSize
                    );

                    // connect tach and selector pointers
                    g.DrawLine(penPointer,
                        scaleX * (sX - tachPointerSize),
                        scaleY * (sY + 0.5f * circleSize),
                        scaleX * valueCircleX1,
                        scaleY * valueCircleY
                    );

                    g.DrawLine(penPointer,
                        scaleX * (sX + circleSize + tachPointerSize),
                        scaleY * (sY + 0.5f * circleSize),
                        scaleX * valueCircleX2,
                        scaleY * valueCircleY
                    );
                }
                
            }
        }

        private void SetValue(double v)
        {
            int i;
            double total = 0;

            if (true)
            {
                for (i = valBuf.Length - 1; i > 0; i--)
                {
                    valBuf[i] = valBuf[i - 1];
                }
                valBuf[0] = v;
                for (i = 0; i < valBuf.Length; i++)
                {
                    total += valBuf[i];
                }
                val = total / valBuf.Length;
            }

            Invalidate();
        }

        private void FillValueBuffer(double v)
        {
            int i;

            for (i = valBuf.Length - 1; i > 0; i--)
            {
                valBuf[i] = v;
            }
        }
    }
}
