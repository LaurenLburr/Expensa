using System.Data;
using Microsoft.Data.Sqlite;

namespace Codex.CommandEngine.Data;

public sealed class CommandDefinitionBrowserRepository
{
    private static readonly IReadOnlyDictionary<string, IReadOnlyList<string>> RequiredSchema =
        new Dictionary<string, IReadOnlyList<string>>(StringComparer.OrdinalIgnoreCase)
        {
            ["CommandDefinition"] =
            [
                "CommandId",
                "CommandDefinitionId",
                "CommandName",
                "DisplayName",
                "Description",
                "Category",
                "Version",
                "HandlerKey",
                "HandlerType",
                "IsEnabled",
                "CreatedUtc",
                "UpdatedUtc"
            ]
        };

    private readonly CommandEngineConnectionFactory _connectionFactory;

    public CommandDefinitionBrowserRepository(CommandEngineConnectionFactory connectionFactory)
    {
        ArgumentNullException.ThrowIfNull(connectionFactory);

        _connectionFactory = connectionFactory;
        EnsureSchema();
    }

    public IReadOnlyList<CommandDefinitionRecord> ListAll()
    {
        EnsureSchema();

        using SqliteConnection connection = _connectionFactory.OpenConnection();
        using SqliteCommand command = connection.CreateCommand();

        command.CommandText =
            """
            SELECT
                CommandDefinitionId,
                CommandName,
                DisplayName,
                Description,
                Category,
                Version,
                IsEnabled,
                HandlerType,
                CreatedUtc,
                UpdatedUtc
            FROM CommandDefinition
            ORDER BY Category, CommandName;
            """;

        DataTable table = SqliteDataTableLoader.Load(command);

        List<CommandDefinitionRecord> records = [];

        foreach (DataRow row in table.Rows)
        {
            records.Add(new CommandDefinitionRecord(
                row.GetRequiredString("CommandDefinitionId"),
                row.GetRequiredString("CommandName"),
                row.GetStringOrDefault("DisplayName"),
                row.GetStringOrDefault("Description"),
                row.GetStringOrDefault("Category"),
                row.GetInt32OrDefault("Version", 1),
                row.GetBooleanOrDefault("IsEnabled", true),
                row.GetStringOrDefault("HandlerType"),
                row.GetRequiredString("CreatedUtc"),
                row.GetNullableString("UpdatedUtc")));
        }

        return records;
    }

    private void EnsureSchema()
    {
        DatabaseSchemaGuard.RequireTablesAndColumns(_connectionFactory, RequiredSchema);
    }
}
