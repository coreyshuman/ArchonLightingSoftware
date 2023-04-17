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
using System.Reflection;
using System.Net;

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
            Default(0);
        }

        public DeviceSettings(int index)
        {
            Default(index);
            DefaultFanCurve();
        }

        public void Default(int index)
        {
            this.Index = index;
            Name = $"Device {index + 1}";
            Sensor = string.Empty;
            SensorName = string.Empty;
            UseFanCurve = false;
        }

        private void DefaultFanCurve()
        {
            FanCurveValues.Clear();
            FanCurveValues.Add(30);
            FanCurveValues.Add(30);
            FanCurveValues.Add(30);
            FanCurveValues.Add(30);
            FanCurveValues.Add(30);
            FanCurveValues.Add(40);
            FanCurveValues.Add(60);
            FanCurveValues.Add(80);
            FanCurveValues.Add(100);
            FanCurveValues.Add(100);
            FanCurveValues.Add(100);
        }

        public void Validate()
        {
            lock(FanCurveValues)
            {
                if(FanCurveValues.Count != 11)
                {
                    Logger.Write(Level.Debug, $"Settings Device {Index} curve out of range, setting default values.");
                    DefaultFanCurve();
                    return;
                }

                for(int i = 0; i < FanCurveValues.Count; i++)
                {
                    if (FanCurveValues[i] < 0) FanCurveValues[i] = 0;
                    if (FanCurveValues[i] > 100) FanCurveValues[i] = 100;
                }
            }
        }
    }

    [Serializable]
    public class ControllerSettings
    {
        [XmlAttribute]
        public int Address { get; set; }
        public string Name { get; set; }
        public bool AlertOnDisconnect { get; set; }
        public List<DeviceSettings> Devices { get; } = new List<DeviceSettings>();

        public ControllerSettings()
        {
            Default(0);
        }

        public ControllerSettings(int index)
        {
            Default(index);
        }

        public DeviceSettings GetDeviceByIndex(int deviceIndex)
        {
            return Devices.Where(d => d.Index == deviceIndex).FirstOrDefault();
        }

        public void Default(int index)
        {
            Address = index;
            Name = $"Controller {index + 1}";
        }

        public void Validate()
        {
            lock(Devices)
            {
                Devices.RemoveAll(d => d.Index >= DeviceControllerDefinitions.DevicePerController);
                Devices.RemoveAll(d => d.Index < 0);

                for (int i = 0; i < DeviceControllerDefinitions.DevicePerController; i++)
                {
                    int count = Devices.Where(d => d.Index == i).Count();
                    if (count == 0)
                    {
                        Logger.Write(Level.Debug, $"Settings Controller {Address} Device {i} adding default.");
                        Devices.Add(new DeviceSettings(i));
                    }
                    else if(count > 1)
                    {
                        while(Devices.Where(d => d.Index == i).Count() > 1)
                        {
                            Logger.Write(Level.Debug, $"Settings Controller {Address} Device {i} removing duplicate.");
                            Devices.Remove(Devices.Where(d => d.Index == i).Last());
                        }
                    }
                    Devices.Where(d => d.Index == i).First().Validate();
                }
            }
        }
    }

    [Serializable]
    public class UserSettings
    {
        [XmlIgnore]
        public UserSettingsManager Manager { get; set; } = null;

        public List<ControllerSettings> Controllers { get; private set; } = new List<ControllerSettings>();

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

        public void Validate()
        {
            lock (Controllers)
            {
                Controllers.RemoveAll(d => d.Address >= DeviceControllerDefinitions.MaxControllers);
                Controllers.RemoveAll(d => d.Address < 0);

                for (int i = 0; i < DeviceControllerDefinitions.MaxControllers; i++)
                {
                    int count = Controllers.Where(d => d.Address == i).Count();
                    if (count == 0)
                    {
                        Logger.Write(Level.Debug, $"Settings Controller {i} adding default.");
                        Controllers.Add(new ControllerSettings(i));
                    }
                    else if (count > 1)
                    {
                        while (Controllers.Where(d => d.Address == i).Count() > 1)
                        {
                            Logger.Write(Level.Debug, $"Settings Controller {i} removing duplicate.");
                            Controllers.Remove(Controllers.Where(d => d.Address == i).Last());
                        }
                    }
                    Controllers.Where(d => d.Address == i).LastOrDefault().Validate();
                }
            }
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
                    settings.Validate();
                    settings.Manager = this;
                    LoadPlatformVariables(settings);
                    return settings;
                }
            }
            catch (Exception ex)
            {
                Logger.Write(Level.Error, $"Failed to open settings file: {ex.Message} {ex.InnerException?.Message}");
            }
            return GetDefaultSettings();
        }

        private UserSettings GetDefaultSettings()
        {
            var settings = new UserSettings();
            settings.Validate();
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
                Logger.Write(Level.Debug, $"Update settings software version from {settings.SoftwareVersion} to {Definitions.SoftwareVersion}");
                settings.SoftwareVersion = Definitions.SoftwareVersion;
                needsSave = true;
            }
            Logger.Write(Level.Information, $"Software Version: {settings.SoftwareVersion}");

            if (settings.ComputerName != System.Environment.MachineName)
            {
                Logger.Write(Level.Debug, $"Update settings computer name from {settings.ComputerName} to {System.Environment.MachineName}");
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
                        Logger.Write(Level.Debug, $"Update settings firmware version from {settings.LatestFirmwareVersion} to {firmwareVer}");
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
