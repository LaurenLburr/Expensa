using System.Data;
using System.Text.Json;

namespace Codex.CommandEngine.Data;

public sealed class WorkflowDefinitionRepository : IWorkflowDefinitionStore
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
            ],
            ["WorkflowDefinitionStep"] =
            [
                "WorkflowDefinitionStepId",
                "WorkflowDefinitionId",
                "StepName",
                "StepOrder",
                "CommandName",
                "ParametersJson",
                "IsEnabled",
                "CommandDefinitionId",
                "InputMapJson",
                "CreatedUtc",
                "UpdatedUtc"
            ]
        };

    private readonly CommandEngineConnectionFactory _connectionFactory;
    private readonly DataTableQueryExecutor _executor;

    public WorkflowDefinitionRepository(CommandEngineConnectionFactory connectionFactory)
    {
        ArgumentNullException.ThrowIfNull(connectionFactory);

        _connectionFactory = connectionFactory;
        _executor = new DataTableQueryExecutor(connectionFactory);

        EnsureSchema();
    }

    public void UpsertWorkflow(WorkflowDefinitionUpsert workflow)
    {
        ArgumentNullException.ThrowIfNull(workflow);
        ArgumentException.ThrowIfNullOrWhiteSpace(workflow.WorkflowDefinitionId);
        ArgumentException.ThrowIfNullOrWhiteSpace(workflow.WorkflowName);

        EnsureSchema();

        _executor.ExecuteNonQuery(
            """
            INSERT INTO WorkflowDefinition (
                WorkflowId,
                WorkflowDefinitionId,
                WorkflowName,
                DisplayName,
                Description,
                Version,
                MetadataJson,
                IsEnabled,
                IsActive
            )
            VALUES (
                $WorkflowDefinitionId,
                $WorkflowDefinitionId,
                $WorkflowName,
                $DisplayName,
                $Description,
                $Version,
                '{}',
                $IsActive,
                $IsActive
            )
            ON CONFLICT(WorkflowName) DO UPDATE SET
                WorkflowDefinitionId = excluded.WorkflowDefinitionId,
                DisplayName = excluded.DisplayName,
                Description = excluded.Description,
                Version = excluded.Version,
                IsEnabled = excluded.IsEnabled,
                IsActive = excluded.IsActive,
                UpdatedUtc = CURRENT_TIMESTAMP;
            """,
            new Dictionary<string, object?>
            {
                ["WorkflowDefinitionId"] = workflow.WorkflowDefinitionId,
                ["WorkflowName"] = workflow.WorkflowName,
                ["DisplayName"] = workflow.DisplayName,
                ["Description"] = workflow.Description,
                ["Version"] = workflow.Version,
                ["IsActive"] = workflow.IsActive ? 1 : 0
            });
    }

    public void UpsertStep(WorkflowDefinitionStepUpsert step)
    {
        ArgumentNullException.ThrowIfNull(step);
        ArgumentException.ThrowIfNullOrWhiteSpace(step.WorkflowDefinitionStepId);
        ArgumentException.ThrowIfNullOrWhiteSpace(step.WorkflowDefinitionId);
        ArgumentException.ThrowIfNullOrWhiteSpace(step.StepName);
        ArgumentException.ThrowIfNullOrWhiteSpace(step.CommandName);

        EnsureSchema();

        _executor.ExecuteNonQuery(
            """
            INSERT INTO WorkflowDefinitionStep (
                WorkflowDefinitionStepId,
                WorkflowDefinitionId,
                StepName,
                StepOrder,
                CommandName,
                ParametersJson,
                IsEnabled,
                CommandDefinitionId,
                InputMapJson
            )
            VALUES (
                $WorkflowDefinitionStepId,
                $WorkflowDefinitionId,
                $StepName,
                $StepOrder,
                $CommandName,
                $ParametersJson,
                1,
                $CommandName,
                $ParametersJson
            )
            ON CONFLICT(WorkflowDefinitionId, StepOrder) DO UPDATE SET
                WorkflowDefinitionStepId = excluded.WorkflowDefinitionStepId,
                StepName = excluded.StepName,
                CommandName = excluded.CommandName,
                ParametersJson = excluded.ParametersJson,
                IsEnabled = excluded.IsEnabled,
                CommandDefinitionId = excluded.CommandDefinitionId,
                InputMapJson = excluded.InputMapJson,
                UpdatedUtc = CURRENT_TIMESTAMP;
            """,
            new Dictionary<string, object?>
            {
                ["WorkflowDefinitionStepId"] = step.WorkflowDefinitionStepId,
                ["WorkflowDefinitionId"] = step.WorkflowDefinitionId,
                ["StepName"] = step.StepName,
                ["StepOrder"] = step.StepOrder,
                ["CommandName"] = step.CommandName,
                ["ParametersJson"] = step.ParametersJson
            });
    }

    public WorkflowDefinitionDocument? FindByName(string workflowName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(workflowName);
        EnsureSchema();

        DataTable workflowTable =
            _executor.ExecuteQuery(
                """
                SELECT
                    WorkflowDefinitionId,
                    WorkflowName,
                    DisplayName,
                    Description,
                    Version,
                    IsActive
                FROM WorkflowDefinition
                WHERE WorkflowName = $WorkflowName;
                """,
                new Dictionary<string, object?>
                {
                    ["WorkflowName"] = workflowName
                });

        return workflowTable.Rows.Count == 0
            ? null
            : ToDocument(ReadWorkflowRecord(workflowTable.Rows[0]));
    }

    public IReadOnlyList<WorkflowDefinitionDocument> ListActive()
    {
        return ListActiveRecords()
            .Select(ToDocument)
            .ToList();
    }

    public IReadOnlyList<WorkflowDefinitionRecord> ListActiveRecords()
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
                    IsActive
                FROM WorkflowDefinition
                WHERE IsActive = 1
                ORDER BY WorkflowName ASC;
                """);

        return DataTableMapper.MapRows(table, ReadWorkflowRecord);
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
                    IsActive
                FROM WorkflowDefinition
                ORDER BY WorkflowName ASC;
                """);

        return DataTableMapper.MapRows(table, ReadWorkflowRecord);
    }

    public IReadOnlyList<WorkflowStepDefinitionRecord> ListSteps(string workflowDefinitionId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(workflowDefinitionId);
        EnsureSchema();

        DataTable table =
            _executor.ExecuteQuery(
                """
                SELECT
                    WorkflowDefinitionStepId,
                    WorkflowDefinitionId,
                    StepName,
                    StepOrder,
                    CommandName,
                    ParametersJson,
                    IsEnabled,
                    CommandDefinitionId,
                    InputMapJson
                FROM WorkflowDefinitionStep
                WHERE WorkflowDefinitionId = $WorkflowDefinitionId
                ORDER BY StepOrder ASC;
                """,
                new Dictionary<string, object?>
                {
                    ["WorkflowDefinitionId"] = workflowDefinitionId
                });

        return DataTableMapper.MapRows(table, ReadStepRecord);
    }

    public IReadOnlyList<WorkflowDefinitionStep> ListDocumentSteps(string workflowDefinitionId)
    {
        return ListSteps(workflowDefinitionId)
            .Select(ToDocumentStep)
            .ToList();
    }

    private void EnsureSchema()
    {
        DatabaseSchemaGuard.RequireTablesAndColumns(_connectionFactory, RequiredSchema);
    }

    private WorkflowDefinitionDocument ToDocument(WorkflowDefinitionRecord record)
    {
        return new WorkflowDefinitionDocument
        {
            WorkflowDefinitionId = record.WorkflowDefinitionId,
            WorkflowName = record.WorkflowName,
            DisplayName = record.DisplayName,
            Description = record.Description,
            Version = record.Version,
            IsActive = record.IsActive,
            Steps = ListDocumentSteps(record.WorkflowDefinitionId)
        };
    }

    private static WorkflowDefinitionStep ToDocumentStep(WorkflowStepDefinitionRecord record)
    {
        return new WorkflowDefinitionStep
        {
            StepName = record.StepName,
            StepOrder = record.StepOrder,
            CommandName = record.CommandName,
            Parameters = DeserializeParameters(record.InputMapJson)
        };
    }

    private static WorkflowDefinitionRecord ReadWorkflowRecord(DataRow row)
    {
        return new WorkflowDefinitionRecord(
            row.GetRequiredString("WorkflowDefinitionId"),
            row.GetRequiredString("WorkflowName"),
            row.GetStringOrDefault("DisplayName"),
            row.GetStringOrDefault("Description"),
            row.GetInt32OrDefault("Version"),
            row.GetBooleanOrDefault("IsActive", true));
    }

    private static WorkflowStepDefinitionRecord ReadStepRecord(DataRow row)
    {
        return new WorkflowStepDefinitionRecord(
            row.GetRequiredString("WorkflowDefinitionStepId"),
            row.GetRequiredString("WorkflowDefinitionId"),
            row.GetRequiredString("StepName"),
            row.GetInt32OrDefault("StepOrder"),
            row.GetRequiredString("CommandName"),
            row.GetStringOrDefault("ParametersJson", "{}"))
        {
            IsEnabled = row.GetBooleanOrDefault("IsEnabled", true),
            CommandDefinitionId = row.GetStringOrDefault("CommandDefinitionId", row.GetRequiredString("CommandName")),
            InputMapJson = row.GetStringOrDefault("InputMapJson", row.GetStringOrDefault("ParametersJson", "{}"))
        };
    }

    private static IReadOnlyDictionary<string, object?> DeserializeParameters(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return new Dictionary<string, object?>();
        }

        Dictionary<string, JsonElement>? values =
            JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json);

        if (values is null)
        {
            return new Dictionary<string, object?>();
        }

        Dictionary<string, object?> result =
            new(StringComparer.OrdinalIgnoreCase);

        foreach (KeyValuePair<string, JsonElement> value in values)
        {
            result[value.Key] = value.Value.ValueKind switch
            {
                JsonValueKind.String => value.Value.GetString(),
                JsonValueKind.Number when value.Value.TryGetInt64(out long longValue) => longValue,
                JsonValueKind.Number when value.Value.TryGetDouble(out double doubleValue) => doubleValue,
                JsonValueKind.True => true,
                JsonValueKind.False => false,
                JsonValueKind.Null => null,
                _ => value.Value.GetRawText()
            };
        }

        return result;
    }
}
