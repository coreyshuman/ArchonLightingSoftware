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
            SafeFileHandle hEventObject = null;
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

                // use overlapped structure
                hEventObject = CreateEvent(IntPtr.Zero, false, true, IntPtr.Zero);
                HIDOverlapped.hEvent = hEventObject.DangerousGetHandle();
                HIDOverlapped.Offset = 0;
                HIDOverlapped.OffsetHigh = 0;

                var watch = Stopwatch.StartNew();
                WriteFile(device.WriteHandleToUSBDevice, usbReport, USB_PACKET_SIZE, ref bytesWritten, ref HIDOverlapped);

                if (Marshal.GetLastWin32Error() == ERROR_IO_PENDING)
                {
                    if(cancelToken.IsCancellationRequested)
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

                            uint result = WaitForSingleObject(hEventObject, 150); // ms timeout period
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
                        catch(Exception ex)
                        {
                            Trace.WriteLine($"Undefined UsbWrite thread Error: {ex.Message}");
                            taskCompletionSource.SetException(ex);
                        }
                        finally
                        {
                            hEventObject?.Dispose();
                        }
                    }, cancelToken.Token, TaskCreationOptions.None, TaskScheduler.Default);

                    return taskCompletionSource.Task;
                }

                watch.Stop();
                Logger.Write(Level.Trace, $"UsbWrite immediate Duration {watch.ElapsedMilliseconds} ms");
                hEventObject?.Dispose();
                return Task.FromResult(bytesWritten);
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"Undefined UsbWrite Error: {ex.Message}");
                hEventObject?.Dispose();
            }

            return Task.FromResult(bytesWritten);
        }

        /// <summary>
        /// Read from USB device
        /// If no cancelation token is provided, the function will wait for a device semaphore,
        /// in which case you must not consume the semaphore in the same thread.
        /// </summary>
        /// <param name="device">device class with device info</param>
        /// <param name="buffer">buffer array</param>
        /// <param name="bufflen">buffer length</param>
        /// <param name="readTimeout">timeout for read operation</param>
        /// <returns></returns>
        internal uint Read(IUsbDevice device, ref Byte[] buffer, uint bufflen, uint readTimeout = 200, CancellationTokenSource cancelToken = null)
        {
            OVERLAPPED HIDOverlapped = new OVERLAPPED();
            SafeFileHandle hEventObject = null;
            IntPtr pINBuffer = IntPtr.Zero;
            uint bytesRead = 0;
            UInt32 result;
            bool needToReleaseDevice = false;

            if (buffer == null || buffer.Length < bufflen)
            {
                Trace.WriteLine("Usb write buffer shorter than bufflen");
                return 0;
            }

            if (device.IsAttached == false)
            {
                return 0;
            }

            if (device.ReadHandleToUSBDevice == null)
            {
                return 0;
            }

            try
            {
                if(cancelToken == null)
                {
                    cancelToken = new CancellationTokenSource();
                    device.Wait(cancelToken);
                    needToReleaseDevice = true;
                }

                // Set the first byte in the buffer to the Report ID.
                buffer[0] = 0;

                // use overlapped structure
                hEventObject = CreateEvent(IntPtr.Zero, false, true, IntPtr.Zero);
                HIDOverlapped.hEvent = hEventObject.DangerousGetHandle();
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
                    return bytesRead;
                }
                else if (Marshal.GetLastWin32Error() == ERROR_IO_PENDING)
                {
                    if (cancelToken.IsCancellationRequested)
                    {
                        CancelIo(device.WriteHandleToUSBDevice);
                        return 0;
                    }

                    result = WaitForSingleObject(hEventObject, readTimeout);
                    watch.Stop();
                    Logger.Write(Level.Trace, $"UsbRead wait Duration {watch.ElapsedMilliseconds} ms");
                    switch (result)
                    {
                        case WAIT_OBJECT_0:
                            // Success;
                            if (GetOverlappedResult(device.ReadHandleToUSBDevice, ref HIDOverlapped, ref bytesRead, false))
                            {
                                Marshal.Copy(pINBuffer, buffer, 0, (int)bytesRead);    //Copy over the data from unmanged memory into the managed byte[] INBuffer
                                Logger.Write(Level.Trace, $"UsbRead {bytesRead} bytes read");
                                return bytesRead;
                            }
                            CancelIo(device.ReadHandleToUSBDevice);
                            break;

                        case WAIT_TIMEOUT:
                            // Timeout error;
                            CancelIo(device.ReadHandleToUSBDevice);
                            Logger.Write(Level.Trace, $"UsbRead timeout");
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
            catch (Exception e)
            {
                Trace.WriteLine($"Undefined UsbWrite Error: {e.Message}");
            }
            finally
            {
                hEventObject?.Dispose();
                if (pINBuffer != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(pINBuffer);
                }
                if(needToReleaseDevice)
                {
                    device.Release(cancelToken);
                }             
            }

            return 0;
        }

    }
}
