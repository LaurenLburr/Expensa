using System;
using System.Windows.Forms;
using CodexExpensa.Core.Abstractions;
using CodexExpensa.Core.Domain.Banks;

namespace CodexExpensa.App.WinForms.UI.Banks;

public sealed class BankDetailsForm : Form
{
    private readonly IBankRepository _repo;
    private readonly ICredentialStore _credentialStore;
    private readonly Action _onSaved;

    private Bank? _bank;

    private readonly TextBox _txtBankName;
    private readonly TextBox _txtRouting;
    private readonly TextBox _txtUrl;
    private readonly CheckBox _chkActive;

    private readonly Button _btnSave;
    private readonly Button _btnCredentials;

    public BankDetailsForm(IBankRepository repo, ICredentialStore credentialStore, Action onSaved)
    {
        _repo = repo ?? throw new ArgumentNullException(nameof(repo));
        _credentialStore = credentialStore ?? throw new ArgumentNullException(nameof(credentialStore));
        _onSaved = onSaved ?? throw new ArgumentNullException(nameof(onSaved));

        Text = "Bank";
        FormBorderStyle = FormBorderStyle.None;

        var panel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(12) };
        Controls.Add(panel);

        var lblTitle = new Label { Text = "Bank Details", AutoSize = true, Left = 12, Top = 12 };

        var lblName = new Label { Text = "Bank Name:", AutoSize = true, Left = 12, Top = 50 };
        _txtBankName = new TextBox { Left = 140, Top = 46, Width = 700, Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };

        var lblRouting = new Label { Text = "Routing #:", AutoSize = true, Left = 12, Top = 86 };
        _txtRouting = new TextBox { Left = 140, Top = 82, Width = 240 };

        var lblUrl = new Label { Text = "URL:", AutoSize = true, Left = 12, Top = 122 };
        _txtUrl = new TextBox { Left = 140, Top = 118, Width = 700, Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };

        _chkActive = new CheckBox { Text = "Active", Left = 140, Top = 154, Width = 120 };

        _btnSave = new Button { Text = "Save", Width = 100, Height = 30, Anchor = AnchorStyles.Bottom | AnchorStyles.Left };
        _btnSave.Click += (_, _) => Save();

        _btnCredentials = new Button { Text = "Credentials...", Width = 120, Height = 30, Anchor = AnchorStyles.Bottom | AnchorStyles.Left };
        _btnCredentials.Click += (_, _) => OpenCredentials();
        _btnCredentials.Enabled = false;

        panel.Controls.Add(lblTitle);
        panel.Controls.Add(lblName);
        panel.Controls.Add(_txtBankName);
        panel.Controls.Add(lblRouting);
        panel.Controls.Add(_txtRouting);
        panel.Controls.Add(lblUrl);
        panel.Controls.Add(_txtUrl);
        panel.Controls.Add(_chkActive);
        panel.Controls.Add(_btnSave);
        panel.Controls.Add(_btnCredentials);

        // Lower Save/Credentials buttons: keep them glued to the bottom.
        panel.Resize += (_, _) =>
        {
            var bottom = panel.Height - 12 - _btnSave.Height;
            _btnSave.Left = 140;
            _btnSave.Top = bottom;

            _btnCredentials.Left = _btnSave.Right + 10;
            _btnCredentials.Top = bottom;
        };
    }

    public void LoadBank(Bank bank)
    {
        _bank = bank ?? throw new ArgumentNullException(nameof(bank));

        _txtBankName.Text = bank.BankName;
        _txtRouting.Text = bank.RoutingNumber;
        _txtUrl.Text = bank.Url ?? string.Empty;
        _chkActive.Checked = bank.IsActive;

        _btnCredentials.Enabled = true;
    }

    private void OpenCredentials()
    {
        if (_bank is null)
            return;

        var key = CredentialKeys.Bank(_bank.BankId);

        using var dlg = new BankCredentialsForm(
            _credentialStore,
            bankDisplayName: $"{_bank.BankName} ({_bank.RoutingNumber})",
            credentialKey: key)
        {
            StartPosition = FormStartPosition.CenterParent
        };

        dlg.ShowDialog(this);
    }

    private void Save()
    {
        if (_bank is null)
            return;

        var bankName = _txtBankName.Text.Trim();
        var routing = _txtRouting.Text.Trim();
        var url = string.IsNullOrWhiteSpace(_txtUrl.Text) ? null : _txtUrl.Text.Trim();
        var active = _chkActive.Checked;

        if (string.IsNullOrWhiteSpace(bankName))
        {
            MessageBox.Show(this, "Bank Name is required.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            _txtBankName.Focus();
            return;
        }

        if (string.IsNullOrWhiteSpace(routing))
        {
            MessageBox.Show(this, "Routing number is required.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            _txtRouting.Focus();
            return;
        }

        if (!string.IsNullOrWhiteSpace(_txtUrl.Text))
        {
            if (!Uri.TryCreate(_txtUrl.Text.Trim(), UriKind.Absolute, out _))
            {
                MessageBox.Show(this, "URL must be a valid absolute URL (or leave it blank).", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _txtUrl.Focus();
                return;
            }
        }

        var updated = new Bank
        {
            BankId = _bank.BankId,
            BankName = bankName,
            RoutingNumber = routing,
            Url = url,
            IsActive = active
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