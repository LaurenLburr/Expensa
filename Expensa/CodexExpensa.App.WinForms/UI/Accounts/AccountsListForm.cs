using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using CodexExpensa.Core.Domain.Accounts;

namespace CodexExpensa.App.WinForms.UI.Accounts;

public sealed class AccountsListForm : Form
{
    private readonly Action<string> _openAccountDetails;
    private IReadOnlyList<Account> _accounts = Array.Empty<Account>();

    private readonly ToolStrip _tool;
    private readonly ToolStripLabel _lbl;
    private readonly DataGridView _grid;

    public AccountsListForm(Action<string> openAccountDetails)
    {
        _openAccountDetails = openAccountDetails ?? throw new ArgumentNullException(nameof(openAccountDetails));

        Text = "Accounts";
        FormBorderStyle = FormBorderStyle.None;

        _tool = new ToolStrip { Dock = DockStyle.Top };
        _lbl = new ToolStripLabel("Double-click an account to edit.");
        _tool.Items.Add(_lbl);

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

        _grid.CellDoubleClick += (_, _) => OpenSelected();
        _grid.KeyDown += (_, e) =>
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                OpenSelected();
            }
        };

        Controls.Add(_grid);
        Controls.Add(_tool);
    }

    public void SetAccounts(IReadOnlyList<Account> accounts)
    {
        _accounts = accounts ?? Array.Empty<Account>();

        // Bind a projection for nicer columns (keeps Account object intact in cache).
        var rows = _accounts
            .OrderBy(a => a.BankName)
            .ThenBy(a => a.SortIndex)
            .ThenBy(a => a.AccountNickname)
            .Select(a => new Row
            {
                AccountId = a.AccountId,
                BankName = a.BankName,
                RoutingNumber = a.RoutingNumber,
                AccountNickname = a.AccountNickname,
                Last4 = Last4(a.AccountNumber),
                SortIndex = a.SortIndex,
                AccountType = a.AccountType.ToString(),
                IsActive = a.IsActive
            })
            .ToList();

        _grid.DataSource = rows;

        if (_grid.Columns.Contains(nameof(Row.AccountId)))
            _grid.Columns[nameof(Row.AccountId)].Visible = false;

        if (_grid.Columns.Contains(nameof(Row.RoutingNumber)))
            _grid.Columns[nameof(Row.RoutingNumber)].HeaderText = "Routing #";

        if (_grid.Columns.Contains(nameof(Row.Last4)))
            _grid.Columns[nameof(Row.Last4)].HeaderText = "Acct Last 4";

        if (_grid.Columns.Contains(nameof(Row.SortIndex)))
            _grid.Columns[nameof(Row.SortIndex)].HeaderText = "Sort";

        if (_grid.Columns.Contains(nameof(Row.IsActive)))
            _grid.Columns[nameof(Row.IsActive)].HeaderText = "Active";
    }

    public void SelectAccount(string accountId)
    {
        if (string.IsNullOrWhiteSpace(accountId))
            return;

        foreach (DataGridViewRow row in _grid.Rows)
        {
            if (row.DataBoundItem is Row r && r.AccountId == accountId)
            {
                _grid.ClearSelection();
                row.Selected = true;
                _grid.FirstDisplayedScrollingRowIndex = Math.Max(0, row.Index);
                break;
            }
        }
    }

    private void OpenSelected()
    {
        if (_grid.CurrentRow?.DataBoundItem is not Row r)
            return;

        if (string.IsNullOrWhiteSpace(r.AccountId))
            return;

        _openAccountDetails(r.AccountId);
    }

    private static string Last4(string accountNumber)
    {
        if (string.IsNullOrWhiteSpace(accountNumber))
            return "????";

        var cleaned = accountNumber.Replace(" ", "").Replace("-", "");
        return cleaned.Length <= 4 ? cleaned : cleaned.Substring(cleaned.Length - 4);
    }

    private sealed class Row
    {
        public string AccountId { get; set; } = string.Empty;
        public string BankName { get; set; } = string.Empty;
        public string RoutingNumber { get; set; } = string.Empty;
        public string AccountNickname { get; set; } = string.Empty;
        public string Last4 { get; set; } = string.Empty;
        public int SortIndex { get; set; }
        public string AccountType { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}