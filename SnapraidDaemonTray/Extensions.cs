using Dapplo.Microsoft.Extensions.Hosting.WinForms;

using Microsoft.Extensions.DependencyInjection;

using SnapraidDeamonApi.Models;
using SnapraidDeamonApi.Snapraid.V1.Activity;

namespace SnapraidDaemonTray;

internal static class Extensions
{
    extension (IServiceCollection services)
    {
        public IServiceCollection AddWinFormsService<T>() where T : class, IWinFormsService
        {
            services.AddSingleton<T>();
            services.AddSingleton<IWinFormsService>(x => x.GetRequiredService<T>());

            return services;
        }
    }

    extension(Task_command? command)
    { 
        public string ToActionName()
        {
            return command?.ToActionName() ?? "-";
        }

        public string ToDisplayName()
        {
            return command?.ToActionName() ?? "-";
        }
    }

    extension (Task_command command)
    {
        public string ToActionName()
        {
            return command switch
            {
                Task_command.Up => "Spinning Up",
                Task_command.Down => "Spinning Down",
                Task_command.Down_idle => "Spinning Down (Idle)",
                Task_command.Sync => "Syncing",
                Task_command.Scrub => "Scrubbing",
                Task_command.Probe => "Probing",
                Task_command.Diff => "Diffing",
                Task_command.Check => "Checking",
                Task_command.Fix => "Fixing",
                _ => command.ToString(),
            };
        }

        public string ToDisplayName()
        {
            return command switch
            {
                Task_command.Up => "Spin Up",
                Task_command.Down => "Spin Down",
                Task_command.Down_idle => "Sping Down (Idle)",
                _ => command.ToString(),
            };
        }
    }

    extension (ActivityGetResponse? response)
    {
        public ActivityGetResponse? NullIfNotHighCommand(Task_high_command command)
        {
            if (response?.HighCommand == command)
            {
                return response;
            }

            return null;
        }
    }
}
