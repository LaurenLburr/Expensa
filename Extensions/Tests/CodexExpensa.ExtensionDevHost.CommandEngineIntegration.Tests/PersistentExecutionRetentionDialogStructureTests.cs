using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests;

public sealed class PersistentExecutionRetentionDialogStructureTests
{
    [Fact]
    public void Designer_IncludesExpectedRetentionControls()
    {
        string repositoryRoot = FindRepositoryRoot();

        string designerPath =
            Path.Combine(
                repositoryRoot,
                "Host",
                "CodexExpensa.ExtensionDevHost",
                "CommandEngineIntegration",
                "PersistentExecutionRetentionDialog.Designer.cs");

        Assert.True(File.Exists(designerPath), $"File was not found: {designerPath}");

        string text = File.ReadAllText(designerPath);

        Assert.Contains("sourceFilterComboBox", text);
        Assert.Contains("statusFilterComboBox", text);
        Assert.Contains("agePresetComboBox", text);
        Assert.Contains("deleteButton", text);
        Assert.Contains("vacuumButton", text);
    }

    private static string FindRepositoryRoot()
    {
        return TestPathHelper.ExtensionsRoot;
    }
}
