using CodexExpensa.Core.Domain.Accounts;
using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Windows.Forms;

namespace CodexExpensa.App.WinForms.UI.Accounts;

public sealed class AccountsListForm : Form
{
    private readonly IAccountRepository _repo;
    private readonly Action _onAccountsChanged;

    private IReadOnlyList<Account> _accounts = Array.Empty<Account>();

    private readonly ToolStrip _tool;
    private readonly ToolStripButton _btnAddChecking;
    private readonly DataGridView _grid;

    public AccountsListForm(IAccountRepository repo, Action onAccountsChanged)
    {
        _repo = repo ?? throw new ArgumentNullException(nameof(repo));
        _onAccountsChanged = onAccountsChanged ?? throw new ArgumentNullException(nameof(onAccountsChanged));

        Text = "Accounts";
        FormBorderStyle = FormBorderStyle.None;

        _tool = new ToolStrip { Dock = DockStyle.Top };
        _btnAddChecking = new ToolStripButton("Add Checking");
        _btnAddChecking.Click += (_, _) => AddCheckingAccount();
        _tool.Items.Add(_btnAddChecking);

        _grid = new DataGridView
        {
            Dock = DockStyle.Fill,
            ReadOnly = true,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            AllowUserToOrderColumns = true,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            RowHeadersVisible = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            MultiSelect = false
        };

        Controls.Add(_grid);
        Controls.Add(_tool);
    }

    public void SetAccounts(IReadOnlyList<Account> accounts)
    {
        _accounts = accounts ?? Array.Empty<Account>();
        _grid.DataSource = _accounts;

        if (_grid.Columns.Contains(nameof(Account.AccountId)))
            _grid.Columns[nameof(Account.AccountId)].Visible = false;

        if (_grid.Columns.Contains(nameof(Account.BankId)))
            _grid.Columns[nameof(Account.BankId)].Visible = false;
    }

    public void SelectAccount(string accountId)
    {
        if (string.IsNullOrWhiteSpace(accountId))
            return;

        for (var i = 0; i < _accounts.Count; i++)
        {
            if (_accounts[i].AccountId == accountId)
            {
                _grid.ClearSelection();
                if (i < _grid.Rows.Count)
                {
                    _grid.Rows[i].Selected = true;
                    _grid.FirstDisplayedScrollingRowIndex = i;
                }
                break;
            }
        }
    }

    private void AddCheckingAccount()
    {
        using var dlg = new AddCheckingAccountForm();
        if (dlg.ShowDialog(this) != DialogResult.OK)
            return;

        var nextSort = 0;
        foreach (var a in _accounts)
            nextSort = Math.Max(nextSort, a.SortIndex);
        nextSort += 1;

        var newAccount = new Account
        {
            AccountId = Guid.NewGuid().ToString("N"),

            AccountNickname = dlg.AccountNickname,
            SortIndex = nextSort,
            AccountNumber = dlg.AccountNumber,

            // BankId is resolved in the sqlite repo (GetOrCreate Bank)
            BankId = string.Empty,

            BankName = dlg.BankName,
            RoutingNumber = dlg.RoutingNumber,
            Url = dlg.Url,

            AccountType = AccountType.Checking,
            IsActive = dlg.IsActive
        };

        try
        {
            _repo.Add(newAccount);
            _onAccountsChanged();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                this,
                ex.ToString(),
                "Add Checking Account failed",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }
}