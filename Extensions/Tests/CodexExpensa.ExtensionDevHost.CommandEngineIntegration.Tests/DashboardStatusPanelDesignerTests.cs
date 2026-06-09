using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests;

public sealed class DashboardStatusPanelDesignerTests
{
    [Fact]
    public void DashboardDesigner_IncludesStatusDetailsLabel()
    {
        string repositoryRoot =
            FindRepositoryRoot();

        string designerPath =
            Path.Combine(
                repositoryRoot,
                "Host",
                "CodexExpensa.ExtensionDevHost",
                "CommandEngineIntegration",
                "ExtensionRuntimeDashboardForm.Designer.cs");

        Assert.True(File.Exists(designerPath), $"File was not found: {designerPath}");

        string text =
            File.ReadAllText(designerPath);

        Assert.Contains("statusDetailsLabel", text, StringComparison.Ordinal);
        Assert.Contains("Mode: Runtime", text, StringComparison.Ordinal);
    }

    private static string FindRepositoryRoot()
    {
        return TestPathHelper.ExtensionsRoot;
    }
}
