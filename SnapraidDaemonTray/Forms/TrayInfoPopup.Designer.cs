namespace SnapraidDaemonTray.Forms;

partial class TrayInfoPopup
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TrayInfoPopup));
        tablePanel = new TableLayoutPanel();
        SuspendLayout();
        // 
        // tablePanel
        // 
        tablePanel.ColumnCount = 1;
        tablePanel.ColumnStyles.Add(new ColumnStyle());
        tablePanel.Dock = DockStyle.Fill;
        tablePanel.Location = new Point(0, 0);
        tablePanel.Name = "tablePanel";
        tablePanel.RowCount = 1;
        tablePanel.RowStyles.Add(new RowStyle());
        tablePanel.Size = new Size(648, 170);
        tablePanel.TabIndex = 0;
        tablePanel.Paint += tablePanel_Paint;
        // 
        // TrayInfoPopup
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(648, 170);
        Controls.Add(tablePanel);
        FormBorderStyle = FormBorderStyle.None;
        Icon = (Icon)resources.GetObject("$this.Icon");
        Name = "TrayInfoPopup";
        ShowInTaskbar = false;
        SizeGripStyle = SizeGripStyle.Show;
        Text = "Snapraid Daemon Tray";
        TopMost = true;
        Deactivate += TrayInfoPopup_Deactivate;
        Load += MainForm_Load;
        Shown += MainForm_Shown;
        ResumeLayout(false);
    }

    #endregion

    private TableLayoutPanel tablePanel;
}
