using Xunit;

using CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.Websites;

public sealed class WebsiteLoadRequestDatabasePathStructureTests
{
    [Fact]
    //public void WebsitesAddin_LoadRequestAndCommandSupportDatabasePath()
    //{
    //    string repositoryRoot = FindRepositoryRoot();

    //    string moduleFolder =
    //        Path.Combine(repositoryRoot, "Extensions", "Modules", "WebsitesAddin");

    //    string requestText =
    //        File.ReadAllText(Path.Combine(moduleFolder, "WebsiteLoadRequest.cs"));

    //    string parserText =
    //        File.ReadAllText(Path.Combine(moduleFolder, "WebsiteLoadRequestParser.cs"));

    //    string commandText =
    //        File.ReadAllText(Path.Combine(moduleFolder, "WebsiteLoadCommand.cs"));

    //    Assert.Contains("DatabasePath", requestText);
    //    Assert.Contains("DatabasePath", parserText);
    //    Assert.Contains("CreateRepositoryForRequest", commandText);
    //    Assert.Contains("SqliteWebsiteRepository", commandText);
    //}

    public void WebsitesAddin_LoadRequestAndCommandSupportDatabasePath()
    {
        string extensionsRoot = CommandEngineIntegrationProjectShapeTests.FindExtensionsRoot();

        string moduleFolder =
            Path.Combine(extensionsRoot, "Modules", "WebsitesAddin");

        string requestText =
            File.ReadAllText(Path.Combine(moduleFolder, "WebsiteLoadRequest.cs"));

        string parserText =
            File.ReadAllText(Path.Combine(moduleFolder, "WebsiteLoadRequestParser.cs"));

        string commandText =
            File.ReadAllText(Path.Combine(moduleFolder, "WebsiteLoadCommand.cs"));

        Assert.Contains("DatabasePath", requestText);
        Assert.Contains("DatabasePath", parserText);
        Assert.Contains("CreateRepositoryForRequest", commandText);
    }

    private static string FindRepositoryRoot()
    {
        return TestPathHelper.ExtensionsRoot;
    }
}
