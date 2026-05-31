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

        Assert.Equal("websitesaddin.db", location.DatabaseName);
        Assert.EndsWith(
            Path.Combine("Expensa", "Extensions", "Runtime", "WebsitesAddin", "websitesaddin.db"),
            location.DatabasePath,
            StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void GenerateTimestampedDatabaseFileName_IncludesSourceLabelAndDbExtension()
    {
        HostWebsiteDatabasePathService service = new();

        string fileName =
            service.GenerateTimestampedDatabaseFileName("sandbox db");

        Assert.StartsWith("websitesaddin.sandbox_db.", fileName, StringComparison.OrdinalIgnoreCase);
        Assert.EndsWith(".db", fileName, StringComparison.OrdinalIgnoreCase);
    }
}
