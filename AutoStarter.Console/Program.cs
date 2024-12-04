
using System.Diagnostics;
using AutoStarter.Shared.Models;

var installedApps = Application.GetInstalledApplications();

Console.WriteLine("Preinstalled Applications:");
foreach (var app in installedApps)
{
    Console.WriteLine(app.DisplayName);
}

Console.WriteLine("\nGive the name of the application you would like to install:");
var appName = Console.ReadLine();

if (appName != null)
{
    foreach (var app in installedApps
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