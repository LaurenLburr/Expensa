using CodexExpensa.ExtensionDevHost.CommandEngineIntegration;
using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests;

public sealed class ExtensionManifestDiscoveryServiceTests
{
    [Fact]
    public void DiscoverFromFolder_FindsManifest()
    {
        string folder =
            Path.Combine(
                Path.GetTempPath(),
                Guid.NewGuid().ToString("N"));

        Directory.CreateDirectory(folder);

        ExtensionManifestWriter.Write(
            Path.Combine(folder, "extension.json"),
            ExtensionManifestTemplate.CreateForWebsitesAddin());

        ExtensionManifestDiscoveryService service = new();

        ExtensionManifestDiscoveryResult result =
            service.DiscoverFromFolder(folder);

        Assert.True(result.Success);
        Assert.Single(result.Manifests);
    }
}
