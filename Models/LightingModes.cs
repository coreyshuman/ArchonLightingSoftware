﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArchonLightingSystem.Models
{
    class LightingModes
    {
        private static readonly string[] modes = new string[] { "Off", "Steady", "Rotate", "Breath" };
        private static readonly string[] speeds = new string[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10" };

        public static string[] GetLightingModes()
        {
            return modes;
        }

        public static string[] GetLightingSpeeds()
        {
            return speeds;
        }
    }
}