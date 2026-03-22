namespace Codex.Data.SQLiteEngine.Configuration;

public sealed class SqliteEngineOptions
{
    public SqlCatalogOptions? SqlCatalog { get; init; }
    public LoggingOptions? Logging { get; init; }
}

public sealed class SqlCatalogOptions
{
    public required string TableName { get; init; }
    public string NameColumn { get; init; } = "QueryName";
    public string SqlColumn { get; init; } = "Sql";
    public TransactionParticipationMode TransactionParticipation { get; init; } =
        TransactionParticipationMode.UseTransactionWhenAvailable;
}

public sealed class LoggingOptions
{
    public required string TableName { get; init; }
    public string TimestampColumn { get; init; } = "Timestamp";
    public string ActionColumn { get; init; } = "Action";
    public string DetailsColumn { get; init; } = "Details";
    public TransactionParticipationMode TransactionParticipation { get; init; } =
        TransactionParticipationMode.UseConnectionOnly;
}
