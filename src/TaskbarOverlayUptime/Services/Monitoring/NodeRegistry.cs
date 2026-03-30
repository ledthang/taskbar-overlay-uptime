using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using TaskbarOverlayUptime.Models;

namespace TaskbarOverlayUptime.Services.Monitoring;

public sealed class NodeRegistry
{
    private readonly string _path;

    public NodeRegistry(string path)
    {
        _path = path;
    }

    public Task<IReadOnlyList<MonitorTarget>> LoadTargetsAsync()
    {
        if (!File.Exists(_path))
        {
            return Task.FromResult<IReadOnlyList<MonitorTarget>>(Array.Empty<MonitorTarget>());
        }

        try
        {
            var json = File.ReadAllText(_path);
            var config = JsonSerializer.Deserialize<AppConfig>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringEnumConverter() }
            });

            return Task.FromResult<IReadOnlyList<MonitorTarget>>(config?.Targets ?? []);
        }
        catch
        {
            return Task.FromResult<IReadOnlyList<MonitorTarget>>(Array.Empty<MonitorTarget>());
        }
    }
}
