using Codex.CommandEngine.Data;
using Microsoft.Data.Sqlite;
using Xunit;

namespace Codex.CommandEngine.IntegrationTests;

public sealed class CommandEngineDatabaseBootstrapperTests
{
    [Fact]
    public void RebuildFromSqlScript_CreatesTablesOnlyWhenCalledExplicitly()
    {
        string databasePath =
            CreateDatabasePath();

        CommandEngineConnectionFactory factory =
            TestDatabasePaths.CreateConnectionFactory(databasePath);

        DatabaseSchemaException beforeException =
            Assert.Throws<DatabaseSchemaException>(
                () => DatabaseSchemaGuard.RequireTablesAndColumns(
                    factory,
                    new Dictionary<string, IReadOnlyList<string>>
                    {
                        ["Example"] = ["ExampleId", "ExampleName"]
                    }));

        Assert.Contains("Required database table 'Example' was not found", beforeException.Message);

        CommandEngineDatabaseBootstrapper bootstrapper =
            new(factory);

        bootstrapper.RebuildFromSqlScript(
            """
            CREATE TABLE Example (
                ExampleId   TEXT PRIMARY KEY,
                ExampleName TEXT NOT NULL
            );
            """);

        DatabaseSchemaGuard.RequireTablesAndColumns(
            factory,
            new Dictionary<string, IReadOnlyList<string>>
            {
                ["Example"] = ["ExampleId", "ExampleName"]
            });
    }

    [Fact]
    public void LoadRequiredScript_WhenFileMissing_ThrowsClearException()
    {
        string missingPath =
            Path.Combine(
                Path.GetTempPath(),
                "Codex.CommandEngine.Tests",
                nameof(CommandEngineDatabaseBootstrapperTests),
                Guid.NewGuid().ToString("N"),
                "missing.sql");

        FileNotFoundException exception =
            Assert.Throws<FileNotFoundException>(
                () => CommandEngineDatabaseScriptLoader.LoadRequiredScript(missingPath));

        Assert.Contains("Database schema script was not found", exception.Message);
    }

    [Fact]
    public void LoadRequiredScript_WhenFileExists_ReturnsSql()
    {
        string folder =
            Path.Combine(
                Path.GetTempPath(),
                "Codex.CommandEngine.Tests",
                nameof(CommandEngineDatabaseBootstrapperTests),
                Guid.NewGuid().ToString("N"));

        Directory.CreateDirectory(folder);

        string scriptPath =
            Path.Combine(folder, "schema.sql");

        File.WriteAllText(
            scriptPath,
            "CREATE TABLE Example (ExampleId TEXT PRIMARY KEY);");

        string sql =
            CommandEngineDatabaseScriptLoader.LoadRequiredScript(scriptPath);

        Assert.Contains("CREATE TABLE Example", sql);
    }

    private static string CreateDatabasePath()
    {
        string folder =
            Path.Combine(
                Path.GetTempPath(),
                "Codex.CommandEngine.Tests",
                nameof(CommandEngineDatabaseBootstrapperTests),
                Guid.NewGuid().ToString("N"));

        Directory.CreateDirectory(folder);

        return Path.Combine(folder, "bootstrap.db");
    }
}
