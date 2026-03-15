using CodexExpensa.App.WinForms.Infrastructure;
using CodexExpensa.App.WinForms.UI.Accounts;
using CodexExpensa.App.WinForms.UI.Banks;
using CodexExpensa.App.WinForms.UI.Budgets;
using CodexExpensa.App.WinForms.UI.Diagnostics;
using CodexExpensa.App.WinForms.UI.Payees;
using CodexExpensa.Core.Abstractions;
using CodexExpensa.Core.Domain.Accounts;
using CodexExpensa.Core.Domain.Banks;
using CodexExpensa.Core.Domain.Payees;
using CodexExpensa.Core.Domain.Transactions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace CodexExpensa.App.WinForms.UI;

public partial class MainForm : Form
{
    private readonly IDatabaseSession _dbSession;
    private readonly IAccountRepository _accounts;
    private readonly IBankRepository _banks;
    private readonly ITransactionRepository _transactions;
    private readonly IPayeeRepository _payees;
    private readonly Func<Form> _createDbStatusForm;

    private readonly ICredentialStore _creds = new WindowsCredentialStore();

    private IReadOnlyList<Account> _accountCache = Array.Empty<Account>();
    private IReadOnlyList<Bank> _bankCache = Array.Empty<Bank>();
    private IReadOnlyList<Payee> _payeeCache = Array.Empty<Payee>();

    private Form? _activeChildForm;

    private readonly ContextMenuStrip _ctxBanksRoot;
    private readonly ContextMenuStrip _ctxAccountsRoot;
    private readonly ContextMenuStrip _ctxBudgetsRoot;
    private readonly ContextMenuStrip _ctxPayeesRoot;

    private readonly ContextMenuStrip _ctxBankNode;
    private readonly ContextMenuStrip _ctxAccountNode;
    private readonly ContextMenuStrip _ctxPayeeNode;

    private string _payeeNameFilter = string.Empty;

    private enum NavTag
    {
        BanksRoot,
        AccountsRoot,
        PayeesRoot,
        BudgetsRoot
    }

    private static class BudgetNav
    {
        public const string Template = "Budget.Template";

        public static string Current(int year, int month) => $"Budget.Current.{year:D4}.{month:D2}";

        public static string Year(int year) => $"Budget.Year.{year}";

        public static string Month(int year, int month) => $"Budget.Month.{year:D4}.{month:D2}";

        public static bool TryParseYear(string id, out int year)
        {
            year = 0;

            if (string.IsNullOrWhiteSpace(id))
                return false;

            string[] parts = id.Split('.');
            if (parts.Length != 3)
                return false;

            if (!string.Equals(parts[0], "Budget", StringComparison.OrdinalIgnoreCase))
                return false;

            if (!string.Equals(parts[1], "Year", StringComparison.OrdinalIgnoreCase))
                return false;

            return int.TryParse(parts[2], out year);
        }

        public static bool TryParseMonth(string id, out int year, out int month)
        {
            year = 0;
            month = 0;

            if (string.IsNullOrWhiteSpace(id))
                return false;

            string[] parts = id.Split('.');
            if (parts.Length != 4)
                return false;

            if (!string.Equals(parts[0], "Budget", StringComparison.OrdinalIgnoreCase))
                return false;

            if (!string.Equals(parts[1], "Month", StringComparison.OrdinalIgnoreCase))
                return false;

            if (!int.TryParse(parts[2], out year))
                return false;

            if (!int.TryParse(parts[3], out month))
                return false;

            return month is >= 1 and <= 12;
        }

        public static bool TryParseCurrent(string id, out int year, out int month)
        {
            year = 0;
            month = 0;

            if (string.IsNullOrWhiteSpace(id))
                return false;

            string[] parts = id.Split('.');
            if (parts.Length != 4)
                return false;

            if (!string.Equals(parts[0], "Budget", StringComparison.OrdinalIgnoreCase))
                return false;

            if (!string.Equals(parts[1], "Current", StringComparison.OrdinalIgnoreCase))
                return false;

            if (!int.TryParse(parts[2], out year))
                return false;

            if (!int.TryParse(parts[3], out month))
                return false;

            return month is >= 1 and <= 12;
        }
    }

    private static class PayeeNav
    {
        public const string NameView = "Payee.NameView";
        public const string TagView = "Payee.TagView";
        public const string TagGroupNone = "Payee.TagGroup.None";

        public static string TagGroup(string tagId) => $"Payee.TagGroup.{tagId}";
        public static string Item(string payeeId) => $"Payee.{payeeId}";

        public static bool TryParseItem(string id, out string payeeId)
        {
            payeeId = string.Empty;

            if (string.IsNullOrWhiteSpace(id))
                return false;

            if (!id.StartsWith("Payee.", StringComparison.OrdinalIgnoreCase))
                return false;

            string[] parts = id.Split('.');
            if (parts.Length != 2)
                return false;

            if (string.Equals(parts[1], "NameView", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(parts[1], "TagView", StringComparison.OrdinalIgnoreCase))
                return false;

            payeeId = parts[1];
            return !string.IsNullOrWhiteSpace(payeeId);
        }
    }

    private static Form WrapControlInForm(Control control, string title)
    {
        Form form = new()
        {
            Text = title,
            TopLevel = false,
            FormBorderStyle = FormBorderStyle.None,
            Dock = DockStyle.Fill
        };

        control.Dock = DockStyle.Fill;
        form.Controls.Add(control);

        return form;
    }

    public MainForm(
        IDatabaseSession dbSession,
        IAccountRepository accounts,
        IBankRepository banks,
        ITransactionRepository transactions,
        IPayeeRepository payees,
        Func<Form> createDbStatusForm)
    {
        _dbSession = dbSession ?? throw new ArgumentNullException(nameof(dbSession));
        _accounts = accounts ?? throw new ArgumentNullException(nameof(accounts));
        _banks = banks ?? throw new ArgumentNullException(nameof(banks));
        _transactions = transactions ?? throw new ArgumentNullException(nameof(transactions));
        _payees = payees ?? throw new ArgumentNullException(nameof(payees));
        _createDbStatusForm = createDbStatusForm ?? throw new ArgumentNullException(nameof(createDbStatusForm));


        InitializeComponent();

        _ctxBanksRoot = BuildBanksRootMenu();
        _ctxAccountsRoot = BuildAccountsRootMenu();
        _ctxPayeesRoot = BuildPayeesRootMenu();
        _ctxBudgetsRoot = BuildBudgetsRootMenu();

        _ctxBankNode = BuildBankNodeMenu();
        _ctxAccountNode = BuildAccountNodeMenu();
        _ctxPayeeNode = BuildPayeeNodeMenu();

        Load += MainForm_Load;

        treeNav.AfterSelect += TreeNav_AfterSelect;
        treeNav.NodeMouseClick += TreeNav_NodeMouseClick;

        menuFileSave.Click += MenuFileSave_Click;
        menuFileExit.Click += (_, _) => Close();
        menuViewRefresh.Click += MenuViewRefresh_Click;
        menuToolsDbStatus.Click += MenuToolsDbStatus_Click;

        menuHelpAbout.Click += MenuHelpAbout_Click;

        UpdateDbStatus();
        SetStatus("Ready");
    }

    private void MainForm_Load(object? sender, EventArgs e)
    {
        try
        {
            RefreshCaches();
            BuildNavigationTree();
            SelectFirstNode();
        }
        catch (Exception ex)
        {
            ShowError("Startup error", ex);
        }
    }

    private void MenuFileSave_Click(object? sender, EventArgs e)
    {
        try
        {
            _dbSession.Save();
            SetStatus("Saved.");
        }
        catch (Exception ex)
        {
            ShowError("Save failed", ex);
        }
    }

    private void MenuViewRefresh_Click(object? sender, EventArgs e)
    {
        try
        {
            RefreshCaches();
            BuildNavigationTree();
            SetStatus("Refreshed.");
        }
        catch (Exception ex)
        {
            ShowError("Refresh failed", ex);
        }
    }

    private void MenuToolsDbStatus_Click(object? sender, EventArgs e)
    {
        try
        {
            using Form dlg = _createDbStatusForm();
            dlg.ShowDialog(this);
        }
        catch (Exception ex)
        {
            ShowError("DB Status failed", ex);
        }
    }

    private void MenuHelpAbout_Click(object? sender, EventArgs e)
    {
        MessageBox.Show(
            this,
            "CodexExpensa\n\nA checkbook-style tracker.\n\n(Still under active development.)",
            "About",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
    }

    private void menuHelpAbout_Click_1(object sender, EventArgs e)
        => MenuHelpAbout_Click(sender, e);

    private void queryCatalogToolStripMenuItem_Click(object sender, EventArgs e)
    {
        using QueryCatalogForm form = new(_dbSession);
        form.ShowDialog(this);
    }

    private void RefreshCaches()
    {
        _bankCache = _banks.GetAll();
        _accountCache = _accounts.GetAll();
        _payeeCache = _payees.GetAll();
    }

    private void BuildNavigationTree()
    {
        treeNav.BeginUpdate();
        try
        {
            treeNav.Nodes.Clear();

            TreeNode banksRoot = new("Banks") { Tag = NavTag.BanksRoot };
            foreach (Bank b in _bankCache.OrderBy(b => b.BankName).ThenBy(b => b.RoutingNumber))
            {
                string label = $"{b.BankName} ({b.RoutingNumber})";
                banksRoot.Nodes.Add(new TreeNode(label) { Tag = b.BankId });
            }
            treeNav.Nodes.Add(banksRoot);

            TreeNode accountsRoot = new("Accounts") { Tag = NavTag.AccountsRoot };

            Dictionary<string, List<Account>> bankAccounts = new(StringComparer.Ordinal);
            foreach (Account acct in _accountCache)
            {
                string bankId = acct.BankId;
                if (string.IsNullOrWhiteSpace(bankId))
                    continue;

                if (!bankAccounts.TryGetValue(bankId, out List<Account>? list))
                {
                    list = new List<Account>();
                    bankAccounts[bankId] = list;
                }

                list.Add(acct);
            }

            foreach (KeyValuePair<string, List<Account>> kvp in bankAccounts.OrderBy(k => GetBankDisplayName(k.Key), StringComparer.OrdinalIgnoreCase))
            {
                string bankId = kvp.Key;
                List<Account> accountsForBank = kvp.Value;

                string bankDisplay = GetBankDisplayName(bankId);
                TreeNode bankNode = new(bankDisplay) { Tag = bankId };

                foreach (Account acct in accountsForBank
                             .OrderBy(a => a.SortIndex)
                             .ThenBy(a => a.AccountNickname)
                             .ThenBy(a => a.AccountNumber))
                {
                    string last4 = Last4(acct.AccountNumber);
                    string label = $"{acct.AccountNickname} - {last4}";
                    bankNode.Nodes.Add(new TreeNode(label) { Tag = acct.AccountId });
                }

                accountsRoot.Nodes.Add(bankNode);
            }

            treeNav.Nodes.Add(accountsRoot);

            TreeNode payeesRoot = new("Payees") { Tag = NavTag.PayeesRoot };

            TreeNode payeeNameNode = new("Name") { Tag = PayeeNav.NameView };
            TreeNode payeeTagNode = new("Tags") { Tag = PayeeNav.TagView };

            payeesRoot.Nodes.Add(payeeNameNode);
            payeesRoot.Nodes.Add(payeeTagNode);

            // Default normal payee list under root for now
            foreach (Payee p in _payeeCache
                         .OrderByDescending(p => p.IncludeInBudgetTemplate)
                         .ThenBy(p => p.PayeeName))
            {
                string label = p.IncludeInBudgetTemplate ? $"{p.PayeeName}  ✓" : p.PayeeName;
                payeesRoot.Nodes.Add(new TreeNode(label) { Tag = PayeeNav.Item(p.PayeeId) });
            }

            treeNav.Nodes.Add(payeesRoot);

            TreeNode budgetsRoot = new("Budgets") { Tag = NavTag.BudgetsRoot };

            budgetsRoot.Nodes.Add(new TreeNode("Template") { Tag = BudgetNav.Template });

            DateTime today = DateTime.Today;
            budgetsRoot.Nodes.Add(new TreeNode($"Current ({today:MMM/yyyy})")
            {
                Tag = BudgetNav.Current(today.Year, today.Month)
            });

            int currentYear = today.Year;
            int currentMonth = today.Month;

            List<int> years = Enumerable.Range(currentYear - 4, 5).Reverse().ToList();

            foreach (int year in years)
            {
                TreeNode yearNode = new(year.ToString()) { Tag = BudgetNav.Year(year) };

                List<int> months;
                if (year == currentYear)
                {
                    int count = Math.Max(0, currentMonth - 1);
                    months = Enumerable.Range(1, count).ToList();
                    months.Reverse();
                }
                else
                {
                    months = Enumerable.Range(1, 12).ToList();
                }

                foreach (int m in months)
                {
                    string monthName = new DateTime(year, m, 1).ToString("MMM");
                    yearNode.Nodes.Add(new TreeNode(monthName) { Tag = BudgetNav.Month(year, m) });
                }

                budgetsRoot.Nodes.Add(yearNode);
            }

            treeNav.Nodes.Add(budgetsRoot);
        }
        finally
        {
            treeNav.EndUpdate();
        }
    }

    private void BuildPayeeTagNodes(TreeNode payeeTagNode)
    {
        if (payeeTagNode is null)
            return;

        payeeTagNode.Nodes.Clear();

        var payeesById = _payeeCache.ToDictionary(p => p.PayeeId, StringComparer.Ordinal);

        var tagRows = _dbSession.QueryDataTable("PayeeTag.SelectAllForNavigation");

        var grouped = new Dictionary<string, List<Payee>>(StringComparer.OrdinalIgnoreCase);
        var taggedPayeeIds = new HashSet<string>(StringComparer.Ordinal);

        foreach (DataRow row in tagRows.Rows)
        {
            string payeeId = row["PayeeId"]?.ToString() ?? string.Empty;
            string tagId = row["TagId"]?.ToString() ?? string.Empty;
            string tagName = row["TagName"]?.ToString() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(payeeId) ||
                string.IsNullOrWhiteSpace(tagId) ||
                string.IsNullOrWhiteSpace(tagName))
                continue;

            if (!payeesById.TryGetValue(payeeId, out Payee? payee))
                continue;

            taggedPayeeIds.Add(payeeId);

            if (!grouped.TryGetValue(tagName, out List<Payee>? list))
            {
                list = new List<Payee>();
                grouped[tagName] = list;
            }

            list.Add(payee);
        }

        // None group
        TreeNode noneNode = new("None") { Tag = PayeeNav.TagGroupNone };

        foreach (Payee payee in _payeeCache
                     .Where(p => !taggedPayeeIds.Contains(p.PayeeId))
                     .OrderBy(p => p.PayeeName))
        {
            noneNode.Nodes.Add(new TreeNode(payee.PayeeName)
            {
                Tag = PayeeNav.Item(payee.PayeeId)
            });
        }

        payeeTagNode.Nodes.Add(noneNode);

        // Tag groups
        foreach (var kvp in grouped.OrderBy(x => x.Key, StringComparer.OrdinalIgnoreCase))
        {
            string tagName = kvp.Key;
            List<Payee> payees = kvp.Value
                .GroupBy(p => p.PayeeId, StringComparer.Ordinal)
                .Select(g => g.First())
                .OrderBy(p => p.PayeeName)
                .ToList();

            TreeNode tagNode = new(tagName)
            {
                Tag = PayeeNav.TagGroup(tagName)
            };

            foreach (Payee payee in payees)
            {
                tagNode.Nodes.Add(new TreeNode(payee.PayeeName)
                {
                    Tag = PayeeNav.Item(payee.PayeeId)
                });
            }

            payeeTagNode.Nodes.Add(tagNode);
        }
    }

    private void ShowPayeeTagView()
    {
        Label label = new()
        {
            Dock = DockStyle.Fill,
            Text = "Payees are grouped in the tree under each tag.\r\nSelect a payee under a tag group to open it.",
            TextAlign = ContentAlignment.MiddleCenter
        };

        Panel panel = new()
        {
            Dock = DockStyle.Fill
        };

        panel.Controls.Add(label);

        ShowChildForm(WrapControlInForm(panel, "Payee Tags"));
    }

    private void BuildPayeeNameNodes(TreeNode payeesRoot)
    {
        if (payeesRoot is null)
            return;

        // Keep Name and Tags nodes, remove only real payee item nodes
        while (payeesRoot.Nodes.Count > 2)
            payeesRoot.Nodes.RemoveAt(2);

        IEnumerable<Payee> filtered = _payeeCache;

        if (!string.IsNullOrWhiteSpace(_payeeNameFilter))
        {
            filtered = filtered.Where(p =>
                p.PayeeName.Contains(_payeeNameFilter, StringComparison.OrdinalIgnoreCase));
        }

        foreach (Payee p in filtered
                     .OrderByDescending(p => p.IncludeInBudgetTemplate)
                     .ThenBy(p => p.PayeeName))
        {
            string label = p.IncludeInBudgetTemplate ? $"{p.PayeeName}  ✓" : p.PayeeName;

            payeesRoot.Nodes.Add(new TreeNode(label)
            {
                Tag = PayeeNav.Item(p.PayeeId)
            });
        }
    }

    private string GetBankDisplayName(string bankId)
    {
        Bank? bank = _bankCache.FirstOrDefault(b => b.BankId == bankId);
        if (bank is null)
            return bankId;

        return $"{bank.BankName} ({bank.RoutingNumber})";
    }

    private void ShowBudgetYear(int year)
    {
        BudgetYearForm form = new(_dbSession, year);
        ShowChildForm(form);
    }

    private static string Last4(string accountNumber)
    {
        if (string.IsNullOrWhiteSpace(accountNumber))
            return "????";

        string cleaned = accountNumber.Replace(" ", "").Replace("-", "");
        return cleaned.Length <= 4 ? cleaned : cleaned.Substring(cleaned.Length - 4);
    }

    private void SelectFirstNode()
    {
        if (treeNav.Nodes.Count == 0)
            return;

        treeNav.SelectedNode = treeNav.Nodes[0];
    }

    private void TreeNav_AfterSelect(object? sender, TreeViewEventArgs e)
    {
        if (e.Node is null)
            return;

        if (TryHandleBudgetNodeSelection(e.Node))
            return;

        if (TryHandlePayeeViewNodeSelection(e.Node))
            return;

        if (TryHandleRootNodeSelection(e.Node))
            return;

        if (TryHandlePayeeNodeSelection(e.Node))
            return;

        if (TryHandleBankNodeSelection(e.Node))
            return;

        if (TryHandleAccountNodeSelection(e.Node))
            return;
    }

    private bool TryHandleBudgetNodeSelection(TreeNode node)
    {
        if (node.Tag is not string tag)
            return false;

        if (string.Equals(tag, BudgetNav.Template, StringComparison.OrdinalIgnoreCase))
        {
            ShowBudgetTemplate();
            SetStatus("Budgets: Template");
            return true;
        }

        if (BudgetNav.TryParseCurrent(tag, out int currentYear, out int currentMonth))
        {
            ShowBudgetMonth(currentYear, currentMonth);
            SetStatus($"Budgets: Current ({new DateTime(currentYear, currentMonth, 1):MMM/yyyy})");
            return true;
        }

        if (BudgetNav.TryParseYear(tag, out int budgetYear))
        {
            ShowBudgetYear(budgetYear);
            SetStatus($"Budgets: {budgetYear}");
            return true;
        }

        if (BudgetNav.TryParseMonth(tag, out int year, out int month))
        {
            ShowBudgetMonth(year, month);
            SetStatus($"Budgets: {year}-{month:D2}");
            return true;
        }

        return false;
    }

    private bool TryHandlePayeeViewNodeSelection(TreeNode node)
    {
        if (node.Tag is not string tag)
            return false;

        if (string.Equals(tag, PayeeNav.NameView, StringComparison.OrdinalIgnoreCase))
        {
            ShowPayeeNameFilter();
            SetStatus("Payees: Name");
            return true;
        }

        if (string.Equals(tag, PayeeNav.TagView, StringComparison.OrdinalIgnoreCase))
        {
            BuildPayeeTagNodes(node);
            node.Expand();
            ShowPayeeTagView();
            SetStatus("Payees: Tags");
            return true;
        }

        return false;
    }

    private bool TryHandleRootNodeSelection(TreeNode node)
    {
        if (node.Tag is not NavTag rootTag)
            return false;

        switch (rootTag)
        {
            case NavTag.BanksRoot:
                SetStatus("Banks");
                ShowBanksLanding();
                return true;

            case NavTag.AccountsRoot:
                SetStatus("Accounts");
                ShowAccountsList();
                return true;

            case NavTag.PayeesRoot:
                SetStatus("Payees");
                ShowPayeesLanding();
                return true;

            case NavTag.BudgetsRoot:
                SetStatus("Budgets");
                ShowBudgetsLanding();
                return true;

            default:
                return false;
        }
    }

    private bool TryHandlePayeeNodeSelection(TreeNode node)
    {
        if (node.Tag is string taggedNode &&
            PayeeNav.TryParseItem(taggedNode, out string parsedPayeeId))
        {
            ShowPayeeDetails(parsedPayeeId);
            SetStatus($"Payee: {node.Text}");
            return true;
        }

        if (node.Tag is string payeeId &&
            node.Parent?.Tag is NavTag payeeRootTag &&
            payeeRootTag == NavTag.PayeesRoot)
        {
            ShowPayeeDetails(payeeId);
            SetStatus($"Payee: {node.Text}");
            return true;
        }

        return false;
    }

    private bool TryHandleBankNodeSelection(TreeNode node)
    {
        if (node.Tag is string bankId &&
            node.Parent?.Tag is NavTag banksRootTag &&
            banksRootTag == NavTag.BanksRoot)
        {
            ShowBankDetails(bankId);
            SetStatus($"Bank: {node.Text}");
            return true;
        }

        if (node.Tag is string accountBankId &&
            node.Parent?.Tag is NavTag accountsRootTag &&
            accountsRootTag == NavTag.AccountsRoot)
        {
            ShowBankDetails(accountBankId);
            SetStatus($"Bank: {node.Text}");
            return true;
        }

        return false;
    }

    private bool TryHandleAccountNodeSelection(TreeNode node)
    {
        if (node.Tag is string accountId &&
            node.Parent?.Parent?.Tag is NavTag accountsRootTag &&
            accountsRootTag == NavTag.AccountsRoot)
        {
            ShowAccountDetails(accountId);
            SetStatus($"Account: {node.Text}");
            return true;
        }

        return false;
    }
    private void TreeNav_NodeMouseClick(object? sender, TreeNodeMouseClickEventArgs e)
    {
        if (e.Node is null)
            return;

        if (e.Button != MouseButtons.Right)
            return;

        treeNav.SelectedNode = e.Node;

        if (e.Node.Tag is NavTag rootTag)
        {
            switch (rootTag)
            {
                case NavTag.BanksRoot:
                    _ctxBanksRoot.Show(treeNav, e.Location);
                    return;

                case NavTag.AccountsRoot:
                    _ctxAccountsRoot.Show(treeNav, e.Location);
                    return;

                case NavTag.PayeesRoot:
                    _ctxPayeesRoot.Show(treeNav, e.Location);
                    return;

                case NavTag.BudgetsRoot:
                    _ctxBudgetsRoot.Show(treeNav, e.Location);
                    return;
            }
        }

        if (e.Node.Tag is string payeeId &&
            payeeId != "Payee.Search" &&
            e.Node.Parent?.Tag is NavTag pr &&
            pr == NavTag.PayeesRoot)
        {
            _ctxPayeeNode.Show(treeNav, e.Location);
            return;
        }

        if (e.Node.Tag is string &&
            e.Node.Parent?.Tag is NavTag parentTag &&
            parentTag == NavTag.BanksRoot)
        {
            _ctxBankNode.Show(treeNav, e.Location);
            return;
        }

        if (e.Node.Tag is string &&
            e.Node.Parent?.Tag is NavTag parentTag2 &&
            parentTag2 == NavTag.AccountsRoot)
        {
            _ctxBankNode.Show(treeNav, e.Location);
            return;
        }

        if (e.Node.Tag is string &&
            e.Node.Parent?.Parent?.Tag is NavTag rootTag2 &&
            rootTag2 == NavTag.AccountsRoot)
        {
            _ctxAccountNode.Show(treeNav, e.Location);
        }
    }

    private ContextMenuStrip BuildBanksRootMenu()
    {
        ContextMenuStrip menu = new();

        ToolStripMenuItem add = new("Add Bank");
        add.Click += (_, _) =>
        {
            using AddBankForm dlg = new();
            if (dlg.ShowDialog(this) != DialogResult.OK)
                return;

            Bank bank = new()
            {
                BankId = Guid.NewGuid().ToString("N"),
                BankName = dlg.BankName,
                RoutingNumber = dlg.RoutingNumber,
                Url = dlg.Url,
                IsActive = dlg.IsActive
            };

            _banks.Add(bank);
            RefreshCaches();
            BuildNavigationTree();
        };

        menu.Items.Add(add);
        return menu;
    }

    private ContextMenuStrip BuildAccountsRootMenu()
    {
        ContextMenuStrip menu = new();

        ToolStripMenuItem addChecking = new("Add Checking Account...");
        addChecking.Click += (_, _) =>
        {
            using AddCheckingAccountForm dlg = new(_bankCache);
            if (dlg.ShowDialog(this) != DialogResult.OK)
                return;

            string? bankId = dlg.BankId;
            if (string.IsNullOrWhiteSpace(bankId))
                return;

            Bank? bank = _bankCache.FirstOrDefault(b => b.BankId == bankId);
            if (bank is null)
                return;

            Account acct = new()
            {
                AccountId = Guid.NewGuid().ToString("N"),
                BankId = bank.BankId,
                BankName = bank.BankName,
                RoutingNumber = bank.RoutingNumber,
                Url = bank.Url,
                AccountNickname = dlg.AccountNickname,
                SortIndex = 0,
                AccountNumber = dlg.AccountNumber,
                AccountType = AccountType.Checking,
                IsActive = dlg.IsActive
            };

            _accounts.Add(acct);
            RefreshCaches();
            BuildNavigationTree();
        };

        menu.Items.Add(addChecking);
        return menu;
    }

    private ContextMenuStrip BuildPayeesRootMenu()
    {
        ContextMenuStrip menu = new();

        ToolStripMenuItem add = new("Add Payee...");
        add.Click += (_, _) =>
        {
            string? name = PromptForText(this, "Add Payee", "Payee name:");
            if (string.IsNullOrWhiteSpace(name))
                return;

            Payee payee = new()
            {
                PayeeId = Guid.NewGuid().ToString("N"),
                PayeeName = name.Trim(),
                IncludeInBudgetTemplate = false
            };

            _payees.Add(payee);
            RefreshCaches();
            BuildNavigationTree();
        };

        menu.Items.Add(add);
        return menu;
    }

    private ContextMenuStrip BuildBudgetsRootMenu()
    {
        ContextMenuStrip menu = new();
        return menu;
    }

    private ContextMenuStrip BuildBankNodeMenu()
    {
        ContextMenuStrip menu = new();

        ToolStripMenuItem addChecking = new("Add Checking Account...");
        addChecking.Click += (_, _) =>
        {
            string? bankId = treeNav.SelectedNode?.Tag as string;
            if (string.IsNullOrWhiteSpace(bankId))
                return;

            using AddCheckingAccountForm dlg = new(_bankCache, preselectedBankId: bankId);
            if (dlg.ShowDialog(this) != DialogResult.OK)
                return;

            string? selectedBankId = dlg.BankId;
            if (string.IsNullOrWhiteSpace(selectedBankId))
                return;

            Bank? bank = _bankCache.FirstOrDefault(b => b.BankId == selectedBankId);
            if (bank is null)
                return;

            Account acct = new()
            {
                AccountId = Guid.NewGuid().ToString("N"),
                BankId = bank.BankId,
                BankName = bank.BankName,
                RoutingNumber = bank.RoutingNumber,
                Url = bank.Url,
                AccountNickname = dlg.AccountNickname,
                SortIndex = 0,
                AccountNumber = dlg.AccountNumber,
                AccountType = AccountType.Checking,
                IsActive = dlg.IsActive
            };

            _accounts.Add(acct);
            RefreshCaches();
            BuildNavigationTree();
        };

        menu.Items.Add(addChecking);
        return menu;
    }

    private ContextMenuStrip BuildAccountNodeMenu()
    {
        ContextMenuStrip menu = new();

        ToolStripMenuItem del = new("Delete Account");
        del.Click += (_, _) =>
        {
            string? accountId = treeNav.SelectedNode?.Tag as string;
            if (string.IsNullOrWhiteSpace(accountId))
                return;

            DialogResult confirm = MessageBox.Show(
                this,
                "Delete this account?",
                "Confirm",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirm != DialogResult.Yes)
                return;

            _accounts.Delete(accountId);
            RefreshCaches();
            BuildNavigationTree();
            ShowAccountsList();
        };

        menu.Items.Add(del);
        return menu;
    }

    private ContextMenuStrip BuildPayeeNodeMenu()
    {
        ContextMenuStrip menu = new();

        ToolStripMenuItem del = new("Delete Payee");
        del.Click += (_, _) =>
        {
            string? payeeId = treeNav.SelectedNode?.Tag as string;
            if (string.IsNullOrWhiteSpace(payeeId))
                return;

            DialogResult confirm = MessageBox.Show(
                this,
                "Delete this payee?",
                "Confirm",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirm != DialogResult.Yes)
                return;

            _payees.Delete(payeeId);
            RefreshCaches();
            BuildNavigationTree();
            ShowPayeesLanding();
        };

        menu.Items.Add(del);
        return menu;
    }

    private void ShowPayeeNameFilter()
    {
        TextBox box = new()
        {
            Dock = DockStyle.Top,
            Width = 300,
            Text = _payeeNameFilter
        };

        Label label = new()
        {
            Dock = DockStyle.Top,
            Height = 24,
            Text = "Filter payees by name",
            TextAlign = ContentAlignment.MiddleLeft
        };

        Panel panel = new()
        {
            Dock = DockStyle.Fill
        };

        box.TextChanged += (_, _) =>
        {
            _payeeNameFilter = box.Text.Trim();

            TreeNode? payeesRoot = treeNav.Nodes
                .Cast<TreeNode>()
                .FirstOrDefault(n => n.Tag is NavTag tag && tag == NavTag.PayeesRoot);

            if (payeesRoot is not null)
            {
                treeNav.BeginUpdate();
                try
                {
                    BuildPayeeNameNodes(payeesRoot);
                    payeesRoot.Expand();
                }
                finally
                {
                    treeNav.EndUpdate();
                }
            }
        };

        panel.Controls.Add(box);
        panel.Controls.Add(label);

        ShowChildForm(WrapControlInForm(panel, "Payee Name Filter"));
    }

    private void ShowBanksLanding()
    {
        BanksLandingForm form = new(openBankDetails: bankId =>
        {
            ShowBankDetails(bankId);
        });

        form.SetBanks(_bankCache);

        ShowChildForm(form);
    }

    private void ShowAccountsList()
    {
        AccountsListForm form = new(openAccountDetails: accountId =>
        {
            ShowAccountDetails(accountId);
        });

        form.SetAccounts(_accountCache);

        ShowChildForm(form);
    }

    private void ShowPayeesLanding()
    {
        PayeesLandingForm form = new(
            _dbSession,
            openPayeeDetails: payeeId =>
            {
                ShowPayeeDetails(payeeId);
            },
            savePayee: payee =>
            {
                _payees.Update(payee);
                RefreshCaches();
                BuildNavigationTree();
            });

        form.SetPayees(_payeeCache);

        ShowChildForm(form);
    }

    private void ShowBudgetsLanding()
    {
        BudgetsLandingForm form = new();
        ShowChildForm(form);
    }

    private void ShowPayeeDetails(string payeeId)
    {
        Payee? payee = _payeeCache.FirstOrDefault(p => p.PayeeId == payeeId);
        if (payee is null)
        {
            MessageBox.Show(this, "Payee not found.", "Expensa", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        PayeeDetailsForm form = new(
            repo: _payees,
            onSaved: () =>
            {
                RefreshCaches();
                BuildNavigationTree();
            });

        form.LoadPayee(payee);
        ShowChildForm(form);
    }

    private void ShowBankDetails(string bankId)
    {
        Bank? bank = _bankCache.FirstOrDefault(b => b.BankId == bankId);
        if (bank is null)
        {
            MessageBox.Show(this, "Bank not found.", "Expensa", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        BankDetailsForm form = new(
            repo: _banks,
            credentialStore: _creds,
            onSaved: () =>
            {
                RefreshCaches();
                BuildNavigationTree();
            });

        form.LoadBank(bank);
        ShowChildForm(form);
    }

    private void ShowAccountDetails(string accountId)
    {
        Account? acct = _accountCache.FirstOrDefault(a => a.AccountId == accountId);
        if (acct is null)
        {
            MessageBox.Show(this, "Account not found.", "Expensa", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        AccountDetailsForm form = new(
            repo: _accounts,
            banks: _bankCache,
            creds: _creds,
            txns: _transactions,
            payees: _payees,
            onSaved: () =>
            {
                RefreshCaches();
                BuildNavigationTree();
            });

        form.LoadAccount(acct);
        ShowChildForm(form);
    }

    private void ShowBudgetTemplate()
    {
        BudgetTemplateForm form = new(_payees);
        ShowChildForm(form);
    }

    private void ShowBudgetMonth(int year, int month)
    {
        BudgetMonthForm form = new(_dbSession, year, month);
        ShowChildForm(form);
    }

    private void ShowChildForm(Form form)
    {
        if (_activeChildForm is not null)
        {
            try
            {
                _activeChildForm.Close();
                _activeChildForm.Dispose();
            }
            catch
            {
                // ignore
            }
        }

        _activeChildForm = form;

        panelHost.Controls.Clear();

        form.TopLevel = false;
        form.FormBorderStyle = FormBorderStyle.None;
        form.Dock = DockStyle.Fill;

        panelHost.Controls.Add(form);
        form.Show();
    }

    private void UpdateDbStatus()
    {
        statusDbMode.Text = "DB: ???";
    }

    private void SetStatus(string message)
    {
        statusText.Text = message;
    }

    private void ShowError(string title, Exception ex)
    {
        MessageBox.Show(
            this,
            $"{ex.GetType().Name}: {ex.Message}",
            title,
            MessageBoxButtons.OK,
            MessageBoxIcon.Error);
    }

    private static string? PromptForText(IWin32Window owner, string title, string prompt)
    {
        using Form form = new()
        {
            Text = title,
            StartPosition = FormStartPosition.CenterParent,
            FormBorderStyle = FormBorderStyle.FixedDialog,
            MinimizeBox = false,
            MaximizeBox = false,
            ShowInTaskbar = false,
            Width = 520,
            Height = 170
        };

        Label lbl = new() { Left = 12, Top = 16, AutoSize = true, Text = prompt };
        TextBox txt = new() { Left = 12, Top = 44, Width = 480, Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };
        Button ok = new() { Text = "OK", Left = 312, Width = 80, Top = 82, DialogResult = DialogResult.OK };
        Button cancel = new() { Text = "Cancel", Left = 412, Width = 80, Top = 82, DialogResult = DialogResult.Cancel };

        form.Controls.Add(lbl);
        form.Controls.Add(txt);
        form.Controls.Add(ok);
        form.Controls.Add(cancel);

        form.AcceptButton = ok;
        form.CancelButton = cancel;

        return form.ShowDialog(owner) == DialogResult.OK ? txt.Text.Trim() : null;
    }

    private void diagnosticsToolStripMenuItem_Click(object sender, EventArgs e)
    {
        using DatabaseDiagnosticsForm form = new(_dbSession);
        form.ShowDialog(this);
    }

    private void budgetMonthsToolStripMenuItem_Click(object sender, EventArgs e)
    {
        DateTime today = DateTime.Today;

        using BudgetMonthForm form = new(_dbSession, today.Year, today.Month);
        form.ShowDialog(this);
    }
}
