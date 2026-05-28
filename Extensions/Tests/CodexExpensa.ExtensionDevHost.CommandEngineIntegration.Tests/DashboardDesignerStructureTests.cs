using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests;

public sealed class DashboardDesignerStructureTests
{
    private static readonly string RepositoryRoot = FindRepositoryRoot();

    [Fact]
    public void DashboardForm_CodeBehind_DoesNotBuildMenuAtRuntime()
    {
        string formPath = Path.Combine(
            RepositoryRoot,
            "Extensions",
            "Host",
            "CodexExpensa.ExtensionDevHost",
            "CommandEngineIntegration",
            "ExtensionRuntimeDashboardForm.cs");

        Assert.True(File.Exists(formPath), $"File was not found: {formPath}");

        string text = File.ReadAllText(formPath);

        Assert.DoesNotContain("new MenuStrip", text, StringComparison.Ordinal);
        Assert.DoesNotContain("new ToolStripMenuItem", text, StringComparison.Ordinal);
        Assert.DoesNotContain("private void BuildMenu(", text, StringComparison.Ordinal);
        Assert.DoesNotContain("private void BuildLayout(", text, StringComparison.Ordinal);
    }

    [Fact]
    public void DashboardForm_Designer_OwnsInitializeComponent()
    {
        string designerPath = Path.Combine(
            RepositoryRoot,
            "Extensions",
            "Host",
            "CodexExpensa.ExtensionDevHost",
            "CommandEngineIntegration",
            "ExtensionRuntimeDashboardForm.Designer.cs");

        Assert.True(File.Exists(designerPath), $"File was not found: {designerPath}");

        string text = File.ReadAllText(designerPath);

        Assert.Contains("private void InitializeComponent()", text, StringComparison.Ordinal);
        Assert.Contains("new MenuStrip", text, StringComparison.Ordinal);
        Assert.Contains("historyMenuItem", text, StringComparison.Ordinal);
        Assert.Contains("commandListView", text, StringComparison.Ordinal);
        Assert.Contains("diagnosticsTextBox", text, StringComparison.Ordinal);
    }

    [Fact]
    public void DashboardForm_Resx_Exists()
    {
        string resxPath = Path.Combine(
            RepositoryRoot,
            "Extensions",
            "Host",
            "CodexExpensa.ExtensionDevHost",
            "CommandEngineIntegration",
            "ExtensionRuntimeDashboardForm.resx");

        Assert.True(File.Exists(resxPath), $"File was not found: {resxPath}");
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

        DirectoryInfo? currentDirectory = new(Directory.GetCurrentDirectory());

        while (currentDirectory is not null)
        {
            if (Directory.Exists(Path.Combine(currentDirectory.FullName, "Extensions")))
            {
                return currentDirectory.FullName;
            }

            currentDirectory = currentDirectory.Parent;
        }

        throw new DirectoryNotFoundException(
            "Could not find repository root containing the Extensions folder.");
    }
}
