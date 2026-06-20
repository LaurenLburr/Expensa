using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using CodexExpensa.App.WinForms.Infrastructure;
using CodexExpensa.App.WinForms.UI.Accounts;
using CodexExpensa.App.WinForms.UI.Addins;
using CodexExpensa.App.WinForms.UI.Banks;
using CodexExpensa.App.WinForms.UI.Budgets;
using CodexExpensa.App.WinForms.UI.Payees;
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
    private readonly TreeAddinTreeViewLoader _treeAddinLoader = new();
    private readonly AddinScreenRuntimeInvoker _addinScreenInvoker = new();
    private readonly WebsiteTagAssignmentService _websiteTagAssignmentService = new();

    private readonly ContextMenuStrip _ctxBanksRoot;
    private readonly ContextMenuStrip _ctxAccountsRoot;
    private readonly ContextMenuStrip _ctxBudgetsRoot;
    private readonly ContextMenuStrip _ctxBankNode;
    private readonly ContextMenuStrip _ctxAccountNode;
    private readonly ContextMenuStrip _ctxWebsiteNode;
    private ToolStripDropDown? _websiteTagPickerDropDown;

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
        _ctxWebsiteNode = BuildWebsiteNodeMenu();

        Load += MainForm_Load;

        treeNav.ShowNodeToolTips = true;
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
            await LoadTreeAddinsAsync();
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
            await LoadTreeAddinsAsync();
            SetStatus("Tree add-ins refreshed.");
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
        _ = LoadTreeAddinsAsync();
    }

    private async Task LoadTreeAddinsAsync()
    {
        try
        {
            SetStatus("Loading tree add-ins...");

            TreeAddinAggregateLoadResult result =
                await _treeAddinLoader.LoadAllIntoTreeViewAsync(
                    treeNav,
                    new TreeAddinLoadOptions
                    {
                        SearchText = string.Empty,
                        IncludeInactive = false,
                        MaximumRows = 500,
                        ExpandAll = false
                    });

            if (treeNav.Nodes.Count > 0 && treeNav.SelectedNode is null)
            {
                treeNav.SelectedNode = treeNav.Nodes[0];
            }

            SetStatus($"Tree add-ins loaded: {result.LoadedAddinCount} loaded, {result.FailedAddinCount} failed. {result.Message}");
        }
        catch (Exception ex)
        {
            treeNav.BeginUpdate();

            try
            {
                treeNav.Nodes.Clear();
                treeNav.Nodes.Add(
                    new TreeNode("Tree add-ins failed")
                    {
                        Tag = null
                    });
            }
            finally
            {
                treeNav.EndUpdate();
            }

            SetStatus("Tree add-ins failed.");
            ShowError("Tree add-ins failed", ex);
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

    private async void TreeNav_AfterSelect(object? sender, TreeViewEventArgs e)
    {
        if (e.Node is null)
            return;

        if (WebsiteTreeNodeTagReader.TryReadPayload(e.Node, out var websitePayload) &&
            websitePayload is not null)
        {
            await ShowAddinScreenAsync(
                TreeAddinKind.Websites,
                new AddinScreenSelection
                {
                    NodeId = websitePayload.NodeId,
                    NodeType = websitePayload.NodeType.ToString(),
                    EntityId = websitePayload.WebsiteId,
                    DisplayText = websitePayload.DisplayText,
                    Url = websitePayload.Url,
                    Category = websitePayload.TagName,
                    IsActive = websitePayload.IsActive
                });

            SetStatus($"Websites: {websitePayload.DisplayText}");
            return;
        }

        if (e.Node.Tag is BudgetTreeNodePayload budgetPayload)
        {
            await ShowAddinScreenAsync(
                TreeAddinKind.Budgets,
                new AddinScreenSelection
                {
                    NodeId = budgetPayload.NodeId,
                    NodeType = budgetPayload.NodeType,
                    DisplayText = budgetPayload.DisplayText,
                    Year = budgetPayload.Year,
                    Month = budgetPayload.Month
                });

            SetStatus($"Budgets: {budgetPayload.DisplayText}");
            return;
        }

        if (e.Node.Tag is PayeeTreeNodePayload payeePayload)
        {
            await ShowAddinScreenAsync(
                TreeAddinKind.Payees,
                new AddinScreenSelection
                {
                    NodeId = e.Node.Name,
                    NodeType = payeePayload.NodeType,
                    EntityId = payeePayload.PayeeId,
                    DisplayText = payeePayload.DisplayText
                });

            SetStatus(
                string.IsNullOrWhiteSpace(payeePayload.DisplayText)
                    ? "Payees"
                    : $"Payees: {payeePayload.DisplayText}");
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

        if (IsWebsiteNavigationNode(e.Node))
        {
            _ctxWebsiteNode.Show(treeNav, e.Location);
            return;
        }

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

    private ContextMenuStrip BuildWebsiteNodeMenu()
    {
        var menu = new ContextMenuStrip();

        var assignTag = new ToolStripMenuItem("Assign Tag...");
        assignTag.Click += (_, _) => ShowWebsiteTagPicker();

        var removeTag = new ToolStripMenuItem("Remove Tag Association");
        removeTag.Click += async (_, _) => await RemoveSelectedWebsiteTagAssociationsAsync();

        menu.Opening += (_, e) =>
        {
            bool isWebsiteNode =
                TryGetSelectedWebsiteForTagging(out WebsiteTagTarget? target) &&
                target is not null;

            assignTag.Enabled = isWebsiteNode;
            removeTag.Enabled =
                isWebsiteNode &&
                target is not null &&
                !string.IsNullOrWhiteSpace(target.TagName) &&
                !string.Equals(target.TagName, "Uncategorized", StringComparison.OrdinalIgnoreCase);

            e.Cancel = treeNav.SelectedNode is null ||
                !IsWebsiteNavigationNode(treeNav.SelectedNode);
        };

        menu.Items.Add(assignTag);
        menu.Items.Add(removeTag);

        return menu;
    }

    private void ShowWebsiteTagPicker()
    {
        if (!TryGetSelectedWebsiteForTagging(out WebsiteTagTarget? target) ||
            target is null)
        {
            return;
        }

        CloseWebsiteTagPicker();

        WebsiteTagPickerPanel picker = new();
        picker.SetTags(GetUnassignedWebsiteTagNames(target.WebsiteId));
        picker.TypedTagAccepted += WebsiteTagPicker_TagAccepted;
        picker.ListedTagAccepted += WebsiteTagPicker_TagAccepted;

        ToolStripControlHost host = new(picker)
        {
            AutoSize = false,
            Size = picker.Size,
            Margin = Padding.Empty,
            Padding = Padding.Empty
        };

        _websiteTagPickerDropDown = new ToolStripDropDown
        {
            Padding = Padding.Empty
        };
        _websiteTagPickerDropDown.Items.Add(host);
        _websiteTagPickerDropDown.Closed += (_, _) => _websiteTagPickerDropDown = null;

        Rectangle nodeBounds = treeNav.SelectedNode?.Bounds ?? Rectangle.Empty;
        Point location = new(nodeBounds.Left, nodeBounds.Bottom);
        _websiteTagPickerDropDown.Show(treeNav, location);
        picker.FocusTextBox();
    }

    private IReadOnlyList<string> GetUnassignedWebsiteTagNames(string websiteId)
    {
        HashSet<string> assignedTags =
            new(_websiteTagAssignmentService.GetAssignedTagNames(websiteId), StringComparer.CurrentCultureIgnoreCase);

        return _websiteTagAssignmentService.GetActiveTagNames()
            .Where(tagName => !assignedTags.Contains(tagName))
            .ToList();
    }

    private async void WebsiteTagPicker_TagAccepted(object? sender, string tagName)
    {
        await AssignTagToSelectedWebsiteAsync(tagName);
    }

    private async Task AssignTagToSelectedWebsiteAsync(string tagName)
    {
        if (!TryGetSelectedWebsiteForTagging(out WebsiteTagTarget? target) ||
            target is null)
        {
            return;
        }

        try
        {
            string assignedTag =
                _websiteTagAssignmentService.AssignTagToWebsite(target.WebsiteId, tagName);

            CloseWebsiteTagPicker();
            await LoadTreeAddinsAsync();
            SetStatus($"Assigned tag '{assignedTag}' to {target.DisplayText}.");
        }
        catch (Exception ex)
        {
            ShowError("Assign website tag failed", ex);
        }
    }

    private async Task RemoveSelectedWebsiteTagAssociationsAsync()
    {
        if (!TryGetSelectedWebsiteForTagging(out WebsiteTagTarget? target) ||
            target is null)
        {
            return;
        }

        try
        {
            int removedCount =
                _websiteTagAssignmentService.RemoveWebsiteTagAssociations(target.WebsiteId);

            await LoadTreeAddinsAsync();
            SetStatus(removedCount == 0
                ? $"{target.DisplayText} had no tag association to remove."
                : $"Removed tag association from {target.DisplayText}.");
        }
        catch (Exception ex)
        {
            ShowError("Remove website tag association failed", ex);
        }
    }

    private void CloseWebsiteTagPicker()
    {
        if (_websiteTagPickerDropDown is null)
        {
            return;
        }

        ToolStripDropDown dropDown = _websiteTagPickerDropDown;
        _websiteTagPickerDropDown = null;
        dropDown.Close();
        dropDown.Dispose();
    }

    private bool TryGetSelectedWebsiteForTagging(out WebsiteTagTarget? target)
    {
        target = null;

        TreeNode? selectedNode = treeNav.SelectedNode;

        if (selectedNode is null ||
            !IsWebsiteNavigationNode(selectedNode))
        {
            return false;
        }

        if (WebsiteTreeNodeTagReader.TryReadPayload(selectedNode, out IHostWebsiteTreeNodePayload? payload) &&
            payload is not null &&
            payload.NodeType == HostWebsiteTreeNodeType.Website &&
            !string.IsNullOrWhiteSpace(payload.WebsiteId))
        {
            target = new WebsiteTagTarget(
                payload.WebsiteId,
                payload.DisplayText,
                payload.TagName);

            return true;
        }

        if (selectedNode.Nodes.Count == 0 &&
            !string.IsNullOrWhiteSpace(selectedNode.Name))
        {
            target = new WebsiteTagTarget(
                selectedNode.Name,
                selectedNode.Text,
                selectedNode.Parent?.Text ?? string.Empty);

            return true;
        }

        return false;
    }

    private static bool IsWebsiteNavigationNode(TreeNode node)
    {
        for (TreeNode? current = node; current is not null; current = current.Parent)
        {
            if (string.Equals(current.Name, "addin.websites", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(current.Text, "Websites", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }

    private sealed record WebsiteTagTarget(
        string WebsiteId,
        string DisplayText,
        string TagName);

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

    private async Task ShowAddinScreenAsync(
        TreeAddinKind kind,
        AddinScreenSelection selection)
    {
        TreeAddinDefinition definition =
            TreeAddinDefinition.DefaultTreeAddins()
                .Single(item => item.Kind == kind);

        try
        {
            SetStatus($"Loading {selection.DisplayText}...");

            AddinScreenRuntimeResult result =
                await _addinScreenInvoker.ExecuteAsync(
                    definition,
                    selection);

            if (!result.Succeeded)
            {
                throw new InvalidOperationException(
                    string.IsNullOrWhiteSpace(result.Message)
                        ? $"{definition.AddinName} screen provider failed."
                        : result.Message);
            }

            ShowChildForm(
                new AddinScreenForm(
                    result.Screen,
                    result.AssemblyPath,
                    result.AssemblyLastWriteTimeUtc,
                    assemblyVersion: result.AssemblyVersion,
                    deployedUtc: result.DeployedUtc,
                    accounts: _accounts,
                    payees: _payees,
                    transactions: _transactions,
                    selection: selection,
                    saveDatabase: _dbSession.Save,
                    reloadAsync: () =>
                        ShowAddinScreenAsync(
                            kind,
                            selection)));
        }
        catch (Exception exception)
        {
            ShowChildForm(
                AddinScreenForm.CreateFailure(
                    selection.DisplayText,
                    exception));
        }
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

    private void ShowPayees()
    {
        var form = new PayeesForm(_payees);
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
