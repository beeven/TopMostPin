using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace TopMostPin
{
    public class API
    {
        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);


        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        static extern IntPtr SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int Y, int cx, int cy, int wFlags);

        [DllImport("user32.dll")]
        static extern bool SetWindowText(IntPtr hWnd, string text);

        const int SW_HIDE = 0;
        const int SW_SHOWNORMAL = 1;
        const int SW_NORMAL = 1;
        const int SW_SHOWMINIMIZED = 2;
        const int SW_SHOWMAXIMIZED = 3;
        const int SW_MAXIMIZE = 3;
        const int SW_SHOWNOACTIVATE = 4;
        const int SW_SHOW = 5;
        const int SW_MINIMIZE = 6;
        const int SW_SHOWMINNOACTIVE = 7;
        const int SW_SHOWNA = 8;
        const int SW_RESTORE = 9;
        const int SWP_NOMOVE = 0x0002;
        const int SWP_NOSIZE = 0x0001;
        const int SWP_SHOWWINDOW = 0x0040;
        const int SWP_NOACTIVATE = 0x0010;
        const int HWND_TOPMOST = -1;
        const int HWND_NOTOPMOST = -2;

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
        static extern bool CloseHandle(IntPtr hWnd);

        [DllImport("user32.dll")]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        static extern int GetWindowModuleFileName(IntPtr hWnd, StringBuilder filename, int count);


        [DllImport("user32.dll")]
        static extern bool IsIconic(IntPtr hWnd);

        [DllImport("user32.dll", EntryPoint = "GetClassLong")]
        public static extern uint GetClassLongPtr32(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", EntryPoint = "GetClassLongPtr")]
        public static extern IntPtr GetClassLongPtr64(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        const int GCL_HICONSM = -34;
        const int GCL_HICON = -14;

        const int ICON_SMALL = 0;
        const int ICON_BIG = 1;
        const int ICON_SMALL2 = 2;

        const int WM_GETICON = 0x7F;

        static IntPtr GetClassLongPtr(IntPtr hWnd, int nIndex)
        {
            if (IntPtr.Size > 4)
                return GetClassLongPtr64(hWnd, nIndex);
            else
                return new IntPtr(GetClassLongPtr32(hWnd, nIndex));
        }

        static IntPtr GetIconHandle(IntPtr hwnd)
        {
            IntPtr iconHandle = IntPtr.Zero;
            // iconHandle = SendMessage(hwnd, WM_GETICON, ICON_SMALL2, 0);
            //if (iconHandle == IntPtr.Zero)
            //    iconHandle = SendMessage(hwnd, WM_GETICON, ICON_SMALL, 0);
            //if (iconHandle == IntPtr.Zero)
                iconHandle = SendMessage(hwnd, WM_GETICON, ICON_BIG, 0);
            if (iconHandle == IntPtr.Zero)
                iconHandle = GetClassLongPtr(hwnd, GCL_HICON);
            if (iconHandle == IntPtr.Zero)
                iconHandle = GetClassLongPtr(hwnd, GCL_HICONSM);

            return iconHandle;
        }



        public static List<PinnedWindowListItem> EnumerateWindows()
        {
            var collection = new List<PinnedWindowListItem>();
            EnumDesktopWindows(IntPtr.Zero, (hWnd, lparam) => {
                StringBuilder sb = new StringBuilder(255);
                GetWindowText(hWnd, sb, sb.Capacity + 1);
                string title = sb.ToString();

                if(IsWindowVisible(hWnd) && !string.IsNullOrEmpty(title))
                {
                    var item = new PinnedWindowListItem
                    {
                        Pinned = false,
                        Title = title,
                        Handle = hWnd
                    };

                    var hIcon = GetIconHandle(hWnd);
                    if(hIcon != IntPtr.Zero)
                    {
                        var iconImgSrc = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(hIcon, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                        item.Icon = iconImgSrc;
                    }

                    StringBuilder sbFilename = new StringBuilder(1024);
                    GetWindowModuleFileName(hWnd, sbFilename, sbFilename.Capacity + 1);
                    item.FileName = sbFilename.ToString();
                    
                    collection.Add(item);
                }
                return true;
            }, IntPtr.Zero);
            return collection;
        }

        public static void PinWindow(IntPtr hWnd)
        {
            if(hWnd != IntPtr.Zero && IsWindowVisible(hWnd))
            {
                if (IsIconic(hWnd))
                {
                    ShowWindow(hWnd, SW_RESTORE);
                }
                //else
                //{
                //    ShowWindow(hWnd, SW_SHOW);
                //}
                //SetForegroundWindow(hWnd);
                SetWindowPos(hWnd, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE | SWP_SHOWWINDOW );
            }
        }

        public static void UnPinWindow(IntPtr hWnd)
        {
            if (hWnd != IntPtr.Zero && IsWindowVisible(hWnd))
            {
                SetWindowPos(hWnd, HWND_NOTOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE);
            }
        }
    }

}
