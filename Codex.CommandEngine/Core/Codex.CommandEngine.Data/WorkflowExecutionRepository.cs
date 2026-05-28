using System.Data;

namespace Codex.CommandEngine.Data;

public sealed class WorkflowExecutionRepository
{
    private static readonly IReadOnlyDictionary<string, IReadOnlyList<string>> RequiredSchema =
        new Dictionary<string, IReadOnlyList<string>>(StringComparer.OrdinalIgnoreCase)
        {
            ["WorkflowExecution"] =
            [
                "WorkflowExecutionId",
                "WorkflowName",
                "CorrelationId",
                "Status",
                "StartedUtc",
                "CompletedUtc",
                "CurrentStepOrder",
                "LastCompletedStepOrder",
                "IsResumable",
                "ResumeToken",
                "LastHeartbeatUtc",
                "RuntimeStateJson",
                "ContextJson",
                "Message"
            ],
            ["WorkflowStepExecution"] =
            [
                "WorkflowStepExecutionId",
                "WorkflowExecutionId",
                "StepName",
                "StepOrder",
                "CommandName",
                "Status",
                "StartedUtc",
                "CompletedUtc",
                "Message",
                "CommandExecutionId"
            ]
        };

    private readonly CommandEngineConnectionFactory _connectionFactory;
    private readonly DataTableQueryExecutor _executor;

    public WorkflowExecutionRepository(CommandEngineConnectionFactory connectionFactory)
    {
        ArgumentNullException.ThrowIfNull(connectionFactory);

        _connectionFactory = connectionFactory;
        _executor = new DataTableQueryExecutor(connectionFactory);
        EnsureSchema();
    }

    public void StartWorkflow(WorkflowExecutionStart start)
    {
        ArgumentNullException.ThrowIfNull(start);
        ArgumentException.ThrowIfNullOrWhiteSpace(start.WorkflowExecutionId);
        ArgumentException.ThrowIfNullOrWhiteSpace(start.WorkflowName);
        ArgumentException.ThrowIfNullOrWhiteSpace(start.CorrelationId);

        EnsureSchema();

        _executor.ExecuteNonQuery(
            """
            INSERT INTO WorkflowExecution (
                WorkflowExecutionId,
                WorkflowName,
                CorrelationId,
                Status,
                StartedUtc,
                CurrentStepOrder,
                LastCompletedStepOrder,
                IsResumable,
                ResumeToken,
                LastHeartbeatUtc,
                RuntimeStateJson,
                ContextJson,
                Message
            )
            VALUES (
                $WorkflowExecutionId,
                $WorkflowName,
                $CorrelationId,
                'Started',
                $StartedUtc,
                0,
                0,
                1,
                $ResumeToken,
                $StartedUtc,
                '{}',
                $ContextJson,
                ''
            );
            """,
            new Dictionary<string, object?>
            {
                ["WorkflowExecutionId"] = start.WorkflowExecutionId,
                ["WorkflowName"] = start.WorkflowName,
                ["CorrelationId"] = start.CorrelationId,
                ["StartedUtc"] = start.StartedUtc,
                ["ContextJson"] = start.ContextJson,
                ["ResumeToken"] = Guid.NewGuid().ToString("N")
            });
    }

    public void CompleteWorkflow(WorkflowExecutionCompletion completion)
    {
        ArgumentNullException.ThrowIfNull(completion);
        ArgumentException.ThrowIfNullOrWhiteSpace(completion.WorkflowExecutionId);
        ArgumentException.ThrowIfNullOrWhiteSpace(completion.Status);

        EnsureSchema();

        _executor.ExecuteNonQuery(
            """
            UPDATE WorkflowExecution
            SET Status = $Status,
                CompletedUtc = $CompletedUtc,
                Message = $Message,
                IsResumable = 0,
                LastHeartbeatUtc = $CompletedUtc
            WHERE WorkflowExecutionId = $WorkflowExecutionId;
            """,
            new Dictionary<string, object?>
            {
                ["WorkflowExecutionId"] = completion.WorkflowExecutionId,
                ["Status"] = completion.Status,
                ["CompletedUtc"] = completion.CompletedUtc,
                ["Message"] = completion.Message
            });
    }

    public void StartStep(WorkflowStepExecutionStart start)
    {
        ArgumentNullException.ThrowIfNull(start);
        ArgumentException.ThrowIfNullOrWhiteSpace(start.WorkflowStepExecutionId);
        ArgumentException.ThrowIfNullOrWhiteSpace(start.WorkflowExecutionId);
        ArgumentException.ThrowIfNullOrWhiteSpace(start.StepName);
        ArgumentException.ThrowIfNullOrWhiteSpace(start.CommandName);

        EnsureSchema();

        _executor.ExecuteNonQuery(
            """
            INSERT INTO WorkflowStepExecution (
                WorkflowStepExecutionId,
                WorkflowExecutionId,
                StepName,
                StepOrder,
                CommandName,
                Status,
                StartedUtc,
                Message
            )
            VALUES (
                $WorkflowStepExecutionId,
                $WorkflowExecutionId,
                $StepName,
                $StepOrder,
                $CommandName,
                'Started',
                $StartedUtc,
                ''
            );

            UPDATE WorkflowExecution
            SET CurrentStepOrder = $StepOrder,
                LastHeartbeatUtc = $StartedUtc
            WHERE WorkflowExecutionId = $WorkflowExecutionId;
            """,
            new Dictionary<string, object?>
            {
                ["WorkflowStepExecutionId"] = start.WorkflowStepExecutionId,
                ["WorkflowExecutionId"] = start.WorkflowExecutionId,
                ["StepName"] = start.StepName,
                ["StepOrder"] = start.StepOrder,
                ["CommandName"] = start.CommandName,
                ["StartedUtc"] = start.StartedUtc
            });
    }

    public void CompleteStep(WorkflowStepExecutionCompletion completion)
    {
        ArgumentNullException.ThrowIfNull(completion);
        ArgumentException.ThrowIfNullOrWhiteSpace(completion.WorkflowStepExecutionId);
        ArgumentException.ThrowIfNullOrWhiteSpace(completion.Status);

        EnsureSchema();

        _executor.ExecuteNonQuery(
            """
            UPDATE WorkflowStepExecution
            SET Status = $Status,
                CompletedUtc = $CompletedUtc,
                Message = $Message,
                CommandExecutionId = $CommandExecutionId
            WHERE WorkflowStepExecutionId = $WorkflowStepExecutionId;

            UPDATE WorkflowExecution
            SET LastCompletedStepOrder = (
                    SELECT COALESCE(MAX(StepOrder), 0)
                    FROM WorkflowStepExecution
                    WHERE WorkflowExecutionId = WorkflowExecution.WorkflowExecutionId
                      AND Status = 'Succeeded'
                ),
                LastHeartbeatUtc = $CompletedUtc
            WHERE WorkflowExecutionId = (
                SELECT WorkflowExecutionId
                FROM WorkflowStepExecution
                WHERE WorkflowStepExecutionId = $WorkflowStepExecutionId
            );
            """,
            new Dictionary<string, object?>
            {
                ["WorkflowStepExecutionId"] = completion.WorkflowStepExecutionId,
                ["Status"] = completion.Status,
                ["CompletedUtc"] = completion.CompletedUtc,
                ["Message"] = completion.Message,
                ["CommandExecutionId"] = completion.CommandExecutionId
            });
    }

    public void UpdateResumeState(WorkflowResumeUpdate update)
    {
        ArgumentNullException.ThrowIfNull(update);
        ArgumentException.ThrowIfNullOrWhiteSpace(update.WorkflowExecutionId);
        ArgumentException.ThrowIfNullOrWhiteSpace(update.ResumeToken);

        EnsureSchema();

        _executor.ExecuteNonQuery(
            """
            UPDATE WorkflowExecution
            SET LastCompletedStepOrder = $LastCompletedStepOrder,
                RuntimeStateJson = $RuntimeStateJson,
                IsResumable = $IsResumable,
                ResumeToken = $ResumeToken,
                LastHeartbeatUtc = $LastHeartbeatUtc
            WHERE WorkflowExecutionId = $WorkflowExecutionId;
            """,
            new Dictionary<string, object?>
            {
                ["WorkflowExecutionId"] = update.WorkflowExecutionId,
                ["LastCompletedStepOrder"] = update.LastCompletedStepOrder,
                ["RuntimeStateJson"] = update.RuntimeStateJson,
                ["IsResumable"] = update.IsResumable ? 1 : 0,
                ["ResumeToken"] = update.ResumeToken,
                ["LastHeartbeatUtc"] = DateTimeOffset.UtcNow.ToString("O")
            });
    }

    public void Heartbeat(string workflowExecutionId, string heartbeatUtc)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(workflowExecutionId);
        ArgumentException.ThrowIfNullOrWhiteSpace(heartbeatUtc);

        EnsureSchema();

        _executor.ExecuteNonQuery(
            """
            UPDATE WorkflowExecution
            SET LastHeartbeatUtc = $LastHeartbeatUtc
            WHERE WorkflowExecutionId = $WorkflowExecutionId;
            """,
            new Dictionary<string, object?>
            {
                ["WorkflowExecutionId"] = workflowExecutionId,
                ["LastHeartbeatUtc"] = heartbeatUtc
            });
    }

    public void MarkAbandoned(string workflowExecutionId, string message)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(workflowExecutionId);

        EnsureSchema();

        _executor.ExecuteNonQuery(
            """
            UPDATE WorkflowExecution
            SET Status = 'Abandoned',
                IsResumable = 0,
                CompletedUtc = $CompletedUtc,
                Message = $Message,
                LastHeartbeatUtc = $CompletedUtc
            WHERE WorkflowExecutionId = $WorkflowExecutionId;
            """,
            new Dictionary<string, object?>
            {
                ["WorkflowExecutionId"] = workflowExecutionId,
                ["CompletedUtc"] = DateTimeOffset.UtcNow.ToString("O"),
                ["Message"] = message
            });
    }

    public WorkflowExecutionRecord? FindWorkflow(string workflowExecutionId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(workflowExecutionId);

        EnsureSchema();

        DataTable table =
            _executor.ExecuteQuery(
                """
                SELECT
                    WorkflowExecutionId,
                    WorkflowName,
                    CorrelationId,
                    Status,
                    StartedUtc,
                    CompletedUtc,
                    CurrentStepOrder,
                    ContextJson,
                    Message
                FROM WorkflowExecution
                WHERE WorkflowExecutionId = $WorkflowExecutionId;
                """,
                new Dictionary<string, object?>
                {
                    ["WorkflowExecutionId"] = workflowExecutionId
                });

        return DataTableMapper.MapSingleOrDefault(table, ReadWorkflow);
    }

    public WorkflowResumeState? FindResumeState(string workflowExecutionId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(workflowExecutionId);

        EnsureSchema();

        DataTable table =
            _executor.ExecuteQuery(
                """
                SELECT
                    WorkflowExecutionId,
                    WorkflowName,
                    CorrelationId,
                    Status,
                    CurrentStepOrder,
                    LastCompletedStepOrder,
                    IsResumable,
                    ResumeToken,
                    RuntimeStateJson,
                    LastHeartbeatUtc
                FROM WorkflowExecution
                WHERE WorkflowExecutionId = $WorkflowExecutionId;
                """,
                new Dictionary<string, object?>
                {
                    ["WorkflowExecutionId"] = workflowExecutionId
                });

        return DataTableMapper.MapSingleOrDefault(table, ReadResumeState);
    }

    public IReadOnlyList<WorkflowExecutionRecord> ListIncomplete()
    {
        EnsureSchema();

        DataTable table =
            _executor.ExecuteQuery(
                """
                SELECT
                    WorkflowExecutionId,
                    WorkflowName,
                    CorrelationId,
                    Status,
                    StartedUtc,
                    CompletedUtc,
                    CurrentStepOrder,
                    ContextJson,
                    Message
                FROM WorkflowExecution
                WHERE Status NOT IN ('Succeeded', 'Failed', 'Cancelled', 'Abandoned')
                ORDER BY StartedUtc ASC;
                """);

        return DataTableMapper.MapRows(table, ReadWorkflow);
    }

    public IReadOnlyList<WorkflowResumeState> ListResumable()
    {
        EnsureSchema();

        DataTable table =
            _executor.ExecuteQuery(
                """
                SELECT
                    WorkflowExecutionId,
                    WorkflowName,
                    CorrelationId,
                    Status,
                    CurrentStepOrder,
                    LastCompletedStepOrder,
                    IsResumable,
                    ResumeToken,
                    RuntimeStateJson,
                    LastHeartbeatUtc
                FROM WorkflowExecution
                WHERE IsResumable = 1
                  AND Status NOT IN ('Succeeded', 'Failed', 'Cancelled', 'Abandoned')
                ORDER BY StartedUtc ASC;
                """);

        return DataTableMapper.MapRows(table, ReadResumeState);
    }

    public IReadOnlyList<WorkflowStepExecutionRecord> ListSteps(string workflowExecutionId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(workflowExecutionId);

        EnsureSchema();

        DataTable table =
            _executor.ExecuteQuery(
                """
                SELECT
                    WorkflowStepExecutionId,
                    WorkflowExecutionId,
                    StepName,
                    StepOrder,
                    CommandName,
                    Status,
                    StartedUtc,
                    CompletedUtc,
                    Message,
                    CommandExecutionId
                FROM WorkflowStepExecution
                WHERE WorkflowExecutionId = $WorkflowExecutionId
                ORDER BY StepOrder ASC;
                """,
                new Dictionary<string, object?>
                {
                    ["WorkflowExecutionId"] = workflowExecutionId
                });

        return DataTableMapper.MapRows(table, ReadStep);
    }

    private void EnsureSchema()
    {
        DatabaseSchemaGuard.RequireTablesAndColumns(_connectionFactory, RequiredSchema);
    }

    private static WorkflowExecutionRecord ReadWorkflow(DataRow row)
    {
        return new WorkflowExecutionRecord(
            row.GetRequiredString("WorkflowExecutionId"),
            row.GetRequiredString("WorkflowName"),
            row.GetRequiredString("CorrelationId"),
            row.GetRequiredString("Status"),
            row.GetRequiredString("StartedUtc"),
            row.GetNullableString("CompletedUtc"),
            row.GetInt32OrDefault("CurrentStepOrder"),
            row.GetStringOrDefault("ContextJson", "{}"),
            row.GetStringOrDefault("Message"));
    }

    private static WorkflowResumeState ReadResumeState(DataRow row)
    {
        return new WorkflowResumeState(
            row.GetRequiredString("WorkflowExecutionId"),
            row.GetRequiredString("WorkflowName"),
            row.GetRequiredString("CorrelationId"),
            row.GetRequiredString("Status"),
            row.GetInt32OrDefault("CurrentStepOrder"),
            row.GetInt32OrDefault("LastCompletedStepOrder"),
            row.GetBooleanOrDefault("IsResumable", true),
            row.GetStringOrDefault("ResumeToken"),
            row.GetStringOrDefault("RuntimeStateJson", "{}"),
            row.GetNullableString("LastHeartbeatUtc"));
    }

    private static WorkflowStepExecutionRecord ReadStep(DataRow row)
    {
        return new WorkflowStepExecutionRecord(
            row.GetRequiredString("WorkflowStepExecutionId"),
            row.GetRequiredString("WorkflowExecutionId"),
            row.GetRequiredString("StepName"),
            row.GetInt32OrDefault("StepOrder"),
            row.GetRequiredString("CommandName"),
            row.GetRequiredString("Status"),
            row.GetRequiredString("StartedUtc"),
            row.GetNullableString("CompletedUtc"),
            row.GetStringOrDefault("Message"),
            row.GetNullableString("CommandExecutionId"));
    }
}
