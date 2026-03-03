using System;
using System.Windows.Forms;

namespace CodexExpensa.App.WinForms.UI.Banks;

public sealed class AddBankForm : Form
{
    private readonly TextBox _txtBankName;
    private readonly TextBox _txtRouting;
    private readonly TextBox _txtUrl;
    private readonly CheckBox _chkActive;

    // Optional credentials captured at Bank creation time
    private readonly TextBox _txtUsername;
    private readonly TextBox _txtPassword;

    private readonly Button _btnOk;
    private readonly Button _btnCancel;

    public string BankName => _txtBankName.Text.Trim();

    public string RoutingNumber => _txtRouting.Text.Trim();

    public string? Url => string.IsNullOrWhiteSpace(_txtUrl.Text) ? null : _txtUrl.Text.Trim();

    public bool IsActive => _chkActive.Checked;

    public string Username => _txtUsername.Text.Trim();

    public string Password => _txtPassword.Text; // keep spaces if user typed them

    public AddBankForm()
    {
        Text = "Add Bank";
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        ShowInTaskbar = false;

        Width = 700;
        Height = 380;

        var panel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(12) };
        Controls.Add(panel);

        // --- Bank fields ---
        var lblBank = new Label { Text = "Bank Name:", AutoSize = true, Left = 12, Top = 16 };
        _txtBankName = new TextBox { Left = 140, Top = 12, Width = 520, Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };

        var lblRouting = new Label { Text = "Routing #:", AutoSize = true, Left = 12, Top = 52 };
        _txtRouting = new TextBox { Left = 140, Top = 48, Width = 260, Anchor = AnchorStyles.Top | AnchorStyles.Left };
        _txtRouting.MaxLength = 32;

        var lblUrl = new Label { Text = "URL:", AutoSize = true, Left = 12, Top = 88 };
        _txtUrl = new TextBox { Left = 140, Top = 84, Width = 520, Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };

        _chkActive = new CheckBox { Text = "Active", Left = 140, Top = 120, Width = 120, Checked = true };

        // --- Credentials group (optional) ---
        var groupCreds = new GroupBox
        {
            Text = "Login (optional — stored with this bank)",
            Left = 12,
            Top = 154,
            Width = 648,
            Height = 120,
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
        };

        var lblUser = new Label { Text = "Username:", AutoSize = true, Left = 12, Top = 30, Parent = groupCreds };
        _txtUsername = new TextBox { Left = 110, Top = 26, Width = 520, Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right, Parent = groupCreds };

        var lblPw = new Label { Text = "Password:", AutoSize = true, Left = 12, Top = 66, Parent = groupCreds };
        _txtPassword = new TextBox
        {
            Left = 110,
            Top = 62,
            Width = 520,
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
            UseSystemPasswordChar = true,
            Parent = groupCreds
        };

        var chkShow = new CheckBox { Text = "Show", Left = 110, Top = 90, AutoSize = true, Parent = groupCreds };
        chkShow.CheckedChanged += (_, _) => _txtPassword.UseSystemPasswordChar = !chkShow.Checked;

        // --- Buttons ---
        _btnOk = new Button { Text = "OK", Width = 90, Height = 30, Anchor = AnchorStyles.Bottom | AnchorStyles.Right };
        _btnCancel = new Button { Text = "Cancel", Width = 90, Height = 30, Anchor = AnchorStyles.Bottom | AnchorStyles.Right };

        _btnOk.Click += (_, _) => OnOk();
        _btnCancel.Click += (_, _) => { DialogResult = DialogResult.Cancel; Close(); };

        panel.Controls.Add(lblBank);
        panel.Controls.Add(_txtBankName);
        panel.Controls.Add(lblRouting);
        panel.Controls.Add(_txtRouting);
        panel.Controls.Add(lblUrl);
        panel.Controls.Add(_txtUrl);
        panel.Controls.Add(_chkActive);
        panel.Controls.Add(groupCreds);

        panel.Controls.Add(_btnOk);
        panel.Controls.Add(_btnCancel);

        // Lower the buttons (and keep them there)
        panel.Resize += (_, _) =>
        {
            _btnCancel.Left = panel.Width - _btnCancel.Width - 12;
            _btnCancel.Top = panel.Height - _btnCancel.Height - 12;

            _btnOk.Left = _btnCancel.Left - _btnOk.Width - 10;
            _btnOk.Top = _btnCancel.Top;
        };

        AcceptButton = _btnOk;
        CancelButton = _btnCancel;

        Shown += (_, _) => _txtBankName.Focus();
    }

    private void OnOk()
    {
        if (string.IsNullOrWhiteSpace(BankName))
        {
            MessageBox.Show(this, "Bank Name is required.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            _txtBankName.Focus();
            return;
        }

        if (string.IsNullOrWhiteSpace(RoutingNumber))
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

        // Credential rule: if either field is used, require username.
        var user = Username;
        var pw = Password;
        if (!string.IsNullOrWhiteSpace(pw) && string.IsNullOrWhiteSpace(user))
        {
            MessageBox.Show(this, "If you enter a password, you must enter a username.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            _txtUsername.Focus();
            return;
        }

        DialogResult = DialogResult.OK;
        Close();
    }
}