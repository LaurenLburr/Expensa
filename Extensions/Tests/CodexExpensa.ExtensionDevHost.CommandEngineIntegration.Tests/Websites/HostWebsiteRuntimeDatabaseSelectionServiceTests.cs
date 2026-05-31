using CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Websites;
using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.Websites;

public sealed class HostWebsiteRuntimeDatabaseSelectionServiceTests
{
    [Fact]
    public void Service_ClassExists()
    {
        HostWebsiteRuntimeDatabaseSelectionService service = new();

        Assert.NotNull(service);
    }
}
