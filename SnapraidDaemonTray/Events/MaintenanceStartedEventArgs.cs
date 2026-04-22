namespace SnapraidDaemonTray.Events;

public class MaintenanceStartedEventArgs : MaintenanceProgressEventArgs
{
    public required bool Manual { get; init; }
}
