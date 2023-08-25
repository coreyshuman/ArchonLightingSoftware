using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ArchonLightingSystem.Interfaces;
using ArchonLightingSystem.Common;

namespace ArchonLightingSystem.UsbApplicationV2
{
    internal class UsbReadWrite : UsbSystemDefinitions
    {

        /// <summary>
        /// Write buffer to USB device. 
        /// </summary>
        /// <param name="device">device class with device info</param>
        /// <param name="buffer">buffer bytes</param>
        /// <param name="bufflen">buffer length</param>
        /// <returns>Number of bytes written</returns>
        internal Task<uint> Write(IUsbDevice device, Byte[] buffer, uint bufflen, CancellationTokenSource cancelToken)
        {
            NativeOverlapped overlapped = new NativeOverlapped();

            SafeFileHandle eventHandle = null;
            uint bytesWritten = 0;
            Byte[] usbReport = new Byte[USB_PACKET_SIZE];
            int i;

            TaskCompletionSource<uint> taskCompletionSource = null;

            if (cancelToken == null)
            {
                throw new ArgumentNullException(nameof(cancelToken));
            }

            if (bufflen > USB_PACKET_SIZE - 1)
            {
                Logger.Write(Level.Trace, "UsbWrite buffer length too short");
                return Task.FromResult(0u);
            }

            if (buffer == null || buffer.Length < bufflen)
            {
                Logger.Write(Level.Trace, "UsbWrite buffer shorter than bufflen");
                return Task.FromResult(0u);
            }

            if (device == null)
            {
                Logger.Write(Level.Trace, "UsbWrite device null");
                return Task.FromResult(0u);
            }

            if (device.IsAttached == false)
            {
                Logger.Write(Level.Trace, "UsbWrite device detached");
                return Task.FromResult(0u);
            }

            if (device.WriteHandleToUSBDevice == null)
            {
                Logger.Write(Level.Trace, "UsbWrite handle null");
                return Task.FromResult(0u);
            }

            if (cancelToken.IsCancellationRequested)
            {
                Logger.Write(Level.Trace, "UsbWrite cancelled");
                return Task.FromResult(0u);
            }

            try
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

                // use overlapped event to wait for response
                eventHandle = CreateEvent(IntPtr.Zero, false, false, IntPtr.Zero);
                if(eventHandle.IsInvalid)
                {
                    Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
                }

                overlapped.EventHandle = eventHandle.DangerousGetHandle();
                overlapped.OffsetLow = 0;
                overlapped.OffsetHigh = 0;

                if (WriteFile(device.WriteHandleToUSBDevice, usbReport, USB_PACKET_SIZE, ref bytesWritten, ref overlapped))
                {
                    if (bytesWritten == 0)
                        Logger.Write(Level.Trace, $"UsbWrite {bytesWritten} bytes written");

                    return Task.FromResult(bytesWritten);
                }

                var lastError = Marshal.GetLastWin32Error();

                if (lastError == ERROR_DEVICE_NOT_CONNECTED)
                {
                    Logger.Write(Level.Trace, "UsbWrite device disconnected");
                    cancelToken.Cancel();
                    return Task.FromResult(0u);
                }

                if (lastError == ERROR_IO_PENDING)
                {
                    if (cancelToken.IsCancellationRequested)
                    {
                        Logger.Write(Level.Trace, "UsbWrite overlapped cancelled");
                        CancelIoEx(device.WriteHandleToUSBDevice, ref overlapped);
                        return Task.FromResult(0u);
                    }

                    taskCompletionSource = new TaskCompletionSource<uint>();

                    Task.Factory.StartNew(() =>
                    {
                        try
                        {
                            if (cancelToken.IsCancellationRequested)
                            {
                                Logger.Write(Level.Trace, "UsbWrite overlapped cancelled 2");
                                CancelIoEx(device.WriteHandleToUSBDevice, ref overlapped);
                                taskCompletionSource.SetResult(0u);
                                return;
                            }

                            uint result = WaitForSingleObject(eventHandle, 150); // ms timeout period

                            if (cancelToken.IsCancellationRequested)
                            {
                                Logger.Write(Level.Trace, "UsbWrite overlapped cancelled 3");
                                CancelIoEx(device.WriteHandleToUSBDevice, ref overlapped);
                                taskCompletionSource.SetResult(0u);
                                return;
                            }

                            uint errorCode;
                            switch (result)
                            {
                                case WAIT_OBJECT_0:
                                    // Success;
                                    if (GetOverlappedResult(device.WriteHandleToUSBDevice, ref overlapped, ref bytesWritten, false))
                                    {
                                        if (bytesWritten == 0)
                                            Logger.Write(Level.Trace, $"UsbWrite overlapped {bytesWritten} bytes written");

                                        taskCompletionSource.SetResult(bytesWritten);
                                        return;
                                    }
                                    errorCode = (uint)Marshal.GetLastWin32Error();
                                    Logger.Write(Level.Trace, $"UsbWrite overlapped no response. LastWin32Error=[{errorCode.ToString("X8")}]");
                                    break;

                                case WAIT_TIMEOUT:
                                    // Timeout error;
                                    Logger.Write(Level.Trace, $"UsbWrite Timeout {device.ShortName}");
                                    break;

                                default:
                                    // Undefined error;
                                    errorCode = (uint)Marshal.GetLastWin32Error();
                                    Logger.Write(Level.Trace, $"UsbWrite undefined error. WaitForSingleObject=[{result.ToString("X8")}] LastWin32Error=[{errorCode.ToString("X8")}]");
                                    break;
                            }

                            CancelIoEx(device.WriteHandleToUSBDevice, ref overlapped);
                            taskCompletionSource.SetResult(0u);
                        }
                        catch (Exception ex)
                        {
                            Logger.Write(Level.Trace, $"Undefined UsbWrite thread Error: {ex.Message}");
                            CancelIoEx(device.WriteHandleToUSBDevice, ref overlapped);
                            taskCompletionSource.SetException(ex);
                        }
                        finally
                        {
                            eventHandle.Close();
                        }
                    }, cancelToken.Token, TaskCreationOptions.None, TaskScheduler.Default);

                    return taskCompletionSource.Task;
                }
                else
                {
                    throw new Exception($"Unknown WIN32 Error: {lastError:X}");
                }
            }
            catch (Exception ex)
            {
                Logger.Write(Level.Error, $"Undefined UsbWrite Error: {ex.Message}");
            }
            finally
            {
                if (taskCompletionSource == null)
                {
                    eventHandle?.Close();
                }
            }

            // failed to write
            Logger.Write(Level.Trace, "UsbWrite failed to write");
            return Task.FromResult(0u);
        }

        /// <summary>
        /// Read from USB device
        /// </summary>
        /// <param name="device">device class with device info</param>
        /// <param name="buffer">buffer array</param>
        /// <param name="bufflen">buffer length</param>
        /// <param name="readTimeout">timeout for read operation</param>
        /// <returns></returns>
        internal Task<uint> Read(IUsbDevice device, Byte[] buffer, uint bufflen, uint readTimeout, CancellationTokenSource cancelToken)
        {
            NativeOverlapped overlapped = new NativeOverlapped();
            SafeFileHandle eventHandle = null;
            IntPtr pINBuffer = IntPtr.Zero;
            uint bytesRead = 0;
            UInt32 result;

            TaskCompletionSource<uint> taskCompletionSource = null;

            if (cancelToken == null)
            {
                throw new ArgumentNullException(nameof(cancelToken));
            }

            if (buffer == null || buffer.Length < bufflen)
            {
                Logger.Write(Level.Trace, "UsbRead buffer shorter than bufflen");
                return Task.FromResult(0u);
            }

            if (device == null)
            {
                Logger.Write(Level.Trace, "UsbRead device null");
                return Task.FromResult(0u);
            }

            if (device.IsAttached == false)
            {
                Logger.Write(Level.Trace, "UsbRead device detached");
                return Task.FromResult(0u);
            }

            if (device.ReadHandleToUSBDevice == null)
            {
                Logger.Write(Level.Trace, "UsbRead handle null");
                return Task.FromResult(0u);
            }

            if (cancelToken.IsCancellationRequested)
            {
                Logger.Write(Level.Trace, "UsbRead cancelled");
                return Task.FromResult(0u);
            }

            try
            {
                // Set the first byte in the buffer to the Report ID.
                buffer[0] = 0;

                // use overlapped event to wait for response
                eventHandle = CreateEvent(IntPtr.Zero, false, false, IntPtr.Zero);
                if (eventHandle.IsInvalid)
                {
                    Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
                }

                overlapped.EventHandle = eventHandle.DangerousGetHandle();
                overlapped.OffsetLow = 0;
                overlapped.OffsetHigh = 0;

                pINBuffer = Marshal.AllocHGlobal((int)bufflen);    //Allocate some unmanged RAM for the receive data buffer.

                if (ReadFile(device.ReadHandleToUSBDevice, pINBuffer, bufflen, ref bytesRead, ref overlapped))
                {
                    Marshal.Copy(pINBuffer, buffer, 0, (int)bytesRead);    //Copy over the data from unmanged memory into the managed byte[] INBuffer
                    if(bytesRead == 0)
                        Logger.Write(Level.Trace, $"UsbRead {bytesRead} bytes read");
                    return Task.FromResult(bytesRead);
                }

                var lastError = Marshal.GetLastWin32Error();

                if (lastError == ERROR_DEVICE_NOT_CONNECTED)
                {
                    Logger.Write(Level.Trace, $"UsbRead error device disconnected.");
                    cancelToken.Cancel();
                    return Task.FromResult(0u);
                }

                if (lastError == ERROR_IO_PENDING)
                {
                    if (cancelToken.IsCancellationRequested)
                    {
                        CancelIoEx(device.ReadHandleToUSBDevice, ref overlapped);
                        Logger.Write(Level.Trace, "UsbRead overlapped cancelled");
                        return Task.FromResult(0u);
                    }

                    taskCompletionSource = new TaskCompletionSource<uint>();

                    Task.Factory.StartNew(() =>
                    {
                        try
                        {
                            result = WaitForSingleObject(eventHandle, readTimeout);

                            if (cancelToken.IsCancellationRequested)
                            {
                                CancelIoEx(device.ReadHandleToUSBDevice, ref overlapped);
                                Logger.Write(Level.Trace, "UsbRead overlapped cancelled 2");
                                taskCompletionSource.SetResult(0u);
                                return;
                            }

                            uint errorCode;
                            switch (result)
                            {
                                case WAIT_OBJECT_0:
                                    // Success;
                                    if (GetOverlappedResult(device.ReadHandleToUSBDevice, ref overlapped, ref bytesRead, false))
                                    {
                                        Marshal.Copy(pINBuffer, buffer, 0, (int)bytesRead);    //Copy over the data from unmanged memory into the managed byte[] INBuffer
                                        if (bytesRead == 0)
                                            Logger.Write(Level.Trace, $"UsbRead overlapped {bytesRead} bytes read");
                                        taskCompletionSource.SetResult(bytesRead);
                                        return;
                                    }
                                    errorCode = (uint)Marshal.GetLastWin32Error();
                                    Logger.Write(Level.Trace, $"UsbW. LastWin32Error=[{errorCode.ToString("X8")}]");
                                    break;

                                case WAIT_TIMEOUT:
                                    // Timeout error;
                                    Logger.Write(Level.Trace, $"UsbRead timeout");
                                    break;

                                default:
                                    // Undefined error;
                                    errorCode = (uint)Marshal.GetLastWin32Error();
                                    Logger.Write(Level.Trace, $"UsbRead undefined error. WaitForSingleObject=[{result.ToString("X8")}] LastWin32Error=[{errorCode.ToString("X8")}]");
                                    break;
                            }

                            CancelIoEx(device.ReadHandleToUSBDevice, ref overlapped);
                            taskCompletionSource.SetResult(0u);
                        }
                        catch (Exception ex)
                        {
                            Logger.Write(Level.Trace, $"Undefined UsbRead thread Error: {ex.Message}");
                            CancelIoEx(device.ReadHandleToUSBDevice, ref overlapped);
                            taskCompletionSource.SetException(ex);
                        }
                        finally
                        {
                            eventHandle.Dispose();
                            Marshal.FreeHGlobal(pINBuffer);
                        }
                    }, cancelToken.Token, TaskCreationOptions.None, TaskScheduler.Default);

                    return taskCompletionSource.Task;
                }
                else
                {
                    throw new Exception($"Unknown WIN32 Error: {lastError:X}");
                }
            }
            catch (Exception e)
            {
                Logger.Write(Level.Error, $"Undefined UsbRead Error: {e.Message}");
            }
            finally
            {
                if (taskCompletionSource == null)
                {
                    eventHandle?.Close();
                    if (pINBuffer != IntPtr.Zero)
                    {
                        Marshal.FreeHGlobal(pINBuffer);
                    }
                }
            }

            // failed to read
            Logger.Write(Level.Trace, "UsbRead failed to read");
            return Task.FromResult(0u);
        }
    }
}
