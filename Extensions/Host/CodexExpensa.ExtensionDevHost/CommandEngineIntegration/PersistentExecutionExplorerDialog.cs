namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration;

public sealed partial class PersistentExecutionExplorerDialog : Form
{
    private readonly ICommandExecutionPersistenceStore _store;
    private IReadOnlyList<CommandExecutionPersistentRecord> _currentRecords = [];

    public PersistentExecutionExplorerDialog(ICommandExecutionPersistenceStore store)
    {
        ArgumentNullException.ThrowIfNull(store);

        _store = store;

        InitializeComponent();
        LoadComboBoxes();
        RefreshRecords();
    }

    public CommandExecutionPersistentRecord? SelectedRecord { get; private set; }

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

        sortModeComboBox.Items.Clear();
        sortModeComboBox.Items.AddRange(Enum.GetNames<CommandExecutionPersistentRecordSortMode>());
        sortModeComboBox.SelectedItem = CommandExecutionPersistentRecordSortMode.NewestFirst.ToString();
    }

    private void RefreshRecords()
    {
        _currentRecords =
            _store.QueryRecords(BuildQuery());

        CommandExecutionPersistentRecordListViewBuilder.ConfigureColumns(recordsListView);
        CommandExecutionPersistentRecordListViewBuilder.Populate(recordsListView, _currentRecords);

        summaryLabel.Text = $"Rows: {_currentRecords.Count}";
        detailsTextBox.Text =
            CommandExecutionPersistentRecordTextFormatter.Format("Persisted Execution Records", _currentRecords);
    }

    private CommandExecutionPersistentRecordQuery BuildQuery()
    {
        CommandExecutionPersistentRecordSourceFilter sourceFilter =
            Enum.TryParse(Convert.ToString(sourceFilterComboBox.SelectedItem), out CommandExecutionPersistentRecordSourceFilter parsedSource)
                ? parsedSource
                : CommandExecutionPersistentRecordSourceFilter.All;

        CommandExecutionPersistentRecordSortMode sortMode =
            Enum.TryParse(Convert.ToString(sortModeComboBox.SelectedItem), out CommandExecutionPersistentRecordSortMode parsedSort)
                ? parsedSort
                : CommandExecutionPersistentRecordSortMode.NewestFirst;

        return new CommandExecutionPersistentRecordQuery
        {
            SourceFilter = sourceFilter,
            Status = Convert.ToString(statusFilterComboBox.SelectedItem) ?? string.Empty,
            SearchText = searchTextBox.Text.Trim(),
            SortMode = sortMode,
            MaximumRows = (int)maximumRowsNumericUpDown.Value
        };
    }

    private CommandExecutionPersistentRecord? GetSelectedRecord()
    {
        return recordsListView.SelectedItems.Count == 0
            ? null
            : recordsListView.SelectedItems[0].Tag as CommandExecutionPersistentRecord;
    }

    private void recordsListView_SelectedIndexChanged(object? sender, EventArgs e)
    {
        SelectedRecord = GetSelectedRecord();

        detailsTextBox.Text =
            SelectedRecord is null
                ? string.Empty
                : CommandExecutionPersistentRecordTextFormatter.Format("Selected Persisted Execution Record", [SelectedRecord]);
    }

    private void refreshButton_Click(object? sender, EventArgs e) => RefreshRecords();

    private void closeButton_Click(object? sender, EventArgs e)
    {
        DialogResult = DialogResult.Cancel;
        Close();
    }

    private void selectButton_Click(object? sender, EventArgs e)
    {
        SelectedRecord = GetSelectedRecord();

        if (SelectedRecord is null)
        {
            MessageBox.Show(this, "Select a persisted execution row first.", "Persistent Execution Explorer",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        DialogResult = DialogResult.OK;
        Close();
    }

    private void copyParametersButton_Click(object? sender, EventArgs e)
    {
        CommandExecutionPersistentRecord? record = GetSelectedRecord();

        if (record is not null)
        {
            Clipboard.SetText(record.ParameterJson);
        }
    }

    private void copyOutputButton_Click(object? sender, EventArgs e)
    {
        CommandExecutionPersistentRecord? record = GetSelectedRecord();

        if (record is not null)
        {
            Clipboard.SetText(record.OutputJson);
        }
    }

    private void exportJsonButton_Click(object? sender, EventArgs e)
    {
        using SaveFileDialog dialog = new()
        {
            Title = "Export Persisted Execution Records as JSON",
            Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*",
            FileName = $"persisted-execution-{DateTimeOffset.Now:yyyyMMdd-HHmmss}.json"
        };

        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        CommandExecutionPersistentRecordJsonExporter.Export(dialog.FileName, _currentRecords);
    }

    private void exportMarkdownButton_Click(object? sender, EventArgs e)
    {
        using SaveFileDialog dialog = new()
        {
            Title = "Export Persisted Execution Records as Markdown",
            Filter = "Markdown files (*.md)|*.md|All files (*.*)|*.*",
            FileName = $"persisted-execution-{DateTimeOffset.Now:yyyyMMdd-HHmmss}.md"
        };

        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        CommandExecutionPersistentRecordMarkdownExporter.Export(dialog.FileName, _currentRecords);
    }
}
