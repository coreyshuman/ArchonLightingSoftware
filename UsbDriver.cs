using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;


namespace ArchonLightingSystem
{
    class UsbDriver : UsbSystemDefinitions
    {
        public const uint USB_BUFFER_SIZE = 64;

        public bool IsAttached = false;                     //Need to keep track of the USB device attachment status for proper plug and play operation.
        public bool IsAttachedButBroken = false;
        

        private SafeFileHandle WriteHandleToUSBDevice = null;
        private SafeFileHandle ReadHandleToUSBDevice = null;
        private string DevicePath = null;   //Need the find the proper device path before you can open file handles.
        //Globally Unique Identifier (GUID) for HID class devices.  Windows uses GUIDs to identify things.
        private Guid InterfaceClassGuid = new Guid(0x4d1e55b2, 0xf16f, 0x11cf, 0x88, 0xcb, 0x00, 0x11, 0x11, 0x00, 0x00, 0x30);
        

        

        private string devicePid = "0000";
        private string deviceVid = "0000";

        public string DevicePid
        {
            get
            {
                return devicePid;
            }
            set
            {
                if (value.Length != 4) return;
                devicePid = value;
            }
        }

        public string DeviceVid
        {
            get
            {
                return deviceVid;
            }
            set
            {
                if (value.Length != 4) return;
                deviceVid = value;
            }
        }

        public UsbDriver()
        {
            
        }

        public void InitializeDevice(string vid, string pid)
        {
            DeviceVid = vid;
            DevicePid = pid;
            UpdateDeviceStatus();
        }

        public void RegisterEventHandler(IntPtr handle)
        {
            //Register for WM_DEVICECHANGE notifications.  This code uses these messages to detect plug and play connection/disconnection events for USB devices
            DEV_BROADCAST_DEVICEINTERFACE DeviceBroadcastHeader = new DEV_BROADCAST_DEVICEINTERFACE();
            DeviceBroadcastHeader.dbcc_devicetype = DBT_DEVTYP_DEVICEINTERFACE;
            DeviceBroadcastHeader.dbcc_size = (uint)Marshal.SizeOf(DeviceBroadcastHeader);
            DeviceBroadcastHeader.dbcc_reserved = 0;	//Reserved says not to use...
            DeviceBroadcastHeader.dbcc_classguid = InterfaceClassGuid;

            //Need to get the address of the DeviceBroadcastHeader to call RegisterDeviceNotification(), but
            //can't use "&DeviceBroadcastHeader".  Instead, using a roundabout means to get the address by 
            //making a duplicate copy using Marshal.StructureToPtr().
            IntPtr pDeviceBroadcastHeader = IntPtr.Zero;  //Make a pointer.
            pDeviceBroadcastHeader = Marshal.AllocHGlobal(Marshal.SizeOf(DeviceBroadcastHeader)); //allocate memory for a new DEV_BROADCAST_DEVICEINTERFACE structure, and return the address 
            Marshal.StructureToPtr(DeviceBroadcastHeader, pDeviceBroadcastHeader, false);  //Copies the DeviceBroadcastHeader structure into the memory already allocated at DeviceBroadcastHeaderWithPointer
            RegisterDeviceNotification(handle, pDeviceBroadcastHeader, DEVICE_NOTIFY_WINDOW_HANDLE);
        }

        public void HandleWindowEvent(ref System.Windows.Forms.Message m)
        {
            if (m.Msg == WM_DEVICECHANGE)
            {
                if (((int)m.WParam == DBT_DEVICEARRIVAL) || ((int)m.WParam == DBT_DEVICEREMOVEPENDING) || ((int)m.WParam == DBT_DEVICEREMOVECOMPLETE) || ((int)m.WParam == DBT_CONFIGCHANGED))
                {
                    //WM_DEVICECHANGE messages by themselves are quite generic, and can be caused by a number of different
                    //sources, not just your USB hardware device.  Therefore, must check to find out if any changes relavant
                    //to your device (with known VID/PID) took place before doing any kind of opening or closing of handles/endpoints.
                    //(the message could have been totally unrelated to your application/USB device)
                    UpdateDeviceStatus();
                }
            }
            
        }

        private void UpdateDeviceStatus()
        {
            if (CheckIfPresentAndGetUSBDevicePath())    //Check and make sure at least one device with matching VID/PID is attached
            {
                //If executes to here, this means the device is currently attached and was found.
                //This code needs to decide however what to do, based on whether or not the device was previously known to be
                //attached or not.
                if ((IsAttached == false) || (IsAttachedButBroken == true))    //Check the previous attachment state
                {
                    uint ErrorStatusWrite;
                    uint ErrorStatusRead;

                    //We obtained the proper device path (from CheckIfPresentAndGetUSBDevicePath() function call), and it
                    //is now possible to open read and write handles to the device.
                    WriteHandleToUSBDevice = CreateFile(DevicePath, GENERIC_WRITE, FILE_SHARE_READ | FILE_SHARE_WRITE, IntPtr.Zero, OPEN_EXISTING, 0, IntPtr.Zero);
                    ErrorStatusWrite = (uint)Marshal.GetLastWin32Error();
                    ReadHandleToUSBDevice = CreateFile(DevicePath, GENERIC_READ, FILE_SHARE_READ | FILE_SHARE_WRITE, IntPtr.Zero, OPEN_EXISTING, 0, IntPtr.Zero);
                    ErrorStatusRead = (uint)Marshal.GetLastWin32Error();

                    if ((ErrorStatusWrite == ERROR_SUCCESS) && (ErrorStatusRead == ERROR_SUCCESS))
                    {
                        IsAttached = true;       //Let the rest of the PC application know the USB device is connected, and it is safe to read/write to it
                        IsAttachedButBroken = false;
                    }
                    else //for some reason the device was physically plugged in, but one or both of the read/write handles didn't open successfully...
                    {
                        IsAttached = false;      //Let the rest of this application known not to read/write to the device.
                        IsAttachedButBroken = true;   //Flag so that next time a WM_DEVICECHANGE message occurs, can retry to re-open read/write pipes
                        if (ErrorStatusWrite == ERROR_SUCCESS)
                            WriteHandleToUSBDevice.Close();
                        if (ErrorStatusRead == ERROR_SUCCESS)
                            ReadHandleToUSBDevice.Close();
                    }
                }
                //else we did find the device, but IsAttached was already true.  In this case, don't do anything to the read/write handles,
                //since the WM_DEVICECHANGE message presumably wasn't caused by our USB device.  
            }
            else    //Device must not be connected (or not programmed with correct firmware)
            {
                if (IsAttached == true)      //If it is currently set to true, that means the device was just now disconnected
                {
                    IsAttached = false;
                    WriteHandleToUSBDevice.Close();
                    ReadHandleToUSBDevice.Close();
                }
                IsAttached = false;
                IsAttachedButBroken = false;
            }

            if(!IsAttached)
            {
                WriteHandleToUSBDevice = null;
                ReadHandleToUSBDevice = null;
            }
            else
            {
                /*
                Byte[] OUTBuffer = new byte[65];
                uint BytesWritten = 0;
                OUTBuffer[0] = 0x00;   
                OUTBuffer[1] = 1;
                OUTBuffer[2] = 53;
                OUTBuffer[3] = 246;
                OUTBuffer[4] = 102;
                OUTBuffer[5] = 4;
                for (uint i = 6; i < 65; i++)
                    OUTBuffer[i] = 0xFF;

                //To get the ADCValue, first, we send a packet with our "READ_POT" command in it.
                bool res = WriteFile(WriteHandleToUSBDevice, OUTBuffer, 65, ref BytesWritten, IntPtr.Zero);
                uint err = (uint)Marshal.GetLastWin32Error();
                */
            }
        }

        /// <summary>
        /// Wrapper function to call ReadFile().
        /// 
        /// Wrapper function used to call the ReadFile() function.  ReadFile() takes a pointer to an unmanaged buffer and deposits
        ///          the bytes read into the buffer.  However, can't pass a pointer to a managed buffer directly to ReadFile().
        ///          This ReadFileManagedBuffer() is a wrapper function to make it so application code can call ReadFile() easier
        ///          by specifying a managed buffer.
        /// </summary>
        /// <param name="hFile"></param>
        /// <param name="INBuffer"></param>
        /// <param name="nNumberOfBytesToRead"></param>
        /// <param name="lpNumberOfBytesRead"></param>
        /// <param name="lpOverlapped"></param>
        /// <returns>Returns boolean indicating if the function call was successful or not. Also returns data in the byte[] INBuffer, and the number of bytes read.</returns>
        private unsafe bool ReadFileManagedBuffer(SafeFileHandle hFile, byte[] INBuffer, uint nNumberOfBytesToRead, ref uint lpNumberOfBytesRead, IntPtr lpOverlapped)
        {
            IntPtr pINBuffer = IntPtr.Zero;

            try
            {
                pINBuffer = Marshal.AllocHGlobal((int)nNumberOfBytesToRead);    //Allocate some unmanged RAM for the receive data buffer.

                if (ReadFile(hFile, pINBuffer, nNumberOfBytesToRead, ref lpNumberOfBytesRead, lpOverlapped))
                {
                    Marshal.Copy(pINBuffer, INBuffer, 0, (int)lpNumberOfBytesRead);    //Copy over the data from unmanged memory into the managed byte[] INBuffer
                    Marshal.FreeHGlobal(pINBuffer);
                    return true;
                }
                else
                {
                    Marshal.FreeHGlobal(pINBuffer);
                    return false;
                }

            }
            catch
            {
                if (pINBuffer != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(pINBuffer);
                }
                return false;
            }
        }


         /// <summary>
         /// Write buffer to USB device
         /// </summary>
         /// <param name="buffer">buffer bytes</param>
         /// <param name="bufflen">buffer length</param>
         /// <returns>Number of bytes written</returns>
        internal uint WriteUSBDevice(Byte[] buffer, uint bufflen)
        {
            bool result = false;
            uint bytesWritten = 0;
            Byte[] usbReport = new Byte[USB_BUFFER_SIZE + 1];
            int i;

            if (this.IsAttached == false)
            {
                return 0;
            }

            if (this.WriteHandleToUSBDevice != null)
            {
               // while (bufflen > 0)
                {
                    // Set output buffer to 0xFF.
                    for(i=0; i<USB_BUFFER_SIZE+1; i++)
                    {
                        usbReport[i] = 0xFF;
                    }
                    usbReport[0] = 0;  // Report ID = 0
                    for(i=0; i<bufflen; i++)
                    {
                        usbReport[i + 1] = buffer[i];
                    }

                    // WriteFile is a blocking call. It will be a good design if made a non-blocking.
                    result = WriteFile(this.WriteHandleToUSBDevice, usbReport, USB_BUFFER_SIZE + 1, ref bytesWritten, IntPtr.Zero);
                    uint error = (uint)Marshal.GetLastWin32Error();
                }

            }

            return result ? bytesWritten : 0;

        }

         /// <summary>
         /// Read from USB device
         /// </summary>
         /// <param name="buffer">buffer array</param>
         /// <param name="bufflen">buffer length</param>
         /// <returns></returns>
        internal uint ReadUSBDevice(ref Byte[] buffer, uint bufflen)
        {

            uint bytesRead = 0;
            bool result;

            if (this.IsAttached == false)
            {
                return 0;
            }

            if (this.ReadHandleToUSBDevice == null)
            {
                return 0;
            }

            // Set the first byte in the buffer to the Report ID.
            buffer[0] = 0;


            result = ReadFileManagedBuffer(this.ReadHandleToUSBDevice, buffer, bufflen, ref bytesRead, IntPtr.Zero);

            return result ? bytesRead : 0;
        }

        /// <summary>
        /// Check if a USB device is currently plugged in with a matching VID and PID.
        /// </summary>
        /// <returns>Returns BOOL.  TRUE when device with matching VID/PID found.  FALSE if device with VID/PID could not be found.</returns>
        private bool CheckIfPresentAndGetUSBDevicePath()
        {
            /* 
		    Before we can "connect" our application to our USB embedded device, we must first find the device.
		    A USB bus can have many devices simultaneously connected, so somehow we have to find our device only.
		    This is done with the Vendor ID (VID) and Product ID (PID).  Each USB product line should have
		    a unique combination of VID and PID.  

		    Microsoft has created a number of functions which are useful for finding plug and play devices.  Documentation
		    for each function used can be found in the MSDN library.  We will be using the following functions (unmanaged C functions):

		    SetupDiGetClassDevs()					//provided by setupapi.dll, which comes with Windows
		    SetupDiEnumDeviceInterfaces()			//provided by setupapi.dll, which comes with Windows
		    GetLastError()							//provided by kernel32.dll, which comes with Windows
		    SetupDiDestroyDeviceInfoList()			//provided by setupapi.dll, which comes with Windows
		    SetupDiGetDeviceInterfaceDetail()		//provided by setupapi.dll, which comes with Windows
		    SetupDiGetDeviceRegistryProperty()		//provided by setupapi.dll, which comes with Windows
		    CreateFile()							//provided by kernel32.dll, which comes with Windows

            In order to call these unmanaged functions, the Marshal class is very useful.
             
		    We will also be using the following unusual data types and structures.  Documentation can also be found in
		    the MSDN library:

		    PSP_DEVICE_INTERFACE_DATA
		    PSP_DEVICE_INTERFACE_DETAIL_DATA
		    SP_DEVINFO_DATA
		    HDEVINFO
		    HANDLE
		    GUID

		    The ultimate objective of the following code is to get the device path, which will be used elsewhere for getting
		    read and write handles to the USB device.  Once the read/write handles are opened, only then can this
		    PC application begin reading/writing to the USB device using the WriteFile() and ReadFile() functions.

		    Getting the device path is a multi-step round about process, which requires calling several of the
		    SetupDixxx() functions provided by setupapi.dll.
		    */

            try
            {
                IntPtr DeviceInfoTable = IntPtr.Zero;
                SP_DEVICE_INTERFACE_DATA InterfaceDataStructure = new SP_DEVICE_INTERFACE_DATA();
                SP_DEVICE_INTERFACE_DETAIL_DATA DetailedInterfaceDataStructure = new SP_DEVICE_INTERFACE_DETAIL_DATA();
                SP_DEVINFO_DATA DevInfoData = new SP_DEVINFO_DATA();

                uint InterfaceIndex = 0;
                uint dwRegType = 0;
                uint dwRegSize = 0;
                uint dwRegSize2 = 0;
                uint StructureSize = 0;
                IntPtr PropertyValueBuffer = IntPtr.Zero;
                bool MatchFound = false;
                uint ErrorStatus;
                uint LoopCounter = 0;

                string DeviceIDToFind = $"Vid_{deviceVid}&Pid_{devicePid}";

                //First populate a list of plugged in devices (by specifying "DIGCF_PRESENT"), which are of the specified class GUID. 
                DeviceInfoTable = SetupDiGetClassDevs(ref InterfaceClassGuid, IntPtr.Zero, IntPtr.Zero, DIGCF_PRESENT | DIGCF_DEVICEINTERFACE);

                if (DeviceInfoTable != IntPtr.Zero)
                {
                    //Now look through the list we just populated.  We are trying to see if any of them match our device. 
                    while (true)
                    {
                        InterfaceDataStructure.cbSize = (uint)Marshal.SizeOf(InterfaceDataStructure);
                        if (SetupDiEnumDeviceInterfaces(DeviceInfoTable, IntPtr.Zero, ref InterfaceClassGuid, InterfaceIndex, ref InterfaceDataStructure))
                        {
                            ErrorStatus = (uint)Marshal.GetLastWin32Error();
                            if (ErrorStatus == ERROR_NO_MORE_ITEMS) //Did we reach the end of the list of matching devices in the DeviceInfoTable?
                            {   //Cound not find the device.  Must not have been attached.
                                SetupDiDestroyDeviceInfoList(DeviceInfoTable);  //Clean up the old structure we no longer need.
                                return false;
                            }
                        }
                        else    //Else some other kind of unknown error ocurred...
                        {
                            ErrorStatus = (uint)Marshal.GetLastWin32Error();
                            SetupDiDestroyDeviceInfoList(DeviceInfoTable);  //Clean up the old structure we no longer need.
                            return false;
                        }

                        //Now retrieve the hardware ID from the registry.  The hardware ID contains the VID and PID, which we will then 
                        //check to see if it is the correct device or not.

                        //Initialize an appropriate SP_DEVINFO_DATA structure.  We need this structure for SetupDiGetDeviceRegistryProperty().
                        DevInfoData.cbSize = (uint)Marshal.SizeOf(DevInfoData);
                        SetupDiEnumDeviceInfo(DeviceInfoTable, InterfaceIndex, ref DevInfoData);

                        //First query for the size of the hardware ID, so we can know how big a buffer to allocate for the data.
                        SetupDiGetDeviceRegistryProperty(DeviceInfoTable, ref DevInfoData, SPDRP_HARDWAREID, ref dwRegType, IntPtr.Zero, 0, ref dwRegSize);

                        //Allocate a buffer for the hardware ID.
                        //Should normally work, but could throw exception "OutOfMemoryException" if not enough resources available.
                        PropertyValueBuffer = Marshal.AllocHGlobal((int)dwRegSize);

                        //Retrieve the hardware IDs for the current device we are looking at.  PropertyValueBuffer gets filled with a 
                        //REG_MULTI_SZ (array of null terminated strings).  To find a device, we only care about the very first string in the
                        //buffer, which will be the "device ID".  The device ID is a string which contains the VID and PID, in the example 
                        //format "Vid_04d8&Pid_003f".
                        SetupDiGetDeviceRegistryProperty(DeviceInfoTable, ref DevInfoData, SPDRP_HARDWAREID, ref dwRegType, PropertyValueBuffer, dwRegSize, ref dwRegSize2);

                        //Now check if the first string in the hardware ID matches the device ID of the USB device we are trying to find.
                        String DeviceIDFromRegistry = Marshal.PtrToStringUni(PropertyValueBuffer); //Make a new string, fill it with the contents from the PropertyValueBuffer

                        Marshal.FreeHGlobal(PropertyValueBuffer);       //No longer need the PropertyValueBuffer, free the memory to prevent potential memory leaks

                        //Convert both strings to lower case.  This makes the code more robust/portable accross OS Versions
                        DeviceIDFromRegistry = DeviceIDFromRegistry.ToLowerInvariant();
                        DeviceIDToFind = DeviceIDToFind.ToLowerInvariant();
                        //Now check if the hardware ID we are looking at contains the correct VID/PID
                        MatchFound = DeviceIDFromRegistry.Contains(DeviceIDToFind);
                        if (MatchFound == true)
                        {
                            //Device must have been found.  In order to open I/O file handle(s), we will need the actual device path first.
                            //We can get the path by calling SetupDiGetDeviceInterfaceDetail(), however, we have to call this function twice:  The first
                            //time to get the size of the required structure/buffer to hold the detailed interface data, then a second time to actually 
                            //get the structure (after we have allocated enough memory for the structure.)
                            DetailedInterfaceDataStructure.cbSize = (uint)Marshal.SizeOf(DetailedInterfaceDataStructure);
                            //First call populates "StructureSize" with the correct value
                            SetupDiGetDeviceInterfaceDetail(DeviceInfoTable, ref InterfaceDataStructure, IntPtr.Zero, 0, ref StructureSize, IntPtr.Zero);
                            //Need to call SetupDiGetDeviceInterfaceDetail() again, this time specifying a pointer to a SP_DEVICE_INTERFACE_DETAIL_DATA buffer with the correct size of RAM allocated.
                            //First need to allocate the unmanaged buffer and get a pointer to it.
                            IntPtr pUnmanagedDetailedInterfaceDataStructure = IntPtr.Zero;  //Declare a pointer.
                            pUnmanagedDetailedInterfaceDataStructure = Marshal.AllocHGlobal((int)StructureSize);    //Reserve some unmanaged memory for the structure.
                            DetailedInterfaceDataStructure.cbSize = 6; //Initialize the cbSize parameter (4 bytes for DWORD + 2 bytes for unicode null terminator)
                            Marshal.StructureToPtr(DetailedInterfaceDataStructure, pUnmanagedDetailedInterfaceDataStructure, false); //Copy managed structure contents into the unmanaged memory buffer.

                            //Now call SetupDiGetDeviceInterfaceDetail() a second time to receive the device path in the structure at pUnmanagedDetailedInterfaceDataStructure.
                            if (SetupDiGetDeviceInterfaceDetail(DeviceInfoTable, ref InterfaceDataStructure, pUnmanagedDetailedInterfaceDataStructure, StructureSize, IntPtr.Zero, IntPtr.Zero))
                            {
                                //Need to extract the path information from the unmanaged "structure".  The path starts at (pUnmanagedDetailedInterfaceDataStructure + sizeof(DWORD)).
                                IntPtr pToDevicePath = new IntPtr((uint)pUnmanagedDetailedInterfaceDataStructure.ToInt32() + 4);  //Add 4 to the pointer (to get the pointer to point to the path, instead of the DWORD cbSize parameter)
                                DevicePath = Marshal.PtrToStringUni(pToDevicePath); //Now copy the path information into the globally defined DevicePath String.

                                //We now have the proper device path, and we can finally use the path to open I/O handle(s) to the device.
                                SetupDiDestroyDeviceInfoList(DeviceInfoTable);	//Clean up the old structure we no longer need.
                                Marshal.FreeHGlobal(pUnmanagedDetailedInterfaceDataStructure);  //No longer need this unmanaged SP_DEVICE_INTERFACE_DETAIL_DATA buffer.  We already extracted the path information.
                                return true;    //Returning the device path in the global DevicePath String
                            }
                            else //Some unknown failure occurred
                            {
                                uint ErrorCode = (uint)Marshal.GetLastWin32Error();
                                SetupDiDestroyDeviceInfoList(DeviceInfoTable);	//Clean up the old structure.
                                Marshal.FreeHGlobal(pUnmanagedDetailedInterfaceDataStructure);  //No longer need this unmanaged SP_DEVICE_INTERFACE_DETAIL_DATA buffer.  We already extracted the path information.
                                return false;
                            }
                        }

                        InterfaceIndex++;
                        //Keep looping until we either find a device with matching VID and PID, or until we run out of devices to check.
                        //However, just in case some unexpected error occurs, keep track of the number of loops executed.
                        //If the number of loops exceeds a very large number, exit anyway, to prevent inadvertent infinite looping.
                        LoopCounter++;
                        if (LoopCounter == 10000000)    //Surely there aren't more than 10 million devices attached to any forseeable PC...
                        {
                            return false;
                        }
                    }//end of while(true)
                }
                return false;
            }//end of try
            catch
            {
                //Something went wrong if PC gets here.  Maybe a Marshal.AllocHGlobal() failed due to insufficient resources or something.
                return false;
            }
        }
    }
}
