namespace TaskbarOverlayUptime.Models;

public sealed record MonitorResult(string Name, bool IsHealthy, string Detail, DateTimeOffset CheckedAt);
