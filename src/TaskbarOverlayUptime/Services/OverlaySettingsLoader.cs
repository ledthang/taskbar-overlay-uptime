using System.IO;
using System.Text.Json;
using TaskbarOverlayUptime.Models;

namespace TaskbarOverlayUptime.Services;

public sealed class OverlaySettingsLoader
{
    private readonly string _path;

    public OverlaySettingsLoader(string path)
    {
        _path = path;
    }

    public OverlaySettings Load()
    {
        if (!File.Exists(_path))
        {
            return new OverlaySettings();
        }

        try
        {
            var json = File.ReadAllText(_path);
            var loaded = JsonSerializer.Deserialize<AppConfig>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return loaded?.Overlay ?? new OverlaySettings();
        }
        catch
        {
            return new OverlaySettings();
        }
    }
}
