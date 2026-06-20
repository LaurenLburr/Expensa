using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.ExtensionManager;

public sealed class AddinMemoryDatabaseSessionStructureTests
{
    [Fact]
    public void AddinMemoryDatabaseSession_DelegatesFileLoadingToSafeSqliteConnection()
    {
        string text = TestPathHelper.ReadHostFile(
            "CodexExpensa.ExtensionDevHost",
            "CommandEngineIntegration",
            "ExtensionManager",
            "AddinMemoryDatabaseSession.cs");

        Assert.Contains(
            "SafeSqliteConnection.OpenFromFile(sourceDatabasePath)",
            text,
            StringComparison.Ordinal);

        Assert.Contains(
            "SafeSqliteConnection.CreateEmptyMemory()",
            text,
            StringComparison.Ordinal);

        Assert.Contains(
            "SafeSqliteConnection.SaveToFile(",
            text,
            StringComparison.Ordinal);

        Assert.Contains(
            "Connection.Close()",
            text,
            StringComparison.Ordinal);

        Assert.Contains(
            "Connection.Dispose()",
            text,
            StringComparison.Ordinal);

        Assert.DoesNotContain(
            "new SqliteConnection",
            text,
            StringComparison.Ordinal);

        Assert.DoesNotContain(
            "SqliteConnectionStringBuilder",
            text,
            StringComparison.Ordinal);
    }

    [Fact]
    public void SafeSqliteConnection_OwnsMemoryAndFileConnectionImplementation()
    {
        string text = TestPathHelper.ReadHostFile(
            "CodexExpensa.ExtensionDevHost",
            "CommandEngineIntegration",
            "ExtensionManager",
            "SafeSqliteConnection.cs");

        Assert.Contains(
            "DataSource = \":memory:\"",
            text,
            StringComparison.Ordinal);

        Assert.Contains(
            "Pooling = false",
            text,
            StringComparison.Ordinal);

        Assert.Contains(
            "fileConnection.BackupDatabase(memoryConnection)",
            text,
            StringComparison.Ordinal);

        Assert.Contains(
            "memoryConnection.BackupDatabase(fileConnection)",
            text,
            StringComparison.Ordinal);

        Assert.Contains(
            "fileConnection.Close()",
            text,
            StringComparison.Ordinal);

        Assert.Contains(
            "SqliteConnection.ClearAllPools()",
            text,
            StringComparison.Ordinal);
    }
}
