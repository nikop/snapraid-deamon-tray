using Dapplo.Microsoft.Extensions.Hosting.WinForms;

using Humanizer;

using Microsoft.Extensions.Logging;
using Microsoft.Windows.AppNotifications;
using Microsoft.Windows.AppNotifications.Builder;

using SnapraidDaemonTray.Events;
using SnapraidDaemonTray.Instances;

namespace SnapraidDaemonTray;

public class NotificationsHandler(ILogger<NotificationsHandler> logger, SystemTray systemTray) : IWinFormsService
{
#if DEBUG
    public const string AppDisplayName = "Snapraid Daemon Tray (Debug)";
#else 
    public const string AppDisplayName = "Snapraid Daemon Tray";
#endif

    public const string NotificationLaunchArgument = "----AppNotificationActivated:";

    public void Initialize()
    {
        AppNotificationManager.Default.NotificationInvoked += async (sender, e) =>
        {
            await systemTray.OpenTrayPopup();
        };
        AppNotificationManager.Default.Register(AppDisplayName, new Uri(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Icons", "tray.ico")));
    }

    private uint _currentNotificationSeqNum = 1;

    private AppNotificationProgressData BuldProgressData(MaintenanceProgressEventArgs e)
    {
        return new AppNotificationProgressData(++_currentNotificationSeqNum)
        {
            Title = e.CurrentCommand?.ToActionName(),
            Status = e switch
            {
                { Eta: var eta } when eta is not null => eta.Value.Humanize(),
                _ => ""
            },
            Value = e.Progress ?? 0,
            ValueStringOverride = e switch
            {
                { SizeTotal: not null, SizeDone: not null } => $"{e.SizeDone.Value.Humanize()} / {e.SizeTotal.Value.Humanize()}",
                { SizeDone: not null } => e.SizeDone.Value.Humanize(),
                _ => "",
            },
        };
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="instance"></param>
    public void MaintenanceStarted(Instance instance, MaintenanceStartedEventArgs e)
    {
        logger.LogTrace("Maintenance Started for {instance}", instance.Name);

        var notification = new AppNotificationBuilder()
            .SetTag($"{instance.Key}:maintenance")
            .AddText($"{instance.Name}: Maintenance in Progress")
            .AddProgressBar((new AppNotificationProgressBar())
                .BindTitle()
                .BindValue()
                .BindStatus()
                .BindValueStringOverride()
              )
            .AddButton(new AppNotificationButton
            {
                Content = "Open Dashboard",
                InvokeUri = new Uri(instance.Address),
            })
            .BuildNotification();

        notification.Progress = BuldProgressData(e);
        notification.SuppressDisplay = e.Manual;

        AppNotificationManager.Default.Show(notification);
    }

    public async Task MaintenanceProgress(Instance instance, MaintenanceProgressEventArgs e)
    {
        var res = await AppNotificationManager.Default.UpdateAsync(BuldProgressData(e), $"{instance.Key}:maintenance");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="instance"></param>
    public void MaintenanceCompleted(Instance instance, MaintenanceCompletedEventArgs e)
    {
        logger.LogTrace("Maintenance Completed for {instance}", instance.Name);

        var notification = new AppNotificationBuilder()
            .SetTag($"{instance.Key}:maintenance_complete")
            .AddText("Maintenance Completed!")
            .AddButton(new AppNotificationButton
            {
                Content = "Open Dashboard",
                InvokeUri = new Uri(instance.Address),
            })
            .BuildNotification();

        AppNotificationManager.Default.Show(notification);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="instance"></param>
    public void MaintenanceError(Instance instance, MaintenanceErrorEventArgs e)
    {
        logger.LogTrace("Maintenance Error for {instance}", instance.Name);

        var notification = new AppNotificationBuilder()
            .SetTag($"{instance.Key}:maintenance_error")
            .AddText("Maintenance Error")
            .AddText(e.Error, new AppNotificationTextProperties { MaxLines = 4 })
            .AddButton(new AppNotificationButton
            {
                Content = "Open Dashboard",
                InvokeUri = new Uri(instance.Address),
            })
            .BuildNotification();

        AppNotificationManager.Default.Show(notification);
    }
}
