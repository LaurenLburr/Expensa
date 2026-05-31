using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.Websites;

public sealed class WebsitesDatabasePanelExpensaProdCopyStructureTests
{
    [Fact]
    public void DatabasePanel_HasCopyFromExpensaProdLink()
    {
        string designerText = ReadFile("Extensions", "Host", "CodexExpensa.ExtensionDevHost", "CommandEngineIntegration", "Websites", "WebsitesDatabasePanelForm.Designer.cs");
        string formText = ReadFile("Extensions", "Host", "CodexExpensa.ExtensionDevHost", "CommandEngineIntegration", "Websites", "WebsitesDatabasePanelForm.cs");
        string serviceText = ReadFile("Extensions", "Host", "CodexExpensa.ExtensionDevHost", "CommandEngineIntegration", "Websites", "HostWebsiteExpensaProdDatabaseCopyService.cs");

        Assert.Contains("copyFromExpensaProdLinkLabel", designerText);
        Assert.Contains("Copy from Expensa Prod", designerText);
        Assert.Contains("copyFromExpensaProdLinkLabel_LinkClicked", formText);
        Assert.Contains("CopyToDevAndRuntime", formText);
        Assert.Contains("websites.current.db", serviceText);
        Assert.Contains("codexexpensa.db", serviceText);
        Assert.Contains("active-runtime-db.txt", serviceText);
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
        DirectoryInfo? directory = new(AppContext.BaseDirectory);

        while (directory is not null)
        {
            if (Directory.Exists(Path.Combine(directory.FullName, "Extensions")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException();
    }
}
