using Microsoft.Data.Sqlite;

namespace CodexExpensa.ExtensionDevHost.Data;

public sealed class ExtensionManagerDatabase
{
    private readonly string _databasePath;

    public ExtensionManagerDatabase(string databasePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(databasePath);
        _databasePath = databasePath;
    }

    public string DatabasePath => _databasePath;

    public void EnsureCreated()
    {
        string? folder = Path.GetDirectoryName(_databasePath);
        if (!string.IsNullOrWhiteSpace(folder) && !Directory.Exists(folder))
        {
            Directory.CreateDirectory(folder);
        }

        using SqliteConnection connection = OpenConnection();
        connection.Open();

        using SqliteTransaction transaction = connection.BeginTransaction();
        try
        {
            ExecuteNonQuery(connection, transaction, @"
CREATE TABLE IF NOT EXISTS [ExtensionProject] (
    [ProjectName]     TEXT    NOT NULL PRIMARY KEY,
    [RelativeBinPath] TEXT    NULL,
    [AssemblyName]    TEXT    NOT NULL,
    [IsEnabled]       INTEGER NOT NULL DEFAULT 1,
    [SortOrder]       INTEGER NOT NULL DEFAULT 0,
    CONSTRAINT [CK_ExtensionProject_IsEnabled] CHECK ([IsEnabled] IN (0, 1))
);");

            ExecuteNonQuery(connection, transaction, @"
CREATE INDEX IF NOT EXISTS [IX_ExtensionProject_IsEnabled_SortOrder]
ON [ExtensionProject]([IsEnabled], [SortOrder], [ProjectName]);");

            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public IReadOnlyList<ExtensionRegistrationRecord> GetEnabledProjects()
    {
        using SqliteConnection connection = OpenConnection();
        connection.Open();

        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = @"
SELECT
    [ProjectName],
    [RelativeBinPath],
    [AssemblyName],
    [IsEnabled],
    [SortOrder]
FROM [ExtensionProject]
WHERE [IsEnabled] = 1
ORDER BY [SortOrder], [ProjectName];";

        using SqliteDataReader reader = command.ExecuteReader();

        List<ExtensionRegistrationRecord> rows = new();
        while (reader.Read())
        {
            rows.Add(new ExtensionRegistrationRecord
            {
                ProjectName = reader.GetString(0),
                RelativeBinPath = reader.IsDBNull(1) ? null : reader.GetString(1),
                AssemblyName = reader.GetString(2),
                IsEnabled = reader.GetInt32(3) == 1,
                SortOrder = reader.GetInt32(4)
            });
        }

        return rows;
    }

    public void UpsertProject(ExtensionRegistrationRecord record)
    {
        ArgumentNullException.ThrowIfNull(record);

        using SqliteConnection connection = OpenConnection();
        connection.Open();

        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = @"
INSERT INTO [ExtensionProject] (
    [ProjectName],
    [RelativeBinPath],
    [AssemblyName],
    [IsEnabled],
    [SortOrder]
)
VALUES (
    $projectName,
    $relativeBinPath,
    $assemblyName,
    $isEnabled,
    $sortOrder
)
ON CONFLICT([ProjectName]) DO UPDATE SET
    [RelativeBinPath] = excluded.[RelativeBinPath],
    [AssemblyName] = excluded.[AssemblyName],
    [IsEnabled] = excluded.[IsEnabled],
    [SortOrder] = excluded.[SortOrder];";

        command.Parameters.AddWithValue("$projectName", record.ProjectName);
        command.Parameters.AddWithValue("$relativeBinPath", (object?)record.RelativeBinPath ?? DBNull.Value);
        command.Parameters.AddWithValue("$assemblyName", record.AssemblyName);
        command.Parameters.AddWithValue("$isEnabled", record.IsEnabled ? 1 : 0);
        command.Parameters.AddWithValue("$sortOrder", record.SortOrder);

        command.ExecuteNonQuery();
    }

    private SqliteConnection OpenConnection()
    {
        SqliteConnectionStringBuilder builder = new()
        {
            DataSource = _databasePath,
            ForeignKeys = true
        };

        return new SqliteConnection(builder.ToString());
    }

    private static void ExecuteNonQuery(SqliteConnection connection, SqliteTransaction transaction, string sql)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText = sql;
        command.ExecuteNonQuery();
    }
}
