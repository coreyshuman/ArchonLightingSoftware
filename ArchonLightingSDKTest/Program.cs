using System;
using ArchonLightingSDK;
using ArchonLightingSDKCommon;
using ArchonLightingSDKCommon.Models;

namespace ArchonLightingSDKTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Writing to Archon SDK.");

            var lightData = new SdkLightData();
            lightData.Lights = new SdkLightColor[12];
            var rand = new Random();

            for (int i = 0; i < 12; i++) {
                lightData.Lights[i] = new SdkLightColor();
                lightData.Lights[i].Red = 0;
                lightData.Lights[i].Green = 255;
                lightData.Lights[i].Blue = 0;
                lightData.Lights[i].Alpha = 255;
            }

            ArchonLighting.SendLights(lightData);

            Console.WriteLine("Done!");
        }
    }
}
