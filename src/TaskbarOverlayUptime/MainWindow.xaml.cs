using System.Windows;
using TaskbarOverlayUptime.Services;
using TaskbarOverlayUptime.Services.Logging;
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

        AppLogger.Info($"App start. Log file: {AppLogger.GetLogPath()}");

        var settings = new OverlaySettingsLoader("Config/overlay-settings.json").LoadAsync().GetAwaiter().GetResult();
        AppLogger.Info($"Settings loaded: width={settings.Width}, height={settings.Height}, resize={settings.AllowResize}, overlayOnTaskbar={settings.OverlayOnTaskbar}");

        Width = settings.Width;
        Height = settings.Height;
        ResizeMode = settings.AllowResize ? ResizeMode.CanResizeWithGrip : ResizeMode.NoResize;

        var registry = new NodeRegistry("Config/nodes.json");
        var pingMonitor = new PingMonitor();
        var serviceMonitor = new ServiceMonitor();
        var httpMonitor = new HttpMonitor();
        var tcpMonitor = new TcpMonitor();
        var anchorService = new TaskbarAnchorService();
        var overlayWindowService = new OverlayWindowService();

        var mainVm = new MainViewModel(registry, pingMonitor, serviceMonitor, httpMonitor, tcpMonitor, settings);
        DataContext = mainVm;

        Loaded += (_, _) =>
        {
            AppLogger.Info($"Loaded event before dock. Left={Left}, Top={Top}, Width={Width}, Height={Height}, WindowState={WindowState}");
            anchorService.DockToTaskbar(this, settings.OverlayOnTaskbar);
            AppLogger.Info($"After dock. Left={Left}, Top={Top}, Width={Width}, Height={Height}");
            overlayWindowService.ApplyOverlayStyle(this, clickThrough: false);
            overlayWindowService.EnsureTopmost(this);
            AppLogger.Info("Overlay style/topmost applied.");
        };

        Deactivated += (_, _) =>
        {
            AppLogger.Info("Window deactivated. Re-applying topmost.");
            overlayWindowService.EnsureTopmost(this);
        };

        StateChanged += (_, _) =>
        {
            AppLogger.Info($"StateChanged: {WindowState}");
            if (WindowState == WindowState.Minimized)
            {
                WindowState = WindowState.Normal;
                overlayWindowService.EnsureTopmost(this);
                AppLogger.Info("Window was minimized; restored to normal and re-applied topmost.");
            }
        };

        _scheduler = new MonitorScheduler(mainVm.RefreshAsync, TimeSpan.FromSeconds(10));
        Loaded += async (_, _) => await mainVm.RefreshAsync();
        Loaded += async (_, _) => await _scheduler.StartAsync();
        Closed += (_, _) =>
        {
            AppLogger.Info("Window closed. Scheduler disposed.");
            _scheduler.Dispose();
        };
    }
}
