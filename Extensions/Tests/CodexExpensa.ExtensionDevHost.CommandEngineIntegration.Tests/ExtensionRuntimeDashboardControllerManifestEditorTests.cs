using CodexExpensa.ExtensionDevHost.CommandEngineIntegration;
using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests;

public sealed class ExtensionRuntimeDashboardControllerManifestEditorTests
{
    [Fact]
    public void SetManifestEnabled_CallsEditorService()
    {
        FakeManifestEditorService editor = new();

        ExtensionRuntimeDashboardController controller =
            new(
                new ExtensionRuntimeManager(),
                new FakeManifestRegistryService(),
                editor);

        ExtensionManifestUpdateResult result =
            controller.SetManifestEnabled("C:\\Temp\\extension.json", enabled: false);

        Assert.True(result.Success);
        Assert.Equal("C:\\Temp\\extension.json", editor.LastManifestPath);
        Assert.False(editor.LastEnabled);
    }

    [Fact]
    public void SetManifestDisplaySort_CallsEditorService()
    {
        FakeManifestEditorService editor = new();

        ExtensionRuntimeDashboardController controller =
            new(
                new ExtensionRuntimeManager(),
                new FakeManifestRegistryService(),
                editor);

        ExtensionManifestUpdateResult result =
            controller.SetManifestDisplaySort("C:\\Temp\\extension.json", displaySort: 250);

        Assert.True(result.Success);
        Assert.Equal("C:\\Temp\\extension.json", editor.LastManifestPath);
        Assert.Equal(250, editor.LastDisplaySort);
    }

    private sealed class FakeManifestRegistryService : IExtensionManifestRegistryService
    {
        public ExtensionManifestRegistrySnapshot Discover(
            string folderPath,
            bool recursive = false)
        {
            return new ExtensionManifestRegistrySnapshot();
        }
    }

    private sealed class FakeManifestEditorService : IExtensionManifestEditorService
    {
        public string LastManifestPath { get; private set; } = string.Empty;

        public bool LastEnabled { get; private set; }

        public int LastDisplaySort { get; private set; }

        public ExtensionManifestUpdateResult SetEnabled(
            string manifestPath,
            bool enabled)
        {
            LastManifestPath = manifestPath;
            LastEnabled = enabled;

            return new ExtensionManifestUpdateResult
            {
                Success = true,
                Message = "Updated."
            };
        }

        public ExtensionManifestUpdateResult SetDisplaySort(
            string manifestPath,
            int displaySort)
        {
            LastManifestPath = manifestPath;
            LastDisplaySort = displaySort;

            return new ExtensionManifestUpdateResult
            {
                Success = true,
                Message = "Updated."
            };
        }
    }
}
