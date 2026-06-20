using Codex.CommandEngine.Core;
using CodexExpensa.ExtensionDevHost.CommandEngineIntegration.ExtensionManager;
using System.Diagnostics;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration;

public sealed partial class ExtensionRuntimeDashboardForm : Form
{
    private readonly ExtensionRuntimeDashboardController _controller;
    private readonly IExtensionRuntimeDashboardSettingsStore _settingsStore;
    private readonly ICommandExecutionHistoryStore _historyStore;
    private readonly DashboardViewState _viewState = new();

    private bool _showDuplicateManifests = true;

    public ExtensionRuntimeDashboardForm()
        : this(
            new ExtensionRuntimeManager(),
            new JsonExtensionRuntimeDashboardSettingsStore(),
            new JsonCommandExecutionHistoryStore())
    {
    }

    public ExtensionRuntimeDashboardForm(
        IExtensionRuntimeManager manager)
        : this(
            manager,
            new JsonExtensionRuntimeDashboardSettingsStore(),
            new JsonCommandExecutionHistoryStore())
    {
    }

    public ExtensionRuntimeDashboardForm(
        IExtensionRuntimeManager manager,
        IExtensionRuntimeDashboardSettingsStore settingsStore)
        : this(
            manager,
            settingsStore,
            new JsonCommandExecutionHistoryStore())
    {
    }

    public ExtensionRuntimeDashboardForm(
        IExtensionRuntimeManager manager,
        IExtensionRuntimeDashboardSettingsStore settingsStore,
        ICommandExecutionHistoryStore historyStore)
    {
        ArgumentNullException.ThrowIfNull(manager);
        ArgumentNullException.ThrowIfNull(settingsStore);
        ArgumentNullException.ThrowIfNull(historyStore);

        _controller = new ExtensionRuntimeDashboardController(manager);
        _settingsStore = settingsStore;
        _historyStore = historyStore;

        InitializeComponent();

        EnsureAddinFolderStructure();
        LoadDashboardSettings();
        RefreshFromSnapshot(_controller.GetSnapshot());
    }

    private bool GetEffectiveRecursive()
    {
        return recursiveCheckBox.Checked ||
            ExtensionOutputFolderResolver.IsModulesFolder(folderTextBox.Text);
    }

    private void BrowseFolderButton_Click(
        object? sender,
        EventArgs e)
    {
        using FolderBrowserDialog dialog = new()
        {
            Description = "Select a folder containing extension manifests or provider DLLs",
            UseDescriptionForTitle = true,
            SelectedPath = Directory.Exists(folderTextBox.Text)
                ? folderTextBox.Text
                : AppContext.BaseDirectory
        };

        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        folderTextBox.Text = dialog.SelectedPath;

        if (ExtensionOutputFolderResolver.IsModulesFolder(folderTextBox.Text))
        {
            recursiveCheckBox.Checked = true;
        }

        SaveDashboardSettings();
    }

    private void FolderTextBox_Leave(
        object? sender,
        EventArgs e)
    {
        SaveDashboardSettings();
    }

    private void RecursiveCheckBox_CheckedChanged(
        object? sender,
        EventArgs e)
    {
        SaveDashboardSettings();
    }

    private void ExtensionRuntimeDashboardForm_FormClosing(
        object? sender,
        FormClosingEventArgs e)
    {
        SaveDashboardSettings();
    }

    private void CommandList_DoubleClick(
        object? sender,
        EventArgs e)
    {
        if (_viewState.IsExecutionHistory)
        {
            ShowSelectedHistoryDetails(sender, e);
            return;
        }

        if (_viewState.IsExecutionQueue)
        {
            ShowSelectedQueueItemDetails(sender, e);
            return;
        }

        ExecuteSelectedCommandAsync(sender, e);
    }

    private void StartSmokeRuntime(
        object? sender,
        EventArgs e)
    {
        RefreshFromSnapshot(_controller.StartSmokeRuntime());
    }

    private void StartFromLoadedAssemblies(
        object? sender,
        EventArgs e)
    {
        TryRefresh(() => _controller.StartFromLoadedAssemblies());
    }

    private void StartFromFolder(
        object? sender,
        EventArgs e)
    {
        TryRefresh(() => _controller.StartFromFolder(folderTextBox.Text, GetEffectiveRecursive()));
    }

    private void StartFromManifests(
        object? sender,
        EventArgs e)
    {
        TryRefresh(() => _controller.StartFromManifests(folderTextBox.Text, GetEffectiveRecursive()));
    }

    private void ReloadFromLoadedAssemblies(
        object? sender,
        EventArgs e)
    {
        TryRefresh(() => _controller.ReloadFromLoadedAssemblies());
    }

    private void ReloadFromFolder(
        object? sender,
        EventArgs e)
    {
        TryRefresh(() => _controller.ReloadFromFolder(folderTextBox.Text, GetEffectiveRecursive()));
    }

    private void ReloadFromManifests(
        object? sender,
        EventArgs e)
    {
        TryRefresh(() => _controller.ReloadFromManifests(folderTextBox.Text, GetEffectiveRecursive()));
    }

    private void StopRuntime(
        object? sender,
        EventArgs e)
    {
        RefreshFromSnapshot(_controller.StopRuntime());
    }

    private void RefreshSnapshot(
        object? sender,
        EventArgs e)
    {
        RefreshFromSnapshot(_controller.GetSnapshot());
    }

    private void DiscoverManifestRegistry(
        object? sender,
        EventArgs e)
    {
        try
        {
            ExtensionManifestRegistryViewModel viewModel =
                _controller.DiscoverManifestRegistry(folderTextBox.Text, GetEffectiveRecursive());

            RefreshFromManifestRegistry(viewModel);
            SaveDashboardSettings();
        }
        catch (Exception exception)
        {
            MessageBox.Show(
                this,
                exception.Message,
                "Manifest Registry",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }

    private void ShowDuplicateManifestRowsMenuItem_CheckedChanged(
        object? sender,
        EventArgs e)
    {
        _showDuplicateManifests = showDuplicateManifestRowsMenuItem.Checked;
        SaveDashboardSettings();

        if (_viewState.IsManifestRegistry)
        {
            DiscoverManifestRegistry(sender, e);
        }
    }

    private void EnableSelectedManifest(
        object? sender,
        EventArgs e)
    {
        SetSelectedManifestEnabled(true);
    }

    private void DisableSelectedManifest(
        object? sender,
        EventArgs e)
    {
        SetSelectedManifestEnabled(false);
    }

    private void SetSelectedManifestDisplaySort(
        object? sender,
        EventArgs e)
    {
        ExtensionManifestRecord? record =
            GetSelectedManifestRecord();

        if (record is null)
        {
            return;
        }

        if (!TryPromptForDisplaySort(record, out int displaySort))
        {
            return;
        }

        ExtensionManifestUpdateResult result =
            _controller.SetManifestDisplaySort(record.ManifestPath, displaySort);

        if (!result.Success)
        {
            MessageBox.Show(
                this,
                result.Message,
                "Manifest Registry",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);

            return;
        }

        DiscoverManifestRegistry(this, EventArgs.Empty);
    }

    private void OpenSelectedManifest(
        object? sender,
        EventArgs e)
    {
        ExtensionManifestRecord? record =
            GetSelectedManifestRecord();

        if (record is not null)
        {
            OpenPath(record.ManifestPath);
        }
    }

    private void OpenSelectedManifestFolder(
        object? sender,
        EventArgs e)
    {
        ExtensionManifestRecord? record =
            GetSelectedManifestRecord();

        if (record is not null)
        {
            OpenPath(record.ManifestFolder);
        }
    }

    private void CopySelectedManifestPath(
        object? sender,
        EventArgs e)
    {
        ExtensionManifestRecord? record =
            GetSelectedManifestRecord();

        if (record is not null)
        {
            Clipboard.SetText(record.ManifestPath);
        }
    }

    private ExtensionManifestRecord? GetSelectedManifestRecord()
    {
        if (!_viewState.IsManifestRegistry)
        {
            MessageBox.Show(
                this,
                "Discover the manifest registry first.",
                "Manifest Registry",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            return null;
        }

        if (commandListView.SelectedItems.Count == 0)
        {
            MessageBox.Show(
                this,
                "Select a manifest first.",
                "Manifest Registry",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            return null;
        }

        if (commandListView.SelectedItems[0].Tag is ExtensionManifestRecord record)
        {
            return record;
        }

        MessageBox.Show(
            this,
            "Selected row does not contain manifest metadata.",
            "Manifest Registry",
            MessageBoxButtons.OK,
            MessageBoxIcon.Warning);

        return null;
    }

    private void SetSelectedManifestEnabled(
        bool enabled)
    {
        ExtensionManifestRecord? record =
            GetSelectedManifestRecord();

        if (record is null)
        {
            return;
        }

        ExtensionManifestUpdateResult result =
            _controller.SetManifestEnabled(record.ManifestPath, enabled);

        if (!result.Success)
        {
            MessageBox.Show(
                this,
                result.Message,
                "Manifest Registry",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);

            return;
        }

        DiscoverManifestRegistry(this, EventArgs.Empty);
    }

    private void SaveDashboardSettingsMenu(
        object? sender,
        EventArgs e)
    {
        SaveDashboardSettings();
    }

    private void ReloadDashboardSettingsMenu(
        object? sender,
        EventArgs e)
    {
        LoadDashboardSettings();
    }

    private void ResetFolderToAppFolder(
        object? sender,
        EventArgs e)
    {
        folderTextBox.Text = AppContext.BaseDirectory;
        recursiveCheckBox.Checked = false;
        SaveDashboardSettings();
    }

    private void UseModulesFolder(
        object? sender,
        EventArgs e)
    {
        SetFolderAndSave(
            ExtensionOutputFolderResolver.FindModulesFolder(),
            recursive: true);
    }

    private void UseWebsitesAddinDebugOutput(
        object? sender,
        EventArgs e)
    {
        SetFolderAndSave(
            ExtensionOutputFolderResolver.GetWebsitesAddinDebugOutputFolder(),
            recursive: false);
    }

    private void UseWebsitesAddinReleaseOutput(
        object? sender,
        EventArgs e)
    {
        SetFolderAndSave(
            ExtensionOutputFolderResolver.GetWebsitesAddinReleaseOutputFolder(),
            recursive: false);
    }

    private void UseFirstExistingWebsitesAddinOutput(
        object? sender,
        EventArgs e)
    {
        SetFolderAndSave(
            ExtensionOutputFolderResolver.GetFirstExistingWebsitesAddinOutputFolder(),
            recursive: false);
    }

    private void SetFolderAndSave(
        string folder,
        bool recursive)
    {
        folderTextBox.Text = folder;
        recursiveCheckBox.Checked = recursive;
        SaveDashboardSettings();

        if (!Directory.Exists(folder))
        {
            MessageBox.Show(
                this,
                $"Folder does not exist yet. Build the project first:{Environment.NewLine}{folder}",
                "CommandEngine Runtime",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
    }

    private void LoadDashboardSettings()
    {
        ExtensionRuntimeDashboardSettings settings =
            _settingsStore.Load();

        if (!string.IsNullOrWhiteSpace(settings.FolderPath))
        {
            folderTextBox.Text = settings.FolderPath;
        }

        recursiveCheckBox.Checked = settings.Recursive;
        _showDuplicateManifests = settings.ShowDuplicateManifests;
        showDuplicateManifestRowsMenuItem.Checked = _showDuplicateManifests;
    }

    private void SaveDashboardSettings()
    {
        try
        {
            _settingsStore.Save(new ExtensionRuntimeDashboardSettings
            {
                FolderPath = folderTextBox.Text,
                Recursive = recursiveCheckBox.Checked,
                ShowDuplicateManifests = _showDuplicateManifests
            });
        }
        catch
        {
            // Settings persistence should never break the dashboard.
        }
    }

    private void TryRefresh(
        Func<ExtensionRuntimeManagerSnapshot> action)
    {
        ArgumentNullException.ThrowIfNull(action);

        try
        {
            RefreshFromSnapshot(action());
            SaveDashboardSettings();
        }
        catch (Exception exception)
        {
            MessageBox.Show(
                this,
                exception.Message,
                "CommandEngine Runtime",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);

            RefreshFromSnapshot(_controller.GetSnapshot());
        }
    }

    private async void ExecuteSelectedCommandAsync(
        object? sender,
        EventArgs e)
    {
        if (_viewState.IsManifestRegistry)
        {
            TryRefresh(() => _controller.StartFromManifests(folderTextBox.Text, GetEffectiveRecursive()));

            MessageBox.Show(
                this,
                "The selected row was a manifest, not a command. CommandEngine has started from manifests. Select a command row and choose Execute Selected again.",
                "CommandEngine Runtime",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            return;
        }

        if (!TryGetSelectedCommandName(out string commandName))
        {
            return;
        }

        await ExecuteCommandAndRecordAsync(commandName, "{}").ConfigureAwait(true);
    }

    private async void ExecuteSelectedCommandWithParameters(
        object? sender,
        EventArgs e)
    {
        if (_viewState.IsManifestRegistry)
        {
            MessageBox.Show(
                this,
                "Start the runtime from manifests first, then select an actual command row.",
                "CommandEngine",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            return;
        }

        if (!TryGetSelectedCommandName(out string commandName))
        {
            return;
        }

        using CommandParameterEditorDialog dialog = new()
        {
            ParameterJson = _controller.GetParameterTemplateJson(commandName)
        };

        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        await ExecuteCommandAndRecordAsync(commandName, dialog.ParameterJson).ConfigureAwait(true);
    }

    private async Task ExecuteCommandAndRecordAsync(
        string commandName,
        string parameterJson)
    {
        DateTimeOffset startedUtc =
            DateTimeOffset.UtcNow;

        try
        {
            IReadOnlyDictionary<string, object?> parameters =
                CommandParameterJsonParser.Parse(parameterJson);

            CommandExecutionResult result =
                parameters.Count == 0
                    ? await _controller.ExecuteCommandAsync(commandName).ConfigureAwait(true)
                    : await _controller.ExecuteCommandAsync(commandName, parameters).ConfigureAwait(true);

            RecordExecution(startedUtc, result, parameterJson);
            ShowExecutionResult(result);
        }
        catch (Exception exception)
        {
            MessageBox.Show(
                this,
                exception.Message,
                "CommandEngine Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }

    private void RecordExecution(
        DateTimeOffset startedUtc,
        CommandExecutionResult result,
        string parameterJson)
    {
        _historyStore.Add(new CommandExecutionHistoryRecord
        {
            StartedUtc = startedUtc,
            CompletedUtc = DateTimeOffset.UtcNow,
            CommandName = result.CommandName,
            CorrelationId = result.CorrelationId,
            Status = result.Status.ToString(),
            Message = result.Message,
            ParameterJson = string.IsNullOrWhiteSpace(parameterJson) ? "{}" : parameterJson,
            OutputJson = result.OutputJson
        });
    }

    private void ShowSelectedCommandMetadata(
        object? sender,
        EventArgs e)
    {
        if (_viewState.IsManifestRegistry)
        {
            MessageBox.Show(
                this,
                "Start the runtime and select a command row first.",
                "CommandEngine",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            return;
        }

        if (!TryGetSelectedCommandName(out string commandName))
        {
            return;
        }

        diagnosticsTextBox.Text =
            CommandMetadataTextFormatter.Format(_controller.FindCommandMetadata(commandName));
    }

    private bool TryGetSelectedCommandName(
        out string commandName)
    {
        commandName = string.Empty;

        if (!_viewState.IsRuntimeCommands)
        {
            MessageBox.Show(
                this,
                "Switch back to runtime commands before executing.",
                "CommandEngine",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            return false;
        }

        if (commandListView.SelectedItems.Count == 0)
        {
            MessageBox.Show(
                this,
                "Select a command first.",
                "CommandEngine",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            return false;
        }

        commandName = commandListView.SelectedItems[0].Text;
        return true;
    }

    private CommandExecutionHistoryRecord? GetSelectedHistoryRecord()
    {
        if (!_viewState.IsExecutionHistory)
        {
            MessageBox.Show(
                this,
                "Show execution history first.",
                "CommandEngine History",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            return null;
        }

        if (commandListView.SelectedItems.Count == 0)
        {
            MessageBox.Show(
                this,
                "Select a history row first.",
                "CommandEngine History",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            return null;
        }

        if (commandListView.SelectedItems[0].Tag is CommandExecutionHistoryRecord record)
        {
            return record;
        }

        MessageBox.Show(
            this,
            "Selected row does not contain history metadata.",
            "CommandEngine History",
            MessageBoxButtons.OK,
            MessageBoxIcon.Warning);

        return null;
    }

    private async void ReplaySelectedHistory(
        object? sender,
        EventArgs e)
    {
        CommandExecutionHistoryRecord? record =
            GetSelectedHistoryRecord();

        if (record is null)
        {
            return;
        }

        DialogResult response =
            MessageBox.Show(
                this,
                $"Replay command '{record.CommandName}' with its saved parameters?",
                "Replay Command",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

        if (response != DialogResult.Yes)
        {
            return;
        }

        await ExecuteCommandAndRecordAsync(record.CommandName, record.ParameterJson).ConfigureAwait(true);
    }

    private void ShowSelectedHistoryDetails(
        object? sender,
        EventArgs e)
    {
        CommandExecutionHistoryRecord? record =
            GetSelectedHistoryRecord();

        if (record is null)
        {
            return;
        }

        diagnosticsTextBox.Text =
            CommandExecutionHistoryTextFormatter.Format([record]);
    }

    private void CopySelectedHistoryParameters(
        object? sender,
        EventArgs e)
    {
        CommandExecutionHistoryRecord? record =
            GetSelectedHistoryRecord();

        if (record is not null)
        {
            Clipboard.SetText(record.ParameterJson);
        }
    }

    private void CopySelectedHistoryOutput(
        object? sender,
        EventArgs e)
    {
        CommandExecutionHistoryRecord? record =
            GetSelectedHistoryRecord();

        if (record is not null)
        {
            Clipboard.SetText(record.OutputJson);
        }
    }

    private void OpenHistoryFile(
        object? sender,
        EventArgs e)
    {
        if (_historyStore is not ICommandExecutionHistoryFileStore fileStore)
        {
            MessageBox.Show(
                this,
                "The active history store does not expose a history file.",
                "CommandEngine History",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            return;
        }

        EnsureHistoryFileExists(fileStore.HistoryPath);
        OpenPath(fileStore.HistoryPath);
    }

    private void OpenHistoryFolder(
        object? sender,
        EventArgs e)
    {
        if (_historyStore is not ICommandExecutionHistoryFileStore fileStore)
        {
            MessageBox.Show(
                this,
                "The active history store does not expose a history file.",
                "CommandEngine History",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            return;
        }

        EnsureHistoryFileExists(fileStore.HistoryPath);

        string? folder =
            Path.GetDirectoryName(fileStore.HistoryPath);

        if (!string.IsNullOrWhiteSpace(folder))
        {
            OpenPath(folder);
        }
    }

    private static void EnsureHistoryFileExists(
        string historyPath)
    {
        if (File.Exists(historyPath))
        {
            return;
        }

        string? folder =
            Path.GetDirectoryName(historyPath);

        if (!string.IsNullOrWhiteSpace(folder))
        {
            Directory.CreateDirectory(folder);
        }

        File.WriteAllText(historyPath, "[]");
    }

    private void ShowExecutionResult(
        CommandExecutionResult result)
    {
        ArgumentNullException.ThrowIfNull(result);

        diagnosticsTextBox.Text =
            CommandExecutionResultTextFormatter.Format(result);

        summaryLabel.Text =
            $"{result.Status}: {result.CommandName}";

        UpdateStatusPanel(
            DashboardViewModeTextFormatter.Format(_viewState.Mode),
            result.Status.ToString(),
            commandListView.Items.Count);

        MessageBox.Show(
            this,
            $"{result.Status}: {result.Message}",
            "CommandEngine Result",
            MessageBoxButtons.OK,
            result.Status == CommandExecutionStatus.Succeeded
                ? MessageBoxIcon.Information
                : MessageBoxIcon.Warning);
    }

    private void ShowExecutionHistory(
        object? sender,
        EventArgs e)
    {
        _viewState.SetMode(DashboardViewMode.ExecutionHistory);

        IReadOnlyList<CommandExecutionHistoryRecord> records =
            _historyStore.ListAll();

        summaryLabel.Text =
            $"Execution history: {records.Count} record(s).";

        UpdateStatusPanel(
            DashboardViewModeTextFormatter.Format(_viewState.Mode),
            "History",
            records.Count);

        CommandExecutionHistoryListViewBuilder.ConfigureColumns(commandListView);
        CommandExecutionHistoryListViewBuilder.Populate(commandListView, records);

        diagnosticsTextBox.Text =
            CommandExecutionHistoryTextFormatter.Format(records);
    }

    private void ClearExecutionHistory(
        object? sender,
        EventArgs e)
    {
        DialogResult response =
            MessageBox.Show(
                this,
                "Clear all execution history?",
                "Clear Execution History",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

        if (response != DialogResult.Yes)
        {
            return;
        }

        _historyStore.Clear();
        ShowExecutionHistory(sender, e);
    }

    private void CopySelectedCommandName(
        object? sender,
        EventArgs e)
    {
        if (commandListView.SelectedItems.Count == 0)
        {
            MessageBox.Show(
                this,
                "Select a row first.",
                "CommandEngine",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            return;
        }

        Clipboard.SetText(commandListView.SelectedItems[0].Text);
    }

    private void CopyDiagnostics(
        object? sender,
        EventArgs e)
    {
        Clipboard.SetText(diagnosticsTextBox.Text);
    }

    private static void OpenPath(
        string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return;
        }

        Process.Start(new ProcessStartInfo
        {
            FileName = path,
            UseShellExecute = true
        });
    }

    private void RefreshFromSnapshot(
        ExtensionRuntimeManagerSnapshot snapshot)
    {
        _viewState.SetMode(DashboardViewMode.RuntimeCommands);

        ExtensionRuntimeHostViewModel host =
            snapshot.Host;

        summaryLabel.Text =
            $"{snapshot.Status}: {host.Summary}";

        commandListView.Columns.Clear();
        commandListView.Columns.Add("Command", 280);
        commandListView.Columns.Add("Display Name", 240);
        commandListView.Columns.Add("Category", 200);
        commandListView.Columns.Add("Enabled", 80);
        commandListView.Items.Clear();

        foreach (ExtensionRuntimeHostCommandViewModel command in host.Commands)
        {
            ListViewItem item = new(command.CommandName);
            item.SubItems.Add(command.DisplayName);
            item.SubItems.Add(command.Category);
            item.SubItems.Add(command.IsEnabled ? "Yes" : "No");
            commandListView.Items.Add(item);
        }

        UpdateStatusPanel(
            DashboardViewModeTextFormatter.Format(_viewState.Mode),
            snapshot.Status.ToString(),
            commandListView.Items.Count);

        diagnosticsTextBox.Text =
            snapshot.DiagnosticText;
    }

    private void RefreshFromManifestRegistry(
        ExtensionManifestRegistryViewModel viewModel)
    {
        _viewState.SetMode(DashboardViewMode.ManifestRegistry);

        IReadOnlyList<ExtensionManifestRecord> visibleRecords =
            _showDuplicateManifests
                ? viewModel.Records
                : ExtensionManifestRegistryFilter.PreferSingleRecordPerExtension(viewModel.Records);

        summaryLabel.Text =
            _showDuplicateManifests
                ? viewModel.Summary
                : $"{viewModel.Summary} Showing {visibleRecords.Count} preferred manifest row(s).";

        commandListView.Columns.Clear();
        commandListView.Columns.Add("Extension Id", 180);
        commandListView.Columns.Add("Display Name", 210);
        commandListView.Columns.Add("Display Sort", 90);
        commandListView.Columns.Add("Version", 80);
        commandListView.Columns.Add("Enabled", 70);
        commandListView.Columns.Add("Duplicate", 80);
        commandListView.Columns.Add("Assembly", 160);
        commandListView.Columns.Add("Manifest Folder", 420);
        commandListView.Items.Clear();

        foreach (ExtensionManifestRecord record in visibleRecords)
        {
            ListViewItem item = new(record.ExtensionId)
            {
                Tag = record
            };

            item.SubItems.Add(record.DisplayName);
            item.SubItems.Add(record.DisplaySort.ToString());
            item.SubItems.Add(record.Version);
            item.SubItems.Add(record.Enabled ? "Yes" : "No");
            item.SubItems.Add(record.IsDuplicate ? $"Yes ({record.DuplicateCount})" : "No");
            item.SubItems.Add(record.AssemblyFile);
            item.SubItems.Add(record.ManifestFolder);
            commandListView.Items.Add(item);
        }

        UpdateStatusPanel(
            DashboardViewModeTextFormatter.Format(_viewState.Mode),
            "Registry",
            commandListView.Items.Count);

        diagnosticsTextBox.Text =
            ExtensionManifestRegistryTextFormatter.Format(viewModel);
    }

    private bool TryPromptForDisplaySort(
        ExtensionManifestRecord record,
        out int displaySort)
    {
        using Form dialog =
            new()
            {
                Text = "Set Display Sort",
                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition = FormStartPosition.CenterParent,
                MinimizeBox = false,
                MaximizeBox = false,
                ClientSize = new Size(320, 120)
            };

        Label label =
            new()
            {
                AutoSize = true,
                Location = new Point(12, 14),
                Text = $"{record.DisplayName} display sort:"
            };

        NumericUpDown input =
            new()
            {
                Location = new Point(12, 42),
                Width = 120,
                Minimum = -100000,
                Maximum = 100000,
                Value = Math.Clamp(record.DisplaySort, -100000, 100000)
            };

        Button ok =
            new()
            {
                Text = "OK",
                DialogResult = DialogResult.OK,
                Location = new Point(154, 78),
                Width = 72
            };

        Button cancel =
            new()
            {
                Text = "Cancel",
                DialogResult = DialogResult.Cancel,
                Location = new Point(234, 78),
                Width = 72
            };

        dialog.Controls.Add(label);
        dialog.Controls.Add(input);
        dialog.Controls.Add(ok);
        dialog.Controls.Add(cancel);
        dialog.AcceptButton = ok;
        dialog.CancelButton = cancel;

        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            displaySort = 0;
            return false;
        }

        displaySort = (int)input.Value;
        return true;
    }
    private static void EnsureAddinFolderStructure()
    {
        ExtensionManagerAddinTestCatalog catalog = new();
        AddinFolderStructureInitializer initializer = new();

        initializer.EnsureCreated(
            catalog
                .GetAddins()
                .Select(static addin => addin.AddinId));
    }

}
