using System;

namespace CodexExpensa.Core.Abstractions;

/// <summary>
/// Central place for building stable credential manager keys.
/// Prevents string duplication and guarantees consistent naming.
/// </summary>
public static class CredentialKeys
{
    private const string Prefix = "CodexExpensa";

    /// <summary>
    /// Bank-level login (most common case).
    /// One credential per bank.
    /// </summary>
    public static string Bank(string bankId)
    {
        Validate(bankId);
        return $"{Prefix}.Bank.{bankId}";
    }

    /// <summary>
    /// Account-level login (if a specific account uses different credentials).
    /// </summary>
    public static string Account(string accountId)
    {
        Validate(accountId);
        return $"{Prefix}.Account.{accountId}";
    }

    /// <summary>
    /// Optional: user-scoped bank credential.
    /// Useful if multi-user support is added later.
    /// </summary>
    public static string BankForUser(string bankId, string userId)
    {
        Validate(bankId);
        Validate(userId);
        return $"{Prefix}.User.{userId}.Bank.{bankId}";
    }

    private static void Validate(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Value is required.", nameof(value));
    }
}