using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using CodexExpensa.Core.Domain.Banks;

namespace CodexExpensa.App.WinForms.UI.Accounts;

public sealed class AddCheckingAccountForm : Form
{
    private readonly IReadOnlyList<Bank> _banks;

    private readonly ComboBox _cmbBank;
    private readonly TextBox _txtNickname;
    private readonly TextBox _txtAccountNumber;
    private readonly CheckBox _chkActive;

    private readonly Button _btnOk;
    private readonly Button _btnCancel;

    public string? BankId => (_cmbBank.SelectedItem as BankItem)?.Bank.BankId;
    public string AccountNickname => _txtNickname.Text.Trim();
    public string AccountNumber => _txtAccountNumber.Text.Trim();
    public bool IsActive => _chkActive.Checked;

    public AddCheckingAccountForm(IReadOnlyList<Bank> banks, string? preselectedBankId = null)
    {
        _banks = banks ?? throw new ArgumentNullException(nameof(banks));

        Text = "Add Checking Account";
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        ShowInTaskbar = false;

        Width = 700;
        Height = 320;

        var root = new Panel { Dock = DockStyle.Fill, Padding = new Padding(12) };
        Controls.Add(root);

        var buttonBar = new Panel
        {
            Dock = DockStyle.Bottom,
            Height = 56,
            Padding = new Padding(0, 8, 0, 0)
        };
        root.Controls.Add(buttonBar);

        var content = new Panel
        {
            Dock = DockStyle.Fill
        };
        root.Controls.Add(content);

        var lblBank = new Label { Text = "Bank:", AutoSize = true, Left = 12, Top = 16 };
        _cmbBank = new ComboBox
        {
            Left = 140,
            Top = 12,
            Width = 520,
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
            DropDownStyle = ComboBoxStyle.DropDownList
        };

        var lblNick = new Label { Text = "Nickname:", AutoSize = true, Left = 12, Top = 52 };
        _txtNickname = new TextBox
        {
            Left = 140,
            Top = 48,
            Width = 520,
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
        };

        var lblAcct = new Label { Text = "Account #:", AutoSize = true, Left = 12, Top = 88 };
        _txtAccountNumber = new TextBox { Left = 140, Top = 84, Width = 240 };

        _chkActive = new CheckBox { Text = "Active", Left = 140, Top = 120, Width = 120, Checked = true };

        _btnOk = new Button { Text = "OK", Width = 90, Height = 30, Anchor = AnchorStyles.Bottom | AnchorStyles.Right };
        _btnCancel = new Button { Text = "Cancel", Width = 90, Height = 30, Anchor = AnchorStyles.Bottom | AnchorStyles.Right };

        _btnOk.Click += (_, _) => OnOk();
        _btnCancel.Click += (_, _) => { DialogResult = DialogResult.Cancel; Close(); };

        buttonBar.Controls.Add(_btnOk);
        buttonBar.Controls.Add(_btnCancel);

        buttonBar.Resize += (_, _) =>
        {
            _btnCancel.Left = buttonBar.Width - _btnCancel.Width - 12;
            _btnCancel.Top = buttonBar.Height - _btnCancel.Height - 10;

            _btnOk.Left = _btnCancel.Left - _btnOk.Width - 10;
            _btnOk.Top = _btnCancel.Top;
        };

        content.Controls.Add(lblBank);
        content.Controls.Add(_cmbBank);
        content.Controls.Add(lblNick);
        content.Controls.Add(_txtNickname);
        content.Controls.Add(lblAcct);
        content.Controls.Add(_txtAccountNumber);
        content.Controls.Add(_chkActive);

        AcceptButton = _btnOk;
        CancelButton = _btnCancel;

        LoadBanks(preselectedBankId);

        Shown += (_, _) =>
        {
            if (_cmbBank.SelectedIndex < 0)
                _cmbBank.Focus();
            else
                _txtNickname.Focus();
        };
    }

    private void LoadBanks(string? preselectedBankId)
    {
        _cmbBank.BeginUpdate();
        try
        {
            _cmbBank.Items.Clear();

            foreach (var b in _banks.OrderBy(b => b.BankName).ThenBy(b => b.RoutingNumber))
                _cmbBank.Items.Add(new BankItem(b));

            if (_cmbBank.Items.Count == 0)
            {
                _cmbBank.SelectedIndex = -1;
                return;
            }

            if (!string.IsNullOrWhiteSpace(preselectedBankId))
            {
                for (var i = 0; i < _cmbBank.Items.Count; i++)
                {
                    if (_cmbBank.Items[i] is BankItem item && item.Bank.BankId == preselectedBankId)
                    {
                        _cmbBank.SelectedIndex = i;
                        return;
                    }
                }
            }

            _cmbBank.SelectedIndex = 0;
        }
        finally
        {
            _cmbBank.EndUpdate();
        }
    }

    private void OnOk()
    {
        if (_cmbBank.SelectedItem is not BankItem)
        {
            MessageBox.Show(this, "Bank selection is required.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            _cmbBank.Focus();
            return;
        }

        if (string.IsNullOrWhiteSpace(AccountNickname))
        {
            MessageBox.Show(this, "Nickname is required.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            _txtNickname.Focus();
            return;
        }

        if (string.IsNullOrWhiteSpace(AccountNumber))
        {
            MessageBox.Show(this, "Account number is required.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            _txtAccountNumber.Focus();
            return;
        }

        DialogResult = DialogResult.OK;
        Close();
    }

    private sealed class BankItem
    {
        public Bank Bank { get; }
        public BankItem(Bank bank) => Bank = bank ?? throw new ArgumentNullException(nameof(bank));
        public override string ToString() => $"{Bank.BankName} ({Bank.RoutingNumber})";
    }
}