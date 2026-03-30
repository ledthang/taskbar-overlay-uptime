using System.Net.Sockets;
using TaskbarOverlayUptime.Models;

namespace TaskbarOverlayUptime.Services.Monitoring;

public sealed class TcpMonitor
{
    public async Task<MonitorResult> CheckAsync(MonitorTarget target)
    {
        var parts = target.Target.Split(':', 2, StringSplitOptions.TrimEntries);
        if (parts.Length != 2 || !int.TryParse(parts[1], out var port))
        {
            return new MonitorResult(target.Name, false, "TCP target must be in host:port format", DateTimeOffset.Now);
        }

        using var client = new TcpClient();
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3));

        try
        {
            await client.ConnectAsync(parts[0], port, cts.Token);
            return new MonitorResult(target.Name, true, $"TCP OK: {parts[0]}:{port}", DateTimeOffset.Now);
        }
        catch (Exception ex)
        {
            return new MonitorResult(target.Name, false, $"TCP error: {ex.Message}", DateTimeOffset.Now);
        }
    }
}
