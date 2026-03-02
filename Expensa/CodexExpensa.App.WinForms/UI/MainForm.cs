using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using CodexExpensa.Core.Abstractions;
using CodexExpensa.Core.Domain.Accounts;
using CodexExpensa.Core.Domain.Banks;
using CodexExpensa.App.WinForms.UI.Accounts;
using CodexExpensa.App.WinForms.UI.Banks;
using CodexExpensa.App.WinForms.Infrastructure;

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
    private readonly ContextMenuStrip _ctxBankNode;
    private readonly ContextMenuStrip _ctxAccountNode;
    private readonly ICredentialStore _creds = new WindowsCredentialStore();


    private enum NavTag
    {
        BanksRoot,
        AccountsRoot
    }

    public MainForm(
        IDatabaseSession dbSession,
        IAccountRepository accounts,
        IBankRepository banks,
        Func<Form> createDbStatusForm)
    {
        _dbSession = dbSession ?? throw new ArgumentNullException(nameof(dbSession));
        _accounts = accounts ?? throw new ArgumentNullException(nameof(accounts));
        _banks = banks ?? throw new ArgumentNullException(nameof(banks));
        _createDbStatusForm = createDbStatusForm ?? throw new ArgumentNullException(nameof(createDbStatusForm));

        InitializeComponent();

        _ctxBanksRoot = BuildBanksRootMenu();
        _ctxAccountsRoot = BuildAccountsRootMenu();
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
            "Codex Expensa\n\nDocked subforms edition.\n(Still not doing your taxes. Calm down.)",
            "About Codex Expensa",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
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

            // Accounts root: Accounts | Bank | Nickname - last4
            var accountsRoot = new TreeNode("Accounts") { Tag = NavTag.AccountsRoot };

            foreach (var bankGroup in _accountCache
                         .OrderBy(a => a.BankName)
                         .ThenBy(a => a.SortIndex)
                         .ThenBy(a => a.AccountNickname)
                         .GroupBy(a => a.BankName))
            {
                var bankNode = new TreeNode(bankGroup.Key);

                foreach (var acct in bankGroup)
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
        }
        finally
        {
            treeNav.EndUpdate();
        }
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
                _ => null
            };
        }

        // Bank node: parent is BanksRoot
        if (node.Tag is string && node.Parent?.Tag is NavTag parentTag && parentTag == NavTag.BanksRoot)
            return _ctxBankNode;

        // Account node: (AccountsRoot) -> (BankGroup) -> (Account leaf)
        if (node.Tag is string && node.Parent is not null && node.Parent.Parent?.Tag is NavTag p2 && p2 == NavTag.AccountsRoot)
            return _ctxAccountNode;

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
            }
        }

        // Bank leaf
        if (e.Node.Tag is string bankId &&
            e.Node.Parent?.Tag is NavTag parentTag &&
            parentTag == NavTag.BanksRoot)
        {
            ShowBankDetails(bankId);
            SetStatus($"Bank: {e.Node.Text}");
            return;
        }

        // Account leaf
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
            Text = "Select a bank to view/edit details.\n(Right-click Banks to add a new one.)"
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

        var form = new BankDetailsForm(_banks, OnDataChanged);
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

        // ✅ FIX: pass credential store into AccountDetailsForm
        var form = new AccountDetailsForm(_accounts, _bankCache, _creds, OnDataChanged);
        form.LoadAccount(acct);
        ShowChildForm(form);
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
        var menu = new ContextMenuStrip();

        var add = new ToolStripMenuItem("Add Checking Account");
        add.Click += (_, _) => AddCheckingAccount();
        menu.Items.Add(add);

        return menu;
    }

    private ContextMenuStrip BuildBankNodeMenu()
    {
        var menu = new ContextMenuStrip();

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
            OnDataChanged();
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.ToString(), "Add Bank failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void AddCheckingAccount()
    {
        using var dlg = new AddCheckingAccountForm(_bankCache);
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