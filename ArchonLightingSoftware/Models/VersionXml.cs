﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ArchonLightingSystem.Models
{
    [Serializable]
    [XmlType("Version")]
    public class VersionXml
    {
        public VersionXml()
        {
            this.Version = null;
        }

        public VersionXml(Version Version)
        {
            this.Version = Version;
        }

        [XmlIgnore]
        public Version Version { get; set; }

        [XmlText]
        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public string Value
        {
            get { return this.Version == null ? string.Empty : this.Version.ToString(); }
            set
            {
                Version temp;
                if(Version.TryParse(value, out temp))
                {
                    this.Version = temp;
                }
            }
        }

        public static implicit operator Version(VersionXml VersionXml)
        {
            return VersionXml.Version;
        }

        public static implicit operator VersionXml(Version Version)
        {
            return new VersionXml(Version);
        }

        public override string ToString()
        {
            return this.Value;
        }
    }
}
