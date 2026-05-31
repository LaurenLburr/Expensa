using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.Websites;

public sealed class WebsiteLoadRequestDatabasePathStructureTests
{
    [Fact]
    public void WebsitesAddin_LoadRequestAndCommandSupportDatabasePath()
    {
        string repositoryRoot = FindRepositoryRoot();

        string moduleFolder =
            Path.Combine(repositoryRoot, "Extensions", "Modules", "WebsitesAddin");

        string requestText =
            File.ReadAllText(Path.Combine(moduleFolder, "WebsiteLoadRequest.cs"));

        string parserText =
            File.ReadAllText(Path.Combine(moduleFolder, "WebsiteLoadRequestParser.cs"));

        string commandText =
            File.ReadAllText(Path.Combine(moduleFolder, "WebsiteLoadCommand.cs"));

        Assert.Contains("DatabasePath", requestText);
        Assert.Contains("DatabasePath", parserText);
        Assert.Contains("CreateRepositoryForRequest", commandText);
        Assert.Contains("SqliteWebsiteRepository", commandText);
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
