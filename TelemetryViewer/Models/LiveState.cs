using System.Text.Json.Serialization;

namespace TelemetryViewer.Models;

public sealed class LiveState
{
    [JsonPropertyName("trial")]
    public int Trial { get; set; }

    [JsonPropertyName("total_trials")]
    public int TotalTrials { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; } = "idle";

    [JsonPropertyName("elapsed_s")]
    public double ElapsedS { get; set; }

    [JsonPropertyName("duration_s")]
    public double DurationS { get; set; }

    [JsonPropertyName("build_profile")]
    public string? BuildProfile { get; set; }

    [JsonPropertyName("difficulty_scalar")]
    public double DifficultyScalar { get; set; }

    [JsonPropertyName("seed")]
    public int Seed { get; set; }

    [JsonPropertyName("tower")]
    public TowerState? Tower { get; set; }

    [JsonPropertyName("economy")]
    public EconomyState? Economy { get; set; }

    [JsonPropertyName("loadout")]
    public List<WeaponSlot>? Loadout { get; set; }

    [JsonPropertyName("upgrades")]
    public List<UpgradeSlot>? Upgrades { get; set; }

    [JsonPropertyName("enemies")]
    public EnemiesState? Enemies { get; set; }

    [JsonPropertyName("combat")]
    public CombatState? Combat { get; set; }

    [JsonPropertyName("kills_by_type")]
    public Dictionary<string, int>? KillsByType { get; set; }

    [JsonPropertyName("recent_events")]
    public List<GameEvent>? RecentEvents { get; set; }

    [JsonPropertyName("hp_timeline")]
    public List<double>? HpTimeline { get; set; }

    [JsonPropertyName("trials_completed")]
    public int TrialsCompleted { get; set; }

    [JsonPropertyName("survived")]
    public int Survived { get; set; }
}

public sealed class TowerState
{
    [JsonPropertyName("hp")]
    public double Hp { get; set; }

    [JsonPropertyName("max_hp")]
    public double MaxHp { get; set; }

    [JsonPropertyName("hp_pct")]
    public double HpPct { get; set; }

    [JsonPropertyName("armor")]
    public double Armor { get; set; }

    [JsonPropertyName("regen_per_s")]
    public double RegenPerS { get; set; }
}

public sealed class EconomyState
{
    [JsonPropertyName("gold")]
    public double Gold { get; set; }

    [JsonPropertyName("total_earned")]
    public double TotalEarned { get; set; }

    [JsonPropertyName("total_spent")]
    public double TotalSpent { get; set; }
}

public sealed class WeaponSlot
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = "";

    [JsonPropertyName("name")]
    public string Name { get; set; } = "";

    [JsonPropertyName("stacks")]
    public int Stacks { get; set; }

    [JsonPropertyName("tier")]
    public int Tier { get; set; }

    [JsonPropertyName("dps")]
    public double Dps { get; set; }

    [JsonPropertyName("range")]
    public double Range { get; set; }
}

public sealed class UpgradeSlot
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = "";

    [JsonPropertyName("count")]
    public int Count { get; set; }
}

public sealed class EnemiesState
{
    [JsonPropertyName("alive")]
    public int Alive { get; set; }

    [JsonPropertyName("breakdown")]
    public Dictionary<string, int>? Breakdown { get; set; }

    [JsonPropertyName("total_spawned")]
    public int TotalSpawned { get; set; }

    [JsonPropertyName("total_killed")]
    public int TotalKilled { get; set; }

    [JsonPropertyName("total_leaked")]
    public int TotalLeaked { get; set; }

    [JsonPropertyName("elites_spawned")]
    public int ElitesSpawned { get; set; }

    [JsonPropertyName("escorts_spawned")]
    public int EscortsSpawned { get; set; }

    [JsonPropertyName("peak_alive")]
    public int PeakAlive { get; set; }
}

public sealed class CombatState
{
    [JsonPropertyName("total_damage_dealt")]
    public double TotalDamageDealt { get; set; }

    [JsonPropertyName("total_hp_spawned")]
    public double TotalHpSpawned { get; set; }

    [JsonPropertyName("total_tower_damage_taken")]
    public double TotalTowerDamageTaken { get; set; }

    [JsonPropertyName("total_healing")]
    public double TotalHealing { get; set; }

    [JsonPropertyName("cc_uptime_pct")]
    public double CcUptimePct { get; set; }

    [JsonPropertyName("difficulty_mult")]
    public double DifficultyMult { get; set; }
}

public sealed class GameEvent
{
    [JsonPropertyName("t")]
    public double T { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; } = "";

    [JsonPropertyName("kind")]
    public string? Kind { get; set; }

    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("cost")]
    public int? Cost { get; set; }

    [JsonPropertyName("enemy")]
    public string? Enemy { get; set; }

    [JsonPropertyName("leader")]
    public string? Leader { get; set; }

    [JsonPropertyName("count")]
    public int? Count { get; set; }
}
