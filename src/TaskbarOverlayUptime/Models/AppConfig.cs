namespace TaskbarOverlayUptime.Models;

public sealed class AppConfig
{
    public OverlaySettings Overlay { get; init; } = new();
    public List<MonitorTarget> Targets { get; init; } = [];
}
