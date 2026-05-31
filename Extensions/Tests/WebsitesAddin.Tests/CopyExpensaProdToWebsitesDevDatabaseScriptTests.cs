using Xunit;

namespace WebsitesAddin.Tests;

public sealed class CopyExpensaProdToWebsitesDevDatabaseScriptTests
{
    [Fact]
    public void CopyScript_ExistsAndTargetsExpectedDatabases()
    {
        string repositoryRoot =
            FindRepositoryRoot();

        string scriptPath =
            Path.Combine(
                repositoryRoot,
                "Extensions",
                "Modules",
                "WebsitesAddin",
                "Scripts",
                "Copy_Expensa_Prod_To_Websites_DevDatabase.ps1");

        Assert.True(File.Exists(scriptPath), $"File was not found: {scriptPath}");

        string text =
            File.ReadAllText(scriptPath);

        Assert.Contains("CodexExpensa", text);
        Assert.Contains("codexexpensa.db", text);
        Assert.Contains("websites.dev.db", text);
        Assert.Contains("ATTACH DATABASE", text);
        Assert.Contains("DELETE FROM [Website]", text);
        Assert.Contains("INSERT OR REPLACE INTO [Website]", text);
        Assert.Contains("Website.Select.Enabled", text);
    }

    [Fact]
    public void CopyCommand_InvokesPowerShellScript()
    {
        string repositoryRoot =
            FindRepositoryRoot();

        string commandPath =
            Path.Combine(
                repositoryRoot,
                "Extensions",
                "Modules",
                "WebsitesAddin",
                "Scripts",
                "Copy_Expensa_Prod_To_Websites_DevDatabase.cmd");

        Assert.True(File.Exists(commandPath), $"File was not found: {commandPath}");

        string text =
            File.ReadAllText(commandPath);

        Assert.Contains("powershell", text, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("Copy_Expensa_Prod_To_Websites_DevDatabase.ps1", text);
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
