using System;
using System.Windows.Forms;

namespace CodexExpensa.App.WinForms.UI.Banks;

public sealed class BankCredentialsForm : Form
{
    private readonly TextBox _txtUsername;
    private readonly TextBox _txtPassword;

    private readonly Button _btnOk;
    private readonly Button _btnCancel;
    private readonly Button _btnClear;

    public BankCredentialsForm(string bankDisplayName, string? existingUsername)
    {
        Text = "Bank Credentials";
        Width = 520;
        Height = 240;
        StartPosition = FormStartPosition.CenterParent;

        var lblTitle = new Label
        {
            Text = $"Credentials for: {bankDisplayName}",
            Left = 12,
            Top = 12,
            AutoSize = true
        };

        var lblUser = new Label { Text = "Username:", Left = 12, Top = 52, AutoSize = true };
        _txtUsername = new TextBox { Left = 110, Top = 48, Width = 370, Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };

        var lblPw = new Label { Text = "Password:", Left = 12, Top = 88, AutoSize = true };
        _txtPassword = new TextBox
        {
            Left = 110,
            Top = 84,
            Width = 370,
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
            UseSystemPasswordChar = true
        };

        var chkShow = new CheckBox { Text = "Show", Left = 110, Top = 112, AutoSize = true };
        chkShow.CheckedChanged += (_, _) => _txtPassword.UseSystemPasswordChar = !chkShow.Checked;

        _btnOk = new Button { Text = "OK", Left = 310, Top = 145, Width = 80, DialogResult = DialogResult.OK };
        _btnCancel = new Button { Text = "Cancel", Left = 400, Top = 145, Width = 80, DialogResult = DialogResult.Cancel };
        _btnClear = new Button { Text = "Clear Stored", Left = 12, Top = 145, Width = 110 };

        _btnOk.Click += (_, _) =>
        {
            if (string.IsNullOrWhiteSpace(_txtUsername.Text))
            {
                MessageBox.Show(this, "Username is required.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _txtUsername.Focus();
                DialogResult = DialogResult.None;
                return;
            }

            // Password can be empty if you really want it to be. Up to you.
        };

        _btnClear.Click += (_, _) =>
        {
            ClearRequested = true;
            DialogResult = DialogResult.OK;
        };

        Controls.Add(lblTitle);
        Controls.Add(lblUser);
        Controls.Add(_txtUsername);
        Controls.Add(lblPw);
        Controls.Add(_txtPassword);
        Controls.Add(chkShow);

        Controls.Add(_btnClear);
        Controls.Add(_btnOk);
        Controls.Add(_btnCancel);

        AcceptButton = _btnOk;
        CancelButton = _btnCancel;

        _txtUsername.Text = existingUsername ?? string.Empty;
    }

    public bool ClearRequested { get; private set; }

    public string Username => _txtUsername.Text.Trim();

    public string Password => _txtPassword.Text; // keep spaces if user typed them
}