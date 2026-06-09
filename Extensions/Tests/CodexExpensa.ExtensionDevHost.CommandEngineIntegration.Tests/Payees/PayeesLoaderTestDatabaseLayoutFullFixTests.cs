using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.Payees;

public sealed class PayeesLoaderTestDatabaseLayoutFullFixTests
{
    [Fact]
    public void LoaderTestForm_DropdownAndSelectedKindIncludePayees()
    {
        string formText = ReadFile(
            "Host",
            "CodexExpensa.ExtensionDevHost",
            "CommandEngineIntegration",
            "ExpensaLoader",
            "ExpensaAddinLoaderTestForm.cs");

        string designerText = ReadFile(
            "Host",
            "CodexExpensa.ExtensionDevHost",
            "CommandEngineIntegration",
            "ExpensaLoader",
            "ExpensaAddinLoaderTestForm.Designer.cs");

        Assert.Contains("\"Payees\"", designerText);
        Assert.Contains("ExpensaAddinLoaderKind.Payees", formText);
    }

    [Fact]
    public void LoaderService_LoadAllIncludesPayees()
    {
        string text = ReadFile(
            "Host",
            "CodexExpensa.ExtensionDevHost",
            "CommandEngineIntegration",
            "ExpensaLoader",
            "ExpensaAddinTreeViewLoaderService.cs");

        Assert.Contains("ExpensaAddinLoaderKind.Websites", text);
        Assert.Contains("ExpensaAddinLoaderKind.Budgets", text);
        Assert.Contains("ExpensaAddinLoaderKind.Payees", text);
    }

    [Fact]
    public void PayeesDatabasePage_UsesHorizontalSplit()
    {
        string text = ReadFile(
            "Host",
            "CodexExpensa.ExtensionDevHost",
            "CommandEngineIntegration",
            "Payees",
            "PayeesDatabasePanelForm.Designer.cs");

        Assert.Contains("mainSplitContainer.Orientation = Orientation.Horizontal", text);
        Assert.Contains("mainSplitContainer.SplitterDistance = 220", text);
    }

    [Fact]
    public void MainForm_ExpensaLoaderTestOpensOnSelection()
    {
        string text = ReadFile(            "Host",
            "CodexExpensa.ExtensionDevHost",
            "MainForm.cs");

        Assert.Contains("case \"Tools.ExpensaAddinLoaderTest\":", text);
        Assert.Contains("ShowEmbeddedForm(new ExpensaAddinLoaderTestForm())", text);
    }

    private static string ReadFile(params string[] parts)
    {
        string repositoryRoot = FindRepositoryRoot();
        string path = Path.Combine([repositoryRoot, .. parts]);

        Assert.True(File.Exists(path), $"File was not found: {path}");

        return File.ReadAllText(path);
    }

    private static string FindRepositoryRoot()
    {
        return TestPathHelper.ExtensionsRoot;
    }
}
