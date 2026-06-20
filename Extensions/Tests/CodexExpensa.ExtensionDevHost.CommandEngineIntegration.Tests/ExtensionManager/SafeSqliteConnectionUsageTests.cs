using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.ExtensionManager;

public sealed class SafeSqliteConnectionUsageTests
{
    [Fact]
    public void ProductionCode_CreatesSqliteConnectionsOnlyInsideSafeHelper()
    {
        string integrationRoot =
            TestPathHelper.HostPath(
                "CodexExpensa.ExtensionDevHost",
                "CommandEngineIntegration");

        string[] files =
            Directory.GetFiles(
                integrationRoot,
                "*.cs",
                SearchOption.AllDirectories);

        List<string> violations = [];

        foreach (string file in files)
        {
            if (string.Equals(
                    Path.GetFileName(file),
                    "SafeSqliteConnection.cs",
                    StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            string text = File.ReadAllText(file);

            if (text.Contains(
                    "new SqliteConnection",
                    StringComparison.Ordinal) ||
                text.Contains(
                    "SqliteConnectionStringBuilder",
                    StringComparison.Ordinal) ||
                text.Contains(
                    "SqliteConnection connection = new(",
                    StringComparison.Ordinal))
            {
                violations.Add(
                    Path.GetRelativePath(
                        integrationRoot,
                        file));
            }
        }

        Assert.True(
            violations.Count == 0,
            "Direct SQLite connection creation found outside SafeSqliteConnection:"
            + Environment.NewLine
            + string.Join(Environment.NewLine, violations));
    }

    [Fact]
    public void SafeHelper_UsesMemoryBackupAndDisablesFilePooling()
    {
        string text = TestPathHelper.ReadHostFile(
            "CodexExpensa.ExtensionDevHost",
            "CommandEngineIntegration",
            "ExtensionManager",
            "SafeSqliteConnection.cs");

        Assert.Contains("DataSource = \":memory:\"", text);
        Assert.Contains("Pooling = false", text);
        Assert.Contains("fileConnection.BackupDatabase(memoryConnection)", text);
        Assert.Contains("memoryConnection.BackupDatabase(fileConnection)", text);
        Assert.Contains("fileConnection.Close()", text);
        Assert.Contains("SqliteConnection.ClearAllPools()", text);
    }
    [Fact]
    public void SafeHelper_ValidatesExactSqliteHeader()
    {
        string text = TestPathHelper.ReadHostFile(
            "CodexExpensa.ExtensionDevHost",
            "CommandEngineIntegration",
            "ExtensionManager",
            "SafeSqliteConnection.cs");

        Assert.Contains("public static bool IsValidSqliteFile(FileInfo file)", text);
        Assert.Contains("\"SQLite format 3\\0\"u8", text);
        Assert.Contains("actualHeader.SequenceEqual(expectedHeader)", text);
        Assert.Contains("FileShare.ReadWrite", text);
    }

    [Fact]
    public void OpenFromFile_RejectsInvalidDatabaseBeforeOpeningSqliteConnection()
    {
        string text = TestPathHelper.ReadHostFile(
            "CodexExpensa.ExtensionDevHost",
            "CommandEngineIntegration",
            "ExtensionManager",
            "SafeSqliteConnection.cs");

        int validationIndex =
            text.IndexOf(
                "if (!IsValidSqliteFile(databaseFile))",
                StringComparison.Ordinal);

        int memoryConnectionIndex =
            text.IndexOf(
                "SqliteConnection memoryConnection = CreateEmptyMemory()",
                StringComparison.Ordinal);

        Assert.True(validationIndex >= 0);
        Assert.True(memoryConnectionIndex > validationIndex);
        Assert.Contains("throw new InvalidDataException(", text);
    }

}
