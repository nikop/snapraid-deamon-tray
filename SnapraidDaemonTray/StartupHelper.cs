using Microsoft.Win32;
using System.Diagnostics;

namespace SnapraidDaemonTray;

/// <summary>
/// Utility class to manage application startup registration with Windows.
/// Handles both registry-based startup and StartUp folder shortcuts.
/// </summary>
public static class StartupHelper
{
    private const string AppName = "SnapraidDaemonTray";
    private const string RegistryPath = @"Software\Microsoft\Windows\CurrentVersion\Run";

    /// <summary>
    /// Registers the application to run at Windows startup via registry.
    /// </summary>
    /// <returns>True if successful, false otherwise.</returns>
    public static bool RegisterStartup()
    {
        try
        {
            var exePath = Process.GetCurrentProcess().MainModule?.FileName;
            if (string.IsNullOrEmpty(exePath))
            {
                Debug.WriteLine("Failed to get executable path");
                return false;
            }

            using (var key = Registry.CurrentUser.OpenSubKey(RegistryPath, writable: true))
            {
                if (key == null)
                {
                    Debug.WriteLine("Failed to open registry key");
                    return false;
                }

                // Set the registry value
                key.SetValue(AppName, exePath);
                Debug.WriteLine($"Registered startup for: {exePath}");
                return true;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error registering startup: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Unregisters the application from Windows startup.
    /// </summary>
    /// <returns>True if successful, false otherwise.</returns>
    public static bool UnregisterStartup()
    {
        try
        {
            using (var key = Registry.CurrentUser.OpenSubKey(RegistryPath, writable: true))
            {
                if (key == null)
                {
                    Debug.WriteLine("Failed to open registry key");
                    return false;
                }

                // Remove the registry value
                if (key.GetValue(AppName) != null)
                {
                    key.DeleteValue(AppName);
                    Debug.WriteLine("Unregistered startup");
                }

                return true;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error unregistering startup: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Checks if the application is currently registered for startup.
    /// </summary>
    /// <returns>True if registered, false otherwise.</returns>
    public static bool IsStartupRegistered()
    {
        try
        {
            using (var key = Registry.CurrentUser.OpenSubKey(RegistryPath))
            {
                if (key == null)
                    return false;

                var value = key.GetValue(AppName);
                return value != null;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error checking startup registration: {ex.Message}");
            return false;
        }
    }
}
