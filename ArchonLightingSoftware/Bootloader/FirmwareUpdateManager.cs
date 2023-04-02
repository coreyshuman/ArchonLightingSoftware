using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using ArchonLightingSystem.Common;
using ArchonLightingSystem.UsbApplication;
using System.Diagnostics;
using System.Timers;

namespace ArchonLightingSystem.Bootloader
{
   

    public class FirmwareEventArgs
    {
        public string FirmwareVersion { get; }
        public FirmwareUpdateManager.ManagerStatus Status { get; }
        public List<FirmwareDevice> Devices { get; }

        public FirmwareEventArgs(string firmwareVersion, FirmwareUpdateManager.ManagerStatus status, IList<FirmwareDevice> devices)
        {
            FirmwareVersion = firmwareVersion;
            Status = status;
            Devices = (List<FirmwareDevice>)devices;
        }
    }

    public delegate void FirmwareEventDelgate(object sender, FirmwareEventArgs args);
    public delegate void FirmwareLogDelegate(object sender, string log);

    public class FirmwareDevice
    {
        public enum StatusCode
        {
            Disconnected = 0,
            Connecting,
            Ready,
            Erasing,
            Updating,
            Verifying,
            Completed,
            StartingApp,
            Canceling,
            Canceled,
            Failed
        }
        public StatusCode DeviceStatus { get; set; }
        public int DeviceIndex { get; set; }
        public int DeviceAddress { get; set; }
        public int Progress { get; set; }
        public uint ResetStatus { get; set; }
        public uint ExceptionStatus { get; set; }
        public uint ExceptionCode { get; set; }
        public uint ExceptionAddress { get; set; }
        public Version BootloaderVersion { get; set; }
        public Version ApplicationVersion { get; set; }

        public FirmwareDevice()
        {
            BootloaderVersion = new Version();
            ApplicationVersion = new Version();
        }
    }

    public class FirmwareUpdateManager
    {
        public enum ManagerStatus
        {
            Disconnected = 0,
            NoFile,
            Idle,
            Erasing,
            Updating,
            Verifying,
            Completed,
            Canceling,
            Canceled
        }
        private ManagerStatus Status = ManagerStatus.Disconnected;
        private static string[] StatusString = { "Disconnected", "No Firmware Image", "Idle", "Erasing", "Updating", "Verifying", "Completed", "Starting App...", "Canceling", "Canceled" };
        private Bootloader bootloader;
        private UInt16 firmwareCRC;
        private string hexFirmwareVersion;
        private UsbDriver bootUsbDriver;
        private List<FirmwareDevice> firmwareDevices = new List<FirmwareDevice>();
        private Timer usbUpdateTimer = new Timer(2000);

        public event FirmwareEventDelgate FirmwareEventHandler;
        public event FirmwareLogDelegate FirmwareLogHandler;

        public FirmwareUpdateManager()
        {
            bootloader = new Bootloader();
        }

        public void InitializeUsb(UsbDriver usbDriver)
        {
            bootUsbDriver = usbDriver;
            bootloader.InitializeBootloader(bootUsbDriver, new ProgressChangedEventHandler((object changeSender, ProgressChangedEventArgs args) =>
            {
                BootloaderStatus status = (BootloaderStatus)args.UserState;
                firmwareDevices.Where(device => device.DeviceIndex == status.DeviceIndex).ToList().ForEach((device) =>
                {
                    device.Progress = args.ProgressPercentage;
                });
                HandleBootloaderResponse(status);
            }));
            usbUpdateTimer.Elapsed += UsbPollTick;
            usbUpdateTimer.Enabled = true;
        }

        public string GetStatus()
        {
            return StatusString[(int)Status];
        }

        /// <summary>
        /// Start an Erase - Program - Verify Cycle
        /// </summary>
        public void EraseProgramVerifyFlash(byte[] deviceIds = null)
        {
            if(!IsManagerBusy())
            {
                WriteLog(Level.Information, "--Erase Flash--");
                firmwareDevices.ForEach((device) => {
                    if(!IsDeviceBusy(device.DeviceStatus) && (deviceIds == null || deviceIds.Contains((byte)device.DeviceAddress)))
                    {
                        bootloader.SendCommand((uint)device.DeviceIndex, BootloaderCmd.ERASE_FLASH, 3, 5000);
                        device.DeviceStatus = FirmwareDevice.StatusCode.Erasing;
                        WriteLog(Level.Trace, "  Send erase cmd, device " + device.DeviceAddress);
                    }
                });

                if(firmwareDevices.Where(device => device.DeviceStatus == FirmwareDevice.StatusCode.Erasing).Count() > 0)
                {
                    Status = ManagerStatus.Erasing;
                    OnFirmwareEvent();
                }
            }
        }

        public void ProgramFlash()
        {
            if (!IsManagerBusy())
            {
                WriteLog(Level.Information, "--Program Flash--");
                firmwareDevices.ForEach((device) => {
                    if (!IsDeviceBusy(device.DeviceStatus))
                    {
                        bootloader.SendCommand((uint)device.DeviceIndex, BootloaderCmd.PROGRAM_FLASH, 3, 5000);
                        device.DeviceStatus = FirmwareDevice.StatusCode.Updating;
                        WriteLog(Level.Trace, "  Send write cmd, device " + device.DeviceAddress);
                    }
                });

                if (firmwareDevices.Where(device => device.DeviceStatus == FirmwareDevice.StatusCode.Updating).Count() > 0)
                {
                    Status = ManagerStatus.Updating;
                    OnFirmwareEvent();
                }
            }

            // throw updating event
        }

        public void VerifyFlash()
        {
            if (!IsManagerBusy())
            {
                WriteLog(Level.Information, "--Verify Flash--");
                firmwareDevices.ForEach((device) => {
                    if (!IsDeviceBusy(device.DeviceStatus))
                    {
                        bootloader.SendCommand((uint)device.DeviceIndex, BootloaderCmd.READ_CRC, 3, 5000);
                        device.DeviceStatus = FirmwareDevice.StatusCode.Verifying;
                        WriteLog(Level.Trace, "  Send verify cmd, device " + device.DeviceAddress);
                    }
                });

                if (firmwareDevices.Where(device => device.DeviceStatus == FirmwareDevice.StatusCode.Verifying).Count() > 0)
                {
                    Status = ManagerStatus.Verifying;
                    OnFirmwareEvent();
                }
            }
        }

        public void StartApp()
        {
            if (!IsManagerBusy())
            {
                WriteLog(Level.Information, "--Start App--");
                firmwareDevices.ForEach((device) => {
                    if (!IsDeviceBusy(device.DeviceStatus))
                    {
                        bootloader.SendCommand((uint)device.DeviceIndex, BootloaderCmd.JMP_TO_APP, 1, 1000);
                        device.DeviceStatus = FirmwareDevice.StatusCode.StartingApp;
                        WriteLog(Level.Trace, "  Send start cmd, device " + device.DeviceAddress);
                    }
                });

                if (firmwareDevices.Where(device => device.DeviceStatus == FirmwareDevice.StatusCode.StartingApp).Count() > 0)
                {
                    Status = ManagerStatus.Disconnected;
                    OnFirmwareEvent();
                }
            }
        }

        public void CancelFirmwareActions()
        {
            // todo
            WriteLog(Level.Information, "--Cancel Operations--");
            Status = ManagerStatus.Idle;
            OnFirmwareEvent();
        }

        public bool OpenCustomHexFile(string filepath)
        {
            if (bootloader.LoadHexFile(filepath))
            {
                try
                {
                    firmwareCRC = bootloader.CalculateFlashCRC();
                    hexFirmwareVersion = bootloader.GetApplicationVersion().ToString();
                    WriteLog(Level.Information, $"Opened file. CRC={firmwareCRC.ToString("X4")} Ver={hexFirmwareVersion}");
                    Status = ManagerStatus.Idle;
                    OnFirmwareEvent();
                    return true;
                }
                catch (Exception ex)
                {
                    WriteLog(Level.Error, $"Couldn't open firmware file, an error occurred: {ex.Message}");
                }
            }

            WriteLog(Level.Information, "Open file failed.");
            hexFirmwareVersion = "v?.?";
            Status = ManagerStatus.NoFile;
            OnFirmwareEvent();
            return false;
        }

        public void CloseFirmwareManager()
        {
            FirmwareEventHandler = null;
            FirmwareLogHandler = null;
            bootloader.ShutdownThread();
            WriteLog(Level.Information, "--Closing Manager--");
        }

        private void UsbPollTick(object sender, EventArgs e)
        {
            while (bootUsbDriver.DeviceCount > firmwareDevices.Count)
            {
                WriteLog(Level.Information, "Bootloader found, getting info...");
                var device = new FirmwareDevice
                {
                    DeviceIndex = firmwareDevices.Count
                };
                firmwareDevices.Add(device);
                bootloader.SendCommand((uint)device.DeviceIndex, BootloaderCmd.READ_BOOT_INFO, 3, 500);
            }

            if (Status == ManagerStatus.Disconnected)
            {
                bootUsbDriver.InitializeDevice(Definitions.BootloaderVid, Definitions.BootloaderPid);
                Status = LoadFirmware() ? ManagerStatus.Idle : ManagerStatus.NoFile;
                OnFirmwareEvent();
            }
        }

        private bool LoadFirmware()
        {
            // read firmware file
            string filepath = AppDomain.CurrentDomain.BaseDirectory;
            try
            {
                bool verified = false;
                if (bootloader.LoadHexFile(filepath + @"FirmwareBinaries\latest.hex"))
                {
                    firmwareCRC = bootloader.CalculateFlashCRC();
                    verified = true;
                }

                // todo - validate crc of result (must be saved in code somewhere)
                if (verified)
                {
                    hexFirmwareVersion = bootloader.GetApplicationVersion().ToString();
                    WriteLog(Level.Information, $"Opened file. CRC={firmwareCRC.ToString("X4")} Ver={hexFirmwareVersion}");
                    Status = ManagerStatus.Idle;
                    OnFirmwareEvent();
                    return true;
                }

            }
            catch (Exception ex)
            {
                WriteLog(Level.Error, $"Couldn't open latest firmware, an error occurred: {ex.Message}");
            }

            WriteLog(Level.Error, "Open file failed.");
            Status = ManagerStatus.NoFile;
            OnFirmwareEvent();
            return false;
        }

        private void HandleBootloaderResponse(BootloaderStatus status)
        {
            if (status.Failed)
            {
                firmwareDevices.Where(device => device.DeviceIndex == status.DeviceIndex).ToList().ForEach((device) =>
                {
                    device.DeviceStatus = FirmwareDevice.StatusCode.Failed;
                    WriteLog(Level.Error, $"!!Device {device.DeviceAddress} failed!!");
                });
            }

            if (status.Length > 0)
            {
                BootloaderCmd cmd = (BootloaderCmd)status.Data[0];
                switch (cmd)
                {
                    case BootloaderCmd.READ_BOOT_INFO:
                        firmwareDevices.Where(device => device.DeviceIndex == status.DeviceIndex).ToList().ForEach((device) =>
                        {
                            device.BootloaderVersion = new Version(status.Data[1], status.Data[2]);
                            device.ApplicationVersion = new Version(status.Data[3], status.Data[4]);
                            device.ResetStatus = Util.UInt32FromBytes(status.Data[5], status.Data[6], status.Data[7], status.Data[8]);
                            device.DeviceAddress = status.Data[9];
                            device.ExceptionStatus = Util.UInt32FromBytes(status.Data[10], status.Data[11], status.Data[12], status.Data[13]);
                            device.ExceptionCode = Util.UInt32FromBytes(status.Data[14], status.Data[15], status.Data[16], status.Data[17]);
                            device.ExceptionAddress = Util.UInt32FromBytes(status.Data[18], status.Data[19], status.Data[20], status.Data[21]);
                            device.DeviceStatus = FirmwareDevice.StatusCode.Ready;
                            WriteLog(Level.Information, "--Device info:");
                            WriteLog(Level.Information, $"  BootVer: {device.BootloaderVersion} AppVer: {device.ApplicationVersion}  Address: [{device.DeviceAddress}]");
                            WriteLog(Level.Information, $"  Reset Status: [{device.ResetStatus.ToString("X8")}]");
                            string exStatSource = "(unknown)";
                            if(device.ExceptionStatus == 0xAAAAAAAA)
                            {
                                exStatSource = "(boot)";
                            }
                            else if(device.ExceptionStatus == 0x5A5A5A5A)
                            {
                                exStatSource = "(app)";
                            }
                            WriteLog(Level.Information, $"  ExStat: [{device.ExceptionStatus.ToString("X8")}] {exStatSource}, ExAdr: [{device.ExceptionAddress.ToString("X8")}], ExCode: [{device.ExceptionCode.ToString("X8")}]");
                        });
                        
                        break;

                    case BootloaderCmd.PROGRAM_FLASH:
                        firmwareDevices.Where(device => device.DeviceIndex == status.DeviceIndex).ToList().ForEach((device) =>
                        {
                            bootloader.SendCommand((uint)device.DeviceIndex, BootloaderCmd.READ_CRC, 3, 500);
                            device.DeviceStatus = FirmwareDevice.StatusCode.Verifying;
                            WriteLog(Level.Information, $"Device {device.DeviceAddress} Program Complete!");
                        });

                        break;
                    case BootloaderCmd.READ_CRC:
                        firmwareDevices.Where(device => device.DeviceIndex == status.DeviceIndex).ToList().ForEach((device) =>
                        {
                            UInt16 crc = Util.UInt16FromBytes(status.Data[1], status.Data[2]);
                            device.DeviceStatus = (crc == firmwareCRC) ? FirmwareDevice.StatusCode.Completed : FirmwareDevice.StatusCode.Failed;
                            WriteLog(Level.Information, $"Device {device.DeviceAddress} CRC { ((crc == firmwareCRC) ? "Passed" : "FAILED")} read: [{crc.ToString("X2")}] expected: [{firmwareCRC.ToString("X2")}]");
                        });
                        break;
                    case BootloaderCmd.ERASE_FLASH:
                        firmwareDevices.Where(device => device.DeviceIndex == status.DeviceIndex).ToList().ForEach((device) =>
                        {
                            bootloader.SendCommand((uint)device.DeviceIndex, BootloaderCmd.PROGRAM_FLASH, 3, 5000);
                            device.DeviceStatus = FirmwareDevice.StatusCode.Updating;
                            WriteLog(Level.Information, $"Device {device.DeviceAddress} Erase Complete.");
                        });
                        break;

                    default:
                        WriteLog(Level.Error, $"!!Unknown command response [{cmd.ToString("X2")}]");
                        break;
                }
            }

            if (Status != ManagerStatus.NoFile)
            {

                int erasingDevices = firmwareDevices.Where(device => device.DeviceStatus == FirmwareDevice.StatusCode.Erasing).Count();

                if (erasingDevices > 0)
                {
                    Status = ManagerStatus.Erasing;
                }

                int updatingDevices = firmwareDevices.Where(device => device.DeviceStatus == FirmwareDevice.StatusCode.Updating).Count();

                if (updatingDevices > 0)
                {
                    Status = ManagerStatus.Updating;
                }

                int verifyingDevices = firmwareDevices.Where(device => device.DeviceStatus == FirmwareDevice.StatusCode.Verifying).Count();

                if (verifyingDevices > 0)
                {
                    Status = ManagerStatus.Verifying;
                }

                int busyDevices = firmwareDevices.Where(device => IsDeviceBusy(device.DeviceStatus)).Count();

                if (busyDevices == 0)
                {
                    Status = ManagerStatus.Completed;
                }
            }
            OnFirmwareEvent();
        }

        private bool IsDeviceBusy(FirmwareDevice.StatusCode code)
        {
            return code == FirmwareDevice.StatusCode.Canceling ||
                code == FirmwareDevice.StatusCode.Connecting ||
                code == FirmwareDevice.StatusCode.Erasing ||
                code == FirmwareDevice.StatusCode.Updating ||
                code == FirmwareDevice.StatusCode.Verifying;
        }

        private bool IsManagerBusy()
        {
            return Status == ManagerStatus.Canceling ||
                Status == ManagerStatus.Disconnected ||
                Status == ManagerStatus.Erasing ||
                Status == ManagerStatus.NoFile ||
                Status == ManagerStatus.Updating ||
                Status == ManagerStatus.Verifying;
        }

        private void OnFirmwareEvent()
        {
            FirmwareEventHandler?.Invoke(this, new FirmwareEventArgs(hexFirmwareVersion, Status, firmwareDevices.ToList()));
        }

        private void WriteLog(Level lvl, string log)
        {
            Logger.Write(lvl, log);
            FirmwareLogHandler?.Invoke(this, log + Environment.NewLine);
        }
    }
}
