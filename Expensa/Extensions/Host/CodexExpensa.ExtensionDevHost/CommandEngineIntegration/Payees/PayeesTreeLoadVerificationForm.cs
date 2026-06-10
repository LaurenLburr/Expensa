using CodexExpensa.ExtensionDevHost.CommandEngineIntegration.CommonTree;
using CodexExpensa.ExtensionDevHost.CommandEngineIntegration.ExpensaLoader;
using CodexExpensa.ExtensionDevHost.CommandEngineIntegration.ExtensionManager;
using CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Templates;
using Microsoft.Data.Sqlite;
using System.Data;
using System.Globalization;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Payees;

public sealed partial class PayeesTreeLoadVerificationForm : TreeTestTemplate
{
    private const string AddinId = "PayeesAddin";
    private const int MaximumRows = 500;

    private readonly ExpensaAddinTreeViewLoaderService _loaderService = new();
    private readonly AddinRuntimeDatabasePathService _pathService = new();

    private string _databasePath = string.Empty;
    private bool _hasAutoLoaded;
    private AddinMemoryDatabaseSession? _databaseSession;

    public PayeesTreeLoadVerificationForm()
    {
        InitializeComponent();

        ConfigureTreeTestTemplate(
            "Payees Tree Test",
            "Loading Payees add-in runtime tree...");

        _databasePath = ResolveDefaultRuntimeDatabasePath();
        TestTreeView.AfterSelect += TestTreeView_AfterSelect;
    }

    protected override async void OnShown(EventArgs e)
    {
        base.OnShown(e);

        if (_hasAutoLoaded)
        {
            return;
        }

        _hasAutoLoaded = true;
        await LoadPayeesTreeAsync().ConfigureAwait(true);
    }

    private async Task LoadPayeesTreeAsync()
    {
        ClearTemplate();

        if (string.IsNullOrWhiteSpace(_databasePath) || !File.Exists(_databasePath))
        {
            SetNotes(
                "Payees add-in runtime database was not found." + Environment.NewLine + Environment.NewLine +
                "Expected add-in runtime path:" + Environment.NewLine +
                _pathService.GetRuntimeDatabaseLocation(AddinId).DatabasePath + Environment.NewLine + Environment.NewLine +
                "Use the Payees Database page to update data from Prod or Dev, then run the sandbox transaction seed script against the add-in runtime copy if test transactions are needed.");

            return;
        }

        SetNotes(
            "Loading Payees from add-in runtime database:" + Environment.NewLine +
            _databasePath);

        try
        {
            await _loaderService.LoadIntoTreeViewAsync(
                    TestTreeView,
                    new ExpensaAddinLoaderRequest
                    {
                        Kind = ExpensaAddinLoaderKind.Payees,
                        DatabasePath = _databasePath,
                        SearchText = string.Empty,
                        IncludeInactive = false,
                        MaximumRows = MaximumRows
                    },
                    expandAll: true)
                .ConfigureAwait(true);

            ReloadMemoryDatabase();
            SelectFirstPayeeNode();
        }
        catch (Exception exception)
        {
            ReleaseMemoryDatabase();
            SetGridData((object?)null);
            SetNotes(exception.ToString());

            MessageBox.Show(
                this,
                exception.Message,
                "Payees Tree Test",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }

    private void TestTreeView_AfterSelect(object? sender, TreeViewEventArgs e)
    {
        if (e.Node is null)
        {
            SetGridData((object?)null);
            return;
        }

        LoadTransactionsForSelectedPayee(e.Node);
    }

    private void LoadTransactionsForSelectedPayee(TreeNode node)
    {
        string payeeId = TryGetPayeeId(node);

        if (string.IsNullOrWhiteSpace(payeeId))
        {
            SetGridData((object?)null);
            SetNotes(
                "Selected node is not a Payee row." + Environment.NewLine + Environment.NewLine +
                FormatSelectedNode(node));
            return;
        }

        try
        {
            DataTable transactions = LoadTransactionsForPayee(GetMemoryConnection(), payeeId);

            SetGridData(transactions);
            SetNotes(
                $"Payee: {node.Text}{Environment.NewLine}" +
                $"PayeeId: {payeeId}{Environment.NewLine}" +
                "Database is loaded into memory from:" + Environment.NewLine +
                _databasePath + Environment.NewLine +
                "The source database file is closed after the memory load completes." + Environment.NewLine +
                $"Transactions shown: {transactions.Rows.Count}");
        }
        catch (Exception exception)
        {
            SetGridData((object?)null);
            SetNotes(exception.ToString());
        }
    }

    private SqliteConnection GetMemoryConnection()
    {
        if (_databaseSession is null)
        {
            ReloadMemoryDatabase();
        }

        return _databaseSession?.Connection
            ?? throw new InvalidOperationException("Payees add-in database is not loaded into memory.");
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

    private static DataTable LoadTransactionsForPayee(SqliteConnection connection, string payeeId)
    {
        ArgumentNullException.ThrowIfNull(connection);
        ArgumentException.ThrowIfNullOrWhiteSpace(payeeId);

        if (!TableExists(connection, "Txn"))
        {
            return CreateMessageTable("Txn table was not found in the selected sandbox database.");
        }

        using SqliteCommand command =
            connection.CreateCommand();

        command.CommandText =
            """
            SELECT
                t.[TransactionId],
                t.[StartDate],
                t.[Status],
                t.[Amount],
                t.[ConfirmationNumber],
                t.[Note],
                t.[AccountId],
                COALESCE(a.[AccountNickname], '') AS [AccountNickname],
                t.[PayeeId],
                COALESCE(p.[PayeeName], '') AS [PayeeName]
            FROM [Txn] t
            LEFT JOIN [Account] a
                ON a.[AccountId] = t.[AccountId]
            LEFT JOIN [Payee] p
                ON p.[PayeeId] = t.[PayeeId]
            WHERE t.[PayeeId] = @PayeeId
            ORDER BY
                date(t.[StartDate]) DESC,
                t.[TransactionId] DESC
            LIMIT 500;
            """;

        command.Parameters.AddWithValue("@PayeeId", payeeId);

        return LoadDataTable(command);
    }

    private static DataTable LoadDataTable(SqliteCommand command)
    {
        using SqliteDataReader reader =
            command.ExecuteReader();

        DataTable table = new();
        table.Load(reader);
        return table;
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

    private static DataTable CreateMessageTable(string message)
    {
        DataTable table = new();
        table.Columns.Add("Message", typeof(string));
        table.Rows.Add(message);
        return table;
    }

    private static string TryGetPayeeId(TreeNode node)
    {
        if (node.Tag is IAddinTreePayload payload)
        {
            string nodeId = payload.NodeId;

            if (nodeId.StartsWith("payee:", StringComparison.OrdinalIgnoreCase))
            {
                return nodeId["payee:".Length..];
            }

            if (string.Equals(payload.NodeType.ToString(), "Item", StringComparison.OrdinalIgnoreCase) &&
                nodeId.StartsWith("legacy-payee-", StringComparison.OrdinalIgnoreCase))
            {
                return nodeId;
            }
        }

        if (node.Name.StartsWith("payee:", StringComparison.OrdinalIgnoreCase))
        {
            return node.Name["payee:".Length..];
        }

        return string.Empty;
    }

    private void SelectFirstPayeeNode()
    {
        TreeNode? payeeNode =
            FindFirstPayeeNode(TestTreeView.Nodes.Cast<TreeNode>());

        if (payeeNode is null)
        {
            SetGridData((object?)null);
            SetNotes(
                "Payees tree loaded, but no payee node was found." + Environment.NewLine + Environment.NewLine +
                "Add-in DB:" + Environment.NewLine +
                _databasePath);
            return;
        }

        TestTreeView.SelectedNode = payeeNode;
        payeeNode.EnsureVisible();
    }

    private static TreeNode? FindFirstPayeeNode(IEnumerable<TreeNode> nodes)
    {
        foreach (TreeNode node in nodes)
        {
            if (!string.IsNullOrWhiteSpace(TryGetPayeeId(node)))
            {
                return node;
            }

            TreeNode? child = FindFirstPayeeNode(node.Nodes.Cast<TreeNode>());

            if (child is not null)
            {
                return child;
            }
        }

        return null;
    }

    private static string FormatSelectedNode(TreeNode node)
    {
        return
            $"Text: {node.Text}{Environment.NewLine}" +
            $"Name: {node.Name}{Environment.NewLine}" +
            $"Tag type: {node.Tag?.GetType().FullName ?? "(null)"}{Environment.NewLine}" +
            $"Tag:{Environment.NewLine}{Convert.ToString(node.Tag, CultureInfo.InvariantCulture)}";
    }

    private string ResolveDefaultRuntimeDatabasePath()
    {
        string activeRuntimePathFile =
            _pathService.GetActiveRuntimePathFile(AddinId);

        if (File.Exists(activeRuntimePathFile))
        {
            string activeDatabasePath = File.ReadAllText(activeRuntimePathFile).Trim();

            if (!string.IsNullOrWhiteSpace(activeDatabasePath) && File.Exists(activeDatabasePath))
            {
                return activeDatabasePath;
            }
        }

        return _pathService.GetRuntimeDatabaseLocation(AddinId).DatabasePath;
    }

}
