namespace SnapraidDaemonTray.Events;

public class MaintenanceErrorEventArgs : EventArgs
{
    public required string Error { get; set; }
}
