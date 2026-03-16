using CodexExpensa.App.WinForms.UI.Common;
using CodexExpensa.App.WinForms.UI.Payees;
using CodexExpensa.Core.Domain.Accounts;
using CodexExpensa.Core.Domain.Payees;
using CodexExpensa.Core.Domain.Transactions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace CodexExpensa.App.WinForms.UI.Accounts;

public sealed class AccountTransactionsPanel : UserControl
{
    private readonly ITransactionRepository _txns;
    private readonly IPayeeRepository _payees;
    private readonly IAccountRepository _accounts;

    private readonly Label _lblScope;
    private readonly DataGridView _grid;
    private readonly Button _btnAdd;
    private readonly Button _btnDelete;
    private readonly Button _btnSave;
    private readonly Button _btnPayees;

    private readonly BindingList<TransactionRow> _rows = new();

    private readonly BindingList<BankChoice> _bankChoices = new();
    private readonly BindingList<AccountChoice> _accountChoices = new();
    private readonly BindingList<PayeeChoice> _payeeChoices = new();

    private readonly Dictionary<string, Account> _accountsById = new(StringComparer.Ordinal);
    private readonly Dictionary<string, List<AccountChoice>> _accountsByBankId = new(StringComparer.Ordinal);

    private string? _currentAccountId;

    public AccountTransactionsPanel(ITransactionRepository txns, IPayeeRepository payees, IAccountRepository accounts)
    {
        _txns = txns ?? throw new ArgumentNullException(nameof(txns));
        _payees = payees ?? throw new ArgumentNullException(nameof(payees));
        _accounts = accounts ?? throw new ArgumentNullException(nameof(accounts));

        Dock = DockStyle.Fill;

        Panel top = new() { Dock = DockStyle.Top, Height = 44, Padding = new Padding(8, 8, 8, 0) };
        Controls.Add(top);

        _lblScope = new Label { AutoSize = true, Left = 8, Top = 12, Text = "Transactions" };
        top.Controls.Add(_lblScope);

        _btnPayees = new Button { Text = "Payees...", Width = 90, Height = 28, Anchor = AnchorStyles.Top | AnchorStyles.Right };
        _btnSave = new Button { Text = "Save", Width = 80, Height = 28, Anchor = AnchorStyles.Top | AnchorStyles.Right };
        _btnDelete = new Button { Text = "Delete", Width = 80, Height = 28, Anchor = AnchorStyles.Top | AnchorStyles.Right };
        _btnAdd = new Button { Text = "Add", Width = 80, Height = 28, Anchor = AnchorStyles.Top | AnchorStyles.Right };

        top.Controls.Add(_btnPayees);
        top.Controls.Add(_btnSave);
        top.Controls.Add(_btnDelete);
        top.Controls.Add(_btnAdd);

        top.Resize += (_, _) =>
        {
            _btnPayees.Left = top.Width - _btnPayees.Width - 8;
            _btnPayees.Top = 8;

            _btnSave.Left = _btnPayees.Left - _btnSave.Width - 8;
            _btnSave.Top = 8;

            _btnDelete.Left = _btnSave.Left - _btnDelete.Width - 8;
            _btnDelete.Top = 8;

            _btnAdd.Left = _btnDelete.Left - _btnAdd.Width - 8;
            _btnAdd.Top = 8;
        };

        _grid = new DataGridView
        {
            Dock = DockStyle.Fill,
            AutoGenerateColumns = false,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            RowHeadersVisible = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            MultiSelect = false,
            DataSource = _rows
        };
        Controls.Add(_grid);

        BuildColumns();

        _grid.DataError += (_, e) =>
        {
            e.ThrowException = false;
        };

        _grid.CellValueChanged += Grid_CellValueChanged;
        _grid.CurrentCellDirtyStateChanged += (_, _) =>
        {
            if (_grid.IsCurrentCellDirty)
                _grid.CommitEdit(DataGridViewDataErrorContexts.Commit);
        };

        _btnAdd.Click += (_, _) => AddRow();
        _btnDelete.Click += (_, _) => DeleteSelectedRow();
        _btnSave.Click += (_, _) => SaveAll();
        _btnPayees.Click += (_, _) => ManagePayees();
    }

    public bool HasAnyRows => _rows.Count > 0;

    public void LoadForAccount(string accountId, string accountDisplayName)
    {
        if (string.IsNullOrWhiteSpace(accountId))
            throw new ArgumentException("accountId is required.", nameof(accountId));

        _currentAccountId = accountId;
        _lblScope.Text = $"Transactions for: {accountDisplayName}";

        ReloadLookups();
        ReloadTransactions();
    }

    private void ReloadLookups()
    {
        _bankChoices.Clear();
        _accountChoices.Clear();
        _payeeChoices.Clear();

        _accountsById.Clear();
        _accountsByBankId.Clear();

        List<Account> allAccounts = _accounts.GetAll()
            .OrderBy(x => x.BankName)
            .ThenBy(x => x.SortIndex)
            .ThenBy(x => x.AccountNickname)
            .ToList();

        foreach (IGrouping<string, Account> grp in allAccounts
                     .GroupBy(a => a.BankId, StringComparer.Ordinal)
                     .OrderBy(g => g.First().BankName, StringComparer.OrdinalIgnoreCase)
                     .ThenBy(g => g.First().RoutingNumber, StringComparer.OrdinalIgnoreCase))
        {
            Account first = grp.First();
            _bankChoices.Add(new BankChoice(first.BankId, $"{first.BankName} ({first.RoutingNumber})"));

            List<AccountChoice> list = grp
                .OrderBy(a => a.SortIndex)
                .ThenBy(a => a.AccountNickname)
                .ThenBy(a => a.AccountNumber)
                .Select(a => new AccountChoice(a.AccountId, $"{a.AccountNickname} - {Last4(a.AccountNumber)}"))
                .ToList();

            _accountsByBankId[first.BankId] = list;
        }

        foreach (Account a in allAccounts)
        {
            _accountsById[a.AccountId] = a;

            _accountChoices.Add(new AccountChoice(
                a.AccountId,
                $"{a.BankName} - {a.AccountNickname} - {Last4(a.AccountNumber)}"));
        }

        _payeeChoices.Add(new PayeeChoice(null, "(none)"));
        foreach (Payee p in _payees.GetAll().OrderBy(x => x.PayeeName))
        {
            _payeeChoices.Add(new PayeeChoice(p.PayeeId, p.PayeeName));
        }
    }

    private void ReloadTransactions()
    {
        _rows.Clear();

        if (string.IsNullOrWhiteSpace(_currentAccountId))
            return;

        foreach (Transaction t in _txns.GetByAccountId(_currentAccountId))
        {
            TransactionRow row = TransactionRow.FromDomain(t);

            if (!string.IsNullOrWhiteSpace(row.AccountId) && _accountsById.TryGetValue(row.AccountId, out Account? acct))
                row.BankId = acct.BankId;

            _rows.Add(row);
        }
    }

    private void BuildColumns()
    {
        _grid.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = nameof(TransactionRow.TransactionId),
            HeaderText = "TransactionId",
            DataPropertyName = nameof(TransactionRow.TransactionId),
            ReadOnly = true,
            Width = 110
        });

        _grid.Columns.Add(new DataGridViewComboBoxColumn
        {
            Name = nameof(TransactionRow.BankId),
            HeaderText = "Bank",
            DataPropertyName = nameof(TransactionRow.BankId),
            DataSource = _bankChoices,
            DisplayMember = nameof(BankChoice.Display),
            ValueMember = nameof(BankChoice.BankId),
            Width = 220,
            DisplayStyle = DataGridViewComboBoxDisplayStyle.DropDownButton,
            FlatStyle = FlatStyle.Flat
        });

        _grid.Columns.Add(new DataGridViewComboBoxColumn
        {
            Name = nameof(TransactionRow.AccountId),
            HeaderText = "Account",
            DataPropertyName = nameof(TransactionRow.AccountId),
            DataSource = _accountChoices,
            DisplayMember = nameof(AccountChoice.Display),
            ValueMember = nameof(AccountChoice.AccountId),
            Width = 240,
            DisplayStyle = DataGridViewComboBoxDisplayStyle.DropDownButton,
            FlatStyle = FlatStyle.Flat
        });

        _grid.Columns.Add(new DataGridViewComboBoxColumn
        {
            Name = nameof(TransactionRow.PayeeId),
            HeaderText = "Payee",
            DataPropertyName = nameof(TransactionRow.PayeeId),
            DataSource = _payeeChoices,
            DisplayMember = nameof(PayeeChoice.Display),
            ValueMember = nameof(PayeeChoice.PayeeId),
            Width = 180,
            DisplayStyle = DataGridViewComboBoxDisplayStyle.DropDownButton,
            FlatStyle = FlatStyle.Flat
        });

        _grid.Columns.Add(TransactionStatusUi.CreateColumn(nameof(TransactionRow.Status), headerText: "Status", width: 130));

        _grid.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = nameof(TransactionRow.Amount),
            HeaderText = "Amount",
            DataPropertyName = nameof(TransactionRow.Amount),
            Width = 90,
            DefaultCellStyle = { Format = "0.00" }
        });

        _grid.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = nameof(TransactionRow.StartDate),
            HeaderText = "StartDate",
            DataPropertyName = nameof(TransactionRow.StartDate),
            Width = 110,
            DefaultCellStyle = { Format = "yyyy-MM-dd" }
        });

        _grid.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = nameof(TransactionRow.Confirm),
            HeaderText = "Confirm",
            DataPropertyName = nameof(TransactionRow.Confirm),
            Width = 110
        });

        _grid.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = nameof(TransactionRow.Note),
            HeaderText = "Note",
            DataPropertyName = nameof(TransactionRow.Note),
            AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
        });
    }

    private void Grid_CellValueChanged(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0)
            return;

        if (_grid.Rows[e.RowIndex].DataBoundItem is not TransactionRow row)
            return;

        if (_grid.Columns[e.ColumnIndex].Name == nameof(TransactionRow.BankId))
        {
            if (string.IsNullOrWhiteSpace(row.BankId))
                return;

            if (!string.IsNullOrWhiteSpace(row.AccountId) &&
                _accountsById.TryGetValue(row.AccountId, out Account? existingAcct) &&
                string.Equals(existingAcct.BankId, row.BankId, StringComparison.Ordinal))
            {
                return;
            }

            if (_accountsByBankId.TryGetValue(row.BankId, out List<AccountChoice>? acctChoices) && acctChoices.Count > 0)
            {
                row.AccountId = acctChoices[0].AccountId;
                _grid.Refresh();
            }

            return;
        }

        if (_grid.Columns[e.ColumnIndex].Name == nameof(TransactionRow.AccountId))
        {
            if (!string.IsNullOrWhiteSpace(row.AccountId) && _accountsById.TryGetValue(row.AccountId, out Account? acct))
            {
                row.BankId = acct.BankId;
                _grid.Refresh();
            }
        }
    }

    private void AddRow()
    {
        if (string.IsNullOrWhiteSpace(_currentAccountId))
            return;

        string? bankId = null;
        if (_accountsById.TryGetValue(_currentAccountId, out Account? acct))
            bankId = acct.BankId;

        _rows.Add(new TransactionRow
        {
            TransactionId = 0,
            BankId = bankId,
            AccountId = _currentAccountId,
            PayeeId = null,
            Status = TransactionStatus.Outstanding,
            Amount = 0m,
            StartDate = DateTime.Today,
            Confirm = string.Empty,
            Note = string.Empty
        });

        _grid.ClearSelection();
        _grid.Rows[^1].Selected = true;
        _grid.FirstDisplayedScrollingRowIndex = Math.Max(0, _grid.Rows.Count - 1);
    }

    private void DeleteSelectedRow()
    {
        if (_grid.CurrentRow?.DataBoundItem is not TransactionRow row)
            return;

        DialogResult confirm = MessageBox.Show(this, "Delete selected transaction?", "Delete",
            MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

        if (confirm != DialogResult.Yes)
            return;

        if (row.TransactionId <= 0)
        {
            _rows.Remove(row);
            return;
        }

        _txns.Delete(row.TransactionId);
        ReloadTransactions();
    }

    private void SaveAll()
    {
        _grid.EndEdit();

        foreach (TransactionRow row in _rows.ToList())
        {
            if (string.IsNullOrWhiteSpace(row.AccountId))
                continue;

            if (row.StartDate == default)
                row.StartDate = DateTime.Today;

            Transaction domain = row.ToDomain();

            if (row.TransactionId <= 0)
                _txns.Add(domain);
            else
                _txns.Update(domain);
        }

        ReloadTransactions();
    }

    private void ManagePayees()
    {
        using PayeesForm dlg = new(_payees);
        dlg.ShowDialog(this);

        ReloadLookups();
        _grid.Refresh();
    }

    private static string Last4(string accountNumber)
    {
        if (string.IsNullOrWhiteSpace(accountNumber))
            return "????";

        string cleaned = accountNumber.Replace(" ", "").Replace("-", "");
        return cleaned.Length <= 4 ? cleaned : cleaned.Substring(cleaned.Length - 4);
    }

    private sealed class BankChoice
    {
        public string BankId { get; }
        public string Display { get; }

        public BankChoice(string bankId, string display)
        {
            BankId = bankId ?? throw new ArgumentNullException(nameof(bankId));
            Display = display ?? throw new ArgumentNullException(nameof(display));
        }
    }

    private sealed class AccountChoice
    {
        public string AccountId { get; }
        public string Display { get; }

        public AccountChoice(string accountId, string display)
        {
            AccountId = accountId ?? throw new ArgumentNullException(nameof(accountId));
            Display = display ?? throw new ArgumentNullException(nameof(display));
        }
    }

    private sealed class PayeeChoice
    {
        public string? PayeeId { get; }
        public string Display { get; }

        public PayeeChoice(string? payeeId, string display)
        {
            PayeeId = payeeId;
            Display = display ?? throw new ArgumentNullException(nameof(display));
        }
    }

    public sealed class TransactionRow
    {
        public int TransactionId { get; set; }

        public string? BankId { get; set; }

        public string AccountId { get; set; } = string.Empty;

        public string? PayeeId { get; set; }

        public TransactionStatus Status { get; set; } = TransactionStatus.Outstanding;

        public decimal Amount { get; set; }

        public DateTime StartDate { get; set; } = DateTime.Today;

        public string? Confirm { get; set; }

        public string? Note { get; set; }

        public Transaction ToDomain()
        {
            Transaction txn = new()
            {
                AccountId = AccountId,
                PayeeId = string.IsNullOrWhiteSpace(PayeeId) ? null : PayeeId,
                Status = Status,
                Amount = Amount,
                StartDate = StartDate.Date,
                Confirm = string.IsNullOrWhiteSpace(Confirm) ? null : Confirm,
                Note = string.IsNullOrWhiteSpace(Note) ? null : Note
            };

            if (TransactionId > 0)
                txn.SetTransactionId(TransactionId);

            return txn;
        }

        public static TransactionRow FromDomain(Transaction t)
        {
            return new TransactionRow
            {
                TransactionId = t.TransactionId,
                AccountId = t.AccountId,
                PayeeId = t.PayeeId,
                Status = t.Status,
                Amount = t.Amount,
                StartDate = t.StartDate.Date,
                Confirm = t.Confirm,
                Note = t.Note
            };
        }
    }
}