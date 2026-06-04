using Microsoft.Data.Sqlite;
using System.Data;
using System.Globalization;

namespace BudgetsAddin;

public sealed class SqliteBudgetRepository
{
    public BudgetLoadResult LoadBudgets(BudgetLoadRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrWhiteSpace(request.DatabasePath))
        {
            return new BudgetLoadResult
            {
                Status = "Failed",
                Message = "DatabasePath is required.",
                Nodes = []
            };
        }

        if (!File.Exists(request.DatabasePath))
        {
            return new BudgetLoadResult
            {
                Status = "Failed",
                Message = $"Database not found: {request.DatabasePath}",
                Nodes = []
            };
        }

        using SqliteConnection connection =
            new($"Data Source={request.DatabasePath};Mode=ReadOnly");

        connection.Open();

        DataTable budgetMonths =
            LoadBudgetMonthRows(connection, request);

        IReadOnlyList<BudgetTreeNode> nodes =
            BuildBudgetTree(budgetMonths);

        return new BudgetLoadResult
        {
            Status = "Succeeded",
            Message = $"Loaded {nodes.Count} budget year node(s).",
            Nodes = nodes
        };
    }

    private static DataTable LoadBudgetMonthRows(SqliteConnection connection, BudgetLoadRequest request)
    {
        using SqliteCommand command = connection.CreateCommand();

        command.CommandText =
            """
            SELECT
                [BudgetMonthId] AS [MonthKey],
                [Year] AS [BudgetYear],
                [Month] AS [BudgetMonthNumber]
            FROM [BudgetMonth]
            ORDER BY [Year] DESC, [Month] ASC
            LIMIT @MaximumRows;
            """;

        command.Parameters.AddWithValue("@MaximumRows", request.MaximumRows <= 0 ? 500 : request.MaximumRows);

        using SqliteDataReader reader = command.ExecuteReader();

        DataTable table = new();
        table.Load(reader);

        return table;
    }

    private static IReadOnlyList<BudgetTreeNode> BuildBudgetTree(DataTable budgetMonths)
    {
        Dictionary<int, List<BudgetTreeNode>> monthsByYear = [];

        foreach (DataRow row in budgetMonths.Rows)
        {
            if (row["BudgetYear"] is DBNull ||
                row["BudgetMonthNumber"] is DBNull)
            {
                continue;
            }

            int year = Convert.ToInt32(row["BudgetYear"], CultureInfo.InvariantCulture);
            int month = Convert.ToInt32(row["BudgetMonthNumber"], CultureInfo.InvariantCulture);
            string monthKey = Convert.ToString(row["MonthKey"], CultureInfo.InvariantCulture) ?? $"{year:D4}-{month:D2}";
            string monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month);

            BudgetTreeNode monthNode = new()
            {
                NodeId = $"budget-month:{year:D4}-{month:D2}",
                DisplayText = monthName,
                NodeType = BudgetTreeNodeTypes.BudgetMonth,
                BudgetYear = year,
                BudgetMonth = month,
                MonthKey = monthKey,
                Children = []
            };

            if (!monthsByYear.TryGetValue(year, out List<BudgetTreeNode>? monthNodes))
            {
                monthNodes = [];
                monthsByYear.Add(year, monthNodes);
            }

            monthNodes.Add(monthNode);
        }

        return monthsByYear
            .OrderByDescending(pair => pair.Key)
            .Select(pair => new BudgetTreeNode
            {
                NodeId = $"budget-year:{pair.Key:D4}",
                DisplayText = pair.Key.ToString(CultureInfo.InvariantCulture),
                NodeType = BudgetTreeNodeTypes.BudgetYear,
                BudgetYear = pair.Key,
                Children = pair.Value
                    .OrderBy(monthNode => monthNode.BudgetMonth)
                    .ToList()
            })
            .ToList();
    }
}
