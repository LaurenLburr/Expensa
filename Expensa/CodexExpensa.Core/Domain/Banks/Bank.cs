namespace CodexExpensa.Core.Domain.Banks;

/// <summary>
/// Domain model representing a bank.
/// </summary>
public sealed class Bank
{
    public required string BankId { get; init; }

    public required string BankName { get; init; }

    /// <summary>
    /// Stored as text to preserve leading zeros.
    /// </summary>
    public required string RoutingNumber { get; init; }

    public string? Url { get; init; }

    public bool IsActive { get; init; } = true;
}