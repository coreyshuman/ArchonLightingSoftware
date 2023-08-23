﻿using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Threading;
using ArchonLightingSystem.Common;

namespace ArchonLightingSystem.UsbApplicationV2
{
    public class UsbDeviceEventArgs : EventArgs
    {
        public int EventCount { get; internal set; }
        public List<UsbDevice> ConnectedDevices { get; } 
        public List<UsbDevice> DisconnectedDevices { get; }

        public UsbDeviceEventArgs()
        {
            ConnectedDevices = new List<UsbDevice>();
            DisconnectedDevices = new List<UsbDevice>();
        }
    }

    public class UsbDeviceManager : UsbSystemDefinitions
    {
        public event EventHandler<UsbDeviceEventArgs> UsbDriverEvent;

        protected List<UsbDevice> usbDevices = new List<UsbDevice>();
        //Globally Unique Identifier (GUID) for HID class devices.  Windows uses GUIDs to identify things.
        private Guid InterfaceClassGuid = new Guid(0x4d1e55b2, 0xf16f, 0x11cf, 0x88, 0xcb, 0x00, 0x11, 0x11, 0x00, 0x00, 0x30);
        private string devicePid = "0000";
        private string deviceVid = "0000";
        private string deviceIDToFind = "Vid_0000&Pid_0000";
        private readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);

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

        public UsbDeviceManager()
        {

        }

        public void RegisterUsbDevice(string vid, string pid)
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
            if (deviceIdx >= usbDevices.Count)
            {
                return null;
            }
            return usbDevices[deviceIdx];
        }

        protected virtual void OnUsbDriverEvent(UsbDeviceEventArgs e)
        {
            UsbDriverEvent?.Invoke(this, e);
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
                    Logger.Write(Level.Debug, $"UsbManger {deviceIDToFind} Windows Event {m.WParam}, {m.LParam}");
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
        }

        private void UpdateDeviceStatus()
        {
            UsbDeviceEventArgs usbEventArgs = new UsbDeviceEventArgs();

            if (semaphore.Wait(10))
            {
                try
                {
                    QueryUsbDevices();

                    usbDevices.ForEach(device =>
                    {
                        //We obtained the proper device path (from QueryUsbDevices function call), and it
                        //is now possible to open read and write handles to the device.
                        if (device.IsFound && ((device.IsAttached == false) || (device.IsAttachedButBroken == true)))
                        {
                            uint errorStatusWrite;
                            uint errorStatusRead;

                            device.WriteHandleToUSBDevice = CreateFile(device.DevicePath, GENERIC_WRITE, FILE_SHARE_READ | FILE_SHARE_WRITE, IntPtr.Zero, OPEN_EXISTING, FILE_FLAG_OVERLAPPED, IntPtr.Zero);
                            errorStatusWrite = (uint)Marshal.GetLastWin32Error();
                            device.ReadHandleToUSBDevice = CreateFile(device.DevicePath, GENERIC_READ, FILE_SHARE_READ | FILE_SHARE_WRITE, IntPtr.Zero, OPEN_EXISTING, FILE_FLAG_OVERLAPPED, IntPtr.Zero);
                            errorStatusRead = (uint)Marshal.GetLastWin32Error();

                            if ((errorStatusWrite == ERROR_SUCCESS) && (errorStatusRead == ERROR_SUCCESS))
                            {
                                device.IsAttached = true;
                                device.IsAttachedButBroken = false;
                                usbEventArgs.ConnectedDevices.Add(device);
                            }
                            else //for some reason the device was physically plugged in, but one or both of the read/write handles didn't open successfully...
                            {
                                device.IsAttached = false;
                                device.IsAttachedButBroken = true;   //Flag so that next time a WM_DEVICECHANGE message occurs, can retry to re-open read/write pipes
                                if (errorStatusWrite == ERROR_SUCCESS)
                                    device.WriteHandleToUSBDevice.Close();
                                if (errorStatusRead == ERROR_SUCCESS)
                                    device.ReadHandleToUSBDevice.Close();
                            }
                            usbEventArgs.EventCount++;
                        }
                        //Device must not be connected (or not programmed with correct firmware)
                        else if (!device.IsFound && (device.IsAttached || device.IsAttachedButBroken))
                        {
                            DetachDevice(device);
                            usbEventArgs.EventCount++;
                            usbEventArgs.DisconnectedDevices.Add(device);
                        }
                        //Else device was already connected and configured
                    });
                    
                    if (usbEventArgs.EventCount > 0)
                    {
                        OnUsbDriverEvent(usbEventArgs);
                    }
                }
                catch (Exception e)
                {
                    Trace.WriteLine(e.Message);
                }
                finally
                {
                    semaphore.Release();
                }
            }
        }

        /// <summary>
        /// Query connected HID devices and update our usb devices list
        /// </summary>
        private void QueryUsbDevices()
        {
            IntPtr pDeviceInfoTable = IntPtr.Zero;

            try
            {
                SP_DEVICE_INTERFACE_DATA interfaceDataStructure = new SP_DEVICE_INTERFACE_DATA();
                SP_DEVICE_INTERFACE_DETAIL_DATA detailedInterfaceDataStructure = new SP_DEVICE_INTERFACE_DETAIL_DATA();
                SP_DEVINFO_DATA devInfoData = new SP_DEVINFO_DATA();

                uint interfaceIndex = 0;
                uint dwRegType = 0;
                uint dwRegSize = 0;
                uint dwRegSize2 = 0;
                uint structureSize = 0;
                IntPtr pPropertyValueBuffer = IntPtr.Zero;
                String deviceIDFromRegistry;
                bool matchFound = false;
                uint errorStatus;

                Trace.WriteLine($"Query usb devices {deviceIDToFind}");

                // clear IsFound flag on existing devices so we can verify they are plugged in
                usbDevices.Select(dev => { return dev.IsFound = false; }).ToList();

                //First populate a list of plugged in devices (by specifying "DIGCF_PRESENT"), which are of the specified class GUID. 
                pDeviceInfoTable = SetupDiGetClassDevs(ref InterfaceClassGuid, IntPtr.Zero, IntPtr.Zero, DIGCF_PRESENT | DIGCF_DEVICEINTERFACE);

                if (pDeviceInfoTable != IntPtr.Zero)
                {
                    //Now look through the list we just populated.  We are trying to see if any of them match our device. 
                    while (true)
                    {
                        interfaceDataStructure.cbSize = (uint)Marshal.SizeOf(interfaceDataStructure);
                        if (false == SetupDiEnumDeviceInterfaces(pDeviceInfoTable, IntPtr.Zero, ref InterfaceClassGuid, interfaceIndex, ref interfaceDataStructure))
                        {
                            errorStatus = (uint)Marshal.GetLastWin32Error();
                            if (errorStatus == ERROR_NO_MORE_ITEMS) //Did we reach the end of the list of matching devices in the DeviceInfoTable?
                            {
                                break;
                            }

                            Logger.Write(Level.Error, $"USB EnumDevice unknown WIN32 error: 0x{errorStatus:X}");
                            break;
                        }

                        //Now retrieve the hardware ID from the registry.  The hardware ID contains the VID and PID, which we will then 
                        //check to see if it is the correct device or not.

                        //Initialize an appropriate SP_DEVINFO_DATA structure.  We need this structure for SetupDiGetDeviceRegistryProperty().
                        devInfoData.cbSize = (uint)Marshal.SizeOf(devInfoData);
                        SetupDiEnumDeviceInfo(pDeviceInfoTable, interfaceIndex, ref devInfoData);

                        //First query for the size of the hardware ID, so we can know how big a buffer to allocate for the data.
                        SetupDiGetDeviceRegistryProperty(pDeviceInfoTable, ref devInfoData, SPDRP_HARDWAREID, ref dwRegType, IntPtr.Zero, 0, ref dwRegSize);

                        //Allocate a buffer for the hardware ID.
                        //Should normally work, but could throw exception "OutOfMemoryException" if not enough resources available.
                        try
                        {
                            pPropertyValueBuffer = Marshal.AllocHGlobal((int)dwRegSize);

                            //Retrieve the hardware IDs for the current device we are looking at.  PropertyValueBuffer gets filled with a 
                            //REG_MULTI_SZ (array of null terminated strings).  To find a device, we only care about the very first string in the
                            //buffer, which will be the "device ID".  The device ID is a string which contains the VID and PID, in the example 
                            //format "Vid_04d8&Pid_00ff".
                            SetupDiGetDeviceRegistryProperty(pDeviceInfoTable, ref devInfoData, SPDRP_HARDWAREID, ref dwRegType, pPropertyValueBuffer, dwRegSize, ref dwRegSize2);

                            //Now check if the first string in the hardware ID matches the device ID of the USB device we are trying to find.
                            deviceIDFromRegistry = Marshal.PtrToStringUni(pPropertyValueBuffer); //Make a new string, fill it with the contents from the PropertyValueBuffer
                        }
                        finally
                        {
                            Marshal.FreeHGlobal(pPropertyValueBuffer);
                        }

                        //Convert both strings to lower case.  This makes the code more robust/portable accross OS Versions
                        deviceIDFromRegistry = deviceIDFromRegistry.ToLowerInvariant();

                        //Now check if the hardware ID we are looking at contains the correct VID/PID
                        matchFound = deviceIDFromRegistry.Contains(deviceIDToFind);
                        if (matchFound == true)
                        {
                            //Device must have been found.  In order to open I/O file handle(s), we will need the actual device path first.
                            //We can get the path by calling SetupDiGetDeviceInterfaceDetail(), however, we have to call this function twice:  The first
                            //time to get the size of the required structure/buffer to hold the detailed interface data, then a second time to actually 
                            //get the structure (after we have allocated enough memory for the structure.)
                            detailedInterfaceDataStructure.cbSize = (uint)Marshal.SizeOf(detailedInterfaceDataStructure);
                            //First call populates "StructureSize" with the correct value
                            SetupDiGetDeviceInterfaceDetail(pDeviceInfoTable, ref interfaceDataStructure, IntPtr.Zero, 0, ref structureSize, IntPtr.Zero);
                            //We expect the result of the last function to return an error of ERROR_INSUFFICIENT_BUFFER
                            errorStatus = (uint)Marshal.GetLastWin32Error();
                            if(errorStatus != ERROR_INSUFFICIENT_BUFFER)
                            {
                                Logger.Write(Level.Error, $"USB DeviceDetail structSize unknown WIN32 error: 0x{errorStatus:X}");
                                continue;
                            }

                            //Need to call SetupDiGetDeviceInterfaceDetail() again, this time specifying a pointer to a SP_DEVICE_INTERFACE_DETAIL_DATA buffer with the correct size of RAM allocated.
                            //First need to allocate the unmanaged buffer and get a pointer to it.
                            IntPtr pUnmanagedDetailedInterfaceDataStructure = IntPtr.Zero;  //Declare a pointer.
                            try
                            {
                                pUnmanagedDetailedInterfaceDataStructure = Marshal.AllocHGlobal((int)structureSize);    //Reserve some unmanaged memory for the structure.
                                detailedInterfaceDataStructure.cbSize = Environment.Is64BitProcess ? 8 : 4 + (uint)Marshal.SystemDefaultCharSize; // DWORD + 2 byte null unicode, see: https://stackoverflow.com/questions/9245595/win32api-usb-setupdigetdeviceinterfacedetail-fail
                                Marshal.StructureToPtr(detailedInterfaceDataStructure, pUnmanagedDetailedInterfaceDataStructure, false); //Copy managed structure contents into the unmanaged memory buffer.

                                //Now call SetupDiGetDeviceInterfaceDetail() a second time to receive the device path in the structure at pUnmanagedDetailedInterfaceDataStructure.
                                if (SetupDiGetDeviceInterfaceDetail(pDeviceInfoTable, ref interfaceDataStructure, pUnmanagedDetailedInterfaceDataStructure, structureSize, IntPtr.Zero, IntPtr.Zero))
                                {
                                    //Need to extract the path information from the unmanaged "structure".  The path starts at (pUnmanagedDetailedInterfaceDataStructure + sizeof(DWORD)).
                                    IntPtr pToDevicePath = new IntPtr((long)pUnmanagedDetailedInterfaceDataStructure + 4);  //Add 4 to the pointer (to get the pointer to point to the path, instead of the DWORD cbSize parameter)
                                    detailedInterfaceDataStructure.DevicePath = Marshal.PtrToStringUni(pToDevicePath); //Now copy the path information into the DevicePath String.

                                    // if device already exists in our list, set IsFound to true. Otherwise, add to list
                                    var foundDevice = usbDevices.Where(dev => dev.DevicePath == detailedInterfaceDataStructure.DevicePath).FirstOrDefault();
                                    if (foundDevice != null)
                                    {
                                        foundDevice.IsFound = true;
                                    }
                                    else
                                    {
                                        usbDevices.Add(new UsbDevice { DevicePath = detailedInterfaceDataStructure.DevicePath, IsFound = true });
                                    }

                                    //We now have the proper device path, and we can finally use the path to open I/O handle(s) to the device.
                                }
                                else //Some unknown failure occurred
                                {
                                    errorStatus = (uint)Marshal.GetLastWin32Error();
                                    Logger.Write(Level.Error, $"USB DeviceDetail unknown WIN32 error: 0x{errorStatus:X}");
                                }
                            }
                            catch(Exception ex)
                            {
                                Logger.Write(Level.Error, $"USB DeviceDetail exception: {ex.Message}");
                            }
                            finally
                            {
                                if(pUnmanagedDetailedInterfaceDataStructure != IntPtr.Zero)
                                    Marshal.FreeHGlobal(pUnmanagedDetailedInterfaceDataStructure);
                            }
                        }

                        interfaceIndex++;
                    }

                    var deviceFoundCount = usbDevices.Where(dev => dev.IsFound == true).Count();
                    Trace.WriteLine($"USB search found {deviceFoundCount} device(s) {deviceIDToFind}");
                }
            }
            catch (Exception ex)
            {
                //Something went wrong if we get here. Maybe a Marshal.AllocHGlobal() failed due to insufficient resources or something.
                Trace.WriteLine($"USB CheckIfPresent Error: {ex}");
            }
            finally
            {
                if(pDeviceInfoTable != IntPtr.Zero)
                    SetupDiDestroyDeviceInfoList(pDeviceInfoTable);	//Clean up the old structure we no longer need.
            }
        }
    }
}
