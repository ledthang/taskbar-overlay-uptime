using System.ServiceProcess;
using TaskbarOverlayUptime.Models;

namespace TaskbarOverlayUptime.Services.Monitoring;

public sealed class ServiceMonitor
{
    public Task<MonitorResult> CheckAsync(MonitorTarget target)
    {
        try
        {
            using var controller = new ServiceController(target.Target);
            var status = controller.Status;
            var healthy = status == ServiceControllerStatus.Running;
            var detail = $"Service status: {status}";

            return Task.FromResult(new MonitorResult(target.Name, healthy, detail, DateTimeOffset.Now));
        }
        catch (Exception ex)
        {
            return Task.FromResult(new MonitorResult(target.Name, false, $"Service error: {ex.Message}", DateTimeOffset.Now));
        }
    }
}
