namespace CodexExpensa.Core.Domain.Accounts;

/// <summary>
/// Domain model representing a user financial account.
/// </summary>
public sealed class Account
{
    public required string AccountId { get; init; }

    public required string AccountNickname { get; init; }

    public required int SortIndex { get; init; }

    public required string BankId { get; init; }

    /// <summary>
    /// Stored as text to preserve leading zeros.
    /// </summary>
    public required string AccountNumber { get; init; }

    // Projection fields from JOIN for display
    public required string BankName { get; init; }

    public required string RoutingNumber { get; init; }

    public string? Url { get; init; }

    public required AccountType AccountType { get; init; }

    public bool IsActive { get; init; } = true;
}