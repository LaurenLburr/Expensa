namespace CodexExpensa.Core.Domain.Budgets;

public sealed class BudgetTemplateRow
{
    public int TemplateRowId { get; }
    public string Name { get; }
    public int SortIndex { get; }
    public decimal DefaultAmount { get; }
    public bool IsActive { get; }

    public BudgetTemplateRow(
        int templateRowId,
        string name,
        int sortIndex,
        decimal defaultAmount,
        bool isActive)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Template row name cannot be empty.", nameof(name));

        TemplateRowId = templateRowId;
        Name = name.Trim();
        SortIndex = sortIndex;
        DefaultAmount = defaultAmount;
        IsActive = isActive;
    }
}