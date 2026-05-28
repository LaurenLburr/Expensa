using System.Data;

namespace Codex.CommandEngine.Data;

public sealed class WorkflowDefinitionBrowserRepository
{
    private static readonly IReadOnlyDictionary<string, IReadOnlyList<string>> RequiredSchema =
        new Dictionary<string, IReadOnlyList<string>>(StringComparer.OrdinalIgnoreCase)
        {
            ["WorkflowDefinition"] =
            [
                "WorkflowId",
                "WorkflowDefinitionId",
                "WorkflowName",
                "DisplayName",
                "Description",
                "Version",
                "MetadataJson",
                "IsEnabled",
                "IsActive",
                "CreatedUtc",
                "UpdatedUtc"
            ]
        };

    private readonly CommandEngineConnectionFactory _connectionFactory;
    private readonly DataTableQueryExecutor _executor;

    public WorkflowDefinitionBrowserRepository(CommandEngineConnectionFactory connectionFactory)
    {
        ArgumentNullException.ThrowIfNull(connectionFactory);

        _connectionFactory = connectionFactory;
        _executor = new DataTableQueryExecutor(connectionFactory);

        EnsureSchema();
    }

    public IReadOnlyList<WorkflowDefinitionRecord> ListAll()
    {
        EnsureSchema();

        DataTable table =
            _executor.ExecuteQuery(
                """
                SELECT
                    WorkflowDefinitionId,
                    WorkflowName,
                    DisplayName,
                    Description,
                    Version,
                    IsActive,
                    CreatedUtc,
                    UpdatedUtc,
                    MetadataJson
                FROM WorkflowDefinition
                ORDER BY WorkflowName ASC, Version DESC;
                """);

        return DataTableMapper.MapRows(table, ReadWorkflow);
    }

    private void EnsureSchema()
    {
        DatabaseSchemaGuard.RequireTablesAndColumns(_connectionFactory, RequiredSchema);
    }

    private static WorkflowDefinitionRecord ReadWorkflow(DataRow row)
    {
        return new WorkflowDefinitionRecord(
            row.GetRequiredString("WorkflowDefinitionId"),
            row.GetRequiredString("WorkflowName"),
            row.GetStringOrDefault("DisplayName"),
            row.GetStringOrDefault("Description"),
            row.GetInt32OrDefault("Version"),
            row.GetBooleanOrDefault("IsActive", true),
            row.GetStringOrDefault("CreatedUtc"),
            row.GetNullableString("UpdatedUtc"),
            row.GetStringOrDefault("MetadataJson", "{}"));
    }
}
