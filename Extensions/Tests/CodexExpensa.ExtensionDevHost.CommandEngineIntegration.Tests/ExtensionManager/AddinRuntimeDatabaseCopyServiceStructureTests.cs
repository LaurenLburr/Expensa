using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.ExtensionManager;

public sealed class AddinRuntimeDatabaseCopyServiceStructureTests
{
    [Fact]
    public void CopyService_CopiesFromSourceToRuntimeWithoutDeletingSource()
    {
        string text = TestPathHelper.ReadHostFile(
            "CodexExpensa.ExtensionDevHost",
            "CommandEngineIntegration",
            "ExtensionManager",
            "AddinRuntimeDatabaseCopyService.cs");

        Assert.Contains("ReplaceRuntimeDatabaseFromProd", text);
        Assert.Contains("ReplaceRuntimeDatabaseFromDev", text);
        Assert.Contains("File.Copy", text);
        Assert.Contains("File.Delete(tempCopyPath)", text);
        Assert.DoesNotContain("File.Delete(sourceDatabasePath)", text);
        Assert.DoesNotContain("File.Move(sourceDatabasePath", text);
    }
}
