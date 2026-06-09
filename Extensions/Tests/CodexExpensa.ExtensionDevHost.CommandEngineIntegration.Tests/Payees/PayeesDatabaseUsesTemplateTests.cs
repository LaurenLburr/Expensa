using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.Payees;

public sealed class PayeesDatabaseUsesTemplateTests
{
    [Fact]
    public void DatabasePanelTemplate_ExposesSharedControlsToDerivedForms()
    {
        string designerText = ReadFile(
            "Host",
            "CodexExpensa.ExtensionDevHost",
            "CommandEngineIntegration",
            "DatabasePanelTemplate.Designer.cs");

        Assert.Contains("protected Panel panel1", designerText);
        Assert.Contains("protected LinkLabel linkDb_filename", designerText);
        Assert.Contains("protected SplitContainer splitContainer1", designerText);
        Assert.Contains("protected TextBox text_Data_", designerText);
        Assert.Contains("protected DataGridView gridDataView", designerText);
        Assert.Contains("protected ToolStripStatusLabel labelNumRows", designerText);
    }

    [Fact]
    public void PayeesDatabasePanel_DerivesFromDatabasePanelTemplate()
    {
        string formText = ReadFile(
            "Host",
            "CodexExpensa.ExtensionDevHost",
            "CommandEngineIntegration",
            "Payees",
            "PayeesDatabasePanelForm.cs");

        Assert.Contains("PayeesDatabasePanelForm : DatabasePanelTemplate", formText);
        Assert.Contains("ConfigureDatabasePanel", formText);
        Assert.Contains("SetGridDataSource", formText);
        Assert.Contains("SetSummaryText", formText);
        Assert.Contains("SetRowStatus", formText);
    }

    [Fact]
    public void PayeesDatabasePanel_DoesNotOwnTemplateLayout()
    {
        string designerText = ReadFile(
            "Host",
            "CodexExpensa.ExtensionDevHost",
            "CommandEngineIntegration",
            "Payees",
            "PayeesDatabasePanelForm.Designer.cs");

        Assert.DoesNotContain("new SplitContainer", designerText);
        Assert.DoesNotContain("new DataGridView", designerText);
        Assert.DoesNotContain("new StatusStrip", designerText);
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
