using Microsoft.Extensions.Hosting;

using SnapraidDaemonTray.Instances;

namespace SnapraidDaemonTray;

internal partial class StartupHandler(InstanceManager instanceManager) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var servers = await instanceManager.GetAll(true);

        if (servers.Count == 0)
        {
            // TODO: Show Setup
        }

        var isLaunchedByNotification = Environment.GetCommandLineArgs().Contains(NotificationsHandler.NotificationLaunchArgument);

        if (isLaunchedByNotification)
        {
            // TODO: Start with UI visible
        }
    }
}
