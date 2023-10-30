using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ArchonLightingSystem.WindowsSystem.Native;

namespace ArchonLightingSystem.WindowsSystem
{
    public class GlobalKeyboardHookEventArgs : HandledEventArgs
    {
        public GlobalKeyboardHook.KeyboardState KeyboardState { get; private set; }
        public GlobalKeyboardHook.LowLevelKeyboardInputEvent KeyboardData { get; private set; }

        public GlobalKeyboardHookEventArgs(
            GlobalKeyboardHook.LowLevelKeyboardInputEvent keyboardData,
            GlobalKeyboardHook.KeyboardState keyboardState)
        {
            KeyboardData = keyboardData;
            KeyboardState = keyboardState;
        }
    }

    public class GlobalKeyboardHook: IDisposable
    {
        public event EventHandler<GlobalKeyboardHookEventArgs> KeyboardPressed;
        public static Keys[] RegisteredKeys;

        public enum KeyboardState
        {
            KeyDown = 0x0100,
            KeyUp = 0x0101,
            SysKeyDown = 0x0104,
            SysKeyUp = 0x0105
        }

        public const int KfAltDown = 0x2000;
        public const int LlkhfAltDown = (KfAltDown >> 8);

        private IntPtr _windowsHookHandle;
        private IntPtr _user32LibraryHandle;
        private User32.HookProc _hookProc;

        private KeyCombination pressedKeys = new KeyCombination();
        private Dictionary<int, KeyValuePair<KeyCombination, Action>?> hookEvents= new Dictionary<int, KeyValuePair<KeyCombination, Action>?>();

        /// <summary>
        /// Initializer
        /// </summary>
        /// <param name="registeredKeys">Keys that should trigger logging. Pass null for full logging.</param>
        public GlobalKeyboardHook(Keys[] registeredKeys = null)
        {
            RegisteredKeys = registeredKeys;
            _windowsHookHandle = IntPtr.Zero;
            _user32LibraryHandle = IntPtr.Zero;
            _hookProc = LowLevelKeyboardProc; // we must keep alive _hookProc, because GC is not aware about SetWindowsHookEx behaviour.

            _user32LibraryHandle = Kernel32.LoadLibrary("User32");
            if (_user32LibraryHandle == IntPtr.Zero)
            {
                int errorCode = Marshal.GetLastWin32Error();
                throw new Win32Exception(errorCode, $"Failed to load library 'User32.dll'. Error {errorCode}: {new Win32Exception(Marshal.GetLastWin32Error()).Message}.");
            }



            _windowsHookHandle = User32.SetWindowsHookEx(User32.WH_KEYBOARD_LL, _hookProc, _user32LibraryHandle, 0);
            if (_windowsHookHandle == IntPtr.Zero)
            {
                int errorCode = Marshal.GetLastWin32Error();
                throw new Win32Exception(errorCode, $"Failed to adjust keyboard hooks for '{Process.GetCurrentProcess().ProcessName}'. Error {errorCode}: {new Win32Exception(Marshal.GetLastWin32Error()).Message}.");
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // because we can unhook only in the same thread, not in garbage collector thread
                if (_windowsHookHandle != IntPtr.Zero)
                {
                    if (!User32.UnhookWindowsHookEx(_windowsHookHandle))
                    {
                        int errorCode = Marshal.GetLastWin32Error();
                        throw new Win32Exception(errorCode, $"Failed to remove keyboard hooks for '{Process.GetCurrentProcess().ProcessName}'. Error {errorCode}: {new Win32Exception(Marshal.GetLastWin32Error()).Message}.");
                    }
                    _windowsHookHandle = IntPtr.Zero;

                    _hookProc -= LowLevelKeyboardProc;
                }
            }

            if (_user32LibraryHandle != IntPtr.Zero)
            {
                if (!Kernel32.FreeLibrary(_user32LibraryHandle))
                {
                    int errorCode = Marshal.GetLastWin32Error();
                    throw new Win32Exception(errorCode, $"Failed to unload library 'User32.dll'. Error {errorCode}: {new Win32Exception(Marshal.GetLastWin32Error()).Message}.");
                }
                _user32LibraryHandle = IntPtr.Zero;
            }
        }

        ~GlobalKeyboardHook()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public bool RegisterKeyboardHook(List<Keys> keys, Action action)
        {
            if (keys == null || action == null) return false;
            if(keys.Count == 0) return false;

            var kc = new KeyCombination(keys);
            int id = kc.GetHashCode();

            if (hookEvents.ContainsKey(id)) return false;

            hookEvents[id] = new KeyValuePair<KeyCombination, Action>(kc, action);
            return true;
        }

        public IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            bool fEatKeyStroke = false;

            var wparamTyped = wParam.ToInt32();
            if (Enum.IsDefined(typeof(KeyboardState), wparamTyped))
            {
                object o = Marshal.PtrToStructure(lParam, typeof(LowLevelKeyboardInputEvent));
                LowLevelKeyboardInputEvent p = (LowLevelKeyboardInputEvent)o;

                // Either the incoming key has to be part of RegisteredKeys (see constructor on top) or RegisterdKeys
                // has to be null for the event to get fired.
                var key = (Keys)p.VirtualCode;
                if (RegisteredKeys == null || RegisteredKeys.Contains(key))
                {
                    EventHandler<GlobalKeyboardHookEventArgs> handler = KeyboardPressed;
                    if(handler!= null)
                    {
                        var eventArguments = new GlobalKeyboardHookEventArgs(p, (KeyboardState)wparamTyped);
                        handler.Invoke(this, eventArguments);
                        fEatKeyStroke = eventArguments.Handled;
                    }
                }

                if(wParam == (IntPtr)KeyboardState.KeyDown)
                {
                    pressedKeys.Add(key);
                    if(pressedKeys.Count > 0)
                    {
                        var matchedHook = hookEvents.Values.FirstOrDefault(hook => hook?.Key.Equals(pressedKeys) == true);
                        if(matchedHook != null)
                        {
                            matchedHook?.Value();
                        }
                    }
                }
                else if (wParam == (IntPtr)KeyboardState.KeyUp)
                {
                    pressedKeys.Clear();
                }
            }

            return fEatKeyStroke ? (IntPtr)1 : User32.CallNextHookEx(IntPtr.Zero, nCode, wParam, lParam);
        }



        [StructLayout(LayoutKind.Sequential)]
        public struct LowLevelKeyboardInputEvent
        {
            /// <summary>
            /// A virtual-key code. The code must be a value in the range 1 to 254.
            /// </summary>
            public int VirtualCode;

            /// <summary>
            /// The VirtualCode converted to typeof(Keys) for higher usability.
            /// </summary>
            public Keys Key { get { return (Keys)VirtualCode; } }

            /// <summary>
            /// A hardware scan code for the key. 
            /// </summary>
            public int HardwareScanCode;

            /// <summary>
            /// The extended-key flag, event-injected Flags, context code, and transition-state flag. This member is specified as follows. An application can use the following values to test the keystroke Flags. Testing LLKHF_INJECTED (bit 4) will tell you whether the event was injected. If it was, then testing LLKHF_LOWER_IL_INJECTED (bit 1) will tell you whether or not the event was injected from a process running at lower integrity level.
            /// </summary>
            public int Flags;

            /// <summary>
            /// The time stamp stamp for this message, equivalent to what GetMessageTime would return for this message.
            /// </summary>
            public int TimeStamp;

            /// <summary>
            /// Additional information associated with the message. 
            /// </summary>
            public IntPtr AdditionalInformation;
        }

        private class KeyCombination : IEquatable<KeyCombination>
        {
            private readonly bool _canModify;
            public KeyCombination(List<Keys> keys)
            {
                _keys = keys ?? new List<Keys>();
            }

            public KeyCombination()
            {
                _keys = new List<Keys>();
                _canModify = true;
            }

            public void Add(Keys key)
            {
                if (_canModify)
                {
                    _keys.Add(key);
                }
            }

            public void Remove(Keys key)
            {
                if (_canModify)
                {
                    _keys.Remove(key);
                }
            }

            public void Clear()
            {
                if (_canModify)
                {
                    _keys.Clear();
                }
            }

            public int Count { get { return _keys.Count; } }

            private readonly List<Keys> _keys;

            public bool Equals(KeyCombination other)
            {
                return other._keys != null && _keys != null && KeysEqual(other._keys);
            }

            private bool KeysEqual(List<Keys> keys)
            {
                if (keys == null || _keys == null || keys.Count != _keys.Count) return false;
                for (int i = 0; i < _keys.Count; i++)
                {
                    if (_keys[i] != keys[i])
                        return false;
                }
                return true;
            }

            public override bool Equals(object obj)
            {
                if (obj is KeyCombination)
                    return Equals((KeyCombination)obj);
                return false;
            }

            public override int GetHashCode()
            {
                if (_keys == null) return 0;

                //http://stackoverflow.com/a/263416
                //http://stackoverflow.com/a/8094931
                //assume keys not going to modify after we use GetHashCode
                unchecked
                {
                    int hash = 19;
                    for (int i = 0; i < _keys.Count; i++)
                    {
                        hash = hash * 31 + _keys[i].GetHashCode();
                    }
                    return hash;
                }
            }

            public override string ToString()
            {
                if (_keys == null)
                    return string.Empty;

                var sb = new StringBuilder((_keys.Count - 1) * 4 + 10);
                for (int i = 0; i < _keys.Count; i++)
                {
                    if (i < _keys.Count - 1)
                        sb.Append(_keys[i] + " , ");
                    else
                        sb.Append(_keys[i]);
                }
                return sb.ToString();
            }
        }
    }


}
