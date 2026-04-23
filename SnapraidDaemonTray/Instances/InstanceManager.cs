using SnapraidDaemonTray.Events;

using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnapraidDaemonTray.Instances;

public class InstanceManager
{
    private readonly AppConfiguration appConfiguration;
    private readonly NotificationsHandler notificationsHandler;

    public InstanceManager(AppConfiguration appConfiguration, NotificationsHandler notificationsHandler)
    {
        this.appConfiguration = appConfiguration;
        this.notificationsHandler = notificationsHandler;

        this.appConfiguration.ConfigurationChanged += AppConfiguration_ConfigurationChanged;
    }

    private readonly ConcurrentDictionary<string, Instance> _instances = [];

    private readonly SemaphoreSlim _lock = new(1, 1);

    private static string MakeFileNameSafe(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return "unknown";

        var invalid = new HashSet<char>(Path.GetInvalidFileNameChars());
        var sb = new StringBuilder(name.Length);
        foreach (var ch in name)
        {
            sb.Append(invalid.Contains(ch) ? '_' : ch);
        }

        var result = sb.ToString().Trim();
        return string.IsNullOrEmpty(result) ? "unknown" : result;
    }

    private async Task UpdateInstancesInternal()
    {
        await _lock.WaitAsync();
        try
        {
            var conf = await appConfiguration.GetCurrentConfiguration();

            // Build set of desired instance keys from enabled servers in config
            var desiredKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var item in conf.Servers)
            {
                if (!item.Enabled)
                    continue;

                var key = MakeFileNameSafe(item.Name);
                desiredKeys.Add(key);

                if (!_instances.TryGetValue(key, out var instance))
                {
                    instance = new Instance(item) { Key = key };
                    // Try to add; another thread might have added concurrently
                    if (!_instances.TryAdd(key, instance))
                        continue;

                    instance.MaintenanceStarted += Instance_MaintenanceStarted;
                    instance.MaintenanceProgress += Instance_MaintenanceProgress;
                    instance.MaintenanceCompleted += Instance_MaintenanceCompleted;
                    instance.MaintenanceCompletedError += Instance_MaintenanceError;
                }

                await instance.Update();
            }

            // Remove instances that are not present/enabled in the new configuration
            foreach (var existingKey in _instances.Keys.ToList())
            {
                if (!desiredKeys.Contains(existingKey))
                {
                    if (_instances.TryRemove(existingKey, out var removed))
                    {
                        // Unsubscribe events to avoid potential memory leaks
                        removed.MaintenanceStarted -= Instance_MaintenanceStarted;
                        removed.MaintenanceProgress -= Instance_MaintenanceProgress;
                        removed.MaintenanceCompleted -= Instance_MaintenanceCompleted;
                        removed.MaintenanceCompletedError -= Instance_MaintenanceError;
                    }
                }
            }
        }
        finally
        {
            _lock.Release();
        }
    }

    private void AppConfiguration_ConfigurationChanged(object? sender, AppConfiguration.ConfigChangedEventArgs e)
    {
        // Run update in background; swallow exceptions to avoid crashing event thread
        _ = Task.Run(async () =>
        {
            try
            {
                await UpdateInstancesInternal();
            }
            catch
            {
                // ignore
            }
        });
    }

    private void Instance_MaintenanceStarted(object? sender, MaintenanceStartedEventArgs e)
    {
        if (sender is not Instance instance)
        {
            return;
        }

        notificationsHandler.MaintenanceStarted(instance, e);
    }

    private void Instance_MaintenanceProgress(object? sender, MaintenanceProgressEventArgs e)
    {
        if (sender is not Instance instance)
        {
            return;
        }

        _ = notificationsHandler.MaintenanceProgress(instance, e);
    }

    private void Instance_MaintenanceCompleted(object? sender, MaintenanceCompletedEventArgs e)
    {
        if (sender is not Instance instance)
        {
            return;
        }

        notificationsHandler.MaintenanceCompleted(instance, e);
    }

    private void Instance_MaintenanceError(object? sender, MaintenanceErrorEventArgs e)
    {
        if (sender is not Instance instance)
        {
            return;
        }

        notificationsHandler.MaintenanceError(instance, e);
    }

    public Task UpdateInstances() => UpdateInstancesInternal(); // TODO

    public async Task<List<Instance>> GetAll(bool update = false)
    {
        if (update)
        {
            await UpdateInstancesInternal();
        }

        return [.. _instances.Values];
    }
}
