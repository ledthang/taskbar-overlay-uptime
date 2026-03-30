using System.Windows;

namespace TaskbarOverlayUptime.Services.Native;

public sealed class TaskbarAnchorService
{
    public void DockToTaskbar(Window window)
    {
        var workArea = SystemParameters.WorkArea;
        window.Left = workArea.Right - window.Width - 8;
        window.Top = workArea.Bottom - window.Height - 8;
    }
}
