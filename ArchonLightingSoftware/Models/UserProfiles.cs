using ArchonLightingSystem.Common;
using ArchonLightingSystem.OpenHardware;
using ArchonLightingSystem.UsbApplicationV2;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ArchonLightingSystem.Models
{
    [Serializable]
    public class UserProfileDevice
    {
        [XmlAttribute]
        public int Index { get; set; }
        public string LightingMode { get; set; }
        public int LightingSpeed { get; set; }
        [XmlElement(Type = typeof(ColorXml))]
        public List<Color> LightColors { get; } = new List<Color>();

        public UserProfileDevice() { }

        public UserProfileDevice(int index)
        {
            Index = index;
        }

        public bool Validate()
        {
            bool needSave = false;

            lock (LightColors)
            {
                if (LightColors.Count != DeviceControllerDefinitions.LedCountPerDevice)
                {
                    needSave = true;
                    Logger.Write(Level.Debug, $"Profile Device {Index} invalid Light count.");
                    LightColors.Clear();
                    for(int i = 0; i < DeviceControllerDefinitions.LedCountPerDevice; i++)
                    {
                        LightColors.Add(Color.Black);
                    }
                }
            }

            if(LightingSpeed < 1 || LightingSpeed > LightingModes.GetLightingSpeeds().Count())
            {
                needSave = true;
                Logger.Write(Level.Debug, $"Profile Device {Index} invalid Lighting Speed {LightingSpeed}.");
                LightingSpeed = 1;
            }

            if(!LightingModes.GetLightingModes().Contains(LightingMode))
            {
                needSave = true;
                Logger.Write(Level.Debug, $"Profile Device {Index} invalid Lighting Mode {LightingMode}.");
                LightingMode = LightingModes.GetLightingModes().First();
            }

            return needSave;
        }
    }

    [Serializable]
    public class UserProfileController
    {
        [XmlAttribute]
        public int Address { get; set; }
        public List<UserProfileDevice> Devices { get; } = new List<UserProfileDevice>();

        public UserProfileController() { }

        public UserProfileController(int address)
        {
            Address = address;
        }

        public bool Validate()
        {
            bool needSave = false;

            lock (Devices)
            {
                Devices.RemoveAll(d => d.Index >= DeviceControllerDefinitions.DevicePerController);
                Devices.RemoveAll(d => d.Index < 0);

                for (int i = 0; i < DeviceControllerDefinitions.DevicePerController; i++)
                {
                    int count = Devices.Where(d => d.Index == i).Count();
                    if (count == 0)
                    {
                        needSave = true;
                        Logger.Write(Level.Debug, $"Profile Controller {Address} Device {i} adding default.");
                        Devices.Add(new UserProfileDevice(i));
                    }
                    else if (count > 1)
                    {
                        while (Devices.Where(d => d.Index == i).Count() > 1)
                        {
                            needSave = true;
                            Logger.Write(Level.Debug, $"Profile Controller {Address} Device {i} removing duplicate.");
                            Devices.Remove(Devices.Where(d => d.Index == i).Last());
                        }
                    }
                    needSave |= Devices.Where(d => d.Index == i).First().Validate();
                }
            }

            return needSave;
        }
    }

    [Serializable]
    public class UserProfile
    {
        [XmlAttribute]
        public int Index { get; set; }
        [XmlElement(Type = typeof(ColorXml))]
        public Color ProfileColor { get; set; } = Color.Black;
        public List<UserProfileController> Controllers { get; } = new List<UserProfileController>();

        public UserProfile() { }
        public UserProfile(int index)
        {
            Index = index;
        }

        public bool Validate()
        {
            bool needSave = false;

            lock (Controllers)
            {
                Controllers.RemoveAll(d => d.Address >= DeviceControllerDefinitions.MaxControllers);
                Controllers.RemoveAll(d => d.Address < 0);

                for (int i = 0; i < DeviceControllerDefinitions.MaxControllers; i++)
                {
                    int count = Controllers.Where(d => d.Address == i).Count();
                    if (count == 0)
                    {
                        needSave = true;
                        Logger.Write(Level.Debug, $"Profile Controller {i} adding default.");
                        Controllers.Add(new UserProfileController(i));
                    }
                    else if (count > 1)
                    {
                        while (Controllers.Where(d => d.Address == i).Count() > 1)
                        {
                            needSave = true;
                            Logger.Write(Level.Debug, $"Profile Controller {i} removing duplicate.");
                            Controllers.Remove(Controllers.Where(d => d.Address == i).Last());
                        }
                    }
                    needSave |= Controllers.Where(d => d.Address == i).First().Validate();
                }
            }

            return needSave;
        }
    }

    public class UserProfileManager
    {
        private UsbControllerManager usbControllerManager = null;

        public UserProfileManager(UsbControllerManager ucm) 
        { 
            usbControllerManager = ucm;
        }

        public void SaveProfile(int index)
        {
            try
            {
                var profile = usbControllerManager.GetUserSettings().UserProfiles[index];
                SaveProfile(profile);
                usbControllerManager.GetUserSettings().Save();
            }
            catch (Exception ex)
            {
                Logger.Write(Level.Error, $"Failed to save profile: {ex.Message}");
            }
            
        }

        private void SaveProfile(UserProfile userProfile)
        {
            foreach(var controller in usbControllerManager.Controllers)
            {
                var profileController = userProfile.Controllers.Where(pc => pc.Address == controller.Address).FirstOrDefault();
                for(int deviceIndex = 0; deviceIndex < DeviceControllerDefinitions.DevicePerController; deviceIndex++)
                {
                    var deviceConfig = controller.ControllerData.DeviceConfig;
                    var profileDevice = profileController.Devices[deviceIndex];

                    profileDevice.LightingMode = LightingModes.GetLightingModes().ElementAt((int)deviceConfig.LedMode[deviceIndex]);
                    profileDevice.LightingSpeed = (int)deviceConfig.LedSpeed[deviceIndex];

                    for(int lightIndex = 0; lightIndex < DeviceControllerDefinitions.LedCountPerDevice; lightIndex++)
                    {
                        profileDevice.LightColors[lightIndex] = deviceConfig.GetColor(deviceIndex, lightIndex);
                    }
                }
            };
        }

        public void ApplyProfile(int index)
        {
            try
            {
                var profile = usbControllerManager.GetUserSettings().UserProfiles[index];
                ApplyProfile(profile);
            }
            catch (Exception ex)
            {
                Logger.Write(Level.Error, $"Failed to apply profile: {ex.Message}");
            }
        }

        private void ApplyProfile(UserProfile userProfile)
        {
            foreach (var controller in usbControllerManager.Controllers)
            {
                var profileController = userProfile.Controllers.Where(pc => pc.Address == controller.Address).FirstOrDefault();
                for (int deviceIndex = 0; deviceIndex < DeviceControllerDefinitions.DevicePerController; deviceIndex++)
                {
                    var deviceConfig = controller.ControllerData.DeviceConfig;
                    var profileDevice = profileController.Devices[deviceIndex];

                    int modeIndex = Array.IndexOf(LightingModes.GetLightingModes(), profileDevice.LightingMode);
                    deviceConfig.LedMode[deviceIndex] = (byte)(modeIndex == -1 ? 0 : modeIndex);
                    deviceConfig.LedSpeed[deviceIndex] = (byte)profileDevice.LightingSpeed;

                    for (int lightIndex = 0; lightIndex < DeviceControllerDefinitions.LedCountPerDevice; lightIndex++)
                    {
                        deviceConfig.SetColor(deviceIndex, lightIndex, profileDevice.LightColors[lightIndex]);
                    }
                }

                controller.AppData.UpdateConfigPending = true;
            };
        }
    }
}
