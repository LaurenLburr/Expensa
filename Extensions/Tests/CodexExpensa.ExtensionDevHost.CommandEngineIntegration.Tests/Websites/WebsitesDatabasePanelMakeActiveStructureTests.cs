using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.Websites;

public sealed class WebsitesDatabasePanelMakeActiveStructureTests
{
    [Fact]
    public void Form_CopyFromExpensaProdCreatesAndActivatesRuntimeDatabase()
    {
        string repositoryRoot = FindRepositoryRoot();

        string formPath =
            Path.Combine(
                repositoryRoot,
                "Extensions",
                "Host",
                "CodexExpensa.ExtensionDevHost",
                "CommandEngineIntegration",
                "Websites",
                "WebsitesDatabasePanelForm.cs");

        string servicePath =
            Path.Combine(
                repositoryRoot,
                "Extensions",
                "Host",
                "CodexExpensa.ExtensionDevHost",
                "CommandEngineIntegration",
                "Websites",
                "HostWebsiteExpensaProdDatabaseCopyService.cs");

        Assert.True(File.Exists(formPath), $"File was not found: {formPath}");
        Assert.True(File.Exists(servicePath), $"File was not found: {servicePath}");

        string formText = File.ReadAllText(formPath);
        string serviceText = File.ReadAllText(servicePath);

        Assert.Contains("copyFromExpensaProdLinkLabel_LinkClicked", formText);
        Assert.Contains("CopyToDevAndRuntime", formText);
        Assert.Contains("websites.current.db", serviceText);
        Assert.Contains("active-runtime-db.txt", serviceText);
        Assert.Contains("File.WriteAllText", serviceText);
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
