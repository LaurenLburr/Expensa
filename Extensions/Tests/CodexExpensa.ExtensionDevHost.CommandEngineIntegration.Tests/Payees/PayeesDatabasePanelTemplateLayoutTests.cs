using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.Payees;

public sealed class PayeesDatabasePanelTemplateLayoutTests
{
    [Fact]
    public void PayeesDatabasePanel_UsesDatabasePanelTemplateControlNames()
    {
        string designerText = ReadFile(
                       "Host",
            "CodexExpensa.ExtensionDevHost",
            "CommandEngineIntegration",
            "Payees",
            "PayeesDatabasePanelForm.Designer.cs");

        Assert.Contains("panel1", designerText);
        Assert.Contains("linkUpdate_from_Dev", designerText);
        Assert.Contains("linkUpdate_from_Prod", designerText);
        Assert.Contains("linkDb_filename", designerText);
        Assert.Contains("labelAdd_in_Name", designerText);
        Assert.Contains("splitContainer1", designerText);
        Assert.Contains("text_Data_", designerText);
        Assert.Contains("gridDataView", designerText);
        Assert.Contains("statusStrip1", designerText);
        Assert.Contains("labelNumRows", designerText);
    }

    [Fact]
    public void PayeesDatabasePanel_UsesHorizontalSplit()
    {
        string designerText = ReadFile(
                     "Host",
            "CodexExpensa.ExtensionDevHost",
            "CommandEngineIntegration",
            "Payees",
            "PayeesDatabasePanelForm.Designer.cs");

        Assert.Contains("splitContainer1.Orientation = Orientation.Horizontal", designerText);
    }

    [Fact]
    public void PayeesDatabasePanel_LoadsPayeesIntoGridDataView()
    {
        string formText = ReadFile(
            "Host",
            "CodexExpensa.ExtensionDevHost",
            "CommandEngineIntegration",
            "Payees",
            "PayeesDatabasePanelForm.cs");

        Assert.Contains("LoadPayeeSummary", formText);
        Assert.Contains("gridDataView.DataSource", formText);
        Assert.Contains("labelNumRows.Text", formText);
        Assert.Contains("text_Data_.Text", formText);
    }

    private static string ReadFile(params string[] parts)
    {
        string repositoryRoot = FindExtensionsRoot();
        string path = Path.Combine([repositoryRoot, .. parts]);

        Assert.True(File.Exists(path), $"File was not found: {path}");

        return File.ReadAllText(path);
    }

    private static string FindRepositoryRoot()
    {
        return TestPathHelper.ExtensionsRoot;
    }
    public static string FindExtensionsRoot()
    {
        DirectoryInfo? directory = new(AppContext.BaseDirectory);

        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "Extensions.sln")))
            {
                return directory.FullName;
            }

            if (Directory.Exists(Path.Combine(directory.FullName, "Host"))
                && Directory.Exists(Path.Combine(directory.FullName, "Modules"))
                && Directory.Exists(Path.Combine(directory.FullName, "Tests")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException(
            "Could not find Extensions solution root.");
    }
}
