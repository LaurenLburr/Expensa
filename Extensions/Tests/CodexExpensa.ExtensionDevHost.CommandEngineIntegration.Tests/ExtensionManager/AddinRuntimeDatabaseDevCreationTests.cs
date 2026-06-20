using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.ExtensionManager;

public sealed class AddinRuntimeDatabaseDevCreationTests
{
    [Fact]
    public void CopyService_CreatesMissingDevDatabaseFromCurrentRuntimeDatabase()
    {
        string text = TestPathHelper.ReadHostFile(
            "CodexExpensa.ExtensionDevHost",
            "CommandEngineIntegration",
            "ExtensionManager",
            "AddinRuntimeDatabaseCopyService.cs");

        Assert.Contains("CreateDevDatabaseFromRuntime", text);
        Assert.Contains("GetRuntimeDatabaseLocation(addinId)", text);
        Assert.Contains("GetDevCurrentDatabasePath(addinId)", text);
        Assert.Contains("File.Copy(", text);
        Assert.Contains("overwrite: false", text);
        Assert.DoesNotContain("File.Delete(runtimeLocation.DatabasePath)", text);
    }
}
