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
            Servers = [
                new ConfigFileServer
                {
                    Enabled = true,
                    Name = "Default",
                    Address = "http://127.0.0.1:7627/",
                }
            ]
        };

        var json = JsonSerializer.Serialize(appConfig, jsonSerializerOptions);

        await File.WriteAllTextAsync(ConfigFile.FullName, json);

        return appConfig;
    }

    public async Task<ConfigFile> GetCurrentConfiguration()
    {
        await _lock.WaitAsync();

        ActiveConfig ??= await ReadConfig();

        _lock.Release();

        return ActiveConfig;
    }
}
