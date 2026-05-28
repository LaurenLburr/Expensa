namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration;

public sealed class CommandParameterEditorDialog : Form
{
    private readonly TextBox _jsonTextBox = new();
    private readonly Button _okButton = new();
    private readonly Button _cancelButton = new();
    private readonly Button _emptyObjectButton = new();

    public CommandParameterEditorDialog()
    {
        Text = "Command Parameters";
        Width = 720;
        Height = 520;
        StartPosition = FormStartPosition.CenterParent;
        MinimizeBox = false;
        MaximizeBox = false;

        BuildLayout();

        ParameterJson = "{}";
    }

    public string ParameterJson
    {
        get => _jsonTextBox.Text;
        set => _jsonTextBox.Text = value;
    }

    public IReadOnlyDictionary<string, object?> Parameters { get; private set; } =
        new Dictionary<string, object?>();

    private void BuildLayout()
    {
        TableLayoutPanel root = new()
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 3,
            Padding = new Padding(8)
        };

        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 28));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 44));

        Label label = new()
        {
            Dock = DockStyle.Fill,
            Text = "Enter command parameters as a JSON object:",
            TextAlign = ContentAlignment.MiddleLeft
        };

        _jsonTextBox.Dock = DockStyle.Fill;
        _jsonTextBox.Multiline = true;
        _jsonTextBox.ScrollBars = ScrollBars.Both;
        _jsonTextBox.WordWrap = false;
        _jsonTextBox.Font = new Font(FontFamily.GenericMonospace, 10);

        FlowLayoutPanel buttons = new()
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.RightToLeft
        };

        _okButton.Text = "OK";
        _okButton.Width = 90;
        _okButton.Click += OkButton_Click;

        _cancelButton.Text = "Cancel";
        _cancelButton.Width = 90;
        _cancelButton.DialogResult = DialogResult.Cancel;

        _emptyObjectButton.Text = "Use {}";
        _emptyObjectButton.Width = 90;
        _emptyObjectButton.Click += (_, _) => ParameterJson = "{}";

        buttons.Controls.Add(_okButton);
        buttons.Controls.Add(_cancelButton);
        buttons.Controls.Add(_emptyObjectButton);

        root.Controls.Add(label, 0, 0);
        root.Controls.Add(_jsonTextBox, 0, 1);
        root.Controls.Add(buttons, 0, 2);

        Controls.Add(root);

        AcceptButton = _okButton;
        CancelButton = _cancelButton;
    }

    private void OkButton_Click(
        object? sender,
        EventArgs e)
    {
        try
        {
            Parameters =
                CommandParameterJsonParser.Parse(ParameterJson);

            DialogResult = DialogResult.OK;
            Close();
        }
        catch (Exception exception)
        {
            MessageBox.Show(
                this,
                exception.Message,
                "Invalid Parameters",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }
}
