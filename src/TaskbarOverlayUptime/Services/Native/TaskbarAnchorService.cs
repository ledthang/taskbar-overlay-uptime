using System.Windows;
using TaskbarOverlayUptime.Services.Logging;

namespace TaskbarOverlayUptime.Services.Native;

public sealed class TaskbarAnchorService
{
    public void DockToTaskbar(Window window, bool overlayOnTaskbar)
    {
        var workArea = SystemParameters.WorkArea;
        var screenWidth = SystemParameters.PrimaryScreenWidth;
        var screenHeight = SystemParameters.PrimaryScreenHeight;
        const int margin = 8;

        AppLogger.Info($"DockToTaskbar called. overlayOnTaskbar={overlayOnTaskbar}, workArea={workArea}, screen={screenWidth}x{screenHeight}");

        if (!overlayOnTaskbar)
        {
            window.Left = workArea.Right - window.Width - margin;
            window.Top = workArea.Bottom - window.Height - margin;
            AppLogger.Info($"Docked (workArea mode): left={window.Left}, top={window.Top}");
            return;
        }

        var bottomThickness = screenHeight - workArea.Bottom;
        var topThickness = workArea.Top;
        var leftThickness = workArea.Left;
        var rightThickness = screenWidth - workArea.Right;

        AppLogger.Info($"Taskbar thickness inference. bottom={bottomThickness}, top={topThickness}, left={leftThickness}, right={rightThickness}");

        if (bottomThickness >= topThickness && bottomThickness >= leftThickness && bottomThickness >= rightThickness)
        {
            window.Left = workArea.Right - window.Width - margin;
            window.Top = workArea.Bottom + Math.Max(0, (bottomThickness - window.Height) / 2.0);
            AppLogger.Info($"Docked (bottom in-taskbar): left={window.Left}, top={window.Top}");
            return;
        }

        if (topThickness >= leftThickness && topThickness >= rightThickness)
        {
            window.Left = workArea.Right - window.Width - margin;
            window.Top = Math.Max(0, (topThickness - window.Height) / 2.0);
            AppLogger.Info($"Docked (top in-taskbar): left={window.Left}, top={window.Top}");
            return;
        }

        if (leftThickness >= rightThickness)
        {
            window.Left = Math.Max(0, (leftThickness - window.Width) / 2.0);
            window.Top = workArea.Bottom - window.Height - margin;
            AppLogger.Info($"Docked (left edge): left={window.Left}, top={window.Top}");
            return;
        }

        window.Left = workArea.Right + Math.Max(0, (rightThickness - window.Width) / 2.0);
        window.Top = workArea.Bottom - window.Height - margin;
        AppLogger.Info($"Docked (right edge): left={window.Left}, top={window.Top}");
    }
}
