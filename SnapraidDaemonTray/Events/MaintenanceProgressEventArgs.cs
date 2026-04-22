using Humanizer;

using SnapraidDeamonApi.Models;

namespace SnapraidDaemonTray.Events;

public class MaintenanceProgressEventArgs : EventArgs
{
    public Task_command? CurrentCommand { get; init; }

    public TimeSpan? Eta { get; init; }

    public double? Progress { get; init; }

    public ByteSize? SizeTotal { get; init; }

    public ByteSize? SizeDone { get; init; }
}
