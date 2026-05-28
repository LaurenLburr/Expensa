using CodexExpensa.ExtensionDevHost.CommandEngineIntegration;
using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests;

public sealed class ExtensionOutputFolderResolverTests
{
    [Fact]
    public void GetWebsitesAddinDebugOutputFolder_ReturnsExpectedPathSuffix()
    {
        string folder =
            ExtensionOutputFolderResolver.GetWebsitesAddinDebugOutputFolder();

        Assert.EndsWith(
            Path.Combine("Modules", "WebsitesAddin", "bin", "Debug", "net8.0-windows"),
            folder,
            StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void GetWebsitesAddinReleaseOutputFolder_ReturnsExpectedPathSuffix()
    {
        string folder =
            ExtensionOutputFolderResolver.GetWebsitesAddinReleaseOutputFolder();

        Assert.EndsWith(
            Path.Combine("Modules", "WebsitesAddin", "bin", "Release", "net8.0-windows"),
            folder,
            StringComparison.OrdinalIgnoreCase);
    }

    [Theory]
    [InlineData("C:\\Git\\CodexExpensa\\Extensions\\Modules", true)]
    [InlineData("C:\\Git\\CodexExpensa\\Extensions\\Modules\\", true)]
    [InlineData("C:\\Git\\CodexExpensa\\Extensions\\Modules\\WebsitesAddin", false)]
    public void IsModulesFolder_ReturnsExpectedResult(
        string folderPath,
        bool expected)
    {
        Assert.Equal(
            expected,
            ExtensionOutputFolderResolver.IsModulesFolder(folderPath));
    }
}
