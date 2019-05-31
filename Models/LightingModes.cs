﻿
namespace ArchonLightingSystem.Models
{
    class LightingModes
    {
        private static readonly string[] modes = new string[] { "Off", "Solid", "Rotate", "Breathe", "Ripple" };
        private static readonly string[] speeds = new string[] { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "20" };

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
