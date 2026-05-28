using Codex.CommandEngine.Data;
using Microsoft.Data.Sqlite;
using Xunit;

namespace Codex.CommandEngine.IntegrationTests;

public sealed class DatabaseSchemaGuardTests
{
    [Fact]
    public void RequireTablesAndColumns_WhenTableIsMissing_ThrowsClearException()
    {
        string databasePath =
            Path.Combine(
                Path.GetTempPath(),
                "Codex.CommandEngine.Tests",
                nameof(DatabaseSchemaGuardTests),
                Guid.NewGuid().ToString("N"),
                "empty.db");

        Directory.CreateDirectory(Path.GetDirectoryName(databasePath)!);

        CommandEngineConnectionFactory factory =
            TestDatabasePaths.CreateConnectionFactory(databasePath);

        DatabaseSchemaException exception =
            Assert.Throws<DatabaseSchemaException>(
                () => DatabaseSchemaGuard.RequireTablesAndColumns(
                    factory,
                    new Dictionary<string, IReadOnlyList<string>>
                    {
                        ["MissingTable"] = ["MissingColumn"]
                    }));

        Assert.Contains("Required database table 'MissingTable' was not found", exception.Message);
    }

    [Fact]
    public void RequireTablesAndColumns_WhenColumnIsMissing_ThrowsClearException()
    {
        string databasePath =
            Path.Combine(
                Path.GetTempPath(),
                "Codex.CommandEngine.Tests",
                nameof(DatabaseSchemaGuardTests),
                Guid.NewGuid().ToString("N"),
                "partial.db");

        Directory.CreateDirectory(Path.GetDirectoryName(databasePath)!);

        using (SqliteConnection connection = TestDatabasePaths.OpenConnection(databasePath))
        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText =
                "CREATE TABLE ExampleTable (ExampleId TEXT PRIMARY KEY);";

            command.ExecuteNonQuery();
        }

        CommandEngineConnectionFactory factory =
            TestDatabasePaths.CreateConnectionFactory(databasePath);

        DatabaseSchemaException exception =
            Assert.Throws<DatabaseSchemaException>(
                () => DatabaseSchemaGuard.RequireTablesAndColumns(
                    factory,
                    new Dictionary<string, IReadOnlyList<string>>
                    {
                        ["ExampleTable"] = ["ExampleId", "MissingColumn"]
                    }));

        Assert.Contains("Required database column 'ExampleTable.MissingColumn' was not found", exception.Message);
    }

    [Fact]
    public void RequireTablesAndColumns_WhenSchemaMatches_DoesNotThrow()
    {
        string databasePath =
            TestDatabasePaths.ResetDatabaseForTestClass(nameof(DatabaseSchemaGuardTests));

        try
        {
            CommandEngineConnectionFactory factory =
                TestDatabasePaths.CreateConnectionFactory(databasePath);

            DatabaseSchemaGuard.RequireTablesAndColumns(
                factory,
                new Dictionary<string, IReadOnlyList<string>>
                {
                    ["SqlQuery"] = ["QueryName", "SqlText", "Category"]
                });
        }
        finally
        {
            TestDatabasePaths.LeaveDatabaseForInspection(databasePath);
        }
    }
}
