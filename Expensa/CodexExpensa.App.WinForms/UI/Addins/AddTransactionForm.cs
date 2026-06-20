using CodexExpensa.Core.Domain.Accounts;
using CodexExpensa.Core.Domain.Transactions;

namespace CodexExpensa.App.WinForms.UI.Addins;

public sealed class AddTransactionForm : Form
{
    private readonly ComboBox _account;
    private readonly ComboBox _status;
    private readonly NumericUpDown _amount;
    private readonly DateTimePicker _startDate;
    private readonly TextBox _confirm;
    private readonly TextBox _note;

    public AddTransactionForm(
        IReadOnlyList<Account> accounts,
        string payeeName,
        DateTime defaultDate)
    {
        ArgumentNullException.ThrowIfNull(accounts);

        Text = "Add Transaction";
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MinimizeBox = false;
        MaximizeBox = false;
        ShowInTaskbar = false;
        ClientSize = new Size(520, 330);

        TableLayoutPanel layout =
            new()
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 8,
                Padding = new Padding(12)
            };

        layout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        _account =
            new ComboBox
            {
                Dock = DockStyle.Fill,
                DropDownStyle = ComboBoxStyle.DropDownList,
                DisplayMember = nameof(AccountChoice.Display),
                ValueMember = nameof(AccountChoice.AccountId),
                DataSource = accounts
                    .OrderBy(static account => account.BankName)
                    .ThenBy(static account => account.SortIndex)
                    .ThenBy(static account => account.AccountNickname)
                    .Select(static account => new AccountChoice(
                        account.AccountId,
                        $"{account.BankName} - {account.AccountNickname}"))
                    .ToList()
            };

        _status =
            new ComboBox
            {
                Dock = DockStyle.Fill,
                DropDownStyle = ComboBoxStyle.DropDownList,
                DataSource = Enum.GetValues<TransactionStatus>()
            };

        _status.SelectedItem = TransactionStatus.Outstanding;

        _amount =
            new NumericUpDown
            {
                Dock = DockStyle.Left,
                DecimalPlaces = 2,
                Minimum = -100000000m,
                Maximum = 100000000m,
                ThousandsSeparator = true,
                Width = 160
            };

        _startDate =
            new DateTimePicker
            {
                Dock = DockStyle.Left,
                Format = DateTimePickerFormat.Short,
                Value = defaultDate,
                Width = 160
            };

        _confirm = new TextBox { Dock = DockStyle.Fill };
        _note =
            new TextBox
            {
                Dock = DockStyle.Fill,
                Multiline = true,
                ScrollBars = ScrollBars.Vertical
            };

        AddRow(layout, 0, "Payee", new Label
        {
            AutoSize = true,
            Text = payeeName,
            Padding = new Padding(0, 6, 0, 0)
        });
        AddRow(layout, 1, "Account", _account);
        AddRow(layout, 2, "Status", _status);
        AddRow(layout, 3, "Amount", _amount);
        AddRow(layout, 4, "Start date", _startDate);
        AddRow(layout, 5, "Confirm", _confirm);
        AddRow(layout, 6, "Note", _note);

        FlowLayoutPanel buttons =
            new()
            {
                AutoSize = true,
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.RightToLeft
            };

        Button ok =
            new()
            {
                Text = "Add",
                DialogResult = DialogResult.OK,
                AutoSize = true
            };

        Button cancel =
            new()
            {
                Text = "Cancel",
                DialogResult = DialogResult.Cancel,
                AutoSize = true
            };

        buttons.Controls.Add(ok);
        buttons.Controls.Add(cancel);
        layout.Controls.Add(buttons, 1, 7);

        AcceptButton = ok;
        CancelButton = cancel;

        ok.Click += Ok_Click;
        Controls.Add(layout);
    }

    public string AccountId =>
        Convert.ToString(_account.SelectedValue)
        ?? string.Empty;

    public TransactionStatus Status =>
        _status.SelectedItem is TransactionStatus value
            ? value
            : TransactionStatus.Outstanding;

    public decimal Amount => _amount.Value;

    public DateTime StartDate => _startDate.Value.Date;

    public string? Confirm =>
        string.IsNullOrWhiteSpace(_confirm.Text)
            ? null
            : _confirm.Text.Trim();

    public string? Note =>
        string.IsNullOrWhiteSpace(_note.Text)
            ? null
            : _note.Text.Trim();

    private void Ok_Click(object? sender, EventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(AccountId))
        {
            return;
        }

        DialogResult = DialogResult.None;

        MessageBox.Show(
            this,
            "Select an account.",
            "Add Transaction",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
    }

    private static void AddRow(
        TableLayoutPanel layout,
        int row,
        string labelText,
        Control control)
    {
        layout.RowStyles.Add(
            new RowStyle(
                row == 6
                    ? SizeType.Percent
                    : SizeType.AutoSize,
                row == 6 ? 100 : 0));

        Label label =
            new()
            {
                AutoSize = true,
                Text = labelText,
                Padding = new Padding(0, 6, 12, 0)
            };

        layout.Controls.Add(label, 0, row);
        layout.Controls.Add(control, 1, row);
    }

    private sealed record AccountChoice(
        string AccountId,
        string Display);
}
