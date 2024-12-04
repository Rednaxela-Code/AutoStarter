using AutoStarter.Shared.Models;
using AutoStarter.Shared.Services;
using AutoStarter.Shared.Services.IServices;
using FluentAssertions;
using Moq;

namespace AutoStarter.Tests.ServicesTests;

public class ApplicationServiceTests
{
    [Fact]
    public void GetInstalledApplications_ShouldReturnApplications_WhenRegistryHasValidEntries()
    {
        // Arrange
        var mockRegistry = new Mock<IRegistry>();
        var mockProcess = new Mock<IProcess>();
        var registryData = new Dictionary<string, Dictionary<string, string>>
        {
            {
                @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall", new Dictionary<string, string>
                {
                    { "SubKey1", @"TestApp1|C:\TestPath1|C:\TestPath1\app.exe" },
                    { "SubKey2", @"TestApp2|C:\TestPath2|C:\TestPath2\app.exe" }
                }
            },
            {
                @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall", new Dictionary<string, string>
                {
                    { "SubKey3", @"TestApp3|C:\TestPath3|C:\TestPath3\app.exe" }
                }
            }
        };

        // Simulate Registry Key-Value retrieval
        mockRegistry.Setup(r => r.GetRegistryData(It.IsAny<string>()))
            .Returns((string key) => registryData.ContainsKey(key) ? registryData[key] : null);

        var service = new ApplicationService(mockRegistry.Object, mockProcess.Object);

        // Act
        var applications = service.GetInstalledApplications();

        // Assert
        applications.Should().NotBeNull();
        applications.Count.Should().Be(3);
        applications.Should().Contain(a => a.DisplayName == "TestApp1" && a.Path == @"C:\TestPath1\app.exe");
        applications.Should().Contain(a => a.DisplayName == "TestApp3" && a.Path == @"C:\TestPath3\app.exe");
    }
    
    [Fact]
    public void RunApplication_ShouldInvokeProcessStart_WhenAppNameMatches()
    {
        // Arrange
        var applications = new List<Application>
        {
            new Application { DisplayName = "TestApp1", Path = @"C:\TestPath1\app.exe" },
            new Application { DisplayName = "TestApp2", Path = @"C:\TestPath2\app.exe" }
        };

        var mockProcess = new Mock<IProcess>();
        var mockRegistry = new Mock<IRegistry>();
        mockProcess.Setup(p => p.Start(It.IsAny<string>()))
            .Verifiable(); // Ensure this method is called

        var service = new ApplicationService(mockRegistry.Object, mockProcess.Object);

        // Act
        service.RunApplication("TestApp1", applications);

        // Assert
        mockProcess.Verify(p => p.Start(@"C:\TestPath1\app.exe"), Times.Once);
    }


}