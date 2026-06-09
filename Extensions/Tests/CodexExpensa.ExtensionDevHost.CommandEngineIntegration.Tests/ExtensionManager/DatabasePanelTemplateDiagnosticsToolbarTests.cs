using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.ExtensionManager;

public sealed class DatabasePanelTemplateDiagnosticsToolbarTests
{
    [Fact]
    public void DatabasePanelTemplate_ExposesDiagnosticsToolStripForDerivedForms()
    {
        string designerText = ReadFile(
            "Host",
            "CodexExpensa.ExtensionDevHost",
            "CommandEngineIntegration",
            "DatabasePanelTemplate.Designer.cs");

        Assert.Contains("protected ToolStrip diagnosticsToolStrip", designerText);
        Assert.Contains("diagnosticsToolStrip = new ToolStrip()", designerText);
        Assert.Contains("Controls.Add(diagnosticsToolStrip)", designerText);
    }

    [Fact]
    public void DatabasePanelTemplate_ProvidesDiagnosticsHelperMethods()
    {
        string formText = ReadFile(
            "Host",
            "CodexExpensa.ExtensionDevHost",
            "CommandEngineIntegration",
            "DatabasePanelTemplate.cs");

        Assert.Contains("DiagnosticsToolStrip", formText);
        Assert.Contains("ClearDiagnosticsToolStrip", formText);
        Assert.Contains("AddDiagnosticsToolStripItem", formText);
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
