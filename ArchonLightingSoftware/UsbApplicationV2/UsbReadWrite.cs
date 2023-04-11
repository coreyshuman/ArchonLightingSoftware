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
            OVERLAPPED HIDOverlapped = new OVERLAPPED();
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
                Trace.WriteLine("Usb write buffer length too long");
                return Task.FromResult(0u);
            }

            if (buffer == null || buffer.Length < bufflen)
            {
                Trace.WriteLine("Usb write buffer shorter than bufflen");
                return Task.FromResult(0u);
            }

            if (device.IsAttached == false)
            {
                return Task.FromResult(0u);
            }

            if (device.WriteHandleToUSBDevice == null)
            {
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

                // use hid overlapped event to wait for response
                eventHandle = CreateEvent(IntPtr.Zero, false, true, IntPtr.Zero);
                if(eventHandle.IsInvalid)
                {
                    Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
                }

                HIDOverlapped.hEvent = eventHandle.DangerousGetHandle();
                HIDOverlapped.Offset = 0;
                HIDOverlapped.OffsetHigh = 0;

                var watch = Stopwatch.StartNew();
                if (WriteFile(device.WriteHandleToUSBDevice, usbReport, USB_PACKET_SIZE, ref bytesWritten, ref HIDOverlapped))
                {
                    watch.Stop();
                    Logger.Write(Level.Trace, $"UsbWrite immediate Duration {watch.ElapsedMilliseconds} ms");
                    return Task.FromResult(bytesWritten);
                }
                else if (Marshal.GetLastWin32Error() == ERROR_IO_PENDING)
                {
                    if (cancelToken.IsCancellationRequested)
                    {
                        CancelIo(device.WriteHandleToUSBDevice);
                        return Task.FromResult(0u);
                    }

                    taskCompletionSource = new TaskCompletionSource<uint>();

                    Task.Factory.StartNew(() =>
                    {
                        try
                        {
                            if (cancelToken.IsCancellationRequested)
                            {
                                CancelIo(device.WriteHandleToUSBDevice);
                                taskCompletionSource.SetResult(0u);
                                return;
                            }

                            uint result = WaitForSingleObject(eventHandle, 150); // ms timeout period
                            watch.Stop();
                            Logger.Write(Level.Trace, $"UsbWrite wait Duration {watch.ElapsedMilliseconds} ms");

                            if (cancelToken.IsCancellationRequested)
                            {
                                CancelIo(device.WriteHandleToUSBDevice);
                                taskCompletionSource.SetResult(0u);
                                return;
                            }

                            switch (result)
                            {
                                case WAIT_OBJECT_0:
                                    // Success;
                                    if (GetOverlappedResult(device.WriteHandleToUSBDevice, ref HIDOverlapped, ref bytesWritten, false))
                                    {
                                        //return bytesWritten;
                                        Logger.Write(Level.Trace, $"UsbWrite wait {bytesWritten} bytes written");
                                        taskCompletionSource.SetResult(bytesWritten);
                                        return;
                                    }
                                    break;

                                case WAIT_TIMEOUT:
                                    // Timeout error;
                                    Trace.WriteLine($"UsbRead Timeout {device.ShortName}");
                                    break;

                                default:
                                    // Undefined error;
                                    uint ErrorCode = (uint)Marshal.GetLastWin32Error();
                                    Trace.WriteLine($"Undefined UsbWrite Error. WaitForSingleObject=[{ErrorCode.ToString("X8")}] LastWin32Error=[{result.ToString("X8")}]");
                                    break;
                            }

                            CancelIo(device.WriteHandleToUSBDevice);
                            taskCompletionSource.SetResult(0u);
                        }
                        catch (Exception ex)
                        {
                            Trace.WriteLine($"Undefined UsbWrite thread Error: {ex.Message}");
                            CancelIo(device.ReadHandleToUSBDevice);
                            taskCompletionSource.SetException(ex);
                        }
                        finally
                        {
                            eventHandle.Close();
                        }
                    }, cancelToken.Token, TaskCreationOptions.None, TaskScheduler.Default);

                    return taskCompletionSource.Task;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"Undefined UsbWrite Error: {ex.Message}");
            }
            finally
            {
                if (taskCompletionSource == null)
                {
                    eventHandle?.Close();
                }
            }

            // failed to write
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
            OVERLAPPED HIDOverlapped = new OVERLAPPED();
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
                Trace.WriteLine("Usb write buffer shorter than bufflen");
                return Task.FromResult(0u);
            }

            if (device.IsAttached == false)
            {
                return Task.FromResult(0u);
            }

            if (device.ReadHandleToUSBDevice == null)
            {
                return Task.FromResult(0u);
            }

            try
            {
                // Set the first byte in the buffer to the Report ID.
                buffer[0] = 0;

                // use hid overlapped event to wait for response
                eventHandle = CreateEvent(IntPtr.Zero, false, true, IntPtr.Zero);
                if (eventHandle.IsInvalid)
                {
                    Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
                }

                HIDOverlapped.hEvent = eventHandle.DangerousGetHandle();
                HIDOverlapped.Offset = 0;
                HIDOverlapped.OffsetHigh = 0;

                pINBuffer = Marshal.AllocHGlobal((int)bufflen);    //Allocate some unmanged RAM for the receive data buffer.

                var watch = Stopwatch.StartNew();
                if (ReadFile(device.ReadHandleToUSBDevice, pINBuffer, bufflen, ref bytesRead, ref HIDOverlapped))
                {
                    watch.Stop();
                    Logger.Write(Level.Trace, $"UsbRead immediate Duration {watch.ElapsedMilliseconds} ms");
                    Marshal.Copy(pINBuffer, buffer, 0, (int)bytesRead);    //Copy over the data from unmanged memory into the managed byte[] INBuffer
                    Logger.Write(Level.Trace, $"UsbRead {bytesRead} bytes read");
                    return Task.FromResult(bytesRead);
                }
                else if (Marshal.GetLastWin32Error() == ERROR_IO_PENDING)
                {
                    if (cancelToken.IsCancellationRequested)
                    {
                        CancelIo(device.ReadHandleToUSBDevice);
                        return Task.FromResult(0u);
                    }

                    taskCompletionSource = new TaskCompletionSource<uint>();

                    Task.Factory.StartNew(() =>
                    {
                        try
                        {
                            result = WaitForSingleObject(eventHandle, readTimeout);
                            watch.Stop();
                            Logger.Write(Level.Trace, $"UsbRead wait Duration {watch.ElapsedMilliseconds} ms");

                            if (cancelToken.IsCancellationRequested)
                            {
                                CancelIo(device.ReadHandleToUSBDevice);
                                taskCompletionSource.SetResult(0u);
                                return;
                            }

                            switch (result)
                            {
                                case WAIT_OBJECT_0:
                                    // Success;
                                    if (GetOverlappedResult(device.ReadHandleToUSBDevice, ref HIDOverlapped, ref bytesRead, false))
                                    {
                                        Marshal.Copy(pINBuffer, buffer, 0, (int)bytesRead);    //Copy over the data from unmanged memory into the managed byte[] INBuffer
                                        Logger.Write(Level.Trace, $"UsbRead {bytesRead} bytes read");
                                        taskCompletionSource.SetResult(bytesRead);
                                        return;
                                    }
                                    break;

                                case WAIT_TIMEOUT:
                                    // Timeout error;
                                    Logger.Write(Level.Trace, $"UsbRead timeout");
                                    break;

                                default:
                                    // Undefined error;
                                    uint ErrorCode = (uint)Marshal.GetLastWin32Error();
                                    Trace.WriteLine($"Undefined UsbRead Error. WaitForSingleObject=[{ErrorCode.ToString("X8")}] LastWin32Error=[{result.ToString("X8")}]");
                                    break;
                            }

                            CancelIo(device.ReadHandleToUSBDevice);
                            taskCompletionSource.SetResult(0u);
                        }
                        catch (Exception ex)
                        {
                            Trace.WriteLine($"Undefined UsbWrite thread Error: {ex.Message}");
                            CancelIo(device.ReadHandleToUSBDevice);
                            taskCompletionSource.SetException(ex);
                        }
                        finally
                        {
                            eventHandle.Dispose();
                            Marshal.FreeHGlobal(pINBuffer);
                        }
                    }, cancelToken.Token, TaskCreationOptions.None, TaskScheduler.Default);
                }

                return taskCompletionSource.Task;
            }
            catch (Exception e)
            {
                Trace.WriteLine($"Undefined UsbWrite Error: {e.Message}");
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
            return Task.FromResult(0u);
        }
    }
}
