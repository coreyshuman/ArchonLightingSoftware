using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace ArchonLightingSystem.Models
{
    [Serializable]
    public class DeviceSettings
    {
        [XmlAttribute]
        public int Index { get; set; }
        public string Name { get; set; }
        public string Sensor { get; set; }
        public string SensorName { get; set; }
        public List<int> FanCurveValues { get; set; } = new List<int>();
        public bool UseFanCurve { get; set; }

        public DeviceSettings()
        {
            
        }

        public void SetFanCurveToDefault()
        {
            FanCurveValues = new List<int>();
            FanCurveValues.Add(10);
            FanCurveValues.Add(10);
            FanCurveValues.Add(10);
            FanCurveValues.Add(10);
            FanCurveValues.Add(20);
            FanCurveValues.Add(40);
            FanCurveValues.Add(60);
            FanCurveValues.Add(80);
            FanCurveValues.Add(100);
            FanCurveValues.Add(100);
            FanCurveValues.Add(100);
        }
    }

    [Serializable]
    public class ControllerSettings
    {
        [XmlAttribute]
        public int Address { get; set; }
        public string Name { get; set; }
        public List<DeviceSettings> Devices { get; } = new List<DeviceSettings>();
        

        public ControllerSettings()
        {
            
        }

        public DeviceSettings GetDeviceByIndex(int deviceIndex)
        {
            return Devices.Where(d => d.Index == deviceIndex).FirstOrDefault();
        }
    }

    [Serializable]
    public class UserSettings
    {
        [XmlIgnore]
        public UserSettingsManager Manager { get; set; } = null;

        public List<ControllerSettings> Controllers { get; private set; } = new List<ControllerSettings>();

        //public ControllerSettings[] Controllers { get; set; } = new ControllerSettings[DeviceControllerDefinitions.MaxControllers];
        public string ComputerName { get; set; } = "Archon";
        public string SoftwareVersion { get; set; } = ".101";
        public string LatestFirmwareVersion { get; set; } = "1.10";

        public UserSettings()
        {
            
        }

        public void Save()
        {
            Manager?.SaveSettings(this);
        }

        public void RevertChanges()
        {
            var revertedSettings = Manager?.GetSettings();
            Controllers = revertedSettings.Controllers;
        }

        public ControllerSettings GetControllerByAddress(int address)
        {
            return Controllers.Where(c => c.Address == address).FirstOrDefault();
        }
    }

    public class UserSettingsManager
    {
        public static string UserSettingsFilePath { get; } = AppDomain.CurrentDomain.BaseDirectory + "settings.xml";

        public void SaveSettings(UserSettings settings)
        {
            XmlSerializer xs = new XmlSerializer(typeof(UserSettings));
            TextWriter tw = new StreamWriter(UserSettingsFilePath);
            xs.Serialize(tw, settings);
        }

        public UserSettings GetSettings()
        {
            XmlSerializer xs = new XmlSerializer(typeof(UserSettings));
            try
            {
                using (var sr = new StreamReader(UserSettingsFilePath))
                {
                    var settings = (UserSettings)xs.Deserialize(sr);
                    settings.Manager = this;
                    return settings;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
            }
            return GetDefaultSettings();
        }

        private UserSettings GetDefaultSettings()
        {
            var settings = new UserSettings();
            for (int i = 0; i < DeviceControllerDefinitions.MaxControllers; i++)
            {
                var controller = new ControllerSettings() { Address = i, Name = "Controller" + (i + 1) };
                for (int j = 0; j < DeviceControllerDefinitions.DevicePerController; j++)
                {
                    controller.Devices.Add(new DeviceSettings() { Index = j });
                }
                controller.Devices.ForEach((devSet) => devSet.SetFanCurveToDefault());
                settings.Controllers.Add(controller);
            }
            settings.Manager = this;
            return settings;
        }
    }
}
