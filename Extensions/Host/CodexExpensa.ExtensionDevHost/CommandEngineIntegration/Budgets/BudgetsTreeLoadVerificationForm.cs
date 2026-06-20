using CodexExpensa.ExtensionDevHost.CommandEngineIntegration.ExtensionManager;
using CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Templates;
using Microsoft.Data.Sqlite;
using System.Data;
using System.Globalization;
using System.Windows.Forms.Integration;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Budgets;

public sealed partial class BudgetsTreeLoadVerificationForm : TreeTestTemplate
{
    private const string AddinId = "BudgetsAddin";
    private const int MaximumRows = 500;
    private const string BudgetMonthHierarchyQueryName = "Budgets.SelectBudgetMonthHierarchy";

    private readonly HostBudgetTreeContributionLoader _loader = new();
    private readonly HostBudgetRuntimeDatabaseSelectionService _databaseSelectionService = new();
    private readonly ContextMenuStrip _budgetGridContextMenu = new();
    private readonly ToolStripMenuItem _enterTransactionMenuItem = new("Enter Transaction...");
    private readonly ToolStripMenuItem _enterPaymentMenuItem = new("Enter Payment...");
    private readonly BudgetTransactionWpfGridControl _budgetRowsGrid = new();
    private readonly ElementHost _budgetRowsHost = new();
    private readonly HashSet<string> _collapsedBudgetRowIds = new(StringComparer.OrdinalIgnoreCase);
    private DataTable? _budgetRowsTable;
    private BudgetTransactionWpfGridRow? _currentBudgetRow;

    private string _databasePath = string.Empty;
    private bool _hasAutoLoaded;
    private AddinMemoryDatabaseSession? _databaseSession;

    public BudgetsTreeLoadVerificationForm()
    {
        InitializeComponent();
        ConfigureTreeTestTemplate(
            "Budgets Tree Load Verification",
            "Loading Budgets add-in runtime tree...");

        ConfigureBudgetRowsGrid();
        TestTreeView.AfterSelect += TestTreeView_AfterSelect;
        ConfigureBudgetGridContextMenu();
    }

    private void ConfigureBudgetRowsGrid()
    {
        ResultGrid.Visible = false;

        _budgetRowsHost.Dock = DockStyle.Fill;
        _budgetRowsHost.Child = _budgetRowsGrid;

        ResultGrid.Parent?.Controls.Add(_budgetRowsHost);
        _budgetRowsHost.BringToFront();
        _budgetRowsGrid.TransactionRowDoubleClicked += (_, row) =>
        {
            _currentBudgetRow = row;
            EnterTransactionForCurrentRow(isPayment: true);
        };
    }

    private void ConfigureBudgetGridContextMenu()
    {
        _enterTransactionMenuItem.Click += (_, _) =>
            EnterTransactionForCurrentRow(isPayment: false);

        _enterPaymentMenuItem.Click += (_, _) =>
            EnterTransactionForCurrentRow(isPayment: true);

        _budgetGridContextMenu.Items.Add(_enterTransactionMenuItem);
        _budgetGridContextMenu.Items.Add(_enterPaymentMenuItem);

        _budgetRowsGrid.RowContextMenuRequested += (_, row) =>
            ShowBudgetGridContextMenu(row);
    }

    private void ShowBudgetGridContextMenu(
        BudgetTransactionWpfGridRow row)
    {
        _currentBudgetRow = row;

        bool isTransactionRow =
            IsTransactionRow(row);

        _enterTransactionMenuItem.Visible = !isTransactionRow;
        _enterPaymentMenuItem.Visible = isTransactionRow;

        _enterTransactionMenuItem.Enabled = !isTransactionRow;
        _enterPaymentMenuItem.Enabled = isTransactionRow;

        _budgetGridContextMenu.Show(
            _budgetRowsHost,
            _budgetRowsHost.PointToClient(Cursor.Position));
    }

    protected override async void OnShown(EventArgs e)
    {
        base.OnShown(e);

        if (_hasAutoLoaded)
        {
            return;
        }

        _hasAutoLoaded = true;
        await LoadBudgetsTreeAsync().ConfigureAwait(true);
    }

    private async Task LoadBudgetsTreeAsync()
    {
        ClearTemplate();
        SetBudgetGridData(null);
        _databasePath = ResolveActiveRuntimeDatabasePath();

        if (string.IsNullOrWhiteSpace(_databasePath) || !File.Exists(_databasePath))
        {
            SetNotes(
                "Budgets add-in runtime database was not found." + Environment.NewLine + Environment.NewLine +
                "Expected add-in runtime path:" + Environment.NewLine +
                _databasePath + Environment.NewLine + Environment.NewLine +
                "Use the Budgets Database page to update data from Prod or Dev, then reload this tree test page.");
            SetBudgetGridData(null);
            return;
        }

        SetNotes(
            "Loading Budgets from add-in runtime database:" + Environment.NewLine +
            _databasePath);

        try
        {
            HostBudgetTreeLoadResult result =
                await _loader.LoadContributionAsync(
                    TestTreeView,
                    new HostBudgetTreeLoadOptions
                    {
                        ExpandAll = true,
                        MaximumRows = MaximumRows
                    }).ConfigureAwait(true);

            ReloadMemoryDatabase();

            SetNotes(
                $"{result.ExecutionResult.Status}: {result.RootNodeCount} root node(s).{Environment.NewLine}" +
                result.ExecutionResult.Message + Environment.NewLine + Environment.NewLine +
                "Database is loaded into memory from:" + Environment.NewLine +
                _databasePath);

            SelectFirstBudgetNode();
        }
        catch (Exception exception)
        {
            ReleaseMemoryDatabase();
            SetBudgetGridData(null);
            SetNotes(exception.ToString());

            MessageBox.Show(
                this,
                exception.Message,
                "Budgets Tree Load Verification",
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
            SetNotes(string.Empty);
            SetBudgetGridData(null);
            return;
        }

        LoadBudgetMonthRowsForSelectedBudget(e.Node);
    }

    private void LoadBudgetMonthRowsForSelectedBudget(TreeNode selectedNode)
    {
        ArgumentNullException.ThrowIfNull(selectedNode);

        if (selectedNode.Tag is not HostBudgetTreeNodePayload payload)
        {
            SetBudgetGridData(null);
            SetNotes(
                "Selected node does not contain a Budgets payload." + Environment.NewLine + Environment.NewLine +
                FormatSelectedNode(selectedNode));
            return;
        }

        try
        {
            BudgetMonthQueryResult queryResult =
                LoadMatchingBudgetMonthRows(GetMemoryConnection(), payload);

            _collapsedBudgetRowIds.Clear();
            SetBudgetGridData(
                CreateBudgetDisplayGridTable(
                    SafeDataGridViewBinding.Sanitize(
                        queryResult.Rows)));

            SetNotes(
                $"Selected budget: {selectedNode.Text}{Environment.NewLine}" +
                $"Node type: {payload.NodeType}{Environment.NewLine}" +
                $"BudgetYear: {payload.BudgetYear}{Environment.NewLine}" +
                $"BudgetMonth: {payload.BudgetMonth}{Environment.NewLine}" +
                $"MonthKey: {payload.MonthKey}{Environment.NewLine}" + Environment.NewLine +
                "Budget rows and matching transactions are loaded into the grid from the add-in runtime database." + Environment.NewLine +
                "The budget selection matches only BudgetMonthRow.BudgetMonthId. Transactions are associated by matching BudgetMonthRow.Name to Payee.PayeeName and by the selected budget year/month." + Environment.NewLine +
                "SQL query used:" + Environment.NewLine +
                queryResult.Sql + Environment.NewLine + Environment.NewLine +
                "SQL parameters:" + Environment.NewLine +
                queryResult.Parameters + Environment.NewLine + Environment.NewLine +
                "Database is loaded into memory from:" + Environment.NewLine +
                _databasePath + Environment.NewLine +
                "The source database file is closed after the memory load completes." + Environment.NewLine +
                $"Grid rows shown: {queryResult.Rows.Rows.Count}");
        }
        catch (Exception exception)
        {
            SetBudgetGridData(null);
            SetNotes(exception.ToString());
        }
    }

    private static DataTable CreateHierarchyGridTable(
        DataTable source)
    {
        ArgumentNullException.ThrowIfNull(source);

        DataColumn? rowTypeColumn =
            FindColumn(
                source,
                "RowType",
                "HierarchyRowType",
                "NodeType",
                "RecordType");

        DataColumn? budgetRowIdColumn =
            FindColumn(
                source,
                "BudgetMonthRowId",
                "BudgetRowId",
                "ParentId");

        DataColumn? transactionIdColumn =
            FindColumn(
                source,
                "TransactionId",
                "TxnId",
                "ChildId");

        if (rowTypeColumn is null ||
            budgetRowIdColumn is null)
        {
            throw new InvalidOperationException(
                "The Budget hierarchy query must return RowType and BudgetMonthRowId columns.");
        }

        DataTable hierarchy =
            source.Copy();

        hierarchy.Columns.Add(
            "NodeId",
            typeof(string));

        hierarchy.Columns.Add(
            "ParentNodeId",
            typeof(string));

        int rowOrdinal = 0;

        foreach (DataRow row in hierarchy.Rows)
        {
            rowOrdinal++;

            string rowType =
                Convert.ToString(
                    row[rowTypeColumn.ColumnName],
                    CultureInfo.InvariantCulture)
                ?? string.Empty;

            string budgetRowId =
                Convert.ToString(
                    row[budgetRowIdColumn.ColumnName],
                    CultureInfo.InvariantCulture)
                ?? string.Empty;

            bool isTransaction =
                rowType.Contains(
                    "Transaction",
                    StringComparison.OrdinalIgnoreCase)
                ||
                rowType.Contains(
                    "Child",
                    StringComparison.OrdinalIgnoreCase);

            if (!isTransaction)
            {
                row["NodeId"] =
                    CreateBudgetNodeId(
                        budgetRowId,
                        rowOrdinal);

                row["ParentNodeId"] =
                    string.Empty;

                continue;
            }

            string transactionId =
                transactionIdColumn is null
                    ? string.Empty
                    : Convert.ToString(
                          row[transactionIdColumn.ColumnName],
                          CultureInfo.InvariantCulture)
                      ?? string.Empty;

            row["NodeId"] =
                CreateTransactionNodeId(
                    transactionId,
                    rowOrdinal);

            row["ParentNodeId"] =
                CreateBudgetNodeId(
                    budgetRowId,
                    rowOrdinal);
        }

        return hierarchy;
    }

    private void SetBudgetGridData(DataTable? table)
    {
        _budgetRowsTable =
            table is null
                ? null
                : table.Copy();

        RenderBudgetGridRows();
    }

    private void RenderBudgetGridRows()
    {
        if (_budgetRowsTable is null)
        {
            _budgetRowsGrid.LoadRows(null);
            return;
        }

        _budgetRowsGrid.LoadRows(_budgetRowsTable);
    }

    private static bool IsTransactionRow(DataRow row)
    {
        string rowType =
            GetDataRowString(row, "RowType");

        return string.Equals(
            rowType,
            "Transaction",
            StringComparison.OrdinalIgnoreCase)
            ||
            string.Equals(
                rowType,
                "Child",
                StringComparison.OrdinalIgnoreCase);
    }

    private static string GetDataRowString(
        DataRow row,
        string columnName)
    {
        if (!row.Table.Columns.Contains(columnName))
        {
            return string.Empty;
        }

        return Convert.ToString(
            row[columnName],
            CultureInfo.InvariantCulture)
            ?? string.Empty;
    }

    private static DataTable CreateBudgetDisplayGridTable(
        DataTable source)
    {
        ArgumentNullException.ThrowIfNull(source);

        DataTable display =
            source.Copy();

        MoveColumnFirst(display, "Item");
        MoveColumnLast(display, "RowType");
        MoveColumnLast(display, "PayeeId");

        return display;
    }

    private static void MoveColumnFirst(
        DataTable table,
        string columnName)
    {
        DataColumn? column =
            FindColumn(table, columnName);

        column?.SetOrdinal(0);
    }

    private static void MoveColumnLast(
        DataTable table,
        string columnName)
    {
        DataColumn? column =
            FindColumn(table, columnName);

        column?.SetOrdinal(table.Columns.Count - 1);
    }

    private static string CreateBudgetNodeId(
        string budgetRowId,
        int fallbackOrdinal)
    {
        return string.IsNullOrWhiteSpace(budgetRowId)
            ? $"budget-row:{fallbackOrdinal}"
            : $"budget-row:{budgetRowId}";
    }

    private static string CreateTransactionNodeId(
        string transactionId,
        int fallbackOrdinal)
    {
        return string.IsNullOrWhiteSpace(transactionId)
            ? $"transaction:{fallbackOrdinal}"
            : $"transaction:{transactionId}";
    }

    private void HideHierarchyIdentityColumns()
    {
        HideHierarchyColumn("NodeId");
        HideHierarchyColumn("ParentNodeId");
    }

    private void HideHierarchyColumn(string columnName)
    {
        if (HierarchyGrid.Columns.Contains(columnName))
        {
            HierarchyGrid.Columns[columnName]!.Visible = false;
        }
    }

    private void EnterTransactionForCurrentRow(bool isPayment)
    {
        try
        {
            BudgetTransactionWpfGridRow? currentRow =
                _currentBudgetRow ?? _budgetRowsGrid.SelectedRow;

            if (currentRow is null)
            {
                MessageBox.Show(
                    this,
                    isPayment
                        ? "Select a transaction row first."
                        : "Select a budget row first.",
                    isPayment ? "Enter Payment" : "Enter Transaction",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            bool selectedRowIsTransaction =
                IsTransactionRow(currentRow);

            if (isPayment && !selectedRowIsTransaction)
            {
                MessageBox.Show(
                    this,
                    "Select an existing transaction row to enter a payment.",
                    "Enter Payment",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            if (!isPayment && selectedRowIsTransaction)
            {
                MessageBox.Show(
                    this,
                    "Select a non-transaction budget row to enter a transaction.",
                    "Enter Transaction",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            SqliteConnection connection =
                GetMemoryConnection();

            string payeeName =
                GetCellString(currentRow, "Payee");

            if (string.IsNullOrWhiteSpace(payeeName))
            {
                MessageBox.Show(
                    this,
                    "The selected BudgetMonthRow does not include a Payee. Fix the test data or SqlCatalog query so every budget row has a mapped Payee.",
                    isPayment ? "Enter Payment" : "Enter Transaction",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            string? payeeId =
                GetCellString(currentRow, "PayeeId");

            if (string.IsNullOrWhiteSpace(payeeId))
            {
                MessageBox.Show(
                    this,
                    "The selected BudgetMonthRow does not include a PayeeId. Fix the test data or SqlCatalog query so every budget row has a mapped Payee.",
                    isPayment ? "Enter Payment" : "Enter Transaction",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            IReadOnlyList<BudgetTransactionAccountChoice> accounts =
                LoadAccountChoices(connection);

            if (accounts.Count == 0)
            {
                MessageBox.Show(
                    this,
                    "No Account rows are available for the transaction.",
                    isPayment ? "Enter Payment" : "Enter Transaction",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            using BudgetTransactionEntryDialog dialog =
                new(
                    accounts,
                    payeeName,
                    GetDefaultTransactionDate(currentRow),
                    isPayment ? "Enter Payment" : "Enter Transaction",
                    isPayment ? "Enter Payment" : "Enter Transaction",
                    isPayment ? "Cleared" : "Outstanding",
                    GetCellDecimal(currentRow, "Amount"));

            if (dialog.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            if (isPayment)
            {
                UpdateExistingTransaction(
                    connection,
                    GetRequiredTransactionId(currentRow),
                    dialog);
            }
            else
            {
                InsertTransaction(
                    connection,
                    payeeId,
                    dialog);
            }

            _databaseSession?.Save();
            ReloadMemoryDatabase();

            if (TestTreeView.SelectedNode is not null)
            {
                LoadBudgetMonthRowsForSelectedBudget(TestTreeView.SelectedNode);
            }
        }
        catch (Exception exception)
        {
            MessageBox.Show(
                this,
                exception.ToString(),
                isPayment ? "Enter Payment failed" : "Enter Transaction failed",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }

    private static DataColumn? FindColumn(
        DataTable table,
        params string[] candidateNames)
    {
        foreach (string candidateName in candidateNames)
        {
            DataColumn? column =
                table.Columns
                    .Cast<DataColumn>()
                    .FirstOrDefault(
                        item =>
                            string.Equals(
                                item.ColumnName,
                                candidateName,
                                StringComparison.OrdinalIgnoreCase));

            if (column is not null)
            {
                return column;
            }
        }

        return null;
    }

    private static bool IsTransactionRow(DataGridViewRow row)
    {
        string rowType =
            GetCellString(row, "RowType");

        return string.Equals(
            rowType,
            "Transaction",
            StringComparison.OrdinalIgnoreCase)
            ||
            string.Equals(
                rowType,
                "Child",
                StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsTransactionRow(BudgetTransactionWpfGridRow row) =>
        row.IsTransaction;

    private static string GetCellString(
        BudgetTransactionWpfGridRow row,
        string columnName) =>
        row.GetValue(columnName);

    private static string GetCellString(
        DataGridViewRow row,
        string columnName)
    {
        if (row.DataBoundItem is DataRowView rowView &&
            rowView.Row.Table.Columns.Contains(columnName))
        {
            return Convert.ToString(
                rowView.Row[columnName],
                CultureInfo.InvariantCulture)
                ?? string.Empty;
        }

        object? value =
            TryGetCellValue(
                row,
                columnName,
                out object? cellValue)
                ? cellValue
                : null;

        return Convert.ToString(
            value,
            CultureInfo.InvariantCulture)
            ?? string.Empty;
    }

    private static bool TryGetCellValue(
        DataGridViewRow row,
        string columnName,
        out object? value)
    {
        value = null;

        DataGridView? grid =
            row.DataGridView;

        if (grid is null)
        {
            return false;
        }

        if (grid.Columns.Contains(columnName))
        {
            value = row.Cells[columnName].Value;
            return true;
        }

        foreach (DataGridViewColumn column in grid.Columns)
        {
            if (string.Equals(
                    column.Name,
                    columnName,
                    StringComparison.OrdinalIgnoreCase)
                ||
                string.Equals(
                    column.HeaderText,
                    columnName,
                    StringComparison.OrdinalIgnoreCase)
                ||
                string.Equals(
                    column.DataPropertyName,
                    columnName,
                    StringComparison.OrdinalIgnoreCase))
            {
                value = row.Cells[column.Index].Value;
                return true;
            }
        }

        return false;
    }

    private static decimal GetCellDecimal(
        BudgetTransactionWpfGridRow row,
        string columnName)
    {
        string value =
            GetCellString(
                row,
                columnName);

        return decimal.TryParse(
            value,
            NumberStyles.Any,
            CultureInfo.InvariantCulture,
            out decimal amount)
            ? amount
            : 0m;
    }

    private DateTime GetDefaultTransactionDate(BudgetTransactionWpfGridRow row)
    {
        string startDate =
            GetCellString(
                row,
                "StartDate");

        if (DateTime.TryParse(
                startDate,
                CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeLocal,
                out DateTime parsedDate))
        {
            return parsedDate.Date;
        }

        if (TestTreeView.SelectedNode?.Tag is HostBudgetTreeNodePayload payload &&
            payload.BudgetYear is int year &&
            payload.BudgetMonth is int month &&
            month is >= 1 and <= 12)
        {
            DateTime today =
                DateTime.Today;

            return today.Year == year &&
                today.Month == month
                ? today
                : new DateTime(year, month, 1);
        }

        return DateTime.Today;
    }

    private static IReadOnlyList<BudgetTransactionAccountChoice> LoadAccountChoices(
        SqliteConnection connection)
    {
        using SqliteCommand command =
            connection.CreateCommand();

        command.CommandText =
            """
            SELECT
                [AccountId],
                [AccountNickname]
            FROM [Account]
            ORDER BY
                [SortIndex],
                [AccountNickname];
            """;

        using SqliteDataReader reader =
            command.ExecuteReader();

        List<BudgetTransactionAccountChoice> accounts = [];

        while (reader.Read())
        {
            string accountId =
                Convert.ToString(
                    reader["AccountId"],
                    CultureInfo.InvariantCulture)
                ?? string.Empty;

            if (string.IsNullOrWhiteSpace(accountId))
            {
                continue;
            }

            string displayText =
                Convert.ToString(
                    reader["AccountNickname"],
                    CultureInfo.InvariantCulture)
                ?? accountId;

            accounts.Add(
                new BudgetTransactionAccountChoice(
                    accountId,
                    string.IsNullOrWhiteSpace(displayText)
                        ? accountId
                        : displayText));
        }

        return accounts;
    }

    private static string GetRequiredTransactionId(BudgetTransactionWpfGridRow row)
    {
        string transactionId =
            GetCellString(
                row,
                "TransactionId");

        if (string.IsNullOrWhiteSpace(transactionId))
        {
            throw new InvalidOperationException(
                "The selected transaction row does not include a TransactionId.");
        }

        return transactionId;
    }

    private static void InsertTransaction(
        SqliteConnection connection,
        string payeeId,
        BudgetTransactionEntryDialog dialog)
    {
        using SqliteCommand command =
            connection.CreateCommand();

        command.CommandText =
            """
            INSERT INTO [Txn]
            (
                [AccountId],
                [PayeeId],
                [Status],
                [Amount],
                [StartDate],
                [ConfirmationNumber],
                [Note]
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

        AddTransactionParameters(
            command,
            payeeId,
            dialog);

        command.ExecuteNonQuery();
    }

    private static void UpdateExistingTransaction(
        SqliteConnection connection,
        string transactionId,
        BudgetTransactionEntryDialog dialog)
    {
        using SqliteCommand command =
            connection.CreateCommand();

        command.CommandText =
            """
            UPDATE [Txn]
            SET
                [AccountId] = @AccountId,
                [Status] = @Status,
                [Amount] = @Amount,
                [StartDate] = @StartDate,
                [ConfirmationNumber] = @ConfirmationNumber,
                [Note] = @Note
            WHERE [TransactionId] = @TransactionId;
            """;

        AddTransactionParameters(
            command,
            payeeId: null,
            dialog);

        command.Parameters.AddWithValue(
            "@TransactionId",
            transactionId);

        command.ExecuteNonQuery();
    }

    private static void AddTransactionParameters(
        SqliteCommand command,
        string? payeeId,
        BudgetTransactionEntryDialog dialog)
    {
        command.Parameters.AddWithValue(
            "@AccountId",
            dialog.AccountId);

        if (payeeId is not null)
        {
            command.Parameters.AddWithValue(
                "@PayeeId",
                payeeId);
        }

        command.Parameters.AddWithValue(
            "@Status",
            dialog.Status);

        command.Parameters.AddWithValue(
            "@Amount",
            dialog.Amount);

        command.Parameters.AddWithValue(
            "@StartDate",
            dialog.StartDate.ToString(
                "yyyy-MM-dd",
                CultureInfo.InvariantCulture));

        command.Parameters.AddWithValue(
            "@ConfirmationNumber",
            string.IsNullOrWhiteSpace(dialog.ConfirmationNumber)
                ? DBNull.Value
                : dialog.ConfirmationNumber);

        command.Parameters.AddWithValue(
            "@Note",
            string.IsNullOrWhiteSpace(dialog.Note)
                ? DBNull.Value
                : dialog.Note);
    }

    private SqliteConnection GetMemoryConnection()
    {
        if (_databaseSession is null)
        {
            ReloadMemoryDatabase();
        }

        return _databaseSession?.Connection
            ?? throw new InvalidOperationException("Budgets add-in database is not loaded into memory.");
    }

    private void ReloadMemoryDatabase()
    {
        ReleaseMemoryDatabase();
        _databaseSession = AddinMemoryDatabaseSession.LoadFromFile(_databasePath);
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        ReleaseMemoryDatabase();
        base.OnFormClosed(e);
    }

    private void ReleaseMemoryDatabase()
    {
        _databaseSession?.Dispose();
        _databaseSession = null;
    }

    private static BudgetMonthQueryResult LoadMatchingBudgetMonthRows(
        SqliteConnection connection,
        HostBudgetTreeNodePayload payload)
    {
        ArgumentNullException.ThrowIfNull(connection);
        ArgumentNullException.ThrowIfNull(payload);

        BudgetsSqlQueryCatalog catalog = new(connection);
        string sql = catalog.GetRequiredSqlText(BudgetMonthHierarchyQueryName);

        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = sql;
        command.Parameters.AddWithValue(
            "@BudgetMonthId",
            string.IsNullOrWhiteSpace(payload.MonthKey)
                ? DBNull.Value
                : payload.MonthKey);

        string parameters = FormatParameters(command);
        DataTable rows = LoadDataTable(command);

        return new BudgetMonthQueryResult(rows, sql.Trim(), parameters);
    }

    private static string FormatParameters(SqliteCommand command)
    {
        List<string> lines = [];

        foreach (SqliteParameter parameter in command.Parameters)
        {
            lines.Add(
                parameter.ParameterName +
                " = " +
                Convert.ToString(parameter.Value, CultureInfo.InvariantCulture));
        }

        lines.Add("@MaximumRows = " + MaximumRows.ToString(CultureInfo.InvariantCulture));

        return lines.Count == 0
            ? "(none)"
            : string.Join(Environment.NewLine, lines);
    }

    private static DataTable LoadDataTable(SqliteCommand command)
    {
        command.Parameters.AddWithValue("@MaximumRows", MaximumRows);

        using SqliteDataReader reader =
            command.ExecuteReader();

        DataTable table = new();

        // Do not use DataTable.Load(reader) here. The hierarchy query returns one
        // BudgetMonthRow parent followed by zero or more transaction children.
        // That intentionally repeats BudgetMonthRowId. DataTable.Load can infer
        // the source primary-key/unique constraint and reject the child rows.
        for (int ordinal = 0; ordinal < reader.FieldCount; ordinal++)
        {
            table.Columns.Add(reader.GetName(ordinal), typeof(object));
        }

        table.BeginLoadData();

        try
        {
            while (reader.Read())
            {
                DataRow row = table.NewRow();

                for (int ordinal = 0; ordinal < reader.FieldCount; ordinal++)
                {
                    row[ordinal] = reader.IsDBNull(ordinal)
                        ? DBNull.Value
                        : reader.GetValue(ordinal);
                }

                table.Rows.Add(row);
            }
        }
        finally
        {
            table.EndLoadData();
        }

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
                "Budgets tree loaded, but no BudgetMonth-backed node was found." + Environment.NewLine + Environment.NewLine +
                "Add-in DB:" + Environment.NewLine +
                _databasePath);
            return;
        }

        TestTreeView.SelectedNode = budgetNode;
        budgetNode.EnsureVisible();
    }

    private static TreeNode? FindFirstBudgetNode(IEnumerable<TreeNode> nodes)
    {
        foreach (TreeNode node in nodes)
        {
            if (node.Tag is HostBudgetTreeNodePayload payload &&
                !string.IsNullOrWhiteSpace(payload.MonthKey))
            {
                return node;
            }

            TreeNode? child =
                FindFirstBudgetNode(node.Nodes.Cast<TreeNode>());

            if (child is not null)
            {
                return child;
            }
        }

        return null;
    }

    private string ResolveActiveRuntimeDatabasePath()
    {
        HostBudgetDatabaseLocation location =
            _databaseSelectionService.GetActiveRuntimeDatabaseLocation();

        return location.DatabasePath;
    }

    private static string FormatSelectedNode(TreeNode node)
    {
        return
            $"Text: {node.Text}{Environment.NewLine}" +
            $"Name: {node.Name}{Environment.NewLine}" +
            $"Tag type: {node.Tag?.GetType().FullName ?? "(null)"}{Environment.NewLine}" +
            $"Tag:{Environment.NewLine}{Convert.ToString(node.Tag, CultureInfo.InvariantCulture)}";
    }

    private sealed record BudgetMonthQueryResult(
        DataTable Rows,
        string Sql,
        string Parameters);
}
