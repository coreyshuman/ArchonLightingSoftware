using System;
using System.Collections.Generic;
using System.Threading;
using System.ComponentModel;
using System.Text;

namespace ArchonLightingSystem
{
    class AppData
    {
        public uint FanSpeed;
        public bool EepromReadPending;
        public bool EepromReadDone;
        public bool EepromWritePending;
        public bool ReadConfigPending;
        public uint EepromAddress;
        public uint EepromLength;
        public Byte[] EepromData;

        public AppData()
        {
            FanSpeed = 0;
            EepromReadPending = false;
            EepromReadDone = false;
            EepromWritePending = false;
            EepromAddress = 1;
            EepromLength = 1;
            EepromData = new byte[30];
        }
    }
    class UsbApp : UsbDriver
    {
        public AppData Data;
        //Variables used by the application/form updates.
        public bool PushbuttonPressed = false;     //Updated by ReadWriteThread, read by FormUpdateTimer tick handler (needs to be atomic)
        public bool ToggleLEDsPending = false;     //Updated by ToggleLED(s) button click event handler, used by ReadWriteThread (needs to be atomic)
        //public uint FanSpeed = 0;			//Updated by ReadWriteThread, read by FormUpdateTimer tick handler (needs to be atomic)
        private BackgroundWorker ReadWriteThread;

        public UsbApp()
        {
            Data = new AppData();
            //Recommend performing USB read/write operations in a separate thread.  Otherwise,
            //the Read/Write operations are effectively blocking functions and can lock up the
            //user interface if the I/O operations take a long time to complete.
            ReadWriteThread = new BackgroundWorker();
            ReadWriteThread.WorkerReportsProgress = true;
            ReadWriteThread.DoWork += new DoWorkEventHandler(ReadWriteThread_DoWork);
            ReadWriteThread.RunWorkerAsync();
        }

        private void ReadWriteThread_DoWork(object sender, DoWorkEventArgs e)
        {
            Byte[] tempBuffer = new Byte[USB_BUFFER_SIZE + 1];
            Byte[] rxtxBuffer = new Byte[USB_BUFFER_SIZE + 1];
            uint byteCnt = 0;
            int i = 0;

            while (true)
            {
                
                try
                {
                    if (IsAttached == true)	//Do not try to use the read/write handles unless the USB device is attached and ready
                    {
                        for(i = 0; i < USB_BUFFER_SIZE+1; i++)
                        {
                            tempBuffer[i] = 0;
                            rxtxBuffer[i] = 0;
                        }
                        //Get Fan Speed
                        tempBuffer[0] = 0x35;	//READ_FAN command (see firmware source code)

                        byteCnt = GenerateFrames(tempBuffer, 1, ref rxtxBuffer, USB_BUFFER_SIZE + 1);

                        //To get the ADCValue, first, we send a packet with our "READ_POT" command in it.
                        if (byteCnt > 0 && WriteUSBDevice(rxtxBuffer, byteCnt) > 0)	
                        {
                            //Now get the response packet from the firmware.
                            if (ReadUSBDevice(ref rxtxBuffer, USB_BUFFER_SIZE+1) > 0)
                            {
                                byteCnt = ValidateFrame(rxtxBuffer, USB_BUFFER_SIZE + 1, ref tempBuffer, USB_BUFFER_SIZE+1);
                                //INBuffer[0] is an echo back of the command (see microcontroller firmware).
                                //INBuffer[1] and INBuffer[2] contains the fan speed value (see microcontroller firmware).  
                                if (byteCnt > 2 && tempBuffer[0] == 0x35)
                                {
                                    Data.FanSpeed = (uint)(tempBuffer[2] << 8) + tempBuffer[1];	//Need to reformat the data from two unsigned chars into one unsigned int.
                                }
                            }
                        }

                        if (Data.EepromReadPending)
                        {
                            Data.EepromReadPending = false;
                            for (i = 0; i < USB_BUFFER_SIZE + 1; i++)
                            {
                                tempBuffer[i] = 0;
                                rxtxBuffer[i] = 0;
                            }
                            tempBuffer[0] = 0x38;   //READ_EEPROM command (see firmware source code)
                            tempBuffer[1] = (byte)Data.EepromAddress;
                            tempBuffer[2] = (byte)Data.EepromLength;

                            byteCnt = GenerateFrame(tempBuffer, 3, ref rxtxBuffer, USB_BUFFER_SIZE + 1);
                            if (byteCnt > 0 && WriteUSBDevice(rxtxBuffer, byteCnt) > 0)
                            {
                                //Now get the response packet from the firmware.
                                if (ReadUSBDevice(ref rxtxBuffer, USB_BUFFER_SIZE + 1) > 0)
                                {
                                    byteCnt = ValidateFrame(rxtxBuffer, USB_BUFFER_SIZE + 1, ref tempBuffer, USB_BUFFER_SIZE + 1);
                                    if (byteCnt > 2 && tempBuffer[0] == 0x38)
                                    {
                                        for (i = 0; i < tempBuffer[1]; i++)
                                        {
                                            Data.EepromData[i] = tempBuffer[i + 2];
                                        }
                                    }
                                    Data.EepromReadDone = true;
                                }
                            }
                        }

                        if (Data.EepromWritePending)
                        {
                            Data.EepromWritePending = false;
                            for (i = 0; i < USB_BUFFER_SIZE + 1; i++)
                            {
                                tempBuffer[i] = 0;
                                rxtxBuffer[i] = 0;
                            }
                            tempBuffer[0] = 0x39;   //WRITE_EEPROM command (see firmware source code)
                            tempBuffer[1] = (byte)Data.EepromAddress;
                            tempBuffer[2] = (byte)Data.EepromLength;
                            for (i = 0; i < tempBuffer[2]; i++)
                            {
                                tempBuffer[i + 3] = Data.EepromData[i];
                            }

                            byteCnt = GenerateFrame(tempBuffer, Data.EepromLength+3, ref rxtxBuffer, USB_BUFFER_SIZE + 1);
                            if (byteCnt > 0 && WriteUSBDevice(rxtxBuffer, byteCnt) > 0)
                            {
                                //Now get the response packet from the firmware.
                                if (ReadUSBDevice(ref rxtxBuffer, USB_BUFFER_SIZE + 1) > 0)
                                {
                                    byteCnt = ValidateFrame(rxtxBuffer, USB_BUFFER_SIZE + 1, ref tempBuffer, USB_BUFFER_SIZE + 1);
                                    if (byteCnt > 2 && tempBuffer[0] == 0x39)
                                    {
                                        Console.WriteLine(tempBuffer[1]);
                                    }
                                }
                            }
                        }

                        if (Data.ReadConfigPending)
                        {
                            Data.ReadConfigPending = false;
                            for (i = 0; i < USB_BUFFER_SIZE + 1; i++)
                            {
                                tempBuffer[i] = 0;
                                rxtxBuffer[i] = 0;
                            }
                            tempBuffer[0] = 0x30;   //READ_EEPROM command (see firmware source code)

                            byteCnt = GenerateFrame(tempBuffer, 3, ref rxtxBuffer, USB_BUFFER_SIZE + 1);
                            if (byteCnt > 0)
                            {
                                if (WriteUSBDevice(rxtxBuffer, byteCnt) > 0)
                                {
                                    //Now get the response packet from the firmware.
                                    if (ReadUSBDevice(ref rxtxBuffer, USB_BUFFER_SIZE + 1) > 0)
                                    {
                                        byteCnt = ValidateFrame(rxtxBuffer, USB_BUFFER_SIZE + 1, ref tempBuffer, USB_BUFFER_SIZE + 1);
                                        if (byteCnt > 2 && tempBuffer[0] == 0x30)
                                        {
                                            for (i = 0; i < tempBuffer[1]; i++)
                                            {
                                                Data.EepromData[i] = tempBuffer[i + 2];
                                            }
                                        }
                                        Data.EepromReadDone = true;
                                    }
                                }
                            }
                        }

                        /*

                        //Get the pushbutton state from the microcontroller firmware.
                        OUTBuffer[0] = 0;			//The first byte is the "Report ID" and does not get sent over the USB bus.  Always set = 0.
                        OUTBuffer[1] = 0x81;		//0x81 is the "Get Pushbutton State" command in the firmware
                        for (uint i = 2; i < 65; i++)	//This loop is not strictly necessary.  Simply initializes unused bytes to
                            OUTBuffer[i] = 0xFF;				//0xFF for lower EMI and power consumption when driving the USB cable.

                        //To get the pushbutton state, first, we send a packet with our "Get Pushbutton State" command in it.
                        if (WriteFile(WriteHandleToUSBDevice, OUTBuffer, 65, ref BytesWritten, IntPtr.Zero))	//Blocking function, unless an "overlapped" structure is used
                        {
                            //Now get the response packet from the firmware.
                            INBuffer[0] = 0;
                            {
                                if (ReadFileManagedBuffer(ReadHandleToUSBDevice, INBuffer, 65, ref BytesRead, IntPtr.Zero))	//Blocking function, unless an "overlapped" structure is used	
                                {
                                    //INBuffer[0] is the report ID, which we don't care about.
                                    //INBuffer[1] is an echo back of the command (see microcontroller firmware).
                                    //INBuffer[2] contains the I/O port pin value for the pushbutton (see microcontroller firmware).  
                                    if ((INBuffer[1] == 0x81) && (INBuffer[2] == 0x01))
                                    {
                                        PushbuttonPressed = false;
                                    }
                                    if ((INBuffer[1] == 0x81) && (INBuffer[2] == 0x00))
                                    {
                                        PushbuttonPressed = true;
                                    }
                                }
                            }
                        }



                        //Check if this thread should send a Toggle LED(s) command to the firmware.  ToggleLEDsPending will get set
                        //by the ToggleLEDs_btn click event handler function if the user presses the button on the form.
                        if (ToggleLEDsPending == true)
                        {
                            OUTBuffer[0] = 0;				//The first byte is the "Report ID" and does not get sent over the USB bus.  Always set = 0.
                            OUTBuffer[1] = 0x80;			//0x80 is the "Toggle LED(s)" command in the firmware
                            for (uint i = 2; i < 65; i++)	//This loop is not strictly necessary.  Simply initializes unused bytes to
                                OUTBuffer[i] = 0xFF;		//0xFF for lower EMI and power consumption when driving the USB cable.
                            //Now send the packet to the USB firmware on the microcontroller
                            WriteFile(WriteHandleToUSBDevice, OUTBuffer, 65, ref BytesWritten, IntPtr.Zero);	//Blocking function, unless an "overlapped" structure is used
                            ToggleLEDsPending = false;
                        }
                        */
                    } //end of: if(IsAttached == true)
                    else
                    {
                        Thread.Sleep(5);    //Add a small delay.  Otherwise, this while(true) loop can execute very fast and cause 
                                            //high CPU utilization, with no particular benefit to the application.
                    }
                }
                catch
                {
                    //Exceptions can occur during the read or write operations.  For example,
                    //exceptions may occur if for instance the USB device is physically unplugged
                    //from the host while the above read/write functions are executing.

                    //Don't need to do anything special in this case.  The application will automatically
                    //re-establish communications based on the global IsAttached boolean variable used
                    //in conjunction with the WM_DEVICECHANGE messages to dyanmically respond to Plug and Play
                    //USB connection events.
                }

            }

        }

        private uint GetDeviceResponse()
        {
            uint byteCnt;
            if (ReadUSBDevice(ref rxtxBuffer, USB_BUFFER_SIZE + 1) > 0)
            {
                byteCnt = ValidateFrame(rxtxBuffer, USB_BUFFER_SIZE + 1, ref tempBuffer, USB_BUFFER_SIZE + 1);
                if (byteCnt > 2 && tempBuffer[0] == 0x30)
                {
                }
            }

        private uint GenerateAndSendFrames(CONTROL_CMD cmd, Byte[] frameData, uint frameLen, ref Byte[] outBuffer, uint outBufferMaxLen)
        {
            uint byteCnt;
            byteCnt = GenerateFrames(frameData, frameLen, ref outBuffer, USB_BUFFER_SIZE + 1);
            if (byteCnt > 0)
            {
                if(WriteUSBDevice(outBuffer, byteCnt) > 0)
                {
                    return;
                }
            }

        private uint GenerateFrames(CONTROL_CMD cmd, Byte[] frameData, uint dataLen, ref Byte[] outBuffer, uint outBufferMaxLen)
        {
            uint outBufferLen = 0;
            uint packetDataCount;
            UInt16 crc = 0;

            while (dataLen > 0)
            {
                if(outBufferLen >= outBufferMaxLen)
                {
                    throw new Exception("Frame Data Overflow.");
                }
                outBuffer[outBufferLen++] = (byte)cmd;
                // if greater than 60 bytes, flag for multibyte
                outBuffer[outBufferLen++] = (byte)((dataLen > 60 ? (62 | 0x80) : dataLen) + 2);
                packetDataCount = 0;
                while (dataLen > 0 && packetDataCount < 60)
                {
                    outBuffer[outBufferLen++] = frameData[packetDataCount++];
                    dataLen--;
                }

                // Add CRC
                crc = CalculateCrc(outBuffer, outBufferLen-packetDataCount-2, outBuffer[outBufferLen - packetDataCount - 1]);
                outBuffer[outBufferLen++] = (byte)(crc & 0xFF);
                outBuffer[outBufferLen++] = (byte)((crc >> 8) & 0xFF);
            }

            // pad 0xFF to 64 byte boundary since we always send 64 bytes per packet
            packetDataCount = outBufferLen % 64;
            while (packetDataCount-- > 0)
            {
                outBuffer[outBufferLen++] = 0xFF;
            }

            return (outBufferLen); // Return buffer length.
        }

        class FrameInfo
        {
            public Byte[] FrameData;
            public uint FrameLen;
            public uint OutBufferLen;
            public uint OutBufferMaxLen;
            public CONTROL_CMD PreviousFrameCmd;
            public bool Multiframe;
            public bool FrameValid;
        }
        private uint ValidateFrame(FrameInfo frameInfo)
        {
            bool escape = false;
            UInt16 crc;
            bool frameValid = false;
            uint frameIdx = 0;
            uint outBufferLen = 0;
            uint dataLen = 0;

            bool multipacket = (frameInfo.FrameData[1] & 0x80) == 0x80; // check multipacket flag
            dataLen = (uint)(frameInfo.FrameData[1] & 0x3F);

            if (frameInfo.Multiframe) {
                frameIdx += 2; // skip cmd and len
            }
            else
            {
                frameInfo.PreviousFrameCmd = (CONTROL_CMD)frameInfo.FrameData[0];
                dataLen += 2; // length + 2 for CMD and LEN
            }


            while (dataLen > 0)
            {
                dataLen--;

                RcvBuff.Data[RcvBuff.Len++] = *RxData;
                RxData++;
            }

            crc.byte.LB = RcvBuff.Data[RcvBuff.Len - 2];
            crc.byte.HB = RcvBuff.Data[RcvBuff.Len - 1];
            if ((CalculateCrc(RcvBuff.Data, (UINT32)(RcvBuff.Len - 2)) == crc.Val) && (RcvBuff.Len > 2))
            {
                // CRC matches and frame received is valid.
                if (multipacket)
                {
                    RxFrameValid = TRUE;
                }
            }
            else
            {
                RcvBuff.Cmd = 0;
                RcvBuff.Len = 0;
            }

            while ((frameLen > 0) && (!frameValid)) // Loop till len = 0 or till frame is valid
            {
                frameLen--;

                if (outBufferLen > outBufferMaxLen)
                {
                    outBufferLen = 0;
                }

                switch (frameData[frameIdx])
                {

                    case (byte)CMD.SOH: //Start of header
                        if (escape)
                        {
                            // Received byte is not SOH, but data.
                            outBuffer[outBufferLen++] = frameData[frameIdx];
                            // Reset Escape Flag.
                            escape = false;
                        }
                        else
                        {
                            // Received byte is indeed a SOH which indicates start of new frame.
                            outBufferLen = 0;
                        }
                        break;

                    case (byte)CMD.EOT: // End of transmission
                        if (escape)
                        {
                            // Received byte is not EOT, but data.
                            outBuffer[outBufferLen++] = frameData[frameIdx];
                            // Reset Escape Flag.
                            escape = false;
                        }
                        else
                        {
                            // Received byte is indeed a EOT which indicates end of frame.
                            // Calculate CRC to check the validity of the frame.
                            if (outBufferLen > 1)
                            {
                                crc = outBuffer[outBufferLen-2];
                                crc += (UInt16)(outBuffer[outBufferLen-1] << 8);
                                if ((CalculateCrc(outBuffer, (outBufferLen - 2)) == crc) && (outBufferLen > 2))
                                {
                                    // CRC matches and frame received is valid.
                                    frameValid = true;
                                }
                            }
                        }
                        break;


                    case (byte)CMD.DLE: // Escape character received.
                        if (escape)
                        {
                            // Received byte is not ESC but data.
                            outBuffer[outBufferLen++] = frameData[frameIdx];
                            // Reset Escape Flag.
                            escape = false;
                        }
                        else
                        {
                            // Received byte is an escape character. Set Escape flag to escape next byte.
                            escape = true;
                        }
                        break;

                    default: // Data field.
                        outBuffer[outBufferLen++] = frameData[frameIdx];
                        // Reset Escape Flag.
                        escape = false;
                        break;

                }

                frameIdx++;
            }
            return frameValid ? outBufferLen : 0;
        }

        private static UInt16[] crc_table = new UInt16[]
        {
            0x0000, 0x1021, 0x2042, 0x3063, 0x4084, 0x50a5, 0x60c6, 0x70e7,
            0x8108, 0x9129, 0xa14a, 0xb16b, 0xc18c, 0xd1ad, 0xe1ce, 0xf1ef
        };

        /// <summary>
        /// Calculate CRC for the given data and len
        /// </summary>
        /// <param name="data">data pointer</param>
        /// <param name="start">start location in array</param>
        /// <param name="len">data length</param>
        /// <returns></returns>
        UInt16 CalculateCrc(Byte[] data, uint start, uint len)
        {
            UInt16 i;
            UInt16 crc = 0;
            uint idx = start;

            while (len-- > 0)
            {
                i = (UInt16)((crc >> 12) ^ (data[idx] >> 4));
                crc = (UInt16)(crc_table[i & 0x0F] ^ (crc << 4));
                i = (UInt16)((crc >> 12) ^ (data[idx] >> 0));
                crc = (UInt16)(crc_table[i & 0x0F] ^ (crc << 4));
                idx++;
            }

            return (UInt16)(crc & 0xFFFF);
        }

    }
}
