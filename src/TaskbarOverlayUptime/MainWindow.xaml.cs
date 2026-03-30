using System.Windows;
using TaskbarOverlayUptime.Services;
using TaskbarOverlayUptime.Services.Monitoring;
using TaskbarOverlayUptime.Services.Native;
using TaskbarOverlayUptime.ViewModels;

namespace TaskbarOverlayUptime;

public partial class MainWindow : Window
{
    private readonly MonitorScheduler _scheduler;

    public MainWindow()
    {
        InitializeComponent();

        var registry = new NodeRegistry("Config/nodes.json");
        var pingMonitor = new PingMonitor();
        var serviceMonitor = new ServiceMonitor();
        var httpMonitor = new HttpMonitor();
        var tcpMonitor = new TcpMonitor();
        var anchorService = new TaskbarAnchorService();
        var overlayWindowService = new OverlayWindowService();

        var mainVm = new MainViewModel(registry, pingMonitor, serviceMonitor, httpMonitor, tcpMonitor);
        DataContext = mainVm;

        Loaded += (_, _) =>
        {
            anchorService.DockToTaskbar(this);
            overlayWindowService.ApplyOverlayStyle(this, clickThrough: false);
        };

        _scheduler = new MonitorScheduler(mainVm.RefreshAsync, TimeSpan.FromSeconds(10));
        Loaded += async (_, _) => await mainVm.RefreshAsync();
        Loaded += async (_, _) => await _scheduler.StartAsync();
        Closed += (_, _) => _scheduler.Dispose();
    }
}
