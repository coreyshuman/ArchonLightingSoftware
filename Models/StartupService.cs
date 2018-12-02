using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;

namespace ArchonLightingSystem.Models
{
    class StartupService
    {
        private const string registryPath = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";
        private const string appName = "ArchonLightingSystem";

        private bool isEnabled = false;
        private bool isAvailable = false;
        private RegistryKey registryKey = null;

        public bool Enabled
        {
            get
            {
                return isEnabled;
            }
            set
            {
                if (!isAvailable) return;
                SetRegistryValue(value);
            }
        }

        public bool Available
        {
            get
            {
                return isAvailable;
            }
            set
            {
                isAvailable = value;
            }
        }

        public StartupService()
        {
            try
            {
                registryKey = Registry.CurrentUser.OpenSubKey(registryPath, true);
                object value = registryKey.GetValue(appName);
                if(value != null && value.ToString() == Application.ExecutablePath)
                {
                    isEnabled = true;
                }
                isAvailable = true;
            }
            catch
            {
                isAvailable = false;
                isEnabled = false;
            }
        }

        private void SetRegistryValue(bool enable)
        {
            if (enable)
                registryKey.SetValue(appName, Application.ExecutablePath);
            else
                registryKey.DeleteValue(appName, false);

            isEnabled = enable;
        }
    }
}


