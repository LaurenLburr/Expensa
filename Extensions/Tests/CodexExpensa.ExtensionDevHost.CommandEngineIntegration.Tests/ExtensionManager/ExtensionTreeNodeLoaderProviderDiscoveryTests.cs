using CodexExpensa.ExtensionDevHost.CommandEngineIntegration.ExtensionManager;
using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.ExtensionManager;

public sealed class ExtensionTreeNodeLoaderProviderDiscoveryTests
{
    [Fact]
    public void DiscoverProviders_FindsProviderInAssembly()
    {
        ExtensionTreeNodeLoaderProviderDiscovery discovery = new();

        IReadOnlyList<IExtensionTreeNodeLoaderProvider> providers =
            discovery.DiscoverProviders([typeof(TestProvider).Assembly]);

        Assert.Contains(providers, provider => provider is TestProvider);
    }

    private sealed class TestProvider : IExtensionTreeNodeLoaderProvider
    {
        public IReadOnlyList<IExtensionTreeNodeLoader> GetLoaders()
        {
            return [];
        }
    }
}
