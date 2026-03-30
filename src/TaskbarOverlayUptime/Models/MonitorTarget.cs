namespace TaskbarOverlayUptime.Models;

public enum MonitorType
{
    Ping,
    Service,
    Http
}

public sealed record MonitorTarget(string Name, MonitorType Type, string Target);
