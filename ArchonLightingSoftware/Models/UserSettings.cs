using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using ArchonLightingSystem.Bootloader;
using ArchonLightingSystem.Common;
using static ArchonLightingSystem.Bootloader.FirmwareUpdateManager;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

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
        public string ComputerName { get; set; } = "XxX";
        public VersionXml SoftwareVersion { get; set; } = new Version(0,0);
        public VersionXml LatestFirmwareVersion { get; set; } = new Version(0, 0);

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
            try
            {
                XmlSerializer xs = new XmlSerializer(typeof(UserSettings));
                using (TextWriter tw = new StreamWriter(UserSettingsFilePath))
                {
                    xs.Serialize(tw, settings);
                    tw.Close();
                }
            }
            catch(Exception ex)
            {
                Logger.Write(Level.Error, $"Failed to write settings file: {ex.Message}");
            }
        }

        public UserSettings GetSettings()
        {
            XmlSerializer xs = new XmlSerializer(typeof(UserSettings));
            try
            {
                using (var sr = new StreamReader(UserSettingsFilePath))
                {
                    var settings = (UserSettings)xs.Deserialize(sr);
                    sr.Close();
                    settings.Manager = this;
                    LoadPlatformVariables(settings);
                    return settings;
                }
            }
            catch (Exception ex)
            {
                Logger.Write(Level.Error, $"Failed to open settings file: {ex.Message}");
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
            LoadPlatformVariables(settings);
            return settings;
        }

        private void LoadPlatformVariables(UserSettings settings)
        {
            bool needsSave = false;
            if(settings.SoftwareVersion < Definitions.SoftwareVersion)
            {
                // todo can run migrations on settings file
                Logger.Write(Level.Trace, $"Update settings software version from {settings.SoftwareVersion} to {Definitions.SoftwareVersion}");
                settings.SoftwareVersion = Definitions.SoftwareVersion;
                needsSave = true;
            }
            Logger.Write(Level.Information, $"Software Version: {settings.SoftwareVersion}");

            if (settings.ComputerName != System.Environment.MachineName)
            {
                Logger.Write(Level.Trace, $"Update settings computer name from {settings.ComputerName} to {System.Environment.MachineName}");
                settings.ComputerName= System.Environment.MachineName;
                needsSave = true;
            }
            Logger.Write(Level.Information, $"Computer Name: {settings.ComputerName}");

            try
            {
                var bootloader = new Bootloader.Bootloader();
                bootloader.InitializeBootloader(null, null);
                if (bootloader.LoadHexFile(AppDomain.CurrentDomain.BaseDirectory + @"FirmwareBinaries\latest.hex"))
                {
                    var firmwareCRC = bootloader.CalculateFlashCRC();
                    var firmwareVer = bootloader.GetApplicationVersion();
                    if (settings.LatestFirmwareVersion != firmwareVer)
                    {
                        Logger.Write(Level.Trace, $"Update settings firmware version from {settings.LatestFirmwareVersion} to {firmwareVer}");
                        settings.LatestFirmwareVersion = firmwareVer;
                        needsSave = true;
                    }
                    Logger.Write(Level.Information, $"Latest firmware version: {settings.LatestFirmwareVersion}");
                }
            }
            catch (Exception ex)
            {
                Logger.Write(Level.Error, $"Couldn't open latest firmware, an error occurred: {ex.Message}");
                settings.LatestFirmwareVersion = new Version(0,0);
            }

            if(needsSave)
            {
                SaveSettings(settings);
            }
        }
    }
}
