namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration;

public sealed partial class PersistentExecutionRetentionDialog : Form
{
    private readonly ICommandExecutionPersistenceStore _store;
    private IReadOnlyList<CommandExecutionPersistentRecord> _previewRecords = [];

    public PersistentExecutionRetentionDialog(
        ICommandExecutionPersistenceStore store)
    {
        ArgumentNullException.ThrowIfNull(store);

        _store = store;

        InitializeComponent();
        LoadComboBoxes();
        PreviewRecords();
    }

    private void LoadComboBoxes()
    {
        sourceFilterComboBox.Items.Clear();
        sourceFilterComboBox.Items.AddRange(Enum.GetNames<CommandExecutionPersistentRecordSourceFilter>());
        sourceFilterComboBox.SelectedItem = CommandExecutionPersistentRecordSourceFilter.All.ToString();

        statusFilterComboBox.Items.Clear();
        statusFilterComboBox.Items.Add("");
        statusFilterComboBox.Items.Add("Pending");
        statusFilterComboBox.Items.Add("Running");
        statusFilterComboBox.Items.Add("Completed");
        statusFilterComboBox.Items.Add("Failed");
        statusFilterComboBox.Items.Add("Cancelled");
        statusFilterComboBox.SelectedIndex = 0;

        agePresetComboBox.Items.Clear();
        agePresetComboBox.Items.Add("Any age");
        agePresetComboBox.Items.Add("Older than 7 days");
        agePresetComboBox.Items.Add("Older than 30 days");
        agePresetComboBox.Items.Add("Older than 90 days");
        agePresetComboBox.Items.Add("Older than 365 days");
        agePresetComboBox.SelectedIndex = 0;
    }

    private void previewButton_Click(
        object? sender,
        EventArgs e)
    {
        PreviewRecords();
    }

    private void deleteButton_Click(
        object? sender,
        EventArgs e)
    {
        if (_previewRecords.Count == 0)
        {
            MessageBox.Show(
                this,
                "There are no matching records to delete.",
                "Persistent Execution Retention",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            return;
        }

        DialogResult response =
            MessageBox.Show(
                this,
                $"Delete {_previewRecords.Count} matching persisted execution record(s)?",
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

        if (response != DialogResult.Yes)
        {
            return;
        }

        int deleted =
            _store.DeleteRecords(BuildRetentionOptions());

        MessageBox.Show(
            this,
            $"Deleted {deleted} persisted execution record(s).",
            "Persistent Execution Retention",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);

        PreviewRecords();
    }

    private void vacuumButton_Click(
        object? sender,
        EventArgs e)
    {
        _store.Vacuum();

        MessageBox.Show(
            this,
            "Execution database vacuum completed.",
            "Persistent Execution Retention",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
    }

    private void closeButton_Click(
        object? sender,
        EventArgs e)
    {
        Close();
    }

    private void PreviewRecords()
    {
        CommandExecutionPersistentRecordQuery query =
            BuildPreviewQuery();

        _previewRecords =
            _store.QueryRecords(query);

        CommandExecutionPersistentRecordListViewBuilder.ConfigureColumns(previewListView);
        CommandExecutionPersistentRecordListViewBuilder.Populate(previewListView, _previewRecords);

        summaryLabel.Text =
            $"Matching records: {_previewRecords.Count}";

        detailsTextBox.Text =
            CommandExecutionPersistentRecordTextFormatter.Format(
                "Retention Preview",
                _previewRecords);
    }

    private CommandExecutionPersistentRecordQuery BuildPreviewQuery()
    {
        return new CommandExecutionPersistentRecordQuery
        {
            SourceFilter = GetSourceFilter(),
            Status = Convert.ToString(statusFilterComboBox.SelectedItem) ?? string.Empty,
            CreatedToUtc = GetCreatedBeforeUtc(),
            SortMode = CommandExecutionPersistentRecordSortMode.OldestFirst,
            MaximumRows = 1000
        };
    }

    private CommandExecutionRetentionOptions BuildRetentionOptions()
    {
        return new CommandExecutionRetentionOptions
        {
            SourceFilter = GetSourceFilter(),
            Status = Convert.ToString(statusFilterComboBox.SelectedItem) ?? string.Empty,
            CreatedBeforeUtc = GetCreatedBeforeUtc()
        };
    }

    private CommandExecutionPersistentRecordSourceFilter GetSourceFilter()
    {
        return Enum.TryParse(
            Convert.ToString(sourceFilterComboBox.SelectedItem),
            out CommandExecutionPersistentRecordSourceFilter parsed)
                ? parsed
                : CommandExecutionPersistentRecordSourceFilter.All;
    }

    private DateTimeOffset? GetCreatedBeforeUtc()
    {
        return Convert.ToString(agePresetComboBox.SelectedItem) switch
        {
            "Older than 7 days" => DateTimeOffset.UtcNow.AddDays(-7),
            "Older than 30 days" => DateTimeOffset.UtcNow.AddDays(-30),
            "Older than 90 days" => DateTimeOffset.UtcNow.AddDays(-90),
            "Older than 365 days" => DateTimeOffset.UtcNow.AddDays(-365),
            _ => null
        };
    }
}
