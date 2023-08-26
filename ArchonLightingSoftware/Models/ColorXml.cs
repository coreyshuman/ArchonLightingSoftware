using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ArchonLightingSystem.Models
{
    public class ColorXml
    {
        private Color color = Color.Black;

        public ColorXml() { }
        public ColorXml(Color c) 
        { 
            color = c;
        }


        public Color ToColor()
        {
            return color;
        }

        public void FromColor(Color c)
        {
            color = c;
        }

        public static implicit operator Color(ColorXml x)
        {
            return x.ToColor();
        }

        public static implicit operator ColorXml(Color c)
        {
            return new ColorXml(c);
        }

        [XmlAttribute]
        public int Red
        {
            get
            {
                return color.R;
            }
            set
            {
                color = Color.FromArgb(value, color.G, color.B);
            }
        }

        [XmlAttribute]
        public int Green
        {
            get
            {
                return color.G;
            }
            set
            {
                color = Color.FromArgb(color.R, value, color.B);
            }
        }

        [XmlAttribute]
        public int Blue
        {
            get
            {
                return color.B;
            }
            set
            {
                color = Color.FromArgb(color.R, color.G, value);
            }
        }
    }
}
