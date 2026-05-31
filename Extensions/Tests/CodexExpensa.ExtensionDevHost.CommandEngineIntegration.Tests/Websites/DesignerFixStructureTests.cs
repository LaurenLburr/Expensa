using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.Websites;

public sealed class DesignerFixStructureTests
{
    [Fact]
    public void DatabaseConnectionPanelDesigner_DoesNotUseLocalFlowLayoutPanelDeclarations()
    {
        string repositoryRoot = FindRepositoryRoot();

        string designerPath =
            Path.Combine(
                repositoryRoot,
                "Extensions",
                "Host",
                "CodexExpensa.ExtensionDevHost",
                "UI",
                "DatabaseConnectionPanelForm.Designer.cs");

        Assert.True(File.Exists(designerPath), $"File was not found: {designerPath}");

        string text = File.ReadAllText(designerPath);

        Assert.DoesNotContain("FlowLayoutPanel modePanel = new()", text);
        Assert.DoesNotContain("FlowLayoutPanel buttonPanel = new()", text);
        Assert.Contains("private FlowLayoutPanel modePanel;", text);
        Assert.Contains("private FlowLayoutPanel buttonPanel;", text);
    }

    [Fact]
    public void WebsitesDatabasePanelDesigner_IncludesControlNameToggle()
    {
        string repositoryRoot = FindRepositoryRoot();

        string designerPath =
            Path.Combine(
                repositoryRoot,
                "Extensions",
                "Host",
                "CodexExpensa.ExtensionDevHost",
                "CommandEngineIntegration",
                "Websites",
                "WebsitesDatabasePanelForm.Designer.cs");

        Assert.True(File.Exists(designerPath), $"File was not found: {designerPath}");

        string text = File.ReadAllText(designerPath);

        Assert.Contains("toggleControlNamesButton", text);
        Assert.Contains("diagnosticsToolStrip", text);
    }

    private static string FindRepositoryRoot()
    {
        DirectoryInfo? directory = new(AppContext.BaseDirectory);

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
