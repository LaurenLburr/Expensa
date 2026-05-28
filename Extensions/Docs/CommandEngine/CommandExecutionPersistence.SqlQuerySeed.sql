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
    'CommandExecution.CreateTable',
    'Creates the persistent command execution table used for queue and history records.',
    'CREATE TABLE IF NOT EXISTS CommandExecution (
        ExecutionId   TEXT PRIMARY KEY,
        SourceKind    TEXT NOT NULL DEFAULT (''''),
        CommandName   TEXT NOT NULL DEFAULT (''''),
        ParameterJson TEXT NOT NULL DEFAULT (''{}''),
        Status        TEXT NOT NULL DEFAULT (''''),
        Message       TEXT NOT NULL DEFAULT (''''),
        CorrelationId TEXT NOT NULL DEFAULT (''''),
        OutputJson    TEXT NOT NULL DEFAULT (''''),
        CreatedUtc    TEXT NOT NULL DEFAULT (strftime(''%Y-%m-%dT%H:%M:%fZ'',''now'')),
        StartedUtc    TEXT NULL,
        CompletedUtc  TEXT NULL,
        IsHistory     INTEGER NOT NULL DEFAULT (0),
        IsQueueItem   INTEGER NOT NULL DEFAULT (0)
    );',
    'CommandExecution',
    1
);
