using System.Windows;

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

        if (!overlayOnTaskbar)
        {
            window.Left = workArea.Right - window.Width - margin;
            window.Top = workArea.Bottom - window.Height - margin;
            return;
        }

        // Infer taskbar edge from screen bounds vs work area and place overlay overlapping taskbar.
        var bottomThickness = screenHeight - workArea.Bottom;
        var topThickness = workArea.Top;
        var leftThickness = workArea.Left;
        var rightThickness = screenWidth - workArea.Right;

        if (bottomThickness >= topThickness && bottomThickness >= leftThickness && bottomThickness >= rightThickness)
        {
            window.Left = workArea.Right - window.Width - margin;
            window.Top = workArea.Bottom - window.Height + overlap;
            return;
        }

        if (topThickness >= leftThickness && topThickness >= rightThickness)
        {
            window.Left = workArea.Right - window.Width - margin;
            window.Top = workArea.Top - overlap;
            return;
        }

        if (leftThickness >= rightThickness)
        {
            window.Left = workArea.Left - overlap;
            window.Top = workArea.Bottom - window.Height - margin;
            return;
        }

        window.Left = workArea.Right - window.Width + overlap;
        window.Top = workArea.Bottom - window.Height - margin;
    }
}
