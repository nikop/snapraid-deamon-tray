using Dapplo.Microsoft.Extensions.Hosting.WinForms;

using SnapraidDaemonTray.Instances;

using System.Windows.Controls.Primitives;

namespace SnapraidDaemonTray.Forms;

public partial class TrayInfoPopup : Form, IWinFormsShell
{
    public const int ItemWidth = 400;

    public const int ItemHeight = 200;

    public const int ItemsPerRow = 1;

    public InstanceManager InstanceManager { get; }

    public TrayInfoPopup(InstanceManager instanceManager)
    {
        InstanceManager = instanceManager;
        InitializeComponent();

        var countRows = (int) Math.Ceiling(instanceManager.Instances.Count / (decimal) ItemsPerRow);

        StartPosition = FormStartPosition.Manual;
        Size = new Size(ItemWidth * ItemsPerRow, ItemHeight * countRows);

        this.SetWindowCornerPreference(Pinvoke.DWM_WINDOW_CORNER_PREFERENCE.DWMWCP_ROUND);
    }

    private async void MainForm_Load(object sender, EventArgs e)
    {
        var instances = InstanceManager.Instances;
        var countRows = (int)Math.Ceiling(instances.Count / (decimal)ItemsPerRow);

        tablePanel.ColumnCount = ItemsPerRow;
        tablePanel.RowCount = Math.Max(1, instances.Count);
        tablePanel.ColumnStyles.Clear();
        tablePanel.RowStyles.Clear();

        var i = 0;

        foreach (var column in Enumerable.Range(0, ItemsPerRow))
        {
            tablePanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f / ItemsPerRow));
        }

        foreach (var row in Enumerable.Range(0, tablePanel.RowCount))
        {
            tablePanel.RowStyles.Add(new RowStyle(SizeType.Absolute, ItemHeight));
        }

        foreach (var instance in instances)
        {
            var instancePanel = new InstanceInfoDisplay(instance)
            {
                Dock = DockStyle.Fill,
                Height = ItemHeight,
            };

            var column = i % ItemsPerRow;
            var row = i / ItemsPerRow;

            tablePanel.Controls.Add(instancePanel);
            tablePanel.SetCellPosition(instancePanel, new TableLayoutPanelCellPosition { Column = column, Row = row });

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
