using CodexExpensa.ExtensionDevHost.CommandEngineIntegration;
using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests;

public sealed class ExtensionManifestEditorServiceTests
{
    [Fact]
    public void SetEnabled_WhenManifestExists_UpdatesEnabledFlag()
    {
        string folder =
            Path.Combine(
                Path.GetTempPath(),
                "ManifestEditorTests",
                Guid.NewGuid().ToString("N"));

        Directory.CreateDirectory(folder);

        string manifestPath =
            Path.Combine(folder, "extension.json");

        ExtensionManifestWriter.Write(
            manifestPath,
            ExtensionManifestTemplate.CreateForWebsitesAddin());

        ExtensionManifestEditorService editor = new();

        ExtensionManifestUpdateResult result =
            editor.SetEnabled(manifestPath, enabled: false);

        Assert.True(result.Success);
        Assert.NotNull(result.Manifest);
        Assert.False(result.Manifest.Enabled);

        ExtensionManifestLoadResult loadResult =
            new ExtensionManifestLoader().Load(manifestPath);

        Assert.True(loadResult.Success);
        Assert.NotNull(loadResult.Manifest);
        Assert.False(loadResult.Manifest.Enabled);
    }

    [Fact]
    public void SetEnabled_WhenManifestMissing_ReturnsFailure()
    {
        ExtensionManifestEditorService editor = new();

        ExtensionManifestUpdateResult result =
            editor.SetEnabled(
                Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"), "extension.json"),
                enabled: false);

        Assert.False(result.Success);
        Assert.Contains("Manifest file was not found", result.Message);
    }

    [Fact]
    public void RegistryService_IncludesManifestPath()
    {
        string folder =
            Path.Combine(
                Path.GetTempPath(),
                "ManifestEditorTests",
                Guid.NewGuid().ToString("N"));

        Directory.CreateDirectory(folder);

        string manifestPath =
            Path.Combine(folder, "extension.json");

        ExtensionManifestWriter.Write(
            manifestPath,
            ExtensionManifestTemplate.CreateForWebsitesAddin());

        ExtensionManifestRegistrySnapshot snapshot =
            new ExtensionManifestRegistryService().Discover(folder);

        Assert.Single(snapshot.Records);
        Assert.Equal(manifestPath, snapshot.Records[0].ManifestPath);
    }
}
