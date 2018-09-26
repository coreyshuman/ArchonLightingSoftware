using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ArchonLightingSystem.Properties;
using ArchonLightingSystem.Models;
using ArchonLightingSystem.Bootloader;
using ArchonLightingSystem.Components;
using System.IO;

namespace ArchonLightingSystem
{
    public partial class FirmwareUpdateForm : Form
    {
        private UsbApplication.UsbDriver bootUsbDriver;
        private UsbApplication.UsbApp usbApp;
        Bootloader.Bootloader bootloader;
        private DragWindowSupport dragSupport = new DragWindowSupport();
        private static string[] StatusString = { "Disconnected", "Idle", "In Progress", "Completed", "Canceling", "Canceled" };
        private enum Status
        {
            Disconnected = 0,
            Idle,
            Updating,
            Completed,
            Canceling,
            Canceled
        }

        private bool fileLoaded = false;
        private bool isUpdating = false;
        private bool isCanceled = false;
        private bool isClosing = false;
        private bool isConnected = false;
        private bool isInitialized = false;

        public FirmwareUpdateForm()
        {
            InitializeComponent();
            dragSupport.Initialize(this);
            lbl_Status.Text = StatusString[(int)Status.Disconnected];
            btn_UpdateAll.Enabled = false;
            bootUsbDriver = new UsbApplication.UsbDriver();
            
            bootloader = new Bootloader.Bootloader();
            
            bootloader.InitializeBootloader(bootUsbDriver, new ProgressChangedEventHandler((object changeSender, ProgressChangedEventArgs args) =>
            {
                progressBar1.Value = args.ProgressPercentage;
                BootloaderStatus status = (BootloaderStatus)args.UserState;
                HandleBootloaderResponse(status);
            }));
            

            timer_ResetHardware.Enabled = true;
            timer_EnableUsb.Enabled = true;
        }

        private void HandleBootloaderResponse(BootloaderStatus status)
        {
            BootloaderCmd cmd;

            if(status.Length == 0)
            {
                if(status.NoResponseFromDevice)
                {
                    MessageBox.Show("No response from device.");
                }
                return;
            }

            switch((BootloaderCmd)status.Data[0])
            {
                case BootloaderCmd.READ_BOOT_INFO:
                    listView1.Items[status.DeviceIndex].SubItems[2].Text = new Version(status.Data[1], status.Data[2]).ToString();
                    listView1.Items[status.DeviceIndex].SubItems[3].Text = new Version(status.Data[3], status.Data[4]).ToString();
                    listView1.Items[status.DeviceIndex].SubItems[1].Text = status.Data[9].ToString();
                    break;

                case BootloaderCmd.PROGRAM_FLASH:
                    MessageBox.Show("Update Successful!");
                    btn_UpdateAll.Enabled = true;
                    break;

                default:
                    throw new Exception("Unkown command response.");
            }
        }

        public void InitializeForm(UsbApplication.UsbApp app)
        {
            usbApp = app;
            var appdata = app.GetAppData(0);

            ImageList imageList = new ImageList { ImageSize = new Size(32, 32) };
            listView1.View = View.Details;
            listView1.FullRowSelect = true;
            listView1.SmallImageList = imageList;

            listView1.Columns.Add("Status", 60, HorizontalAlignment.Center);
            listView1.Columns.Add("Device Address", 120, HorizontalAlignment.Left);
            listView1.Columns.Add("Bootloader Version", 120, HorizontalAlignment.Left);
            listView1.Columns.Add("Firmware Version", 120, HorizontalAlignment.Left);
            listView1.Columns.Add("Progress", 180, HorizontalAlignment.Left);

            listView1.Columns[0].DisplayIndex = listView1.Columns.Count - 2;

            imageList.Images.Add(Icon.FromHandle(Resources.cancel.GetHicon()));
            imageList.Images.Add(Icon.FromHandle(Resources.checkmark.GetHicon()));
            imageList.Images.Add(Icon.FromHandle(Resources.processing.GetHicon()));

        }

        private void btn_UpdateAll_Click(object sender, EventArgs e)
        {
            lbl_Status.Text = StatusString[(int)Status.Updating];
            btn_UpdateAll.Enabled = false;
            isUpdating = true;
            this.Cursor = Cursors.AppStarting;
            bootloader.SendCommand(0, BootloaderCmd.PROGRAM_FLASH, 3, 5000);
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            if(isUpdating)
            {
                if (MessageBox.Show("This could prevent your devices working. Are you sure?", "Cancel Update", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    lbl_Status.Text = StatusString[(int)Status.Canceling];
                    btn_UpdateAll.Enabled = true;
                    isUpdating = false;
                    isCanceled = true;
                    this.Cursor = Cursors.Arrow;
                    if(e.GetType() == typeof(FormClosingEventArgs))
                    {
                        CloseWindow();
                    }
                }
            }
            else
            {
                CloseWindow();
            }
        }

        private void CloseWindow()
        {
            isClosing = true;
            bootloader.ShutdownThread();
            this.Close();
        }

        //This is a callback function that gets called when a Windows message is received by the form.
        //We will receive various different types of messages, but the ones we really want to use are the WM_DEVICECHANGE messages.
        protected override void WndProc(ref Message m)
        {
            bootUsbDriver.HandleWindowEvent(ref m);
            base.WndProc(ref m);
            /*
            if(bootUsbDriver.GetDevice(0).IsAttached && !isConnected)
            {
                isConnected = true;
                lbl_Status.Text = StatusString[(int)Status.Disconnected];
                bootloader.SendCommand(BootloaderCmd.READ_BOOT_INFO, 3, 500);
            } 
            else if(!bootUsbDriver.GetDevice(0).IsAttached && isConnected)
            {
                isConnected = false;
                lbl_Status.Text = StatusString[(int)Status.Idle];
            }
            */
        }

        private void btn_OpenHexFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Hex File (*.hex)|*.hex";
            try
            {
                openFileDialog.InitialDirectory = Path.GetDirectoryName(Settings.Default.HexFileLocation);
            } 
            catch(Exception)
            {
                // do nothing
            }
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                Settings.Default.HexFileLocation = openFileDialog.FileName;
                Settings.Default.Save();

                try
                {
                    
                    bootloader.LoadHexFile(openFileDialog.FileName);
                    btn_UpdateAll.Enabled = true;
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        private void FirmwareUpdateForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!isClosing)
            {
                e.Cancel = true;
                btn_Cancel_Click(sender, e);
            }
        }

        private void btn_StartApp_Click(object sender, EventArgs e)
        {
            bootloader.SendCommand(0, BootloaderCmd.JMP_TO_APP, 1, 1000);
        }

        private void timer_EnableUsb_Tick(object sender, EventArgs e)
        {
            while (bootUsbDriver.DeviceCount > listView1.Items.Count)
            {
                bootloader.SendCommand(listView1.Items.Count, BootloaderCmd.READ_BOOT_INFO, 3, 500);
                ListViewItem listItem1 = new ListViewItem("", 0);
                listItem1.SubItems.Add("?");
                listItem1.SubItems.Add("?");
                listItem1.SubItems.Add("?");
                listView1.Items.Add(listItem1);
            }

            if(!isInitialized)
            {
                isInitialized = true;
                bootUsbDriver.InitializeDevice("04D8", "003C");
            }
            /*
            if (bootUsbDriver.GetDevice(0).IsAttached)
            {
                timer_EnableUsb.Enabled = false;
                if (bootUsbDriver.GetDevice(0).IsAttached && !isConnected)
                {
                    isConnected = true;
                    lbl_Status.Text = StatusString[(int)Status.Idle];
                    bootloader.SendCommand(BootloaderCmd.READ_BOOT_INFO, 3, 500);
                }
                else if (!bootUsbDriver.GetDevice(0).IsAttached && isConnected)
                {
                    isConnected = false;
                    lbl_Status.Text = StatusString[(int)Status.Disconnected];
                }
            }
            else
            {
                bootUsbDriver.InitializeDevice("04D8", "003C");
            }
            */
        }

        private void timer_ResetHardware_Tick(object sender, EventArgs e)
        {
            timer_ResetHardware.Enabled = false;
            foreach (var device in usbApp.UsbDevices)
            {
                device.AppData.ResetToBootloaderPending = true;
            }
        }
    }
}
