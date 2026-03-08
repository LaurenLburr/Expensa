using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using CodexExpensa.App.WinForms.Infrastructure;
using CodexExpensa.App.WinForms.UI.Accounts;
using CodexExpensa.App.WinForms.UI.Banks;
using CodexExpensa.App.WinForms.UI.Budgets;
using CodexExpensa.App.WinForms.UI.Payees;
using CodexExpensa.Core.Abstractions;
using CodexExpensa.Core.Domain.Accounts;
using CodexExpensa.Core.Domain.Banks;
using CodexExpensa.Core.Domain.Payees;
using CodexExpensa.Core.Domain.Transactions;

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

    private enum NavTag
    {
        BanksRoot,
        AccountsRoot,
        PayeesRoot,
        BudgetsRoot
    }

    // Budget node ids (string tags)
    private static class BudgetNav
    {
        public const string Template = "Budget.Template";
        public static string Current(int year, int month) => $"Budget.Current.{year:D4}.{month:D2}";
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

        public static bool TryParseCurrent(string id, out int year, out int month)
        {
            year = 0;
            month = 0;

            // Budget.Current.2026.03
            if (string.IsNullOrWhiteSpace(id))
                return false;

            var parts = id.Split('.');
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

        // Designer wires menuHelpAbout_Click_1; keep both safe.
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
        MessageBox.Show(
            this,
            "CodexExpensa\n\nA checkbook-style tracker.\n\n(Still under active development.)",
            "About",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
    }

    // Designer wires this method name.
    private void menuHelpAbout_Click_1(object sender, EventArgs e)
        => MenuHelpAbout_Click(sender, e);

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

            // Banks root
            var banksRoot = new TreeNode("Banks") { Tag = NavTag.BanksRoot };
            foreach (var b in _bankCache.OrderBy(b => b.BankName).ThenBy(b => b.RoutingNumber))
            {
                var label = $"{b.BankName} ({b.RoutingNumber})";
                banksRoot.Nodes.Add(new TreeNode(label) { Tag = b.BankId });
            }
            //banksRoot.Expand();
            treeNav.Nodes.Add(banksRoot);

            // Accounts root: Accounts -> Bank (tag=bankId) -> Account leaf (tag=accountId)
            var accountsRoot = new TreeNode("Accounts") { Tag = NavTag.AccountsRoot };

            var bankAccounts = new Dictionary<string, List<Account>>(StringComparer.Ordinal);
            foreach (var acct in _accountCache)
            {
                var bankId = acct.BankId;
                if (string.IsNullOrWhiteSpace(bankId))
                    continue;

                if (!bankAccounts.TryGetValue(bankId, out var list))
                {
                    list = new List<Account>();
                    bankAccounts[bankId] = list;
                }

                list.Add(acct);
            }

            foreach (var kvp in bankAccounts.OrderBy(k => GetBankDisplayName(k.Key), StringComparer.OrdinalIgnoreCase))
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

                //bankNode.Expand();
                accountsRoot.Nodes.Add(bankNode);
            }

            //accountsRoot.Expand();
            treeNav.Nodes.Add(accountsRoot);

            // Payees root
            var payeesRoot = new TreeNode("Payees") { Tag = NavTag.PayeesRoot };
            foreach (var p in _payeeCache
                         .OrderByDescending(p => p.IncludeInBudgetTemplate)
                         .ThenBy(p => p.PayeeName))
            {
                var label = p.IncludeInBudgetTemplate ? $"{p.PayeeName}  ✓" : p.PayeeName;
                payeesRoot.Nodes.Add(new TreeNode(label) { Tag = p.PayeeId });
            }
            //payeesRoot.Expand();
            treeNav.Nodes.Add(payeesRoot);

            // Budgets root
            var budgetsRoot = new TreeNode("Budgets") { Tag = NavTag.BudgetsRoot };

            // Template node immediately under Budgets
            budgetsRoot.Nodes.Add(new TreeNode("Template") { Tag = BudgetNav.Template });

            // Current node immediately under Budgets
            var today = DateTime.Today;
            budgetsRoot.Nodes.Add(new TreeNode($"Current ({today:MMM/yyyy})")
            {
                Tag = BudgetNav.Current(today.Year, today.Month)
            });

            // Years and months scaffold
            var currentYear = today.Year;
            var currentMonth = today.Month;

            // Keep a simple rolling range for now.
            var years = Enumerable.Range(currentYear - 4, 5).Reverse().ToList();

            foreach (var year in years)
            {
                var yearNode = new TreeNode(year.ToString()) { Tag = BudgetNav.Year(year) };

                List<int> months;

                if (year == currentYear)
                {
                    // Current year: only months before Current.
                    var count = Math.Max(0, currentMonth - 1);
                    months = Enumerable.Range(1, count).ToList();
                    months.Reverse(); // show most recent first
                }
                else
                {
                    // Past years: show all months Jan..Dec
                    months = Enumerable.Range(1, 12).ToList();
                }

                foreach (var m in months)
                {
                    var monthName = new DateTime(year, m, 1).ToString("MMM");
                    yearNode.Nodes.Add(new TreeNode(monthName) { Tag = BudgetNav.Month(year, m) });
                }

                //yearNode.Expand();
                budgetsRoot.Nodes.Add(yearNode);
            }

            //budgetsRoot.Expand();
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

    private static string Last4(string accountNumber)
    {
        if (string.IsNullOrWhiteSpace(accountNumber))
            return "????";

        var cleaned = accountNumber.Replace(" ", "").Replace("-", "");
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

        if (e.Node.Tag is NavTag tag)
        {
            switch (tag)
            {
                case NavTag.BanksRoot:
                    SetStatus("Banks");
                    ShowBanksLanding();
                    return;

                case NavTag.AccountsRoot:
                    SetStatus("Accounts");
                    ShowAccountsList();
                    return;

                case NavTag.PayeesRoot:
                    SetStatus("Payees");
                    ShowPayeesLanding();
                    return;

                case NavTag.BudgetsRoot:
                    SetStatus("Budgets");
                    ShowBudgetsLanding();
                    return;
            }
        }

        // Budgets
        if (e.Node.Tag is string budgetTag)
        {
            if (string.Equals(budgetTag, BudgetNav.Template, StringComparison.OrdinalIgnoreCase))
            {
                ShowBudgetTemplate();
                SetStatus("Budgets: Template");
                return;
            }

            if (BudgetNav.TryParseCurrent(budgetTag, out var cy, out var cm))
            {
                ShowBudgetMonth(cy, cm);
                SetStatus($"Budgets: Current ({new DateTime(cy, cm, 1):MMM/yyyy})");
                return;
            }

            if (BudgetNav.TryParseMonth(budgetTag, out var year, out var month))
            {
                ShowBudgetMonth(year, month);
                SetStatus($"Budgets: {year}-{month:D2}");
                return;
            }
        }

        // Payee leaf under Payees root
        if (e.Node.Tag is string payeeId &&
            e.Node.Parent?.Tag is NavTag pRoot &&
            pRoot == NavTag.PayeesRoot)
        {
            ShowPayeeDetails(payeeId);
            SetStatus($"Payee: {e.Node.Text}");
            return;
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

        // Bank node under Accounts root
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

        // Payee node under Payees root
        // Payee node under Payees root (ignore Search node)
        if (e.Node.Tag is string payeeId &&
            payeeId != "Payee.Search" &&
            e.Node.Parent?.Tag is NavTag pr &&
            pr == NavTag.PayeesRoot)
        {
            _ctxPayeeNode.Show(treeNav, e.Location);
            return;
        }

        // Bank node under Banks root
        if (e.Node.Tag is string &&
            e.Node.Parent?.Tag is NavTag parentTag &&
            parentTag == NavTag.BanksRoot)
        {
            _ctxBankNode.Show(treeNav, e.Location);
            return;
        }

        // Bank node under Accounts root
        if (e.Node.Tag is string &&
            e.Node.Parent?.Tag is NavTag parentTag2 &&
            parentTag2 == NavTag.AccountsRoot)
        {
            _ctxBankNode.Show(treeNav, e.Location);
            return;
        }

        // Account leaf node under Accounts root
        if (e.Node.Tag is string &&
            e.Node.Parent?.Parent?.Tag is NavTag rootTag2 &&
            rootTag2 == NavTag.AccountsRoot)
        {
            _ctxAccountNode.Show(treeNav, e.Location);
        }
    }

    private ContextMenuStrip BuildBanksRootMenu()
    {
        var menu = new ContextMenuStrip();

        var add = new ToolStripMenuItem("Add Bank");
        add.Click += (_, _) =>
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

            _banks.Add(bank);
            RefreshCaches();
            BuildNavigationTree();
        };

        menu.Items.Add(add);
        return menu;
    }

    private ContextMenuStrip BuildAccountsRootMenu()
    {
        var menu = new ContextMenuStrip();

        var addChecking = new ToolStripMenuItem("Add Checking Account...");
        addChecking.Click += (_, _) =>
        {
            using var dlg = new AddCheckingAccountForm(_bankCache);
            if (dlg.ShowDialog(this) != DialogResult.OK)
                return;

            var bankId = dlg.BankId;
            if (string.IsNullOrWhiteSpace(bankId))
                return;

            var bank = _bankCache.FirstOrDefault(b => b.BankId == bankId);
            if (bank is null)
                return;

            var acct = new Account
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
        var menu = new ContextMenuStrip();

        var add = new ToolStripMenuItem("Add Payee...");
        add.Click += (_, _) =>
        {
            // Minimal add dialog (InputBox style) without building a separate form yet.
            var name = PromptForText(this, "Add Payee", "Payee name:");
            if (string.IsNullOrWhiteSpace(name))
                return;

            var payee = new Payee
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
        var menu = new ContextMenuStrip();
        return menu;
    }

    private ContextMenuStrip BuildBankNodeMenu()
    {
        var menu = new ContextMenuStrip();

        var addChecking = new ToolStripMenuItem("Add Checking Account...");
        addChecking.Click += (_, _) =>
        {
            var bankId = treeNav.SelectedNode?.Tag as string;
            if (string.IsNullOrWhiteSpace(bankId))
                return;

            using var dlg = new AddCheckingAccountForm(_bankCache, preselectedBankId: bankId);
            if (dlg.ShowDialog(this) != DialogResult.OK)
                return;

            var selectedBankId = dlg.BankId;
            if (string.IsNullOrWhiteSpace(selectedBankId))
                return;

            var bank = _bankCache.FirstOrDefault(b => b.BankId == selectedBankId);
            if (bank is null)
                return;

            var acct = new Account
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
        var menu = new ContextMenuStrip();

        var del = new ToolStripMenuItem("Delete Account");
        del.Click += (_, _) =>
        {
            var accountId = treeNav.SelectedNode?.Tag as string;
            if (string.IsNullOrWhiteSpace(accountId))
                return;

            var confirm = MessageBox.Show(
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
        var menu = new ContextMenuStrip();

        var del = new ToolStripMenuItem("Delete Payee");
        del.Click += (_, _) =>
        {
            var payeeId = treeNav.SelectedNode?.Tag as string;
            if (string.IsNullOrWhiteSpace(payeeId))
                return;

            var confirm = MessageBox.Show(
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

    private void ShowBanksLanding()
    {
        var form = new BanksLandingForm(openBankDetails: bankId =>
        {
            ShowBankDetails(bankId);
        });

        form.SetBanks(_bankCache);

        ShowChildForm(form);
    }

    private void ShowAccountsList()
    {
        var form = new AccountsListForm(openAccountDetails: accountId =>
        {
            ShowAccountDetails(accountId);
        });

        form.SetAccounts(_accountCache);

        ShowChildForm(form);
    }

    private void ShowPayeesLanding()
    {
        var form = new PayeesLandingForm(openPayeeDetails: payeeId =>
        {
            ShowPayeeDetails(payeeId);
        });

        form.SetPayees(_payeeCache);

        ShowChildForm(form);
    }

    private void ShowBudgetsLanding()
    {
        var form = new BudgetsLandingForm();
        ShowChildForm(form);
    }

    private void ShowPayeeDetails(string payeeId)
    {
        var payee = _payeeCache.FirstOrDefault(p => p.PayeeId == payeeId);
        if (payee is null)
        {
            MessageBox.Show(this, "Payee not found.", "Expensa", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var form = new PayeeDetailsForm(
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
        var bank = _bankCache.FirstOrDefault(b => b.BankId == bankId);
        if (bank is null)
        {
            MessageBox.Show(this, "Bank not found.", "Expensa", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var form = new BankDetailsForm(
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
        var acct = _accountCache.FirstOrDefault(a => a.AccountId == accountId);
        if (acct is null)
        {
            MessageBox.Show(this, "Account not found.", "Expensa", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var form = new AccountDetailsForm(
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
        var form = new BudgetTemplateForm(_payees);
        ShowChildForm(form);
    }

    private void ShowBudgetMonth(int year, int month)
    {
        var form = new BudgetMonthForm(year, month);
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
        using var form = new Form
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

        var lbl = new Label { Left = 12, Top = 16, AutoSize = true, Text = prompt };
        var txt = new TextBox { Left = 12, Top = 44, Width = 480, Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };
        var ok = new Button { Text = "OK", Left = 312, Width = 80, Top = 82, DialogResult = DialogResult.OK };
        var cancel = new Button { Text = "Cancel", Left = 412, Width = 80, Top = 82, DialogResult = DialogResult.Cancel };

        form.Controls.Add(lbl);
        form.Controls.Add(txt);
        form.Controls.Add(ok);
        form.Controls.Add(cancel);

        form.AcceptButton = ok;
        form.CancelButton = cancel;

        return form.ShowDialog(owner) == DialogResult.OK ? txt.Text.Trim() : null;
    }
}