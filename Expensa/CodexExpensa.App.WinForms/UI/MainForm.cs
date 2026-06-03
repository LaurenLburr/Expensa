using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using CodexExpensa.App.WinForms.Infrastructure;
using CodexExpensa.App.WinForms.UI.Accounts;
using CodexExpensa.App.WinForms.UI.Banks;
using CodexExpensa.App.WinForms.UI.Budgets;
using CodexExpensa.App.WinForms.UI.Websites;
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

    private Form? _activeChildForm;
    private readonly WebsiteAddinTreeLoader _websiteTreeLoader = new();

    private readonly ContextMenuStrip _ctxBanksRoot;
    private readonly ContextMenuStrip _ctxAccountsRoot;
    private readonly ContextMenuStrip _ctxBudgetsRoot;
    private readonly ContextMenuStrip _ctxBankNode;
    private readonly ContextMenuStrip _ctxAccountNode;

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
        _ctxBudgetsRoot = BuildBudgetsRootMenu();
        _ctxBankNode = BuildBankNodeMenu();
        _ctxAccountNode = BuildAccountNodeMenu();

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

    private async void MainForm_Load(object? sender, EventArgs e)
    {
        try
        {
            await LoadWebsiteAddinTreeAsync();
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

    private async void MenuViewRefresh_Click(object? sender, EventArgs e)
    {
        try
        {
            await LoadWebsiteAddinTreeAsync();
            SetStatus("Websites add-in refreshed.");
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
    }

    private void BuildNavigationTree()
    {
        _ = LoadWebsiteAddinTreeAsync();
    }

    private async Task LoadWebsiteAddinTreeAsync()
    {
        try
        {
            SetStatus("Loading Websites add-in...");

            WebsiteTreeLoadResult result =
                await _websiteTreeLoader.LoadIntoTreeViewAsync(
                    treeNav,
                    new WebsiteTreeLoadOptions
                    {
                        SearchText = string.Empty,
                        IncludeDisabled = false,
                        MaximumRows = 500,
                        ExpandAll = false
                    });

            if (treeNav.Nodes.Count > 0 && treeNav.SelectedNode is null)
            {
                treeNav.SelectedNode = treeNav.Nodes[0];
            }

            SetStatus($"Websites add-in loaded: {result.TotalCount} website row(s). {result.Message}");
        }
        catch (Exception ex)
        {
            treeNav.BeginUpdate();

            try
            {
                treeNav.Nodes.Clear();
                treeNav.Nodes.Add(
                    new TreeNode("Websites add-in failed")
                    {
                        Tag = null
                    });
            }
            finally
            {
                treeNav.EndUpdate();
            }

            SetStatus("Websites add-in failed.");
            ShowError("Websites add-in failed", ex);
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

        if (WebsiteTreeNodeTagReader.TryReadPayload(e.Node, out var websitePayload) &&
            websitePayload is not null)
        {
            ShowWebsiteNode(websitePayload);
            SetStatus($"Websites: {websitePayload.DisplayText}");
            return;
        }

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

                case NavTag.BudgetsRoot:
                    _ctxBudgetsRoot.Show(treeNav, e.Location);
                    return;
            }
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

    private void ShowWebsiteNode(IHostWebsiteTreeNodePayload payload)
    {
        ArgumentNullException.ThrowIfNull(payload);

        var form = new WebsiteDetailsForm(payload);
        ShowChildForm(form);
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

    private void ShowBudgetsLanding()
    {
        var form = new BudgetsLandingForm();
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
        var form = new BudgetTemplateForm();
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
}