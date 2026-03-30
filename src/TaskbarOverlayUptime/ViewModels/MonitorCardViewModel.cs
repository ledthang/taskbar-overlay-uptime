using TaskbarOverlayUptime.Infrastructure;

namespace TaskbarOverlayUptime.ViewModels;

public sealed class MonitorCardViewModel : ObservableObject
{
    private string _statusText = "Unknown";
    private string _statusColor = "#6B7280";
    private string _detail = "Waiting for first refresh...";

    public MonitorCardViewModel(string name)
    {
        Name = name;
    }

    public string Name { get; }

    public string StatusText
    {
        get => _statusText;
        set => Set(ref _statusText, value);
    }

    public string StatusColor
    {
        get => _statusColor;
        set => Set(ref _statusColor, value);
    }

    public string Detail
    {
        get => _detail;
        set => Set(ref _detail, value);
    }
}
