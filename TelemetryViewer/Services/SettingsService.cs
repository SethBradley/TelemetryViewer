using System.IO;
using System.Text.Json;

namespace TelemetryViewer.Services;

public static class SettingsService
{
    private static readonly string SettingsDir =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "TelemetryViewer");

    private static readonly string SettingsFile = Path.Combine(SettingsDir, "settings.json");

    public static string? LoadGamePath()
    {
        if (!File.Exists(SettingsFile))
            return null;

        try
        {
            var json = File.ReadAllText(SettingsFile);
            var doc = JsonDocument.Parse(json);
            if (doc.RootElement.TryGetProperty("game_path", out var val))
            {
                var path = val.GetString();
                if (!string.IsNullOrWhiteSpace(path) && Directory.Exists(path))
                    return path;
            }
        }
        catch { }

        return null;
    }

    public static void SaveGamePath(string path)
    {
        Directory.CreateDirectory(SettingsDir);
        var json = JsonSerializer.Serialize(new { game_path = path });
        File.WriteAllText(SettingsFile, json);
    }
}
