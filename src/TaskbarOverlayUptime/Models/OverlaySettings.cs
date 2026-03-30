namespace TaskbarOverlayUptime.Models;

public sealed class OverlaySettings
{
    public double FontSize { get; init; } = 11;
    public int MaxRows { get; init; } = 1;
    public double Width { get; init; } = 420;
    public double Height { get; init; } = 32;
    public bool AllowResize { get; init; }
    public bool OverlayOnTaskbar { get; init; } = true;
}
