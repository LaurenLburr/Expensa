using System;
using System.Collections.Generic;
using CodexExpensa.Core.Domain.Banks;
using CodexExpensa.Data.Sqlite.Db;
using Microsoft.Data.Sqlite;

namespace CodexExpensa.Data.Sqlite.Banks;

public sealed class SqliteBankRepository : IBankRepository
{
    private readonly SqliteDatabase _db;

    public SqliteBankRepository(SqliteDatabase db)
    {
        _db = db ?? throw new ArgumentNullException(nameof(db));
    }

    public IReadOnlyList<Bank> GetAll()
    {
        const string sql =
            """
            SELECT
                BankId,
                BankName,
                RoutingNumber,
                Url,
                IsActive
            FROM Bank
            ORDER BY BankName, RoutingNumber;
            """;

        return _db.Query(sql, MapBank);
    }

    public Bank? GetById(string bankId)
    {
        if (string.IsNullOrWhiteSpace(bankId))
            throw new ArgumentException("bankId is required.", nameof(bankId));

        const string sql =
            """
            SELECT
                BankId,
                BankName,
                RoutingNumber,
                Url,
                IsActive
            FROM Bank
            WHERE BankId = @BankId
            LIMIT 1;
            """;

        var rows = _db.Query(
            sql,
            MapBank,
            new[] { new SqliteParameter("@BankId", bankId) });

        return rows.Count == 0 ? null : rows[0];
    }

    public void Add(Bank bank)
    {
        if (bank is null) throw new ArgumentNullException(nameof(bank));
        Validate(bank);

        const string insert =
            """
            INSERT INTO Bank
            (
                BankId,
                BankName,
                RoutingNumber,
                Url,
                IsActive
            )
            VALUES
            (
                @BankId,
                @BankName,
                @RoutingNumber,
                @Url,
                @IsActive
            );
            """;

        try
        {
            _db.ExecuteNonQuery(
                insert,
                new[]
                {
                    new SqliteParameter("@BankId", bank.BankId),
                    new SqliteParameter("@BankName", bank.BankName),
                    new SqliteParameter("@RoutingNumber", bank.RoutingNumber),
                    new SqliteParameter("@Url", (object?)bank.Url ?? DBNull.Value),
                    new SqliteParameter("@IsActive", bank.IsActive ? 1 : 0),
                });
        }
        catch (SqliteException ex) when (ex.SqliteErrorCode == 19)
        {
            throw new InvalidOperationException(
                $"Could not add bank '{bank.BankName}'. A constraint was violated (likely duplicate BankName + RoutingNumber).",
                ex);
        }
    }

    public void Update(Bank bank)
    {
        if (bank is null) throw new ArgumentNullException(nameof(bank));
        Validate(bank);

        const string update =
            """
            UPDATE Bank
            SET
                BankName = @BankName,
                RoutingNumber = @RoutingNumber,
                Url = @Url,
                IsActive = @IsActive
            WHERE BankId = @BankId;
            """;

        try
        {
            _db.ExecuteNonQuery(
                update,
                new[]
                {
                    new SqliteParameter("@BankId", bank.BankId),
                    new SqliteParameter("@BankName", bank.BankName),
                    new SqliteParameter("@RoutingNumber", bank.RoutingNumber),
                    new SqliteParameter("@Url", (object?)bank.Url ?? DBNull.Value),
                    new SqliteParameter("@IsActive", bank.IsActive ? 1 : 0),
                });
        }
        catch (SqliteException ex) when (ex.SqliteErrorCode == 19)
        {
            throw new InvalidOperationException(
                $"Could not update bank '{bank.BankName}'. A constraint was violated (likely duplicate BankName + RoutingNumber).",
                ex);
        }
    }

    public void Delete(string bankId)
    {
        if (string.IsNullOrWhiteSpace(bankId))
            throw new ArgumentException("bankId is required.", nameof(bankId));

        // Prevent deleting a bank that still has accounts
        const string countAccounts =
            """
            SELECT COUNT(1)
            FROM Account
            WHERE BankId = @BankId;
            """;

        var counts = _db.Query(
            countAccounts,
            r => Convert.ToInt32(r.GetValue(0)),
            new[] { new SqliteParameter("@BankId", bankId) });

        if (counts.Count > 0 && counts[0] > 0)
            throw new InvalidOperationException("Cannot delete bank because it still has accounts.");

        const string delete =
            """
            DELETE FROM Bank
            WHERE BankId = @BankId;
            """;

        _db.ExecuteNonQuery(delete, new[] { new SqliteParameter("@BankId", bankId) });
    }

    private static void Validate(Bank bank)
    {
        if (string.IsNullOrWhiteSpace(bank.BankId))
            throw new ArgumentException("BankId is required.", nameof(bank));

        if (string.IsNullOrWhiteSpace(bank.BankName))
            throw new ArgumentException("BankName is required.", nameof(bank));

        if (string.IsNullOrWhiteSpace(bank.RoutingNumber))
            throw new ArgumentException("RoutingNumber is required.", nameof(bank));
    }

    private static Bank MapBank(SqliteDataReader r)
    {
        var bankId = GetRequiredString(r, "BankId");
        var bankName = GetRequiredString(r, "BankName");
        var routing = GetRequiredString(r, "RoutingNumber");
        var url = GetNullableString(r, "Url");
        var isActive = GetBool(r, "IsActive", true);

        return new Bank
        {
            BankId = bankId,
            BankName = bankName,
            RoutingNumber = routing,
            Url = url,
            IsActive = isActive
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

    private static bool GetBool(SqliteDataReader reader, string columnName, bool defaultValue)
    {
        var ord = reader.GetOrdinal(columnName);
        if (reader.IsDBNull(ord))
            return defaultValue;

        try { return Convert.ToInt32(reader.GetValue(ord)) != 0; }
        catch { return defaultValue; }
    }
}