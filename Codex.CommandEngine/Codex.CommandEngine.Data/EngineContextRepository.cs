using Microsoft.Data.Sqlite;

namespace Codex.CommandEngine.Data;

public sealed class EngineContextRepository
{
    private readonly CommandEngineConnectionFactory _connectionFactory;

    public EngineContextRepository(CommandEngineConnectionFactory connectionFactory)
    {
        ArgumentNullException.ThrowIfNull(connectionFactory);
        _connectionFactory = connectionFactory;
    }

    public void Upsert(EngineContextUpsert context)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentException.ThrowIfNullOrWhiteSpace(context.ContextId);
        ArgumentException.ThrowIfNullOrWhiteSpace(context.ContextName);

        using SqliteConnection connection = _connectionFactory.OpenConnection();
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = GetRequiredSqlText(RepositorySqlQueryNames.EngineContext.InsertOrReplace);

        command.Parameters.AddWithValue("$ContextId", context.ContextId);
        command.Parameters.AddWithValue("$ContextName", context.ContextName);
        command.Parameters.AddWithValue("$Scope", context.Scope);
        command.Parameters.AddWithValue("$Description", context.Description);
        command.Parameters.AddWithValue("$ContextJson", context.ContextJson);
        command.Parameters.AddWithValue("$MetadataJson", context.MetadataJson);
        command.Parameters.AddWithValue("$IsEnabled", context.IsEnabled ? 1 : 0);

        command.ExecuteNonQuery();
    }

    public EngineContextRecord? FindByName(string contextName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(contextName);

        using SqliteConnection connection = _connectionFactory.OpenConnection();
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = GetRequiredSqlText(RepositorySqlQueryNames.EngineContext.SelectByName);
        command.Parameters.AddWithValue("$ContextName", contextName);

        using SqliteDataReader reader = command.ExecuteReader();
        return reader.Read() ? ReadContext(reader) : null;
    }

    public IReadOnlyList<EngineContextRecord> ListAll()
    {
        using SqliteConnection connection = _connectionFactory.OpenConnection();
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = GetRequiredSqlText(RepositorySqlQueryNames.EngineContext.SelectAll);

        List<EngineContextRecord> records = [];
        using SqliteDataReader reader = command.ExecuteReader();

        while (reader.Read())
        {
            records.Add(ReadContext(reader));
        }

        return records;
    }

    private string GetRequiredSqlText(string queryName)
    {
        SqlQueryCatalog catalog = new(_connectionFactory);
        return catalog.GetRequiredSqlText(queryName);
    }

    private static EngineContextRecord ReadContext(SqliteDataReader reader)
    {
        return new EngineContextRecord(
            reader.GetRequiredString("ContextId"),
            reader.GetRequiredString("ContextName"),
            reader.GetStringOrDefault("Scope"),
            reader.GetStringOrDefault("Description"),
            reader.GetStringOrDefault("ContextJson", "{}"),
            reader.GetStringOrDefault("MetadataJson", "{}"),
            reader.GetBooleanOrDefault("IsEnabled"),
            reader.GetStringOrDefault("CreatedUtc"),
            reader.GetNullableString("UpdatedUtc"));
    }
}

