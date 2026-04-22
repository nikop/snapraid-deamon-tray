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
        dataGridViewServers = new DataGridView();
        buttonAdd = new Button();
        buttonRemove = new Button();
        buttonSave = new Button();
        colEnabled = new DataGridViewCheckBoxColumn();
        colName = new DataGridViewTextBoxColumn();
        colAddress = new DataGridViewTextBoxColumn();
        ((System.ComponentModel.ISupportInitialize)dataGridViewServers).BeginInit();
        SuspendLayout();
        // 
        // dataGridViewServers
        // 
        dataGridViewServers.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        dataGridViewServers.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        dataGridViewServers.Columns.AddRange(new DataGridViewColumn[] { colEnabled, colName, colAddress });
        dataGridViewServers.Location = new Point(12, 12);
        dataGridViewServers.Name = "dataGridViewServers";
        dataGridViewServers.RowTemplate.Height = 25;
        dataGridViewServers.Size = new Size(560, 300);
        dataGridViewServers.TabIndex = 0;
        // 
        // buttonAdd
        // 
        buttonAdd.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        buttonAdd.Location = new Point(12, 320);
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
        buttonRemove.Location = new Point(93, 320);
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
        buttonSave.Location = new Point(497, 320);
        buttonSave.Name = "buttonSave";
        buttonSave.Size = new Size(75, 23);
        buttonSave.TabIndex = 3;
        buttonSave.Text = "Save";
        buttonSave.UseVisualStyleBackColor = true;
        buttonSave.Click += buttonSave_Click;
        // 
        // colEnabled
        // 
        colEnabled.DataPropertyName = "Enabled";
        colEnabled.HeaderText = "Enabled";
        colEnabled.Name = "colEnabled";
        // 
        // colName
        // 
        colName.DataPropertyName = "Name";
        colName.HeaderText = "Name";
        colName.Name = "colName";
        colName.Width = 150;
        // 
        // colAddress
        // 
        colAddress.DataPropertyName = "Address";
        colAddress.HeaderText = "Address";
        colAddress.Name = "colAddress";
        colAddress.Width = 300;
        // 
        // ConfigEditor
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(584, 355);
        Controls.Add(buttonSave);
        Controls.Add(buttonRemove);
        Controls.Add(buttonAdd);
        Controls.Add(dataGridViewServers);
        Name = "ConfigEditor";
        Text = "Configuration Editor";
        Load += ConfigEditor_Load;
        ((System.ComponentModel.ISupportInitialize)dataGridViewServers).EndInit();
        ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.DataGridView dataGridViewServers;
    private System.Windows.Forms.Button buttonAdd;
    private System.Windows.Forms.Button buttonRemove;
    private System.Windows.Forms.Button buttonSave;
    private System.Windows.Forms.DataGridViewCheckBoxColumn colEnabled;
    private System.Windows.Forms.DataGridViewTextBoxColumn colName;
    private System.Windows.Forms.DataGridViewTextBoxColumn colAddress;
}
