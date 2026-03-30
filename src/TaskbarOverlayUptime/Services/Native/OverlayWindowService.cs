using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using TaskbarOverlayUptime.Services.Logging;

namespace TaskbarOverlayUptime.Services.Native;

public sealed class OverlayWindowService
{
    private const int GwlExstyle = -20;
    private const int WsExToolwindow = 0x00000080;
    private const int WsExLayered = 0x00080000;
    private const int WsExTransparent = 0x00000020;

    private static readonly IntPtr HwndTopmost = new(-1);
    private const uint SwpNomove = 0x0002;
    private const uint SwpNosize = 0x0001;
    private const uint SwpShowwindow = 0x0040;

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
        AppLogger.Info($"ApplyOverlayStyle: hwnd={hwnd}, exStyle=0x{exStyle:X}, clickThrough={clickThrough}");
    }

    public void EnsureTopmost(Window window)
    {
        var hwnd = new WindowInteropHelper(window).Handle;
        SetWindowPos(hwnd, HwndTopmost, 0, 0, 0, 0, SwpNomove | SwpNosize | SwpShowwindow);
        AppLogger.Info($"EnsureTopmost: hwnd={hwnd}");
    }

    [DllImport("user32.dll")]
    private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll")]
    private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
}
