using CodexExpensa.ExtensionDevHost.CommandEngineIntegration;
using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests;

public sealed class RuntimeProviderManifestDiscoveryServiceTests
{
    [Fact]
    public void DiscoverProvidersFromFolder_WhenFolderMissing_ReturnsIssue()
    {
        RuntimeProviderManifestDiscoveryService service = new();

        RuntimeProviderDiscoveryResult result =
            service.DiscoverProvidersFromFolder(
                Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N")));

        Assert.Empty(result.Providers);
        Assert.Single(result.Issues);
        Assert.Contains("Folder was not found", result.Issues[0].Message);
    }

    [Fact]
    public void DiscoverProvidersFromFolder_WhenAssemblyMissing_ReportsManifestSpecificPath()
    {
        string rootFolder =
            Path.Combine(
                Path.GetTempPath(),
                "ManifestProviderRelativeFolderTests",
                Guid.NewGuid().ToString("N"));

        string manifestFolder =
            Path.Combine(rootFolder, "Modules", "Example", "bin", "Debug", "net8.0-windows");

        Directory.CreateDirectory(manifestFolder);

        ExtensionManifestWriter.Write(
            Path.Combine(manifestFolder, "extension.json"),
            new ExtensionManifest
            {
                ExtensionId = "Example",
                DisplayName = "Example",
                Version = "1.0.0",
                AssemblyFile = "Missing.dll",
                ProviderType = "Example.Provider",
                Enabled = true
            });

        RuntimeProviderManifestDiscoveryService service = new();

        RuntimeProviderDiscoveryResult result =
            service.DiscoverProvidersFromFolder(rootFolder, recursive: true);

        Assert.Empty(result.Providers);
        Assert.Single(result.Issues);
        Assert.Contains("Missing.dll", result.Issues[0].Message);
        Assert.Contains("extension.json", result.Issues[0].Source);
    }
}
