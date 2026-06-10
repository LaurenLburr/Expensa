using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.ExtensionManager;

public sealed class AddinMemoryDatabaseSessionStructureTests
{
    [Fact]
    public void AddinMemoryDatabaseSession_LoadsFileIntoMemoryAndClosesSourceConnection()
    {
        string text = TestPathHelper.ReadHostFile(
            "CodexExpensa.ExtensionDevHost",
            "CommandEngineIntegration",
            "ExtensionManager",
            "AddinMemoryDatabaseSession.cs");

        Assert.Contains("Data Source=:memory:", text, StringComparison.Ordinal);
        Assert.Contains("Mode=ReadOnly", text, StringComparison.Ordinal);
        Assert.Contains("sourceConnection.BackupDatabase(memoryConnection)", text, StringComparison.Ordinal);
        Assert.Contains("using SqliteConnection sourceConnection", text, StringComparison.Ordinal);
        Assert.Contains("Connection.Dispose()", text, StringComparison.Ordinal);
    }
}
