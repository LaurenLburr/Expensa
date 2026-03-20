using CodexExpensa.Core.Domain.Transactions;
using CodexExpensa.Data.Sqlite.Db;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;

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
                ConfirmationNumber,
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
        if (txn is null)
            throw new ArgumentNullException(nameof(txn));

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
            new SqliteParameter("@ConfirmationNumber", (object?)txn.ConfirmationNumber ?? DBNull.Value),
            new SqliteParameter("@Note", (object?)txn.Note ?? DBNull.Value),
        });

        const string identitySql =
            """
            SELECT last_insert_rowid();
            """;

        IReadOnlyList<int> ids = _db.Query(
            identitySql,
            reader => Convert.ToInt32(reader.GetInt64(0)),
            Array.Empty<SqliteParameter>());

        if (ids.Count == 0 || ids[0] <= 0)
            throw new InvalidOperationException("Failed to retrieve TransactionId after insert.");

        txn.SetTransactionId(ids[0]);
    }

    public void Update(Transaction txn)
    {
        if (txn is null)
            throw new ArgumentNullException(nameof(txn));

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
            new SqliteParameter("@ConfirmationNumber", (object?)txn.ConfirmationNumber ?? DBNull.Value),
            new SqliteParameter("@Note", (object?)txn.Note ?? DBNull.Value),
        });
    }

    public void ChangeStatus(
        int transactionId,
        TransactionStatus newStatus,
        TransactionChangeReason reasonCode,
        string? reasonText,
        string source)
    {
        if (transactionId <= 0)
            throw new ArgumentException("transactionId must be > 0.", nameof(transactionId));

        if (string.IsNullOrWhiteSpace(source))
            throw new ArgumentException("source is required.", nameof(source));

        const string getCurrentSql =
            """
            SELECT
                TransactionId,
                AccountId,
                PayeeId,
                Status,
                Amount,
                StartDate,
                ConfirmationNumber,
                Note
            FROM Txn
            WHERE TransactionId = @TransactionId;
            """;

        IReadOnlyList<Transaction> rows = _db.Query(
            getCurrentSql,
            MapTxn,
            new[] { new SqliteParameter("@TransactionId", transactionId) });

        if (rows.Count == 0)
            throw new InvalidOperationException($"Transaction {transactionId} was not found.");

        Transaction existing = rows[0];
        TransactionStatus oldStatus = existing.Status;

        if (oldStatus == newStatus)
            return;

        if (!TransactionStatusRules.IsValidTransition(oldStatus, newStatus))
        {
            throw new InvalidOperationException(
                $"Invalid status transition from {oldStatus} to {newStatus}.");
        }

        const string updateSql =
            """
            UPDATE Txn
            SET
                Status = @Status
            WHERE TransactionId = @TransactionId;
            """;

        _db.ExecuteNonQuery(updateSql, new[]
        {
            new SqliteParameter("@TransactionId", transactionId),
            new SqliteParameter("@Status", newStatus.ToString())
        });

        const string logSql =
            """
            INSERT INTO TxnStatusLog
            (
                TransactionId,
                OldStatus,
                NewStatus,
                ReasonCode,
                ReasonText,
                ChangedUtc,
                ChangedBy,
                Source
            )
            VALUES
            (
                @TransactionId,
                @OldStatus,
                @NewStatus,
                @ReasonCode,
                @ReasonText,
                @ChangedUtc,
                @ChangedBy,
                @Source
            );
            """;

        _db.ExecuteNonQuery(logSql, new[]
        {
            new SqliteParameter("@TransactionId", transactionId),
            new SqliteParameter("@OldStatus", oldStatus.ToString()),
            new SqliteParameter("@NewStatus", newStatus.ToString()),
            new SqliteParameter("@ReasonCode", reasonCode.ToString()),
            new SqliteParameter("@ReasonText", (object?)reasonText ?? DBNull.Value),
            new SqliteParameter("@ChangedUtc", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")),
            new SqliteParameter("@ChangedBy", DBNull.Value),
            new SqliteParameter("@Source", source)
        });
    }

    public IReadOnlyList<TxnStatusLog> GetStatusHistory(int transactionId)
    {
        if (transactionId <= 0)
            throw new ArgumentException("transactionId must be > 0.", nameof(transactionId));

        const string sql =
            """
            SELECT
                TxnStatusLogId,
                TransactionId,
                OldStatus,
                NewStatus,
                ReasonCode,
                ReasonText,
                ChangedUtc,
                ChangedBy,
                Source
            FROM TxnStatusLog
            WHERE TransactionId = @TransactionId
            ORDER BY ChangedUtc DESC, TxnStatusLogId DESC;
            """;

        return _db.Query(
            sql,
            MapStatusLog,
            new[] { new SqliteParameter("@TransactionId", transactionId) });
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

        _db.ExecuteNonQuery(sql, new[]
        {
            new SqliteParameter("@TransactionId", transactionId)
        });
    }

    private static void ValidateForWrite(Transaction txn)
    {
        if (string.IsNullOrWhiteSpace(txn.AccountId))
            throw new ArgumentException("AccountId is required.", nameof(txn));

        if (txn.StartDate == default)
            throw new ArgumentException("StartDate is required.", nameof(txn));
    }

    private static Transaction MapTxn(SqliteDataReader reader)
    {
        int id = GetInt(reader, "TransactionId", 0);
        string accountId = GetRequiredString(reader, "AccountId");
        string? payeeId = GetNullableString(reader, "PayeeId");
        string statusText = GetRequiredString(reader, "Status");
        decimal amount = GetDecimal(reader, "Amount", 0m);
        string startDateText = GetRequiredString(reader, "StartDate");
        string? confirmationNumber = GetNullableString(reader, "ConfirmationNumber");
        string? note = GetNullableString(reader, "Note");

        if (!Enum.TryParse(statusText, out TransactionStatus status))
            status = TransactionStatus.Projected;

        if (!DateTime.TryParse(startDateText, out DateTime startDate))
            startDate = DateTime.Today;

        Transaction txn = new()
        {
            AccountId = accountId,
            PayeeId = payeeId,
            Status = status,
            Amount = amount,
            StartDate = startDate.Date,
            ConfirmationNumber = confirmationNumber,
            Note = note
        };

        if (id > 0)
            txn.SetTransactionId(id);

        return txn;
    }

    private static TxnStatusLog MapStatusLog(SqliteDataReader reader)
    {
        int id = GetInt(reader, "TxnStatusLogId", 0);
        int transactionId = GetInt(reader, "TransactionId", 0);
        string? oldStatus = GetNullableString(reader, "OldStatus");
        string newStatus = GetRequiredString(reader, "NewStatus");
        string reasonCode = GetRequiredString(reader, "ReasonCode");
        string? reasonText = GetNullableString(reader, "ReasonText");
        string changedUtcText = GetRequiredString(reader, "ChangedUtc");
        string? changedBy = GetNullableString(reader, "ChangedBy");
        string? source = GetNullableString(reader, "Source");

        if (!DateTime.TryParse(changedUtcText, out DateTime changedUtc))
            changedUtc = DateTime.UtcNow;

        return new TxnStatusLog
        {
            TxnStatusLogId = id,
            TransactionId = transactionId,
            OldStatus = oldStatus,
            NewStatus = newStatus,
            ReasonCode = reasonCode,
            ReasonText = reasonText,
            ChangedUtc = changedUtc,
            ChangedBy = changedBy,
            Source = source
        };
    }

    private static string GetRequiredString(SqliteDataReader reader, string columnName)
    {
        int ord = reader.GetOrdinal(columnName);

        if (reader.IsDBNull(ord))
            throw new InvalidOperationException($"Column '{columnName}' was NULL but is required.");

        string value = reader.GetString(ord);

        if (string.IsNullOrWhiteSpace(value))
            throw new InvalidOperationException($"Column '{columnName}' was empty/whitespace but is required.");

        return value;
    }

    private static string? GetNullableString(SqliteDataReader reader, string columnName)
    {
        int ord = reader.GetOrdinal(columnName);
        return reader.IsDBNull(ord) ? null : reader.GetString(ord);
    }

    private static int GetInt(SqliteDataReader reader, string columnName, int defaultValue)
    {
        int ord = reader.GetOrdinal(columnName);

        if (reader.IsDBNull(ord))
            return defaultValue;

        try
        {
            return Convert.ToInt32(reader.GetValue(ord));
        }
        catch
        {
            return defaultValue;
        }
    }

    private static decimal GetDecimal(SqliteDataReader reader, string columnName, decimal defaultValue)
    {
        int ord = reader.GetOrdinal(columnName);

        if (reader.IsDBNull(ord))
            return defaultValue;

        try
        {
            return Convert.ToDecimal(reader.GetValue(ord));
        }
        catch
        {
            return defaultValue;
        }
    }
}
