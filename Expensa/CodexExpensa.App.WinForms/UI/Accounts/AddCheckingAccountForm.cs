using System;
using System.Windows.Forms;

namespace CodexExpensa.App.WinForms.UI.Accounts;

public sealed class AddCheckingAccountForm : Form
{
    private readonly TextBox _txtNickname;
    private readonly TextBox _txtBankName;
    private readonly TextBox _txtRouting;
    private readonly TextBox _txtAccountNumber;
    private readonly TextBox _txtUrl;
    private readonly CheckBox _chkActive;

    private readonly Button _btnOk;
    private readonly Button _btnCancel;

    public string AccountNickname => _txtNickname.Text.Trim();
    public string BankName => _txtBankName.Text.Trim();
    public string RoutingNumber => _txtRouting.Text.Trim();
    public string AccountNumber => _txtAccountNumber.Text.Trim();
    public string? Url => string.IsNullOrWhiteSpace(_txtUrl.Text) ? null : _txtUrl.Text.Trim();
    public bool IsActive => _chkActive.Checked;

    public AddCheckingAccountForm()
    {
        Text = "Add Checking Account";
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        ShowInTaskbar = false;

        Width = 720;
        Height = 360;

        var panel = new Panel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(12)
        };

        // --- Labels / Inputs ---

        var lblNickname = new Label { Text = "Nickname:", AutoSize = true, Left = 12, Top = 16 };
        _txtNickname = new TextBox
        {
            Left = 150,
            Top = 12,
            Width = 520,
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
        };

        var lblBank = new Label { Text = "Bank Name:", AutoSize = true, Left = 12, Top = 52 };
        _txtBankName = new TextBox
        {
            Left = 150,
            Top = 48,
            Width = 520,
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
        };

        var lblRouting = new Label { Text = "Routing #:", AutoSize = true, Left = 12, Top = 88 };
        _txtRouting = new TextBox
        {
            Left = 150,
            Top = 84,
            Width = 260,
            Anchor = AnchorStyles.Top | AnchorStyles.Left
        };
        _txtRouting.MaxLength = 32; // Keep permissive; banks sometimes include leading zeros.

        var lblAccountNumber = new Label { Text = "Account #:", AutoSize = true, Left = 12, Top = 124 };
        _txtAccountNumber = new TextBox
        {
            Left = 150,
            Top = 120,
            Width = 260,
            Anchor = AnchorStyles.Top | AnchorStyles.Left
        };
        _txtAccountNumber.MaxLength = 64; // Permissive; some institutions have long strings.

        var lblUrl = new Label { Text = "URL:", AutoSize = true, Left = 12, Top = 160 };
        _txtUrl = new TextBox
        {
            Left = 150,
            Top = 156,
            Width = 520,
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
        };

        _chkActive = new CheckBox
        {
            Text = "Active",
            Left = 150,
            Top = 192,
            Width = 120,
            Checked = true
        };

        // --- Buttons ---

        _btnOk = new Button
        {
            Text = "OK",
            Width = 90,
            Height = 30,
            Anchor = AnchorStyles.Bottom | AnchorStyles.Right
        };

        _btnCancel = new Button
        {
            Text = "Cancel",
            Width = 90,
            Height = 30,
            Anchor = AnchorStyles.Bottom | AnchorStyles.Right
        };

        _btnOk.Click += (_, _) => OnOk();
        _btnCancel.Click += (_, _) => { DialogResult = DialogResult.Cancel; Close(); };

        // Place buttons at bottom-right
        var buttonTop = 240;
        _btnCancel.Left = panel.Width - _btnCancel.Width - 12;
        _btnCancel.Top = buttonTop;

        _btnOk.Left = _btnCancel.Left - _btnOk.Width - 10;
        _btnOk.Top = buttonTop;

        panel.Resize += (_, _) =>
        {
            _btnCancel.Left = panel.Width - _btnCancel.Width - 12;
            _btnCancel.Top = panel.Height - _btnCancel.Height - 12;

            _btnOk.Left = _btnCancel.Left - _btnOk.Width - 10;
            _btnOk.Top = _btnCancel.Top;
        };

        // Add controls
        panel.Controls.Add(lblNickname);
        panel.Controls.Add(_txtNickname);

        panel.Controls.Add(lblBank);
        panel.Controls.Add(_txtBankName);

        panel.Controls.Add(lblRouting);
        panel.Controls.Add(_txtRouting);

        panel.Controls.Add(lblAccountNumber);
        panel.Controls.Add(_txtAccountNumber);

        panel.Controls.Add(lblUrl);
        panel.Controls.Add(_txtUrl);

        panel.Controls.Add(_chkActive);

        panel.Controls.Add(_btnOk);
        panel.Controls.Add(_btnCancel);

        Controls.Add(panel);

        AcceptButton = _btnOk;
        CancelButton = _btnCancel;

        Shown += (_, _) => _txtNickname.Focus();
    }

    private void OnOk()
    {
        if (string.IsNullOrWhiteSpace(AccountNickname))
        {
            MessageBox.Show(this, "Nickname is required.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            _txtNickname.Focus();
            return;
        }

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

        if (string.IsNullOrWhiteSpace(AccountNumber))
        {
            MessageBox.Show(this, "Account number is required.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            _txtAccountNumber.Focus();
            return;
        }

        // Light-touch URL validation (optional)
        if (!string.IsNullOrWhiteSpace(_txtUrl.Text))
        {
            if (!Uri.TryCreate(_txtUrl.Text.Trim(), UriKind.Absolute, out _))
            {
                MessageBox.Show(this, "URL must be a valid absolute URL (or leave it blank).", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _txtUrl.Focus();
                return;
            }
        }

        DialogResult = DialogResult.OK;
        Close();
    }
}