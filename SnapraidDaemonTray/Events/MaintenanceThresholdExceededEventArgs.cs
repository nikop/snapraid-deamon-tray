namespace SnapraidDaemonTray.Events;

public class MaintenanceThresholdExceededEventArgs : EventArgs
{
    public required string Message { get; set; }
}
