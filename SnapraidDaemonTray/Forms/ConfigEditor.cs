using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using SnapraidDaemonTray.Config;

namespace SnapraidDaemonTray.Forms;

public partial class ConfigEditor : Form
{
    private readonly AppConfiguration _appConfiguration;

    private BindingList<ConfigFileServer> _serversBinding;

    public ConfigEditor(AppConfiguration appConfiguration)
    {
        _appConfiguration = appConfiguration;
        InitializeComponent();
    }

    private async void ConfigEditor_Load(object sender, EventArgs e)
    {
        var config = await _appConfiguration.GetCurrentConfiguration();

        _serversBinding = new BindingList<ConfigFileServer>(config.Servers ?? new List<ConfigFileServer>());

        dataGridViewServers.AutoGenerateColumns = false;
        dataGridViewServers.DataSource = _serversBinding;
    }

    private async void buttonSave_Click(object sender, EventArgs e)
    {
        var servers = _serversBinding.ToList();

        // Validate
        if (!ValidateServers(servers, out var errorMessage))
        {
            MessageBox.Show(errorMessage, "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var config = new ConfigFile
        {
            Servers = servers
        };

        try
        {
            await _appConfiguration.SaveConfig(config);
            MessageBox.Show("Configuration saved.", "Save", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Unable to save configuration: {ex.Message}", "Save", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
