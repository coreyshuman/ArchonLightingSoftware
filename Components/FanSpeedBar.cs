using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace ArchonLightingSystem.Components
{
    public class FanSpeedBar
    {
        private UInt16[] fanBuffer;
        private Panel container;
        private Panel bar;
        private Label lblValue;
        private int maximum;
        private int minimum;
        private int value;
        private bool useAverage;

        public int Top
        {
            get
            {
                return container.Top;
            }
            set
            {
                container.Top = value;
            }
        }

        public int Left
        {
            get
            {
                return container.Left;
            }
            set
            {
                container.Left = value;
            }
        }

        public int Width
        {
            get
            {
                return container.Width;
            }
            set
            {
                container.Width = value;
                bar.Width = value;
                lblValue.Width = value;
            }
        }

        public int Height
        {
            get
            {
                return container.Height;
            }
            set
            {
                container.Height = value;
                SetFanSpeedValue(this.value);
            }
        }

        public Color BackgroundColor
        {
            get
            {
                return container.BackColor;
            }
            set
            {
                container.BackColor = value;
            }
        }

        public Color BarColor
        {
            get
            {
                return bar.BackColor;
            }
            set
            {
                bar.BackColor = value;
            }
        }

        public Color ForeColor
        {
            get
            {
                return lblValue.ForeColor;
            }
            set
            {
                lblValue.ForeColor = value;
            }
        }

        public Control Parent
        {
            get
            {
                return container.Parent;
            }
            set
            {
                container.Parent = value;
            }
        }

        public int Maximum
        {
            get
            {
                return maximum;
            }
            set
            {
                maximum = value;
                if(minimum > maximum)
                {
                    throw new Exception("Maximum cannot be smaller than minimum.");
                }
                SetFanSpeedValue(this.value);
            }
        }

        public int Minimum
        {
            get
            {
                return minimum;
            }
            set
            {
                minimum = value;
                if (minimum > maximum)
                {
                    throw new Exception("Minimum cannot be greater than maximum.");
                }
                SetFanSpeedValue(this.value);
            }
        }

        public int Value
        {
            get
            {
                return value;
            }
            set
            {
                SetFanSpeedValue(value);
            }
        }

        public BorderStyle BorderStyle
        {
            get
            {
                return container.BorderStyle;
            }
            set
            {
                container.BorderStyle = value;
            }
        }

        public bool UseAverage
        {
            get
            {
                return useAverage;
            }
            set
            {
                useAverage = value;
                SetFanSpeedValue(this.value);
            }
        }

        public FanSpeedBar()
        {
            container = new Panel();
            bar = new Panel();
            lblValue = new Label();
            fanBuffer = new UInt16[10];

            maximum = 100;
            minimum = 0;
            Top = 0;
            Left = 0;
            Width = 10;
            Height = 40;
            bar.Left = 0;
            lblValue.Left = 0;
            lblValue.Top = 0;
            lblValue.Height = 20;
            lblValue.Font = new Font("Microsoft San Serif", 7);
            lblValue.Text = maximum.ToString();
            lblValue.TextAlign = ContentAlignment.MiddleCenter;
            lblValue.Parent = container;
            lblValue.BackColor = Color.Transparent;
            lblValue.Parent = bar;
            
            BarColor = Color.Blue;
            bar.Parent = container;
        }

        public void Show()
        {
            container.Show();
            bar.Show();
        }

        public void Hide()
        {
            container.Hide();
            bar.Hide();
        }

        public void SetFanSpeedValue(int v)
        {
            int i;
            int total = 0;
            if (value > maximum) v = maximum;
            value = v;

            if (useAverage)
            {
                for (i = 9; i > 0; i--)
                {
                    fanBuffer[i] = fanBuffer[i - 1];
                }
                fanBuffer[0] = (UInt16)value;
                for (i = 0; i < 10; i++)
                {
                    total += fanBuffer[i];
                }
                value = total / 10;
            }

            int size = (int)((double)container.Height * (((double)value - (double)minimum) / ((double)maximum - (double)minimum)));
            int top = container.Height - size;
            bar.Height = size;
            bar.Top = top;
            lblValue.Text = value.ToString();
        }
    }
}
