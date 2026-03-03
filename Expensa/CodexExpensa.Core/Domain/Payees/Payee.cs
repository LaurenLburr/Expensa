namespace CodexExpensa.Core.Domain.Payees;

public sealed class Payee
{
    public required string PayeeId { get; init; }

    public required string PayeeName { get; init; }
}