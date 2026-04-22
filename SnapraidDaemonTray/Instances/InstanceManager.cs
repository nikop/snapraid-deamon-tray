using SnapraidDaemonTray.Events;

using System.Collections.Concurrent;
using System.Text;

namespace SnapraidDaemonTray.Instances;

public class InstanceManager(AppConfiguration appConfiguration, NotificationsHandler notificationsHandler)
{
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

        var conf = await appConfiguration.GetCurrentConfiguration();

        foreach (var item in conf.Servers)
        {
            var key = MakeFileNameSafe(item.Name);

            if (!item.Enabled)
            {
                continue;
            }

            if (!_instances.TryGetValue(key, out var instance))
            {
                instance = new Instance(item) { Key = key };
                _instances[key] = instance;

                instance.MaintenanceStarted += Instance_MaintenanceStarted;
                instance.MaintenanceProgress += Instance_MaintenanceProgress;
                instance.MaintenanceCompleted += Instance_MaintenanceCompleted;
                instance.MaintenanceCompletedError += Instance_MaintenanceError;
            }

            await instance.Update();
        }

        _lock.Release();
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
