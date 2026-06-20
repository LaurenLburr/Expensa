using System;
using System.Collections.Generic;
using CodexExpensa.Core.Domain.Transactions;
using CodexExpensa.Data.Sqlite.Db;
using Microsoft.Data.Sqlite;

namespace CodexExpensa.Data.Sqlite.Transactions;

public sealed class SqliteTransactionRepository : ITransactionRepository
{
    private readonly SqliteDatabase _db;

    public SqliteTransactionRepository(SqliteDatabase db)
    {
        _db = db ?? throw new ArgumentNullException(nameof(db));
    }

    public IReadOnlyList<Transaction> GetByAccountId(string accountId)
    {
        if (string.IsNullOrWhiteSpace(accountId))
            throw new ArgumentException("accountId is required.", nameof(accountId));

        const string sql =
            """
            SELECT
                TransactionId,
                AccountId,
                PayeeId,
                Status,
                Amount,
                StartDate,
                ConfirmationNumber AS Confirm,
                Note
            FROM Txn
            WHERE AccountId = @AccountId
            ORDER BY StartDate, TransactionId;
            """;

        return _db.Query(
            sql,
            MapTxn,
            new[] { new SqliteParameter("@AccountId", accountId) });
    }

    public void Add(Transaction txn)
    {
        if (txn is null) throw new ArgumentNullException(nameof(txn));
        ValidateForWrite(txn);

        const string sql =
            """
            INSERT INTO Txn
            (
                AccountId,
                PayeeId,
                Status,
                Amount,
                StartDate,
                ConfirmationNumber,
                Note
            )
            VALUES
            (
                @AccountId,
                @PayeeId,
                @Status,
                @Amount,
                @StartDate,
                @ConfirmationNumber,
                @Note
            );
            """;

        _db.ExecuteNonQuery(sql, new[]
        {
            new SqliteParameter("@AccountId", txn.AccountId),
            new SqliteParameter("@PayeeId", (object?)txn.PayeeId ?? DBNull.Value),
            new SqliteParameter("@Status", txn.Status.ToString()),
            new SqliteParameter("@Amount", txn.Amount),
            new SqliteParameter("@StartDate", txn.StartDate.ToString("yyyy-MM-dd")),
            new SqliteParameter("@ConfirmationNumber", (object?)txn.Confirm ?? DBNull.Value),
            new SqliteParameter("@Note", (object?)txn.Note ?? DBNull.Value),
        });
    }

    public void Update(Transaction txn)
    {
        if (txn is null) throw new ArgumentNullException(nameof(txn));
        ValidateForWrite(txn);

        if (txn.TransactionId <= 0)
            throw new ArgumentException("TransactionId must be set for update.", nameof(txn));

        const string sql =
            """
            UPDATE Txn
            SET
                AccountId = @AccountId,
                PayeeId = @PayeeId,
                Status = @Status,
                Amount = @Amount,
                StartDate = @StartDate,
                ConfirmationNumber = @ConfirmationNumber,
                Note = @Note
            WHERE TransactionId = @TransactionId;
            """;

        _db.ExecuteNonQuery(sql, new[]
        {
            new SqliteParameter("@TransactionId", txn.TransactionId),
            new SqliteParameter("@AccountId", txn.AccountId),
            new SqliteParameter("@PayeeId", (object?)txn.PayeeId ?? DBNull.Value),
            new SqliteParameter("@Status", txn.Status.ToString()),
            new SqliteParameter("@Amount", txn.Amount),
            new SqliteParameter("@StartDate", txn.StartDate.ToString("yyyy-MM-dd")),
            new SqliteParameter("@ConfirmationNumber", (object?)txn.Confirm ?? DBNull.Value),
            new SqliteParameter("@Note", (object?)txn.Note ?? DBNull.Value),
        });
    }

    public void Delete(int transactionId)
    {
        if (transactionId <= 0)
            throw new ArgumentException("transactionId must be > 0.", nameof(transactionId));

        const string sql =
            """
            DELETE FROM Txn
            WHERE TransactionId = @TransactionId;
            """;

        _db.ExecuteNonQuery(sql, new[] { new SqliteParameter("@TransactionId", transactionId) });
    }

    private static void ValidateForWrite(Transaction txn)
    {
        if (string.IsNullOrWhiteSpace(txn.AccountId))
            throw new ArgumentException("AccountId is required.", nameof(txn));

        // Amount can be 0 legitimately (e.g. placeholder/entry in progress), so no validation here.
        // StartDate required:
        if (txn.StartDate == default)
            throw new ArgumentException("StartDate is required.", nameof(txn));
    }

    private static Transaction MapTxn(SqliteDataReader r)
    {
        var id = GetInt(r, "TransactionId", 0);
        var accountId = GetRequiredString(r, "AccountId");
        var payeeId = GetNullableString(r, "PayeeId");
        TransactionStatus status =
            GetStatus(r, "Status");
        var amount = GetDecimal(r, "Amount", 0m);
        var startDateText = GetRequiredString(r, "StartDate");
        var confirm = GetNullableString(r, "Confirm");
        var note = GetNullableString(r, "Note");

        if (!DateTime.TryParse(startDateText, out var startDate))
            startDate = DateTime.Today;

        return new Transaction
        {
            TransactionId = id,
            AccountId = accountId,
            PayeeId = payeeId,
            Status = status,
            Amount = amount,
            StartDate = startDate.Date,
            Confirm = confirm,
            Note = note
        };
    }

    private static string GetRequiredString(SqliteDataReader reader, string columnName)
    {
        var ord = reader.GetOrdinal(columnName);
        if (reader.IsDBNull(ord))
            throw new InvalidOperationException($"Column '{columnName}' was NULL but is required.");

        var value = reader.GetString(ord);
        if (string.IsNullOrWhiteSpace(value))
            throw new InvalidOperationException($"Column '{columnName}' was empty/whitespace but is required.");

        return value;
    }

    private static string? GetNullableString(SqliteDataReader reader, string columnName)
    {
        var ord = reader.GetOrdinal(columnName);
        return reader.IsDBNull(ord) ? null : reader.GetString(ord);
    }

    private static int GetInt(SqliteDataReader reader, string columnName, int defaultValue)
    {
        var ord = reader.GetOrdinal(columnName);
        if (reader.IsDBNull(ord))
            return defaultValue;

        try { return Convert.ToInt32(reader.GetValue(ord)); }
        catch { return defaultValue; }
    }

    private static TransactionStatus GetStatus(SqliteDataReader reader, string columnName)
    {
        var ord = reader.GetOrdinal(columnName);
        if (reader.IsDBNull(ord))
            return TransactionStatus.Outstanding;

        object value = reader.GetValue(ord);

        if (value is string text)
        {
            if (Enum.TryParse(text, ignoreCase: true, out TransactionStatus parsed))
                return parsed;

            if (int.TryParse(text, out int numeric) &&
                Enum.IsDefined(typeof(TransactionStatus), numeric))
            {
                return (TransactionStatus)numeric;
            }
        }

        try
        {
            int numeric = Convert.ToInt32(value);
            return Enum.IsDefined(typeof(TransactionStatus), numeric)
                ? (TransactionStatus)numeric
                : TransactionStatus.Outstanding;
        }
        catch
        {
            return TransactionStatus.Outstanding;
        }
    }

    private static decimal GetDecimal(SqliteDataReader reader, string columnName, decimal defaultValue)
    {
        var ord = reader.GetOrdinal(columnName);
        if (reader.IsDBNull(ord))
            return defaultValue;

        try { return Convert.ToDecimal(reader.GetValue(ord)); }
        catch { return defaultValue; }
    }
}
