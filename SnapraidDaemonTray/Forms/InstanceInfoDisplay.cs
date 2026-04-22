using SnapraidDaemonTray.Instances;
using SnapraidDaemonTray.Properties;

using System.Diagnostics;

namespace SnapraidDaemonTray.Forms;

public partial class InstanceInfoDisplay : UserControl
{
    public Instance Instance { get; }

    public InstanceInfoDisplay(Instance instance)
    {
        Instance = instance;
        InitializeComponent();

        instanceStatusInfoBindingSource.DataSource = instance.Status;
    }

    private void InstanceInfoDisplay_Load(object sender, EventArgs e)
    {

    }

    private void openButton_Click(object sender, EventArgs e)
    {
        contextMenu.Show(openButton, new Point(0, 0), ToolStripDropDownDirection.BelowLeft);
    }

    private void labelInstanceName_Click(object sender, EventArgs e)
    {

    }

    private void updateUi()
    {
        labelValueBad.ForeColor = Instance.Status.ArrayBadBlocks > 0 ? Color.Red : Color.Green;
        labelValueUnsynced.ForeColor = Instance.Status.ArrayUnsyncedBlocks > 0 ? Color.Red : Color.Green;

        if (Instance.Status.CurrentCommand != null)
        {
            panelTaskStatus.Visible = true;
            panelArrayStatus.Visible = false;
            toolStripMenuItemMaintenance.Enabled = false;


            labelTaskName.Text = Instance.Status.CurrentCommand.ToActionName();

            if (Instance.Status.CurrentCommandProgress is null)
            {
                taskProgressBar.Style = ProgressBarStyle.Marquee;
            }
            else
            {
                taskProgressBar.Style = ProgressBarStyle.Continuous;
                taskProgressBar.Value = Instance.Status.CurrentCommandProgress ?? 0;
            }
        }
        else
        {
            panelTaskStatus.Visible = false;
            panelArrayStatus.Visible = true;
            toolStripMenuItemMaintenance.Enabled = true;
        }

        statusImage.Image = Instance.Status.Health switch
        {
            Models.HealthStatus.Healthy => Resources.ok,
            Models.HealthStatus.Error => Resources.error,
            Models.HealthStatus.Warning => Resources.warning,
            _ => null,
        };
    }

    private void instanceStatusInfoBindingSource_CurrentChanged(object sender, EventArgs e)
    {
        if (InvokeRequired)
        {
            Invoke(updateUi);
        }
        else
        {
            updateUi();
        }
    }

    private void instanceStatusInfoBindingSource_CurrentItemChanged(object sender, EventArgs e)
    {
        if (InvokeRequired)
        {
            Invoke(updateUi);
        }
        else
        {
            updateUi();
        }
    }

    private void openDashboard()
    {
        var url = Instance?.Address;
        if (string.IsNullOrWhiteSpace(url))
        {
            return;
        }

        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true,
            });
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Unable to open '{url}': {ex.Message}", "Open", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void toolStripMenuItemDashboard_Click(object sender, EventArgs e)
    {
        openDashboard();
    }

    private async void toolStripMenuItemMaintenance_Click(object sender, EventArgs e)
    {
        await Instance.StartMaintenance(supressNotification: true);
    }

    private void labelInstanceName_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
        openDashboard();
    }
}
