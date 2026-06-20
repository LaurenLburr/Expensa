using CodexExpensa.ExtensionDevHost.CommandEngineIntegration;
using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests;

public sealed class ExtensionManifestRegistryServiceTests
{
    [Fact]
    public void Discover_WhenManifestExists_ReturnsRegistryRecord()
    {
        string folder = Path.Combine(Path.GetTempPath(), "ManifestRegistryTests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(folder);

        ExtensionManifestWriter.Write(
            Path.Combine(folder, "extension.json"),
            ExtensionManifestTemplate.CreateForWebsitesAddin());

        ExtensionManifestRegistryService service = new();

        ExtensionManifestRegistrySnapshot snapshot = service.Discover(folder);

        Assert.Single(snapshot.Records);
        Assert.Equal("WebsitesAddin", snapshot.Records[0].ExtensionId);
        Assert.Equal(100, snapshot.Records[0].DisplaySort);
        Assert.Equal(1, snapshot.EnabledCount);
        Assert.Equal(0, snapshot.DisabledCount);
        Assert.Empty(snapshot.Errors);
    }

    [Fact]
    public void CreateViewModel_OrdersRecordsAndBuildsSummary()
    {
        ExtensionManifestRegistrySnapshot snapshot = new()
        {
            Records =
            [
                new ExtensionManifestRecord { ExtensionId = "z", DisplayName = "Zed", Enabled = true },
                new ExtensionManifestRecord { ExtensionId = "a", DisplayName = "Alpha", DisplaySort = 20, Enabled = false },
                new ExtensionManifestRecord { ExtensionId = "b", DisplayName = "Beta", DisplaySort = 10, Enabled = true }
            ],
            Errors = ["bad manifest"]
        };

        ExtensionManifestRegistryViewModel viewModel = ExtensionManifestRegistryViewModelFactory.Create(snapshot);

        Assert.Equal(3, viewModel.TotalCount);
        Assert.Equal(2, viewModel.EnabledCount);
        Assert.Equal(1, viewModel.DisabledCount);
        Assert.Equal(1, viewModel.ErrorCount);
        Assert.Equal("z", viewModel.Records[0].ExtensionId);
        Assert.Equal("b", viewModel.Records[1].ExtensionId);
        Assert.Contains("3 manifest", viewModel.Summary);
    }

    [Fact]
    public void Format_ReturnsManifestText()
    {
        ExtensionManifestRegistryViewModel viewModel =
            ExtensionManifestRegistryViewModelFactory.Create(new ExtensionManifestRegistrySnapshot
            {
                Records =
                [
                    new ExtensionManifestRecord
                    {
                        ExtensionId = "WebsitesAddin",
                        DisplayName = "Websites Add-in",
                        Version = "1.0.0",
                        AssemblyFile = "WebsitesAddin.dll",
                        ProviderType = "WebsitesAddin.WebsitesAddinCommandProvider",
                        Enabled = true
                    }
                ]
            });

        string text = ExtensionManifestRegistryTextFormatter.Format(viewModel);

        Assert.Contains("Extension Manifest Registry", text);
        Assert.Contains("WebsitesAddin", text);
        Assert.Contains("WebsitesAddin.dll", text);
    }

    [Fact]
    public void DashboardForm_ProvidesDisplaySortEditor()
    {
        string formText =
            TestPathHelper.ReadHostFile(
                "CodexExpensa.ExtensionDevHost",
                "CommandEngineIntegration",
                "ExtensionRuntimeDashboardForm.cs");

        string designerText =
            TestPathHelper.ReadHostFile(
                "CodexExpensa.ExtensionDevHost",
                "CommandEngineIntegration",
                "ExtensionRuntimeDashboardForm.Designer.cs");

        Assert.Contains("SetSelectedManifestDisplaySort", formText);
        Assert.Contains("NumericUpDown", formText);
        Assert.Contains("Display Sort", formText);
        Assert.Contains("setManifestDisplaySortMenuItem", designerText);
        Assert.Contains("Set Display &Sort", designerText);
    }
}
