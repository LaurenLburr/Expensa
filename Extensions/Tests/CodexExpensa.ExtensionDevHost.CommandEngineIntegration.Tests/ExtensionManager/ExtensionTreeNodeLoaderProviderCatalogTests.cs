using CodexExpensa.ExtensionDevHost.CommandEngineIntegration.ExtensionManager;
using CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Websites;
using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.ExtensionManager;

public sealed class ExtensionTreeNodeLoaderProviderCatalogTests
{
    [Fact]
    public void GetProviders_IncludesWebsitesProvider()
    {
        ExtensionTreeNodeLoaderProviderCatalog catalog = new();

        IReadOnlyList<IExtensionTreeNodeLoaderProvider> providers =
            catalog.GetProviders();

        Assert.Contains(providers, provider => provider is WebsitesExtensionTreeNodeLoaderProvider);
    }
}
