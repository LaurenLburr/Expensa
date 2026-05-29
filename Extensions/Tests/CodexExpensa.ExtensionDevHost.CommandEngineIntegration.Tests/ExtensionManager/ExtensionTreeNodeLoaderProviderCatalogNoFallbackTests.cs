using CodexExpensa.ExtensionDevHost.CommandEngineIntegration.ExtensionManager;
using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.ExtensionManager;

public sealed class ExtensionTreeNodeLoaderProviderCatalogNoFallbackTests
{
    [Fact]
    public void GetProviders_WhenModulesFolderEmpty_ReturnsEmpty()
    {
        string folder =
            Path.Combine(
                Path.GetTempPath(),
                "ExtensionTreeNodeLoaderProviderCatalogNoFallbackTests",
                Guid.NewGuid().ToString("N"));

        Directory.CreateDirectory(folder);

        ExtensionTreeNodeLoaderProviderCatalog catalog = new(folder);

        IReadOnlyList<IExtensionTreeNodeLoaderProvider> providers =
            catalog.GetProviders();

        Assert.Empty(providers);
    }
}
