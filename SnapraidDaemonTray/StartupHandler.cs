using Microsoft.Extensions.Hosting;

using SnapraidDaemonTray.Instances;

namespace SnapraidDaemonTray;

internal partial class StartupHandler(InstanceManager instanceManager, SystemTray systemTray) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await instanceManager.ApplyCurrentConfiguration();

        var instances = instanceManager.Instances;

        if (instances.Count == 0)
        {
            // TODO: Show Setup
        }

        var isLaunchedByNotification = Environment.GetCommandLineArgs().Contains(NotificationsHandler.NotificationLaunchArgument);

        if (isLaunchedByNotification)
        {
            await systemTray.OpenTrayPopup();
        }
    }
}
