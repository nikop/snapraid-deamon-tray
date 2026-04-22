using Microsoft.Extensions.Hosting;

using SnapraidDaemonTray.Instances;

namespace SnapraidDaemonTray;

internal partial class StatusPoller(SystemTray systemTray, InstanceManager instanceManager) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            var timer = new PeriodicTimer(TimeSpan.FromSeconds(1));

            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                systemTray.Update();
                await instanceManager.UpdateInstances();
            }
        }
        catch (OperationCanceledException)
        {
        }


    }
}
