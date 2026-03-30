using System.Runtime.InteropServices;
using System.Windows;
using TaskbarOverlayUptime.Services.Logging;

namespace TaskbarOverlayUptime.Services.Native;

public sealed class TaskbarAnchorService
{
    public double GetRecommendedRightPadding(double configuredPadding, bool autoRightPaddingFromTray)
    {
        var padding = Math.Max(0, configuredPadding);
        if (!autoRightPaddingFromTray)
        {
            return padding;
        }

        var trayPadding = GetTrayPaddingEstimate();
        var finalPadding = Math.Max(padding, trayPadding);
        AppLogger.Info($"Right padding resolved. configured={configuredPadding}, tray={trayPadding}, final={finalPadding}");
        return finalPadding;
    }

    public void DockToTaskbar(Window window, bool overlayOnTaskbar)
    {
        var workArea = SystemParameters.WorkArea;
        var screenWidth = SystemParameters.PrimaryScreenWidth;
        var screenHeight = SystemParameters.PrimaryScreenHeight;
        const int rightMargin = 4;

        AppLogger.Info($"DockToTaskbar called. overlayOnTaskbar={overlayOnTaskbar}, workArea={workArea}, screen={screenWidth}x{screenHeight}");

        var bottomThickness = screenHeight - workArea.Bottom;
        var topThickness = workArea.Top;
        var leftThickness = workArea.Left;
        var rightThickness = screenWidth - workArea.Right;

        if (!overlayOnTaskbar)
        {
            window.Left = workArea.Right - window.Width - rightMargin;
            window.Top = workArea.Bottom - window.Height - 8;
            AppLogger.Info($"Docked (workArea mode): left={window.Left}, top={window.Top}");
            return;
        }

        AppLogger.Info($"Taskbar thickness inference. bottom={bottomThickness}, top={topThickness}, left={leftThickness}, right={rightThickness}");

        if (bottomThickness >= topThickness && bottomThickness >= leftThickness && bottomThickness >= rightThickness)
        {
            window.Height = Math.Max(28, bottomThickness);
            window.Left = workArea.Right - window.Width - rightMargin;
            window.Top = workArea.Bottom;
            AppLogger.Info($"Docked (bottom in-taskbar): left={window.Left}, top={window.Top}, height={window.Height}");
            return;
        }

        if (topThickness >= leftThickness && topThickness >= rightThickness)
        {
            window.Height = Math.Max(28, topThickness);
            window.Left = workArea.Right - window.Width - rightMargin;
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

        return GetWindowRect(notify, out var rect) ? Math.Max(0, rect.Right - rect.Left) + 16 : 0;
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
