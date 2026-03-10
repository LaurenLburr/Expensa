using System;
using System.Collections.Generic;
using CodexExpensa.Core.Abstractions;
using Microsoft.Data.Sqlite;

namespace CodexExpensa.Infrastructure.Budgets;

public sealed class BudgetMonthRepository
{
    private readonly SqliteConnection _connection;
    private readonly ISqlQueryProvider _sqlQueryProvider;

    public BudgetMonthRepository(
        SqliteConnection connection,
        ISqlQueryProvider sqlQueryProvider)
    {
        _connection = connection ?? throw new ArgumentNullException(nameof(connection));
        _sqlQueryProvider = sqlQueryProvider ?? throw new ArgumentNullException(nameof(sqlQueryProvider));
    }

    public bool Exists(int year, int month)
    {
        string sql = _sqlQueryProvider.GetSql("BudgetMonth.ExistsByYearMonth");

        using SqliteCommand command = _connection.CreateCommand();

        command.CommandText = sql;
        command.Parameters.AddWithValue("@Year", year);
        command.Parameters.AddWithValue("@Month", month);

        object? result = command.ExecuteScalar();

        return result is not null && result != DBNull.Value;
    }

    public void Create(string budgetMonthId, int year, int month)
    {
        string sql = _sqlQueryProvider.GetSql("BudgetMonth.Create");

        using SqliteCommand command = _connection.CreateCommand();

        command.CommandText = sql;
        command.Parameters.AddWithValue("@BudgetMonthId", budgetMonthId);
        command.Parameters.AddWithValue("@Year", year);
        command.Parameters.AddWithValue("@Month", month);

        command.ExecuteNonQuery();
    }

    public IReadOnlyList<BudgetMonthListItem> SelectAll()
    {
        string sql = _sqlQueryProvider.GetSql("BudgetMonth.SelectAll");

        using SqliteCommand command = _connection.CreateCommand();

        command.CommandText = sql;

        using SqliteDataReader reader = command.ExecuteReader();

        List<BudgetMonthListItem> rows = new();

        while (reader.Read())
        {
            rows.Add(new BudgetMonthListItem
            {
                BudgetMonthId = reader.GetString(0),
                Year = reader.GetInt32(1),
                Month = reader.GetInt32(2),
                CreatedUtc = reader.IsDBNull(3)
                    ? null
                    : reader.GetString(3)
            });
        }

        return rows;
    }
}

public sealed class BudgetMonthListItem
{
    public required string BudgetMonthId { get; init; }

    public int Year { get; init; }

    public int Month { get; init; }

    public string? CreatedUtc { get; init; }
}