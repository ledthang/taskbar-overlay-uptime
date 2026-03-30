using System.Windows;
using System.Windows.Threading;
using TaskbarOverlayUptime.Services;
using TaskbarOverlayUptime.Services.Logging;
using TaskbarOverlayUptime.Services.Monitoring;
using TaskbarOverlayUptime.Services.Native;
using TaskbarOverlayUptime.ViewModels;

namespace TaskbarOverlayUptime;

public partial class MainWindow : Window
{
    private readonly MonitorScheduler _scheduler;
    private readonly DispatcherTimer _topmostHeartbeat;

    public MainWindow()
    {
        InitializeComponent();

        AppLogger.Info($"App start. Log file: {AppLogger.GetLogPath()}");

        var settings = new OverlaySettingsLoader("Config/appsettings.json").Load();
        AppLogger.Info($"Settings loaded: fontSize={settings.FontSize}, maxRows={settings.MaxRows}, rightPadding={settings.RightPadding}, overlayOnTaskbar={settings.OverlayOnTaskbar}");

        ResizeMode = ResizeMode.NoResize;

        var registry = new NodeRegistry("Config/appsettings.json");
        var pingMonitor = new PingMonitor();
        var serviceMonitor = new ServiceMonitor();
        var httpMonitor = new HttpMonitor();
        var tcpMonitor = new TcpMonitor();
        var anchorService = new TaskbarAnchorService();
        var overlayWindowService = new OverlayWindowService();

        var resolvedRightPadding = anchorService.GetRecommendedRightPadding(settings.RightPadding, settings.AutoRightPaddingFromTray);
        var mainVm = new MainViewModel(registry, pingMonitor, serviceMonitor, httpMonitor, tcpMonitor, settings, resolvedRightPadding);
        DataContext = mainVm;

        _topmostHeartbeat = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(3)
        };

        _topmostHeartbeat.Tick += (_, _) =>
        {
            overlayWindowService.EnsureTopmost(this);
            AppLogger.Info($"Topmost heartbeat. Left={Left}, Top={Top}, Width={Width}, Height={Height}");
        };

        Loaded += (_, _) =>
        {
            AppLogger.Info($"Loaded event before dock. Left={Left}, Top={Top}, Width={Width}, Height={Height}, WindowState={WindowState}");
            anchorService.DockToTaskbar(this, settings.OverlayOnTaskbar);
            AppLogger.Info($"After dock. Left={Left}, Top={Top}, Width={Width}, Height={Height}");
            overlayWindowService.ApplyOverlayStyle(this, clickThrough: false);
            overlayWindowService.EnsureTopmost(this);
            _topmostHeartbeat.Start();
            AppLogger.Info("Overlay style/topmost applied. Heartbeat started.");
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
            _topmostHeartbeat.Stop();
            AppLogger.Info("Window closed. Scheduler disposed. Heartbeat stopped.");
            _scheduler.Dispose();
        };
    }
}
