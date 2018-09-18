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
        private static string[] StatusString = { "Idle", "In Progress", "Completed", "Canceling", "Canceled" };
        private enum Status
        {
            Idle = 0,
            Updating,
            Completed,
            Canceling,
            Canceled
        }

        private bool isUpdating = false;
        private bool isCanceled = false;

        public FirmwareUpdateForm()
        {
            InitializeComponent();
        }

        public void InitializeForm(ApplicationData appdata)
        {
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
                }
            }
            else
            {
                this.Close();
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
                    Hex hex = new Hex();
                    hex.LoadHexFile(openFileDialog.FileName);
                    UInt32 startAddress = 0, progLen = 0;
                    UInt16 crc = 0;
                    hex.VerifyFlash(ref startAddress, ref progLen, ref crc);
                    crc = crc;
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}
