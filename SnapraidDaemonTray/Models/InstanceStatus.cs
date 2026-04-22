using SnapraidDeamonApi.Models;

using System.ComponentModel;

namespace SnapraidDaemonTray.Models;

public partial class InstanceStatusInfo : INotifyPropertyChanged
{
    public string Name
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged(nameof(Name));
        }
    } = string.Empty;

    public Task_high_command? CurrentHighCommand
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged(nameof(CurrentHighCommand));
        }
    }

    public Task_command? CurrentCommand
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged(nameof(CurrentCommand));
        }
    }

    public int? CurrentCommandProgress
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged(nameof(CurrentCommandProgress));
        }
    }

    public HealthStatus Health
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged(nameof(Health));
        }
    } = HealthStatus.NoConnection;

    public double ArrayUsage
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged(nameof(ArrayUsage));
        }
    }

    public long ArrayBadBlocks
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged(nameof(ArrayBadBlocks));
        }
    }

    public long ArrayUnsyncedBlocks
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged(nameof(ArrayUnsyncedBlocks));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
