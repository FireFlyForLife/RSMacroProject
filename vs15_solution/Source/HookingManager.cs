using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RSMacroProgram
{
    internal class Hooking
    {
        #region Singleton
        static Hooking instance;
        internal static Hooking Manager {
            get { if (instance == null)
                    instance = new Hooking();
                return instance; }
        }
        #endregion

        internal IntPtr ModuleHandle = IntPtr.Zero;
        List<HookInfo> hookIDs;
        Thread hookCallbackThread;

        HookInfo mouseHook, keyboardHook;
        
        private Hooking() {
            hookIDs = new List<HookInfo>();
            hookCallbackThread = new Thread(keepAlive)
;
            mouseHook = new HookInfo();
            mouseHook.callback = clearMouseLLInjectedFlag;
            mouseHook.hID = Win32.HookingIDs.WH_Mouse_LL;
            keyboardHook = new HookInfo();
            keyboardHook.callback = clearKeyboardLLInjectedFlag;
            keyboardHook.hID = Win32.HookingIDs.WH_Keyboard_LL;
        }

        ~Hooking() {
            HookInfo[] h = new HookInfo[hookIDs.Count];
            hookIDs.CopyTo(h);
            foreach(HookInfo hook in h) {
                removeHook(hook.hookID);
            }
        }

        private void keepAlive() {
            while (true) {
                System.Windows.Application.Current.Run();
            }
        }

        internal IntPtr addHook(Win32.HookingIDs hID, WinAPI.LowLevelMouseProc callback) {
            if (ModuleHandle == IntPtr.Zero) {
                using (var curProcess = Process.GetCurrentProcess())
                using (var curModule = curProcess.MainModule) {
                    IntPtr ret = WinAPI.SetWindowsHookEx((int)hID, callback, WinAPI.GetModuleHandle(curModule.ModuleName), 0);
                    HookInfo info = new HookInfo();
                    info.hookID = ret;
                    info.callback = callback;
                    info.hID = hID;
                    hookIDs.Add(info);
                    return ret;
                }
            } else {
                IntPtr ret = WinAPI.SetWindowsHookEx((int)hID, callback, ModuleHandle, 0);
                HookInfo info = new HookInfo();
                info.hookID = ret;
                info.callback = callback;
                info.hID = hID;
                hookIDs.Add(info);
                hookIDs.Add(info);
                return ret;
            }
        }

        internal IntPtr reHook(Win32.HookingIDs hID, WinAPI.LowLevelMouseProc callback) {
            
            removeHook(GetHookInfo(hID, callback).hookID);
            return addHook(hID, callback);
        }

        internal bool hookLLInput() {
            bool success = true;

            if (mouseHook.hookID != IntPtr.Zero)
                removeHook(mouseHook.hookID);
            mouseHook.hookID = addHook(mouseHook.hID, mouseHook.callback);
            success = mouseHook.hookID != IntPtr.Zero && success;
            if (keyboardHook.hookID != IntPtr.Zero)
                removeHook(keyboardHook.hookID);
            keyboardHook.hookID = addHook(keyboardHook.hID, keyboardHook.callback);
            success = keyboardHook.hookID != IntPtr.Zero && success;

            if (!success)
                Console.WriteLine("Not hooked correctly ffs");

            return success;
        }

        internal void stopLLInput() {
            if (mouseHook.hookID != IntPtr.Zero)
                removeHook(mouseHook.hookID);

            if (keyboardHook.hookID != IntPtr.Zero)
                removeHook(keyboardHook.hookID);
        }

        internal HookInfo GetHookInfo(Win32.HookingIDs hID, WinAPI.LowLevelMouseProc callback) {
            foreach (HookInfo info in hookIDs) {
                if (info.hID == hID && info.callback == callback)
                    return info;
            }
            return HookInfo.Empty;
        }

        internal HookInfo GetHookInfo(IntPtr hookID) {
            foreach (HookInfo info in hookIDs) {
                if (info.hookID == hookID)
                    return info;
            }
            return HookInfo.Empty;
        }

        internal bool removeHook(IntPtr id) {
            if (id == null && id == IntPtr.Zero)
                return false;
            if (WinAPI.UnhookWindowsHookEx(id)) {
                hookIDs.Remove(GetHookInfo(id));
                return true;
            }
            return false;
        }

        //public IntPtr ClearInjectedFlag(int nCode, IntPtr wParam, IntPtr lParam) {
        //    Console.WriteLine("Standard Callback Called");
        //    if(nCode >= 0) {

        //    }
        //    return WinAPI.CallNextHookEx(hookIDs[0].hookID, nCode, wParam, lParam);
        //}

        private IntPtr clearMouseLLInjectedFlag(int nCode, IntPtr wParam, IntPtr lParam) {
            //Console.WriteLine("Mouse callback called");

            //if (lParam != IntPtr.Zero) {
            //    Win32.MSLLHOOKSTRUCT str = Marshal.PtrToStructure<Win32.MSLLHOOKSTRUCT>(lParam);
            //    Console.WriteLine("dwExtraInfo: " + str.dwExtraInfo + "\nflags: " + str.flags + "\nmouseData: " + str.mouseData);
            //} else
            //    Console.WriteLine("lParam is NULL in mousehook_LL");

            if (nCode >= 0) {
                IntPtr offset = Marshal.OffsetOf<Win32.MSLLHOOKSTRUCT>("flags");
                Marshal.WriteInt32(lParam, offset.ToInt32(), 0);
            }
            return WinAPI.CallNextHookEx(mouseHook.hookID, nCode, wParam, lParam);
        }

        private IntPtr clearKeyboardLLInjectedFlag(int nCode, IntPtr wParam, IntPtr lParam) {
            //Console.WriteLine("Keyboard callback called");

            //if (lParam != IntPtr.Zero) {
            //    Win32.KBDLLHOOKSTRUCT str = Marshal.PtrToStructure<Win32.KBDLLHOOKSTRUCT>(lParam);
            //    Console.WriteLine("dwExtraInfo: " + str.dwExtraInfo + "\nflags: " + str.flags + "\nscanCode: " + str.scanCode + "\nvkCode: " + str.vkCode);
            //} else
            //    Console.WriteLine("lParam is NULL in mousehook_LL");

            if (nCode >= 0) {
                IntPtr offset = Marshal.OffsetOf<Win32.KBDLLHOOKSTRUCT>("flags");
                Marshal.WriteInt32(lParam, offset.ToInt32(), 0);
            }
            return WinAPI.CallNextHookEx(keyboardHook.hookID, nCode, wParam, lParam);
        }

        internal struct HookInfo
        {
            internal static readonly HookInfo Empty = new HookInfo();

            public IntPtr hookID;
            public Win32.HookingIDs hID;
            public WinAPI.LowLevelMouseProc callback;
        }
    }
}
