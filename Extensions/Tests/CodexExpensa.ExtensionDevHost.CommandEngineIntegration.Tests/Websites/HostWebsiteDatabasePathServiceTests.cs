using CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Websites;
using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.Websites;

public sealed class HostWebsiteDatabasePathServiceTests
{
    [Fact]
    public void GetRuntimeDatabaseLocation_ReturnsGeneratedRuntimeLocation()
    {
        HostWebsiteDatabasePathService service = new();

        HostWebsiteDatabaseLocation location =
            service.GetRuntimeDatabaseLocation();

        Assert.Equal("websites.current.db", location.DatabaseName);
        Assert.EndsWith(
            Path.Combine("Expensa", "Extensions", "Runtime", "WebsitesAddin", "websites.current.db"),
            location.DatabasePath,
            StringComparison.OrdinalIgnoreCase);
    }


}
