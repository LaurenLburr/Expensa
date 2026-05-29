using CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Websites;
using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.Websites;

public sealed class HostWebsiteRuntimeLoadRequestTests
{
    [Fact]
    public void NewRequest_UsesExpectedDefaults()
    {
        HostWebsiteRuntimeLoadRequest request = new();

        Assert.Equal(string.Empty, request.SearchText);
        Assert.False(request.IncludeDisabled);
        Assert.Equal(500, request.MaximumRows);
    }
}
