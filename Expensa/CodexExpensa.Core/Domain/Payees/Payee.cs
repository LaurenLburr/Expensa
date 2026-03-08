namespace CodexExpensa.Core.Domain.Payees;

public sealed class Payee
{
    public required string PayeeId { get; init; }

    public required string PayeeName { get; set; }

    public bool IncludeInBudgetTemplate { get; set; }

    public int SortIndex { get; set; }

    public bool IsActive { get; set; } = true;

    public string? WebsiteId { get; set; }
}