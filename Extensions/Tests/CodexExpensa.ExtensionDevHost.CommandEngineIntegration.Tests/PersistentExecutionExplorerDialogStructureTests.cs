using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests;

public sealed class PersistentExecutionExplorerDialogStructureTests
{
    [Fact]
    public void Designer_IncludesExpectedFilterControls()
    {
        string repositoryRoot = FindRepositoryRoot();

        string designerPath =
            Path.Combine(
                repositoryRoot,
                "Host",
                "CodexExpensa.ExtensionDevHost",
                "CommandEngineIntegration",
                "PersistentExecutionExplorerDialog.Designer.cs");

        Assert.True(File.Exists(designerPath), $"File was not found: {designerPath}");

        string text = File.ReadAllText(designerPath);

        Assert.Contains("sourceFilterComboBox", text);
        Assert.Contains("statusFilterComboBox", text);
        Assert.Contains("searchTextBox", text);
        Assert.Contains("sortModeComboBox", text);
        Assert.Contains("maximumRowsNumericUpDown", text);
    }

    private static string FindRepositoryRoot()
    {
        return TestPathHelper.ExtensionsRoot;
    }
}
