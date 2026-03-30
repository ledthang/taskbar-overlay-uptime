using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace TaskbarOverlayUptime.Services.Native;

public sealed class OverlayWindowService
{
    private const int GwlExstyle = -20;
    private const int WsExToolwindow = 0x00000080;
    private const int WsExLayered = 0x00080000;
    private const int WsExTransparent = 0x00000020;

    public void ApplyOverlayStyle(Window window, bool clickThrough)
    {
        var hwnd = new WindowInteropHelper(window).Handle;
        var exStyle = GetWindowLong(hwnd, GwlExstyle);
        exStyle |= WsExToolwindow | WsExLayered;

        if (clickThrough)
        {
            exStyle |= WsExTransparent;
        }
        else
        {
            exStyle &= ~WsExTransparent;
        }

        SetWindowLong(hwnd, GwlExstyle, exStyle);
    }

    [DllImport("user32.dll")]
    private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll")]
    private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
}
