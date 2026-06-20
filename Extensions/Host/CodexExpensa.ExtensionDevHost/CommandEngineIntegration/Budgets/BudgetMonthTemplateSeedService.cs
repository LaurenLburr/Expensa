using Microsoft.Data.Sqlite;
using System.Globalization;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Budgets;

public sealed class BudgetMonthTemplateSeedService
{
    public BudgetMonthTemplateSeedResult GenerateBudgetMonths(
        SqliteConnection connection,
        IReadOnlyCollection<int> years)
    {
        ArgumentNullException.ThrowIfNull(connection);
        ArgumentNullException.ThrowIfNull(years);

        if (years.Count == 0)
        {
            throw new ArgumentException("At least one target year is required.", nameof(years));
        }

        List<string> warnings = [];

        bool hasBudgetMonth = TableExists(connection, "BudgetMonth");
        bool hasBudgetTemplateRow = TableExists(connection, "BudgetTemplateRow");
        bool hasBudgetMonthRow = TableExists(connection, "BudgetMonthRow");

        if (!hasBudgetMonth)
        {
            warnings.Add("BudgetMonth table was not found; no month header rows were inserted.");
        }

        if (!hasBudgetTemplateRow)
        {
            warnings.Add("BudgetTemplateRow table was not found; no monthly detail rows were generated.");
        }

        if (!hasBudgetMonthRow)
        {
            warnings.Add("BudgetMonthRow table was not found; no monthly detail rows were generated.");
        }

        if (!hasBudgetMonth)
        {
            return new BudgetMonthTemplateSeedResult
            {
                TableName = "BudgetMonth/BudgetMonthRow",
                Years = years.Order().ToArray(),
                Warnings = warnings
            };
        }

        int templateRowCount = hasBudgetTemplateRow
            ? CountActiveTemplateRows(connection)
            : 0;

        int insertedMonthRows = 0;
        int insertedBudgetRows = 0;
        int skippedMonths = 0;

        using SqliteTransaction transaction =
            connection.BeginTransaction();

        foreach (int year in years.Order())
        {
            for (int month = 1; month <= 12; month++)
            {
                string budgetMonthId =
                    FormattableString.Invariant($"{year:D4}-{month:D2}");

                if (BudgetMonthExists(connection, transaction, budgetMonthId))
                {
                    skippedMonths++;
                }
                else
                {
                    InsertBudgetMonth(connection, transaction, budgetMonthId, year, month);
                    insertedMonthRows++;
                }

                if (hasBudgetTemplateRow && hasBudgetMonthRow)
                {
                    insertedBudgetRows += InsertMissingBudgetMonthRows(
                        connection,
                        transaction,
                        budgetMonthId);
                }
            }
        }

        transaction.Commit();

        if (templateRowCount == 0 && hasBudgetTemplateRow)
        {
            warnings.Add("BudgetTemplateRow contains no active rows; BudgetMonthRow had nothing to generate.");
        }

        return new BudgetMonthTemplateSeedResult
        {
            TableName = "BudgetMonth/BudgetMonthRow",
            TemplateRowCount = templateRowCount,
            InsertedRowCount = insertedMonthRows + insertedBudgetRows,
            SkippedMonthCount = skippedMonths,
            Years = years.Order().ToArray(),
            Warnings = warnings
        };
    }

    private static bool TableExists(
        SqliteConnection connection,
        string tableName)
    {
        using SqliteCommand command =
            connection.CreateCommand();

        command.CommandText =
            """
            SELECT 1
            FROM [sqlite_master]
            WHERE [type] = 'table'
              AND [name] = @TableName
            LIMIT 1;
            """;

        command.Parameters.AddWithValue("@TableName", tableName);

        return command.ExecuteScalar() is not null;
    }

    private static int CountActiveTemplateRows(
        SqliteConnection connection)
    {
        using SqliteCommand command =
            connection.CreateCommand();

        command.CommandText =
            """
            SELECT COUNT(*)
            FROM [BudgetTemplateRow]
            WHERE [IsActive] = 1;
            """;

        return Convert.ToInt32(command.ExecuteScalar(), CultureInfo.InvariantCulture);
    }

    private static bool BudgetMonthExists(
        SqliteConnection connection,
        SqliteTransaction transaction,
        string budgetMonthId)
    {
        using SqliteCommand command =
            connection.CreateCommand();

        command.Transaction = transaction;
        command.CommandText =
            """
            SELECT 1
            FROM [BudgetMonth]
            WHERE [BudgetMonthId] = @BudgetMonthId
            LIMIT 1;
            """;

        command.Parameters.AddWithValue("@BudgetMonthId", budgetMonthId);

        return command.ExecuteScalar() is not null;
    }

    private static void InsertBudgetMonth(
        SqliteConnection connection,
        SqliteTransaction transaction,
        string budgetMonthId,
        int year,
        int month)
    {
        using SqliteCommand command =
            connection.CreateCommand();

        command.Transaction = transaction;
        command.CommandText =
            """
            INSERT INTO [BudgetMonth]
            (
                [BudgetMonthId],
                [Year],
                [Month],
                [CreatedUtc]
            )
            VALUES
            (
                @BudgetMonthId,
                @Year,
                @Month,
                datetime('now')
            );
            """;

        command.Parameters.AddWithValue("@BudgetMonthId", budgetMonthId);
        command.Parameters.AddWithValue("@Year", year);
        command.Parameters.AddWithValue("@Month", month);
        command.ExecuteNonQuery();
    }

    private static int InsertMissingBudgetMonthRows(
        SqliteConnection connection,
        SqliteTransaction transaction,
        string budgetMonthId)
    {
        using SqliteCommand command =
            connection.CreateCommand();

        command.Transaction = transaction;
        command.CommandText =
            """
            INSERT INTO [BudgetMonthRow]
            (
                [BudgetMonthRowId],
                [BudgetMonthId],
                [TemplateRowId],
                [Name],
                [SortIndex],
                [PlannedAmount]
            )
            SELECT
                'budget-month-row-' || @BudgetMonthId || '-' || [BudgetTemplateRow].[TemplateRowId] AS [BudgetMonthRowId],
                @BudgetMonthId AS [BudgetMonthId],
                [BudgetTemplateRow].[TemplateRowId],
                [BudgetTemplateRow].[Name],
                [BudgetTemplateRow].[SortIndex],
                [BudgetTemplateRow].[DefaultAmount]
            FROM [BudgetTemplateRow]
            WHERE [BudgetTemplateRow].[IsActive] = 1
              AND NOT EXISTS
              (
                  SELECT 1
                  FROM [BudgetMonthRow] AS [existing]
                  WHERE [existing].[BudgetMonthId] = @BudgetMonthId
                    AND [existing].[TemplateRowId] = [BudgetTemplateRow].[TemplateRowId]
              );
            """;

        command.Parameters.AddWithValue("@BudgetMonthId", budgetMonthId);

        return command.ExecuteNonQuery();
    }
}
