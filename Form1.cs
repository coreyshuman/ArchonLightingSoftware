using System;
using System.Windows.Forms;



namespace ArchonLightingSystem
{
    public partial class Form1 : Form
    {
        UsbApp usbApp = null;

        public unsafe Form1()
        {
            InitializeComponent();


            //Initialize tool tips, to provide pop up help when the mouse cursor is moved over objects on the form.
            ANxVoltageToolTip.SetToolTip(this.ANxVoltage_lbl, "If using a board/PIM without a potentiometer, apply an adjustable voltage to the I/O pin.");
            ANxVoltageToolTip.SetToolTip(this.progressBar1, "If using a board/PIM without a potentiometer, apply an adjustable voltage to the I/O pin.");
            ToggleLEDToolTip.SetToolTip(this.ToggleLEDs_btn, "Sends a packet of data to the USB device.");
            PushbuttonStateTooltip.SetToolTip(this.PushbuttonState_lbl, "Try pressing pushbuttons on the USB demo board/PIM.");

            usbApp = new UsbApp();
            usbApp.RegisterEventHandler(this.Handle);
            usbApp.InitializeDevice("04D8", "0033");

            if (usbApp.IsAttached == true)
            {
                StatusBox_txtbx.Text = "Device Found, AttachedState = TRUE";
            }
            else
            {
                StatusBox_txtbx.Text = "Device not found, verify connect/correct firmware";
            }
        }

        //This is a callback function that gets called when a Windows message is received by the form.
        //We will receive various different types of messages, but the ones we really want to use are the WM_DEVICECHANGE messages.
        protected override void WndProc(ref Message m)
        {
            usbApp.HandleWindowEvent(ref m);
            base.WndProc(ref m);
        } 

        private void ToggleLEDs_btn_Click(object sender, EventArgs e)
        {
            //ToggleLEDsPending = true;	//Will get used asynchronously by the ReadWriteThread
        }

        private void FormUpdateTimer_Tick(object sender, EventArgs e)
        {
            //This timer tick event handler function is used to update the user interface on the form, based on data
            //obtained asynchronously by the ReadWriteThread and the WM_DEVICECHANGE event handler functions.

            //Check if user interface on the form should be enabled or not, based on the attachment state of the USB device.
            if (usbApp.IsAttached == true)
            {
                //Device is connected and ready to communicate, enable user interface on the form 
                StatusBox_txtbx.Text = "Device Found: AttachedState = TRUE";
                PushbuttonState_lbl.Enabled = true;	//Make the label no longer greyed out
                ANxVoltage_lbl.Enabled = true;
                ToggleLEDs_btn.Enabled = true;
            }
            if ((usbApp.IsAttached == false) || (usbApp.IsAttachedButBroken == true))
            {
                //Device not available to communicate. Disable user interface on the form.
                StatusBox_txtbx.Text = "Device Not Detected: Verify Connection/Correct Firmware";
                PushbuttonState_lbl.Enabled = false;	//Make the label no longer greyed out
                ANxVoltage_lbl.Enabled = false;
                ToggleLEDs_btn.Enabled = false;

                PushbuttonState_lbl.Text = "Pushbutton State: Unknown";
                //ADCValue = 0;
                progressBar1.Value = 0;
            }

            //Update the various status indicators on the form with the latest info obtained from the ReadWriteThread()
            if (usbApp.IsAttached == true)
            {
                //Update the pushbutton state label.
                if (false == false)
                    PushbuttonState_lbl.Text = "Pushbutton State: Not Pressed";		//Update the pushbutton state text label on the form, so the user can see the result 
                else
                    PushbuttonState_lbl.Text = "Pushbutton State: Pressed";			//Update the pushbutton state text label on the form, so the user can see the result 

                //Update the ANxx/POT Voltage indicator value (progressbar)
                progressBar1.Value = (int)usbApp.Data.FanSpeed > 3000 ? 3000 : (int)usbApp.Data.FanSpeed;
                lbl_fanSpeed.Text = usbApp.Data.FanSpeed.ToString();
                if(usbApp.Data.EepromReadDone)
                {
                    for (int i = 0; i < usbApp.Data.EepromLength; i++)
                    {
                        Control ctn = this.Controls["txt_Data_" + (i + 1)];
                        ctn.Text = usbApp.Data.EepromData[i].ToString();
                    }
                    usbApp.Data.EepromReadDone = false;
                }
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {
            
        }

        private void btn_Read_Click(object sender, EventArgs e)
        {
            usbApp.Data.EepromAddress = (uint)Int32.Parse(num_Addr.Text);
            usbApp.Data.EepromLength = (uint)Int32.Parse(num_Len.Text);
            usbApp.Data.EepromReadPending = true;
        }

        private void btn_Write_Click(object sender, EventArgs e)
        {
            usbApp.Data.EepromAddress = (uint)Int32.Parse(num_Addr.Text);
            usbApp.Data.EepromLength = (uint)Int32.Parse(num_Len.Text);
            usbApp.Data.EepromData[0] = (byte)Int32.Parse(txt_Data_1.Text);
            for (int i = 0; i < usbApp.Data.EepromLength; i++)
            {
                Control ctn = this.Controls["txt_Data_" + (i + 1)];
                usbApp.Data.EepromData[i] = (byte)Int32.Parse(ctn.Text);
            }
            usbApp.Data.EepromWritePending = true;
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                int len = (int)num_Len.Value;
                if (len < 1 || len > 8) throw new Exception();
                int i = 0;
                for (i = 0; i < len; i++)
                {
                    Control ctn = this.Controls["txt_Data_" + (i + 1)];
                    ctn.Enabled = true;
                }
                for (; i < 8; i++)
                {
                    Control ctn = this.Controls["txt_Data_" + (i + 1)];
                    ctn.Enabled = false;
                }
            }
            catch
            {
                num_Len.Text = "1";
                for (int i = 1; i < 8; i++)
                {
                    Control ctn = this.Controls["txt_Data_" + (i + 1)];
                    ctn.Enabled = false;
                }
            }
        }

        private void btn_ReadConfig_Click(object sender, EventArgs e)
        {
            usbApp.Data.ReadConfigPending = true;
        }
    } 
} 