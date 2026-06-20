using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.ExtensionManager;

public sealed class SafeSqliteConnectionSaveToFileStructureTests
{
    [Fact]
    public void SaveToFile_RejectsFolderTargetsAndReportsReplacePath()
    {
        string text = TestPathHelper.ReadHostFile(
            "CodexExpensa.ExtensionDevHost",
            "CommandEngineIntegration",
            "ExtensionManager",
            "SafeSqliteConnection.cs");

        Assert.Contains("Directory.Exists(fullPath)", text);
        Assert.Contains("The SQLite database path is a folder, not a file", text);
        Assert.Contains("ReplaceDatabaseFile", text);
        Assert.Contains("Access was denied while replacing the SQLite database file", text);
        Assert.Contains("Could not replace the SQLite database file", text);
        Assert.Contains("BackupMemoryDatabaseToExistingFile", text);
        Assert.Contains("SqliteOpenMode.ReadWrite", text);
        Assert.Contains("Could not save the in-memory SQLite database back to the existing database file", text);
    }
}
