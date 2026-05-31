using Xunit;

namespace WebsitesAddin.Tests;

public sealed class CopyExpensaProdToWebsitesDevAndRuntimeScriptTests
{
    [Fact]
    public void Script_CopiesToDevAndRuntimeAndSetsActivePath()
    {
        string repositoryRoot = FindRepositoryRoot();

        string scriptPath =
            Path.Combine(
                repositoryRoot,
                "Extensions",
                "Modules",
                "WebsitesAddin",
                "Scripts",
                "Copy_Expensa_Prod_To_Websites_Dev_And_Runtime.ps1");

        Assert.True(File.Exists(scriptPath), $"File was not found: {scriptPath}");

        string text =
            File.ReadAllText(scriptPath);

        Assert.Contains("websites.dev.db", text);
        Assert.Contains("websitesaddin.db", text);
        Assert.Contains("active-runtime-db.txt", text);
        Assert.Contains("Copy-Item -Path $WebsitesDevDatabasePath -Destination $RuntimeDatabasePath -Force", text);
        Assert.Contains("Set-Content -Path $activePathFile", text);
        Assert.Contains("Runtime rows", text);
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
