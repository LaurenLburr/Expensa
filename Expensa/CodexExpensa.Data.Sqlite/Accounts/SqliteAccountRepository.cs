using System;
using System.Collections.Generic;
using CodexExpensa.Core.Domain.Accounts;
using CodexExpensa.Data.Sqlite.Db;
using Microsoft.Data.Sqlite;

namespace CodexExpensa.Data.Sqlite.Accounts;

public sealed class SqliteAccountRepository : IAccountRepository
{
    private readonly SqliteDatabase _db;

    public SqliteAccountRepository(SqliteDatabase db)
    {
        _db = db ?? throw new ArgumentNullException(nameof(db));
    }

    public IReadOnlyList<Account> GetAll()
    {
        const string sql =
            """
            SELECT
                a.AccountId,
                a.AccountNickname,
                a.SortIndex,
                a.AccountNumber,
                a.BankId,

                b.BankName,
                b.RoutingNumber,
                b.Url,

                a.AccountType,
                a.IsActive
            FROM Account a
            JOIN Bank b ON b.BankId = a.BankId
            ORDER BY a.SortIndex, a.AccountNickname;
            """;

        return _db.Query(sql, MapAccount);
    }

    public Account? GetById(string accountId)
    {
        if (string.IsNullOrWhiteSpace(accountId))
            throw new ArgumentException("accountId is required.", nameof(accountId));

        const string sql =
            """
            SELECT
                a.AccountId,
                a.AccountNickname,
                a.SortIndex,
                a.AccountNumber,
                a.BankId,

                b.BankName,
                b.RoutingNumber,
                b.Url,

                a.AccountType,
                a.IsActive
            FROM Account a
            JOIN Bank b ON b.BankId = a.BankId
            WHERE a.AccountId = @AccountId
            LIMIT 1;
            """;

        var rows = _db.Query(
            sql,
            MapAccount,
            new[] { new SqliteParameter("@AccountId", accountId) });

        return rows.Count == 0 ? null : rows[0];
    }

    public void Add(Account account)
    {
        if (account is null) throw new ArgumentNullException(nameof(account));
        ValidateForWrite(account);

        _db.ExecuteInTransaction(tx =>
        {
            var bankId = GetOrCreateBankId(
                bankName: account.BankName,
                routingNumber: account.RoutingNumber,
                url: account.Url,
                tx: tx);

            const string insertAccount =
                """
                INSERT INTO Account
                (
                    AccountId,
                    BankId,
                    AccountNickname,
                    SortIndex,
                    AccountNumber,
                    AccountType,
                    IsActive
                )
                VALUES
                (
                    @AccountId,
                    @BankId,
                    @AccountNickname,
                    @SortIndex,
                    @AccountNumber,
                    @AccountType,
                    @IsActive
                );
                """;

            var parameters = new[]
            {
                new SqliteParameter("@AccountId", account.AccountId),
                new SqliteParameter("@BankId", bankId),
                new SqliteParameter("@AccountNickname", account.AccountNickname),
                new SqliteParameter("@SortIndex", account.SortIndex),
                new SqliteParameter("@AccountNumber", account.AccountNumber),
                new SqliteParameter("@AccountType", account.AccountType.ToString()),
                new SqliteParameter("@IsActive", account.IsActive ? 1 : 0),
            };

            try
            {
                _db.ExecuteNonQuery(insertAccount, parameters, tx);
            }
            catch (SqliteException ex) when (ex.SqliteErrorCode == 19)
            {
                throw new InvalidOperationException($"Could not add account '{account.AccountNickname}'. A constraint was violated.", ex);
            }
        });
    }

    public void Update(Account account)
    {
        if (account is null) throw new ArgumentNullException(nameof(account));
        ValidateForWrite(account);

        _db.ExecuteInTransaction(tx =>
        {
            var bankId = GetOrCreateBankId(
                bankName: account.BankName,
                routingNumber: account.RoutingNumber,
                url: account.Url,
                tx: tx);

            const string update =
                """
                UPDATE Account
                SET
                    BankId = @BankId,
                    AccountNickname = @AccountNickname,
                    SortIndex = @SortIndex,
                    AccountNumber = @AccountNumber,
                    AccountType = @AccountType,
                    IsActive = @IsActive
                WHERE AccountId = @AccountId;
                """;

            _db.ExecuteNonQuery(
                update,
                new[]
                {
                    new SqliteParameter("@AccountId", account.AccountId),
                    new SqliteParameter("@BankId", bankId),
                    new SqliteParameter("@AccountNickname", account.AccountNickname),
                    new SqliteParameter("@SortIndex", account.SortIndex),
                    new SqliteParameter("@AccountNumber", account.AccountNumber),
                    new SqliteParameter("@AccountType", account.AccountType.ToString()),
                    new SqliteParameter("@IsActive", account.IsActive ? 1 : 0),
                },
                tx);
        });
    }

    public void Delete(string accountId)
    {
        if (string.IsNullOrWhiteSpace(accountId))
            throw new ArgumentException("accountId is required.", nameof(accountId));

        const string delete =
            """
            DELETE FROM Account
            WHERE AccountId = @AccountId;
            """;

        _db.ExecuteNonQuery(delete, new[] { new SqliteParameter("@AccountId", accountId) });
    }

    private static void ValidateForWrite(Account account)
    {
        if (string.IsNullOrWhiteSpace(account.AccountId)) throw new ArgumentException("AccountId is required.", nameof(account));
        if (string.IsNullOrWhiteSpace(account.AccountNickname)) throw new ArgumentException("AccountNickname is required.", nameof(account));
        if (string.IsNullOrWhiteSpace(account.BankName)) throw new ArgumentException("BankName is required.", nameof(account));
        if (string.IsNullOrWhiteSpace(account.RoutingNumber)) throw new ArgumentException("RoutingNumber is required.", nameof(account));
        if (string.IsNullOrWhiteSpace(account.AccountNumber)) throw new ArgumentException("AccountNumber is required.", nameof(account));
    }

    private string GetOrCreateBankId(string bankName, string routingNumber, string? url, SqliteTransaction tx)
    {
        const string select =
            """
            SELECT BankId, Url
            FROM Bank
            WHERE BankName = @BankName
              AND RoutingNumber = @RoutingNumber
            LIMIT 1;
            """;

        var existing = _db.Query(
            select,
            r =>
            {
                var id = r.GetString(0);
                var existingUrl = r.IsDBNull(1) ? null : r.GetString(1);
                return (BankId: id, Url: existingUrl);
            },
            new[]
            {
                new SqliteParameter("@BankName", bankName),
                new SqliteParameter("@RoutingNumber", routingNumber),
            },
            tx);

        if (existing.Count > 0)
        {
            if (!string.IsNullOrWhiteSpace(url) && string.IsNullOrWhiteSpace(existing[0].Url))
            {
                const string updateBankUrl =
                    """
                    UPDATE Bank
                    SET Url = @Url
                    WHERE BankId = @BankId;
                    """;

                _db.ExecuteNonQuery(
                    updateBankUrl,
                    new[]
                    {
                        new SqliteParameter("@Url", url),
                        new SqliteParameter("@BankId", existing[0].BankId),
                    },
                    tx);
            }

            return existing[0].BankId;
        }

        var newId = Guid.NewGuid().ToString("N");

        const string insert =
            """
            INSERT INTO Bank
            (
                BankId,
                BankName,
                RoutingNumber,
                Url
            )
            VALUES
            (
                @BankId,
                @BankName,
                @RoutingNumber,
                @Url
            );
            """;

        try
        {
            _db.ExecuteNonQuery(
                insert,
                new[]
                {
                    new SqliteParameter("@BankId", newId),
                    new SqliteParameter("@BankName", bankName),
                    new SqliteParameter("@RoutingNumber", routingNumber),
                    new SqliteParameter("@Url", (object?)url ?? DBNull.Value),
                },
                tx);

            return newId;
        }
        catch (SqliteException ex) when (ex.SqliteErrorCode == 19)
        {
            var retry = _db.Query(
                """
                SELECT BankId
                FROM Bank
                WHERE BankName = @BankName
                  AND RoutingNumber = @RoutingNumber
                LIMIT 1;
                """,
                r => r.GetString(0),
                new[]
                {
                    new SqliteParameter("@BankName", bankName),
                    new SqliteParameter("@RoutingNumber", routingNumber),
                },
                tx);

            if (retry.Count > 0)
                return retry[0];

            throw;
        }
    }

    private static Account MapAccount(SqliteDataReader r)
    {
        var accountId = GetRequiredString(r, "AccountId");
        var nickname = GetRequiredString(r, "AccountNickname");
        var sortIndex = GetInt(r, "SortIndex", 0);
        var accountNumber = GetRequiredString(r, "AccountNumber");
        var bankId = GetRequiredString(r, "BankId");

        var bankName = GetRequiredString(r, "BankName");
        var routingNumber = GetRequiredString(r, "RoutingNumber");
        var url = GetNullableString(r, "Url");

        var accountTypeText = GetRequiredString(r, "AccountType");
        var isActive = GetBool(r, "IsActive", true);

        if (!Enum.TryParse<AccountType>(accountTypeText, ignoreCase: true, out var accountType))
            throw new InvalidOperationException($"Unknown AccountType '{accountTypeText}' for AccountId '{accountId}'.");

        return new Account
        {
            AccountId = accountId,
            AccountNickname = nickname,
            SortIndex = sortIndex,
            AccountNumber = accountNumber,
            BankId = bankId,
            BankName = bankName,
            RoutingNumber = routingNumber,
            Url = url,
            AccountType = accountType,
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

    private static int GetInt(SqliteDataReader reader, string columnName, int defaultValue)
    {
        var ord = reader.GetOrdinal(columnName);
        if (reader.IsDBNull(ord))
            return defaultValue;

        try { return Convert.ToInt32(reader.GetValue(ord)); }
        catch { return defaultValue; }
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