namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration;

public sealed class CommandExecutionPersistentRecordQuery
{
    public CommandExecutionPersistentRecordSourceFilter SourceFilter { get; init; } =
        CommandExecutionPersistentRecordSourceFilter.All;

    public string Status { get; init; } = string.Empty;

    public string SearchText { get; init; } = string.Empty;

    public DateTimeOffset? CreatedFromUtc { get; init; }

    public DateTimeOffset? CreatedToUtc { get; init; }

    public CommandExecutionPersistentRecordSortMode SortMode { get; init; } =
        CommandExecutionPersistentRecordSortMode.NewestFirst;

    public int MaximumRows { get; init; } = 500;
}
