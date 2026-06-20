namespace PayeesAddin;

public sealed class PayeeScreenRequest
{
    public string DatabasePath { get; init; } = string.Empty;

    public string NodeId { get; init; } = string.Empty;

    public string NodeType { get; init; } = string.Empty;

    public string EntityId { get; init; } = string.Empty;

    public string DisplayText { get; init; } = string.Empty;
}
