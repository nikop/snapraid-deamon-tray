using SnapraidDaemonTray.Config;

using System.ComponentModel;

namespace SnapraidDaemonTray.Forms;

public partial class ConfigEditor : Form
{
    private readonly AppConfiguration _appConfiguration;

    private BindingList<ConfigFileServer> _serversBinding = [];
    private bool _isDirty = false;

    public ConfigEditor(AppConfiguration appConfiguration)
    {
        _appConfiguration = appConfiguration;
        InitializeComponent();
        FormClosing += ConfigEditor_FormClosing;
    }

    private void DataGridViewServers_CellValidating(object? sender, DataGridViewCellValidatingEventArgs e)
    {
        var dgv = (DataGridView)sender!;
        var cell = dgv.Rows[e.RowIndex].Cells[e.ColumnIndex];
        var value = (e.FormattedValue ?? string.Empty).ToString() ?? "";

        var error = GetCellValidationError(dgv, e.RowIndex, e.ColumnIndex, value);
        ApplyCellValidationVisuals(cell, error);
        e.Cancel = error is not null;
    }

    private void DataGridViewServers_CellEndEdit(object? sender, DataGridViewCellEventArgs e)
    {
        var dgv = (DataGridView)sender!;
        var cell = dgv.Rows[e.RowIndex].Cells[e.ColumnIndex];
        // Re-validate the cell and apply visuals
        var val = cell.Value?.ToString() ?? string.Empty;
        var error = GetCellValidationError(dgv, e.RowIndex, e.ColumnIndex, val);
        ApplyCellValidationVisuals(cell, error);
    }

    private string? GetCellValidationError(DataGridView dgv, int rowIndex, int columnIndex, string value)
    {
        var column = dgv.Columns[columnIndex];

        if (column == colName)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return "Name is required.";
            }

            var name = value.Trim();
            for (int i = 0; i < dgv.Rows.Count; i++)
            {
                if (i == rowIndex) continue;
                var other = dgv.Rows[i].Cells[colName.Index].Value?.ToString()?.Trim();
                if (!string.IsNullOrWhiteSpace(other) && string.Equals(other, name, StringComparison.OrdinalIgnoreCase))
                {
                    return "Duplicate name.";
                }
            }
        }
        else if (column == colAddress)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return "Address is required.";
            }

            if (!Uri.TryCreate(value.Trim(), UriKind.Absolute, out var uri) || (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
            {
                return "Address must be an absolute http/https URL.";
            }
        }

        return null;
    }

    private void ApplyCellValidationVisuals(DataGridViewCell cell, string? error)
    {
        if (error is null)
        {
            // Clear visuals
            cell.Style.BackColor = Color.Empty;
            cell.ToolTipText = string.Empty;
            cell.ErrorText = string.Empty;
        }
        else
        {
            cell.Style.BackColor = Color.MistyRose;
            cell.ToolTipText = error;
            cell.ErrorText = error;
        }
    }

    private bool HasValidationErrors()
    {
        foreach (DataGridViewRow row in dataGridViewServers.Rows)
        {
            foreach (DataGridViewCell cell in row.Cells)
            {
                if (!string.IsNullOrEmpty(cell.ErrorText))
                    return true;
            }
        }

        return false;
    }

    private async void ConfigEditor_Load(object sender, EventArgs e)
    {
        var config = await _appConfiguration.GetCurrentConfiguration();

        _serversBinding = new BindingList<ConfigFileServer>(config.Servers ?? []);
        dataGridViewServers.AutoGenerateColumns = false;
        dataGridViewServers.DataSource = _serversBinding;

        // Track changes to detect unsaved edits
        _isDirty = false;
        _serversBinding.ListChanged += (s, ev) => { _isDirty = true; };

        dataGridViewServers.CellValueChanged += (s, ev) => { _isDirty = true; };
        dataGridViewServers.CurrentCellDirtyStateChanged += (s, ev) =>
        {
            if (dataGridViewServers.IsCurrentCellDirty)
            {
                dataGridViewServers.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        };
    }

    private async void buttonSave_Click(object sender, EventArgs e)
    {
        await TrySaveAsync();
    }

    private async Task<bool> TrySaveAsync()
    {
        // Ensure any current edits are committed
        dataGridViewServers.EndEdit();

        // If any inline validation errors exist, prevent save
        if (HasValidationErrors())
        {
            MessageBox.Show("Please fix validation errors before saving.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }

        var servers = _serversBinding.ToList();

        if (!ValidateServers(servers, out var errorMessage))
        {
            MessageBox.Show(errorMessage, "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }

        var config = new ConfigFile
        {
            Servers = servers
        };

        try
        {
            await _appConfiguration.SaveConfig(config);
            _isDirty = false;
            MessageBox.Show("Configuration saved.", "Save", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return true;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Unable to save configuration: {ex.Message}", "Save", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }
    }

    private void buttonAdd_Click(object sender, EventArgs e)
    {
        _serversBinding.Add(new ConfigFileServer { Enabled = true, Name = "New", Address = "http://" });
    }

    private void buttonRemove_Click(object sender, EventArgs e)
    {
        if (dataGridViewServers.CurrentRow?.DataBoundItem is ConfigFileServer server)
        {
            _serversBinding.Remove(server);
        }
    }

    private void buttonCancel_Click(object sender, EventArgs e)
    {
        Close();
    }

    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
        // Allow Esc to close if not handled by a control
        if (keyData == Keys.Escape)
        {
            Close();
            return true;
        }

        return base.ProcessCmdKey(ref msg, keyData);
    }

    private async void ConfigEditor_FormClosing(object? sender, FormClosingEventArgs e)
    {
        if (!_isDirty)
        {
            return;
        }

        var result = MessageBox.Show("You have unsaved changes. Save before closing?", "Unsaved Changes", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);

        if (result == DialogResult.Cancel)
        {
            e.Cancel = true;
            return;
        }

        if (result == DialogResult.Yes)
        {
            var saved = await TrySaveAsync();
            if (!saved)
            {
                e.Cancel = true;
            }
        }
        else
        {
            // No - discard changes
            _isDirty = false;
        }
    }

    private bool ValidateServers(List<ConfigFileServer> servers, out string errorMessage)
    {
        var errors = new List<string>();

        // Name: not empty and no duplicates (case-insensitive)
        var duplicateNames = servers
            .Where(s => !string.IsNullOrWhiteSpace(s.Name))
            .GroupBy(s => s.Name.Trim(), StringComparer.OrdinalIgnoreCase)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();

        if (servers.Any(s => string.IsNullOrWhiteSpace(s.Name)))
        {
            errors.Add("All servers must have a non-empty Name.");
        }

        if (duplicateNames.Any())
        {
            errors.Add($"Duplicate server names found: {string.Join(", ", duplicateNames)}");
        }

        // Address: must be valid absolute http/https URL
        var invalidAddresses = new List<string>();
        foreach (var s in servers)
        {
            if (string.IsNullOrWhiteSpace(s.Address))
            {
                invalidAddresses.Add(s.Name ?? "(no name)");
                continue;
            }

            if (!Uri.TryCreate(s.Address.Trim(), UriKind.Absolute, out var uri) || (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
            {
                invalidAddresses.Add(s.Name ?? s.Address);
            }
        }

        if (invalidAddresses.Any())
        {
            errors.Add($"Servers with invalid Address (must be absolute http/https): {string.Join(", ", invalidAddresses)}");
        }

        if (errors.Any())
        {
            errorMessage = string.Join("\n", errors);
            return false;
        }

        errorMessage = string.Empty;
        return true;
    }
}
