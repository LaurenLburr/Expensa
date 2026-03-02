using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using CodexExpensa.Core.Domain.Banks;

namespace CodexExpensa.App.WinForms.UI.Accounts;

public sealed class AddCheckingAccountForm : Form
{
    private readonly IReadOnlyList<Bank> _banks;

    private readonly TextBox _txtNickname;
    private readonly TextBox _txtAccountNumber;
    private readonly CheckBox _chkActive;

    private readonly LinkLabel _lnkBankLabel;
    private readonly ComboBox _cmbBank;

    private readonly TextBox _txtRouting;
    private readonly TextBox _txtUrl;

    private readonly Button _btnOk;
    private readonly Button _btnCancel;

    private sealed class BankItem
    {
        public Bank Bank { get; }
        public BankItem(Bank bank) => Bank = bank ?? throw new ArgumentNullException(nameof(bank));
        public override string ToString() => $"{Bank.BankName} ({Bank.RoutingNumber})";
    }

    public AddCheckingAccountForm(IReadOnlyList<Bank> banks)
    {
        _banks = banks ?? throw new ArgumentNullException(nameof(banks));

        Text = "Add Checking Account";
        Width = 720;
        Height = 380;
        StartPosition = FormStartPosition.CenterParent;

        var lblNick = new Label { Text = "Account Nickname:", Left = 12, Top = 18, AutoSize = true };
        _txtNickname = new TextBox { Left = 160, Top = 14, Width = 520, Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };

        var lblAcctNo = new Label { Text = "Account Number:", Left = 12, Top = 54, AutoSize = true };
        _txtAccountNumber = new TextBox { Left = 160, Top = 50, Width = 240 };

        _chkActive = new CheckBox { Text = "Active", Left = 160, Top = 84, Width = 120, Checked = true };

        var groupBank = new GroupBox
        {
            Text = "Bank",
            Left = 12,
            Top = 120,
            Width = 668,
            Height = 150,
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
        };

        _lnkBankLabel = new LinkLabel { Text = "Bank:", Left = 12, Top = 30, AutoSize = true, Parent = groupBank };
        _lnkBankLabel.LinkClicked += (_, _) => ShowBankPopup();

        _cmbBank = new ComboBox
        {
            Left = 160,
            Top = 26,
            Width = 480,
            Parent = groupBank,
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        _cmbBank.SelectedIndexChanged += (_, _) => BankSelectionChanged();

        var lblRouting = new Label { Text = "Routing #:", Left = 12, Top = 70, AutoSize = true, Parent = groupBank };
        _txtRouting = new TextBox { Left = 160, Top = 66, Width = 240, ReadOnly = true, Parent = groupBank };

        var lblUrl = new Label { Text = "URL:", Left = 12, Top = 106, AutoSize = true, Parent = groupBank };
        _txtUrl = new TextBox
        {
            Left = 160,
            Top = 102,
            Width = 480,
            ReadOnly = true,
            Parent = groupBank,
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
        };

        _btnOk = new Button { Text = "OK", Left = 500, Top = 285, Width = 80, DialogResult = DialogResult.OK };
        _btnCancel = new Button { Text = "Cancel", Left = 600, Top = 285, Width = 80, DialogResult = DialogResult.Cancel };

        _btnOk.Click += (_, _) => { if (!ValidateInputs()) DialogResult = DialogResult.None; };

        Controls.Add(lblNick);
        Controls.Add(_txtNickname);
        Controls.Add(lblAcctNo);
        Controls.Add(_txtAccountNumber);
        Controls.Add(_chkActive);

        Controls.Add(groupBank);

        Controls.Add(_btnOk);
        Controls.Add(_btnCancel);

        AcceptButton = _btnOk;
        CancelButton = _btnCancel;

        LoadBanksIntoCombo();

        if (_cmbBank.Items.Count > 0)
            _cmbBank.SelectedIndex = 0;
    }

    private void LoadBanksIntoCombo()
    {
        _cmbBank.BeginUpdate();
        try
        {
            _cmbBank.Items.Clear();

            foreach (var b in _banks.OrderBy(b => b.BankName).ThenBy(b => b.RoutingNumber))
                _cmbBank.Items.Add(new BankItem(b));
        }
        finally
        {
            _cmbBank.EndUpdate();
        }
    }

    private void ShowBankPopup()
    {
        if (_cmbBank.Items.Count == 0)
        {
            MessageBox.Show(this, "No banks exist yet. Add one under Banks first.", "Expensa",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        _cmbBank.Focus();
        _cmbBank.DroppedDown = true;
    }

    private void BankSelectionChanged()
    {
        if (_cmbBank.SelectedItem is not BankItem item)
        {
            _txtRouting.Text = string.Empty;
            _txtUrl.Text = string.Empty;
            return;
        }

        _txtRouting.Text = item.Bank.RoutingNumber;
        _txtUrl.Text = item.Bank.Url ?? string.Empty;
    }

    private bool ValidateInputs()
    {
        if (string.IsNullOrWhiteSpace(AccountNickname))
        {
            MessageBox.Show(this, "Account nickname is required.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            _txtNickname.Focus();
            return false;
        }

        if (string.IsNullOrWhiteSpace(AccountNumber))
        {
            MessageBox.Show(this, "Account number is required.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            _txtAccountNumber.Focus();
            return false;
        }

        if (_cmbBank.SelectedItem is not BankItem)
        {
            MessageBox.Show(this, "Bank selection is required.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            ShowBankPopup();
            return false;
        }

        return true;
    }

    public string AccountNickname => _txtNickname.Text.Trim();
    public string AccountNumber => _txtAccountNumber.Text.Trim();
    public bool IsActive => _chkActive.Checked;

    public string BankId => (_cmbBank.SelectedItem as BankItem)?.Bank.BankId ?? string.Empty;
    public string BankName => (_cmbBank.SelectedItem as BankItem)?.Bank.BankName ?? string.Empty;
    public string RoutingNumber => (_cmbBank.SelectedItem as BankItem)?.Bank.RoutingNumber ?? string.Empty;
    public string? Url => (_cmbBank.SelectedItem as BankItem)?.Bank.Url;
}