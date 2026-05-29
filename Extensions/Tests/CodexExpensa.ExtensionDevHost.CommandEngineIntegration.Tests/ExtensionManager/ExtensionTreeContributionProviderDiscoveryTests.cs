using CodexExpensa.ExtensionDevHost.CommandEngineIntegration.ExtensionManager;
using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.ExtensionManager;

public sealed class ExtensionTreeContributionProviderDiscoveryTests
{
    [Fact]
    public void DiscoverDescriptors_FindsConventionProvider()
    {
        ExtensionTreeContributionProviderDiscovery discovery = new();

        IReadOnlyList<ExtensionTreeContributionDescriptor> descriptors =
            discovery.DiscoverDescriptors([typeof(TestTreeContributionProvider).Assembly]);

        Assert.Contains(descriptors, descriptor => descriptor.AddinId == "test");
    }

    public sealed class TestTreeContributionProvider
    {
        public string AddinId => "test";
        public string DisplayName => "Test";
        public int SortOrder => 10;
        public string CommandName => "test.load";
        public string RootNodeName => "test";
        public string RootDisplayText => "Test";
    }
}
