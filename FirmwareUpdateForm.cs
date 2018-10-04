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
using ArchonLightingSystem.Common;
using System.IO;
using System.Threading;

namespace ArchonLightingSystem
{
    public partial class FirmwareUpdateForm : Form
    {
        private UsbApplication.UsbDriver bootUsbDriver;
        private UsbApplication.UsbApp usbApp;
        Bootloader.Bootloader bootloader;
        private DragWindowSupport dragSupport = new DragWindowSupport();
        private static string[] StatusString = { "Disconnected", "No Firmware Image", "Idle", "In Progress", "Completed", "Canceling", "Canceled" };
        private int deviceCntUpdating = 0;
        private int deviceCntFinishedUpdating = 0;
        private int deviceCntFailedUpdating = 0;
        private enum Status
        {
            Disconnected = 0,
            NoFile,
            Idle,
            Updating,
            Completed,
            Canceling,
            Canceled
        }

        private enum Image
        {
            Error,
            Checkmark,
            Processing
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
                BootloaderStatus status = (BootloaderStatus)args.UserState;
                ((ProgressBar)listView1.Controls[$"progressBar_{status.DeviceIndex}"]).Value = args.ProgressPercentage;
                HandleBootloaderResponse(status);
            }));
            

            timer_ResetHardware.Enabled = true;
            timer_EnableUsb.Enabled = true;
        }

        private void InitializeProgressBar(int deviceIdx)
        {
                ProgressBar bar = new ProgressBar();
                bar.Left = 440;
                bar.Top = 30 + 33 * deviceIdx;
                bar.Width = 140;
                bar.Height = 20;
                bar.Maximum = 100;
                bar.Value = 0;
                bar.Style = ProgressBarStyle.Blocks;
                bar.Parent = listView1;
                bar.Name = $"progressBar_{deviceIdx}";
                bar.Show();
        }

        private void HandleBootloaderResponse(BootloaderStatus status)
        {
            if(status.Failed)
            {
                MessageBox.Show($"No response from device {status.Address}.");
                listView1.Items[status.DeviceIndex].ImageIndex = (int)Image.Error;
                deviceCntFailedUpdating++;
            }

            if (status.Length > 0)
            {
                BootloaderCmd cmd = (BootloaderCmd)status.Data[0];
                switch (cmd)
                {
                    case BootloaderCmd.READ_BOOT_INFO:
                        listView1.Items[status.DeviceIndex].SubItems[2].Text = new Version(status.Data[1], status.Data[2]).ToString();
                        listView1.Items[status.DeviceIndex].SubItems[3].Text = new Version(status.Data[3], status.Data[4]).ToString();
                        listView1.Items[status.DeviceIndex].SubItems[1].Text = status.Data[9].ToString();
                        listView1.Items[status.DeviceIndex].ImageIndex = (int)Image.Checkmark;
                        break;

                    case BootloaderCmd.PROGRAM_FLASH:
                        deviceCntFinishedUpdating++;
                        listView1.Items[status.DeviceIndex].ImageIndex = (int)Image.Checkmark;
                        break;

                    default:
                        throw new Exception("Unknown command response.");
                }
            }

            if(deviceCntFinishedUpdating + deviceCntFailedUpdating == deviceCntUpdating && deviceCntUpdating > 0)
            {
                deviceCntUpdating = deviceCntFailedUpdating = deviceCntFinishedUpdating = 0;
                btn_UpdateAll.Enabled = true;
                btn_StartApp.Enabled = true;
                lbl_Status.Text = StatusString[(int)Status.Completed];
                isUpdating = false;
                MessageBox.Show("Firmware loading complete!");
            }
        }

        public void InitializeForm(UsbApplication.UsbApp app)
        {
            usbApp = app;
            var appdata = app.GetAppData(0);

            ImageList imageList = new ImageList { ImageSize = new Size(60, 32) };
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

            // read firmware file
            string filepath = AppDomain.CurrentDomain.BaseDirectory;
            try
            {
                bool verified = false;
                UInt16 crc = 0;
                if (bootloader.LoadHexFile(filepath + @"firmware\latest.hex"))
                {
                    crc = bootloader.CalculateFlashCRC(0);
                    verified = true;
                    lbl_CurrentVersion.Text = bootloader.GetApplicationVersion().ToString();
                }

                // todo - validate crc of result
                if (verified)
                {
                    btn_UpdateAll.Enabled = true;
                    fileLoaded = true;
                    lbl_Status.Text = StatusString[(int)Status.Disconnected];
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Couldn't open latest firmware, an error occured: {ex.Message}");
                lbl_Status.Text = StatusString[(int)Status.NoFile];
            }

        }

        private void btn_UpdateAll_Click(object sender, EventArgs e)
        {
            lbl_Status.Text = StatusString[(int)Status.Updating];
            btn_UpdateAll.Enabled = false;
            btn_StartApp.Enabled = false;
            isUpdating = true;
            this.Cursor = Cursors.AppStarting;
            deviceCntUpdating = listView1.Items.Count;
            deviceCntFailedUpdating = deviceCntFinishedUpdating = 0;
            for(uint i = 0; i < deviceCntUpdating; i++)
            {
                listView1.Items[(int)i].ImageIndex = (int)Image.Processing;
                bootloader.SendCommand(i, BootloaderCmd.PROGRAM_FLASH, 3, 5000);
            }
            
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
                    deviceCntUpdating = deviceCntFailedUpdating = deviceCntFinishedUpdating = 0;
                    for (int i = 0; i < deviceCntUpdating; i++)
                    {
                        if(listView1.Items[i].ImageIndex == (int)Image.Processing)
                        {
                            listView1.Items[i].ImageIndex = (int)Image.Error;
                            // todo - send bootloader cancel
                        }
                        
                    }
                    if (e.GetType() == typeof(FormClosingEventArgs))
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
                    bool verified = false;
                    UInt16 crc = 0;
                    if (bootloader.LoadHexFile(openFileDialog.FileName))
                    {
                        crc = bootloader.CalculateFlashCRC(0);
                        verified = true;
                    }

                    // todo - validate crc of result
                    if(verified)
                    {
                        btn_UpdateAll.Enabled = true;
                    }
                    
                }
                catch(Exception ex)
                {
                    MessageBox.Show($"An error occured: {ex.Message}");
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
            this.Cursor = Cursors.WaitCursor;
            for (int i = 0; i < listView1.Items.Count; i++)
            {
                bootloader.SendCommand(0, BootloaderCmd.JMP_TO_APP, 1, 1000);
            }
            Thread.Sleep(1000);
            usbApp.ClearDevices();
            CloseWindow();
        }

        private void timer_EnableUsb_Tick(object sender, EventArgs e)
        {
            while (bootUsbDriver.DeviceCount > listView1.Items.Count)
            {
                bootloader.SendCommand((uint)listView1.Items.Count, BootloaderCmd.READ_BOOT_INFO, 3, 500);
                InitializeProgressBar(listView1.Items.Count);
                ListViewItem listItem1 = new ListViewItem("", 0);
                listItem1.SubItems.Add("?");
                listItem1.SubItems.Add("?");
                listItem1.SubItems.Add("?");
                listView1.Items.Add(listItem1);
                if(fileLoaded && !isConnected && !isUpdating)
                {
                    lbl_Status.Text = StatusString[(int)Status.Idle];
                }
                isConnected = true;
            }

            if(!isInitialized)
            {
                isInitialized = true;
                bootUsbDriver.InitializeDevice(Consts.BootloaderVid, Consts.BootloaderPid);
            }
        }

        private void timer_ResetHardware_Tick(object sender, EventArgs e)
        {
            timer_ResetHardware.Enabled = false;
            foreach (var device in usbApp.UsbDevices)
            {
                if(device.AppIsInitialized)
                    device.AppData.ResetToBootloaderPending = true;
            }
        }
    }
}
