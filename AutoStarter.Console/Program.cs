#pragma warning disable CA1416
using System.Diagnostics;
using Microsoft.Win32;

var installedApps = GetInstalledApplications();

Console.WriteLine("Preinstalled Applications:");
foreach (var app in installedApps)
{
    Console.WriteLine(app);
}

Console.WriteLine("\nGive the name of the application you would like to install:");
var appName = Console.ReadLine();

foreach (var app in installedApps
             .Where(app => app.Contains(appName!, StringComparison.OrdinalIgnoreCase)))
{
    try
    {
        Process.Start(app);
        Console.WriteLine($"{appName} is started!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error with starting of {appName}: {ex.Message}");
    }

    break;
}

return;


static List<string> GetInstalledApplications()
{
    var applications = new List<string>();
 
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
            using var subkey = key.OpenSubKey(subkeyName);
            var displayName = subkey?.GetValue("DisplayName") as string;
            //var installLocation = subkey?.GetValue("InstallLocation") as string;
            var exePath = subkey?.GetValue("DisplayIcon") as string;

            if (!string.IsNullOrEmpty(displayName) && !string.IsNullOrEmpty(exePath))
            {
                applications.Add(exePath);
            }
        }
    }

    return applications;
}
#pragma warning restore CA1416