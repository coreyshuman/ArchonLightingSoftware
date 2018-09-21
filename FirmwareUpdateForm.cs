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
using System.IO;

namespace ArchonLightingSystem
{
    public partial class FirmwareUpdateForm : Form
    {
        private UsbApplication.UsbDriver bootUsbDriver;
        private UsbApplication.UsbApp usbApp;
        Bootloader.Bootloader bootloader;
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

        public FirmwareUpdateForm()
        {
            InitializeComponent();
            lbl_Status.Text = StatusString[(int)Status.Disconnected];
            btn_UpdateAll.Enabled = false;
            bootUsbDriver = new UsbApplication.UsbDriver();
            
            bootloader = new Bootloader.Bootloader();
            bootloader.InitializeBootloader(bootUsbDriver, new ProgressChangedEventHandler((object changeSender, ProgressChangedEventArgs args) =>
            {
                progressBar1.Value = args.ProgressPercentage;
                BootloaderState state = (BootloaderState)args.UserState;
                HandleBootloaderResponse(state);
            }));

            timer_ResetHardware.Enabled = true;
            timer_EnableUsb.Enabled = true;
        }

        private void HandleBootloaderResponse(BootloaderState state)
        {
            BootloaderCmd cmd;

            if(state.Length == 0)
            {
                if(state.NoResponseFromDevice)
                {
                    MessageBox.Show("No response from device.");
                }
                return;
            }

            switch((BootloaderCmd)state.Data[0])
            {
                case BootloaderCmd.READ_BOOT_INFO:
                    listView1.Items[0].SubItems[2].Text = new Version(state.Data[1], state.Data[2]).ToString();
                    listView1.Items[0].SubItems[3].Text = new Version(state.Data[3], state.Data[4]).ToString();
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
            var appdata = app.AppData;

            ImageList imageList = new ImageList { ImageSize = new Size(32, 32) };
            listView1.View = View.Details;
            listView1.FullRowSelect = true;
            listView1.SmallImageList = imageList;

            listView1.Columns.Add("Status", 80, HorizontalAlignment.Center);
            listView1.Columns.Add("Device Address", 140, HorizontalAlignment.Left);
            listView1.Columns.Add("Bootloader Version", 140, HorizontalAlignment.Left);
            listView1.Columns.Add("Firmware Version", 140, HorizontalAlignment.Left);

            listView1.Columns[0].DisplayIndex = listView1.Columns.Count - 1;

            imageList.Images.Add(Icon.FromHandle(Resources.cancel.GetHicon()));
            imageList.Images.Add(Icon.FromHandle(Resources.checkmark.GetHicon()));
            imageList.Images.Add(Icon.FromHandle(Resources.processing.GetHicon()));

            ListViewItem listItem1 = new ListViewItem("", 0);
            listItem1.SubItems.Add(appdata.DeviceControllerData.DeviceAddress.ToString());
            listItem1.SubItems.Add(appdata.DeviceControllerData.BootloaderVersion.ToString());
            listItem1.SubItems.Add(appdata.DeviceControllerData.ApplicationVersion.ToString());
            listView1.Items.Add(listItem1);

        }

        private void btn_UpdateAll_Click(object sender, EventArgs e)
        {
            lbl_Status.Text = StatusString[(int)Status.Updating];
            btn_UpdateAll.Enabled = false;
            isUpdating = true;
            this.Cursor = Cursors.AppStarting;
            bootloader.SendCommand(BootloaderCmd.PROGRAM_FLASH, 3, 5000);
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
            if(bootUsbDriver.IsAttached && !isConnected)
            {
                isConnected = true;
                lbl_Status.Text = StatusString[(int)Status.Disconnected];
                bootloader.SendCommand(BootloaderCmd.READ_BOOT_INFO, 3, 500);
            } 
            else if(!bootUsbDriver.IsAttached && isConnected)
            {
                isConnected = false;
                lbl_Status.Text = StatusString[(int)Status.Idle];
            }
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
            bootloader.SendCommand(BootloaderCmd.JMP_TO_APP, 1, 1000);
        }

        private void timer_EnableUsb_Tick(object sender, EventArgs e)
        {
            if (bootUsbDriver.IsAttached)
            {
                timer_EnableUsb.Enabled = false;
                if (bootUsbDriver.IsAttached && !isConnected)
                {
                    isConnected = true;
                    lbl_Status.Text = StatusString[(int)Status.Idle];
                    bootloader.SendCommand(BootloaderCmd.READ_BOOT_INFO, 3, 500);
                }
                else if (!bootUsbDriver.IsAttached && isConnected)
                {
                    isConnected = false;
                    lbl_Status.Text = StatusString[(int)Status.Disconnected];
                }
            }
            else
            {
                bootUsbDriver.InitializeDevice("04D8", "003C");
            }
        }

        private void timer_ResetHardware_Tick(object sender, EventArgs e)
        {
            timer_ResetHardware.Enabled = false;
            usbApp.AppData.ResetToBootloaderPending = true;
        }
    }
}
