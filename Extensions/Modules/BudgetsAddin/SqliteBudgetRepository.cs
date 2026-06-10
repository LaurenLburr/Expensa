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
            BuildBudgetTree(budgetMonths, DateTime.Today);

        return new BudgetLoadResult
        {
            Status = "Succeeded",
            Message = $"Loaded {nodes.Count} budget node(s).",
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

    private static IReadOnlyList<BudgetTreeNode> BuildBudgetTree(
        DataTable budgetMonths,
        DateTime today)
    {
        Dictionary<int, List<BudgetTreeNode>> monthsByYear = [];
        Dictionary<string, BudgetTreeNode> monthNodesByKey = [];

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
            monthNodesByKey[$"{year:D4}-{month:D2}"] = monthNode;
        }

        List<BudgetTreeNode> topLevelNodes =
        [
            CreateRelativeMonthNode(
                "budget-current",
                "Current",
                BudgetTreeNodeTypes.BudgetCurrentMonth,
                today,
                monthNodesByKey),

            CreateRelativeMonthNode(
                "budget-previous",
                "Previous",
                BudgetTreeNodeTypes.BudgetPreviousMonth,
                today.AddMonths(-1),
                monthNodesByKey),

            new BudgetTreeNode
            {
                NodeId = "budget-templates",
                DisplayText = "Templates",
                NodeType = BudgetTreeNodeTypes.BudgetTemplates,
                Children = []
            }
        ];

        topLevelNodes.AddRange(
            monthsByYear
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
                }));

        return topLevelNodes;
    }

    private static BudgetTreeNode CreateRelativeMonthNode(
        string nodeId,
        string label,
        string nodeType,
        DateTime monthDate,
        IReadOnlyDictionary<string, BudgetTreeNode> monthNodesByKey)
    {
        int year = monthDate.Year;
        int month = monthDate.Month;
        string lookupKey = $"{year:D4}-{month:D2}";
        string monthAbbreviation = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(month);

        if (monthNodesByKey.TryGetValue(lookupKey, out BudgetTreeNode? existingMonthNode))
        {
            return new BudgetTreeNode
            {
                NodeId = nodeId,
                DisplayText = $"{label} ({monthAbbreviation})",
                NodeType = nodeType,
                BudgetYear = existingMonthNode.BudgetYear,
                BudgetMonth = existingMonthNode.BudgetMonth,
                MonthKey = existingMonthNode.MonthKey,
                Children = []
            };
        }

        return new BudgetTreeNode
        {
            NodeId = nodeId,
            DisplayText = $"{label} ({monthAbbreviation})",
            NodeType = nodeType,
            BudgetYear = year,
            BudgetMonth = month,
            MonthKey = lookupKey,
            Children = []
        };
    }
}
