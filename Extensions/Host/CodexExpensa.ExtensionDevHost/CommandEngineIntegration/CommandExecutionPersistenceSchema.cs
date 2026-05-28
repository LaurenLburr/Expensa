namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration;

public static class CommandExecutionPersistenceSchema
{
    public const string CreateCommandExecutionTable = """
    CREATE TABLE IF NOT EXISTS CommandExecution (
        ExecutionId   TEXT PRIMARY KEY,
        SourceKind    TEXT NOT NULL DEFAULT (''),
        CommandName   TEXT NOT NULL DEFAULT (''),
        ParameterJson TEXT NOT NULL DEFAULT ('{}'),
        Status        TEXT NOT NULL DEFAULT (''),
        Message       TEXT NOT NULL DEFAULT (''),
        CorrelationId TEXT NOT NULL DEFAULT (''),
        OutputJson    TEXT NOT NULL DEFAULT (''),
        CreatedUtc    TEXT NOT NULL DEFAULT (strftime('%Y-%m-%dT%H:%M:%fZ','now')),
        StartedUtc    TEXT NULL,
        CompletedUtc  TEXT NULL,
        IsHistory     INTEGER NOT NULL DEFAULT (0),
        IsQueueItem   INTEGER NOT NULL DEFAULT (0)
    );
    """;

    public const string CreateCommandExecutionIndexes = """
    CREATE INDEX IF NOT EXISTS IX_CommandExecution_IsQueueItem_Status_CreatedUtc
        ON CommandExecution (IsQueueItem, Status, CreatedUtc);

    CREATE INDEX IF NOT EXISTS IX_CommandExecution_IsHistory_CompletedUtc
        ON CommandExecution (IsHistory, CompletedUtc);
    """;

    public const string UpsertCommandExecution = """
    INSERT INTO CommandExecution (
        ExecutionId, SourceKind, CommandName, ParameterJson, Status,
        Message, CorrelationId, OutputJson, CreatedUtc, StartedUtc,
        CompletedUtc, IsHistory, IsQueueItem
    )
    VALUES (
        @ExecutionId, @SourceKind, @CommandName, @ParameterJson, @Status,
        @Message, @CorrelationId, @OutputJson, @CreatedUtc, @StartedUtc,
        @CompletedUtc, @IsHistory, @IsQueueItem
    )
    ON CONFLICT(ExecutionId) DO UPDATE SET
        SourceKind = excluded.SourceKind,
        CommandName = excluded.CommandName,
        ParameterJson = excluded.ParameterJson,
        Status = excluded.Status,
        Message = excluded.Message,
        CorrelationId = excluded.CorrelationId,
        OutputJson = excluded.OutputJson,
        CreatedUtc = excluded.CreatedUtc,
        StartedUtc = excluded.StartedUtc,
        CompletedUtc = excluded.CompletedUtc,
        IsHistory = excluded.IsHistory,
        IsQueueItem = excluded.IsQueueItem;
    """;

    public const string SelectQueueItems = """
    SELECT
        ExecutionId, SourceKind, CommandName, ParameterJson, Status,
        Message, CorrelationId, OutputJson, CreatedUtc, StartedUtc,
        CompletedUtc, IsHistory, IsQueueItem
    FROM CommandExecution
    WHERE IsQueueItem = 1
    ORDER BY CreatedUtc DESC;
    """;

    public const string SelectHistoryRecords = """
    SELECT
        ExecutionId, SourceKind, CommandName, ParameterJson, Status,
        Message, CorrelationId, OutputJson, CreatedUtc, StartedUtc,
        CompletedUtc, IsHistory, IsQueueItem
    FROM CommandExecution
    WHERE IsHistory = 1
    ORDER BY CompletedUtc DESC, CreatedUtc DESC;
    """;

    public const string DeleteCompletedQueueItems = """
    DELETE FROM CommandExecution
    WHERE IsQueueItem = 1
      AND Status IN ('Completed', 'Failed', 'Cancelled');
    """;
}
