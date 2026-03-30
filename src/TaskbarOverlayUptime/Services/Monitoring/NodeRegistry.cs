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

    public async Task<IReadOnlyList<MonitorTarget>> LoadTargetsAsync()
    {
        if (!File.Exists(_path))
        {
            return Array.Empty<MonitorTarget>();
        }

        await using var stream = File.OpenRead(_path);
        var targets = await JsonSerializer.DeserializeAsync<List<MonitorTarget>>(stream, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() }
        });

        return targets ?? Array.Empty<MonitorTarget>();
    }
}
