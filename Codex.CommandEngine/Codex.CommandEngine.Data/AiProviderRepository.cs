using Microsoft.Data.Sqlite;

namespace Codex.CommandEngine.Data;

public sealed class AiProviderRepository
{
    private readonly CommandEngineConnectionFactory _connectionFactory;

    public AiProviderRepository(CommandEngineConnectionFactory connectionFactory)
    {
        ArgumentNullException.ThrowIfNull(connectionFactory);
        _connectionFactory = connectionFactory;
    }

    public void Upsert(AiProviderUpsert provider)
    {
        ArgumentNullException.ThrowIfNull(provider);
        ArgumentException.ThrowIfNullOrWhiteSpace(provider.AiProviderId);
        ArgumentException.ThrowIfNullOrWhiteSpace(provider.ProviderName);

        using SqliteConnection connection = _connectionFactory.OpenConnection();
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = GetRequiredSqlText(RepositorySqlQueryNames.AiProvider.InsertOrReplace);

        command.Parameters.AddWithValue("$AiProviderId", provider.AiProviderId);
        command.Parameters.AddWithValue("$ProviderName", provider.ProviderName);
        command.Parameters.AddWithValue("$DisplayName", provider.DisplayName);
        command.Parameters.AddWithValue("$ProviderKind", provider.ProviderKind);
        command.Parameters.AddWithValue("$Description", provider.Description);
        command.Parameters.AddWithValue("$ConfigurationJson", provider.ConfigurationJson);
        command.Parameters.AddWithValue("$MetadataJson", provider.MetadataJson);
        command.Parameters.AddWithValue("$IsEnabled", provider.IsEnabled ? 1 : 0);

        command.ExecuteNonQuery();
    }

    public AiProviderRecord? FindByName(string providerName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(providerName);

        using SqliteConnection connection = _connectionFactory.OpenConnection();
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = GetRequiredSqlText(RepositorySqlQueryNames.AiProvider.SelectByName);
        command.Parameters.AddWithValue("$ProviderName", providerName);

        using SqliteDataReader reader = command.ExecuteReader();
        return reader.Read() ? ReadProvider(reader) : null;
    }

    public IReadOnlyList<AiProviderRecord> ListAll()
    {
        using SqliteConnection connection = _connectionFactory.OpenConnection();
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = GetRequiredSqlText(RepositorySqlQueryNames.AiProvider.SelectAll);

        List<AiProviderRecord> records = [];
        using SqliteDataReader reader = command.ExecuteReader();

        while (reader.Read())
        {
            records.Add(ReadProvider(reader));
        }

        return records;
    }

    public void UpsertCapability(AiProviderCapabilityUpsert capability)
    {
        ArgumentNullException.ThrowIfNull(capability);
        ArgumentException.ThrowIfNullOrWhiteSpace(capability.AiProviderCapabilityId);
        ArgumentException.ThrowIfNullOrWhiteSpace(capability.AiProviderId);
        ArgumentException.ThrowIfNullOrWhiteSpace(capability.CapabilityName);

        using SqliteConnection connection = _connectionFactory.OpenConnection();
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = GetRequiredSqlText(RepositorySqlQueryNames.AiProviderCapability.InsertOrReplace);

        command.Parameters.AddWithValue("$AiProviderCapabilityId", capability.AiProviderCapabilityId);
        command.Parameters.AddWithValue("$AiProviderId", capability.AiProviderId);
        command.Parameters.AddWithValue("$CapabilityName", capability.CapabilityName);
        command.Parameters.AddWithValue("$Description", capability.Description);
        command.Parameters.AddWithValue("$MetadataJson", capability.MetadataJson);
        command.Parameters.AddWithValue("$IsEnabled", capability.IsEnabled ? 1 : 0);

        command.ExecuteNonQuery();
    }

    public IReadOnlyList<AiProviderCapabilityRecord> ListCapabilities(string aiProviderId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(aiProviderId);

        using SqliteConnection connection = _connectionFactory.OpenConnection();
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = GetRequiredSqlText(RepositorySqlQueryNames.AiProviderCapability.SelectByProvider);
        command.Parameters.AddWithValue("$AiProviderId", aiProviderId);

        List<AiProviderCapabilityRecord> records = [];
        using SqliteDataReader reader = command.ExecuteReader();

        while (reader.Read())
        {
            records.Add(ReadCapability(reader));
        }

        return records;
    }

    private string GetRequiredSqlText(string queryName)
    {
        SqlQueryCatalog catalog = new(_connectionFactory);
        return catalog.GetRequiredSqlText(queryName);
    }

    private static AiProviderRecord ReadProvider(SqliteDataReader reader)
    {
        return new AiProviderRecord(
            reader.GetRequiredString("AiProviderId"),
            reader.GetRequiredString("ProviderName"),
            reader.GetStringOrDefault("DisplayName"),
            reader.GetStringOrDefault("ProviderKind"),
            reader.GetStringOrDefault("Description"),
            reader.GetStringOrDefault("ConfigurationJson", "{}"),
            reader.GetStringOrDefault("MetadataJson", "{}"),
            reader.GetBooleanOrDefault("IsEnabled"),
            reader.GetStringOrDefault("CreatedUtc"),
            reader.GetNullableString("UpdatedUtc"));
    }

    private static AiProviderCapabilityRecord ReadCapability(SqliteDataReader reader)
    {
        return new AiProviderCapabilityRecord(
            reader.GetRequiredString("AiProviderCapabilityId"),
            reader.GetRequiredString("AiProviderId"),
            reader.GetRequiredString("CapabilityName"),
            reader.GetStringOrDefault("Description"),
            reader.GetStringOrDefault("MetadataJson", "{}"),
            reader.GetBooleanOrDefault("IsEnabled"),
            reader.GetStringOrDefault("CreatedUtc"),
            reader.GetNullableString("UpdatedUtc"));
    }
(SqliteDataReader reader, int ordinal)
    {
        string value = GetStringOrDefault(reader, ordinal);

        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException($"Required string value was missing at ordinal {ordinal}.");
        }

        return value;
    }

    private static string GetStringOrDefault(SqliteDataReader reader, int ordinal, string defaultValue = "")
    {
        if (ordinal >= reader.FieldCount || reader.IsDBNull(ordinal))
        {
            return defaultValue;
        }

        return reader.GetString(ordinal);
    }

    private static string? GetNullableString(SqliteDataReader reader, int ordinal)
    {
        if (ordinal >= reader.FieldCount || reader.IsDBNull(ordinal))
        {
            return null;
        }

        return reader.GetString(ordinal);
    }

    private static bool GetBoolean(SqliteDataReader reader, int ordinal)
    {
        if (ordinal >= reader.FieldCount || reader.IsDBNull(ordinal))
        {
            return false;
        }

        return reader.GetInt32(ordinal) == 1;
    }
}
