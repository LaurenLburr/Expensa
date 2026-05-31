using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.Websites;

public sealed class WebsitesDatabasePanelDiagnosticsLayoutTests
{
    [Fact]
    public void DiagnosticsLayoutPartial_PositionsToggleAtRuntime()
    {
        string repositoryRoot =
            FindRepositoryRoot();

        string filePath =
            Path.Combine(
                repositoryRoot,
                "Extensions",
                "Host",
                "CodexExpensa.ExtensionDevHost",
                "CommandEngineIntegration",
                "Websites",
                "WebsitesDatabasePanelForm.DiagnosticsLayout.cs");

        Assert.True(File.Exists(filePath), $"File was not found: {filePath}");

        string text =
            File.ReadAllText(filePath);

        Assert.Contains("PositionDiagnosticsToolStrip", text);
        Assert.Contains("BringToFront", text);
        Assert.Contains("OnResize", text);
    }

    private static string FindRepositoryRoot()
    {
        DirectoryInfo? directory =
            new(AppContext.BaseDirectory);

        while (directory is not null)
        {
            if (Directory.Exists(Path.Combine(directory.FullName, "Extensions")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException("Could not find repository root containing Extensions folder.");
    }
}
