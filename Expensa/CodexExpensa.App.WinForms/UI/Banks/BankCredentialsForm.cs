using System;
using System.Windows.Forms;
using CodexExpensa.Core.Abstractions;

namespace CodexExpensa.App.WinForms.UI.Banks;

public sealed class BankCredentialsForm : Form
{
    private readonly ICredentialStore _creds;
    private readonly string _credentialKey;

    private readonly TextBox _txtUsername;
    private readonly TextBox _txtPassword;

    private readonly Button _btnSave;
    private readonly Button _btnCancel;
    private readonly Button _btnClear;

    public BankCredentialsForm(ICredentialStore creds, string bankDisplayName, string credentialKey)
    {
        _creds = creds ?? throw new ArgumentNullException(nameof(creds));

        if (string.IsNullOrWhiteSpace(bankDisplayName))
            throw new ArgumentException("bankDisplayName is required.", nameof(bankDisplayName));

        if (string.IsNullOrWhiteSpace(credentialKey))
            throw new ArgumentException("credentialKey is required.", nameof(credentialKey));

        _credentialKey = credentialKey;

        Text = "Bank Credentials";
        Width = 540;
        Height = 270;
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        ShowInTaskbar = false;

        var lblTitle = new Label
        {
            Text = $"Credentials for: {bankDisplayName}",
            Left = 12,
            Top = 12,
            AutoSize = true
        };

        var lblUser = new Label { Text = "Username:", Left = 12, Top = 52, AutoSize = true };
        _txtUsername = new TextBox { Left = 110, Top = 48, Width = 390, Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };

        var lblPw = new Label { Text = "Password:", Left = 12, Top = 88, AutoSize = true };
        _txtPassword = new TextBox
        {
            Left = 110,
            Top = 84,
            Width = 390,
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
            UseSystemPasswordChar = true
        };

        var chkShow = new CheckBox { Text = "Show", Left = 110, Top = 112, AutoSize = true };
        chkShow.CheckedChanged += (_, _) => _txtPassword.UseSystemPasswordChar = !chkShow.Checked;

        _btnSave = new Button { Text = "Save", Left = 330, Top = 165, Width = 80 };
        _btnCancel = new Button { Text = "Cancel", Left = 420, Top = 165, Width = 80, DialogResult = DialogResult.Cancel };
        _btnClear = new Button { Text = "Clear Stored", Left = 12, Top = 165, Width = 110 };

        _btnSave.Click += (_, _) => Save();
        _btnClear.Click += (_, _) => ClearStored();

        Controls.Add(lblTitle);
        Controls.Add(lblUser);
        Controls.Add(_txtUsername);
        Controls.Add(lblPw);
        Controls.Add(_txtPassword);
        Controls.Add(chkShow);

        Controls.Add(_btnClear);
        Controls.Add(_btnSave);
        Controls.Add(_btnCancel);

        AcceptButton = _btnSave;
        CancelButton = _btnCancel;

        Shown += (_, _) => LoadExisting();
    }

    private void LoadExisting()
    {
        try
        {
            if (_creds.TryGet(_credentialKey, out var username, out var password))
            {
                _txtUsername.Text = username ?? string.Empty;
                _txtPassword.Text = password ?? string.Empty;
            }
            else
            {
                _txtUsername.Text = string.Empty;
                _txtPassword.Text = string.Empty;
            }

            _txtUsername.Focus();
            _txtUsername.SelectAll();
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.ToString(), "Credential Load failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void Save()
    {
        var username = _txtUsername.Text.Trim();
        var password = _txtPassword.Text; // allow spaces

        if (!string.IsNullOrWhiteSpace(password) && string.IsNullOrWhiteSpace(username))
        {
            MessageBox.Show(this, "If you enter a password, you must enter a username.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            _txtUsername.Focus();
            return;
        }

        try
        {
            // Treat completely empty as "clear" (less surprise than storing empties forever)
            if (string.IsNullOrWhiteSpace(username) && string.IsNullOrEmpty(password))
            {
                _creds.Delete(_credentialKey);
            }
            else
            {
                _creds.Save(_credentialKey, username, password);
            }

            DialogResult = DialogResult.OK;
            Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.ToString(), "Credential Save failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void ClearStored()
    {
        var confirm = MessageBox.Show(
            this,
            "Clear the stored username/password for this bank?",
            "Clear Stored",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning);

        if (confirm != DialogResult.Yes)
            return;

        try
        {
            _creds.Delete(_credentialKey);

            _txtUsername.Text = string.Empty;
            _txtPassword.Text = string.Empty;

            DialogResult = DialogResult.OK;
            Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.ToString(), "Credential Clear failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}