using CodexExpensa.ExtensionDevHost.CommandEngineIntegration;
using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests;

public sealed class ExtensionRuntimeDashboardSettingsStoreTests
{
    [Fact]
    public void SaveThenLoad_ReturnsSettings()
    {
        string settingsPath =
            Path.Combine(
                Path.GetTempPath(),
                "CommandEngineDashboardSettingsTests",
                Guid.NewGuid().ToString("N"),
                "settings.json");

        JsonExtensionRuntimeDashboardSettingsStore store =
            new(settingsPath);

        store.Save(new ExtensionRuntimeDashboardSettings
        {
            FolderPath = "C:\\Temp\\Extensions",
            Recursive = true
        });

        ExtensionRuntimeDashboardSettings settings =
            store.Load();

        Assert.Equal("C:\\Temp\\Extensions", settings.FolderPath);
        Assert.True(settings.Recursive);
    }

    [Fact]
    public void Load_WhenFileMissing_ReturnsDefaultSettings()
    {
        string settingsPath =
            Path.Combine(
                Path.GetTempPath(),
                "CommandEngineDashboardSettingsTests",
                Guid.NewGuid().ToString("N"),
                "settings.json");

        JsonExtensionRuntimeDashboardSettingsStore store =
            new(settingsPath);

        ExtensionRuntimeDashboardSettings settings =
            store.Load();

        Assert.Equal(string.Empty, settings.FolderPath);
        Assert.False(settings.Recursive);
    }
}
