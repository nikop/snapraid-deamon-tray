using Dapplo.Microsoft.Extensions.Hosting.WinForms;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using SnapraidDaemonTray.Forms;
using SnapraidDaemonTray.Instances;
using SnapraidDaemonTray.Models;
using SnapraidDaemonTray.Properties;

using System.Text;

namespace SnapraidDaemonTray;

public class SystemTray(IHostApplicationLifetime lifetime, IServiceProvider serviceProvider, IWinFormsContext winFormsContext) : IWinFormsService
{
    struct StatusMatrix
    {
        public int Idle { get; set; }

        public int Warning { get; set; }

        public int Error { get; set; }

        public int Active { get; set; }
    }

    NotifyIcon? systemTray;

    private readonly ContextMenuStrip contextMenuStrip = new();

    private volatile IServiceScope? _trayScope;

    public async Task OpenTrayPopup()
    {
        if (_trayScope is not null)
        {
            return;
        }

        _trayScope = serviceProvider.CreateScope();

        await winFormsContext.Dispatcher.InvokeAsync(async () =>
        {
            var instances = _trayScope.ServiceProvider.GetRequiredService<InstanceManager>().Instances;
            var trayForm = _trayScope.ServiceProvider.GetRequiredService<TrayInfoPopup>();
            var screen = Screen.FromPoint(Cursor.Position);

            trayForm.StartPosition = FormStartPosition.Manual;
            trayForm.Size = new Size(400, TrayInfoPopup.ItemHeight * Math.Max(1, instances.Count));

            var x = screen.WorkingArea.Right - trayForm.Width;
            var y = screen.WorkingArea.Bottom - trayForm.Height;

            trayForm.WindowState = FormWindowState.Normal;
            trayForm.Location = new Point(x, y);

            trayForm.Show();
            trayForm.Activate();

            trayForm.FormClosed += async (sender, e) =>
            {
                await Task.Delay(500);
                _trayScope.Dispose();
                _trayScope = null;
            };
        });
    }

    public async Task OpenSettings()
    {
        // Open config editor in a new scope
        if (_trayScope is not null)
        {
            return;
        }

        var scope = serviceProvider.CreateScope();

        await winFormsContext.Dispatcher.InvokeAsync(() =>
        {
            try
            {
                var editor = scope.ServiceProvider.GetRequiredService<ConfigEditor>();
                editor.Show();
                editor.FormClosed += (s, ev) =>
                {
                    Task.Run(async () =>
                    {
                        await Task.Delay(500);
                        scope.Dispose();
                        scope = null;
                    });
                };
            }
            catch
            {
                scope?.Dispose();
                scope = null;
            }
        });
    }

    public async Task StartMaintenance()
    {
        using var scope = serviceProvider.CreateScope();
        var instances = scope.ServiceProvider.GetRequiredService<InstanceManager>().Instances;

        foreach (var instance in instances)
        {
            await instance.StartMaintenance(supressNotification: false);
        }
    }

    public void Initialize()
    {
        systemTray = new NotifyIcon
        {
            Icon = Resources.TrayIcon,
            Visible = true,
            Text = ""
        };

        contextMenuStrip.Items.Add("Run Maintenance on All Instances", null, async (sender, e) =>
        {
            await StartMaintenance();
        });

        contextMenuStrip.Items.Add("Edit Configuration", null, async (sender, e) =>
        {
            await OpenSettings();
        });

        contextMenuStrip.Items.Add("Exit", null, (sender, e) =>
        {
            systemTray.Visible = false;
            lifetime.StopApplication();
        });

        systemTray.MouseClick += async (sender, e) =>
        {
            if (e.Button == MouseButtons.Right)
            {
                return;
            }

            if (_trayScope is not null)
            {
                return;
            }

            await OpenTrayPopup();
        };

        systemTray.ContextMenuStrip = contextMenuStrip;

        Update();
    }

    public void Update()
    {
        winFormsContext.Dispatcher.InvokeAsync(async () =>
        {
            if (systemTray is null)
            {
                return;
            }

            using var scope = serviceProvider.CreateScope();
            var instances = scope.ServiceProvider.GetRequiredService<InstanceManager>().Instances;

            var sb = new StringBuilder();
            sb.AppendLine("Snapraid Daemon Tray");

            StatusMatrix statusMatrix = new();

            foreach (var instance in instances)
            {
                if (instance.Status.Health is HealthStatus.Error)
                {
                    statusMatrix.Error++;
                }
                else if (instance.Status.Health is HealthStatus.Warning)
                {
                    statusMatrix.Warning++;
                }

                if (instance.Status.CurrentCommand is null)
                {
                    statusMatrix.Idle++;
                }
                else
                {
                    statusMatrix.Active++;
                }

                sb.AppendLine($"{instance.Name}: {instance.Status.Health}");
            }

            systemTray.Icon = statusMatrix switch
            {
                { Error: > 0 } => Resources.statusError,
                { Warning: > 0 } => Resources.statusWarning,
                { Active: > 0 } => Resources.statusActive,
                { Idle: > 0 } => Resources.statusOk,
                _ => Resources.TrayIcon
            };

            systemTray.Text = sb.ToString();
        });
    }
}
