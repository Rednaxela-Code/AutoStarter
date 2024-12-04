using Microsoft.Win32;
#pragma warning disable CA1416

namespace AutoStarter.Shared.Models;

public class Application
{
    public string? DisplayName { get; set; }
    public string? Path { get; set; }
    public string? InstallationLocation { get; set; }

    public Application()
    {
    }
    
    public static List<Application> GetInstalledApplications()
    {
        var applications = new List<Application>();
 
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
                Application app = new();
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
}
#pragma warning restore CA1416