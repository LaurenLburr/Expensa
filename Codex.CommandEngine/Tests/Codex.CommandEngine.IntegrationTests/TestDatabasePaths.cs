using Codex.CommandEngine.Data;
using Microsoft.Data.Sqlite;

namespace Codex.CommandEngine.IntegrationTests;

internal static class TestDatabasePaths
{
    private const string TemplateDatabasePath =
        @"D:\Git\CodexExpensa\Codex.CommandEngine\DevDatabase\CommandEngine.dev.db";

    public static string ResetDatabaseForTestClass(string testClassName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(testClassName);

        string folder = Path.Combine(
            Path.GetTempPath(),
            "Codex.CommandEngine.Tests",
            testClassName,
            Guid.NewGuid().ToString("N"));

        Directory.CreateDirectory(folder);

        string databasePath = Path.Combine(folder, "command-engine.db");

        if (!File.Exists(TemplateDatabasePath))
        {
            throw new FileNotFoundException(
                $"Template database was not found. Expected: {TemplateDatabasePath}",
                TemplateDatabasePath);
        }

        File.Copy(TemplateDatabasePath, databasePath, overwrite: true);

        return databasePath;
    }

    public static CommandEngineConnectionFactory CreateConnectionFactory(string databasePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(databasePath);

        return new CommandEngineConnectionFactory(
            CommandEngineDatabaseOptions.ForFile(databasePath));
    }

    public static SqliteConnection OpenConnection(string databasePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(databasePath);

        SqliteConnectionStringBuilder builder = new()
        {
            DataSource = databasePath,
            Pooling = false
        };

        SqliteConnection connection = new(builder.ToString());
        connection.Open();

        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = "PRAGMA foreign_keys = ON;";
        command.ExecuteNonQuery();

        return connection;
    }

    public static void LeaveDatabaseForInspection(string databasePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(databasePath);

        // Intentionally do not call SqliteConnection.ClearAllPools() here.
        // ClearAllPools is global and can invalidate active connections in parallel tests.
        // Each test run gets a unique database path, so pool clearing is unnecessary.
    }
}
