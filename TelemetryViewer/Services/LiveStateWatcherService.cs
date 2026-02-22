using System.IO;
using System.Text.Json;
using TelemetryViewer.Models;

namespace TelemetryViewer.Services;

public sealed class LiveStateWatcherService : IDisposable
{
    private const string WatchDirectory = @"C:\Users\sethb\TD-Game\reports";
    private const string WatchFile = "live_state.json";
    private const int MaxRetries = 3;
    private const int RetryDelayMs = 25;

    private FileSystemWatcher? _watcher;
    private readonly Timer _debounceTimer;
    private volatile bool _readPending;

    public event Action<LiveState>? StateUpdated;

    public LiveStateWatcherService()
    {
        _debounceTimer = new Timer(_ => OnDebounceElapsed(), null, Timeout.Infinite, Timeout.Infinite);
    }

    public void Start()
    {
        if (!Directory.Exists(WatchDirectory))
            Directory.CreateDirectory(WatchDirectory);

        _watcher = new FileSystemWatcher(WatchDirectory, WatchFile)
        {
            NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size,
            EnableRaisingEvents = true
        };

        _watcher.Changed += OnFileChanged;

        TryReadAndPublish();
    }

    private void OnFileChanged(object sender, FileSystemEventArgs e)
    {
        if (_readPending) return;
        _readPending = true;
        _debounceTimer.Change(50, Timeout.Infinite);
    }

    private void OnDebounceElapsed()
    {
        _readPending = false;
        TryReadAndPublish();
    }

    private void TryReadAndPublish()
    {
        var filePath = Path.Combine(WatchDirectory, WatchFile);
        if (!File.Exists(filePath)) return;

        for (int attempt = 0; attempt < MaxRetries; attempt++)
        {
            try
            {
                string json;
                using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (var sr = new StreamReader(fs))
                {
                    json = sr.ReadToEnd();
                }

                if (string.IsNullOrWhiteSpace(json)) continue;

                var state = JsonSerializer.Deserialize<LiveState>(json);
                if (state is not null)
                {
                    StateUpdated?.Invoke(state);
                    return;
                }
            }
            catch (IOException)
            {
                Thread.Sleep(RetryDelayMs * (attempt + 1));
            }
            catch (JsonException)
            {
                return;
            }
        }
    }

    public void Dispose()
    {
        _watcher?.Dispose();
        _debounceTimer.Dispose();
    }
}
