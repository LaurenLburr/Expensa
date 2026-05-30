using System.Windows.Forms;
using CodexExpensa.ExtensionDevHost.CommandEngineIntegration.ExtensionManager;
using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.ExtensionManager;

public sealed class ModuleCommandTreeNodeLoaderTests
{
    [Fact]
    public void Properties_ReturnDescriptorValues()
    {
        ExtensionTreeContributionDescriptor descriptor = new()
        {
            AddinId = "websites",
            DisplayName = "Websites",
            SortOrder = 100,
            CommandName = "websites.load",
            RootNodeName = "websites",
            RootDisplayText = "Websites"
        };

        ModuleCommandTreeNodeLoader loader = new(descriptor);

        Assert.Equal("websites", loader.AddinId);
        Assert.Equal("Websites", loader.DisplayName);
        Assert.Equal(100, loader.SortOrder);
    }

    [Fact]
    public async Task LoadNodeAsync_WhenCommandUnsupported_Throws()
    {
        ExtensionTreeContributionDescriptor descriptor = new()
        {
            AddinId = "bad",
            DisplayName = "Bad",
            SortOrder = 999,
            CommandName = "bad.load",
            RootNodeName = "bad",
            RootDisplayText = "Bad"
        };

        using TreeView treeView = new();

        ModuleCommandTreeNodeLoader loader = new(descriptor);

        await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await loader.LoadNodeAsync(treeView));
    }
}
