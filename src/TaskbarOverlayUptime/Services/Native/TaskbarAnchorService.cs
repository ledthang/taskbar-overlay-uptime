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
        const int overlap = 6;

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
            window.Top = workArea.Bottom - window.Height + overlap;
            AppLogger.Info($"Docked (bottom): left={window.Left}, top={window.Top}");
            return;
        }

        if (topThickness >= leftThickness && topThickness >= rightThickness)
        {
            window.Left = workArea.Right - window.Width - margin;
            window.Top = workArea.Top - overlap;
            AppLogger.Info($"Docked (top): left={window.Left}, top={window.Top}");
            return;
        }

        if (leftThickness >= rightThickness)
        {
            window.Left = workArea.Left - overlap;
            window.Top = workArea.Bottom - window.Height - margin;
            AppLogger.Info($"Docked (left): left={window.Left}, top={window.Top}");
            return;
        }

        window.Left = workArea.Right - window.Width + overlap;
        window.Top = workArea.Bottom - window.Height - margin;
        AppLogger.Info($"Docked (right): left={window.Left}, top={window.Top}");
    }
}
