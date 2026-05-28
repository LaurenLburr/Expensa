using System.Data;

namespace Codex.CommandEngine.Data;

// PATCH MARKER: ExecutionHistory legacy-compatible insert v4 reads actual kind/target.
public sealed class ExecutionHistoryRepository
{
    private static readonly IReadOnlyDictionary<string, IReadOnlyList<string>> RequiredSchema =
        new Dictionary<string, IReadOnlyList<string>>(StringComparer.OrdinalIgnoreCase)
        {
            ["ExecutionHistory"] =
            [
                "ExecutionId",
                "CorrelationId",
                "ExecutionKind",
                "TargetId",
                "TargetName",
                "CommandName",
                "Status",
                "StartedUtc",
                "CompletedUtc",
                "DurationMilliseconds",
                "RequestJson",
                "OutputJson",
                "Message",
                "ErrorMessage",
                "ExceptionText"
            ],
            ["ExecutionStepHistory"] =
            [
                "ExecutionStepHistoryId",
                "ExecutionId",
                "StepOrder",
                "Status",
                "StartedUtc",
                "CompletedUtc",
                "CommandDefinitionId",
                "WorkflowStepDefinitionId",
                "ErrorMessage"
            ]
        };

    private readonly CommandEngineConnectionFactory _connectionFactory;
    private readonly DataTableQueryExecutor _executor;

    public ExecutionHistoryRepository(CommandEngineConnectionFactory connectionFactory)
    {
        ArgumentNullException.ThrowIfNull(connectionFactory);

        _connectionFactory = connectionFactory;
        _executor = new DataTableQueryExecutor(connectionFactory);
        EnsureSchema();
    }

    public void Start(ExecutionHistoryStart start)
    {
        ArgumentNullException.ThrowIfNull(start);
        ArgumentException.ThrowIfNullOrWhiteSpace(start.ExecutionId);
        ArgumentException.ThrowIfNullOrWhiteSpace(start.CorrelationId);
        ArgumentException.ThrowIfNullOrWhiteSpace(start.CommandName);

        EnsureSchema();

        _executor.ExecuteNonQuery(
            """
            INSERT INTO ExecutionHistory (
                ExecutionId,
                CorrelationId,
                ExecutionKind,
                TargetId,
                TargetName,
                CommandName,
                Status,
                StartedUtc,
                CompletedUtc,
                DurationMilliseconds,
                RequestJson,
                OutputJson,
                Message,
                ErrorMessage,
                ExceptionText
            )
            VALUES (
                $ExecutionId,
                $CorrelationId,
                $ExecutionKind,
                $TargetId,
                $TargetName,
                $CommandName,
                $Status,
                $StartedUtc,
                NULL,
                NULL,
                $RequestJson,
                '{}',
                '',
                '',
                NULL
            );
            """,
            new Dictionary<string, object?>
            {
                ["ExecutionId"] = start.ExecutionId,
                ["CorrelationId"] = start.CorrelationId,
                ["ExecutionKind"] = "Command",
                ["TargetId"] = start.CommandName,
                ["TargetName"] = start.CommandName,
                ["CommandName"] = start.CommandName,
                ["Status"] = "Started",
                ["StartedUtc"] = start.StartedUtc,
                ["RequestJson"] = start.RequestJson
            });
    }

    public void Complete(ExecutionHistoryCompletion completion)
    {
        ArgumentNullException.ThrowIfNull(completion);
        ArgumentException.ThrowIfNullOrWhiteSpace(completion.ExecutionId);
        ArgumentException.ThrowIfNullOrWhiteSpace(completion.Status);

        EnsureSchema();

        _executor.ExecuteNonQuery(
            """
            UPDATE ExecutionHistory
            SET Status = $Status,
                CompletedUtc = $CompletedUtc,
                DurationMilliseconds = $DurationMilliseconds,
                OutputJson = $OutputJson,
                Message = $Message,
                ErrorMessage = COALESCE($ExceptionText, ''),
                ExceptionText = $ExceptionText
            WHERE ExecutionId = $ExecutionId;
            """,
            new Dictionary<string, object?>
            {
                ["ExecutionId"] = completion.ExecutionId,
                ["Status"] = completion.Status,
                ["CompletedUtc"] = completion.CompletedUtc,
                ["DurationMilliseconds"] = completion.DurationMilliseconds,
                ["OutputJson"] = completion.OutputJson,
                ["Message"] = completion.Message,
                ["ExceptionText"] = completion.ExceptionText
            });
    }

    public ExecutionHistoryRecord? FindByExecutionId(string executionId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(executionId);

        EnsureSchema();

        DataTable table =
            _executor.ExecuteQuery(
                """
                SELECT
                    ExecutionId,
                    CorrelationId,
                    CommandName,
                    Status,
                    StartedUtc,
                    CompletedUtc,
                    DurationMilliseconds,
                    RequestJson,
                    OutputJson,
                    Message,
                    ExceptionText,
                    ExecutionKind,
                    TargetId,
                    TargetName,
                    ErrorMessage
                FROM ExecutionHistory
                WHERE ExecutionId = $ExecutionId;
                """,
                new Dictionary<string, object?>
                {
                    ["ExecutionId"] = executionId
                });

        return DataTableMapper.MapSingleOrDefault(table, ReadRecord);
    }

    public IReadOnlyList<ExecutionHistoryRecord> ListRecent(int limit = 100)
    {
        if (limit <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(limit), limit, "Limit must be greater than zero.");
        }

        EnsureSchema();

        DataTable table =
            _executor.ExecuteQuery(
                """
                SELECT
                    ExecutionId,
                    CorrelationId,
                    CommandName,
                    Status,
                    StartedUtc,
                    CompletedUtc,
                    DurationMilliseconds,
                    RequestJson,
                    OutputJson,
                    Message,
                    ExceptionText,
                    ExecutionKind,
                    TargetId,
                    TargetName,
                    ErrorMessage
                FROM ExecutionHistory
                ORDER BY StartedUtc DESC
                LIMIT $Limit;
                """,
                new Dictionary<string, object?>
                {
                    ["Limit"] = limit
                });

        return DataTableMapper.MapRows(table, ReadRecord);
    }

    public IReadOnlyList<ExecutionStepHistoryRecord> ListSteps(string executionId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(executionId);

        EnsureSchema();

        DataTable table =
            _executor.ExecuteQuery(
                """
                SELECT
                    ExecutionStepHistoryId,
                    ExecutionId,
                    StepOrder,
                    Status,
                    StartedUtc,
                    CompletedUtc,
                    CommandDefinitionId,
                    WorkflowStepDefinitionId,
                    ErrorMessage
                FROM ExecutionStepHistory
                WHERE ExecutionId = $ExecutionId
                ORDER BY StepOrder ASC;
                """,
                new Dictionary<string, object?>
                {
                    ["ExecutionId"] = executionId
                });

        return DataTableMapper.MapRows(table, ReadStepRecord);
    }

    private void EnsureSchema()
    {
        DatabaseSchemaGuard.RequireTablesAndColumns(_connectionFactory, RequiredSchema);
    }

    private static ExecutionHistoryRecord ReadRecord(DataRow row)
    {
        long? durationMilliseconds = null;
        string? durationText = row.GetNullableString("DurationMilliseconds");

        if (!string.IsNullOrWhiteSpace(durationText))
        {
            durationMilliseconds = row.GetInt32OrDefault("DurationMilliseconds");
        }

        return new ExecutionHistoryRecord(
            row.GetRequiredString("ExecutionId"),
            row.GetRequiredString("CorrelationId"),
            row.GetRequiredString("CommandName"),
            row.GetRequiredString("Status"),
            row.GetRequiredString("StartedUtc"),
            row.GetNullableString("CompletedUtc"),
            durationMilliseconds,
            row.GetStringOrDefault("RequestJson", "{}"),
            row.GetStringOrDefault("OutputJson", "{}"),
            row.GetStringOrDefault("Message"),
            row.GetNullableString("ExceptionText"),
            row.GetStringOrDefault("ExecutionKind", "Command"),
            row.GetStringOrDefault("TargetId"),
            row.GetStringOrDefault("TargetName"),
            row.GetStringOrDefault("ErrorMessage"));
    }

    private static ExecutionStepHistoryRecord ReadStepRecord(DataRow row)
    {
        return new ExecutionStepHistoryRecord(
            row.GetRequiredString("ExecutionStepHistoryId"),
            row.GetRequiredString("ExecutionId"),
            row.GetInt32OrDefault("StepOrder"),
            row.GetRequiredString("Status"),
            row.GetRequiredString("StartedUtc"),
            row.GetNullableString("CompletedUtc"),
            row.GetNullableString("CommandDefinitionId"),
            row.GetNullableString("WorkflowStepDefinitionId"),
            row.GetStringOrDefault("ErrorMessage"));
    }
}
