using System.IO;
using System.Text.Json;
using TaskbarOverlayUptime.Infrastructure;
using TaskbarOverlayUptime.Models;
using TaskbarOverlayUptime.Services.Logging;

namespace TaskbarOverlayUptime.Services;

public sealed class OverlaySettingsLoader
{
    private readonly string _path;
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true,
        Converters = { new MonitorTypeJsonConverter() }
    };

    public OverlaySettingsLoader(string path)
    {
        _path = path;
    }

    public OverlaySettings Load()
    {
        if (!File.Exists(_path))
        {
            AppLogger.Error($"Config file not found: {_path}");
            return new OverlaySettings();
        }

        try
        {
            var json = File.ReadAllText(_path);
            var loaded = JsonSerializer.Deserialize<AppConfig>(json, JsonOptions);

            if (loaded?.Overlay is null)
            {
                AppLogger.Error($"Overlay settings missing in config: {_path}");
                return new OverlaySettings();
            }

            return loaded.Overlay;
        }
        catch (Exception ex)
        {
            AppLogger.Error($"Failed to parse config: {_path}", ex);
            return new OverlaySettings();
        }
    }
}
