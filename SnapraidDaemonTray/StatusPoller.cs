using Microsoft.Extensions.Hosting;

using SnapraidDaemonTray.Instances;

namespace SnapraidDaemonTray;

internal partial class StatusPoller(SystemTray systemTray, InstanceManager intanceManager) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            var timer = new PeriodicTimer(TimeSpan.FromSeconds(1));

            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                systemTray.Update();
                await intanceManager.UpdateInstances();
            }
        }
        catch (OperationCanceledException)
        {
        }
    }
}
