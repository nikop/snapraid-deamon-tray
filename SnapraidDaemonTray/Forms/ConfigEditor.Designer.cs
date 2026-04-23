namespace SnapraidDaemonTray.Forms;

partial class ConfigEditor
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

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        tabControlMain = new TabControl();
        tabPageServers = new TabPage();
        dataGridViewServers = new DataGridView();
        buttonAdd = new Button();
        buttonRemove = new Button();
        buttonSave = new Button();
        buttonCancel = new Button();
        colEnabled = new DataGridViewCheckBoxColumn();
        colName = new DataGridViewTextBoxColumn();
        colAddress = new DataGridViewTextBoxColumn();
        tabControlMain.SuspendLayout();
        tabPageServers.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)dataGridViewServers).BeginInit();
        SuspendLayout();
        // 
        // tabControlMain
        // 
        tabControlMain.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        tabControlMain.Controls.Add(tabPageServers);
        tabControlMain.Location = new Point(12, 12);
        tabControlMain.Name = "tabControlMain";
        tabControlMain.SelectedIndex = 0;
        tabControlMain.Size = new Size(603, 291);
        tabControlMain.TabIndex = 0;
        // 
        // tabPageServers
        // 
        tabPageServers.BackColor = SystemColors.Control;
        tabPageServers.Controls.Add(dataGridViewServers);
        tabPageServers.Controls.Add(buttonAdd);
        tabPageServers.Controls.Add(buttonRemove);
        tabPageServers.Location = new Point(4, 24);
        tabPageServers.Name = "tabPageServers";
        tabPageServers.Padding = new Padding(3);
        tabPageServers.Size = new Size(595, 263);
        tabPageServers.TabIndex = 0;
        tabPageServers.Text = "Servers";
        // 
        // dataGridViewServers
        // 
        dataGridViewServers.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        dataGridViewServers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dataGridViewServers.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        dataGridViewServers.Columns.AddRange(new DataGridViewColumn[] { colEnabled, colName, colAddress });
        dataGridViewServers.Location = new Point(6, 6);
        dataGridViewServers.Name = "dataGridViewServers";
        dataGridViewServers.RowHeadersVisible = false;
        dataGridViewServers.SelectionMode = DataGridViewSelectionMode.CellSelect;
        dataGridViewServers.Size = new Size(583, 191);
        dataGridViewServers.TabIndex = 0;
        // 
        // buttonAdd
        // 
        buttonAdd.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        buttonAdd.Location = new Point(6, 203);
        buttonAdd.Name = "buttonAdd";
        buttonAdd.Size = new Size(75, 23);
        buttonAdd.TabIndex = 1;
        buttonAdd.Text = "Add";
        buttonAdd.UseVisualStyleBackColor = true;
        buttonAdd.Click += buttonAdd_Click;
        // 
        // buttonRemove
        // 
        buttonRemove.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        buttonRemove.Location = new Point(87, 203);
        buttonRemove.Name = "buttonRemove";
        buttonRemove.Size = new Size(75, 23);
        buttonRemove.TabIndex = 2;
        buttonRemove.Text = "Remove";
        buttonRemove.UseVisualStyleBackColor = true;
        buttonRemove.Click += buttonRemove_Click;
        // 
        // buttonSave
        // 
        buttonSave.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        buttonSave.Location = new Point(540, 309);
        buttonSave.Name = "buttonSave";
        buttonSave.Size = new Size(75, 23);
        buttonSave.TabIndex = 3;
        buttonSave.Text = "Save";
        buttonSave.UseVisualStyleBackColor = true;
        buttonSave.Click += buttonSave_Click;
        // 
        // buttonCancel
        // 
        buttonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        buttonCancel.Location = new Point(459, 309);
        buttonCancel.Name = "buttonCancel";
        buttonCancel.Size = new Size(75, 23);
        buttonCancel.TabIndex = 4;
        buttonCancel.Text = "Cancel";
        buttonCancel.UseVisualStyleBackColor = true;
        buttonCancel.Click += buttonCancel_Click;
        // 
        // colEnabled
        // 
        colEnabled.DataPropertyName = "Enabled";
        colEnabled.FillWeight = 29.9845352F;
        colEnabled.HeaderText = "Enabled";
        colEnabled.MinimumWidth = 25;
        colEnabled.Name = "colEnabled";
        colEnabled.Resizable = DataGridViewTriState.False;
        // 
        // colName
        // 
        colName.DataPropertyName = "Name";
        colName.FillWeight = 83.456955F;
        colName.HeaderText = "Name";
        colName.Name = "colName";
        // 
        // colAddress
        // 
        colAddress.DataPropertyName = "Address";
        colAddress.FillWeight = 83.456955F;
        colAddress.HeaderText = "Address";
        colAddress.Name = "colAddress";
        // 
        // ConfigEditor
        // 
        AcceptButton = buttonSave;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        CancelButton = buttonCancel;
        ClientSize = new Size(627, 344);
        Controls.Add(buttonCancel);
        Controls.Add(buttonSave);
        Controls.Add(tabControlMain);
        KeyPreview = true;
        Name = "ConfigEditor";
        Text = "Configuration Editor";
        Load += ConfigEditor_Load;
        tabControlMain.ResumeLayout(false);
        tabPageServers.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)dataGridViewServers).EndInit();
        ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TabControl tabControlMain;
    private System.Windows.Forms.TabPage tabPageServers;
    private System.Windows.Forms.DataGridView dataGridViewServers;
    private System.Windows.Forms.Button buttonAdd;
    private System.Windows.Forms.Button buttonRemove;
    private System.Windows.Forms.Button buttonSave;
    private System.Windows.Forms.Button buttonCancel;
    private DataGridViewCheckBoxColumn colEnabled;
    private DataGridViewTextBoxColumn colName;
    private DataGridViewTextBoxColumn colAddress;
}
