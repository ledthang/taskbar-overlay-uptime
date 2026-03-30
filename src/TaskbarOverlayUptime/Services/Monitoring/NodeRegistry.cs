using System.IO;
using System.Text.Json;
using TaskbarOverlayUptime.Infrastructure;
using TaskbarOverlayUptime.Models;
using TaskbarOverlayUptime.Services.Logging;

namespace TaskbarOverlayUptime.Services.Monitoring;

public sealed class NodeRegistry
{
    private readonly string _path;
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true,
        Converters = { new MonitorTypeJsonConverter() }
    };

    public NodeRegistry(string path)
    {
        _path = path;
    }

    public Task<IReadOnlyList<MonitorTarget>> LoadTargetsAsync()
    {
        if (!File.Exists(_path))
        {
            AppLogger.Error($"Config file not found: {_path}");
            return Task.FromResult<IReadOnlyList<MonitorTarget>>(Array.Empty<MonitorTarget>());
        }

        try
        {
            var json = File.ReadAllText(_path);
            var config = JsonSerializer.Deserialize<AppConfig>(json, JsonOptions);
            var targets = config?.Targets;

            if (targets is null || targets.Count == 0)
            {
                AppLogger.Error($"No targets found in config: {_path}");
                return Task.FromResult<IReadOnlyList<MonitorTarget>>(Array.Empty<MonitorTarget>());
            }

            return Task.FromResult<IReadOnlyList<MonitorTarget>>(targets);
        }
        catch (Exception ex)
        {
            AppLogger.Error($"Failed to parse targets from config: {_path}", ex);
            return Task.FromResult<IReadOnlyList<MonitorTarget>>(Array.Empty<MonitorTarget>());
        }
    }
}
