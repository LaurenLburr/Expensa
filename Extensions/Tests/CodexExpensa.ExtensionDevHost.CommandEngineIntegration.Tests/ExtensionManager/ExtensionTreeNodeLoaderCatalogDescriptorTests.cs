using CodexExpensa.ExtensionDevHost.CommandEngineIntegration.ExtensionManager;
using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.ExtensionManager;

public sealed class ExtensionTreeNodeLoaderCatalogDescriptorTests
{
    [Fact]
    public void GetLoaders_WhenModulesFolderEmpty_ReturnsEmpty()
    {
        string folder =
            Path.Combine(
                Path.GetTempPath(),
                "ExtensionTreeNodeLoaderCatalogDescriptorTests",
                Guid.NewGuid().ToString("N"));

        Directory.CreateDirectory(folder);

        ExtensionTreeNodeLoaderCatalog catalog = new(folder);

        IReadOnlyList<IExtensionTreeNodeLoader> loaders =
            catalog.GetLoaders();

        Assert.Empty(loaders);
    }
}
