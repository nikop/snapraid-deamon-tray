using Dapplo.Microsoft.Extensions.Hosting.WinForms;

using SnapraidDaemonTray.Forms;
using SnapraidDaemonTray.Instances;

namespace SnapraidDaemonTray;

public partial class TrayInfoPopup : Form, IWinFormsShell
{
    public const int ItemHeight = 200;

    public InstanceManager InstanceManager { get; }

    public TrayInfoPopup(InstanceManager instanceManager)
    {
        InstanceManager = instanceManager;
        InitializeComponent();

        this.SetWindowCornerPreference(Pinvoke.DWM_WINDOW_CORNER_PREFERENCE.DWMWCP_ROUND);
    }

    private async void MainForm_Load(object sender, EventArgs e)
    {
        var intances = (await InstanceManager.GetAll()).ToList();
        tablePanel.RowCount = Math.Max(1, intances.Count);
        var i = 0;

        foreach (var instance in intances)
        {
            var instancePanel = new InstanceInfoDisplay(instance)
            {
                Dock = DockStyle.Fill,
                Height = ItemHeight,
            };

            tablePanel.Controls.Add(instancePanel);
            tablePanel.SetCellPosition(instancePanel, new TableLayoutPanelCellPosition { Column = 0, Row = i });

            i++;
        }
    }

    private void MainForm_Shown(object sender, EventArgs e)
    {
    }

    private void TrayInfoPopup_Deactivate(object sender, EventArgs e)
    {
        Close();
    }

    private void tablePanel_Paint(object sender, PaintEventArgs e)
    {

    }
}
