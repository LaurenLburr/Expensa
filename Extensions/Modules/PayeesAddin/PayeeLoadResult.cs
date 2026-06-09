namespace PayeesAddin;

public sealed class PayeeLoadResult
{
    public required string Status { get; init; }
    public required string Message { get; init; }
    public IReadOnlyList<PayeeTreeNode> Nodes { get; init; } = [];
    public int TotalCount { get; init; }
}
