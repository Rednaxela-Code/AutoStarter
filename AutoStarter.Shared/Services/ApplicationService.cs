using System.Diagnostics;
using AutoStarter.Shared.Models;
using AutoStarter.Shared.Services.IServices;
using Microsoft.Win32;

#pragma warning disable CA1416

namespace AutoStarter.Shared.Services;

public class ApplicationService : IApplicationService
{
    private readonly IRegistry _registry;
    private readonly IProcess _process;

    public ApplicationService(IRegistry registry, IProcess process)
    {
        _registry = registry;
        _process = process;
    }

    public List<Application> GetInstalledApplications()
    {
        var applications = new List<Application>();
        var registryKeys = new[]
        {
            @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall",
            @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall"
        };

        foreach (var registryKey in registryKeys)
        {
            var subKeys = _registry.GetRegistryData(registryKey);
            if (subKeys == null) continue;

            foreach (var (subKey, values) in subKeys)
            {
                var parts = values.Split('|');
                if (parts.Length < 3) continue;

                applications.Add(new Application
                {
                    DisplayName = parts[0],
                    InstallationLocation = parts[1],
                    Path = parts[2]
                });
            }
        }

        return applications;
    }

    public void RunApplication(string? appName, List<Application> applications)
    {
        if (appName == null) return;

        foreach (var app in applications
                     .Where(app => app.DisplayName!.Contains(appName, StringComparison.OrdinalIgnoreCase)))
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(app.Path))
                {
                    _process.Start(app.Path!);
                    Console.WriteLine($"{appName} is started!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error with starting {appName}: {ex.Message}");
            }

            break;
        }
    }
}

#pragma warning restore CA1416