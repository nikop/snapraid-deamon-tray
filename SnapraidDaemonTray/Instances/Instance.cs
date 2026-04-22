using Humanizer;

using Microsoft.Kiota.Abstractions.Authentication;
using Microsoft.Kiota.Http.HttpClientLibrary;

using SnapraidDaemonTray.Config;
using SnapraidDaemonTray.Events;
using SnapraidDaemonTray.Models;

using SnapraidDeamonApi;
using SnapraidDeamonApi.Models;
using SnapraidDeamonApi.Snapraid.V1.Activity;

using System.Text;

namespace SnapraidDaemonTray.Instances;

public class Instance
{
    private ConfigFileServer Config { get; }

    private readonly SnapraidDaemonClient _client;

    public DateTimeOffset? NextCheck { get; internal set; }

    private Pulse? Pulse { get; set; }

    public required string Key { get; init; }

    public string Name => Config.Name;

    public string Address => Config.Address;

    public InstanceStatusInfo Status { get; }

    private long _blockSize = 256*1024;

    private ActivityGetResponse? ActivityResponse { get; set; }

    public event EventHandler<MaintenanceStartedEventArgs>? MaintenanceStarted;

    public event EventHandler<MaintenanceProgressEventArgs>? MaintenanceProgress;

    public event EventHandler<MaintenanceCompletedEventArgs>? MaintenanceCompleted;

    public event EventHandler<MaintenanceErrorEventArgs>? MaintenanceCompletedError;

    private bool _manualMaintenanceRequested;

    public Instance(ConfigFileServer serverConfig)
    {
        Config = serverConfig;

        var adapter = new HttpClientRequestAdapter(authenticationProvider: new AnonymousAuthenticationProvider())
        {
            BaseUrl = serverConfig.Address
        };

        _client = new SnapraidDaemonClient(adapter);
        Status = new InstanceStatusInfo
        { 
            Name = serverConfig.Name
        };
    }

    public async Task StartMaintenance()
    {
        _manualMaintenanceRequested = true;
        var response = await _client.Snapraid.V1.Maintenance.PostAsMaintenancePostResponseAsync(new CommandRequest
        {
            IgnoreThresholds = false
        });
        await UpdateActivity(force: true);
    }

    private async Task<bool> UpdateRequired() => (await Task.WhenAll(UpdateActivity(), UpdateArray(), UpdateDisks())).Any(x => true);

    private async Task UpdateState(bool force = false)
    {
        try
        {
            //
            var response = await _client.Snapraid.V1.State.GetAsStateGetResponseAsync();
            if (response?.Pulse is not null)
            {
                Pulse = response.Pulse;
            }
        }
        catch (Exception ex)
        {
            Status.Health = HealthStatus.NoConnection;
        }
    }

    private async Task HandleMaintenance(ActivityGetResponse? previous, ActivityGetResponse? current)
    {
        var startedAt = current?.StartedAt is not null ? DateTime.Parse(current.StartedAt) : DateTime.MinValue;

        var progress = current switch
        {
            { Progress: var val } when val is not null => val / 100.0,
            _ => null,
        };

        ByteSize? sizeDone = current?.SizeDoneBytes?.Bytes();
        ByteSize? sizeTotal = current switch
        {
            { BlocksCount: var val, Command: Task_command.Sync } when val is not null => (val.Value * _blockSize).Bytes(),
            _ => null,
        };
        TimeSpan? eta = current?.EtaSeconds?.Seconds();

        // Update Tasks after task has ended
        if (current?.Status is Task_status.Terminated || (current is null && previous is not null))
        {
            await UpdateTasks(force: true);
            current = null;
        }

        // Started
        if (previous is null && current is not null)
        {
            MaintenanceStarted?.Invoke(this, new MaintenanceStartedEventArgs
            {
                Manual = _manualMaintenanceRequested,
                CurrentCommand = current.Command,
                Eta = eta,
                Progress = 0,
                SizeDone = sizeDone,
                SizeTotal = sizeTotal,
            });
            _manualMaintenanceRequested = false;
        }
        else if (current is not null)
        {
            MaintenanceProgress?.Invoke(this, new MaintenanceProgressEventArgs
            {
                CurrentCommand = current.Command,
                Progress = progress,
                Eta = eta,
                SizeDone = sizeDone,
                SizeTotal = sizeTotal,
            });
        }
    }

    private async Task<bool> UpdateActivity(bool force = false)
    {
        if (!force && ActivityResponse is not null && Pulse?.Activity == ActivityResponse?.Pulse?.Activity)
        {
            return false;
        }

        try
        {
            var previousResponse = ActivityResponse; 
            var response = await _client.Snapraid.V1.Activity.GetAsActivityGetResponseAsync();
            if (response?.Pulse is not null)
            {
                Pulse = response.Pulse;
            }

            if (response is null)
            {
                return false;
            }

            ActivityResponse = response;

            if (previousResponse?.HighCommand is Task_high_command.Maintenance || response.HighCommand is Task_high_command.Maintenance)
            {
                await HandleMaintenance(
                    previousResponse.NullIfNotHighCommand(Task_high_command.Maintenance),
                    response.NullIfNotHighCommand(Task_high_command.Maintenance)
                );
            }

            if (response.Command is Task_command.Probe or Task_command.Down_idle || response.Status is Task_status.Terminated)
            {
                Status.CurrentHighCommand = null;
                Status.CurrentCommand = null;
                Status.CurrentCommandProgress = null;
            }
            else
            {
                Status.CurrentHighCommand = response.HighCommand;
                Status.CurrentCommand = response.Command;
                Status.CurrentCommandProgress = response.Progress ?? 0;
            }
        }
        catch (Exception ex)
        {
        }

        return true;
    }

    private long _arayPulse = -1;

    public async Task<bool> UpdateArray(bool force = false)
    {
        if (!force && Pulse?.Array == _arayPulse)
        {
            return false;
        }

        try
        {
            var response = await _client.Snapraid.V1.Array.GetAsArrayGetResponseAsync();
            if (response?.Pulse is not null)
            {
                Pulse = response.Pulse;
            }

            if (response is null)
            {
                return false;
            }

            _arayPulse = response.Pulse?.Array ?? 0;

            if (response.BlockSizeBytes is not null)
            {
                _blockSize = response.BlockSizeBytes.Value;
            }

            var bytesFree = response.FreeSpaceBytes ?? 0;
            var bytesTotal = response.TotalSpaceBytes ?? 1;

            Status.ArrayUsage = ((bytesTotal - bytesFree) / (double)bytesTotal) * 100;
            Status.ArrayBadBlocks = response.BlocksBad ?? 0;
            Status.ArrayUnsyncedBlocks = response.BlocksUnsynced ?? 0;

            Status.Health = response switch
            {
                { Health: Array_health.Corrupt or Array_health.Failing } => HealthStatus.Error,
                { BlocksBad: > 0 } => HealthStatus.Error,
                { BlocksUnsynced: > 0 } => HealthStatus.Warning,
                { Health: Array_health.Prefail  } => HealthStatus.Warning,
                { Health: Array_health.Pending } => HealthStatus.NoConnection,
                { Health: Array_health.Passed } => HealthStatus.Healthy,
                _ => HealthStatus.Error,
            };
        }
        catch (Exception ex)
        {
            Status.Health = HealthStatus.NoConnection;
        }

        return true;
    }

    private long _disksPulse = -1;

    public async Task<bool> UpdateDisks(bool force = false)
    {
        if (!force && Pulse?.Disks == _disksPulse)
        {
            return false;
        }

        try
        {
            var response = await _client.Snapraid.V1.Disks.GetAsDisksGetResponseAsync();
            if (response is null)
            {
                return false;
            }

            if (response.Pulse is not null)
            {
                Pulse = response.Pulse;
            }

            _disksPulse = response.Pulse?.Disks ?? 0;
        }
        catch (Exception ex)
        {
        }

        return true;
    }

    private int _previousTaskNumber = -1;

    private async Task<bool> UpdateTasks(bool force = false)
    {
        if (!force && Pulse?.Tasks == _previousTaskNumber)
        {
            return false;
        }

        try
        {
            var response = await _client.Snapraid.V1.Tasks.GetAsTasksGetResponseAsync();

            if (response is null)
            {
                return false;
            }

            if (response.Pulse is not null)
            {
                Pulse = response.Pulse;
            }

            var newTasks = response.History?.Where(x => x.Number > _previousTaskNumber).ToList() ?? [];

            _previousTaskNumber = response.History?.Max(x => x.Number) ?? 0;

            var latestSync = newTasks
                .Where(x => x.Command is Task_command.Sync)
                .OrderByDescending(x => x.Number)
                .FirstOrDefault();

            if (latestSync is not null)
            {
                var latestReport = newTasks
                    .Where(x => x.Command is Task_command.Report && latestSync.Number < x.Number)
                    .OrderByDescending(x => x.Number)
                    .FirstOrDefault();

                var latestScrub = newTasks
                    .Where(x => x.Command is Task_command.Scrub && latestSync.Number < x.Number)
                    .OrderByDescending(x => x.Number)
                    .FirstOrDefault();

                if (latestSync.ExitCode is 1)
                {
                    var sb = new StringBuilder();

                    if (latestSync.ErrorIo > 0)
                    {
                        sb.AppendLine($"{latestSync.ErrorIo} io errors");
                    }

                    if (latestSync.ErrorData > 0)
                    {
                        sb.AppendLine($"{latestSync.ErrorData} data errors");
                    }

                    if (latestSync.ErrorSoft > 0)
                    {
                        sb.AppendLine($"{latestSync.ErrorSoft} soft errors");
                    }

                    // Error
                    MaintenanceCompletedError?.Invoke(this, new MaintenanceErrorEventArgs {
                        Error = sb.ToString(),
                    });
                }
                else if (latestSync.ExitCode is 2)
                {
                    // TODO: Notification for threshold exceeded
                }
                else
                {
                    //
                }

            }
        }
        catch (Exception ex)
        {
        }

        return true;
    }

    public async Task Update(bool force = false)
    {
        if (!force && NextCheck is not null && NextCheck >= DateTimeOffset.Now)
        {
            return;
        }

        if (_arayPulse <= 0)
        {
            await UpdateArray();
        }
        else if (Status.CurrentCommand is not null)
        {
            await UpdateActivity(force: true);
        }
        else
        {
            await UpdateState();
        }

        await UpdateRequired();

        if (Pulse is null)
        {
            // TODO: Better Error Handling
            NextCheck = DateTimeOffset.Now.AddSeconds(30);
            return;
        }

        var pollPeriod = ActivityResponse switch
        {
            { Status: Task_status.Finalizing or Task_status.Processing or Task_status.Stopping } => TimeSpan.FromSeconds(1),
            _ => TimeSpan.FromSeconds(3),
        };

        NextCheck = DateTimeOffset.Now.Add(pollPeriod);
    }
}
