using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArchonLightingSystem.WindowsSystem
{
    public enum WindowsVersion
    {
        Unknown,
        Win7,
        Win8,
        Win10,
        Win11
    }

    public class WindowsSystem
    {
        public static WindowsVersion WindowsVersion
        {
            get
            {
                var version = Environment.OSVersion.Version;
                if (version.Major == 6 && version.Minor == 1)
                {
                    return WindowsVersion.Win7;
                }
                else if (version.Major == 6 && (version.Minor == 2 || version.Minor == 3))
                {
                    return WindowsVersion.Win8;
                }
                else if (version.Major == 10)
                {
                    if (version.Build >= 22000)
                    {
                        return WindowsVersion.Win11;
                    }
                    else
                    {
                        return WindowsVersion.Win10;
                    }
                }
                else
                {
                    return WindowsVersion.Unknown;
                }
            }
        }
    }
}
