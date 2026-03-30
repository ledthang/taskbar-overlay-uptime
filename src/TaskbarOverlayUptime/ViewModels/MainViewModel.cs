using System.Collections.ObjectModel;
using TaskbarOverlayUptime.Models;
using TaskbarOverlayUptime.Services.Monitoring;

namespace TaskbarOverlayUptime.ViewModels;

public sealed class MainViewModel
{
    private readonly NodeRegistry _registry;
    private readonly PingMonitor _pingMonitor;
    private readonly ServiceMonitor _serviceMonitor;
    private readonly HttpMonitor _httpMonitor;

    public MainViewModel(NodeRegistry registry, PingMonitor pingMonitor, ServiceMonitor serviceMonitor, HttpMonitor httpMonitor)
    {
        _registry = registry;
        _pingMonitor = pingMonitor;
        _serviceMonitor = serviceMonitor;
        _httpMonitor = httpMonitor;

        Cards = new ObservableCollection<MonitorCardViewModel>();
    }

    public ObservableCollection<MonitorCardViewModel> Cards { get; }

    public async Task RefreshAsync()
    {
        var targets = await _registry.LoadTargetsAsync();

        if (Cards.Count != targets.Count)
        {
            Cards.Clear();
            foreach (var target in targets)
            {
                Cards.Add(new MonitorCardViewModel(target.Name));
            }
        }

        for (var i = 0; i < targets.Count; i++)
        {
            var target = targets[i];
            var result = target.Type switch
            {
                MonitorType.Ping => await _pingMonitor.CheckAsync(target),
                MonitorType.Service => await _serviceMonitor.CheckAsync(target),
                MonitorType.Http => await _httpMonitor.CheckAsync(target),
                _ => new MonitorResult(target.Name, false, "Unsupported monitor type", DateTimeOffset.Now)
            };

            Cards[i].StatusText = result.IsHealthy ? "Healthy" : "Down";
            Cards[i].StatusColor = result.IsHealthy ? "#16A34A" : "#DC2626";
            Cards[i].Detail = $"{result.Detail} • {result.CheckedAt:HH:mm:ss}";
        }
    }
}
