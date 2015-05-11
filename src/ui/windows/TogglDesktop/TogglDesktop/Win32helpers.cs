using System;
using System.Runtime.InteropServices;

namespace TogglDesktop
{
    static class Win32helper
    {
        [DllImport("user32.dll")]
        private static extern int ReleaseCapture();

        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hwnd, int msg, int wparam, int lparam);

        private const int wmNcLButtonDown = 0xA1;
        private const int wmNcLButtonUp = 0xA2;
        private const int HtBottomRight = 17;

        // FIXME: what is happening here
        public static void DoSomething(IntPtr handle, bool isResizing)
        {
            ReleaseCapture();

            int buttonEvent = isResizing ? wmNcLButtonDown : wmNcLButtonUp;

            SendMessage(handle, buttonEvent, HtBottomRight, 0);
        }

        [StructLayout(LayoutKind.Sequential)]
        struct LASTINPUTINFO
        {
            public static readonly int SizeOf =
                Marshal.SizeOf(typeof(LASTINPUTINFO));

            [MarshalAs(UnmanagedType.U4)]
            public int cbSize;

            [MarshalAs(UnmanagedType.U4)]
            public int dwTime;
        }

        [DllImport("user32.dll")]
        static extern bool GetLastInputInfo(out LASTINPUTINFO plii);

        public static int GetIdleSeconds()
        {
            LASTINPUTINFO lastInputInfo = new LASTINPUTINFO();
            lastInputInfo.cbSize = Marshal.SizeOf(lastInputInfo);
            lastInputInfo.dwTime = 0;
            if (!GetLastInputInfo(out lastInputInfo))
            {
                return 0;
            }
            int idle_seconds = unchecked(Environment.TickCount - (int)lastInputInfo.dwTime) / 1000;
            if (idle_seconds < 1)
            {
                return 0;
            }

            return idle_seconds;
        }

        [DllImport("user32", CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool ShowScrollBar(IntPtr hwnd, int wBar, [MarshalAs(UnmanagedType.Bool)] bool bShow);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetWindowPos(IntPtr hWnd,
            int hWndInsertAfter, int x, int u, int cx, int cy, int uFlags);

        private const int HWND_TOPMOST = -1;
        private const int HWND_NOTOPMOST = -2;
        private const int SWP_NOMOVE = 0x0002;
        private const int SWP_NOSIZE = 0x0001;
        private const int SB_HORZ = 0;

        public static void SetWindowTopMost(IntPtr handle)
        {
            SetWindowPos(handle, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE);
        }

        public static void UnsetWindowTopMost(IntPtr handle)
        {
            SetWindowPos(handle, HWND_NOTOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE);
        }
    }
}