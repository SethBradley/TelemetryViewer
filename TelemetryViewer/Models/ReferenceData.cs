using System.Text.Json.Serialization;

namespace TelemetryViewer.Models;

public sealed class WeaponConfig
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = "";

    [JsonPropertyName("category")]
    public string? Category { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; } = "";

    [JsonPropertyName("tier")]
    public int Tier { get; set; }

    [JsonPropertyName("damage")]
    public double Damage { get; set; }

    [JsonPropertyName("fire_rate")]
    public double FireRate { get; set; }

    [JsonPropertyName("range")]
    public double Range { get; set; }

    [JsonPropertyName("cost")]
    public int Cost { get; set; }

    [JsonPropertyName("armor_pierce")]
    public double ArmorPierce { get; set; }

    [JsonPropertyName("attack_mode")]
    public string? AttackMode { get; set; }

    [JsonPropertyName("rarity")]
    public string? Rarity { get; set; }

    [JsonPropertyName("weight")]
    public int Weight { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; } = "";

    [JsonPropertyName("effects")]
    public Dictionary<string, object>? Effects { get; set; }

    [JsonPropertyName("visual")]
    public WeaponVisual? Visual { get; set; }
}

public sealed class WeaponVisual
{
    [JsonPropertyName("color")]
    public string? Color { get; set; }

    [JsonPropertyName("emission")]
    public string? Emission { get; set; }

    [JsonPropertyName("shape")]
    public string? Shape { get; set; }
}

public sealed class EnemyConfig
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = "";

    [JsonPropertyName("type")]
    public string Type { get; set; } = "";

    [JsonPropertyName("health")]
    public double Health { get; set; }

    [JsonPropertyName("armor")]
    public double Armor { get; set; }

    [JsonPropertyName("speed")]
    public double Speed { get; set; }

    [JsonPropertyName("bounty")]
    public int Bounty { get; set; }

    [JsonPropertyName("tower_damage")]
    public double TowerDamage { get; set; }

    [JsonPropertyName("send_cost")]
    public int SendCost { get; set; }

    [JsonPropertyName("attack_range")]
    public double AttackRange { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; } = "";
}

public sealed class UpgradeConfig
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = "";

    [JsonPropertyName("category")]
    public string? Category { get; set; }

    [JsonPropertyName("stat")]
    public string Stat { get; set; } = "";

    [JsonPropertyName("value")]
    public double Value { get; set; }

    [JsonPropertyName("cost")]
    public int Cost { get; set; }

    [JsonPropertyName("rarity")]
    public string? Rarity { get; set; }

    [JsonPropertyName("weight")]
    public int Weight { get; set; }

    [JsonPropertyName("stackable")]
    public bool Stackable { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; } = "";
}
