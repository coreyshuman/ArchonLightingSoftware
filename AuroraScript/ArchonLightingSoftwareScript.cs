using Aurora;
using Aurora.Devices;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using ArchonLightingSDK;
using ArchonLightingSDKCommon;
using ArchonLightingSDKCommon.Models;

public class ArchonLightingSoftwareScript
{
    public string devicename = "Archon Lighting Software";
    public bool enabled = true;

    //private Color device_color = Color.Black;

    public bool Initialize()
    {
        try
        {
            //Perform necessary actions to initialize your device
            return true;
        }
        catch (Exception exc)
        {
            return false;
        }
    }

    public void Reset()
    {
        //Perform necessary actions to reset your device
    }

    public void Shutdown()
    {
        //Perform necessary actions to shutdown your device
    }

    public bool UpdateDevice(Dictionary<DeviceKeys, Color> keyColors, bool forced)
    {
        try
        {
            var archonData = new SdkLightData();
            archonData.Lights = new SdkLightColor[12];
            archonData.Lights[0] = ColorToSdkColor(keyColors[DeviceKeys.MOUSEPADLIGHT1]);
            archonData.Lights[1] = ColorToSdkColor(keyColors[DeviceKeys.MOUSEPADLIGHT2]);
            archonData.Lights[2] = ColorToSdkColor(keyColors[DeviceKeys.MOUSEPADLIGHT3]);
            archonData.Lights[3] = ColorToSdkColor(keyColors[DeviceKeys.MOUSEPADLIGHT4]);
            archonData.Lights[4] = ColorToSdkColor(keyColors[DeviceKeys.MOUSEPADLIGHT5]);
            archonData.Lights[5] = ColorToSdkColor(keyColors[DeviceKeys.MOUSEPADLIGHT6]);
            archonData.Lights[6] = ColorToSdkColor(keyColors[DeviceKeys.MOUSEPADLIGHT7]);
            archonData.Lights[7] = ColorToSdkColor(keyColors[DeviceKeys.MOUSEPADLIGHT8]);
            archonData.Lights[8] = ColorToSdkColor(keyColors[DeviceKeys.MOUSEPADLIGHT9]);
            archonData.Lights[9] = ColorToSdkColor(keyColors[DeviceKeys.MOUSEPADLIGHT10]);
            archonData.Lights[10] = ColorToSdkColor(keyColors[DeviceKeys.MOUSEPADLIGHT11]);
            archonData.Lights[11] = ColorToSdkColor(keyColors[DeviceKeys.MOUSEPADLIGHT12]);

            ArchonLighting.SendLights(archonData);
            return true;
        }
        catch (Exception exc)
        {
            return false;
        }
    }

    private SdkLightColor ColorToSdkColor(Color color)
    {
        var light = new SdkLightColor();
        light.Red = color.R;
        light.Green = color.G;
        light.Blue = color.B;
        light.Alpha = color.A;
        return light;
    }
}