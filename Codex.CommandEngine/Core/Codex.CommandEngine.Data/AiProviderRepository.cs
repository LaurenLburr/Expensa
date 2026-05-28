using System.Data;
using Microsoft.Data.Sqlite;

namespace Codex.CommandEngine.Data;

public sealed class AiProviderRepository
{
    private static readonly IReadOnlyDictionary<string, IReadOnlyList<string>> RequiredSchema =
        new Dictionary<string, IReadOnlyList<string>>(StringComparer.OrdinalIgnoreCase)
        {
            ["AiProvider"] =
            [
                "AiProviderId",
                "ProviderName",
                "DisplayName",
                "ProviderKind",
                "Description",
                "ConfigurationJson",
                "BaseUrl",
                "ApiKeyEnvironmentVar",
                "MetadataJson",
                "IsEnabled",
                "CreatedUtc",
                "UpdatedUtc"
            ],
            ["AiProviderCapability"] =
            [
                "AiProviderCapabilityId",
                "AiProviderId",
                "CapabilityName",
                "CapabilityKind",
                "CapabilityValue",
                "Description",
                "MetadataJson",
                "IsEnabled",
                "CreatedUtc",
                "UpdatedUtc"
            ]
        };

    private readonly CommandEngineConnectionFactory _connectionFactory;

    public AiProviderRepository(CommandEngineConnectionFactory connectionFactory)
    {
        ArgumentNullException.ThrowIfNull(connectionFactory);

        _connectionFactory = connectionFactory;
        EnsureSchema();
    }

    public void Upsert(AiProviderUpsert provider)
    {
        ArgumentNullException.ThrowIfNull(provider);
        ArgumentException.ThrowIfNullOrWhiteSpace(provider.AiProviderId);
        ArgumentException.ThrowIfNullOrWhiteSpace(provider.ProviderName);

        EnsureSchema();

        using SqliteConnection connection = _connectionFactory.OpenConnection();
        using SqliteCommand command = connection.CreateCommand();

        command.CommandText =
            """
            INSERT INTO AiProvider (
                AiProviderId,
                ProviderName,
                DisplayName,
                ProviderKind,
                Description,
                ConfigurationJson,
                MetadataJson,
                IsEnabled
            )
            VALUES (
                $AiProviderId,
                $ProviderName,
                $DisplayName,
                $ProviderKind,
                $Description,
                $ConfigurationJson,
                $MetadataJson,
                $IsEnabled
            )
            ON CONFLICT(ProviderName) DO UPDATE SET
                DisplayName = excluded.DisplayName,
                ProviderKind = excluded.ProviderKind,
                Description = excluded.Description,
                ConfigurationJson = excluded.ConfigurationJson,
                MetadataJson = excluded.MetadataJson,
                IsEnabled = excluded.IsEnabled,
                UpdatedUtc = CURRENT_TIMESTAMP;
            """;

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

        EnsureSchema();

        using SqliteConnection connection = _connectionFactory.OpenConnection();
        using SqliteCommand command = connection.CreateCommand();

        command.CommandText =
            """
            SELECT
                AiProviderId,
                ProviderName,
                DisplayName,
                ProviderKind,
                Description,
                ConfigurationJson,
                MetadataJson,
                IsEnabled,
                CreatedUtc,
                UpdatedUtc
            FROM AiProvider
            WHERE ProviderName = $ProviderName;
            """;

        command.Parameters.AddWithValue("$ProviderName", providerName);

        DataTable table = SqliteDataTableLoader.Load(command);

        if (table.Rows.Count == 0)
        {
            return null;
        }

        return ReadProvider(table.Rows[0]);
    }

    public IReadOnlyList<AiProviderRecord> ListAll()
    {
        EnsureSchema();

        using SqliteConnection connection = _connectionFactory.OpenConnection();
        using SqliteCommand command = connection.CreateCommand();

        command.CommandText =
            """
            SELECT
                AiProviderId,
                ProviderName,
                DisplayName,
                ProviderKind,
                Description,
                ConfigurationJson,
                MetadataJson,
                IsEnabled,
                CreatedUtc,
                UpdatedUtc
            FROM AiProvider
            ORDER BY ProviderName ASC;
            """;

        DataTable table = SqliteDataTableLoader.Load(command);

        List<AiProviderRecord> providers = [];

        foreach (DataRow row in table.Rows)
        {
            providers.Add(ReadProvider(row));
        }

        return providers;
    }

    public void UpsertCapability(AiProviderCapabilityUpsert capability)
    {
        ArgumentNullException.ThrowIfNull(capability);
        ArgumentException.ThrowIfNullOrWhiteSpace(capability.AiProviderCapabilityId);
        ArgumentException.ThrowIfNullOrWhiteSpace(capability.AiProviderId);
        ArgumentException.ThrowIfNullOrWhiteSpace(capability.CapabilityName);

        EnsureSchema();

        using SqliteConnection connection = _connectionFactory.OpenConnection();
        using SqliteCommand command = connection.CreateCommand();

        command.CommandText =
            """
            INSERT INTO AiProviderCapability (
                AiProviderCapabilityId,
                AiProviderId,
                CapabilityName,
                CapabilityKind,
                CapabilityValue,
                Description,
                MetadataJson,
                IsEnabled
            )
            VALUES (
                $AiProviderCapabilityId,
                $AiProviderId,
                $CapabilityName,
                $CapabilityKind,
                $CapabilityValue,
                $Description,
                $MetadataJson,
                $IsEnabled
            )
            ON CONFLICT(AiProviderId, CapabilityName) DO UPDATE SET
                CapabilityKind = excluded.CapabilityKind,
                CapabilityValue = excluded.CapabilityValue,
                Description = excluded.Description,
                MetadataJson = excluded.MetadataJson,
                IsEnabled = excluded.IsEnabled,
                UpdatedUtc = CURRENT_TIMESTAMP;
            """;

        command.Parameters.AddWithValue("$AiProviderCapabilityId", capability.AiProviderCapabilityId);
        command.Parameters.AddWithValue("$AiProviderId", capability.AiProviderId);
        command.Parameters.AddWithValue("$CapabilityName", capability.CapabilityName);
        command.Parameters.AddWithValue("$CapabilityKind", capability.CapabilityKind);
        command.Parameters.AddWithValue("$CapabilityValue", capability.CapabilityName);
        command.Parameters.AddWithValue("$Description", capability.Description);
        command.Parameters.AddWithValue("$MetadataJson", capability.MetadataJson);
        command.Parameters.AddWithValue("$IsEnabled", capability.IsEnabled ? 1 : 0);

        command.ExecuteNonQuery();
    }

    public IReadOnlyList<AiProviderCapabilityRecord> ListCapabilities(string aiProviderId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(aiProviderId);

        EnsureSchema();

        using SqliteConnection connection = _connectionFactory.OpenConnection();
        using SqliteCommand command = connection.CreateCommand();

        command.CommandText =
            """
            SELECT
                AiProviderCapabilityId,
                AiProviderId,
                CapabilityName,
                CapabilityKind,
                Description,
                MetadataJson,
                IsEnabled,
                CreatedUtc,
                UpdatedUtc
            FROM AiProviderCapability
            WHERE AiProviderId = $AiProviderId
            ORDER BY CapabilityName ASC;
            """;

        command.Parameters.AddWithValue("$AiProviderId", aiProviderId);

        DataTable table = SqliteDataTableLoader.Load(command);

        List<AiProviderCapabilityRecord> capabilities = [];

        foreach (DataRow row in table.Rows)
        {
            capabilities.Add(ReadCapability(row));
        }

        return capabilities;
    }

    private void EnsureSchema()
    {
        DatabaseSchemaGuard.RequireTablesAndColumns(_connectionFactory, RequiredSchema);
    }

    private static AiProviderRecord ReadProvider(DataRow row)
    {
        return new AiProviderRecord(
            row.GetRequiredString("AiProviderId"),
            row.GetRequiredString("ProviderName"),
            row.GetStringOrDefault("DisplayName"),
            row.GetStringOrDefault("ProviderKind"),
            row.GetStringOrDefault("Description"),
            row.GetStringOrDefault("ConfigurationJson", "{}"),
            row.GetStringOrDefault("MetadataJson", "{}"),
            row.GetBooleanOrDefault("IsEnabled", true),
            row.GetRequiredString("CreatedUtc"),
            row.GetNullableString("UpdatedUtc"));
    }

    private static AiProviderCapabilityRecord ReadCapability(DataRow row)
    {
        return new AiProviderCapabilityRecord(
            row.GetRequiredString("AiProviderCapabilityId"),
            row.GetRequiredString("AiProviderId"),
            row.GetRequiredString("CapabilityName"),
            row.GetStringOrDefault("CapabilityKind"),
            row.GetStringOrDefault("Description"),
            row.GetStringOrDefault("MetadataJson", "{}"),
            row.GetBooleanOrDefault("IsEnabled", true),
            row.GetRequiredString("CreatedUtc"),
            row.GetNullableString("UpdatedUtc"));
    }
}
