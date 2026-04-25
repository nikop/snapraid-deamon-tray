using Dapplo.Microsoft.Extensions.Hosting.WinForms;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using SnapraidDaemonTray.Forms;
using SnapraidDaemonTray.Instances;
using SnapraidDaemonTray.Models;
using SnapraidDaemonTray.Properties;

using System.Text;

namespace SnapraidDaemonTray;

public class SystemTray
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

    private Dictionary<string, ToolStripMenuItem> instanceMenuItems = [];

    private volatile IServiceScope? _trayScope;

    IHostApplicationLifetime Lifetime { get; }

    AppConfiguration AppConfiguration { get; }

    IServiceProvider ServiceProvider { get; }

    IWinFormsContext WinFormsContext { get; }

    public SystemTray(IHostApplicationLifetime lifetime, AppConfiguration appConfiguration, IServiceProvider serviceProvider, IWinFormsContext winFormsContext)
    {
        Lifetime = lifetime;
        AppConfiguration = appConfiguration;
        ServiceProvider = serviceProvider;
        WinFormsContext = winFormsContext;

        AppConfiguration.ConfigurationChanged += (sender, e) =>
        {
            WinFormsContext.Dispatcher.Invoke(UpdateContextMenu);
            WinFormsContext.Dispatcher.Invoke(Update);
        };
    }

    public async Task OpenTrayPopup()
    {
        if (_trayScope is not null)
        {
            return;
        }

        _trayScope = ServiceProvider.CreateScope();

        await WinFormsContext.Dispatcher.InvokeAsync(async () =>
        {
            var instances = _trayScope.ServiceProvider.GetRequiredService<InstanceManager>().Instances;
            var trayForm = _trayScope.ServiceProvider.GetRequiredService<TrayInfoPopup>();
            var screen = Screen.FromPoint(Cursor.Position);

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

        var scope = ServiceProvider.CreateScope();

        await WinFormsContext.Dispatcher.InvokeAsync(() =>
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
        using var scope = ServiceProvider.CreateScope();
        var instances = scope.ServiceProvider.GetRequiredService<InstanceManager>().Instances;

        foreach (var instance in instances)
        {
            await instance.StartMaintenance(supressNotification: false);
        }
    }

    private void UpdateContextMenu()
    {
        if (systemTray is null)
        {
            return;
        }

        using var scope = ServiceProvider.CreateScope();
        var instances = scope.ServiceProvider.GetRequiredService<InstanceManager>().Instances;

        contextMenuStrip.Items.Clear();

        //contextMenuStrip.Items.Add("Run Maintenance on All Instances", null, async (sender, e) =>
        //{
        //    await StartMaintenance();
        //});

        instanceMenuItems.Clear();

        foreach (var item in instances)
        {
            var menuItem = new ToolStripMenuItem(item.Name, null, async (sender, e) =>
            {
                //await item.StartMaintenance(supressNotification: false);
            });

            menuItem.DropDownItems.Add("Run Maintenance", null, async (sender, e) =>
            {
                await item.StartMaintenance(supressNotification: false);
            });

            instanceMenuItems[item.Key] = menuItem;
            contextMenuStrip.Items.Add(menuItem);
        }

        contextMenuStrip.Items.Add(new ToolStripSeparator());

        //contextMenuStrip.Items.Add();

        contextMenuStrip.Items.Add("Edit Configuration", null, async (sender, e) =>
        {
            await OpenSettings();
        });

        contextMenuStrip.Items.Add("Exit", null, (sender, e) =>
        {
            systemTray.Visible = false;
            Lifetime.StopApplication();
        });


    }

    private void InitializeInternal()
    {
        systemTray = new NotifyIcon
        {
            Icon = Resources.TrayIcon,
            Visible = true,
            Text = "",
            ContextMenuStrip = contextMenuStrip,
        };

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

        UpdateContextMenu();
        Update();
    }

    public void Initialize()
    {
        WinFormsContext.Dispatcher.Invoke(InitializeInternal);
    }

    public void Update()
    {
        WinFormsContext.Dispatcher.InvokeAsync(async () =>
        {
            if (systemTray is null)
            {
                return;
            }

            using var scope = ServiceProvider.CreateScope();
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

                if (instance.Status.Health != HealthStatus.NoConnection)
                {
                    if (instance.Status.CurrentCommand is null)
                    {
                        statusMatrix.Idle++;
                    }
                    else
                    {
                        statusMatrix.Active++;
                    }
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
