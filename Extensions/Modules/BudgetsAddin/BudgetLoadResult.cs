namespace BudgetsAddin;

public sealed class BudgetLoadResult
{
    public required string Status { get; init; }

    public required string Message { get; init; }

    public required IReadOnlyList<BudgetTreeNode> Nodes { get; init; }

    public int TotalCount =>
        CountNodes(Nodes);

    private static int CountNodes(
        IReadOnlyList<BudgetTreeNode> nodes)
    {
        int count = 0;

        foreach (BudgetTreeNode node in nodes)
        {
            count++;
            count += CountNodes(node.Children);
        }

        return count;
    }
}
