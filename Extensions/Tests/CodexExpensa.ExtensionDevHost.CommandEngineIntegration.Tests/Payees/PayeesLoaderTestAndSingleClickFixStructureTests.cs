using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.Payees;

public sealed class PayeesLoaderTestAndSingleClickFixStructureTests
{
    [Fact]
    public void LoaderTestForm_IncludesPayeesKind()
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

        Assert.Contains("Payees", designerText);
        Assert.Contains("ExpensaAddinLoaderKind.Payees", formText);
    }

    [Fact]
    public void MainForm_LoaderTestNodeCanOpenOnSelection()
    {
        string text = ReadFile(
            "Host",
            "CodexExpensa.ExtensionDevHost",
            "MainForm.cs");

        Assert.Contains("Tools.ExpensaAddinLoaderTest", text);
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
