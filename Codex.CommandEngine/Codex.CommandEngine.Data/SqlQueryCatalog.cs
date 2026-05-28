using Microsoft.Data.Sqlite;

namespace Codex.CommandEngine.Data;

public sealed class SqlQueryCatalog
{
    private readonly CommandEngineConnectionFactory _connectionFactory;

    public SqlQueryCatalog(CommandEngineConnectionFactory connectionFactory)
    {
        ArgumentNullException.ThrowIfNull(connectionFactory);

        _connectionFactory = connectionFactory;
    }

    public void Upsert(SqlQueryDefinition definition)
    {
        ArgumentNullException.ThrowIfNull(definition);
        ArgumentException.ThrowIfNullOrWhiteSpace(definition.QueryName);
        ArgumentException.ThrowIfNullOrWhiteSpace(definition.SqlText);

        using SqliteConnection connection = _connectionFactory.OpenConnection();
        CommandEngineDatabaseInitializer.EnsureCreated(connection);
        Upsert(connection, definition);
    }

    public SqlQueryRecord? FindByName(string queryName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(queryName);

        using SqliteConnection connection = _connectionFactory.OpenConnection();
        CommandEngineDatabaseInitializer.EnsureCreated(connection);

        return FindByName(connection, queryName);
    }

    public string GetRequiredSqlText(string queryName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(queryName);

        SqlQueryRecord? query = FindByName(queryName);

        if (query is null)
        {
            throw new InvalidOperationException($"SQL query was not found: {queryName}");
        }

        if (!query.IsActive)
        {
            throw new InvalidOperationException($"SQL query is inactive: {queryName}");
        }

        return query.SqlText;
    }

    public IReadOnlyList<SqlQueryRecord> ListActive()
    {
        using SqliteConnection connection = _connectionFactory.OpenConnection();
        CommandEngineDatabaseInitializer.EnsureCreated(connection);

        List<SqlQueryRecord> records = [];

        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = """
SELECT QueryName,
       Description,
       SqlText,
       Category,
       IsActive,
       CreatedUtc,
       UpdatedUtc
FROM SqlQuery
WHERE IsActive = 1
  AND Category NOT IN (
      'SQL Catalog',
      'Command Registration',
      'Workflow Foundation',
      'Execution History',
      'Context System Foundation',
      'AI Provider Foundation'
  )
ORDER BY Category ASC,
         QueryName ASC;
""";

        using SqliteDataReader reader = command.ExecuteReader();

        while (reader.Read())
        {
            records.Add(ReadSqlQueryRecord(reader));
        }

        return records;
    }

    public static void Upsert(SqliteConnection connection, SqlQueryDefinition definition)
    {
        ArgumentNullException.ThrowIfNull(connection);
        ArgumentNullException.ThrowIfNull(definition);
        ArgumentException.ThrowIfNullOrWhiteSpace(definition.QueryName);
        ArgumentException.ThrowIfNullOrWhiteSpace(definition.SqlText);

        if (connection.State != System.Data.ConnectionState.Open)
        {
            connection.Open();
        }

        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = """
INSERT INTO SqlQuery (
    QueryName,
    Description,
    SqlText,
    Category,
    IsActive
)
VALUES (
    $QueryName,
    $Description,
    $SqlText,
    $Category,
    $IsActive
)
ON CONFLICT(QueryName) DO UPDATE SET
    Description = excluded.Description,
    SqlText = excluded.SqlText,
    Category = excluded.Category,
    IsActive = excluded.IsActive,
    UpdatedUtc = CURRENT_TIMESTAMP;
""";
        command.Parameters.AddWithValue("$QueryName", definition.QueryName);
        command.Parameters.AddWithValue("$Description", definition.Description);
        command.Parameters.AddWithValue("$SqlText", definition.SqlText);
        command.Parameters.AddWithValue("$Category", definition.Category);
        command.Parameters.AddWithValue("$IsActive", definition.IsActive ? 1 : 0);
        command.ExecuteNonQuery();
    }

    public static SqlQueryRecord? FindByName(SqliteConnection connection, string queryName)
    {
        ArgumentNullException.ThrowIfNull(connection);
        ArgumentException.ThrowIfNullOrWhiteSpace(queryName);

        if (connection.State != System.Data.ConnectionState.Open)
        {
            connection.Open();
        }

        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = """
SELECT QueryName,
       Description,
       SqlText,
       Category,
       IsActive,
       CreatedUtc,
       UpdatedUtc
FROM SqlQuery
WHERE QueryName = $QueryName;
""";
        command.Parameters.AddWithValue("$QueryName", queryName);

        using SqliteDataReader reader = command.ExecuteReader();

        if (!reader.Read())
        {
            return null;
        }

        return ReadSqlQueryRecord(reader);
    }

    private static SqlQueryRecord ReadSqlQueryRecord(SqliteDataReader reader)
    {
        return new SqlQueryRecord(
            reader.GetString(0),
            reader.GetString(1),
            reader.GetString(2),
            reader.GetString(3),
            reader.GetInt32(4) == 1,
            reader.GetString(5),
            reader.IsDBNull(6) ? null : reader.GetString(6));
    }
}
