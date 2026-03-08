using System;
using System.Collections.Generic;
using CodexExpensa.Core.Domain.Payees;
using CodexExpensa.Data.Sqlite.Db;
using Microsoft.Data.Sqlite;

namespace CodexExpensa.Data.Sqlite.Payees;

public sealed class SqlitePayeeRepository : IPayeeRepository
{
    private readonly SqliteDatabase _db;

    public SqlitePayeeRepository(SqliteDatabase db)
    {
        _db = db ?? throw new ArgumentNullException(nameof(db));
    }

    public IReadOnlyList<Payee> GetAll()
    {
        const string sql =
            """
            SELECT PayeeId, PayeeName, IncludeInBudgetTemplate
            FROM Payee
            ORDER BY PayeeName;
            """;

        return _db.Query(sql, MapPayee);
    }

    public IReadOnlyList<Payee> GetBudgetTemplatePayees()
    {
        const string sql =
            """
            SELECT PayeeId, PayeeName, SortIndex
            FROM vw_BudgetTemplatePayees
            ORDER BY SortIndex, PayeeName;
            """;

        // View doesn't include IncludeInBudgetTemplate, but these are all included by definition.
        return _db.Query(sql, r =>
        {
            var id = GetRequiredString(r, "PayeeId");
            var name = GetRequiredString(r, "PayeeName");

            return new Payee
            {
                PayeeId = id,
                PayeeName = name,
                IncludeInBudgetTemplate = true
            };
        });
    }

    public Payee? GetById(string payeeId)
    {
        if (string.IsNullOrWhiteSpace(payeeId))
            throw new ArgumentException("payeeId is required.", nameof(payeeId));

        const string sql =
            """
            SELECT PayeeId, PayeeName, IncludeInBudgetTemplate
            FROM Payee
            WHERE PayeeId = @PayeeId
            LIMIT 1;
            """;

        var rows = _db.Query(
            sql,
            MapPayee,
            new[] { new SqliteParameter("@PayeeId", payeeId) });

        return rows.Count == 0 ? null : rows[0];
    }

    public Payee? GetByName(string payeeName)
    {
        if (string.IsNullOrWhiteSpace(payeeName))
            throw new ArgumentException("payeeName is required.", nameof(payeeName));

        const string sql =
            """
            SELECT PayeeId, PayeeName, IncludeInBudgetTemplate
            FROM Payee
            WHERE PayeeName = @PayeeName
            LIMIT 1;
            """;

        var rows = _db.Query(
            sql,
            MapPayee,
            new[] { new SqliteParameter("@PayeeName", payeeName.Trim()) });

        return rows.Count == 0 ? null : rows[0];
    }

    public void Add(Payee payee)
    {
        if (payee is null) throw new ArgumentNullException(nameof(payee));
        Validate(payee);

        const string sql =
            """
            INSERT INTO Payee (PayeeId, PayeeName, IncludeInBudgetTemplate)
            VALUES (@PayeeId, @PayeeName, @IncludeInBudgetTemplate);
            """;

        _db.ExecuteNonQuery(sql, new[]
        {
            new SqliteParameter("@PayeeId", payee.PayeeId),
            new SqliteParameter("@PayeeName", payee.PayeeName.Trim()),
            new SqliteParameter("@IncludeInBudgetTemplate", payee.IncludeInBudgetTemplate ? 1 : 0)
        });
    }

    public void Update(Payee payee)
    {
        if (payee is null) throw new ArgumentNullException(nameof(payee));
        Validate(payee);

        const string sql =
            """
            UPDATE Payee
            SET
                PayeeName = @PayeeName,
                IncludeInBudgetTemplate = @IncludeInBudgetTemplate
            WHERE PayeeId = @PayeeId;
            """;

        _db.ExecuteNonQuery(sql, new[]
        {
            new SqliteParameter("@PayeeId", payee.PayeeId),
            new SqliteParameter("@PayeeName", payee.PayeeName.Trim()),
            new SqliteParameter("@IncludeInBudgetTemplate", payee.IncludeInBudgetTemplate ? 1 : 0)
        });
    }

    public void UpdateIncludeInBudgetTemplate(string payeeId, bool includeInBudgetTemplate)
    {
        if (string.IsNullOrWhiteSpace(payeeId))
            throw new ArgumentException("payeeId is required.", nameof(payeeId));

        const string sql =
            """
            UPDATE Payee
            SET IncludeInBudgetTemplate = @IncludeInBudgetTemplate
            WHERE PayeeId = @PayeeId;
            """;

        _db.ExecuteNonQuery(sql, new[]
        {
            new SqliteParameter("@PayeeId", payeeId),
            new SqliteParameter("@IncludeInBudgetTemplate", includeInBudgetTemplate ? 1 : 0)
        });
    }

    public void Delete(string payeeId)
    {
        if (string.IsNullOrWhiteSpace(payeeId))
            throw new ArgumentException("payeeId is required.", nameof(payeeId));

        const string sql =
            """
            DELETE FROM Payee
            WHERE PayeeId = @PayeeId;
            """;

        _db.ExecuteNonQuery(sql, new[] { new SqliteParameter("@PayeeId", payeeId) });
    }

    private static void Validate(Payee payee)
    {
        if (string.IsNullOrWhiteSpace(payee.PayeeId))
            throw new ArgumentException("PayeeId is required.", nameof(payee));

        if (string.IsNullOrWhiteSpace(payee.PayeeName))
            throw new ArgumentException("PayeeName is required.", nameof(payee));
    }

    private static Payee MapPayee(SqliteDataReader r)
    {
        var id = GetRequiredString(r, "PayeeId");
        var name = GetRequiredString(r, "PayeeName");

        var include = false;
        var ord = r.GetOrdinal("IncludeInBudgetTemplate");
        if (!r.IsDBNull(ord))
            include = r.GetInt32(ord) == 1;

        return new Payee
        {
            PayeeId = id,
            PayeeName = name,
            IncludeInBudgetTemplate = include
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
}