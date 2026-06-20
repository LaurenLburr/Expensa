using Codex.CommandEngine.Core;
using Microsoft.Data.Sqlite;
using System.Data;
using System.Globalization;
using System.Text.Json;

namespace BudgetsAddin;

public sealed class BudgetScreenRuntimeRunner
{
    public Task<CommandExecutionResult> ExecuteAsync(
        BudgetScreenRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        cancellationToken.ThrowIfCancellationRequested();

        BudgetScreenDocument document =
            BuildScreen(request);

        return Task.FromResult(
            new CommandExecutionResult
            {
                CorrelationId = Guid.NewGuid().ToString("N"),
                CommandName = "Budgets.LoadScreen",
                Status = CommandExecutionStatus.Succeeded,
                Message = $"Loaded screen: {document.Title}",
                OutputJson = JsonSerializer.Serialize(
                    document,
                    BudgetJsonSerializerOptions.Default)
            });
    }

    private static BudgetScreenDocument BuildScreen(
        BudgetScreenRequest request)
    {
        if (string.Equals(
                request.NodeType,
                BudgetTreeNodeTypes.BudgetTemplates,
                StringComparison.OrdinalIgnoreCase))
        {
            return LoadTemplates(request);
        }

        if (request.Year is not null &&
            request.Month is not null)
        {
            return LoadMonth(
                request,
                request.Year.Value,
                request.Month.Value);
        }

        return new BudgetScreenDocument
        {
            Title = string.IsNullOrWhiteSpace(request.DisplayText)
                ? "Budgets"
                : request.DisplayText,
            Subtitle = "Select a budget month or Templates.",
            Columns = ["Item", "Value"],
            Rows = []
        };
    }

    private static BudgetScreenDocument LoadTemplates(
        BudgetScreenRequest request)
    {
        using SqliteConnection connection =
            OpenReadOnly(request.DatabasePath);

        using SqliteCommand command =
            connection.CreateCommand();

        command.CommandText =
            """
            SELECT
                [Name] AS [Item],
                [DefaultAmount] AS [Amount],
                [SortIndex],
                [IsActive]
            FROM [BudgetTemplateRow]
            ORDER BY [SortIndex], [Name];
            """;

        DataTable table =
            LoadTable(command);

        return new BudgetScreenDocument
        {
            Title = "Budget Templates",
            Subtitle = $"{table.Rows.Count} template row(s)",
            Columns =
            [
                "Item",
                "Amount",
                "SortIndex",
                "IsActive"
            ],
            Rows = ToRows(table)
        };
    }

    private static BudgetScreenDocument LoadMonth(
        BudgetScreenRequest request,
        int year,
        int month)
    {
        using SqliteConnection connection =
            OpenReadOnly(request.DatabasePath);

        string? budgetMonthId =
            FindBudgetMonthId(
                connection,
                year,
                month);

        if (string.IsNullOrWhiteSpace(budgetMonthId))
        {
            return new BudgetScreenDocument
            {
                Title = $"{new DateTime(year, month, 1):MMMM yyyy}",
                Subtitle = "No BudgetMonth row exists for this month.",
                Columns = ["RowType", "Item", "Status", "Amount"],
                Rows = [],
                AllowAddTransaction = true
            };
        }

        using SqliteCommand command =
            connection.CreateCommand();

        command.CommandText =
            """
            SELECT
                'budget:' || bmr.[BudgetMonthRowId] AS [NodeId],
                NULL AS [ParentNodeId],
                0 AS [HierarchyLevel],
                'Budget' AS [RowType],
                bmr.[Name] AS [Item],
                COALESCE(p.[PayeeName], bmr.[Name]) AS [Payee],
                p.[PayeeId],
                NULL AS [Status],
                bmr.[PlannedAmount] AS [Amount],
                NULL AS [StartDate],
                NULL AS [Account],
                bmr.[SortIndex] AS [SortIndex],
                bmr.[BudgetMonthRowId] AS [BudgetMonthRowId],
                NULL AS [TransactionId]
            FROM [BudgetMonthRow] bmr
            LEFT JOIN [Payee] p
                ON p.[PayeeName] = bmr.[Name]
            WHERE bmr.[BudgetMonthId] = @BudgetMonthId

            UNION ALL

            SELECT
                'transaction:' || t.[TransactionId] AS [NodeId],
                'budget:' || bmr.[BudgetMonthRowId] AS [ParentNodeId],
                1 AS [HierarchyLevel],
                'Transaction' AS [RowType],
                p.[PayeeName] AS [Item],
                p.[PayeeName] AS [Payee],
                p.[PayeeId],
                t.[Status],
                t.[Amount],
                t.[StartDate],
                a.[AccountNickname] AS [Account],
                bmr.[SortIndex] AS [SortIndex],
                bmr.[BudgetMonthRowId] AS [BudgetMonthRowId],
                t.[TransactionId]
            FROM [BudgetMonthRow] bmr
            INNER JOIN [Payee] p
                ON p.[PayeeName] = bmr.[Name]
            INNER JOIN [Txn] t
                ON t.[PayeeId] = p.[PayeeId]
               AND CAST(strftime('%Y', t.[StartDate]) AS INTEGER) = @Year
               AND CAST(strftime('%m', t.[StartDate]) AS INTEGER) = @Month
            LEFT JOIN [Account] a
                ON a.[AccountId] = t.[AccountId]
            WHERE bmr.[BudgetMonthId] = @BudgetMonthId

            ORDER BY
                [SortIndex],
                [BudgetMonthRowId],
                [HierarchyLevel],
                [StartDate],
                [TransactionId];
            """;

        command.Parameters.AddWithValue(
            "@BudgetMonthId",
            budgetMonthId);

        command.Parameters.AddWithValue(
            "@Year",
            year);

        command.Parameters.AddWithValue(
            "@Month",
            month);

        DataTable source =
            LoadTable(command);

        return new BudgetScreenDocument
        {
            Title = $"{new DateTime(year, month, 1):MMMM yyyy}",
            Subtitle =
                $"BudgetMonthId: {budgetMonthId}; {source.Rows.Count} displayed row(s)",
            Columns =
            [
                "RowType",
                "Item",
                "Payee",
                "Status",
                "Amount",
                "StartDate",
                "Account"
            ],
            Rows = ToRows(source),
            IsHierarchical = true,
            IdColumnName = "NodeId",
            ParentIdColumnName = "ParentNodeId",
            HiddenColumns =
            [
                "NodeId",
                "ParentNodeId",
                "HierarchyLevel",
                "SortIndex",
                "BudgetMonthRowId",
                "PayeeId",
                "TransactionId"
            ],
            AllowAddTransaction = true
        };
    }

    private static string? FindBudgetMonthId(
        SqliteConnection connection,
        int year,
        int month)
    {
        using SqliteCommand command =
            connection.CreateCommand();

        command.CommandText =
            """
            SELECT [BudgetMonthId]
            FROM [BudgetMonth]
            WHERE [Year] = @Year
              AND [Month] = @Month
            LIMIT 1;
            """;

        command.Parameters.AddWithValue("@Year", year);
        command.Parameters.AddWithValue("@Month", month);

        return Convert.ToString(
            command.ExecuteScalar(),
            CultureInfo.InvariantCulture);
    }

    private static SqliteConnection OpenReadOnly(
        string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            throw new InvalidOperationException(
                "DatabasePath is required.");
        }

        if (!File.Exists(path))
        {
            throw new FileNotFoundException(
                "The Expensa database was not found.",
                path);
        }

        SqliteConnection connection =
            new($"Data Source={path};Mode=ReadOnly;Pooling=False");

        connection.Open();

        return connection;
    }

    private static DataTable LoadTable(
        SqliteCommand command)
    {
        using SqliteDataReader reader =
            command.ExecuteReader();

        DataTable table = new();

        for (int index = 0; index < reader.FieldCount; index++)
        {
            string columnName =
                reader.GetName(index);

            if (string.IsNullOrWhiteSpace(columnName))
            {
                columnName = $"Column{index + 1}";
            }

            if (table.Columns.Contains(columnName))
            {
                columnName = $"{columnName}{index + 1}";
            }

            table.Columns.Add(columnName, typeof(object));
        }

        while (reader.Read())
        {
            DataRow row =
                table.NewRow();

            for (int index = 0; index < reader.FieldCount; index++)
            {
                row[index] =
                    reader.IsDBNull(index)
                        ? DBNull.Value
                        : reader.GetValue(index);
            }

            table.Rows.Add(row);
        }

        return table;
    }

    private static IReadOnlyList<IReadOnlyDictionary<string, object?>>
        ToRows(DataTable table)
    {
        List<IReadOnlyDictionary<string, object?>> rows = [];

        foreach (DataRow sourceRow in table.Rows)
        {
            Dictionary<string, object?> row =
                new(StringComparer.OrdinalIgnoreCase);

            foreach (DataColumn column in table.Columns)
            {
                row[column.ColumnName] =
                    sourceRow[column] is DBNull
                        ? null
                        : sourceRow[column];
            }

            rows.Add(row);
        }

        return rows;
    }
}
