using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using CodexExpensa.Core.Abstractions;
using CodexExpensa.Core.Domain.Accounts;
using CodexExpensa.Core.Domain.Banks;
using CodexExpensa.App.WinForms.UI.Accounts;
using CodexExpensa.App.WinForms.UI.Banks;

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

    private readonly ContextMenuStrip _ctxAccountsRoot;
    private readonly ContextMenuStrip _ctxBanksRoot;
    private readonly ContextMenuStrip _ctxAccountNode;
    private readonly ContextMenuStrip _ctxBankNode;

    private enum NavTag
    {
        AccountsRoot,
        BanksRoot
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

        _ctxAccountsRoot = BuildAccountsRootMenu();
        _ctxBanksRoot = BuildBanksRootMenu();
        _ctxAccountNode = BuildAccountNodeMenu();
        _ctxBankNode = BuildBankNodeMenu();

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
            "Expensa\n\nDocked subforms edition.\n(Still not doing your taxes. Calm down.)",
            "About Expensa",
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

            // Accounts root -> grouped by BankName
            var accountsRoot = new TreeNode("Accounts") { Tag = NavTag.AccountsRoot };

            foreach (var bankGroup in _accountCache
                         .OrderBy(a => a.BankName)
                         .ThenBy(a => a.SortIndex)
                         .ThenBy(a => a.AccountNickname)
                         .GroupBy(a => a.BankName))
            {
                var bankNode = new TreeNode(bankGroup.Key); // bank grouping node (no Tag)

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

        // Right click: select node and show menu
        if (e.Button == MouseButtons.Right)
        {
            treeNav.SelectedNode = e.Node;

            var menu = GetContextMenuForNode(e.Node);
            if (menu is not null)
                menu.Show(treeNav, e.Location);
        }
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

        // Bank leaf nodes under Banks root have Tag = bankId
        if (node.Tag is string && node.Parent?.Tag is NavTag parentTag && parentTag == NavTag.BanksRoot)
            return _ctxBankNode;

        // Account leaf nodes have Tag = accountId (under Accounts->BankGroup)
        if (node.Tag is string && node.Parent is not null && node.Parent.Parent?.Tag is NavTag p2 && p2 == NavTag.AccountsRoot)
            return _ctxAccountNode;

        return null;
    }

    private void TreeNav_AfterSelect(object? sender, TreeViewEventArgs e)
    {
        if (e.Node is null)
            return;

        // Root selections show "list" views (optional; here we keep it simple)
        if (e.Node.Tag is NavTag tag)
        {
            if (tag == NavTag.BanksRoot)
            {
                ShowBanksList();
                SetStatus("Banks");
                return;
            }

            if (tag == NavTag.AccountsRoot)
            {
                ShowAccountsList(null);
                SetStatus("Accounts");
                return;
            }
        }

        // Bank leaf
        if (e.Node.Tag is string bankId && e.Node.Parent?.Tag is NavTag parentTag && parentTag == NavTag.BanksRoot)
        {
            ShowBankDetails(bankId);
            SetStatus($"Bank: {e.Node.Text}");
            return;
        }

        // Account leaf
        if (e.Node.Tag is string accountId && e.Node.Parent?.Parent?.Tag is NavTag rootTag && rootTag == NavTag.AccountsRoot)
        {
            ShowAccountDetails(accountId);
            SetStatus($"Account: {e.Node.Text}");
        }
    }

    private void ShowBanksList()
    {
        // Simple placeholder list: reuse BankDetails for first bank or show empty panel
        var form = new Form
        {
            Text = "Banks",
            FormBorderStyle = FormBorderStyle.None
        };

        var label = new Label
        {
            Dock = DockStyle.Fill,
            TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
            Text = "Select a bank to view/edit details.\n(Right-click Banks to add a new one.)"
        };

        form.Controls.Add(label);
        ShowChildForm(form);
    }

    private void ShowAccountsList(string? selectedAccountId)
    {
        if (_activeChildForm is AccountsListForm existing)
        {
            existing.SetAccounts(_accountCache);
            if (!string.IsNullOrWhiteSpace(selectedAccountId))
                existing.SelectAccount(selectedAccountId);
            return;
        }

        var form = new AccountsListForm(_accounts, OnDataChanged);
        form.SetAccounts(_accountCache);

        if (!string.IsNullOrWhiteSpace(selectedAccountId))
            form.SelectAccount(selectedAccountId);

        ShowChildForm(form);
    }

    private void ShowBankDetails(string bankId)
    {
        var bank = _banks.GetById(bankId);
        if (bank is null)
        {
            MessageBox.Show(this, "Bank not found.", "Expensa", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
            MessageBox.Show(this, "Account not found.", "Expensa", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var form = new AccountDetailsForm(_accounts, OnDataChanged);
        form.LoadAccount(acct);
        ShowChildForm(form);
    }

    private void OnDataChanged()
    {
        RefreshCaches();
        BuildNavigationTree();
    }

    private void ShowChildForm(Form child)
    {
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
        using var dlg = new AddCheckingAccountForm();
        if (dlg.ShowDialog(this) != DialogResult.OK)
            return;

        var nextSort = 0;
        foreach (var a in _accountCache)
            nextSort = Math.Max(nextSort, a.SortIndex);
        nextSort += 1;

        var acct = new Account
        {
            AccountId = Guid.NewGuid().ToString("N"),
            AccountNickname = dlg.AccountNickname,
            SortIndex = nextSort,
            AccountNumber = dlg.AccountNumber,

            BankId = string.Empty,
            BankName = dlg.BankName,
            RoutingNumber = dlg.RoutingNumber,
            Url = dlg.Url,

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