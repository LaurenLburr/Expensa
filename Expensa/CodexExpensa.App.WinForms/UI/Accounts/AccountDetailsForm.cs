using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using CodexExpensa.Core.Abstractions;
using CodexExpensa.Core.Domain.Accounts;
using CodexExpensa.Core.Domain.Banks;
using CodexExpensa.App.WinForms.UI.Banks;

namespace CodexExpensa.App.WinForms.UI.Accounts;

public sealed class AccountDetailsForm : Form
{
    private readonly IAccountRepository _repo;
    private readonly ICredentialStore _creds;
    private readonly Action _onSaved;

    private readonly IReadOnlyList<Bank> _banks;

    private Account? _account;

    private readonly TextBox _txtNickname;
    private readonly NumericUpDown _numSort;
    private readonly TextBox _txtAccountNumber;
    private readonly CheckBox _chkActive;

    private readonly TextBox _txtAccountType;

    private readonly LinkLabel _lnkBankLabel;
    private readonly ComboBox _cmbBank;
    private readonly TextBox _txtRouting;

    private readonly LinkLabel _lnkOpenBankUrl;
    private readonly LinkLabel _lnkUsername;
    private readonly LinkLabel _lnkPw;

    private string? _urlTarget;

    private readonly Button _btnSave;

    public AccountDetailsForm(IAccountRepository repo, IReadOnlyList<Bank> banks, ICredentialStore creds, Action onSaved)
    {
        _repo = repo ?? throw new ArgumentNullException(nameof(repo));
        _banks = banks ?? throw new ArgumentNullException(nameof(banks));
        _creds = creds ?? throw new ArgumentNullException(nameof(creds));
        _onSaved = onSaved ?? throw new ArgumentNullException(nameof(onSaved));

        Text = "Account";
        FormBorderStyle = FormBorderStyle.None;

        var panel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(12) };
        Controls.Add(panel);

        var lblTitle = new Label { Text = "Account Details", AutoSize = true, Left = 12, Top = 12 };

        var lblNick = new Label { Text = "Nickname:", AutoSize = true, Left = 12, Top = 50 };
        _txtNickname = new TextBox { Left = 140, Top = 46, Width = 700, Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };

        var lblSort = new Label { Text = "Sort Index:", AutoSize = true, Left = 12, Top = 86 };
        _numSort = new NumericUpDown { Left = 140, Top = 82, Width = 140, Minimum = 0, Maximum = 1000000 };

        var lblAcctNo = new Label { Text = "Account #:", AutoSize = true, Left = 12, Top = 122 };
        _txtAccountNumber = new TextBox { Left = 140, Top = 118, Width = 240 };

        var lblType = new Label { Text = "Type:", AutoSize = true, Left = 400, Top = 122 };
        _txtAccountType = new TextBox { Left = 450, Top = 118, Width = 180, ReadOnly = true };

        _chkActive = new CheckBox { Text = "Active", Left = 140, Top = 154, Width = 120 };

        var groupBank = new GroupBox
        {
            Text = "Bank (linked record)",
            Left = 12,
            Top = 190,
            Width = 860,
            Height = 230,
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
        };

        _lnkBankLabel = new LinkLabel
        {
            Text = "Bank:",
            AutoSize = true,
            Left = 12,
            Top = 30,
            Parent = groupBank
        };
        _lnkBankLabel.LinkClicked += (_, _) => ShowBankPopup();

        _cmbBank = new ComboBox
        {
            Left = 120,
            Top = 26,
            Width = 700,
            Parent = groupBank,
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        _cmbBank.SelectedIndexChanged += (_, _) => BankSelectionChanged();

        var lblRouting = new Label { Text = "Routing #:", AutoSize = true, Left = 12, Top = 70, Parent = groupBank };
        _txtRouting = new TextBox { Left = 120, Top = 66, Width = 240, ReadOnly = true, Parent = groupBank };

        _lnkOpenBankUrl = new LinkLabel
        {
            Text = "Open Bank URL",
            AutoSize = true,
            Left = 12,
            Top = 106,
            Parent = groupBank
        };
        _lnkOpenBankUrl.LinkClicked += (_, _) => OpenUrl();

        _lnkUsername = new LinkLabel
        {
            Text = "Username (not set)",
            AutoSize = true,
            Left = 140,
            Top = 106,
            Parent = groupBank
        };
        _lnkUsername.LinkClicked += (_, _) => EditCredentials();

        _lnkPw = new LinkLabel
        {
            Text = "PW (not set)",
            AutoSize = true,
            Left = 320,
            Top = 106,
            Parent = groupBank
        };
        _lnkPw.LinkClicked += (_, _) => EditCredentials();

        var hint = new Label
        {
            Text = "Tip: click “Bank:” to change the bank. Credentials are stored in Windows Credential Manager.",
            AutoSize = true,
            Left = 12,
            Top = 145,
            Parent = groupBank
        };

        _btnSave = new Button { Text = "Save", Width = 100, Height = 30, Left = 140, Top = 435 };
        _btnSave.Click += (_, _) => Save();

        panel.Controls.Add(lblTitle);
        panel.Controls.Add(lblNick);
        panel.Controls.Add(_txtNickname);
        panel.Controls.Add(lblSort);
        panel.Controls.Add(_numSort);
        panel.Controls.Add(lblAcctNo);
        panel.Controls.Add(_txtAccountNumber);
        panel.Controls.Add(lblType);
        panel.Controls.Add(_txtAccountType);
        panel.Controls.Add(_chkActive);
        panel.Controls.Add(groupBank);
        panel.Controls.Add(_btnSave);

        LoadBanksIntoCombo();
        SetUrl(null);
        UpdateCredentialLinkText();
        UpdateBankLinksEnabled();
    }

    public void LoadAccount(Account account)
    {
        _account = account ?? throw new ArgumentNullException(nameof(account));

        _txtNickname.Text = account.AccountNickname;
        _numSort.Value = account.SortIndex;
        _txtAccountNumber.Text = account.AccountNumber;
        _chkActive.Checked = account.IsActive;

        _txtAccountType.Text = account.AccountType.ToString();

        SelectBankByAccount(account);
        UpdateCredentialLinkText();
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

    private void SelectBankByAccount(Account account)
    {
        Bank? match = null;

        if (!string.IsNullOrWhiteSpace(account.BankId))
            match = _banks.FirstOrDefault(b => b.BankId == account.BankId);

        if (match is null)
        {
            match = _banks.FirstOrDefault(b =>
                string.Equals(b.BankName, account.BankName, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(b.RoutingNumber, account.RoutingNumber, StringComparison.OrdinalIgnoreCase));
        }

        if (match is null && _banks.Count > 0)
            match = _banks[0];

        if (match is null)
        {
            _cmbBank.SelectedIndex = -1;
            _txtRouting.Text = account.RoutingNumber;
            SetUrl(account.Url);
            UpdateBankLinksEnabled();
            return;
        }

        for (var i = 0; i < _cmbBank.Items.Count; i++)
        {
            if (_cmbBank.Items[i] is BankItem item && item.Bank.BankId == match.BankId)
            {
                _cmbBank.SelectedIndex = i;
                break;
            }
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
            SetUrl(null);
            UpdateBankLinksEnabled();
            UpdateCredentialLinkText();
            return;
        }

        _txtRouting.Text = item.Bank.RoutingNumber;
        SetUrl(item.Bank.Url);
        UpdateBankLinksEnabled();
        UpdateCredentialLinkText();
    }

    private void SetUrl(string? url)
    {
        _urlTarget = NormalizeUrl(url);
    }

    private static string? NormalizeUrl(string? url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return null;

        var trimmed = url.Trim();

        if (!trimmed.StartsWith("http://", StringComparison.OrdinalIgnoreCase) &&
            !trimmed.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
        {
            trimmed = "https://" + trimmed;
        }

        return trimmed;
    }

    private void UpdateBankLinksEnabled()
    {
        _lnkOpenBankUrl.Enabled = !string.IsNullOrWhiteSpace(_urlTarget);
        _lnkUsername.Enabled = _cmbBank.SelectedItem is BankItem;
        _lnkPw.Enabled = _cmbBank.SelectedItem is BankItem;
    }

    private void OpenUrl()
    {
        if (string.IsNullOrWhiteSpace(_urlTarget))
        {
            MessageBox.Show(this, "No bank URL is set for the selected bank.", "Open Bank URL",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = _urlTarget,
                UseShellExecute = true
            });
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, $"Could not open URL:\n\n{ex.Message}", "Open Bank URL",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private string? GetSelectedBankId()
    {
        return (_cmbBank.SelectedItem as BankItem)?.Bank.BankId;
    }

    private string GetCredentialKeyForSelectedBank()
    {
        var bankId = GetSelectedBankId();
        if (string.IsNullOrWhiteSpace(bankId))
            throw new InvalidOperationException("No bank is selected.");

        return $"CodexExpensa.Bank.{bankId}";
    }

    private void UpdateCredentialLinkText()
    {
        try
        {
            if (_cmbBank.SelectedItem is not BankItem bankItem)
            {
                _lnkUsername.Text = "Username (n/a)";
                _lnkPw.Text = "PW (n/a)";
                return;
            }

            var key = GetCredentialKeyForSelectedBank();
            if (_creds.TryGet(key, out var username, out var password))
            {
                _lnkUsername.Text = string.IsNullOrWhiteSpace(username) ? "Username (set?)" : $"Username: {username}";
                _lnkPw.Text = string.IsNullOrEmpty(password) ? "PW (empty)" : "PW (set)";
            }
            else
            {
                _lnkUsername.Text = "Username (not set)";
                _lnkPw.Text = "PW (not set)";
            }
        }
        catch
        {
            _lnkUsername.Text = "Username (error)";
            _lnkPw.Text = "PW (error)";
        }
    }

    private void EditCredentials()
    {
        if (_cmbBank.SelectedItem is not BankItem bankItem)
            return;

        var key = GetCredentialKeyForSelectedBank();

        _creds.TryGet(key, out var existingUser, out _);

        using var dlg = new BankCredentialsForm(
            bankDisplayName: $"{bankItem.Bank.BankName} ({bankItem.Bank.RoutingNumber})",
            existingUsername: existingUser);

        if (dlg.ShowDialog(this) != DialogResult.OK)
            return;

        try
        {
            if (dlg.ClearRequested)
            {
                _creds.Delete(key);
            }
            else
            {
                _creds.Save(key, dlg.Username, dlg.Password);
            }

            UpdateCredentialLinkText();
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.ToString(), "Credential Save failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
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

        if (_cmbBank.SelectedItem is not BankItem bankItem)
        {
            MessageBox.Show(this, "Bank selection is required.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            ShowBankPopup();
            return;
        }

        var bank = bankItem.Bank;

        var updated = new Account
        {
            AccountId = _account.AccountId,

            AccountNickname = nickname,
            SortIndex = (int)_numSort.Value,
            AccountNumber = acctNo,

            BankId = bank.BankId,
            BankName = bank.BankName,
            RoutingNumber = bank.RoutingNumber,
            Url = bank.Url,

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

    private sealed class BankItem
    {
        public Bank Bank { get; }
        public BankItem(Bank bank) => Bank = bank ?? throw new ArgumentNullException(nameof(bank));
        public override string ToString() => $"{Bank.BankName} ({Bank.RoutingNumber})";
    }
}