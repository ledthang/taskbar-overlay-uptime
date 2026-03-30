using System.IO;

namespace TaskbarOverlayUptime.Services.Logging;

public static class AppLogger
{
    private static readonly object Sync = new();
    private static readonly string LogDir = Path.Combine(AppContext.BaseDirectory, "logs");
    private static readonly string LogPath = Path.Combine(LogDir, "overlay.log");

    public static void Info(string message)
    {
        Write("INFO", message);
    }

    public static void Error(string message, Exception? ex = null)
    {
        Write("ERROR", ex is null ? message : $"{message} | {ex}");
    }

    private static void Write(string level, string message)
    {
        lock (Sync)
        {
            Directory.CreateDirectory(LogDir);
            File.AppendAllText(LogPath, $"{DateTimeOffset.Now:yyyy-MM-dd HH:mm:ss.fff zzz} [{level}] {message}{Environment.NewLine}");
        }
    }

    public static string GetLogPath() => LogPath;
}
