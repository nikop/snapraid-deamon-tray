using Microsoft.Extensions.Hosting;

using SnapraidDaemonTray.Instances;

namespace SnapraidDaemonTray;

internal partial class StartupHandler(InstanceManager instanceManager, SystemTray systemTray) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await instanceManager.ApplyCurrentConfiguration();
        systemTray.Initialize();

        var instances = instanceManager.Instances;

        if (instances.Count == 0)
        {
            MessageBox.Show("No Instances Configured!");
            await systemTray.OpenSettings();
        }

        var isLaunchedByNotification = Environment.GetCommandLineArgs().Contains(NotificationsHandler.NotificationLaunchArgument);

        if (isLaunchedByNotification)
        {
            await systemTray.OpenTrayPopup();
        }
    }
}
