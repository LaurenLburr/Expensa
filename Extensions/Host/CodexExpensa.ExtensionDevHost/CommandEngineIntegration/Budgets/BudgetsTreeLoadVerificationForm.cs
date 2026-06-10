using CodexExpensa.ExtensionDevHost.CommandEngineIntegration.ExtensionManager;
using CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Templates;
using Microsoft.Data.Sqlite;
using System.Data;
using System.Globalization;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Budgets;

public sealed partial class BudgetsTreeLoadVerificationForm : TreeTestTemplate
{
    private const string AddinId = "BudgetsAddin";
    private const int MaximumRows = 500;

    private readonly HostBudgetTreeContributionLoader loader = new();
    private readonly AddinRuntimeDatabasePathService pathService = new();

    private string databasePath = string.Empty;
    private bool hasAutoLoaded;
    private AddinMemoryDatabaseSession? databaseSession;

    public BudgetsTreeLoadVerificationForm()
    {
        InitializeComponent();

        ConfigureTreeTestTemplate(
            "Budgets Tree Test",
            "Loading Budgets add-in runtime tree...");

        databasePath = ResolveDefaultRuntimeDatabasePath();
        SafeDataGridViewBinding.Attach(ResultGrid);
        TestTreeView.AfterSelect += TestTreeView_AfterSelect;
    }

    protected override async void OnShown(EventArgs e)
    {
        base.OnShown(e);

        if (hasAutoLoaded)
        {
            return;
        }

        hasAutoLoaded = true;
        await LoadBudgetsTreeAsync().ConfigureAwait(true);
    }

    private async Task LoadBudgetsTreeAsync()
    {
        ClearTemplate();

        if (string.IsNullOrWhiteSpace(databasePath) || !File.Exists(databasePath))
        {
            SetNotes(
                "Budgets add-in runtime database was not found." + Environment.NewLine + Environment.NewLine +
                "Expected add-in runtime path:" + Environment.NewLine +
                pathService.GetRuntimeDatabaseLocation(AddinId).DatabasePath + Environment.NewLine + Environment.NewLine +
                "Use the Budgets Database page to update data from Prod or Dev, then run this tree test again.");

            return;
        }

        SetNotes(
            "Loading Budgets from add-in runtime database:" + Environment.NewLine +
            databasePath);

        try
        {
            HostBudgetTreeLoadResult result =
                await loader.LoadContributionAsync(
                    TestTreeView,
                    new HostBudgetTreeLoadOptions
                    {
                        ExpandAll = true,
                        MaximumRows = MaximumRows
                    }).ConfigureAwait(true);

            ReloadMemoryDatabase();
            SelectFirstBudgetNode();

            if (TestTreeView.SelectedNode is null)
            {
                SetNotes(
                    $"{result.ExecutionResult.Status}: {result.RootNodeCount} root node(s)." + Environment.NewLine +
                    result.ExecutionResult.Message + Environment.NewLine + Environment.NewLine +
                    "No budget month node was found to select.");
            }
        }
        catch (Exception exception)
        {
            ReleaseMemoryDatabase();
            SetGridData((object?)null);
            SetNotes(exception.ToString());

            MessageBox.Show(
                this,
                exception.Message,
                "Budgets Tree Test",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }

    private void TestTreeView_AfterSelect(
        object? sender,
        TreeViewEventArgs e)
    {
        if (e.Node is null)
        {
            SetGridData((object?)null);
            return;
        }

        LoadSelectedBudget(e.Node);
    }

    private void LoadSelectedBudget(TreeNode node)
    {
        HostBudgetTreeNodePayload? payload =
            node.Tag as HostBudgetTreeNodePayload;

        if (payload is null)
        {
            SetGridData((object?)null);
            SetNotes(
                "Selected node does not contain a Budgets payload." + Environment.NewLine + Environment.NewLine +
                FormatSelectedNode(node));
            return;
        }

        if (!HasBudgetMonthIdentity(payload))
        {
            DataTable childBudgets = BuildChildBudgetTable(node);
            SetGridData(SafeDataGridViewBinding.Sanitize(childBudgets));
            SetNotes(
                $"Selected: {node.Text}{Environment.NewLine}" +
                $"Node type: {payload.NodeType}{Environment.NewLine}" +
                $"Child budgets shown: {childBudgets.Rows.Count}{Environment.NewLine}" +
                "Select a budget month to show the selected BudgetMonth row from the add-in runtime database." + Environment.NewLine + Environment.NewLine +
                "Database is loaded into memory from:" + Environment.NewLine +
                databasePath);
            return;
        }

        try
        {
            DataTable selectedBudget =
                LoadSelectedBudgetMonth(GetMemoryConnection(), payload);

            SetGridData(SafeDataGridViewBinding.Sanitize(selectedBudget));
            SetNotes(
                $"Selected budget: {payload.DisplayText}{Environment.NewLine}" +
                $"Node type: {payload.NodeType}{Environment.NewLine}" +
                $"Budget year: {FormatNullableInt(payload.BudgetYear)}{Environment.NewLine}" +
                $"Budget month: {FormatNullableInt(payload.BudgetMonth)}{Environment.NewLine}" +
                $"Month key: {payload.MonthKey}{Environment.NewLine}" +
                "Database is loaded into memory from:" + Environment.NewLine +
                databasePath + Environment.NewLine +
                "The source database file is closed after the memory load completes." + Environment.NewLine +
                $"Rows shown: {selectedBudget.Rows.Count}");
        }
        catch (Exception exception)
        {
            SetGridData((object?)null);
            SetNotes(exception.ToString());
        }
    }

    private SqliteConnection GetMemoryConnection()
    {
        if (databaseSession is null)
        {
            ReloadMemoryDatabase();
        }

        return databaseSession?.Connection
            ?? throw new InvalidOperationException("Budgets add-in database is not loaded into memory.");
    }

    private void ReloadMemoryDatabase()
    {
        ReleaseMemoryDatabase();
        databaseSession = AddinMemoryDatabaseSession.LoadFromFile(databasePath);
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        ReleaseMemoryDatabase();
        base.OnFormClosed(e);
    }

    private void ReleaseMemoryDatabase()
    {
        databaseSession?.Dispose();
        databaseSession = null;
    }

    private static DataTable LoadSelectedBudgetMonth(
        SqliteConnection connection,
        HostBudgetTreeNodePayload payload)
    {
        ArgumentNullException.ThrowIfNull(connection);
        ArgumentNullException.ThrowIfNull(payload);

        if (!TableExists(connection, "BudgetMonth"))
        {
            return CreateMessageTable("BudgetMonth table was not found in the selected add-in runtime database.");
        }

        List<string> columns =
            GetColumnNames(connection, "BudgetMonth");

        using SqliteCommand command =
            connection.CreateCommand();

        List<string> whereClauses = [];

        if (columns.Contains("BudgetMonthId", StringComparer.OrdinalIgnoreCase) &&
            !string.IsNullOrWhiteSpace(payload.MonthKey))
        {
            whereClauses.Add("[BudgetMonthId] = @MonthKey");
            command.Parameters.AddWithValue("@MonthKey", payload.MonthKey);
        }

        if (columns.Contains("Year", StringComparer.OrdinalIgnoreCase) &&
            columns.Contains("Month", StringComparer.OrdinalIgnoreCase) &&
            payload.BudgetYear.HasValue &&
            payload.BudgetMonth.HasValue)
        {
            whereClauses.Add("([Year] = @BudgetYear AND [Month] = @BudgetMonth)");
            command.Parameters.AddWithValue("@BudgetYear", payload.BudgetYear.Value);
            command.Parameters.AddWithValue("@BudgetMonth", payload.BudgetMonth.Value);
        }

        if (whereClauses.Count == 0)
        {
            return CreateMessageTable("BudgetMonth exists, but no usable BudgetMonthId or Year/Month columns were found.");
        }

        command.CommandText =
            $"""
            SELECT *
            FROM [BudgetMonth]
            WHERE {string.Join(" OR ", whereClauses)}
            LIMIT 500;
            """;

        return LoadDataTable(command);
    }

    private static DataTable BuildChildBudgetTable(TreeNode selectedNode)
    {
        DataTable table = new("ChildBudgets");
        table.Columns.Add("DisplayText", typeof(string));
        table.Columns.Add("NodeType", typeof(string));
        table.Columns.Add("BudgetYear", typeof(string));
        table.Columns.Add("BudgetMonth", typeof(string));
        table.Columns.Add("MonthKey", typeof(string));
        table.Columns.Add("NodeId", typeof(string));

        AddChildBudgetRows(table, selectedNode.Nodes.Cast<TreeNode>());

        if (table.Rows.Count == 0)
        {
            table.Rows.Add(
                "No child budget months",
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                selectedNode.Name);
        }

        return table;
    }

    private static void AddChildBudgetRows(
        DataTable table,
        IEnumerable<TreeNode> nodes)
    {
        foreach (TreeNode node in nodes)
        {
            if (node.Tag is HostBudgetTreeNodePayload payload && HasBudgetMonthIdentity(payload))
            {
                table.Rows.Add(
                    payload.DisplayText,
                    payload.NodeType.ToString(),
                    FormatNullableInt(payload.BudgetYear),
                    FormatNullableInt(payload.BudgetMonth),
                    payload.MonthKey,
                    payload.NodeId);
            }

            AddChildBudgetRows(table, node.Nodes.Cast<TreeNode>());
        }
    }

    private static bool HasBudgetMonthIdentity(HostBudgetTreeNodePayload payload)
    {
        ArgumentNullException.ThrowIfNull(payload);

        return !string.IsNullOrWhiteSpace(payload.MonthKey) ||
            (payload.BudgetYear.HasValue && payload.BudgetMonth.HasValue);
    }

    private static bool TableExists(SqliteConnection connection, string tableName)
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

        object? result = command.ExecuteScalar();
        return result is not null && result is not DBNull;
    }

    private static List<string> GetColumnNames(SqliteConnection connection, string tableName)
    {
        using SqliteCommand command =
            connection.CreateCommand();

        command.CommandText =
            $"PRAGMA table_info([{tableName.Replace("]", "]]", StringComparison.Ordinal)}]);";

        DataTable table = LoadDataTable(command);

        return table.Rows
            .Cast<DataRow>()
            .Select(row => Convert.ToString(row["name"], CultureInfo.InvariantCulture) ?? string.Empty)
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .ToList();
    }

    private static DataTable LoadDataTable(SqliteCommand command)
    {
        using SqliteDataReader reader =
            command.ExecuteReader();

        DataTable table = new();
        table.Load(reader);
        return table;
    }

    private static DataTable CreateMessageTable(string message)
    {
        DataTable table = new();
        table.Columns.Add("Message", typeof(string));
        table.Rows.Add(message);
        return table;
    }

    private void SelectFirstBudgetNode()
    {
        TreeNode? budgetNode =
            FindFirstBudgetNode(TestTreeView.Nodes.Cast<TreeNode>());

        if (budgetNode is null)
        {
            SetGridData((object?)null);
            SetNotes(
                "Budgets tree loaded, but no budget month node was found." + Environment.NewLine + Environment.NewLine +
                "Add-in DB:" + Environment.NewLine +
                databasePath);
            return;
        }

        TestTreeView.SelectedNode = budgetNode;
        budgetNode.EnsureVisible();
    }

    private static TreeNode? FindFirstBudgetNode(IEnumerable<TreeNode> nodes)
    {
        foreach (TreeNode node in nodes)
        {
            if (node.Tag is HostBudgetTreeNodePayload payload && HasBudgetMonthIdentity(payload))
            {
                return node;
            }

            TreeNode? child = FindFirstBudgetNode(node.Nodes.Cast<TreeNode>());

            if (child is not null)
            {
                return child;
            }
        }

        return null;
    }

    private string ResolveDefaultRuntimeDatabasePath()
    {
        string activeRuntimePathFile =
            pathService.GetActiveRuntimePathFile(AddinId);

        if (File.Exists(activeRuntimePathFile))
        {
            string activeDatabasePath = File.ReadAllText(activeRuntimePathFile).Trim();

            if (!string.IsNullOrWhiteSpace(activeDatabasePath) && File.Exists(activeDatabasePath))
            {
                return activeDatabasePath;
            }
        }

        return pathService.GetRuntimeDatabaseLocation(AddinId).DatabasePath;
    }

    private static string FormatSelectedNode(TreeNode node)
    {
        return
            $"Text: {node.Text}{Environment.NewLine}" +
            $"Name: {node.Name}{Environment.NewLine}" +
            $"Tag type: {node.Tag?.GetType().FullName ?? "(null)"}{Environment.NewLine}" +
            $"Tag:{Environment.NewLine}{Convert.ToString(node.Tag, CultureInfo.InvariantCulture)}";
    }

    private static string FormatNullableInt(int? value)
    {
        return value.HasValue
            ? value.Value.ToString(CultureInfo.InvariantCulture)
            : string.Empty;
    }
}
