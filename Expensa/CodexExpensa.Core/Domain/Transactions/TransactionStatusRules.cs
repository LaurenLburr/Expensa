using System.Collections.Generic;

namespace CodexExpensa.Core.Domain.Transactions;

public static class TransactionStatusRules
{
    private static readonly Dictionary<TransactionStatus, HashSet<TransactionStatus>> AllowedTransitions =
        new()
        {
            {
                TransactionStatus.Projected,
                new HashSet<TransactionStatus>
                {
                    TransactionStatus.Outstanding,
                    TransactionStatus.Invalid
                }
            },
            {
                TransactionStatus.Outstanding,
                new HashSet<TransactionStatus>
                {
                    TransactionStatus.Cleared,
                    TransactionStatus.Invalid
                }
            },
            {
                TransactionStatus.Cleared,
                new HashSet<TransactionStatus>
                {
                    TransactionStatus.Invalid
                }
            },
            {
                TransactionStatus.Invalid,
                new HashSet<TransactionStatus>
                {
                    TransactionStatus.Projected
                }
            }
        };

    public static bool IsValidTransition(TransactionStatus from, TransactionStatus to)
    {
        if (from == to)
            return true;

        if (!AllowedTransitions.TryGetValue(from, out HashSet<TransactionStatus>? allowed))
            return false;

        return allowed.Contains(to);
    }

    public static IReadOnlyCollection<TransactionStatus> GetAllowedTransitions(TransactionStatus from)
    {
        if (AllowedTransitions.TryGetValue(from, out HashSet<TransactionStatus>? allowed))
            return allowed;

        return [];
    }
}
