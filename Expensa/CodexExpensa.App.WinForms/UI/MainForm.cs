using CodexExpensa.App.WinForms.Infrastructure;
using CodexExpensa.App.WinForms.UI.Accounts;
using CodexExpensa.App.WinForms.UI.Banks;
using CodexExpensa.App.WinForms.UI.Budgets;
using CodexExpensa.Core.Abstractions;
using CodexExpensa.Core.Domain.Accounts;
using CodexExpensa.Core.Domain.Banks;
using CodexExpensa.Core.Domain.Payees;
using CodexExpensa.Core.Domain.Transactions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace CodexExpensa.App.WinForms.UI;

public partial class MainForm : Form
{
    private readonly IDatabaseSession _dbSession;
    private readonly IAccountRepository _accounts;
    private readonly IBankRepository _banks;
    private readonly Func<Form> _createDbStatusForm;

    private IReadOnlyList<Account> _accountCache = Array.Empty<Account>();
    private IReadOnlyList<Bank> _bankCache = Array.Empty<Bank>();

    private Form? _activeChildForm;

    private readonly ContextMenuStrip _ctxBanksRoot;
    private readonly ContextMenuStrip _ctxAccountsRoot;
    private readonly ContextMenuStrip _ctxBudgetsRoot;
    private readonly ContextMenuStrip _ctxBankNode;
    private readonly ContextMenuStrip _ctxAccountNode;

    private readonly ICredentialStore _creds = new WindowsCredentialStore();

    private readonly ITransactionRepository _transactions;
    private readonly IPayeeRepository _payees;

    private enum NavTag
    {
        BanksRoot,
        AccountsRoot,
        BudgetsRoot
    }

    // Budget node ids (string tags)
    private static class BudgetNav
    {
        public const string Template = "Budget.Template";

        public static string Year(int year) => $"Budget.Year.{year}";
        public static string Month(int year, int month) => $"Budget.Month.{year:D4}.{month:D2}";

        public static bool TryParseMonth(string id, out int year, out int month)
        {
            year = 0;
            month = 0;

            // Budget.Month.2026.12
            if (string.IsNullOrWhiteSpace(id))
                return false;

            var parts = id.Split('.');
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

            if (month < 1 || month > 12)
                return false;

            return true;
        }
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
        _createDbStatusForm = createDbStatusForm ?? throw new ArgumentNullException(nameof(createDbStatusForm));

        _transactions = transactions ?? throw new ArgumentNullException(nameof(transactions));
        _payees = payees ?? throw new ArgumentNullException(nameof(payees));

        InitializeComponent();

        _ctxBanksRoot = BuildBanksRootMenu();
        _ctxAccountsRoot = BuildAccountsRootMenu();
        _ctxBudgetsRoot = BuildBudgetsRootMenu();
        _ctxBankNode = BuildBankNodeMenu();
        _ctxAccountNode = BuildAccountNodeMenu();

        Load += MainForm_Load;

        treeNav.AfterSelect += TreeNav_AfterSelect;
        treeNav.NodeMouseClick += TreeNav_NodeMouseClick;

        // Menu wiring (names must exist in designer)
        menuFileSave.Click += MenuFileSave_Click;
        menuFileExit.Click += (_, _) => Close();
        menuViewRefresh.Click += MenuViewRefresh_Click;
        menuToolsDbStatus.Click += MenuToolsDbStatus_Click;

        // Keep both handlers safe: some designers wire one or the other.
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
            using var dlg = _createDbStatusForm();
            dlg.ShowDialog(this);
        }
        catch (Exception ex)
        {
            ShowError("DB Status failed", ex);
        }
    }

    private void MenuHelpAbout_Click(object? sender, EventArgs e)
    {
        var dbPath = GetDatabasePathForDisplay();

        MessageBox.Show(
            this,
            "Codex Expensa\n\n" +
            "Database Location:\n" +
            $"{dbPath}\n\n" +
            "Migrations, startup, and database configuration are stable.\n" +
            "Do not modify bootstrap or migration logic.\n\n" +
            "(Still not doing your taxes. Calm down.)",
            "About Codex Expensa",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
    }

    // If your designer still points to this, keep it and forward to the real handler.
    private void menuHelpAbout_Click_1(object sender, EventArgs e)
        => MenuHelpAbout_Click(sender, e);

    private static string GetDatabasePathForDisplay()
    {
        // Per your preference:
        // var folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        //     "CodexExpensa","db");
        var folder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "CodexExpensa",
            "db");

        return Path.Combine(folder, "codexexpensa.db");
    }

    private void RefreshCaches()
    {
        _bankCache = _banks.GetAll();
        _accountCache = _accounts.GetAll();
    }

    private void BuildNavigationTree()
    {
        treeNav.BeginUpdate();
        try
        {
            treeNav.Nodes.Clear();

            // Banks root
            var banksRoot = new TreeNode("Banks") { Tag = NavTag.BanksRoot };
            foreach (var b in _bankCache.OrderBy(b => b.BankName).ThenBy(b => b.RoutingNumber))
            {
                var label = $"{b.BankName} ({b.RoutingNumber})";
                banksRoot.Nodes.Add(new TreeNode(label) { Tag = b.BankId });
            }
            banksRoot.Expand();
            treeNav.Nodes.Add(banksRoot);

            // Accounts root: Accounts -> Bank (tag=bankId) -> Account leaf (tag=accountId)
            var accountsRoot = new TreeNode("Accounts") { Tag = NavTag.AccountsRoot };

            // Build bankId -> accounts list (resolving bankId when missing)
            var bankAccounts = new Dictionary<string, List<Account>>(StringComparer.Ordinal);
            foreach (var acct in _accountCache)
            {
                var bankId = ResolveBankId(acct);
                if (string.IsNullOrWhiteSpace(bankId))
                    continue;

                if (!bankAccounts.TryGetValue(bankId, out var list))
                {
                    list = new List<Account>();
                    bankAccounts[bankId] = list;
                }

                list.Add(acct);
            }

            foreach (var kvp in bankAccounts
                         .OrderBy(k => GetBankDisplayName(k.Key), StringComparer.OrdinalIgnoreCase))
            {
                var bankId = kvp.Key;
                var accountsForBank = kvp.Value;

                var bankDisplay = GetBankDisplayName(bankId);
                var bankNode = new TreeNode(bankDisplay) { Tag = bankId };

                foreach (var acct in accountsForBank
                             .OrderBy(a => a.SortIndex)
                             .ThenBy(a => a.AccountNickname)
                             .ThenBy(a => a.AccountNumber))
                {
                    var last4 = Last4(acct.AccountNumber);
                    var label = $"{acct.AccountNickname} - {last4}";
                    bankNode.Nodes.Add(new TreeNode(label) { Tag = acct.AccountId });
                }

                bankNode.Expand();
                accountsRoot.Nodes.Add(bankNode);
            }

            accountsRoot.Expand();
            treeNav.Nodes.Add(accountsRoot);

            // Budgets root
            var budgetsRoot = new TreeNode("Budgets") { Tag = NavTag.BudgetsRoot };

            // Template node immediately under Budgets
            budgetsRoot.Nodes.Add(new TreeNode("Template") { Tag = BudgetNav.Template });

            // Years and months
            // For now (no budget repo yet), we generate a predictable scaffold.
            // Years descending.
            var currentYear = DateTime.Today.Year;
            var years = Enumerable.Range(currentYear - 4, 5).Reverse().ToList(); // currentYear..currentYear-4

            foreach (var year in years)
            {
                var yearNode = new TreeNode(year.ToString()) { Tag = BudgetNav.Year(year) };

                var months = Enumerable.Range(1, 12).ToList();
                if (year == currentYear)
                    months.Reverse(); // current year descending
                // older years: ascending (Jan..Dec)

                foreach (var m in months)
                {
                    var monthName = new DateTime(year, m, 1).ToString("MMM");
                    yearNode.Nodes.Add(new TreeNode(monthName) { Tag = BudgetNav.Month(year, m) });
                }

                yearNode.Expand();
                budgetsRoot.Nodes.Add(yearNode);
            }

            budgetsRoot.Expand();
            treeNav.Nodes.Add(budgetsRoot);
        }
        finally
        {
            treeNav.EndUpdate();
        }
    }

    private string GetBankDisplayName(string bankId)
    {
        var bank = _bankCache.FirstOrDefault(b => b.BankId == bankId);
        if (bank is null)
            return bankId;

        return $"{bank.BankName} ({bank.RoutingNumber})";
    }

    private string? ResolveBankId(Account acct)
    {
        if (!string.IsNullOrWhiteSpace(acct.BankId))
            return acct.BankId;

        // Fallback: match by BankName + RoutingNumber if older rows didn’t store BankId
        if (string.IsNullOrWhiteSpace(acct.BankName) || string.IsNullOrWhiteSpace(acct.RoutingNumber))
            return null;

        var match = _bankCache.FirstOrDefault(b =>
            string.Equals(b.BankName, acct.BankName, StringComparison.OrdinalIgnoreCase) &&
            string.Equals(b.RoutingNumber, acct.RoutingNumber, StringComparison.OrdinalIgnoreCase));

        return match?.BankId;
    }

    private static string Last4(string accountNumber)
    {
        if (string.IsNullOrWhiteSpace(accountNumber))
            return "????";

        var cleaned = accountNumber.Replace(" ", "").Replace("-", "");
        return cleaned.Length <= 4 ? cleaned : cleaned.Substring(cleaned.Length - 4);
    }

    private void SelectFirstNode()
    {
        if (treeNav.Nodes.Count > 0)
            treeNav.SelectedNode = treeNav.Nodes[0];
    }

    private void TreeNav_NodeMouseClick(object? sender, TreeNodeMouseClickEventArgs e)
    {
        if (e.Node is null)
            return;

        if (e.Button != MouseButtons.Right)
            return;

        treeNav.SelectedNode = e.Node;

        var menu = GetContextMenuForNode(e.Node);
        if (menu is not null)
            menu.Show(treeNav, e.Location);
    }

    private ContextMenuStrip? GetContextMenuForNode(TreeNode node)
    {
        if (node.Tag is NavTag tag)
        {
            return tag switch
            {
                NavTag.BanksRoot => _ctxBanksRoot,
                NavTag.AccountsRoot => _ctxAccountsRoot,
                NavTag.BudgetsRoot => _ctxBudgetsRoot,
                _ => null
            };
        }

        // Bank node under Banks root
        if (node.Tag is string && node.Parent?.Tag is NavTag parentTag && parentTag == NavTag.BanksRoot)
            return _ctxBankNode;

        // Bank node under Accounts root (works like real bank nodes)
        if (node.Tag is string && node.Parent?.Tag is NavTag parentTag2 && parentTag2 == NavTag.AccountsRoot)
            return _ctxBankNode;

        // Account leaf under Accounts root: AccountsRoot -> BankNode -> AccountLeaf
        if (node.Tag is string && node.Parent is not null && node.Parent.Parent?.Tag is NavTag p2 && p2 == NavTag.AccountsRoot)
            return _ctxAccountNode;

        // Budgets subtree: no context menus yet (keep it simple)
        return null;
    }

    private void TreeNav_AfterSelect(object? sender, TreeViewEventArgs e)
    {
        if (e.Node is null)
            return;

        // Root nodes
        if (e.Node.Tag is NavTag tag)
        {
            switch (tag)
            {
                case NavTag.BanksRoot:
                    ShowBanksLanding();
                    SetStatus("Banks");
                    return;

                case NavTag.AccountsRoot:
                    ShowAccountsList();
                    SetStatus("Accounts");
                    return;

                case NavTag.BudgetsRoot:
                    ShowBudgetsLanding();
                    SetStatus("Budgets");
                    return;
            }
        }

        // Budgets: template/month nodes are string tags
        if (e.Node.Tag is string budgetTag)
        {
            if (string.Equals(budgetTag, BudgetNav.Template, StringComparison.OrdinalIgnoreCase))
            {
                ShowBudgetTemplate();
                SetStatus("Budgets: Template");
                return;
            }

            if (BudgetNav.TryParseMonth(budgetTag, out var year, out var month))
            {
                ShowBudgetMonth(year, month);
                SetStatus($"Budgets: {year}-{month:D2}");
                return;
            }
        }

        // Bank leaf under Banks root
        if (e.Node.Tag is string bankId &&
            e.Node.Parent?.Tag is NavTag parentTag &&
            parentTag == NavTag.BanksRoot)
        {
            ShowBankDetails(bankId);
            SetStatus($"Bank: {e.Node.Text}");
            return;
        }

        // Bank node under Accounts root (behave the same as real bank nodes)
        if (e.Node.Tag is string bankId2 &&
            e.Node.Parent?.Tag is NavTag parentTag2 &&
            parentTag2 == NavTag.AccountsRoot)
        {
            ShowBankDetails(bankId2);
            SetStatus($"Bank: {e.Node.Text}");
            return;
        }

        // Account leaf under Accounts root
        if (e.Node.Tag is string accountId &&
            e.Node.Parent?.Parent?.Tag is NavTag rootTag &&
            rootTag == NavTag.AccountsRoot)
        {
            ShowAccountDetails(accountId);
            SetStatus($"Account: {e.Node.Text}");
        }
    }

    private void ShowBanksLanding()
    {
        var form = new Form { Text = "Banks", FormBorderStyle = FormBorderStyle.None };

        var label = new Label
        {
            Dock = DockStyle.Fill,
            TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
            Text = "Select a bank to view/edit details.\n(Right-click Banks to add a new one.)\n(Right-click a bank to add an account.)"
        };

        form.Controls.Add(label);
        ShowChildForm(form);
    }

    private void ShowAccountsList()
    {
        var form = new AccountsListForm(OpenAccountDetailsFromList);
        form.SetAccounts(_accountCache);
        ShowChildForm(form);
    }

    private void ShowBudgetsLanding()
    {
        var form = new BudgetsLandingForm();
        ShowChildForm(form);
    }

    private void ShowBudgetTemplate()
    {
        var form = new BudgetTemplateForm();
        ShowChildForm(form);
    }

    private void ShowBudgetMonth(int year, int month)
    {
        var form = new BudgetMonthForm(year, month);
        ShowChildForm(form);
    }

    private void OpenAccountDetailsFromList(string accountId)
    {
        TrySelectAccountNode(accountId);
        ShowAccountDetails(accountId);
        SetStatus("Account (from list)");
    }

    private void TrySelectAccountNode(string accountId)
    {
        if (string.IsNullOrWhiteSpace(accountId))
            return;

        foreach (TreeNode root in treeNav.Nodes)
        {
            if (root.Tag is not NavTag tag || tag != NavTag.AccountsRoot)
                continue;

            foreach (TreeNode bankGroup in root.Nodes)
            {
                foreach (TreeNode acct in bankGroup.Nodes)
                {
                    if (acct.Tag is string id && id == accountId)
                    {
                        treeNav.SelectedNode = acct;
                        return;
                    }
                }
            }
        }
    }

    private void ShowBankDetails(string bankId)
    {
        var bank = _banks.GetById(bankId);
        if (bank is null)
        {
            MessageBox.Show(this, "Bank not found.", "Codex Expensa", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var form = new BankDetailsForm(_banks, _creds, OnDataChanged);
        form.LoadBank(bank);
        ShowChildForm(form);
    }

    private void ShowAccountDetails(string accountId)
    {
        var acct = _accounts.GetById(accountId);
        if (acct is null)
        {
            MessageBox.Show(this, "Account not found.", "Codex Expensa", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var form = new AccountDetailsForm(
            _accounts,
            _bankCache,
            _creds,
            _transactions,
            _payees,
            OnDataChanged);

        form.LoadAccount(acct);
        ShowChildForm(form); // <-- this was missing; without it, nothing shows.
    }

    private void OnDataChanged()
    {
        RefreshCaches();
        BuildNavigationTree();

        // Optional: keep the current view stable if you're on Accounts root
        if (treeNav.SelectedNode?.Tag is NavTag tag && tag == NavTag.AccountsRoot)
            ShowAccountsList();
    }

    private void ShowChildForm(Form child)
    {
        if (child is null) throw new ArgumentNullException(nameof(child));

        if (_activeChildForm is not null)
        {
            try { _activeChildForm.Close(); } catch { }
            try { _activeChildForm.Dispose(); } catch { }
        }

        _activeChildForm = child;

        child.TopLevel = false;
        child.FormBorderStyle = FormBorderStyle.None;
        child.Dock = DockStyle.Fill;

        panelHost.Controls.Clear();
        panelHost.Controls.Add(child);

        child.Show();
        child.BringToFront();
    }

    private ContextMenuStrip BuildBanksRootMenu()
    {
        var menu = new ContextMenuStrip();

        var add = new ToolStripMenuItem("Add Bank");
        add.Click += (_, _) => AddBank();
        menu.Items.Add(add);

        return menu;
    }

    private ContextMenuStrip BuildAccountsRootMenu()
    {
        // Empty for now. “Add account” lives on Bank nodes.
        return new ContextMenuStrip();
    }

    private ContextMenuStrip BuildBudgetsRootMenu()
    {
        // No actions yet (we’ll add once budget repo + screens exist)
        return new ContextMenuStrip();
    }

    private ContextMenuStrip BuildBankNodeMenu()
    {
        var menu = new ContextMenuStrip();

        var addAcct = new ToolStripMenuItem("Add Checking Account");
        addAcct.Click += (_, _) => AddCheckingAccount(preselectedBankId: GetSelectedBankIdFromTree());
        menu.Items.Add(addAcct);

        menu.Items.Add(new ToolStripSeparator());

        var del = new ToolStripMenuItem("Delete Bank");
        del.Click += (_, _) => DeleteSelectedBank();
        menu.Items.Add(del);

        return menu;
    }

    private ContextMenuStrip BuildAccountNodeMenu()
    {
        var menu = new ContextMenuStrip();

        var del = new ToolStripMenuItem("Delete Account");
        del.Click += (_, _) => DeleteSelectedAccount();
        menu.Items.Add(del);

        return menu;
    }

    private string? GetSelectedBankIdFromTree()
    {
        var node = treeNav.SelectedNode;
        return node?.Tag as string;
    }

    private void AddBank()
    {
        using var dlg = new AddBankForm();
        if (dlg.ShowDialog(this) != DialogResult.OK)
            return;

        var bank = new Bank
        {
            BankId = Guid.NewGuid().ToString("N"),
            BankName = dlg.BankName,
            RoutingNumber = dlg.RoutingNumber,
            Url = dlg.Url,
            IsActive = dlg.IsActive
        };

        try
        {
            _banks.Add(bank);

            // Note: only do this if your AddBankForm actually has Username/Password.
            // If not, remove these lines.
            var username = dlg.Username;
            var password = dlg.Password;

            if (!string.IsNullOrWhiteSpace(username) || !string.IsNullOrEmpty(password))
            {
                var key = CredentialKeys.Bank(bank.BankId);
                _creds.Save(key, username, password);
            }

            OnDataChanged();
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.ToString(), "Add Bank failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void AddCheckingAccount(string? preselectedBankId)
    {
        using var dlg = new AddCheckingAccountForm(_bankCache, preselectedBankId);
        if (dlg.ShowDialog(this) != DialogResult.OK)
            return;

        var nextSort = 0;
        foreach (var a in _accountCache)
            nextSort = Math.Max(nextSort, a.SortIndex);
        nextSort += 1;

        if (string.IsNullOrWhiteSpace(dlg.BankId))
        {
            MessageBox.Show(this, "No bank selected.", "Codex Expensa", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var bank = _bankCache.FirstOrDefault(b => b.BankId == dlg.BankId);
        if (bank is null)
        {
            MessageBox.Show(this, "Selected bank not found.", "Codex Expensa", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var acct = new Account
        {
            AccountId = Guid.NewGuid().ToString("N"),
            AccountNickname = dlg.AccountNickname,
            SortIndex = nextSort,
            AccountNumber = dlg.AccountNumber,

            BankId = bank.BankId,
            BankName = bank.BankName,
            RoutingNumber = bank.RoutingNumber,
            Url = bank.Url,

            AccountType = AccountType.Checking,
            IsActive = dlg.IsActive
        };

        try
        {
            _accounts.Add(acct);
            OnDataChanged();
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.ToString(), "Add Account failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void DeleteSelectedBank()
    {
        var node = treeNav.SelectedNode;
        if (node?.Tag is not string bankId)
            return;

        var confirm = MessageBox.Show(
            this,
            $"Delete bank:\n\n{node.Text}\n\nThis cannot be undone.",
            "Delete Bank",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning);

        if (confirm != DialogResult.Yes)
            return;

        try
        {
            var key = CredentialKeys.Bank(bankId);
            try { _creds.Delete(key); } catch { }

            _banks.Delete(bankId);
            OnDataChanged();
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "Delete Bank failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void DeleteSelectedAccount()
    {
        var node = treeNav.SelectedNode;
        if (node?.Tag is not string accountId)
            return;

        var confirm = MessageBox.Show(
            this,
            $"Delete account:\n\n{node.Text}\n\nThis cannot be undone.",
            "Delete Account",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning);

        if (confirm != DialogResult.Yes)
            return;

        try
        {
            _accounts.Delete(accountId);
            OnDataChanged();
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.ToString(), "Delete Account failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void UpdateDbStatus()
    {
        var path = _dbSession.PersistedFilePath ?? "(no file)";
        statusDbMode.Text = _dbSession.IsInMemory
            ? $"DB: Memory ({path})"
            : $"DB: File ({path})";
    }

    private void SetStatus(string message)
    {
        statusText.Text = string.IsNullOrWhiteSpace(message) ? "Ready" : message;
    }

    private void ShowError(string title, Exception ex)
    {
        SetStatus($"{title} (see dialog)");

        MessageBox.Show(
            this,
            ex.ToString(),
            title,
            MessageBoxButtons.OK,
            MessageBoxIcon.Error);
    }
}