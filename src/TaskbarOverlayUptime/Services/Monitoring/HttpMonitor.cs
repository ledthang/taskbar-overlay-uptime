using TaskbarOverlayUptime.Models;

namespace TaskbarOverlayUptime.Services.Monitoring;

public sealed class HttpMonitor
{
    private static readonly HttpClient Client = new()
    {
        Timeout = TimeSpan.FromSeconds(5)
    };

    public async Task<MonitorResult> CheckAsync(MonitorTarget target)
    {
        try
        {
            using var response = await Client.GetAsync(target.Target);
            var healthy = (int)response.StatusCode is >= 200 and < 300;
            var detail = $"HTTP {(int)response.StatusCode} {response.ReasonPhrase}";
            return new MonitorResult(target.Name, healthy, detail, DateTimeOffset.Now);
        }
        catch (Exception ex)
        {
            return new MonitorResult(target.Name, false, $"HTTP error: {ex.Message}", DateTimeOffset.Now);
        }
    }
}
