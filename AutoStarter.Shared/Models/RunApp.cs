using System.Diagnostics;
using Microsoft.Win32;
#pragma warning disable CA1416

namespace AutoStarter.Shared.Models;

public class RunApp
{
    public string? DisplayName { get; private set; }
    private string? Path { get; set; }
    public string? InstallationLocation { get; set; }

    private RunApp()
    {
    }
    
    public static List<RunApp> GetInstalledApplications()
    {
        var applications = new List<RunApp>();
 
        var registryKeys = new[]
        {
            @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall",
            @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall"
        };

        foreach (var registryKey in registryKeys)
        {
            using var key = Registry.LocalMachine.OpenSubKey(registryKey);
            if (key == null) continue;

            foreach (var subkeyName in key.GetSubKeyNames())
            {
                RunApp app = new();
                using var subkey = key.OpenSubKey(subkeyName);
                if (subkey == null) continue;
                app.DisplayName = subkey.GetValue("DisplayName") as string;
                app.InstallationLocation = subkey.GetValue("InstallLocation") as string;
                app.Path = subkey.GetValue("DisplayIcon") as string;

                if (!string.IsNullOrEmpty(app.DisplayName) && !string.IsNullOrEmpty(app.Path))
                {
                    applications.Add(app);
                }
            }
        }

        return applications;
    }
    
    public static void RunApplication(string? appName, List<RunApp> applications)
    {
        if (appName == null) return;
        foreach (var app in applications
                     .Where(app => app.DisplayName!.Contains(appName, StringComparison.OrdinalIgnoreCase)))
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(app.Path))
                {
                    Process.Start(app.Path!);
                    Console.WriteLine($"{appName} is started!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error with starting of {appName}: {ex.Message}");
            }

            break;
        }
    }
}
#pragma warning restore CA1416