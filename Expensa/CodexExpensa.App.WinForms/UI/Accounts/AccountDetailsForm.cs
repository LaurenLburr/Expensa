using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using CodexExpensa.App.WinForms.UI.Banks;
using CodexExpensa.App.WinForms.UI.Common;
using CodexExpensa.Core.Abstractions;
using CodexExpensa.Core.Domain.Accounts;
using CodexExpensa.Core.Domain.Banks;
using CodexExpensa.Core.Domain.Payees;
using CodexExpensa.Core.Domain.Transactions;

namespace CodexExpensa.App.WinForms.UI.Accounts;

public sealed class AccountDetailsForm : Form
{
    private readonly IDatabaseSession _db;
    private readonly IAccountRepository _repo;
    private readonly ICredentialStore _creds;
    private readonly Action _onSaved;

    private readonly IReadOnlyList<Bank> _banks;

    private readonly ITransactionRepository _txns;
    private readonly IPayeeRepository _payees;

    private Account? _account;

    private readonly TabControl _tabs;
    private readonly TabPage _tabDetails;
    private readonly TabPage _tabTransactions;

    // Details controls
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

    private readonly GroupBox _groupBank;
    private readonly GroupBox _groupTags;

    private readonly TagAssignmentControl _tagAssignmentControl;

    private string? _urlTarget;

    private readonly Button _btnSave;

    private readonly AccountTransactionsPanel _transactionsPanel;

    public AccountDetailsForm(
        IDatabaseSession db,
        IAccountRepository repo,
        IReadOnlyList<Bank> banks,
        ICredentialStore creds,
        ITransactionRepository txns,
        IPayeeRepository payees,
        Action onSaved)
    {
        _db = db ?? throw new ArgumentNullException(nameof(db));
        _repo = repo ?? throw new ArgumentNullException(nameof(repo));
        _banks = banks ?? throw new ArgumentNullException(nameof(banks));
        _creds = creds ?? throw new ArgumentNullException(nameof(creds));
        _txns = txns ?? throw new ArgumentNullException(nameof(txns));
        _payees = payees ?? throw new ArgumentNullException(nameof(payees));
        _onSaved = onSaved ?? throw new ArgumentNullException(nameof(onSaved));

        Text = "Account";
        FormBorderStyle = FormBorderStyle.None;

        var root = new Panel { Dock = DockStyle.Fill, Padding = new Padding(12) };
        Controls.Add(root);

        var lblTitle = new Label { Text = "Account Details", AutoSize = true, Left = 12, Top = 12 };
        root.Controls.Add(lblTitle);

        _tabs = new TabControl
        {
            Left = 12,
            Top = 40,
            Width = 900,
            Height = 520,
            Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
        };
        root.Controls.Add(_tabs);

        _tabDetails = new TabPage("Details");
        _tabTransactions = new TabPage("Transactions");

        _tabs.TabPages.Add(_tabDetails);
        _tabs.TabPages.Add(_tabTransactions);

        _btnSave = new Button
        {
            Text = "Save",
            Width = 100,
            Height = 30,
            Anchor = AnchorStyles.Bottom | AnchorStyles.Left
        };
        _btnSave.Click += (_, _) => Save();
        root.Controls.Add(_btnSave);

        root.Resize += (_, _) =>
        {
            _btnSave.Left = 12;
            _btnSave.Top = root.Height - _btnSave.Height - 12;
            _tabs.Height = _btnSave.Top - _tabs.Top - 10;
            _tabs.Width = root.Width - 24;
        };

        // -------- Details Tab --------
        var details = new Panel { Dock = DockStyle.Fill, AutoScroll = true };
        _tabDetails.Controls.Add(details);

        var lblNick = new Label { Text = "Nickname:", AutoSize = true, Left = 12, Top = 16 };
        _txtNickname = new TextBox { Left = 140, Top = 12, Width = 700, Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };

        var lblSort = new Label { Text = "Sort Index:", AutoSize = true, Left = 12, Top = 52 };
        _numSort = new NumericUpDown { Left = 140, Top = 48, Width = 140, Minimum = 0, Maximum = 1000000 };

        var lblAcctNo = new Label { Text = "Account #:", AutoSize = true, Left = 12, Top = 88 };
        _txtAccountNumber = new TextBox { Left = 140, Top = 84, Width = 240 };

        var lblType = new Label { Text = "Type:", AutoSize = true, Left = 400, Top = 88 };
        _txtAccountType = new TextBox { Left = 450, Top = 84, Width = 180, ReadOnly = true };

        _chkActive = new CheckBox { Text = "Active", Left = 140, Top = 120, Width = 120 };

        _groupBank = new GroupBox
        {
            Text = "Bank (linked record)",
            Left = 12,
            Top = 155,
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
            Parent = _groupBank
        };
        _lnkBankLabel.LinkClicked += (_, _) => ShowBankPopup();

        _cmbBank = new ComboBox
        {
            Left = 120,
            Top = 26,
            Width = 700,
            Parent = _groupBank,
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        _cmbBank.SelectedIndexChanged += (_, _) => BankSelectionChanged();

        var lblRouting = new Label { Text = "Routing #:", AutoSize = true, Left = 12, Top = 70, Parent = _groupBank };
        _txtRouting = new TextBox { Left = 120, Top = 66, Width = 240, ReadOnly = true, Parent = _groupBank };

        _lnkOpenBankUrl = new LinkLabel
        {
            Text = "Open Bank URL",
            AutoSize = true,
            Left = 12,
            Top = 106,
            Parent = _groupBank
        };
        _lnkOpenBankUrl.LinkClicked += (_, _) => OpenUrl();

        _lnkUsername = new LinkLabel
        {
            Text = "Username (not set)",
            AutoSize = true,
            Left = 140,
            Top = 106,
            Parent = _groupBank
        };
        _lnkUsername.LinkClicked += (_, _) => EditCredentials();

        _lnkPw = new LinkLabel
        {
            Text = "PW (not set)",
            AutoSize = true,
            Left = 320,
            Top = 106,
            Parent = _groupBank
        };
        _lnkPw.LinkClicked += (_, _) => EditCredentials();

        var hint = new Label
        {
            Text = "Tip: click “Bank:” to change the bank. Credentials are stored in Windows Credential Manager.",
            AutoSize = true,
            Left = 12,
            Top = 145,
            Parent = _groupBank
        };

        _groupTags = new GroupBox
        {
            Text = "Tags",
            Left = 12,
            Top = 395,
            Width = 860,
            Height = 275,
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
        };

        _tagAssignmentControl = new TagAssignmentControl(
            _db,
            new TagAssignmentOptions
            {
                EntityDisplayName = "account",
                LoadAssignedTagsQueryName = "AccountTag.GetByAccountId",
                SearchTagsQueryName = "Tag.SearchByName",
                InsertTagQueryName = "Tag.Insert",
                AssignTagQueryName = "AccountTag.Insert",
                RemoveTagQueryName = "AccountTag.Delete",
                EntityIdParameterName = "@AccountId",
                SearchParameterName = "@Search",
                TagIdParameterName = "@TagId",
                TagNameParameterName = "@TagName",
                AssignmentIdParameterName = "@AccountTagId",
                AssignedTagIdColumnName = "TagId",
                AssignedTagNameColumnName = "TagName",
                SearchTagIdColumnName = "TagId",
                SearchTagNameColumnName = "TagName"
            })
        {
            Parent = _groupTags,
            Dock = DockStyle.Fill
        };
        _tagAssignmentControl.TagsChanged += (_, _) => _onSaved();

        details.Controls.Add(lblNick);
        details.Controls.Add(_txtNickname);
        details.Controls.Add(lblSort);
        details.Controls.Add(_numSort);
        details.Controls.Add(lblAcctNo);
        details.Controls.Add(_txtAccountNumber);
        details.Controls.Add(lblType);
        details.Controls.Add(_txtAccountType);
        details.Controls.Add(_chkActive);
        details.Controls.Add(_groupBank);
        details.Controls.Add(_groupTags);

        _tabDetails.Resize += (_, _) =>
        {
            _groupBank.Width = _tabDetails.ClientSize.Width - 24;
            _groupTags.Width = _tabDetails.ClientSize.Width - 24;
            _txtNickname.Width = _tabDetails.ClientSize.Width - _txtNickname.Left - 24;
            _cmbBank.Width = _groupBank.ClientSize.Width - _cmbBank.Left - 24;
        };

        // -------- Transactions Tab --------
        _transactionsPanel = new AccountTransactionsPanel(_txns, _payees, _repo);
        _tabTransactions.Controls.Add(_transactionsPanel);

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

        _tagAssignmentControl.LoadForEntity(account.AccountId);

        _transactionsPanel.LoadForAccount(
            accountId: account.AccountId,
            accountDisplayName: $"{account.AccountNickname} - {Last4(account.AccountNumber)}");
    }

    private static string Last4(string accountNumber)
    {
        if (string.IsNullOrWhiteSpace(accountNumber))
            return "????";

        var cleaned = accountNumber.Replace(" ", "").Replace("-", "");
        return cleaned.Length <= 4 ? cleaned : cleaned.Substring(cleaned.Length - 4);
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

        return CredentialKeys.Bank(bankId);
    }

    private void UpdateCredentialLinkText()
    {
        try
        {
            if (_cmbBank.SelectedItem is not BankItem)
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

        string key;
        try
        {
            key = GetCredentialKeyForSelectedBank();
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "Credentials", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        using var dlg = new BankCredentialsForm(
            _creds,
            bankDisplayName: $"{bankItem.Bank.BankName} ({bankItem.Bank.RoutingNumber})",
            credentialKey: key);

        dlg.ShowDialog(this);
        UpdateCredentialLinkText();
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
            BankId = bank.BankId,
            AccountNumber = acctNo,
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
