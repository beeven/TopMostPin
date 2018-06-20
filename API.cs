using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TopMostPin
{
    public class API
    {
        /******** For GetWindowTitle() *********************/
        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);
        /*********** END ***********************************/

        /********* Setting Windows on Top ******************/

        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        static extern IntPtr SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int Y, int cx, int cy, int wFlags);

        [DllImport("user32.dll")]
        static extern bool SetWindowText(IntPtr hWnd, string text);

        const UInt32 SW_HIDE = 0;
        const UInt32 SW_SHOWNORMAL = 1;
        const UInt32 SW_NORMAL = 1;
        const UInt32 SW_SHOWMINIMIZED = 2;
        const UInt32 SW_SHOWMAXIMIZED = 3;
        const UInt32 SW_MAXIMIZE = 3;
        const UInt32 SW_SHOWNOACTIVATE = 4;
        const UInt32 SW_SHOW = 5;
        const UInt32 SW_MINIMIZE = 6;
        const UInt32 SW_SHOWMINNOACTIVE = 7;
        const UInt32 SW_SHOWNA = 8;
        const UInt32 SW_RESTORE = 9;
        const int SWP_NOMOVE = 0x0002;
        const int SWP_NOSIZE = 0x0001;
        const int SWP_SHOWWINDOW = 0x0040;
        const int SWP_NOACTIVATE = 0x0010;
        const int HWND_TOPMOST = -1;
        const int HWND_NOTOPMOST = -2;
        /*********** END ***********************************/

        delegate bool EnumWindowsProc(int hwnd, int lParam);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool EnumWindows(EnumWindowsProc callback, IntPtr extraData);
        

        [DllImport("user32.dll")]
        static extern bool EnumDesktopWindows(IntPtr hDesktop, EnumDesktopWindowsDelegate lpfn, IntPtr lParam);

        private delegate bool EnumDesktopWindowsDelegate(IntPtr hWnd, int lParam);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool CloseHandle(IntPtr hHandle);

        public static List<KeyValuePair<IntPtr,string>> EnumerateWindows()
        {
            var collection = new List<KeyValuePair<IntPtr, string>>();
            EnumDesktopWindows(IntPtr.Zero, (hWnd, lparam) => {
                StringBuilder sb = new StringBuilder(255);
                GetWindowText(hWnd, sb, sb.Capacity + 1);
                string title = sb.ToString();
                if(IsWindowVisible(hWnd) && !string.IsNullOrEmpty(title))
                {
                    collection.Add(new KeyValuePair<IntPtr, string>(hWnd, title));
                }
                return true;
            }, IntPtr.Zero);
            return collection;
        }

        public static void PinWindow(IntPtr hWnd)
        {

        }

        public static void UnPinWindow(IntPtr hWnd)
        {

        }
    }
}
