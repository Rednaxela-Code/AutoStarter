namespace AutoStarter.Shared.Models;

public class Application
{
    public string? DisplayName { get; set; }
    public string? Path { get; set; }
    public string? InstallationLocation { get; set; }

    public Application()
    {
    }
}