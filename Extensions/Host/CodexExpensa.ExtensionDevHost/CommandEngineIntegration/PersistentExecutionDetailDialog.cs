namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration;

public sealed partial class PersistentExecutionDetailDialog : Form
{
    private readonly CommandExecutionPersistentRecord _record;

    public PersistentExecutionDetailDialog(
        CommandExecutionPersistentRecord record)
    {
        ArgumentNullException.ThrowIfNull(record);

        _record = record;

        InitializeComponent();
        LoadRecord();
    }

    public bool ReplayRequested { get; private set; }

    public CommandExecutionPersistentRecord Record =>
        _record;

    private void LoadRecord()
    {
        titleLabel.Text =
            $"{_record.Status}: {_record.CommandName}";

        summaryTextBox.Text =
            CommandExecutionPersistentRecordTextFormatter.Format(
                "Persisted Execution Record",
                [_record]);

        parameterTextBox.Text =
            string.IsNullOrWhiteSpace(_record.ParameterJson)
                ? "{}"
                : _record.ParameterJson;

        outputTextBox.Text =
            string.IsNullOrWhiteSpace(_record.OutputJson)
                ? "{}"
                : _record.OutputJson;

        replayButton.Enabled =
            CommandExecutionPersistentRecordReplayMapper.CanReplay(_record);
    }

    private void replayButton_Click(
        object? sender,
        EventArgs e)
    {
        ReplayRequested = true;
        DialogResult = DialogResult.OK;
        Close();
    }

    private void copyParametersButton_Click(
        object? sender,
        EventArgs e)
    {
        Clipboard.SetText(parameterTextBox.Text);
    }

    private void copyOutputButton_Click(
        object? sender,
        EventArgs e)
    {
        Clipboard.SetText(outputTextBox.Text);
    }

    private void copySummaryButton_Click(
        object? sender,
        EventArgs e)
    {
        Clipboard.SetText(summaryTextBox.Text);
    }

    private void closeButton_Click(
        object? sender,
        EventArgs e)
    {
        Close();
    }
}
