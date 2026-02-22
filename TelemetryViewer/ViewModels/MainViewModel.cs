using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using TelemetryViewer.Models;
using TelemetryViewer.Services;

namespace TelemetryViewer.ViewModels;

public sealed class MainViewModel : INotifyPropertyChanged, IDisposable
{
    private readonly ReferenceDataService _refData;
    private readonly LiveStateWatcherService _watcher;

    public MainViewModel(string gamePath)
    {
        _refData = new ReferenceDataService(gamePath);
        _refData.Load();

        _watcher = new LiveStateWatcherService(gamePath);
        _watcher.StateUpdated += OnStateUpdated;
        _watcher.Start();
    }

    private void OnStateUpdated(LiveState state)
    {
        if (Application.Current?.Dispatcher is null) return;
        Application.Current.Dispatcher.Invoke(() => ApplyState(state));
    }

    private void ApplyState(LiveState s)
    {
        Status = s.Status;
        Trial = s.Trial;
        TotalTrials = s.TotalTrials;
        ElapsedS = s.ElapsedS;
        DurationS = s.DurationS;
        BuildProfile = s.BuildProfile?.ToUpperInvariant() ?? "";
        DifficultyScalar = s.DifficultyScalar;
        Seed = s.Seed;

        ElapsedDisplay = FormatTime(s.ElapsedS);
        DurationDisplay = FormatTime(s.DurationS);
        TimeProgress = s.DurationS > 0 ? s.ElapsedS / s.DurationS * 100 : 0;
        TrialDisplay = $"Trial {s.Trial} / {s.TotalTrials}";

        switch (s.Status)
        {
            case "starting":
                ApplyStarting(s);
                break;
            case "running":
                ApplyRunning(s);
                break;
            case "survived":
            case "dead":
                ApplyRunning(s);
                ApplyEndState(s);
                break;
            case "idle":
                ApplyIdle(s);
                break;
        }

        IsRunning = s.Status == "running";
        IsStarting = s.Status == "starting";
        IsEnded = s.Status is "survived" or "dead";
        IsIdle = s.Status == "idle";
        ShowMainPanels = s.Status is "running" or "survived" or "dead";
        ShowBanner = s.Status is "starting" or "survived" or "dead" or "idle";
    }

    private void ApplyStarting(LiveState s)
    {
        TowerHp = 0; TowerMaxHp = 0; TowerHpPct = 0; TowerArmor = 0; TowerRegen = 0;
        Gold = 0; TotalEarned = 0; TotalSpent = 0;
        DifficultyMult = 0;
        Loadout.Clear();
        UpgradesList.Clear();
        EventLog.Clear();
        KillLog.Clear();
        EnemyBreakdown.Clear();
        HpTimelinePoints = new PointCollection();
        EnemiesAlive = 0; PeakAlive = 0; TotalSpawned = 0; TotalKilled = 0;
        TotalLeaked = 0; ElitesSpawned = 0; EscortsSpawned = 0;
        DamageDealt = 0; HpSpawned = 0; TowerDamageTaken = 0;
        Healing = 0; CcUptime = 0;
        BannerText = $"STARTING — TRIAL {s.Trial}/{s.TotalTrials}";
        BannerBrush = new SolidColorBrush(Color.FromRgb(100, 160, 255));
    }

    private void ApplyRunning(LiveState s)
    {
        if (s.Tower is not null)
        {
            TowerHp = s.Tower.Hp;
            TowerMaxHp = s.Tower.MaxHp;
            TowerHpPct = s.Tower.HpPct * 100;
            TowerArmor = s.Tower.Armor;
            TowerRegen = s.Tower.RegenPerS;
            HpBarBrush = s.Tower.HpPct switch
            {
                > 0.6 => new SolidColorBrush(Color.FromRgb(0, 200, 120)),
                > 0.3 => new SolidColorBrush(Color.FromRgb(230, 180, 30)),
                _ => new SolidColorBrush(Color.FromRgb(220, 50, 50))
            };
        }

        if (s.Economy is not null)
        {
            Gold = s.Economy.Gold;
            TotalEarned = s.Economy.TotalEarned;
            TotalSpent = s.Economy.TotalSpent;
        }

        if (s.Combat is not null)
        {
            DamageDealt = s.Combat.TotalDamageDealt;
            HpSpawned = s.Combat.TotalHpSpawned;
            TowerDamageTaken = s.Combat.TotalTowerDamageTaken;
            Healing = s.Combat.TotalHealing;
            CcUptime = s.Combat.CcUptimePct * 100;
            DifficultyMult = s.Combat.DifficultyMult;
        }

        if (s.Enemies is not null)
        {
            EnemiesAlive = s.Enemies.Alive;
            PeakAlive = s.Enemies.PeakAlive;
            TotalSpawned = s.Enemies.TotalSpawned;
            TotalKilled = s.Enemies.TotalKilled;
            TotalLeaked = s.Enemies.TotalLeaked;
            ElitesSpawned = s.Enemies.ElitesSpawned;
            EscortsSpawned = s.Enemies.EscortsSpawned;

            EnemyBreakdown.Clear();
            if (s.Enemies.Breakdown is not null)
            {
                foreach (var (id, count) in s.Enemies.Breakdown.OrderByDescending(kv => kv.Value))
                    EnemyBreakdown.Add(new DisplayEntry(_refData.GetEnemyName(id), count.ToString("N0")));
            }
        }

        if (s.Loadout is not null)
        {
            Loadout.Clear();
            foreach (var w in s.Loadout)
            {
                Loadout.Add(new LoadoutEntry(
                    w.Name,
                    $"x{w.Stacks}",
                    $"{w.Dps:N0} DPS",
                    $"R:{w.Range:N0}",
                    $"T{w.Tier}"));
            }
        }

        if (s.Upgrades is not null)
        {
            UpgradesList.Clear();
            foreach (var u in s.Upgrades.OrderByDescending(x => x.Count))
            {
                UpgradesList.Add(new DisplayEntry(
                    _refData.GetUpgradeName(u.Id),
                    $"x{u.Count}"));
            }
        }

        if (s.KillsByType is not null)
        {
            KillLog.Clear();
            foreach (var (id, kills) in s.KillsByType.OrderByDescending(kv => kv.Value))
                KillLog.Add(new DisplayEntry(_refData.GetEnemyName(id), kills.ToString("N0")));
        }

        if (s.RecentEvents is not null)
        {
            EventLog.Clear();
            foreach (var evt in s.RecentEvents.OrderByDescending(e => e.T))
                EventLog.Add(FormatEvent(evt));
        }

        if (s.HpTimeline is not null && s.HpTimeline.Count > 1)
            HpTimelinePoints = BuildSparkline(s.HpTimeline, s.Tower?.MaxHp ?? 1000);
    }

    private void ApplyEndState(LiveState s)
    {
        if (s.Status == "survived")
        {
            BannerText = "SURVIVED";
            BannerBrush = new SolidColorBrush(Color.FromRgb(0, 200, 120));
        }
        else
        {
            BannerText = "TOWER DESTROYED";
            BannerBrush = new SolidColorBrush(Color.FromRgb(220, 50, 50));
        }
    }

    private void ApplyIdle(LiveState s)
    {
        ShowMainPanels = false;
        BannerText = $"ALL TRIALS COMPLETE — {s.Survived}/{s.TrialsCompleted} SURVIVED";
        BannerBrush = new SolidColorBrush(Color.FromRgb(100, 160, 255));
    }

    private EventEntry FormatEvent(GameEvent evt)
    {
        var time = FormatTime(evt.T);
        return evt.Type switch
        {
            "purchase" => new EventEntry(time, "PURCHASE",
                $"{evt.Name} ({evt.Kind}) — {evt.Cost:N0}g",
                new SolidColorBrush(Color.FromRgb(80, 220, 120))),

            "elite_spawn" => new EventEntry(time, "ELITE",
                _refData.GetEnemyName(evt.Enemy ?? ""),
                new SolidColorBrush(Color.FromRgb(220, 60, 60))),

            "escort_spawn" => new EventEntry(time, "ESCORT",
                $"{_refData.GetEnemyName(evt.Leader ?? "")} (+{evt.Count} scouts)",
                new SolidColorBrush(Color.FromRgb(230, 200, 50))),

            _ => new EventEntry(time, evt.Type.ToUpperInvariant(), "",
                new SolidColorBrush(Color.FromRgb(180, 180, 180)))
        };
    }

    private static PointCollection BuildSparkline(List<double> data, double maxHp)
    {
        const double canvasWidth = 600;
        const double canvasHeight = 50;
        var pts = new PointCollection();
        if (data.Count < 2) return pts;

        double xStep = canvasWidth / (data.Count - 1);
        for (int i = 0; i < data.Count; i++)
        {
            double x = i * xStep;
            double y = canvasHeight - (data[i] / maxHp * canvasHeight);
            pts.Add(new Point(x, Math.Max(0, Math.Min(canvasHeight, y))));
        }
        return pts;
    }

    private static string FormatTime(double seconds)
    {
        int totalSec = (int)seconds;
        return $"{totalSec / 60:D2}:{totalSec % 60:D2}";
    }

    // --- Observable Properties ---

    private string _status = "idle";
    public string Status { get => _status; set => Set(ref _status, value); }

    private int _trial;
    public int Trial { get => _trial; set => Set(ref _trial, value); }

    private int _totalTrials;
    public int TotalTrials { get => _totalTrials; set => Set(ref _totalTrials, value); }

    private double _elapsedS;
    public double ElapsedS { get => _elapsedS; set => Set(ref _elapsedS, value); }

    private double _durationS;
    public double DurationS { get => _durationS; set => Set(ref _durationS, value); }

    private string _buildProfile = "";
    public string BuildProfile { get => _buildProfile; set => Set(ref _buildProfile, value); }

    private double _difficultyScalar;
    public double DifficultyScalar { get => _difficultyScalar; set => Set(ref _difficultyScalar, value); }

    private int _seed;
    public int Seed { get => _seed; set => Set(ref _seed, value); }

    private string _elapsedDisplay = "00:00";
    public string ElapsedDisplay { get => _elapsedDisplay; set => Set(ref _elapsedDisplay, value); }

    private string _durationDisplay = "00:00";
    public string DurationDisplay { get => _durationDisplay; set => Set(ref _durationDisplay, value); }

    private double _timeProgress;
    public double TimeProgress { get => _timeProgress; set => Set(ref _timeProgress, value); }

    private string _trialDisplay = "";
    public string TrialDisplay { get => _trialDisplay; set => Set(ref _trialDisplay, value); }

    // Tower
    private double _towerHp;
    public double TowerHp { get => _towerHp; set => Set(ref _towerHp, value); }

    private double _towerMaxHp;
    public double TowerMaxHp { get => _towerMaxHp; set => Set(ref _towerMaxHp, value); }

    private double _towerHpPct;
    public double TowerHpPct { get => _towerHpPct; set => Set(ref _towerHpPct, value); }

    private double _towerArmor;
    public double TowerArmor { get => _towerArmor; set => Set(ref _towerArmor, value); }

    private double _towerRegen;
    public double TowerRegen { get => _towerRegen; set => Set(ref _towerRegen, value); }

    private SolidColorBrush _hpBarBrush = new(Color.FromRgb(0, 200, 120));
    public SolidColorBrush HpBarBrush { get => _hpBarBrush; set => Set(ref _hpBarBrush, value); }

    // Economy
    private double _gold;
    public double Gold { get => _gold; set => Set(ref _gold, value); }

    private double _totalEarned;
    public double TotalEarned { get => _totalEarned; set => Set(ref _totalEarned, value); }

    private double _totalSpent;
    public double TotalSpent { get => _totalSpent; set => Set(ref _totalSpent, value); }

    // Combat
    private double _damageDealt;
    public double DamageDealt { get => _damageDealt; set => Set(ref _damageDealt, value); }

    private double _hpSpawned;
    public double HpSpawned { get => _hpSpawned; set => Set(ref _hpSpawned, value); }

    private double _towerDamageTaken;
    public double TowerDamageTaken { get => _towerDamageTaken; set => Set(ref _towerDamageTaken, value); }

    private double _healing;
    public double Healing { get => _healing; set => Set(ref _healing, value); }

    private double _ccUptime;
    public double CcUptime { get => _ccUptime; set => Set(ref _ccUptime, value); }

    private double _difficultyMult;
    public double DifficultyMult { get => _difficultyMult; set => Set(ref _difficultyMult, value); }

    // Enemies
    private int _enemiesAlive;
    public int EnemiesAlive { get => _enemiesAlive; set => Set(ref _enemiesAlive, value); }

    private int _peakAlive;
    public int PeakAlive { get => _peakAlive; set => Set(ref _peakAlive, value); }

    private int _totalSpawned;
    public int TotalSpawned { get => _totalSpawned; set => Set(ref _totalSpawned, value); }

    private int _totalKilled;
    public int TotalKilled { get => _totalKilled; set => Set(ref _totalKilled, value); }

    private int _totalLeaked;
    public int TotalLeaked { get => _totalLeaked; set => Set(ref _totalLeaked, value); }

    private int _elitesSpawned;
    public int ElitesSpawned { get => _elitesSpawned; set => Set(ref _elitesSpawned, value); }

    private int _escortsSpawned;
    public int EscortsSpawned { get => _escortsSpawned; set => Set(ref _escortsSpawned, value); }

    // Banner
    private string _bannerText = "WAITING FOR SIMULATION…";
    public string BannerText { get => _bannerText; set => Set(ref _bannerText, value); }

    private SolidColorBrush _bannerBrush = new(Color.FromRgb(100, 160, 255));
    public SolidColorBrush BannerBrush { get => _bannerBrush; set => Set(ref _bannerBrush, value); }

    // Visibility flags
    private bool _isRunning;
    public bool IsRunning { get => _isRunning; set => Set(ref _isRunning, value); }

    private bool _isStarting = true;
    public bool IsStarting { get => _isStarting; set => Set(ref _isStarting, value); }

    private bool _isEnded;
    public bool IsEnded { get => _isEnded; set => Set(ref _isEnded, value); }

    private bool _isIdle;
    public bool IsIdle { get => _isIdle; set => Set(ref _isIdle, value); }

    private bool _showMainPanels;
    public bool ShowMainPanels { get => _showMainPanels; set => Set(ref _showMainPanels, value); }

    private bool _showBanner = true;
    public bool ShowBanner { get => _showBanner; set => Set(ref _showBanner, value); }

    // HP Timeline
    private PointCollection _hpTimelinePoints = new();
    public PointCollection HpTimelinePoints { get => _hpTimelinePoints; set => Set(ref _hpTimelinePoints, value); }

    // Collections
    public ObservableCollection<LoadoutEntry> Loadout { get; } = new();
    public ObservableCollection<DisplayEntry> UpgradesList { get; } = new();
    public ObservableCollection<DisplayEntry> KillLog { get; } = new();
    public ObservableCollection<DisplayEntry> EnemyBreakdown { get; } = new();
    public ObservableCollection<EventEntry> EventLog { get; } = new();

    // --- INotifyPropertyChanged ---

    public event PropertyChangedEventHandler? PropertyChanged;

    private void Set<T>(ref T field, T value, [CallerMemberName] string? prop = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return;
        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }

    public void Dispose() => _watcher.Dispose();
}

// Display helper records

public record LoadoutEntry(string Name, string Stacks, string Dps, string Range, string Tier);
public record DisplayEntry(string Label, string Value);
public record EventEntry(string Time, string Type, string Detail, SolidColorBrush Color);
