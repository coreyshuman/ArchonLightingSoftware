using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace ArchonLightingSDKCommon.Models
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct SdkLightData
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)]
        public SdkLightColor[] Lights;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct SdkLightColor
    {
        public byte Red;
        public byte Green;
        public byte Blue;
        public byte Alpha;
    }
}
