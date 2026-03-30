using System.Net.NetworkInformation;
using TaskbarOverlayUptime.Models;

namespace TaskbarOverlayUptime.Services.Monitoring;

public sealed class PingMonitor
{
    public async Task<MonitorResult> CheckAsync(MonitorTarget target)
    {
        using var ping = new Ping();

        try
        {
            var reply = await ping.SendPingAsync(target.Target, 3_000);
            var healthy = reply.Status == IPStatus.Success;
            var detail = healthy
                ? $"Ping OK: {reply.RoundtripTime} ms"
                : $"Ping failed: {reply.Status}";

            return new MonitorResult(target.Name, healthy, detail, DateTimeOffset.Now);
        }
        catch (Exception ex)
        {
            return new MonitorResult(target.Name, false, $"Ping error: {ex.Message}", DateTimeOffset.Now);
        }
    }
}
