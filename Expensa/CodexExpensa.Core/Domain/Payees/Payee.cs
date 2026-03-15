namespace CodexExpensa.Core.Domain.Payees;

public sealed class Payee
{
    public string PayeeId { get; set; } = string.Empty;

    public string PayeeName { get; set; } = string.Empty;

    public int SortIndex { get; set; }

    public bool IsActive { get; set; } = true;

    /// <summary>
    /// If true this payee will appear automatically when a new
    /// monthly budget is created from the template.
    /// </summary>
    public bool IncludeInBudgetTemplate { get; set; }

    /// <summary>
    /// Default bank account normally used to pay this payee.
    /// Can be overridden in individual budget months.
    /// </summary>
    public string? DefaultAccountId { get; set; }

    /// <summary>
    /// Suggested default monthly amount.
    /// Used when creating a budget from template.
    /// </summary>
    public decimal? DefaultAmount { get; set; }
}