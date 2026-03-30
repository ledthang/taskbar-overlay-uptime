using System.Runtime.InteropServices;
using System.Windows;
using TaskbarOverlayUptime.Services.Logging;

namespace TaskbarOverlayUptime.Services.Native;

public sealed class TaskbarAnchorService
{
    public void DockToTaskbar(Window window, bool overlayOnTaskbar, double rightPadding, bool autoRightPaddingFromTray)
    {
        var workArea = SystemParameters.WorkArea;
        var screenWidth = SystemParameters.PrimaryScreenWidth;
        var screenHeight = SystemParameters.PrimaryScreenHeight;

        AppLogger.Info($"DockToTaskbar called. overlayOnTaskbar={overlayOnTaskbar}, workArea={workArea}, screen={screenWidth}x{screenHeight}");

        var bottomThickness = screenHeight - workArea.Bottom;
        var topThickness = workArea.Top;
        var leftThickness = workArea.Left;
        var rightThickness = screenWidth - workArea.Right;

        var effectiveRightPadding = Math.Max(0, rightPadding);
        if (autoRightPaddingFromTray)
        {
            var trayPadding = GetTrayPaddingEstimate();
            effectiveRightPadding = Math.Max(effectiveRightPadding, trayPadding);
            AppLogger.Info($"Auto tray padding enabled. trayPadding={trayPadding}, effectiveRightPadding={effectiveRightPadding}");
        }

        if (!overlayOnTaskbar)
        {
            window.Left = workArea.Right - window.Width - effectiveRightPadding;
            window.Top = workArea.Bottom - window.Height - 8;
            AppLogger.Info($"Docked (workArea mode): left={window.Left}, top={window.Top}");
            return;
        }

        AppLogger.Info($"Taskbar thickness inference. bottom={bottomThickness}, top={topThickness}, left={leftThickness}, right={rightThickness}");

        if (bottomThickness >= topThickness && bottomThickness >= leftThickness && bottomThickness >= rightThickness)
        {
            window.Height = Math.Max(28, bottomThickness);
            window.Left = workArea.Right - window.Width - effectiveRightPadding;
            window.Top = workArea.Bottom + Math.Max(0, (bottomThickness - window.Height) / 2.0);
            AppLogger.Info($"Docked (bottom in-taskbar): left={window.Left}, top={window.Top}, height={window.Height}");
            return;
        }

        if (topThickness >= leftThickness && topThickness >= rightThickness)
        {
            window.Height = Math.Max(28, topThickness);
            window.Left = workArea.Right - window.Width - effectiveRightPadding;
            window.Top = 0;
            AppLogger.Info($"Docked (top in-taskbar): left={window.Left}, top={window.Top}, height={window.Height}");
            return;
        }

        if (leftThickness >= rightThickness)
        {
            window.Left = 0;
            window.Top = workArea.Bottom - window.Height - 8;
            AppLogger.Info($"Docked (left edge): left={window.Left}, top={window.Top}");
            return;
        }

        window.Left = workArea.Right;
        window.Top = workArea.Bottom - window.Height - 8;
        AppLogger.Info($"Docked (right edge): left={window.Left}, top={window.Top}");
    }

    private static double GetTrayPaddingEstimate()
    {
        var shell = FindWindow("Shell_TrayWnd", null);
        if (shell == IntPtr.Zero)
        {
            return 0;
        }

        var notify = FindWindowEx(shell, IntPtr.Zero, "TrayNotifyWnd", null);
        if (notify == IntPtr.Zero)
        {
            return 0;
        }

        return GetWindowRect(notify, out var rect) ? Math.Max(0, rect.Right - rect.Left) + 24 : 0;
    }

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    private static extern IntPtr FindWindow(string? lpClassName, string? lpWindowName);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    private static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr childAfter, string? className, string? windowTitle);

    [DllImport("user32.dll")]
    private static extern bool GetWindowRect(IntPtr hWnd, out RectNative lpRect);

    [StructLayout(LayoutKind.Sequential)]
    private struct RectNative
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }
}
