using CodexExpensa.Core.Domain.Transactions;

namespace CodexExpensa.App.WinForms.UI.Budgets;

public sealed class BudgetRow
{
    public string BudgetMonthPayeeId { get; set; } = string.Empty;

    public string BudgetMonthId { get; set; } = string.Empty;

    public string PayeeId { get; set; } = string.Empty;

    public string PayeeName { get; set; } = string.Empty;

    public int SortIndex { get; set; }

    public decimal PlannedAmount { get; set; }

    public string? AccountId { get; set; }

    public string? AccountName { get; set; }

    public string? TransactionId { get; set; }

    public decimal? ActualAmount { get; set; }

    public bool IsCleared { get; set; }

    public string Group { get; set; } = string.Empty;

    public string Confirm { get; set; } = string.Empty;

    // REAL transaction
    public Transaction? Transaction { get; set; }

    public string Status
    {
        get
        {
            if (Transaction == null)
            {
                if (PlannedAmount > 0)
                    return "Projected";

                return string.Empty;
            }

            return Transaction.Status switch
            {
                TransactionStatus.Projected => "Projected",
                TransactionStatus.Outstanding => "Outstanding",
                TransactionStatus.Cleared => "Cleared",
                _ => string.Empty
            };
        }
    }

    public string ConfirmDisplay
    {
        get
        {
            if (Transaction == null)
                return string.Empty;

            return Transaction.Confirm ?? string.Empty;
        }
    }
}