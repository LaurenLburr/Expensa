namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Budgets;

internal sealed class BudgetTransactionEntryDialog : Form
{
    private readonly ComboBox _accountComboBox;
    private readonly ComboBox _statusComboBox;
    private readonly NumericUpDown _amountNumericUpDown;
    private readonly DateTimePicker _startDatePicker;
    private readonly TextBox _confirmationTextBox;
    private readonly TextBox _noteTextBox;

    public BudgetTransactionEntryDialog(
        IReadOnlyList<BudgetTransactionAccountChoice> accounts,
        string payeeName,
        DateTime defaultDate,
        string title,
        string acceptButtonText,
        string defaultStatus,
        decimal defaultAmount)
    {
        ArgumentNullException.ThrowIfNull(accounts);

        Text =
            string.IsNullOrWhiteSpace(payeeName)
                ? title
                : $"{title} - {payeeName}";
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

        _accountComboBox =
            new ComboBox
            {
                Dock = DockStyle.Fill,
                DropDownStyle = ComboBoxStyle.DropDownList,
                DisplayMember = nameof(BudgetTransactionAccountChoice.DisplayText),
                ValueMember = nameof(BudgetTransactionAccountChoice.AccountId),
                DataSource = accounts.ToList()
            };

        _statusComboBox =
            new ComboBox
            {
                Dock = DockStyle.Left,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Width = 160,
                DataSource = new[]
                {
                    "Projected",
                    "Outstanding",
                    "Cleared"
                }
            };

        if (_statusComboBox.Items.Contains(defaultStatus))
        {
            _statusComboBox.SelectedItem = defaultStatus;
        }

        _amountNumericUpDown =
            new NumericUpDown
            {
                Dock = DockStyle.Left,
                DecimalPlaces = 2,
                Minimum = -100000000m,
                Maximum = 100000000m,
                ThousandsSeparator = true,
                Width = 160
            };

        _amountNumericUpDown.Value =
            Math.Min(
                _amountNumericUpDown.Maximum,
                Math.Max(
                    _amountNumericUpDown.Minimum,
                    defaultAmount));

        _startDatePicker =
            new DateTimePicker
            {
                Dock = DockStyle.Left,
                Format = DateTimePickerFormat.Short,
                Value = defaultDate,
                Width = 160
            };

        _confirmationTextBox =
            new TextBox
            {
                Dock = DockStyle.Fill
            };

        _noteTextBox =
            new TextBox
            {
                Dock = DockStyle.Fill,
                Multiline = true,
                ScrollBars = ScrollBars.Vertical
            };

        AddRow(
            layout,
            0,
            "Payee",
            new Label
            {
                AutoSize = true,
                Text = payeeName,
                Padding = new Padding(0, 6, 0, 0)
            });

        AddRow(layout, 1, "Account", _accountComboBox);
        AddRow(layout, 2, "Status", _statusComboBox);
        AddRow(layout, 3, "Amount", _amountNumericUpDown);
        AddRow(layout, 4, "Start date", _startDatePicker);
        AddRow(layout, 5, "Confirm", _confirmationTextBox);
        AddRow(layout, 6, "Note", _noteTextBox);

        FlowLayoutPanel buttons =
            new()
            {
                AutoSize = true,
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.RightToLeft
            };

        Button okButton =
            new()
            {
                Text = acceptButtonText,
                DialogResult = DialogResult.OK,
                AutoSize = true
            };

        Button cancelButton =
            new()
            {
                Text = "Cancel",
                DialogResult = DialogResult.Cancel,
                AutoSize = true
            };

        okButton.Click += OkButton_Click;

        buttons.Controls.Add(okButton);
        buttons.Controls.Add(cancelButton);
        layout.Controls.Add(buttons, 1, 7);

        AcceptButton = okButton;
        CancelButton = cancelButton;

        Controls.Add(layout);
    }

    public string AccountId =>
        Convert.ToString(_accountComboBox.SelectedValue)
        ?? string.Empty;

    public string Status =>
        Convert.ToString(_statusComboBox.SelectedItem)
        ?? "Outstanding";

    public decimal Amount => _amountNumericUpDown.Value;

    public DateTime StartDate => _startDatePicker.Value.Date;

    public string ConfirmationNumber =>
        _confirmationTextBox.Text.Trim();

    public string Note =>
        _noteTextBox.Text.Trim();

    private void OkButton_Click(
        object? sender,
        EventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(AccountId))
        {
            return;
        }

        DialogResult = DialogResult.None;

        MessageBox.Show(
            this,
            "Select an account.",
            Text,
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
}

internal sealed record BudgetTransactionAccountChoice(
    string AccountId,
    string DisplayText);
