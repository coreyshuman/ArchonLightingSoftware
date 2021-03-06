﻿/*
 
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
 
  Copyright (C) 2009-2012 Michael Möller <mmoeller@openhardwaremonitor.org>
  Copyright (C) 2018-2019 Corey Shuman <ctshumancode@gmail.com>
	
*/

using ArchonLightingSystem.Common;
using ArchonLightingSystem.Properties;
using OpenHardwareMonitor.Hardware;

namespace ArchonLightingSystem.OpenHardware
{
    public class HardwareTypeImage
    {
        private static HardwareTypeImage instance = new HardwareTypeImage();

        private HardwareTypeImage() { }

        public static HardwareTypeImage Instance
        {
            get { return instance; }
        }

        public string GetImageKey(HardwareType hardwareType)
        {
            string image = "";

            switch (hardwareType)
            {
                case HardwareType.CPU:
                    image = Util.GetPropertyName(() => Resources .cpu);
                    break;
                case HardwareType.GpuNvidia:
                    image = Util.GetPropertyName(() => Resources .nvidia);
                    break;
                case HardwareType.GpuAti:
                    image = Util.GetPropertyName(() => Resources .ati);
                    break;
                case HardwareType.HDD:
                    image = Util.GetPropertyName(() => Resources .hdd);
                    break;
                case HardwareType.Heatmaster:
                    image = Util.GetPropertyName(() => Resources .bigng);
                    break;
                case HardwareType.Mainboard:
                    image = Util.GetPropertyName(() => Resources .mainboard);
                    break;
                case HardwareType.SuperIO:
                    image = Util.GetPropertyName(() => Resources .chip);
                    break;
                case HardwareType.TBalancer:
                    image = Util.GetPropertyName(() => Resources .bigng);
                    break;
                case HardwareType.RAM:
                    image = Util.GetPropertyName(() => Resources .ram);
                    break;
                case HardwareType.Aquacomputer:
                    image = Util.GetPropertyName(() => Resources .acicon);
                    break;
                case HardwareType.NIC:
                    image = Util.GetPropertyName(() => Resources .nic);
                    break;

            }
            return image;

        }
    }
}
