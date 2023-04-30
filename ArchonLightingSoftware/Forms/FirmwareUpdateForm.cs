using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ArchonLightingSystem.Properties;
using ArchonLightingSystem.Models;
using ArchonLightingSystem.Bootloader;
using ArchonLightingSystem.Components;
using ArchonLightingSystem.Common;
using System.IO;
using System.Threading;
using ArchonLightingSystem.UsbApplicationV2;

namespace ArchonLightingSystem
{
    public partial class FirmwareUpdateForm : Form
    {
        #region initializers
        private UsbDeviceManager usbDeviceManager;
        private bool isClosing = false;
        private bool isBusy = true;
        private DragWindowSupport dragSupport = new DragWindowSupport();
        private FirmwareUpdateManager firmwareManager = new FirmwareUpdateManager();
        

        private enum Image
        {
            Error,
            Checkmark,
            Processing
        }

        public FirmwareUpdateForm()
        {
            InitializeComponent();
            dragSupport.Initialize(this);
            btn_UpdateAll.Enabled = false;
            lbl_Status.Text = firmwareManager.GetStatus();
            InitializeForm();
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

        public void InitializeForm()
        {
            usbDeviceManager = new UsbDeviceManager();

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

            firmwareManager.FirmwareEventHandler += new FirmwareEventDelgate(HandleBootloaderUpdates);
            firmwareManager.FirmwareLogHandler += new FirmwareLogDelegate(WriteLog);
            firmwareManager.InitializeUsb(usbDeviceManager);

            AppTheme.ApplyThemeToForm(this);
        }
        #endregion

        #region update_ui
        private void WriteLog(object sender, string log)
        {
            if (this.IsDisposed) return;

            if (InvokeRequired)
            {
                this.Invoke(new Action<object, string>(WriteLog), new object[] { sender, log });
                return;
            }
            txt_Log.AppendText(log);
        }

        private void HandleBootloaderUpdates(object sender, FirmwareEventArgs args)
        {
            if (this.IsDisposed) return;

            if (InvokeRequired)
            {
                this.Invoke(new Action<object, FirmwareEventArgs>(HandleBootloaderUpdates), new object[] { sender, args });
                return;
            }
            lbl_Status.Text = firmwareManager.GetStatus();
            lbl_CurrentVersion.Text = args.FirmwareVersion;
            while(listView1.Items.Count < args.Devices.Count)
            {
                AddDeviceToList();
            }
            args.Devices.ForEach(device => UpdateDeviceInfo(device));

            if(args.Status == FirmwareUpdateManager.ManagerStatus.Canceled || 
                args.Status == FirmwareUpdateManager.ManagerStatus.Completed ||
                args.Status == FirmwareUpdateManager.ManagerStatus.Idle)
            {
                SetFormIsIdle();
            }

            if(args.Status == FirmwareUpdateManager.ManagerStatus.NoFile)
            {
                SetFormNotReady();
            }
        }

        private void AddDeviceToList()
        {
            InitializeProgressBar(listView1.Items.Count);
            ListViewItem listItem1 = new ListViewItem("", 0);
            listItem1.SubItems.Add("?");
            listItem1.SubItems.Add("?");
            listItem1.SubItems.Add("?");
            listView1.Items.Add(listItem1);
        }

        private void UpdateDeviceInfo(FirmwareDevice device)
        {
            listView1.Items[device.DeviceIndex].SubItems[2].Text = device.BootloaderVersion.ToString();
            listView1.Items[device.DeviceIndex].SubItems[3].Text = device.ApplicationVersion.ToString();
            listView1.Items[device.DeviceIndex].SubItems[1].Text = device.DeviceAddress.ToString();
            listView1.Items[device.DeviceIndex].Tag = device;
            ((ProgressBar)listView1.Controls[$"progressBar_{device.DeviceIndex}"]).Value = device.Progress;
            switch (device.DeviceStatus)
            {
                case FirmwareDevice.StatusCode.Canceled:
                case FirmwareDevice.StatusCode.Failed:
                case FirmwareDevice.StatusCode.Disconnected:
                    ((ProgressBar)listView1.Controls[$"progressBar_{device.DeviceIndex}"]).ForeColor = Color.Red;
                    listView1.Items[device.DeviceIndex].ImageIndex = (int)Image.Error;
                    break;

                case FirmwareDevice.StatusCode.Canceling:
                case FirmwareDevice.StatusCode.Connecting:
                    ((ProgressBar)listView1.Controls[$"progressBar_{device.DeviceIndex}"]).ForeColor = Color.Blue;
                    listView1.Items[device.DeviceIndex].ImageIndex = (int)Image.Processing;
                    break;

                case FirmwareDevice.StatusCode.Erasing:
                    ((ProgressBar)listView1.Controls[$"progressBar_{device.DeviceIndex}"]).ForeColor = Color.OrangeRed;
                    listView1.Items[device.DeviceIndex].ImageIndex = (int)Image.Processing;
                    break;

                case FirmwareDevice.StatusCode.Updating:
                    ((ProgressBar)listView1.Controls[$"progressBar_{device.DeviceIndex}"]).ForeColor = Color.Yellow;
                    listView1.Items[device.DeviceIndex].ImageIndex = (int)Image.Processing;
                    break;

                case FirmwareDevice.StatusCode.Verifying:
                    ((ProgressBar)listView1.Controls[$"progressBar_{device.DeviceIndex}"]).ForeColor = Color.Green;
                    listView1.Items[device.DeviceIndex].ImageIndex = (int)Image.Processing;
                    break;

                case FirmwareDevice.StatusCode.Completed:
                case FirmwareDevice.StatusCode.Ready:
                case FirmwareDevice.StatusCode.StartingApp:
                    ((ProgressBar)listView1.Controls[$"progressBar_{device.DeviceIndex}"]).ForeColor = Color.Green;
                    listView1.Items[device.DeviceIndex].ImageIndex = (int)Image.Checkmark;
                    break;

                default:
                    ((ProgressBar)listView1.Controls[$"progressBar_{device.DeviceIndex}"]).ForeColor = Color.Red;
                    listView1.Items[device.DeviceIndex].ImageIndex = (int)Image.Error;
                    break;
            }
            
        }

        private void SetFormNotReady()
        {
            btn_UpdateAll.Enabled = false;
            btn_UpdateSelected.Enabled = false;
        }

        private void SetFormIsUpdating()
        {
            btn_UpdateAll.Enabled = false;
            btn_StartApp.Enabled = false;
            btn_OpenHexFile.Enabled = false;
            btn_Cancel.Text = "Cancel";
            isBusy = true;
            this.Cursor = Cursors.AppStarting;
        }

        private void SetFormIsIdle()
        {
            btn_UpdateAll.Enabled = true;
            btn_UpdateSelected.Enabled = true;
            btn_StartApp.Enabled = true;
            btn_Cancel.Text = "Close";
            isBusy = false;
            this.Cursor = Cursors.Default;
        }
        #endregion

        #region event_handlers
        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            if(isBusy)
            {
                if (MessageBox.Show("Firmware update in progress. Canceling now could prevent your devices from working. Are you sure?", "Warning: Cancel Actions", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    firmwareManager.CancelFirmwareActions();
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
            firmwareManager.CloseFirmwareManager();
            this.Close();
        }

        //This is a callback function that gets called when a Windows message is received by the form.
        //We will receive various different types of messages, but the ones we really want to use are the WM_DEVICECHANGE messages.
        protected override void WndProc(ref Message m)
        {
            usbDeviceManager.HandleWindowEvent(ref m);
            base.WndProc(ref m);
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

                // todo open file
                try
                {
                    if(!firmwareManager.OpenCustomHexFile(openFileDialog.FileName))
                    {
                        btn_UpdateAll.Enabled = false;
                        btn_UpdateSelected.Enabled = false;
                        MessageBox.Show("Error: The file was invalid.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred: {ex.Message}");
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
            firmwareManager.StartApp();

            Thread.Sleep(1000);

            CloseWindow();
        }

        private void btn_UpdateSelected_Click(object sender, EventArgs e)
        {
            SetFormIsUpdating();
            List<byte> devices = new List<byte>();
            var selectedItems = listView1.SelectedItems;
            foreach(ListViewItem item in selectedItems)
            {
                FirmwareDevice fDevice = (FirmwareDevice)(item.Tag);
                devices.Add((byte)fDevice.DeviceAddress);
            }
            // this begins an erase, program, verify cycle
            firmwareManager.EraseProgramVerifyFlash(devices.ToArray()); 
        }

        private void btn_UpdateAll_Click(object sender, EventArgs e)
        {
            SetFormIsUpdating();
            // this begins an erase, program, verify cycle
            firmwareManager.EraseProgramVerifyFlash();
        }

        private void btn_Verify_Click(object sender, EventArgs e)
        {
            SetFormIsUpdating();
            firmwareManager.VerifyFlash();
        }
        #endregion
    }
}
