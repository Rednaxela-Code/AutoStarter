
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

Application.RunApplication(appName, installedApps);