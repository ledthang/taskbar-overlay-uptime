namespace TaskbarOverlayUptime.Models;

public sealed class OverlaySettings
{
    public double FontSize { get; init; } = 11;
    public int MaxRows { get; init; } = 1;
    public bool AllowResize { get; init; }
    public bool OverlayOnTaskbar { get; init; } = true;
    public double RightPadding { get; init; } = 160;
    public bool AutoRightPaddingFromTray { get; init; } = true;
}
