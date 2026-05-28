using CodexExpensa.ExtensionDevHost.CommandEngineIntegration;
using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests;

public sealed class ExtensionManifestRegistryDuplicateTests
{
    [Fact]
    public void Discover_WhenDuplicateExtensionIds_MarksDuplicates()
    {
        string folder =
            Path.Combine(
                Path.GetTempPath(),
                "ManifestDuplicateTests",
                Guid.NewGuid().ToString("N"));

        string debugFolder =
            Path.Combine(folder, "WebsitesAddin", "bin", "Debug", "net8.0-windows");

        string releaseFolder =
            Path.Combine(folder, "WebsitesAddin", "bin", "Release", "net8.0-windows");

        Directory.CreateDirectory(debugFolder);
        Directory.CreateDirectory(releaseFolder);

        ExtensionManifestWriter.Write(
            Path.Combine(debugFolder, "extension.json"),
            ExtensionManifestTemplate.CreateForWebsitesAddin());

        ExtensionManifestWriter.Write(
            Path.Combine(releaseFolder, "extension.json"),
            ExtensionManifestTemplate.CreateForWebsitesAddin());

        ExtensionManifestRegistrySnapshot snapshot =
            new ExtensionManifestRegistryService().Discover(folder, recursive: true);

        Assert.Equal(2, snapshot.Records.Count);
        Assert.Equal(2, snapshot.DuplicateCount);
        Assert.All(snapshot.Records, static record => Assert.True(record.IsDuplicate));
    }

    [Fact]
    public void PreferSingleRecordPerExtension_PrefersDebugOverRelease()
    {
        IReadOnlyList<ExtensionManifestRecord> records =
        [
            new ExtensionManifestRecord
            {
                ExtensionId = "WebsitesAddin",
                DisplayName = "Websites",
                ManifestPath = "C:\\Test\\Release\\extension.json"
            },
            new ExtensionManifestRecord
            {
                ExtensionId = "WebsitesAddin",
                DisplayName = "Websites",
                ManifestPath = "C:\\Test\\Debug\\extension.json"
            }
        ];

        IReadOnlyList<ExtensionManifestRecord> filtered =
            ExtensionManifestRegistryFilter.PreferSingleRecordPerExtension(records);

        Assert.Single(filtered);
        Assert.Contains("Debug", filtered[0].ManifestPath);
    }

    [Fact]
    public void ViewModel_SummaryIncludesDuplicateCount()
    {
        ExtensionManifestRegistryViewModel viewModel =
            ExtensionManifestRegistryViewModelFactory.Create(new ExtensionManifestRegistrySnapshot
            {
                Records =
                [
                    new ExtensionManifestRecord
                    {
                        ExtensionId = "A",
                        DisplayName = "A",
                        IsDuplicate = true
                    }
                ]
            });

        Assert.Equal(1, viewModel.DuplicateCount);
        Assert.Contains("duplicate", viewModel.Summary);
    }
}
