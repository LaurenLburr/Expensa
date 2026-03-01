using System;
using System.Windows.Forms;
using CodexExpensa.Core.Domain.Accounts;

namespace CodexExpensa.App.WinForms.UI.Accounts;

public sealed class AccountDetailsForm : Form
{
    private readonly IAccountRepository _repo;
    private readonly Action _onSaved;

    private Account? _account;

    private readonly TextBox _txtNickname;
    private readonly NumericUpDown _numSort;
    private readonly TextBox _txtAccountNumber;
    private readonly CheckBox _chkActive;

    private readonly TextBox _txtBankName;
    private readonly TextBox _txtRouting;
    private readonly TextBox _txtUrl;

    private readonly Button _btnSave;

    public AccountDetailsForm(IAccountRepository repo, Action onSaved)
    {
        _repo = repo ?? throw new ArgumentNullException(nameof(repo));
        _onSaved = onSaved ?? throw new ArgumentNullException(nameof(onSaved));

        Text = "Account";
        FormBorderStyle = FormBorderStyle.None;

        var panel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(12) };
        Controls.Add(panel);

        var lblTitle = new Label { Text = "Account Details", AutoSize = true, Left = 12, Top = 12 };

        var lblNick = new Label { Text = "Nickname:", AutoSize = true, Left = 12, Top = 50 };
        _txtNickname = new TextBox { Left = 140, Top = 46, Width = 700, Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };

        var lblSort = new Label { Text = "Sort Index:", AutoSize = true, Left = 12, Top = 86 };
        _numSort = new NumericUpDown { Left = 140, Top = 82, Width = 120, Minimum = 0, Maximum = 1000000 };

        var lblAcctNo = new Label { Text = "Account #:", AutoSize = true, Left = 12, Top = 122 };
        _txtAccountNumber = new TextBox { Left = 140, Top = 118, Width = 240 };

        _chkActive = new CheckBox { Text = "Active", Left = 140, Top = 154, Width = 120 };

        var groupBank = new GroupBox { Text = "Bank (from linked bank record)", Left = 12, Top = 190, Width = 860, Height = 150, Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };

        var lblBank = new Label { Text = "Bank Name:", AutoSize = true, Left = 12, Top = 30, Parent = groupBank };
        _txtBankName = new TextBox { Left = 120, Top = 26, Width = 700, ReadOnly = true, Parent = groupBank, Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };

        var lblRouting = new Label { Text = "Routing #:", AutoSize = true, Left = 12, Top = 66, Parent = groupBank };
        _txtRouting = new TextBox { Left = 120, Top = 62, Width = 240, ReadOnly = true, Parent = groupBank };

        var lblUrl = new Label { Text = "URL:", AutoSize = true, Left = 12, Top = 102, Parent = groupBank };
        _txtUrl = new TextBox { Left = 120, Top = 98, Width = 700, ReadOnly = true, Parent = groupBank, Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };

        _btnSave = new Button { Text = "Save", Width = 100, Height = 30, Left = 140, Top = 355 };
        _btnSave.Click += (_, _) => Save();

        panel.Controls.Add(lblTitle);
        panel.Controls.Add(lblNick);
        panel.Controls.Add(_txtNickname);
        panel.Controls.Add(lblSort);
        panel.Controls.Add(_numSort);
        panel.Controls.Add(lblAcctNo);
        panel.Controls.Add(_txtAccountNumber);
        panel.Controls.Add(_chkActive);
        panel.Controls.Add(groupBank);
        panel.Controls.Add(_btnSave);
    }

    public void LoadAccount(Account account)
    {
        _account = account ?? throw new ArgumentNullException(nameof(account));

        _txtNickname.Text = account.AccountNickname;
        _numSort.Value = account.SortIndex;
        _txtAccountNumber.Text = account.AccountNumber;
        _chkActive.Checked = account.IsActive;

        _txtBankName.Text = account.BankName;
        _txtRouting.Text = account.RoutingNumber;
        _txtUrl.Text = account.Url ?? string.Empty;
    }

    private void Save()
    {
        if (_account is null)
            return;

        var nickname = _txtNickname.Text.Trim();
        if (string.IsNullOrWhiteSpace(nickname))
        {
            MessageBox.Show(this, "Nickname is required.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            _txtNickname.Focus();
            return;
        }

        var acctNo = _txtAccountNumber.Text.Trim();
        if (string.IsNullOrWhiteSpace(acctNo))
        {
            MessageBox.Show(this, "Account number is required.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            _txtAccountNumber.Focus();
            return;
        }

        var updated = new Account
        {
            AccountId = _account.AccountId,
            BankId = _account.BankId,

            AccountNickname = nickname,
            SortIndex = (int)_numSort.Value,
            AccountNumber = acctNo,

            // keep bank projection fields as-is
            BankName = _account.BankName,
            RoutingNumber = _account.RoutingNumber,
            Url = _account.Url,

            AccountType = _account.AccountType,
            IsActive = _chkActive.Checked
        };

        try
        {
            _repo.Update(updated);
            _onSaved();
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.ToString(), "Save failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}