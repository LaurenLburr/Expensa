using System.Windows.Forms;
using CodexExpensa.ExtensionDevHost.CommandEngineIntegration.ExtensionManager;
using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.ExtensionManager;

public sealed class ExtensionTreeNodeLoaderRegistryTests
{
    [Fact]
    public void GetLoaders_ReturnsLoadersSortedBySortOrder()
    {
        ExtensionTreeNodeLoaderRegistry registry =
            new(
                [
                    new TestProvider(
                    [
                        new TestLoader("second", "Second", 200),
                        new TestLoader("first", "First", 100)
                    ])
                ]);

        IReadOnlyList<IExtensionTreeNodeLoader> loaders =
            registry.GetLoaders();

        Assert.Equal(2, loaders.Count);
        Assert.Equal("first", loaders[0].AddinId);
        Assert.Equal("second", loaders[1].AddinId);
    }

    [Fact]
    public void GetLoaders_WhenMultipleProviders_FlattensAllLoaders()
    {
        ExtensionTreeNodeLoaderRegistry registry =
            new(
                [
                    new TestProvider([new TestLoader("one", "One", 100)]),
                    new TestProvider([new TestLoader("two", "Two", 200)])
                ]);

        IReadOnlyList<IExtensionTreeNodeLoader> loaders =
            registry.GetLoaders();

        Assert.Equal(2, loaders.Count);
    }

    private sealed class TestProvider : IExtensionTreeNodeLoaderProvider
    {
        private readonly IReadOnlyList<IExtensionTreeNodeLoader> _loaders;

        public TestProvider(
            IReadOnlyList<IExtensionTreeNodeLoader> loaders)
        {
            _loaders = loaders;
        }

        public IReadOnlyList<IExtensionTreeNodeLoader> GetLoaders()
        {
            return _loaders;
        }
    }

    private sealed class TestLoader : IExtensionTreeNodeLoader
    {
        public TestLoader(
            string addinId,
            string displayName,
            int sortOrder)
        {
            AddinId = addinId;
            DisplayName = displayName;
            SortOrder = sortOrder;
        }

        public string AddinId { get; }

        public string DisplayName { get; }

        public int SortOrder { get; }

        public Task LoadNodeAsync(
            TreeView treeView,
            CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }
}
