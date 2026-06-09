using CodexExpensa.Navigation.Abstractions;
using WebsitesAddin;
using Xunit;

namespace WebsitesAddin.Tests;

public sealed class WebsitesAddinExtensionCurrentShapeTests
{
    [Fact]
    public void Extension_ImplementsTreeNodeExtension()
    {
        WebsitesAddinExtension extension =
            new();

        Assert.IsAssignableFrom<ITreeNodeExtension>(extension);
    }

    [Fact]
    public void Extension_UsesExpectedIdentity()
    {
        WebsitesAddinExtension extension =
            new();

        Assert.Equal("WebsitesAddin", extension.ExtensionKey);
        Assert.Equal(100, extension.SortOrder);
    }
}
