using SnapraidDaemonTray.Config;

using System.Text.Json;

namespace SnapraidDaemonTray;

public class AppConfiguration
{
    private static readonly JsonSerializerOptions jsonSerializerOptions = new()
    {
        WriteIndented = true,
    };

    DirectoryInfo ConfigDirectory { get; }

    FileInfo ConfigFile { get; }

    ConfigFile? ActiveConfig { get; set; }

    private readonly SemaphoreSlim _lock = new(1, 1);

    public event EventHandler<ConfigChangedEventArgs>? ConfigurationChanged;

    public sealed class ConfigChangedEventArgs(ConfigFile config) : EventArgs
    {
        public ConfigFile Config { get; } = config;
    }

    public AppConfiguration()
    {
        var configPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "SnapraidDeamonTray");
        ConfigDirectory = new DirectoryInfo(configPath);
        ConfigFile = new FileInfo(Path.Combine(configPath, "config.json"));

        if (!ConfigDirectory.Exists)
        {
            ConfigDirectory.Create();
        }
    }

    private async Task<ConfigFile> ReadConfig()
    {
        ConfigFile? appConfig = null;

        if (ConfigFile.Exists)
        {
            var config = await File.ReadAllTextAsync(ConfigFile.FullName);
            appConfig = JsonSerializer.Deserialize<ConfigFile>(config);

            if (appConfig is not null)
                return appConfig;
        }

        appConfig = new ConfigFile
        {
            Servers = [],
        };

        var json = JsonSerializer.Serialize(appConfig, jsonSerializerOptions);

        await File.WriteAllTextAsync(ConfigFile.FullName, json);

        return appConfig;
    }

    public async Task<ConfigFile> GetCurrentConfiguration()
    {
        await _lock.WaitAsync();

        var shouldRaise = false;

        if (ActiveConfig is null)
        {
            ActiveConfig = await ReadConfig();
            shouldRaise = true;
        }

        var config = ActiveConfig!;

        _lock.Release();

        if (shouldRaise)
            OnConfigurationChanged(config);

        return config;
    }

    public async Task SaveConfig(ConfigFile config)
    {
        await _lock.WaitAsync();

        var json = JsonSerializer.Serialize(config, jsonSerializerOptions);

        await File.WriteAllTextAsync(ConfigFile.FullName, json);

        ActiveConfig = config;

        _lock.Release();

        OnConfigurationChanged(config);
    }

    protected virtual void OnConfigurationChanged(ConfigFile config)
    {
        try
        {
            ConfigurationChanged?.Invoke(this, new ConfigChangedEventArgs(config));
        }
        catch
        {
            // Swallow exceptions from handlers to avoid crashing caller
        }
    }
}
