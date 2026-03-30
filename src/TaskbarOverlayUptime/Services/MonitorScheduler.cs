namespace TaskbarOverlayUptime.Services;

public sealed class MonitorScheduler : IDisposable
{
    private readonly Func<Task> _refresh;
    private readonly PeriodicTimer _timer;
    private readonly CancellationTokenSource _cts = new();

    public MonitorScheduler(Func<Task> refresh, TimeSpan period)
    {
        _refresh = refresh;
        _timer = new PeriodicTimer(period);
    }

    public async Task StartAsync()
    {
        while (await _timer.WaitForNextTickAsync(_cts.Token))
        {
            await _refresh();
        }
    }

    public void Dispose()
    {
        _cts.Cancel();
        _timer.Dispose();
        _cts.Dispose();
    }
}
