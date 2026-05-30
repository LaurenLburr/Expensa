using System.Windows.Forms;
using CodexExpensa.ExtensionDevHost.CommandEngineIntegration.ExtensionManager;
using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.ExtensionManager;

public sealed class TreeContributionCommandAdapterRegistryTests
{
    [Fact]
    public void GetRequiredAdapter_WhenCommandExists_ReturnsAdapter()
    {
        TestAdapter adapter = new("test.load");

        TreeContributionCommandAdapterRegistry registry =
            new([adapter]);

        ITreeContributionCommandAdapter result =
            registry.GetRequiredAdapter("test.load");

        Assert.Same(adapter, result);
    }

    [Fact]
    public void GetRequiredAdapter_WhenCommandUsesDifferentCase_ReturnsAdapter()
    {
        TestAdapter adapter = new("test.load");

        TreeContributionCommandAdapterRegistry registry =
            new([adapter]);

        ITreeContributionCommandAdapter result =
            registry.GetRequiredAdapter("TEST.LOAD");

        Assert.Same(adapter, result);
    }

    [Fact]
    public void GetRequiredAdapter_WhenCommandMissing_Throws()
    {
        TreeContributionCommandAdapterRegistry registry =
            new([]);

        Assert.Throws<InvalidOperationException>(
            () => registry.GetRequiredAdapter("missing.load"));
    }

    private sealed class TestAdapter : ITreeContributionCommandAdapter
    {
        public TestAdapter(string commandName)
        {
            CommandName = commandName;
        }

        public string CommandName { get; }

        public Task LoadNodeAsync(
            TreeView treeView,
            ExtensionTreeContributionDescriptor descriptor,
            CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }
}
