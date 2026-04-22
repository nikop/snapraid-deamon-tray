namespace SnapraidDaemonTray.Forms;

partial class InstanceInfoDisplay
{
    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary> 
    /// Clean up any resources being used.
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

    #region Component Designer generated code

    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        openButton = new Button();
        labelInstanceName = new Label();
        instanceStatusInfoBindingSource = new BindingSource(components);
        panelActions = new Panel();
        btnSync = new Button();
        statusImage = new PictureBox();
        panelArrayStatus = new Panel();
        tableLayoutPanel1 = new TableLayoutPanel();
        labelValueBad = new Label();
        labelTextBad = new Label();
        labelTextUnsynched = new Label();
        labelValueUnsynced = new Label();
        panelTaskStatus = new Panel();
        tableLayoutPanel2 = new TableLayoutPanel();
        labelTaskName = new Label();
        taskProgressBar = new ProgressBar();
        ((System.ComponentModel.ISupportInitialize)instanceStatusInfoBindingSource).BeginInit();
        panelActions.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)statusImage).BeginInit();
        panelArrayStatus.SuspendLayout();
        tableLayoutPanel1.SuspendLayout();
        panelTaskStatus.SuspendLayout();
        tableLayoutPanel2.SuspendLayout();
        SuspendLayout();
        // 
        // openButton
        // 
        openButton.Dock = DockStyle.Right;
        openButton.Location = new Point(432, 8);
        openButton.Name = "openButton";
        openButton.Size = new Size(111, 24);
        openButton.TabIndex = 0;
        openButton.Text = "Open Dashboard";
        openButton.UseVisualStyleBackColor = true;
        openButton.Click += openButton_Click;
        // 
        // labelInstanceName
        // 
        labelInstanceName.AutoSize = true;
        labelInstanceName.DataBindings.Add(new Binding("Text", instanceStatusInfoBindingSource, "Name", true));
        labelInstanceName.Dock = DockStyle.Left;
        labelInstanceName.Font = new Font("Segoe UI", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
        labelInstanceName.Location = new Point(40, 8);
        labelInstanceName.Name = "labelInstanceName";
        labelInstanceName.Size = new Size(64, 25);
        labelInstanceName.TabIndex = 1;
        labelInstanceName.Text = "Name";
        labelInstanceName.Click += labelInstanceName_Click;
        // 
        // instanceStatusInfoBindingSource
        // 
        instanceStatusInfoBindingSource.DataSource = typeof(Models.InstanceStatusInfo);
        instanceStatusInfoBindingSource.CurrentChanged += instanceStatusInfoBindingSource_CurrentChanged;
        instanceStatusInfoBindingSource.CurrentItemChanged += instanceStatusInfoBindingSource_CurrentItemChanged;
        // 
        // panelActions
        // 
        panelActions.Controls.Add(btnSync);
        panelActions.Controls.Add(labelInstanceName);
        panelActions.Controls.Add(statusImage);
        panelActions.Controls.Add(openButton);
        panelActions.Dock = DockStyle.Top;
        panelActions.Location = new Point(0, 0);
        panelActions.Name = "panelActions";
        panelActions.Padding = new Padding(8);
        panelActions.Size = new Size(551, 40);
        panelActions.TabIndex = 2;
        // 
        // btnSync
        // 
        btnSync.Dock = DockStyle.Right;
        btnSync.Location = new Point(363, 8);
        btnSync.Name = "btnSync";
        btnSync.Size = new Size(69, 24);
        btnSync.TabIndex = 3;
        btnSync.Text = "Sync";
        btnSync.UseVisualStyleBackColor = true;
        btnSync.Click += button1_Click;
        // 
        // statusImage
        // 
        statusImage.BackgroundImageLayout = ImageLayout.None;
        statusImage.Dock = DockStyle.Left;
        statusImage.Location = new Point(8, 8);
        statusImage.Name = "statusImage";
        statusImage.Size = new Size(32, 24);
        statusImage.SizeMode = PictureBoxSizeMode.Zoom;
        statusImage.TabIndex = 2;
        statusImage.TabStop = false;
        // 
        // panelArrayStatus
        // 
        panelArrayStatus.Controls.Add(tableLayoutPanel1);
        panelArrayStatus.Dock = DockStyle.Top;
        panelArrayStatus.Location = new Point(0, 40);
        panelArrayStatus.Name = "panelArrayStatus";
        panelArrayStatus.Padding = new Padding(8);
        panelArrayStatus.Size = new Size(551, 115);
        panelArrayStatus.TabIndex = 3;
        // 
        // tableLayoutPanel1
        // 
        tableLayoutPanel1.ColumnCount = 2;
        tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        tableLayoutPanel1.Controls.Add(labelValueBad, 1, 0);
        tableLayoutPanel1.Controls.Add(labelTextBad, 0, 0);
        tableLayoutPanel1.Controls.Add(labelTextUnsynched, 0, 1);
        tableLayoutPanel1.Controls.Add(labelValueUnsynced, 1, 1);
        tableLayoutPanel1.Dock = DockStyle.Fill;
        tableLayoutPanel1.Location = new Point(8, 8);
        tableLayoutPanel1.Name = "tableLayoutPanel1";
        tableLayoutPanel1.RowCount = 3;
        tableLayoutPanel1.RowStyles.Add(new RowStyle());
        tableLayoutPanel1.RowStyles.Add(new RowStyle());
        tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
        tableLayoutPanel1.Size = new Size(535, 99);
        tableLayoutPanel1.TabIndex = 2;
        // 
        // labelValueBad
        // 
        labelValueBad.Anchor = AnchorStyles.Left;
        labelValueBad.AutoSize = true;
        labelValueBad.DataBindings.Add(new Binding("Text", instanceStatusInfoBindingSource, "ArrayBadBlocks", true, DataSourceUpdateMode.OnValidation, null, "N0"));
        labelValueBad.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        labelValueBad.Location = new Point(270, 0);
        labelValueBad.Name = "labelValueBad";
        labelValueBad.Size = new Size(14, 15);
        labelValueBad.TabIndex = 0;
        labelValueBad.Text = "0";
        // 
        // labelTextBad
        // 
        labelTextBad.Anchor = AnchorStyles.Left;
        labelTextBad.AutoSize = true;
        labelTextBad.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        labelTextBad.Location = new Point(3, 0);
        labelTextBad.Name = "labelTextBad";
        labelTextBad.Size = new Size(28, 15);
        labelTextBad.TabIndex = 1;
        labelTextBad.Text = "Bad";
        // 
        // labelTextUnsynched
        // 
        labelTextUnsynched.Anchor = AnchorStyles.Left;
        labelTextUnsynched.AutoSize = true;
        labelTextUnsynched.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        labelTextUnsynched.Location = new Point(3, 15);
        labelTextUnsynched.Name = "labelTextUnsynched";
        labelTextUnsynched.Size = new Size(61, 15);
        labelTextUnsynched.TabIndex = 2;
        labelTextUnsynched.Text = "Unsynced";
        // 
        // labelValueUnsynced
        // 
        labelValueUnsynced.Anchor = AnchorStyles.Left;
        labelValueUnsynced.AutoSize = true;
        labelValueUnsynced.DataBindings.Add(new Binding("Text", instanceStatusInfoBindingSource, "ArrayUnsyncedBlocks", true, DataSourceUpdateMode.OnValidation, null, "N0"));
        labelValueUnsynced.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        labelValueUnsynced.Location = new Point(270, 15);
        labelValueUnsynced.Name = "labelValueUnsynced";
        labelValueUnsynced.Size = new Size(14, 15);
        labelValueUnsynced.TabIndex = 3;
        labelValueUnsynced.Text = "0";
        // 
        // panelTaskStatus
        // 
        panelTaskStatus.Controls.Add(tableLayoutPanel2);
        panelTaskStatus.Dock = DockStyle.Top;
        panelTaskStatus.Location = new Point(0, 155);
        panelTaskStatus.Name = "panelTaskStatus";
        panelTaskStatus.Padding = new Padding(8);
        panelTaskStatus.Size = new Size(551, 125);
        panelTaskStatus.TabIndex = 4;
        // 
        // tableLayoutPanel2
        // 
        tableLayoutPanel2.ColumnCount = 2;
        tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        tableLayoutPanel2.Controls.Add(labelTaskName, 0, 0);
        tableLayoutPanel2.Controls.Add(taskProgressBar, 0, 1);
        tableLayoutPanel2.Dock = DockStyle.Fill;
        tableLayoutPanel2.Location = new Point(8, 8);
        tableLayoutPanel2.Name = "tableLayoutPanel2";
        tableLayoutPanel2.RowCount = 4;
        tableLayoutPanel2.RowStyles.Add(new RowStyle());
        tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 59.6330261F));
        tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Absolute, 43F));
        tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
        tableLayoutPanel2.Size = new Size(535, 109);
        tableLayoutPanel2.TabIndex = 0;
        // 
        // labelTaskName
        // 
        labelTaskName.AutoSize = true;
        labelTaskName.Location = new Point(3, 0);
        labelTaskName.Name = "labelTaskName";
        labelTaskName.Size = new Size(38, 15);
        labelTaskName.TabIndex = 0;
        labelTaskName.Text = "label1";
        // 
        // taskProgressBar
        // 
        tableLayoutPanel2.SetColumnSpan(taskProgressBar, 2);
        taskProgressBar.Dock = DockStyle.Fill;
        taskProgressBar.Location = new Point(3, 18);
        taskProgressBar.Name = "taskProgressBar";
        taskProgressBar.Size = new Size(529, 25);
        taskProgressBar.TabIndex = 1;
        // 
        // InstanceInfoDisplay
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        Controls.Add(panelTaskStatus);
        Controls.Add(panelArrayStatus);
        Controls.Add(panelActions);
        Name = "InstanceInfoDisplay";
        Size = new Size(551, 382);
        Load += InstanceInfoDisplay_Load;
        ((System.ComponentModel.ISupportInitialize)instanceStatusInfoBindingSource).EndInit();
        panelActions.ResumeLayout(false);
        panelActions.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)statusImage).EndInit();
        panelArrayStatus.ResumeLayout(false);
        tableLayoutPanel1.ResumeLayout(false);
        tableLayoutPanel1.PerformLayout();
        panelTaskStatus.ResumeLayout(false);
        tableLayoutPanel2.ResumeLayout(false);
        tableLayoutPanel2.PerformLayout();
        ResumeLayout(false);
    }

    #endregion

    private Button openButton;
    private Label labelInstanceName;
    private BindingSource instanceStatusInfoBindingSource;
    private Panel panelActions;
    private Panel panelArrayStatus;
    private PictureBox statusImage;
    private TableLayoutPanel tableLayoutPanel1;
    private Label labelValueBad;
    private Label labelTextBad;
    private Label labelTextUnsynched;
    private Label labelValueUnsynced;
    private Button btnSync;
    private Panel panelTaskStatus;
    private TableLayoutPanel tableLayoutPanel2;
    private Label labelTaskName;
    private ProgressBar taskProgressBar;
}
