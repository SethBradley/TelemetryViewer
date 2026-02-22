using System.IO;
using System.Text.Json;
using TelemetryViewer.Models;

namespace TelemetryViewer.Services;

public sealed class ReferenceDataService
{
    private readonly string _basePath;

    public Dictionary<string, WeaponConfig> Weapons { get; private set; } = new();
    public Dictionary<string, EnemyConfig> Enemies { get; private set; } = new();
    public Dictionary<string, UpgradeConfig> Upgrades { get; private set; } = new();

    public ReferenceDataService(string gamePath)
    {
        _basePath = gamePath;
    }

    public void Load()
    {
        Weapons = LoadConfig<WeaponConfig>(Path.Combine(_basePath, "data", "weapons", "weapons_config.json"));
        Enemies = LoadConfig<EnemyConfig>(Path.Combine(_basePath, "data", "enemies", "enemies_config.json"));
        Upgrades = LoadConfig<UpgradeConfig>(Path.Combine(_basePath, "data", "upgrades", "upgrades_config.json"));
    }

    public string GetWeaponName(string id) =>
        Weapons.TryGetValue(id, out var w) ? w.Name : id;

    public string GetEnemyName(string id) =>
        Enemies.TryGetValue(id, out var e) ? e.Name : id;

    public string GetUpgradeName(string id) =>
        Upgrades.TryGetValue(id, out var u) ? u.Name : id;

    private static Dictionary<string, T> LoadConfig<T>(string path)
    {
        if (!File.Exists(path))
            return new Dictionary<string, T>();

        var json = File.ReadAllText(path);
        var raw = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json)
                  ?? new Dictionary<string, JsonElement>();

        var result = new Dictionary<string, T>();
        foreach (var (key, element) in raw)
        {
            if (key.StartsWith("_"))
                continue;

            var item = element.Deserialize<T>();
            if (item is not null)
                result[key] = item;
        }

        return result;
    }
}
