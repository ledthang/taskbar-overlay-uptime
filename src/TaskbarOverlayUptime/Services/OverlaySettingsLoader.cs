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

    public async Task<OverlaySettings> LoadAsync()
    {
        if (!File.Exists(_path))
        {
            return new OverlaySettings();
        }

        await using var stream = File.OpenRead(_path);
        var loaded = await JsonSerializer.DeserializeAsync<OverlaySettings>(stream, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        return loaded ?? new OverlaySettings();
    }
}
