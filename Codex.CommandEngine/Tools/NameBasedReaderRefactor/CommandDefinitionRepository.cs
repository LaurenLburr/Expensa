using Microsoft.Data.Sqlite;

namespace Codex.CommandEngine.Data;

public sealed class CommandDefinitionRepository
{
    private readonly CommandEngineConnectionFactory _connectionFactory;

    public CommandDefinitionRepository(CommandEngineConnectionFactory connectionFactory)
    {
        ArgumentNullException.ThrowIfNull(connectionFactory);
        _connectionFactory = connectionFactory;
    }

    public void Upsert(CommandDefinitionUpsert definition)
    {
        ArgumentNullException.ThrowIfNull(definition);
        ArgumentException.ThrowIfNullOrWhiteSpace(definition.CommandDefinitionId);
        ArgumentException.ThrowIfNullOrWhiteSpace(definition.CommandName);
        ArgumentException.ThrowIfNullOrWhiteSpace(definition.DisplayName);

        if (definition.Version <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(definition), "Command version must be greater than zero.");
        }

        using SqliteConnection connection = _connectionFactory.OpenConnection();
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = GetRequiredSqlText(RepositorySqlQueryNames.CommandDefinition.InsertOrReplace);

        command.Parameters.AddWithValue("$CommandId", definition.CommandDefinitionId);
        command.Parameters.AddWithValue("$CommandDefinitionId", definition.CommandDefinitionId);
        command.Parameters.AddWithValue("$CommandName", definition.CommandName);
        command.Parameters.AddWithValue("$DisplayName", definition.DisplayName);
        command.Parameters.AddWithValue("$Description", definition.Description);
        command.Parameters.AddWithValue("$Category", definition.Category);
        command.Parameters.AddWithValue("$Version", definition.Version);
        command.Parameters.AddWithValue("$HandlerType", definition.HandlerType);
        command.Parameters.AddWithValue("$HandlerKey", definition.HandlerType);
        command.Parameters.AddWithValue("$InputJson", "{}");
        command.Parameters.AddWithValue("$OutputJson", "{}");
        command.Parameters.AddWithValue("$MetadataJson", "{}");
        command.Parameters.AddWithValue("$IsEnabled", definition.IsEnabled ? 1 : 0);

        command.ExecuteNonQuery();
    }

    public CommandDefinitionRecord? FindByName(string commandName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(commandName);

        using SqliteConnection connection = _connectionFactory.OpenConnection();
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = GetRequiredSqlText(RepositorySqlQueryNames.CommandDefinition.SelectByName);
        command.Parameters.AddWithValue("$CommandName", commandName);

        using SqliteDataReader reader = command.ExecuteReader();
        return reader.Read() ? ReadCommandDefinition(reader) : null;
    }

    public IReadOnlyList<CommandDefinitionRecord> ListAll()
    {
        using SqliteConnection connection = _connectionFactory.OpenConnection();
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = GetRequiredSqlText(RepositorySqlQueryNames.CommandDefinition.SelectAll);

        List<CommandDefinitionRecord> records = [];
        using SqliteDataReader reader = command.ExecuteReader();

        while (reader.Read())
        {
            records.Add(ReadCommandDefinition(reader));
        }

        return records;
    }

    public void UpsertParameter(CommandParameterDefinitionUpsert parameter)
    {
        ArgumentNullException.ThrowIfNull(parameter);
        ArgumentException.ThrowIfNullOrWhiteSpace(parameter.CommandParameterDefinitionId);
        ArgumentException.ThrowIfNullOrWhiteSpace(parameter.CommandDefinitionId);
        ArgumentException.ThrowIfNullOrWhiteSpace(parameter.ParameterName);
        ArgumentException.ThrowIfNullOrWhiteSpace(parameter.ParameterType);

        using SqliteConnection connection = _connectionFactory.OpenConnection();
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = GetRequiredSqlText(RepositorySqlQueryNames.CommandParameterDefinition.InsertOrReplace);

        command.Parameters.AddWithValue("$CommandParameterDefinitionId", parameter.CommandParameterDefinitionId);
        command.Parameters.AddWithValue("$CommandId", parameter.CommandDefinitionId);
        command.Parameters.AddWithValue("$CommandDefinitionId", parameter.CommandDefinitionId);
        command.Parameters.AddWithValue("$ParameterName", parameter.ParameterName);
        command.Parameters.AddWithValue("$ParameterType", parameter.ParameterType);
        command.Parameters.AddWithValue("$IsRequired", parameter.IsRequired ? 1 : 0);
        command.Parameters.AddWithValue("$DefaultValue", parameter.DefaultValue);
        command.Parameters.AddWithValue("$Description", parameter.Description);
        command.Parameters.AddWithValue("$SortOrder", parameter.SortOrder);

        command.ExecuteNonQuery();
    }

    public IReadOnlyList<CommandParameterDefinitionRecord> ListParameters(string commandDefinitionId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(commandDefinitionId);

        using SqliteConnection connection = _connectionFactory.OpenConnection();
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = GetRequiredSqlText(RepositorySqlQueryNames.CommandParameterDefinition.SelectByCommand);
        command.Parameters.AddWithValue("$CommandId", commandDefinitionId);
        command.Parameters.AddWithValue("$CommandDefinitionId", commandDefinitionId);

        List<CommandParameterDefinitionRecord> records = [];
        using SqliteDataReader reader = command.ExecuteReader();

        while (reader.Read())
        {
            records.Add(ReadCommandParameterDefinition(reader));
        }

        return records;
    }

    private string GetRequiredSqlText(string queryName)
    {
        SqlQueryCatalog catalog = new(_connectionFactory);
        return catalog.GetRequiredSqlText(queryName);
    }

    private static CommandDefinitionRecord ReadCommandDefinition(SqliteDataReader reader)
    {
        return new CommandDefinitionRecord(
            reader.GetRequiredString("CommandId"),
            reader.GetRequiredString("CommandName"),
            reader.GetStringOrDefault("DisplayName"),
            reader.GetStringOrDefault("Description"),
            reader.GetStringOrDefault("Category"),
            reader.GetInt32OrDefault("Version", 1),
            reader.GetBooleanOrDefault("IsEnabled"),
            reader.GetStringOrDefault("HandlerType"),
            reader.GetStringOrDefault("CreatedUtc"),
            reader.GetNullableString("UpdatedUtc"));
    }

    private static CommandParameterDefinitionRecord ReadCommandParameterDefinition(SqliteDataReader reader)
    {
        return new CommandParameterDefinitionRecord(
            reader.GetRequiredString("CommandParameterDefinitionId"),
            reader.GetRequiredString("CommandId"),
            reader.GetRequiredString("ParameterName"),
            reader.GetRequiredString("ParameterType"),
            reader.GetBooleanOrDefault("IsRequired"),
            reader.GetStringOrDefault("DefaultValue"),
            reader.GetStringOrDefault("Description"),
            reader.GetInt32OrDefault("SortOrder"),
            reader.GetStringOrDefault("CreatedUtc"),
            reader.GetNullableString("UpdatedUtc"));
    }
}

