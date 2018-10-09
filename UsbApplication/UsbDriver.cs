using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Threading;

namespace ArchonLightingSystem.UsbApplication
{
    public partial class UsbDevice
    {
        public bool IsAttached { get; set; }
        public bool IsAttachedButBroken { get; set; }
        public bool IsFound { get; set; } // used to verify device is attached when system event occurs
        public SafeFileHandle WriteHandleToUSBDevice { get; set; }
        public SafeFileHandle ReadHandleToUSBDevice { get; set; }
        public string DevicePath { get; set; }
    }

    public class UsbDriver : UsbSystemDefinitions
    {
        public const uint USB_PACKET_SIZE = 64 + 1;

        protected List<UsbDevice> usbDevices = new List<UsbDevice>();
        //Globally Unique Identifier (GUID) for HID class devices.  Windows uses GUIDs to identify things.
        private Guid InterfaceClassGuid = new Guid(0x4d1e55b2, 0xf16f, 0x11cf, 0x88, 0xcb, 0x00, 0x11, 0x11, 0x00, 0x00, 0x30);
        private string devicePid = "0000";
        private string deviceVid = "0000";
        private string deviceIDToFind = "Vid_0000&Pid_0000";

        public int DeviceCount
        {
            get
            {
                return usbDevices.Count;
            }
        }

        public IList<UsbDevice> UsbDevices
        {
            get
            {
                return usbDevices;
            }
        }

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
            deviceIDToFind = $"Vid_{deviceVid}&Pid_{devicePid}".ToLowerInvariant();
            UpdateDeviceStatus();
        }

        public bool IsAttached(int deviceIdx)
        {
            if (deviceIdx >= usbDevices.Count)
            {
                return false;
            }
            return usbDevices[deviceIdx].IsAttached;
        }

        public bool IsAttachedButBroken(int deviceIdx)
        {
            if (deviceIdx >= usbDevices.Count)
            {
                return false;
            }
            return usbDevices[deviceIdx].IsAttachedButBroken;
        }

        internal UsbDevice GetDevice(int deviceIdx)
        {
            if(deviceIdx >= usbDevices.Count)
            {
                return null;
            }
            return usbDevices[deviceIdx];
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

        protected void DetachDevice(UsbDevice device)
        {
            if (device.WriteHandleToUSBDevice?.IsClosed == false)
            {
                device.WriteHandleToUSBDevice.Close();
            }
            if (device.ReadHandleToUSBDevice?.IsClosed == false)
            {
                device.ReadHandleToUSBDevice.Close();
            }
            device.IsAttached = false;
            device.IsAttachedButBroken = false;
            device.DevicePath = "";
        }

        private void UpdateDeviceStatus()
        {
            if (CheckIfPresentAndGetUSBDevicePath())    //Check and make sure at least one device with matching VID/PID is attached
            {
                usbDevices.ForEach(device =>
                {
                    //If executes to here, this means the device is currently attached and was found.
                    //This code needs to decide however what to do, based on whether or not the device was previously known to be
                    //attached or not.
                    if (device.IsFound && ((device.IsAttached == false) || (device.IsAttachedButBroken == true)))    //Check the previous attachment state
                    {
                        uint ErrorStatusWrite;
                        uint ErrorStatusRead;

                        //We obtained the proper device path (from CheckIfPresentAndGetUSBDevicePath() function call), and it
                        //is now possible to open read and write handles to the device.
                        device.WriteHandleToUSBDevice = CreateFile(device.DevicePath, GENERIC_WRITE, FILE_SHARE_READ | FILE_SHARE_WRITE, IntPtr.Zero, OPEN_EXISTING, FILE_FLAG_OVERLAPPED, IntPtr.Zero);
                        ErrorStatusWrite = (uint)Marshal.GetLastWin32Error();
                        device.ReadHandleToUSBDevice = CreateFile(device.DevicePath, GENERIC_READ, FILE_SHARE_READ | FILE_SHARE_WRITE, IntPtr.Zero, OPEN_EXISTING, FILE_FLAG_OVERLAPPED, IntPtr.Zero);
                        ErrorStatusRead = (uint)Marshal.GetLastWin32Error();

                        if ((ErrorStatusWrite == ERROR_SUCCESS) && (ErrorStatusRead == ERROR_SUCCESS))
                        {
                            device.IsAttached = true;       //Let the rest of the PC application know the USB device is connected, and it is safe to read/write to it
                            device.IsAttachedButBroken = false;
                        }
                        else //for some reason the device was physically plugged in, but one or both of the read/write handles didn't open successfully...
                        {
                            device.IsAttached = false;      //Let the rest of this application known not to read/write to the device.
                            device.IsAttachedButBroken = true;   //Flag so that next time a WM_DEVICECHANGE message occurs, can retry to re-open read/write pipes
                            if (ErrorStatusWrite == ERROR_SUCCESS)
                                device.WriteHandleToUSBDevice.Close();
                            if (ErrorStatusRead == ERROR_SUCCESS)
                                device.ReadHandleToUSBDevice.Close();
                        }
                    }
                    //Device must not be connected (or not programmed with correct firmware)
                    else if (!device.IsFound && (device.IsAttached || device.IsAttachedButBroken))
                    {
                        DetachDevice(device);
                    }
                    //else we did find the device, but IsAttached was already true.  In this case, don't do anything to the read/write handles,
                    //since the WM_DEVICECHANGE message presumably wasn't caused by our USB device.  
                });
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
        private unsafe bool ReadFileManagedBuffer(SafeFileHandle hFile, byte[] INBuffer, uint nNumberOfBytesToRead, ref uint lpNumberOfBytesRead, ref OVERLAPPED lpOverlapped)
        {
            IntPtr pINBuffer = IntPtr.Zero;

            try
            {
                pINBuffer = Marshal.AllocHGlobal((int)nNumberOfBytesToRead);    //Allocate some unmanged RAM for the receive data buffer.

                if (ReadFile(hFile, pINBuffer, nNumberOfBytesToRead, ref lpNumberOfBytesRead, ref lpOverlapped))
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
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
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
        /// <param name="device">device class with device info</param>
        /// <param name="buffer">buffer bytes</param>
        /// <param name="bufflen">buffer length</param>
        /// <returns>Number of bytes written</returns>
        internal uint WriteUSBDevice(UsbDevice device, Byte[] buffer, uint bufflen)
        {
            OVERLAPPED HIDOverlapped = new OVERLAPPED();
            SafeFileHandle hEventObject = null;
            uint result = 0;
            uint bytesWritten = 0;
            Byte[] usbReport = new Byte[USB_PACKET_SIZE];
            int i;

            if (device.IsAttached == false)
            {
                return 0;
            }

            if (device.WriteHandleToUSBDevice == null)
            {
                return 0;
            }

            // while (bufflen > 0)
            {
                // Set output buffer to 0xFF.
                for (i = 0; i < USB_PACKET_SIZE; i++)
                {
                    usbReport[i] = 0xFF;
                }
                usbReport[0] = 0;  // Report ID = 0
                for (i = 0; i < bufflen; i++)
                {
                    usbReport[i + 1] = buffer[i];
                }

                try
                {
                    // use overlapped structure
                    hEventObject = CreateEvent(IntPtr.Zero, false, true, IntPtr.Zero);
                    HIDOverlapped.hEvent = hEventObject.DangerousGetHandle();
                    HIDOverlapped.Offset = 0;
                    HIDOverlapped.OffsetHigh = 0;

                    WriteFile(device.WriteHandleToUSBDevice, usbReport, USB_PACKET_SIZE, ref bytesWritten, ref HIDOverlapped);
                    if (Marshal.GetLastWin32Error() == ERROR_IO_PENDING)
                    {
                        result = WaitForSingleObject(hEventObject, 200); //200ms timeout period

                        switch (result)
                        {
                            case WAIT_OBJECT_0: 
                                // Success;
                                if (GetOverlappedResult(device.WriteHandleToUSBDevice, ref HIDOverlapped, ref bytesWritten, false))
                                {
                                    return bytesWritten;
                                }
                                CancelIo(device.WriteHandleToUSBDevice);
                                break;

                            case WAIT_TIMEOUT: 
                                // Timeout error;
                                CancelIo(device.WriteHandleToUSBDevice);
                                break;

                            default:
                                // Undefined error;
                                uint ErrorCode = (uint)Marshal.GetLastWin32Error();
                                Trace.WriteLine($"Undefined UsbWrite Error. WaitForSingleObject=[{ErrorCode.ToString("X8")}] LastWin32Error=[{result.ToString("X8")}]");
                                CancelIo(device.WriteHandleToUSBDevice);
                                break;

                        }
                    }
                }
                finally
                {
                    hEventObject?.Dispose();
                }
            }
            return 0;
        }

        /// <summary>
        /// Read from USB device
        /// </summary>
        /// <param name="device">device class with device info</param>
        /// <param name="buffer">buffer array</param>
        /// <param name="bufflen">buffer length</param>
        /// <param name="readTimeout">timeout for read operation</param>
        /// <returns></returns>
        internal uint ReadUSBDevice(UsbDevice device, ref Byte[] buffer, uint bufflen, uint readTimeout = 200)
        {
            OVERLAPPED HIDOverlapped = new OVERLAPPED();
            SafeFileHandle hEventObject = null;
            IntPtr pINBuffer = IntPtr.Zero;
            uint bytesRead = 0;
            UInt32 result;

            if (device.IsAttached == false)
            {
                return 0;
            }

            if (device.ReadHandleToUSBDevice == null)
            {
                return 0;
            }

            // Set the first byte in the buffer to the Report ID.
            buffer[0] = 0;

            try
            {
                // use overlapped structure
                hEventObject = CreateEvent(IntPtr.Zero, false, true, IntPtr.Zero);
                HIDOverlapped.hEvent = hEventObject.DangerousGetHandle();
                HIDOverlapped.Offset = 0;
                HIDOverlapped.OffsetHigh = 0;

                pINBuffer = Marshal.AllocHGlobal((int)bufflen);    //Allocate some unmanged RAM for the receive data buffer.

                //ReadFileManagedBuffer(device.ReadHandleToUSBDevice, buffer, bufflen, ref bytesRead, ref HIDOverlapped);
                if(ReadFile(device.ReadHandleToUSBDevice, pINBuffer, bufflen, ref bytesRead, ref HIDOverlapped))
                {
                    Marshal.Copy(pINBuffer, buffer, 0, (int)bytesRead);    //Copy over the data from unmanged memory into the managed byte[] INBuffer
                    return bytesRead;
                }
                else if (Marshal.GetLastWin32Error() == ERROR_IO_PENDING)
                {
                    result = WaitForSingleObject(hEventObject, readTimeout);

                    switch (result)
                    {
                        case WAIT_OBJECT_0:
                            // Success;
                            if (GetOverlappedResult(device.ReadHandleToUSBDevice, ref HIDOverlapped, ref bytesRead, false))
                            {
                                Marshal.Copy(pINBuffer, buffer, 0, (int)bytesRead);    //Copy over the data from unmanged memory into the managed byte[] INBuffer
                                return bytesRead;
                            }
                            CancelIo(device.ReadHandleToUSBDevice);
                            break;

                        case WAIT_TIMEOUT:
                            // Timeout error;
                            CancelIo(device.ReadHandleToUSBDevice);
                            break;

                        default:
                            // Undefined error;
                            uint ErrorCode = (uint)Marshal.GetLastWin32Error();
                            Trace.WriteLine($"Undefined UsbRead Error. WaitForSingleObject=[{ErrorCode.ToString("X8")}] LastWin32Error=[{result.ToString("X8")}]");
                            CancelIo(device.ReadHandleToUSBDevice);
                            break;

                    }
                }
            }
            finally
            {
                hEventObject?.Dispose();
                if (pINBuffer != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(pINBuffer);
                }
            }

            
            return 0;
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
                IntPtr deviceInfoTable = IntPtr.Zero;
                SP_DEVICE_INTERFACE_DATA interfaceDataStructure = new SP_DEVICE_INTERFACE_DATA();
                SP_DEVICE_INTERFACE_DETAIL_DATA detailedInterfaceDataStructure = new SP_DEVICE_INTERFACE_DETAIL_DATA();
                SP_DEVINFO_DATA devInfoData = new SP_DEVINFO_DATA();

                uint interfaceIndex = 0;
                uint dwRegType = 0;
                uint dwRegSize = 0;
                uint dwRegSize2 = 0;
                uint structureSize = 0;
                IntPtr propertyValueBuffer = IntPtr.Zero;
                bool matchFound = false;
                uint errorStatus;
                uint loopCounter = 0;

                Trace.WriteLine("USB CheckIfPresent");

                // clear check flag on existing devices so we can verify they are plugged in
                usbDevices.Select(dev => { return dev.IsFound = false; }).ToList();

                //First populate a list of plugged in devices (by specifying "DIGCF_PRESENT"), which are of the specified class GUID. 
                deviceInfoTable = SetupDiGetClassDevs(ref InterfaceClassGuid, IntPtr.Zero, IntPtr.Zero, DIGCF_PRESENT | DIGCF_DEVICEINTERFACE);

                if (deviceInfoTable != IntPtr.Zero)
                {
                    //Now look through the list we just populated.  We are trying to see if any of them match our device. 
                    while (true)
                    {
                        interfaceDataStructure.cbSize = (uint)Marshal.SizeOf(interfaceDataStructure);
                        if (SetupDiEnumDeviceInterfaces(deviceInfoTable, IntPtr.Zero, ref InterfaceClassGuid, interfaceIndex, ref interfaceDataStructure))
                        {
                            errorStatus = (uint)Marshal.GetLastWin32Error();
                            if (errorStatus == ERROR_NO_MORE_ITEMS) //Did we reach the end of the list of matching devices in the DeviceInfoTable?
                            {   
                                SetupDiDestroyDeviceInfoList(deviceInfoTable);  //Clean up the old structure we no longer need.
                                break;
                            }
                        }
                        else    //Else some other kind of unknown error ocurred...
                        {
                            errorStatus = (uint)Marshal.GetLastWin32Error();
                            SetupDiDestroyDeviceInfoList(deviceInfoTable);  //Clean up the old structure we no longer need.
                            break;
                        }

                        //Now retrieve the hardware ID from the registry.  The hardware ID contains the VID and PID, which we will then 
                        //check to see if it is the correct device or not.

                        //Initialize an appropriate SP_DEVINFO_DATA structure.  We need this structure for SetupDiGetDeviceRegistryProperty().
                        devInfoData.cbSize = (uint)Marshal.SizeOf(devInfoData);
                        SetupDiEnumDeviceInfo(deviceInfoTable, interfaceIndex, ref devInfoData);

                        //First query for the size of the hardware ID, so we can know how big a buffer to allocate for the data.
                        SetupDiGetDeviceRegistryProperty(deviceInfoTable, ref devInfoData, SPDRP_HARDWAREID, ref dwRegType, IntPtr.Zero, 0, ref dwRegSize);

                        //Allocate a buffer for the hardware ID.
                        //Should normally work, but could throw exception "OutOfMemoryException" if not enough resources available.
                        propertyValueBuffer = Marshal.AllocHGlobal((int)dwRegSize);

                        //Retrieve the hardware IDs for the current device we are looking at.  PropertyValueBuffer gets filled with a 
                        //REG_MULTI_SZ (array of null terminated strings).  To find a device, we only care about the very first string in the
                        //buffer, which will be the "device ID".  The device ID is a string which contains the VID and PID, in the example 
                        //format "Vid_04d8&Pid_003f".
                        SetupDiGetDeviceRegistryProperty(deviceInfoTable, ref devInfoData, SPDRP_HARDWAREID, ref dwRegType, propertyValueBuffer, dwRegSize, ref dwRegSize2);

                        //Now check if the first string in the hardware ID matches the device ID of the USB device we are trying to find.
                        String DeviceIDFromRegistry = Marshal.PtrToStringUni(propertyValueBuffer); //Make a new string, fill it with the contents from the PropertyValueBuffer

                        Marshal.FreeHGlobal(propertyValueBuffer);       //No longer need the PropertyValueBuffer, free the memory to prevent potential memory leaks

                        //Convert both strings to lower case.  This makes the code more robust/portable accross OS Versions
                        DeviceIDFromRegistry = DeviceIDFromRegistry.ToLowerInvariant();
                        
                        //Now check if the hardware ID we are looking at contains the correct VID/PID
                        matchFound = DeviceIDFromRegistry.Contains(deviceIDToFind);
                        if (matchFound == true)
                        {
                            //Device must have been found.  In order to open I/O file handle(s), we will need the actual device path first.
                            //We can get the path by calling SetupDiGetDeviceInterfaceDetail(), however, we have to call this function twice:  The first
                            //time to get the size of the required structure/buffer to hold the detailed interface data, then a second time to actually 
                            //get the structure (after we have allocated enough memory for the structure.)
                            detailedInterfaceDataStructure.cbSize = (uint)Marshal.SizeOf(detailedInterfaceDataStructure);
                            //First call populates "StructureSize" with the correct value
                            SetupDiGetDeviceInterfaceDetail(deviceInfoTable, ref interfaceDataStructure, IntPtr.Zero, 0, ref structureSize, IntPtr.Zero);
                            //Need to call SetupDiGetDeviceInterfaceDetail() again, this time specifying a pointer to a SP_DEVICE_INTERFACE_DETAIL_DATA buffer with the correct size of RAM allocated.
                            //First need to allocate the unmanaged buffer and get a pointer to it.
                            IntPtr pUnmanagedDetailedInterfaceDataStructure = IntPtr.Zero;  //Declare a pointer.
                            pUnmanagedDetailedInterfaceDataStructure = Marshal.AllocHGlobal((int)structureSize);    //Reserve some unmanaged memory for the structure.
                            detailedInterfaceDataStructure.cbSize = 6; //Initialize the cbSize parameter (4 bytes for DWORD + 2 bytes for unicode null terminator)
                            Marshal.StructureToPtr(detailedInterfaceDataStructure, pUnmanagedDetailedInterfaceDataStructure, false); //Copy managed structure contents into the unmanaged memory buffer.

                            //Now call SetupDiGetDeviceInterfaceDetail() a second time to receive the device path in the structure at pUnmanagedDetailedInterfaceDataStructure.
                            if (SetupDiGetDeviceInterfaceDetail(deviceInfoTable, ref interfaceDataStructure, pUnmanagedDetailedInterfaceDataStructure, structureSize, IntPtr.Zero, IntPtr.Zero))
                            {
                                //Need to extract the path information from the unmanaged "structure".  The path starts at (pUnmanagedDetailedInterfaceDataStructure + sizeof(DWORD)).
                                IntPtr pToDevicePath = new IntPtr((uint)pUnmanagedDetailedInterfaceDataStructure.ToInt32() + 4);  //Add 4 to the pointer (to get the pointer to point to the path, instead of the DWORD cbSize parameter)
                                string devicePath = Marshal.PtrToStringUni(pToDevicePath); //Now copy the path information into the globally defined DevicePath String.

                                // if device already exists in our list, set IsStillActive to true. Otherwise, add to list
                                var foundDevice = usbDevices.Where(dev => dev.DevicePath == devicePath).FirstOrDefault();
                                if(foundDevice != null)
                                {
                                    usbDevices[usbDevices.IndexOf(foundDevice)].IsFound = true;
                                }
                                else
                                {
                                    usbDevices.Add(new UsbDevice { DevicePath = devicePath, IsFound = true });
                                }

                                //We now have the proper device path, and we can finally use the path to open I/O handle(s) to the device.
                                Marshal.FreeHGlobal(pUnmanagedDetailedInterfaceDataStructure);  //No longer need this unmanaged SP_DEVICE_INTERFACE_DETAIL_DATA buffer.  We already extracted the path information.
                            }
                            else //Some unknown failure occurred
                            {
                                uint ErrorCode = (uint)Marshal.GetLastWin32Error();
                                Marshal.FreeHGlobal(pUnmanagedDetailedInterfaceDataStructure);  //No longer need this unmanaged SP_DEVICE_INTERFACE_DETAIL_DATA buffer.  We already extracted the path information.
                            }
                        }

                        interfaceIndex++;
                        //Keep looping until we either find a device with matching VID and PID, or until we run out of devices to check.
                        //However, just in case some unexpected error occurs, keep track of the number of loops executed.
                        //If the number of loops exceeds a very large number, exit anyway, to prevent inadvertent infinite looping.
                        loopCounter++;
                        if (loopCounter == 10000000)    //Surely there aren't more than 10 million devices attached to any forseeable PC...
                        {
                            break;
                        }
                    }//end of while(true)

                    SetupDiDestroyDeviceInfoList(deviceInfoTable);	//Clean up the old structure we no longer need.

                    var deviceFoundCount = usbDevices.Where(dev => dev.IsFound == true).Count();
                    Trace.WriteLine($"USB found {deviceFoundCount}");
                    return deviceFoundCount > 0;
                }
                return false;
            }//end of try
            catch(Exception ex)
            {
                //Something went wrong if PC gets here.  Maybe a Marshal.AllocHGlobal() failed due to insufficient resources or something.
                Trace.WriteLine($"CheckIfPresent Error: {ex.ToString()}");
                return false;
            }
        }
    }
}
