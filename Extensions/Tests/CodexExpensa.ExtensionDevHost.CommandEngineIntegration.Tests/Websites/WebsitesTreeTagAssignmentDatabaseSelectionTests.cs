using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.Websites;

public sealed class WebsitesTreeTagAssignmentDatabaseSelectionTests
{
    [Fact]
    public void TagPicker_UsesDatabasePathFromRuntimeDatabaseLocation()
    {
        string text =
            ReadFile(
                "Host",
                "CodexExpensa.ExtensionDevHost",
                "CommandEngineIntegration",
                "Websites",
                "WebsitesTreeLoadVerificationForm.TagPicker.cs");

        Assert.Contains("HostWebsiteDatabaseLocation databaseLocation", text);
        Assert.Contains("_runtimeDatabaseSelectionService.GetActiveRuntimeDatabaseLocation()", text);
        Assert.Contains("databaseLocation.DatabasePath", text);
        Assert.DoesNotContain("string databasePath =\r\n            _runtimeDatabaseSelectionService.GetActiveRuntimeDatabaseLocation()", text);
        Assert.DoesNotContain("string databasePath =\n            _runtimeDatabaseSelectionService.GetActiveRuntimeDatabaseLocation()", text);
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
