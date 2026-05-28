using CodexExpensa.ExtensionDevHost.CommandEngineIntegration;
using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests;

public sealed class ExtensionRuntimeDashboardControllerManifestRegistryTests
{
    [Fact]
    public void DiscoverManifestRegistry_ReturnsRegistryViewModel()
    {
        ExtensionRuntimeDashboardController controller =
            new(
                new ExtensionRuntimeManager(),
                new FakeManifestRegistryService());

        ExtensionManifestRegistryViewModel viewModel =
            controller.DiscoverManifestRegistry("C:\\Temp", recursive: true);

        Assert.Equal(1, viewModel.TotalCount);
        Assert.Equal("Fake", viewModel.Records[0].ExtensionId);
        Assert.Contains("1 manifest", viewModel.Summary);
    }

    private sealed class FakeManifestRegistryService : IExtensionManifestRegistryService
    {
        public ExtensionManifestRegistrySnapshot Discover(
            string folderPath,
            bool recursive = false)
        {
            return new ExtensionManifestRegistrySnapshot
            {
                Records =
                [
                    new ExtensionManifestRecord
                    {
                        ExtensionId = "Fake",
                        DisplayName = "Fake Extension",
                        Version = "1.0.0",
                        Enabled = true,
                        AssemblyFile = "Fake.dll",
                        ProviderType = "Fake.Provider",
                        ManifestFolder = folderPath
                    }
                ]
            };
        }
    }
}
