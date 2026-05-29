INSERT OR REPLACE INTO SqlQuery
(
    QueryName,
    Description,
    SqlText,
    Category,
    IsActive
)
VALUES
(
    'CommandExecution.QueryRecords.Base',
    'Base query shape for persisted command execution explorer records. Runtime code adds filters and ordering.',
    'SELECT
        ExecutionId,
        SourceKind,
        CommandName,
        ParameterJson,
        Status,
        Message,
        CorrelationId,
        OutputJson,
        CreatedUtc,
        StartedUtc,
        CompletedUtc,
        IsHistory,
        IsQueueItem
    FROM CommandExecution
    WHERE 1 = 1',
    'CommandExecution',
    1
);
