/*
 
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
 
  Copyright (C) 2009-2010 Michael Möller <mmoeller@openhardwaremonitor.org>
  Copyright (C) 2018-2019 Corey Shuman <ctshumancode@gmail.com>
	
*/

using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Principal;
using System.Windows.Forms;
using Microsoft.Win32;

namespace ArchonLightingSystem.WindowsSystem.Startup
{
    public class StartupManager
    {
        public bool IsAvailable { get; }

        private TaskSchedulerClass scheduler;
        private bool startup;
        private const string registryRunPath =
          @"Software\Microsoft\Windows\CurrentVersion\Run";
        private const string appName = "ArchonLightingSystem";
        private const string folderName = "Archon Lighting System";
        private readonly string appPath = Application.ExecutablePath;
        private readonly string appArgs = "/background";
        private string registryKeyValue;
        
        

        private bool IsAdministrator()
        {
            try
            {
                WindowsIdentity identity = WindowsIdentity.GetCurrent();
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            catch
            {
                return false;
            }
        }

        public StartupManager()
        {
            registryKeyValue = $"\"{appPath}\" {appArgs}";

            int p = (int)System.Environment.OSVersion.Platform;
            // check if Unix (4) or Unix on Mono 1.x/2.x (128)
            if ((p == 4) || (p == 128))
            {
                scheduler = null;
                IsAvailable = false;
                return;
            }

            if (IsAdministrator())
            {
                try
                {
                    scheduler = new TaskSchedulerClass();
                    scheduler.Connect(null, null, null, null);
                }
                catch
                {
                    scheduler = null;
                }

                if (scheduler != null)
                {
                    try
                    {
                        // check if the taskscheduler is running
                        IRunningTaskCollection collection = scheduler.GetRunningTasks(0);

                        ITaskFolder folder = scheduler.GetFolder("\\Archon Lighting System");
                        IRegisteredTask task = folder.GetTask("Startup");
                        startup = (task != null) &&
                          (task.Definition.Triggers.Count > 0) &&
                          (task.Definition.Triggers[1].Type == TASK_TRIGGER_TYPE2.TASK_TRIGGER_LOGON) &&
                          (task.Definition.Actions.Count > 0) &&
                          (task.Definition.Actions[1].Type == TASK_ACTION_TYPE.TASK_ACTION_EXEC) &&
                          (task.Definition.Actions[1] as IExecAction != null) &&
                          ((task.Definition.Actions[1] as IExecAction).Path == appPath);

                    }
                    catch (IOException)
                    {
                        startup = false;
                    }
                    catch (UnauthorizedAccessException)
                    {
                        scheduler = null;
                    }
                    catch (COMException)
                    {
                        scheduler = null;
                    }
                }
            }
            else
            {
                scheduler = null;
            }

            if (scheduler == null)
            {
                try
                {
                    using (RegistryKey key =
                      Registry.CurrentUser.OpenSubKey(registryRunPath))
                    {
                        startup = false;
                        if (key != null)
                        {
                            string value = (string)key.GetValue(appName);
                            if (value != null)
                                startup = value == registryKeyValue;
                        }
                    }
                    IsAvailable = true;
                }
                catch (SecurityException)
                {
                    IsAvailable = false;
                }
            }
            else
            {
                IsAvailable = true;
            }
        }

        private void CreateSchedulerTask()
        {
            ITaskDefinition definition = scheduler.NewTask(0);
            definition.RegistrationInfo.Description =
              "This task starts the Archon Lighting System on Windows startup.";
            definition.Principal.RunLevel =
              TASK_RUNLEVEL.TASK_RUNLEVEL_HIGHEST;
            definition.Settings.DisallowStartIfOnBatteries = false;
            definition.Settings.StopIfGoingOnBatteries = false;
            definition.Settings.ExecutionTimeLimit = "PT0S";

            ILogonTrigger trigger = (ILogonTrigger)definition.Triggers.Create(
              TASK_TRIGGER_TYPE2.TASK_TRIGGER_LOGON);

            IExecAction action = (IExecAction)definition.Actions.Create(
              TASK_ACTION_TYPE.TASK_ACTION_EXEC);
            action.Path = appPath;
            action.Arguments = appArgs;
            action.WorkingDirectory =
              Path.GetDirectoryName(Application.ExecutablePath);

            ITaskFolder root = scheduler.GetFolder("\\");
            ITaskFolder folder;
            try
            {
                folder = root.GetFolder(folderName);
            }
            catch (IOException)
            {
                folder = root.CreateFolder(folderName, "");
            }
            folder.RegisterTaskDefinition("Startup", definition,
              (int)TASK_CREATION.TASK_CREATE_OR_UPDATE, null, null,
              TASK_LOGON_TYPE.TASK_LOGON_INTERACTIVE_TOKEN, "");
        }

        private void DeleteSchedulerTask()
        {
            ITaskFolder root = scheduler.GetFolder("\\");
            try
            {
                ITaskFolder folder = root.GetFolder(folderName);
                folder.DeleteTask("Startup", 0);
            }
            catch (IOException) { }
            try
            {
                root.DeleteFolder(folderName, 0);
            }
            catch (IOException) { }
        }

        private void CreateRegistryRun()
        {
            RegistryKey key = Registry.CurrentUser.CreateSubKey(registryRunPath);
            key.SetValue(appName, registryKeyValue);
        }

        private void DeleteRegistryRun()
        {
            RegistryKey key = Registry.CurrentUser.CreateSubKey(registryRunPath);
            key.DeleteValue(appName);
        }

        

        public bool Startup
        {
            get
            {
                return startup;
            }
            set
            {
                if (startup != value)
                {
                    if (IsAvailable)
                    {
                        if (scheduler != null)
                        {
                            if (value)
                                CreateSchedulerTask();
                            else
                                DeleteSchedulerTask();
                            startup = value;
                        }
                        else
                        {
                            try
                            {
                                if (value)
                                    CreateRegistryRun();
                                else
                                    DeleteRegistryRun();
                                startup = value;
                            }
                            catch (UnauthorizedAccessException)
                            {
                                throw new InvalidOperationException();
                            }
                        }
                    }
                    else
                    {
                        throw new InvalidOperationException();
                    }
                }
            }
        }
    }
}
