using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Windows.Forms;
using CodexExpensa.App.WinForms.Infrastructure;
using CodexExpensa.App.WinForms.Navigation;
using CodexExpensa.App.WinForms.UI.Accounts;
using CodexExpensa.App.WinForms.UI.Banks;
using CodexExpensa.App.WinForms.UI.Budgets;
using CodexExpensa.App.WinForms.UI.Diagnostics;
using CodexExpensa.App.WinForms.UI.Payees;
using CodexExpensa.App.WinForms.UI.Websites;
using CodexExpensa.Core.Abstractions;
using CodexExpensa.Core.Domain.Accounts;
using CodexExpensa.Core.Domain.Banks;
using CodexExpensa.Core.Domain.Payees;
using CodexExpensa.Core.Domain.Transactions;
using CodexExpensa.Core.Events;
using Microsoft.Data.Sqlite;

namespace CodexExpensa.App.WinForms.UI;

public partial class MainForm : Form
{
    private const string RootAccounts = "root:accounts";
    private const string RootBanks = "root:banks";
    private const string RootPayees = "root:payees";
    private const string RootWebsites = "root:websites";
    private const string RootBudgets = "root:budgets";

    private const string NodeAllAccounts = "accounts:all";
    private const string NodeAllBanks = "banks:all";
    private const string NodeAllPayees = "payees:all";
    private const string NodeAllWebsites = "websites:all";
    private const string NodeBudgets = "budgets:landing";
    private const string NodeBudgetTemplate = "budgets:template";

    private readonly IDatabaseSession _dbSession;
    private readonly IAccountRepository _accounts;
    private readonly IBankRepository _banks;
    private readonly ITransactionRepository _transactions;
    private readonly IPayeeRepository _payees;
    private readonly Func<Form> _createDbStatusForm;
    private readonly ITableChangePublisher _tableChangePublisher;
    private readonly ICredentialStore _creds;
    private readonly ScreenHost _screenHost;
    private readonly System.Windows.Forms.Timer _autoRefreshTimer;

    private IReadOnlyList<Account> _accountCache = Array.Empty<Account>();
    private IReadOnlyList<Bank> _bankCache = Array.Empty<Bank>();
    private IReadOnlyList<Payee> _payeeCache = Array.Empty<Payee>();

    private readonly HashSet<string> _pendingChangedTables = new(StringComparer.OrdinalIgnoreCase);

    public MainForm(
        IDatabaseSession dbSession,
        IAccountRepository accounts,
        IBankRepository banks,
        ITransactionRepository transactions,
        IPayeeRepository payees,
        Func<Form> createDbStatusForm,
        ITableChangePublisher tableChangePublisher)
    {
        _dbSession = dbSession ?? throw new ArgumentNullException(nameof(dbSession));
        _accounts = accounts ?? throw new ArgumentNullException(nameof(accounts));
        _banks = banks ?? throw new ArgumentNullException(nameof(banks));
        _transactions = transactions ?? throw new ArgumentNullException(nameof(transactions));
        _payees = payees ?? throw new ArgumentNullException(nameof(payees));
        _createDbStatusForm = createDbStatusForm ?? throw new ArgumentNullException(nameof(createDbStatusForm));
        _tableChangePublisher = tableChangePublisher ?? throw new ArgumentNullException(nameof(tableChangePublisher));
        _creds = new WindowsCredentialStore();

        InitializeComponent();

        _screenHost = new ScreenHost(panelHost);
        _autoRefreshTimer = new System.Windows.Forms.Timer
        {
            Interval = 250
        };
        _autoRefreshTimer.Tick += AutoRefreshTimer_Tick;

        WireEvents();
        WireAutoRefresh();
        RefreshAll(preserveTreeState: false);
    }

    private void WireEvents()
    {
        treeNav.AfterSelect -= TreeNav_AfterSelect;
        treeNav.AfterSelect += TreeNav_AfterSelect;

        menuFileSave.Click -= MenuFileSave_Click;
        menuFileSave.Click += MenuFileSave_Click;

        menuFileExit.Click -= MenuFileExit_Click;
        menuFileExit.Click += MenuFileExit_Click;

        menuViewRefresh.Click -= MenuViewRefresh_Click;
        menuViewRefresh.Click += MenuViewRefresh_Click;

        menuToolsDbStatus.Click -= MenuToolsDbStatus_Click;
        menuToolsDbStatus.Click += MenuToolsDbStatus_Click;
    }

    private void WireAutoRefresh()
    {
        _tableChangePublisher.TableChanged += (_, args) =>
        {
            string tableName = string.IsNullOrWhiteSpace(args.TableName)
                ? string.Empty
                : args.TableName;

            if (!ShouldAutoRefreshForTable(tableName))
            {
                return;
            }

            ScheduleAutoRefresh(tableName);
        };
    }

    private static bool ShouldAutoRefreshForTable(string? tableName)
    {
        if (string.IsNullOrWhiteSpace(tableName))
        {
            return true;
        }

        return tableName.Equals("Account", StringComparison.OrdinalIgnoreCase)
            || tableName.Equals("TagAssignment", StringComparison.OrdinalIgnoreCase)
            || tableName.Equals("Tag", StringComparison.OrdinalIgnoreCase)
            || tableName.Equals("Bank", StringComparison.OrdinalIgnoreCase)
            || tableName.Equals("Payee", StringComparison.OrdinalIgnoreCase)
            || tableName.Equals("Website", StringComparison.OrdinalIgnoreCase)
            || tableName.Equals("WebsiteCredential", StringComparison.OrdinalIgnoreCase)
            || tableName.Equals("WebQuestion", StringComparison.OrdinalIgnoreCase)
            || tableName.Equals("Txn", StringComparison.OrdinalIgnoreCase)
            || tableName.Equals("BudgetMonth", StringComparison.OrdinalIgnoreCase)
            || tableName.Equals("BudgetMonthItem", StringComparison.OrdinalIgnoreCase);
    }

    private void ScheduleAutoRefresh(string tableName)
    {
        if (IsDisposed)
        {
            return;
        }

        if (InvokeRequired)
        {
            BeginInvoke(new Action<string>(ScheduleAutoRefresh), tableName);
            return;
        }

        if (!string.IsNullOrWhiteSpace(tableName))
        {
            _pendingChangedTables.Add(tableName);
        }

        _autoRefreshTimer.Stop();
        _autoRefreshTimer.Start();
    }

    private void AutoRefreshTimer_Tick(object? sender, EventArgs e)
    {
        _autoRefreshTimer.Stop();

        try
        {
            ApplyPendingAutoRefresh();
        }
        catch (Exception ex)
        {
            _pendingChangedTables.Clear();
            statusText.Text = "Auto-refresh failed.";
            MessageBox.Show(this, ex.ToString(), "Auto-refresh failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void ApplyPendingAutoRefresh()
    {
        HashSet<string> changedTables = new(_pendingChangedTables, StringComparer.OrdinalIgnoreCase);
        _pendingChangedTables.Clear();

        if (changedTables.Count == 0)
        {
            RefreshAll(preserveTreeState: true);
            statusText.Text = "Auto-refreshed.";
            return;
        }

        HashSet<string> rootsToRefresh = GetRootsToRefresh(changedTables);
        RefreshCachesForTables(changedTables);

        if (rootsToRefresh.Count == 0 || rootsToRefresh.Count >= 5)
        {
            RefreshAll(preserveTreeState: true);
        }
        else
        {
            RefreshTreeRoots(rootsToRefresh);
        }

        statusText.Text = $"Auto-refreshed ({string.Join(", ", changedTables.OrderBy(x => x, StringComparer.OrdinalIgnoreCase))}).";
        UpdateDbModeText();
    }

    private static HashSet<string> GetRootsToRefresh(HashSet<string> changedTables)
    {
        HashSet<string> roots = new(StringComparer.OrdinalIgnoreCase);

        foreach (string table in changedTables)
        {
            if (table.Equals("Account", StringComparison.OrdinalIgnoreCase)
                || table.Equals("TagAssignment", StringComparison.OrdinalIgnoreCase)
                || table.Equals("Tag", StringComparison.OrdinalIgnoreCase))
            {
                roots.Add(RootAccounts);
                roots.Add(RootWebsites);
                continue;
            }

            if (table.Equals("Bank", StringComparison.OrdinalIgnoreCase))
            {
                roots.Add(RootAccounts);
                roots.Add(RootBanks);
                roots.Add(RootWebsites);
                continue;
            }

            if (table.Equals("Payee", StringComparison.OrdinalIgnoreCase))
            {
                roots.Add(RootPayees);
                continue;
            }

            if (table.Equals("Website", StringComparison.OrdinalIgnoreCase)
                || table.Equals("WebsiteCredential", StringComparison.OrdinalIgnoreCase)
                || table.Equals("WebQuestion", StringComparison.OrdinalIgnoreCase))
            {
                roots.Add(RootWebsites);
                continue;
            }

            if (table.Equals("Txn", StringComparison.OrdinalIgnoreCase)
                || table.Equals("BudgetMonth", StringComparison.OrdinalIgnoreCase)
                || table.Equals("BudgetMonthItem", StringComparison.OrdinalIgnoreCase))
            {
                roots.Add(RootBudgets);
            }
        }

        return roots;
    }

    private void RefreshCachesForTables(HashSet<string> changedTables)
    {
        bool refreshAccounts = changedTables.Contains("Account")
            || changedTables.Contains("TagAssignment")
            || changedTables.Contains("Tag")
            || changedTables.Contains("Bank");

        bool refreshBanks = changedTables.Contains("Bank");
        bool refreshPayees = changedTables.Contains("Payee");

        if (refreshAccounts)
        {
            _accountCache = _accounts.GetAll()
                .OrderBy(a => a.BankName, StringComparer.OrdinalIgnoreCase)
                .ThenBy(a => a.SortIndex)
                .ThenBy(a => a.AccountNickname, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        if (refreshBanks)
        {
            _bankCache = _banks.GetAll()
                .OrderBy(b => b.BankName, StringComparer.OrdinalIgnoreCase)
                .ThenBy(b => b.RoutingNumber, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        if (refreshPayees)
        {
            _payeeCache = _payees.GetAll()
                .OrderBy(p => p.PayeeName, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }
    }

    private void MenuFileSave_Click(object? sender, EventArgs e)
    {
        try
        {
            _dbSession.Save();
            statusText.Text = "Saved.";
            UpdateDbModeText();
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.ToString(), "Save failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void MenuFileExit_Click(object? sender, EventArgs e)
    {
        Close();
    }

    private void MenuViewRefresh_Click(object? sender, EventArgs e)
    {
        RefreshAll(preserveTreeState: true);
    }

    private void MenuToolsDbStatus_Click(object? sender, EventArgs e)
    {
        using Form form = _createDbStatusForm();
        form.ShowDialog(this);
    }

    private void RefreshAll(bool preserveTreeState)
    {
        RefreshAllCaches();
        RebuildNavigationTree(preserveTreeState);
        UpdateDbModeText();
        statusText.Text = "Ready";
    }

    private void RefreshAllCaches()
    {
        _accountCache = _accounts.GetAll()
            .OrderBy(a => a.BankName, StringComparer.OrdinalIgnoreCase)
            .ThenBy(a => a.SortIndex)
            .ThenBy(a => a.AccountNickname, StringComparer.OrdinalIgnoreCase)
            .ToList();

        _bankCache = _banks.GetAll()
            .OrderBy(b => b.BankName, StringComparer.OrdinalIgnoreCase)
            .ThenBy(b => b.RoutingNumber, StringComparer.OrdinalIgnoreCase)
            .ToList();

        _payeeCache = _payees.GetAll()
            .OrderBy(p => p.PayeeName, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private void RebuildNavigationTree(bool preserveTreeState)
    {
        HashSet<string> expandedKeys = preserveTreeState
            ? CaptureExpandedNodeKeys(treeNav.Nodes)
            : new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        string? selectedKey = preserveTreeState
            ? treeNav.SelectedNode?.Tag as string
            : null;

        IReadOnlyList<TreeNode> roots = BuildTreeNodes();

        treeNav.BeginUpdate();
        try
        {
            treeNav.Nodes.Clear();

            foreach (TreeNode root in roots)
            {
                treeNav.Nodes.Add(root);
            }
        }
        finally
        {
            treeNav.EndUpdate();
        }

        if (preserveTreeState)
        {
            RestoreExpandedNodeKeys(treeNav.Nodes, expandedKeys);
            RestoreSelectedNode(selectedKey);
        }
        else if (treeNav.Nodes.Count > 0)
        {
            treeNav.SelectedNode = treeNav.Nodes[0];
        }
    }

    private void RefreshTreeRoots(HashSet<string> rootKeys)
    {
        HashSet<string> expandedKeys = CaptureExpandedNodeKeys(treeNav.Nodes);
        string? selectedKey = treeNav.SelectedNode?.Tag as string;

        treeNav.BeginUpdate();
        try
        {
            foreach (string rootKey in rootKeys)
            {
                ReplaceRootNode(rootKey);
            }
        }
        finally
        {
            treeNav.EndUpdate();
        }

        RestoreExpandedNodeKeys(treeNav.Nodes, expandedKeys);
        RestoreSelectedNode(selectedKey);
    }

    private void ReplaceRootNode(string rootKey)
    {
        TreeNode replacement = BuildRootNode(rootKey);
        TreeNode? existing = FindRootNode(rootKey);

        if (existing is null)
        {
            treeNav.Nodes.Add(replacement);
            return;
        }

        int index = existing.Index;
        treeNav.Nodes.RemoveAt(index);
        treeNav.Nodes.Insert(index, replacement);
    }

    private TreeNode? FindRootNode(string rootKey)
    {
        foreach (TreeNode node in treeNav.Nodes)
        {
            if (string.Equals(node.Tag as string, rootKey, StringComparison.OrdinalIgnoreCase))
            {
                return node;
            }
        }

        return null;
    }

    private IReadOnlyList<TreeNode> BuildTreeNodes()
    {
        return new[]
        {
            BuildAccountsRootNode(),
            BuildBanksRootNode(),
            BuildPayeesRootNode(),
            BuildWebsitesRootNode(),
            BuildBudgetsRootNode()
        };
    }

    private TreeNode BuildRootNode(string rootKey)
    {
        if (string.Equals(rootKey, RootAccounts, StringComparison.OrdinalIgnoreCase))
        {
            return BuildAccountsRootNode();
        }

        if (string.Equals(rootKey, RootBanks, StringComparison.OrdinalIgnoreCase))
        {
            return BuildBanksRootNode();
        }

        if (string.Equals(rootKey, RootPayees, StringComparison.OrdinalIgnoreCase))
        {
            return BuildPayeesRootNode();
        }

        if (string.Equals(rootKey, RootWebsites, StringComparison.OrdinalIgnoreCase))
        {
            return BuildWebsitesRootNode();
        }

        if (string.Equals(rootKey, RootBudgets, StringComparison.OrdinalIgnoreCase))
        {
            return BuildBudgetsRootNode();
        }

        throw new InvalidOperationException($"Unsupported root key '{rootKey}'.");
    }

    private TreeNode BuildAccountsRootNode()
    {
        TreeNode accountsRoot = CreateNode(RootAccounts, "Accounts");
        accountsRoot.Nodes.Add(CreateNode(NodeAllAccounts, "All Accounts"));
        accountsRoot.Nodes.Add(BuildAccountsByBankNode());
        accountsRoot.Nodes.Add(BuildAccountsByTagNode());
        return accountsRoot;
    }

    private TreeNode BuildBanksRootNode()
    {
        TreeNode banksRoot = CreateNode(RootBanks, "Banks");
        banksRoot.Nodes.Add(CreateNode(NodeAllBanks, "All Banks"));

        foreach (Bank bank in _bankCache)
        {
            banksRoot.Nodes.Add(CreateNode($"bank:{bank.BankId}", $"{bank.BankName} ({bank.RoutingNumber})"));
        }

        return banksRoot;
    }

    private TreeNode BuildPayeesRootNode()
    {
        TreeNode payeesRoot = CreateNode(RootPayees, "Payees");
        payeesRoot.Nodes.Add(CreateNode(NodeAllPayees, "All Payees"));

        foreach (Payee payee in _payeeCache)
        {
            payeesRoot.Nodes.Add(CreateNode($"payee:{payee.PayeeId}", payee.PayeeName));
        }

        return payeesRoot;
    }

    private TreeNode BuildWebsitesRootNode()
    {
        TreeNode websitesRoot = CreateNode(RootWebsites, "Websites");
        TreeNode allWebsitesNode = CreateNode(NodeAllWebsites, "All Websites");

        foreach ((string id, string name, string url, string tags) in LoadWebsiteRows())
        {
            string displayText = string.IsNullOrWhiteSpace(name) ? id : name;
            allWebsitesNode.Nodes.Add(CreateNode($"website:{id}", displayText));
        }

        websitesRoot.Nodes.Add(allWebsitesNode);
        return websitesRoot;
    }

    private TreeNode BuildBudgetsRootNode()
    {
        TreeNode budgetsRoot = CreateNode(RootBudgets, "Budgets");
        budgetsRoot.Nodes.Add(CreateNode(NodeBudgets, "Overview"));
        budgetsRoot.Nodes.Add(CreateNode(NodeBudgetTemplate, "Template"));

        int currentYear = DateTime.Now.Year;
        int currentMonth = DateTime.Now.Month;

        TreeNode currentYearNode = CreateNode($"budgetyear:{currentYear}", currentYear.ToString());
        for (int month = 1; month <= currentMonth; month++)
        {
            string text = new DateTime(currentYear, month, 1).ToString("MMMM yyyy");
            currentYearNode.Nodes.Add(CreateNode($"budgetmonth:{currentYear:D4}:{month:D2}", text));
        }

        budgetsRoot.Nodes.Add(currentYearNode);

        return budgetsRoot;
    }

    private TreeNode BuildAccountsByBankNode()
    {
        TreeNode byBank = CreateNode("accounts:bybank", "By Bank");

        foreach (Bank bank in _bankCache)
        {
            List<Account> accountsForBank = _accountCache
                .Where(a => string.Equals(a.BankId, bank.BankId, StringComparison.OrdinalIgnoreCase))
                .OrderBy(a => a.SortIndex)
                .ThenBy(a => a.AccountNickname, StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (accountsForBank.Count == 0)
            {
                continue;
            }

            TreeNode bankNode = CreateNode($"accounts:bank:{bank.BankId}", $"{bank.BankName} ({bank.RoutingNumber})");

            foreach (Account account in accountsForBank)
            {
                bankNode.Nodes.Add(CreateAccountNode(account));
            }

            byBank.Nodes.Add(bankNode);
        }

        return byBank;
    }

    private TreeNode BuildAccountsByTagNode()
    {
        TreeNode byTag = CreateNode("accounts:bytag", "By Tag");

        Dictionary<string, List<Account>> tagMap = BuildTagMap();

        foreach ((string tagName, List<Account> accounts) in tagMap.OrderBy(x => x.Key, StringComparer.OrdinalIgnoreCase))
        {
            TreeNode tagNode = CreateNode($"tag:{tagName}", tagName);

            foreach (Account account in accounts
                         .OrderBy(a => a.BankName, StringComparer.OrdinalIgnoreCase)
                         .ThenBy(a => a.SortIndex)
                         .ThenBy(a => a.AccountNickname, StringComparer.OrdinalIgnoreCase))
            {
                tagNode.Nodes.Add(CreateAccountNode(account));
            }

            byTag.Nodes.Add(tagNode);
        }

        if (byTag.Nodes.Count == 0)
        {
            byTag.Nodes.Add(CreateNode("tag:none", "(No tags assigned)"));
        }

        return byTag;
    }

    private Dictionary<string, List<Account>> BuildTagMap()
    {
        Dictionary<string, List<Account>> result = new(StringComparer.OrdinalIgnoreCase);

        foreach (Account account in _accountCache)
        {
            foreach (string tagName in LoadTagNamesForAccount(account.AccountId))
            {
                if (!result.TryGetValue(tagName, out List<Account>? accounts))
                {
                    accounts = new List<Account>();
                    result[tagName] = accounts;
                }

                if (!accounts.Any(a => string.Equals(a.AccountId, account.AccountId, StringComparison.OrdinalIgnoreCase)))
                {
                    accounts.Add(account);
                }
            }
        }

        return result;
    }

    private IReadOnlyList<string> LoadTagNamesForAccount(string accountId)
    {
        if (string.IsNullOrWhiteSpace(accountId))
        {
            return Array.Empty<string>();
        }

        try
        {
            IEnumerable<DbParameter> parameters =
            [
                new SqliteParameter("@EntityType", "Account"),
                new SqliteParameter("@EntityId", accountId)
            ];

            DataTable table = _dbSession.QueryDataTable("Tag.GetByEntity", parameters);

            List<string> names = new();

            foreach (DataRow row in table.Rows)
            {
                string? tagName = row.Table.Columns.Contains("TagName")
                    ? row["TagName"]?.ToString()
                    : null;

                if (string.IsNullOrWhiteSpace(tagName))
                {
                    continue;
                }

                if (!names.Contains(tagName, StringComparer.OrdinalIgnoreCase))
                {
                    names.Add(tagName);
                }
            }

            return names;
        }
        catch
        {
            return Array.Empty<string>();
        }
    }

    private string LoadTagSummary(string entityType, string entityId)
    {
        if (string.IsNullOrWhiteSpace(entityType) || string.IsNullOrWhiteSpace(entityId))
        {
            return string.Empty;
        }

        try
        {
            IEnumerable<DbParameter> parameters =
            [
                new SqliteParameter("@EntityType", entityType),
                new SqliteParameter("@EntityId", entityId)
            ];

            DataTable table = _dbSession.QueryDataTable("Tag.GetByEntity", parameters);

            List<string> names = new();

            foreach (DataRow row in table.Rows)
            {
                string? tagName = row.Table.Columns.Contains("TagName")
                    ? row["TagName"]?.ToString()
                    : null;

                if (string.IsNullOrWhiteSpace(tagName))
                {
                    continue;
                }

                if (!names.Contains(tagName, StringComparer.OrdinalIgnoreCase))
                {
                    names.Add(tagName);
                }
            }

            return string.Join(", ", names);
        }
        catch
        {
            return string.Empty;
        }
    }

    private IReadOnlyList<(string Id, string Name, string Url, string Tags)> LoadWebsiteRows()
    {
        try
        {
            DataTable table = _dbSession.QueryDataTable("Website.GetAllActive");

            List<(string Id, string Name, string Url, string Tags)> rows = new();

            foreach (DataRow row in table.Rows)
            {
                string id = row.Table.Columns.Contains("WebsiteId")
                    ? row["WebsiteId"]?.ToString() ?? string.Empty
                    : string.Empty;

                string name = row.Table.Columns.Contains("Name")
                    ? row["Name"]?.ToString() ?? string.Empty
                    : string.Empty;

                string url = row.Table.Columns.Contains("Url")
                    ? row["Url"]?.ToString() ?? string.Empty
                    : string.Empty;

                if (string.IsNullOrWhiteSpace(id))
                {
                    continue;
                }

                string tags = LoadTagSummary("Website", id);
                rows.Add((id, name, url, tags));
            }

            return rows;
        }
        catch
        {
            return Array.Empty<(string Id, string Name, string Url, string Tags)>();
        }
    }

    private (string Id, string Name, string Url, string Notes)? LoadWebsiteById(string websiteId)
    {
        if (string.IsNullOrWhiteSpace(websiteId))
        {
            return null;
        }

        try
        {
            IEnumerable<DbParameter> parameters =
            [
                new SqliteParameter("@WebsiteId", websiteId)
            ];

            DataTable table = _dbSession.QueryDataTable("Website.GetById", parameters);
            if (table.Rows.Count == 0)
            {
                return null;
            }

            DataRow row = table.Rows[0];

            string id = row.Table.Columns.Contains("WebsiteId")
                ? row["WebsiteId"]?.ToString() ?? string.Empty
                : string.Empty;

            if (string.IsNullOrWhiteSpace(id))
            {
                return null;
            }

            string name = row.Table.Columns.Contains("Name")
                ? row["Name"]?.ToString() ?? string.Empty
                : string.Empty;

            string url = row.Table.Columns.Contains("Url")
                ? row["Url"]?.ToString() ?? string.Empty
                : string.Empty;

            string notes = row.Table.Columns.Contains("Notes")
                ? row["Notes"]?.ToString() ?? string.Empty
                : string.Empty;

            return (id, name, url, notes);
        }
        catch
        {
            return null;
        }
    }

    private TreeNode CreateAccountNode(Account account)
    {
        string displayName = $"{account.AccountNickname} - {Last4(account.AccountNumber)}";
        return CreateNode($"account:{account.AccountId}", displayName);
    }

    private static string Last4(string? accountNumber)
    {
        if (string.IsNullOrWhiteSpace(accountNumber))
        {
            return "????";
        }

        string cleaned = accountNumber.Replace(" ", string.Empty).Replace("-", string.Empty);
        return cleaned.Length <= 4 ? cleaned : cleaned[^4..];
    }

    private static TreeNode CreateNode(string id, string text)
    {
        return new TreeNode(text)
        {
            Name = id,
            Tag = id
        };
    }

    private void TreeNav_AfterSelect(object? sender, TreeViewEventArgs e)
    {
        if (e.Node?.Tag is not string id)
        {
            return;
        }

        switch (id)
        {
            case NodeAllAccounts:
                ShowAccountsLanding();
                return;

            case NodeAllBanks:
                ShowBanksLanding();
                return;

            case NodeAllPayees:
                ShowPayeesLanding();
                return;

            case NodeAllWebsites:
                ShowWebsitesLanding();
                return;

            case NodeBudgets:
                ShowBudgetsLanding();
                return;

            case NodeBudgetTemplate:
                ShowBudgetTemplate();
                return;
        }

        if (id.StartsWith("account:", StringComparison.OrdinalIgnoreCase))
        {
            ShowAccountDetails(id["account:".Length..]);
            return;
        }

        if (id.StartsWith("bank:", StringComparison.OrdinalIgnoreCase))
        {
            ShowBankDetails(id["bank:".Length..]);
            return;
        }

        if (id.StartsWith("payee:", StringComparison.OrdinalIgnoreCase))
        {
            ShowPayeeDetails(id["payee:".Length..]);
            return;
        }

        if (id.StartsWith("website:", StringComparison.OrdinalIgnoreCase))
        {
            ShowWebsiteDetails(id["website:".Length..]);
            return;
        }

        if (id.StartsWith("budgetyear:", StringComparison.OrdinalIgnoreCase))
        {
            if (int.TryParse(id["budgetyear:".Length..], out int year))
            {
                ShowBudgetYear(year);
            }

            return;
        }

        if (id.StartsWith("budgetmonth:", StringComparison.OrdinalIgnoreCase))
        {
            string[] parts = id.Split(':');
            if (parts.Length == 3 &&
                int.TryParse(parts[1], out int year) &&
                int.TryParse(parts[2], out int month))
            {
                ShowBudgetMonth(year, month);
            }
        }
    }

    private void ShowAccountsLanding()
    {
        const string screenId = "screen:accounts:list";
        _screenHost.Show(
            screenId,
            () =>
            {
                AccountsListForm form = new(ShowAccountDetails);
                form.SetAccounts(_accountCache);
                return form;
            },
            singleInstance: false);
    }

    private void ShowBanksLanding()
    {
        const string screenId = "screen:banks:list";
        _screenHost.Show(
            screenId,
            () =>
            {
                BanksLandingForm form = new(ShowBankDetails);
                form.SetBanks(_bankCache);
                return form;
            },
            singleInstance: false);
    }

    private void ShowPayeesLanding()
    {
        const string screenId = "screen:payees:list";
        _screenHost.Show(
            screenId,
            () =>
            {
                PayeesLandingForm form = new(_dbSession, ShowPayeeDetails);
                form.SetPayees(_payeeCache);
                return form;
            },
            singleInstance: false);
    }

    private void ShowWebsitesLanding()
    {
        const string screenId = "screen:websites:list";
        _screenHost.Show(
            screenId,
            () =>
            {
                WebsitesLandingForm form = new(ShowWebsiteDetails);
                form.SetRows(LoadWebsiteRows());
                return form;
            },
            singleInstance: false);
    }

    private void ShowWebsiteDetails(string websiteId)
    {
        if (string.IsNullOrWhiteSpace(websiteId))
        {
            return;
        }

        (string Id, string Name, string Url, string Notes)? website = LoadWebsiteById(websiteId);
        if (website is null)
        {
            MessageBox.Show(this, $"Website '{websiteId}' was not found.", "Website", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        string screenId = $"screen:website:{website.Value.Id}";
        _screenHost.Show(
            screenId,
            () =>
            {
                WebsiteDetailsForm form = new(
                    db: _dbSession,
                    onSaved: () =>
                    {
                        RefreshTreeRoots(new HashSet<string>(new[] { RootWebsites }, StringComparer.OrdinalIgnoreCase));
                        RestoreSelectedNode($"website:{website.Value.Id}");
                    });

                form.LoadWebsite(
                    website.Value.Id,
                    website.Value.Name,
                    website.Value.Url,
                    website.Value.Notes);

                return form;
            },
            singleInstance: false);
    }

    private void ShowBudgetsLanding()
    {
        const string screenId = "screen:budgets:landing";
        _screenHost.Show(screenId, () => new BudgetsLandingForm(), singleInstance: true);
    }

    private void ShowBudgetTemplate()
    {
        const string screenId = "screen:budgets:template";
        _screenHost.Show(screenId, () => new BudgetTemplateForm(_payees), singleInstance: false);
    }

    private void ShowBudgetYear(int year)
    {
        string screenId = $"screen:budgetyear:{year}";
        _screenHost.Show(screenId, () => new BudgetYearForm(_dbSession, year), singleInstance: false);
    }

    private void ShowBudgetMonth(int year, int month)
    {
        string screenId = $"screen:budgetmonth:{year:D4}:{month:D2}";
        _screenHost.Show(screenId, () => new BudgetMonthForm(_dbSession, _transactions, year, month), singleInstance: false);
    }

    private void ShowAccountDetails(string accountId)
    {
        if (string.IsNullOrWhiteSpace(accountId))
        {
            return;
        }

        Account? account = _accountCache.FirstOrDefault(a =>
            string.Equals(a.AccountId, accountId, StringComparison.OrdinalIgnoreCase));

        if (account is null)
        {
            MessageBox.Show(this, $"Account '{accountId}' was not found.", "Account", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        string screenId = $"screen:account:{account.AccountId}";
        _screenHost.Show(
            screenId,
            () =>
            {
                AccountDetailsForm form = new(
                    db: _dbSession,
                    repo: _accounts,
                    banks: _bankCache,
                    creds: _creds,
                    txns: _transactions,
                    payees: _payees,
                    onSaved: () =>
                    {
                        RefreshAllCaches();
                        RefreshTreeRoots(
                            new HashSet<string>(new[] { RootAccounts, RootBanks }, StringComparer.OrdinalIgnoreCase));
                        RestoreSelectedNode($"account:{account.AccountId}");
                    });

                form.LoadAccount(account);
                return form;
            },
            singleInstance: false);
    }

    private void ShowBankDetails(string bankId)
    {
        if (string.IsNullOrWhiteSpace(bankId))
        {
            return;
        }

        Bank? bank = _bankCache.FirstOrDefault(b =>
            string.Equals(b.BankId, bankId, StringComparison.OrdinalIgnoreCase));

        if (bank is null)
        {
            MessageBox.Show(this, $"Bank '{bankId}' was not found.", "Bank", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        string screenId = $"screen:bank:{bank.BankId}";
        _screenHost.Show(
            screenId,
            () =>
            {
                BankDetailsForm form = new(
                    repo: _banks,
                    credentialStore: _creds,
                    onSaved: () =>
                    {
                        RefreshAllCaches();
                        RefreshTreeRoots(
                            new HashSet<string>(new[] { RootAccounts, RootBanks }, StringComparer.OrdinalIgnoreCase));
                        RestoreSelectedNode($"bank:{bank.BankId}");
                    });

                form.LoadBank(bank);
                return form;
            },
            singleInstance: false);
    }

    private void ShowPayeeDetails(string payeeId)
    {
        if (string.IsNullOrWhiteSpace(payeeId))
        {
            return;
        }

        Payee? payee = _payeeCache.FirstOrDefault(p =>
            string.Equals(p.PayeeId, payeeId, StringComparison.OrdinalIgnoreCase));

        if (payee is null)
        {
            MessageBox.Show(this, $"Payee '{payeeId}' was not found.", "Payee", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        string screenId = $"screen:payee:{payee.PayeeId}";
        _screenHost.Show(
            screenId,
            () =>
            {
                PayeeDetailsForm form = new(
                    repo: _payees,
                    onSaved: () =>
                    {
                        RefreshAllCaches();
                        RefreshTreeRoots(
                            new HashSet<string>(new[] { RootPayees }, StringComparer.OrdinalIgnoreCase));
                        RestoreSelectedNode($"payee:{payee.PayeeId}");
                    });

                form.LoadPayee(payee);
                return form;
            },
            singleInstance: false);
    }

    private HashSet<string> CaptureExpandedNodeKeys(TreeNodeCollection nodes)
    {
        HashSet<string> result = new(StringComparer.OrdinalIgnoreCase);

        foreach (TreeNode node in nodes)
        {
            CaptureExpandedNodeKeysRecursive(node, result);
        }

        return result;
    }

    private void CaptureExpandedNodeKeysRecursive(TreeNode node, HashSet<string> expandedKeys)
    {
        if (node.Tag is string key && node.IsExpanded)
        {
            expandedKeys.Add(key);
        }

        foreach (TreeNode child in node.Nodes)
        {
            CaptureExpandedNodeKeysRecursive(child, expandedKeys);
        }
    }

    private void RestoreExpandedNodeKeys(TreeNodeCollection nodes, HashSet<string> expandedKeys)
    {
        foreach (TreeNode node in nodes)
        {
            RestoreExpandedNodeKeysRecursive(node, expandedKeys);
        }
    }

    private void RestoreExpandedNodeKeysRecursive(TreeNode node, HashSet<string> expandedKeys)
    {
        if (node.Tag is string key && expandedKeys.Contains(key))
        {
            node.Expand();
        }

        foreach (TreeNode child in node.Nodes)
        {
            RestoreExpandedNodeKeysRecursive(child, expandedKeys);
        }
    }

    private void RestoreSelectedNode(string? selectedKey)
    {
        if (string.IsNullOrWhiteSpace(selectedKey))
        {
            return;
        }

        TreeNode? node = FindNodeByKey(treeNav.Nodes, selectedKey);
        if (node is null)
        {
            return;
        }

        treeNav.SelectedNode = node;
        node.EnsureVisible();
    }

    private TreeNode? FindNodeByKey(TreeNodeCollection nodes, string key)
    {
        foreach (TreeNode node in nodes)
        {
            if (string.Equals(node.Tag as string, key, StringComparison.OrdinalIgnoreCase))
            {
                return node;
            }

            TreeNode? childMatch = FindNodeByKey(node.Nodes, key);
            if (childMatch is not null)
            {
                return childMatch;
            }
        }

        return null;
    }

    private void UpdateDbModeText()
    {
        string mode = _dbSession.IsInMemory ? "Memory" : "File";
        string path = string.IsNullOrWhiteSpace(_dbSession.PersistedFilePath)
            ? string.Empty
            : $" - {_dbSession.PersistedFilePath}";

        statusDbMode.Text = $"DB: {mode}{path}";
    }

    private void diagnosticsToolStripMenuItem_Click(object? sender, EventArgs e)
    {
        using DatabaseDiagnosticsForm form = new(_dbSession);
        form.ShowDialog(this);
    }

    private void queryCatalogToolStripMenuItem_Click(object? sender, EventArgs e)
    {
        using QueryCatalogForm form = new(_dbSession);
        form.ShowDialog(this);
    }

    private void budgetMonthsToolStripMenuItem_Click(object? sender, EventArgs e)
    {
        ShowBudgetsLanding();
        RestoreSelectedNode(NodeBudgets);
    }

    private void menuHelpAbout_Click_1(object? sender, EventArgs e)
    {
        MessageBox.Show(
            this,
            "CodexExpensa\n\nWinForms desktop budget and account manager.",
            "About CodexExpensa",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        _autoRefreshTimer.Stop();
        _autoRefreshTimer.Dispose();
        _screenHost.Dispose();

        base.OnFormClosed(e);
    }
}
