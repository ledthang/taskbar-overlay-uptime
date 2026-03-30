using System.Runtime.InteropServices;
using System.Windows;

namespace TaskbarOverlayUptime.Services.Native;

public sealed class TaskbarAnchorService
{
    private const int AbmGettaskbarpos = 0x00000005;

    public void DockToTaskbar(Window window, bool overlayOnTaskbar)
    {
        if (!overlayOnTaskbar)
        {
            var workArea = SystemParameters.WorkArea;
            window.Left = workArea.Right - window.Width - 8;
            window.Top = workArea.Bottom - window.Height - 8;
            return;
        }

        var data = new AppBarData
        {
            cbSize = Marshal.SizeOf<AppBarData>()
        };

        var result = SHAppBarMessage(AbmGettaskbarpos, ref data);
        if (result == IntPtr.Zero)
        {
            var workArea = SystemParameters.WorkArea;
            window.Left = workArea.Right - window.Width - 8;
            window.Top = workArea.Bottom - window.Height - 2;
            return;
        }

        const int margin = 8;
        switch (data.uEdge)
        {
            case AppBarEdge.Bottom:
                window.Left = data.rc.Right - window.Width - margin;
                window.Top = data.rc.Top + (data.rc.Bottom - data.rc.Top - window.Height) / 2.0;
                break;
            case AppBarEdge.Top:
                window.Left = data.rc.Right - window.Width - margin;
                window.Top = data.rc.Top + (data.rc.Bottom - data.rc.Top - window.Height) / 2.0;
                break;
            case AppBarEdge.Left:
                window.Left = data.rc.Left + (data.rc.Right - data.rc.Left - window.Width) / 2.0;
                window.Top = data.rc.Bottom - window.Height - margin;
                break;
            case AppBarEdge.Right:
                window.Left = data.rc.Left + (data.rc.Right - data.rc.Left - window.Width) / 2.0;
                window.Top = data.rc.Bottom - window.Height - margin;
                break;
        }
    }

    [DllImport("shell32.dll")]
    private static extern IntPtr SHAppBarMessage(int dwMessage, ref AppBarData pData);

    [StructLayout(LayoutKind.Sequential)]
    private struct AppBarData
    {
        public int cbSize;
        public IntPtr hWnd;
        public uint uCallbackMessage;
        public AppBarEdge uEdge;
        public RectNative rc;
        public int lParam;
    }

    private enum AppBarEdge : uint
    {
        Left = 0,
        Top = 1,
        Right = 2,
        Bottom = 3
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct RectNative
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }
}
