using AutoStarter.Shared.Models;

namespace AutoStarter.Shared.Services.IServices;

public interface IApplicationService
{
    List<Application> GetInstalledApplications();
    void RunApplication(string? appName, List<Application> applications);
}

public interface IRegistry
{
    Dictionary<string, string> GetRegistryData(string key);
}

public interface IProcess
{
    void Start(string path);
}