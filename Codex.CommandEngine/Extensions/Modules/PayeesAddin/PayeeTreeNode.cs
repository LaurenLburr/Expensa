namespace PayeesAddin;

public sealed class PayeeTreeNode
{
    public required string NodeId { get; init; }
    public required string DisplayText { get; init; }
    public required string NodeType { get; init; }
    public string PayeeId { get; init; } = string.Empty;
    public string PayeeName { get; init; } = string.Empty;
    public bool IsActive { get; init; } = true;
    public IReadOnlyList<PayeeTreeNode> Children { get; init; } = [];
}
