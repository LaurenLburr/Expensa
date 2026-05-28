namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration;

public sealed class CommandExecutionHistoryRecord
{
    public DateTimeOffset StartedUtc { get; init; }

    public DateTimeOffset CompletedUtc { get; init; }

    public TimeSpan Duration => CompletedUtc - StartedUtc;

    public required string CommandName { get; init; }

    public string CorrelationId { get; init; } = string.Empty;

    public string Status { get; init; } = string.Empty;

    public string Message { get; init; } = string.Empty;

    public string ParameterJson { get; init; } = "{}";

    public string OutputJson { get; init; } = string.Empty;
}
