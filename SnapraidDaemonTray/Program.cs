using Dapplo.Microsoft.Extensions.Hosting.WinForms;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using SnapraidDaemonTray;
using SnapraidDaemonTray.Instances;
using SnapraidDaemonTray.Forms;

var builder = Host.CreateApplicationBuilder(args);

builder.ConfigureWinForms(options =>
{
    options.EnableVisualStyles = true;
});

Application.SetColorMode(SystemColorMode.System);

builder.Services.AddSingleton<AppConfiguration>();
builder.Services.AddSingleton<InstanceManager>();

builder.Services.AddWinFormsService<SystemTray>();
builder.Services.AddWinFormsService<NotificationsHandler>();
builder.Services.AddTransient<TrayInfoPopup>();
builder.Services.AddTransient<ConfigEditor>();

builder.Services.AddHostedService<StartupHandler>();
builder.Services.AddHostedService<StatusPoller>();

var host = builder.Build();
await host.RunAsync();