namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration;

public sealed partial class ExtensionRuntimeDashboardForm
{
    private readonly HashSet<Guid> _queueItemsRecordedToHistory = [];

    private ICommandExecutionQueue? _executionQueue;

    private ICommandExecutionQueue ExecutionQueue
    {
        get
        {
            if (_executionQueue is not null)
            {
                return _executionQueue;
            }

            _executionQueue = new InMemoryCommandExecutionQueue(_controller);
            _executionQueue.Changed += ExecutionQueue_Changed;

            return _executionQueue;
        }
    }

    private void QueueSelectedCommand(
        object? sender,
        EventArgs e)
    {
        if (!TryGetSelectedCommandForQueue(out string commandName))
        {
            return;
        }

        RecoverPersistentQueueIfNeeded();

        ExecutionQueue.Enqueue(commandName, "{}");
        ShowExecutionQueue(sender, e);
    }

    private void QueueSelectedCommandWithParameters(
        object? sender,
        EventArgs e)
    {
        if (!TryGetSelectedCommandForQueue(out string commandName))
        {
            return;
        }

        using CommandParameterEditorDialog parameterDialog = new()
        {
            ParameterJson = _controller.GetParameterTemplateJson(commandName)
        };

        if (parameterDialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        RecoverPersistentQueueIfNeeded();

        ExecutionQueue.Enqueue(commandName, parameterDialog.ParameterJson);
        ShowExecutionQueue(sender, e);
    }

    private void ShowExecutionQueue(
        object? sender,
        EventArgs e)
    {
        _viewState.SetMode(DashboardViewMode.ExecutionQueue);

        RecoverPersistentQueueIfNeeded();
        RefreshExecutionQueueView();
    }

    private void CancelSelectedQueueItem(
        object? sender,
        EventArgs e)
    {
        CommandExecutionQueueItem? item =
            GetSelectedQueueItem();

        if (item is null)
        {
            return;
        }

        if (!ExecutionQueue.TryCancel(item.QueueItemId))
        {
            MessageBox.Show(
                this,
                "The selected queue item could not be cancelled.",
                "Command Queue",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        RefreshExecutionQueueView();
    }

    private void ClearCompletedQueueItems(
        object? sender,
        EventArgs e)
    {
        ExecutionQueue.ClearCompleted();
        PersistenceStore.DeleteCompletedQueueItems();
        RefreshExecutionQueueView();
    }

    private void ShowSelectedQueueItemDetails(
        object? sender,
        EventArgs e)
    {
        CommandExecutionQueueItem? item =
            GetSelectedQueueItem();

        if (item is null)
        {
            return;
        }

        diagnosticsTextBox.Text =
            CommandExecutionQueueTextFormatter.Format(new CommandExecutionQueueSnapshot
            {
                Items = [item]
            });
    }

    private void CopySelectedQueueParameters(
        object? sender,
        EventArgs e)
    {
        CommandExecutionQueueItem? item =
            GetSelectedQueueItem();

        if (item is not null)
        {
            Clipboard.SetText(item.ParameterJson);
        }
    }

    private void CopySelectedQueueOutput(
        object? sender,
        EventArgs e)
    {
        CommandExecutionQueueItem? item =
            GetSelectedQueueItem();

        if (item is not null)
        {
            Clipboard.SetText(item.OutputJson);
        }
    }

    private void CopyQueueReport(
        object? sender,
        EventArgs e)
    {
        Clipboard.SetText(
            CommandExecutionQueueTextFormatter.Format(ExecutionQueue.GetSnapshot()));
    }

    private bool TryGetSelectedCommandForQueue(
        out string commandName)
    {
        commandName = string.Empty;

        if (_viewState.IsManifestRegistry)
        {
            MessageBox.Show(
                this,
                "Start the runtime from manifests first, then select an actual command row.",
                "Command Queue",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            return false;
        }

        if (!_viewState.IsRuntimeCommands)
        {
            MessageBox.Show(
                this,
                "Switch back to runtime commands before queueing.",
                "Command Queue",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            return false;
        }

        if (commandListView.SelectedItems.Count == 0)
        {
            MessageBox.Show(
                this,
                "Select a command first.",
                "Command Queue",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            return false;
        }

        commandName = commandListView.SelectedItems[0].Text;
        return true;
    }

    private CommandExecutionQueueItem? GetSelectedQueueItem()
    {
        if (!_viewState.IsExecutionQueue)
        {
            MessageBox.Show(
                this,
                "Show the command queue first.",
                "Command Queue",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            return null;
        }

        if (commandListView.SelectedItems.Count == 0)
        {
            MessageBox.Show(
                this,
                "Select a queue item first.",
                "Command Queue",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            return null;
        }

        if (commandListView.SelectedItems[0].Tag is CommandExecutionQueueItem item)
        {
            return item;
        }

        MessageBox.Show(
            this,
            "Selected row does not contain queue metadata.",
            "Command Queue",
            MessageBoxButtons.OK,
            MessageBoxIcon.Warning);

        return null;
    }

    private void RefreshExecutionQueueView()
    {
        CommandExecutionQueueSnapshot snapshot =
            ExecutionQueue.GetSnapshot();

        PersistQueueSnapshot(snapshot);
        RecordCompletedQueueItemsToHistory(snapshot);

        summaryLabel.Text =
            $"Queue: {snapshot.PendingCount} pending, {snapshot.RunningCount} running, {snapshot.CompletedCount} completed, {snapshot.FailedCount} failed, {snapshot.CancelledCount} cancelled.";

        UpdateStatusPanel(
            DashboardViewModeTextFormatter.Format(_viewState.Mode),
            "Active",
            snapshot.Items.Count);

        CommandExecutionQueueListViewBuilder.ConfigureColumns(commandListView);
        CommandExecutionQueueListViewBuilder.Populate(commandListView, snapshot);

        diagnosticsTextBox.Text =
            CommandExecutionQueueTextFormatter.Format(snapshot);
    }

    private void RecordCompletedQueueItemsToHistory(
        CommandExecutionQueueSnapshot snapshot)
    {
        foreach (CommandExecutionQueueItem item in snapshot.Items)
        {
            if (_queueItemsRecordedToHistory.Contains(item.QueueItemId))
            {
                continue;
            }

            if (!CommandExecutionQueueHistoryMapper.ShouldRecordToHistory(item))
            {
                continue;
            }

            CommandExecutionHistoryRecord record =
                CommandExecutionQueueHistoryMapper.ToHistoryRecord(item);

            _historyStore.Add(record);
            PersistHistoryRecord(record);

            _queueItemsRecordedToHistory.Add(item.QueueItemId);
        }
    }

    private void ExecutionQueue_Changed(
        object? sender,
        EventArgs e)
    {
        if (IsDisposed)
        {
            return;
        }

        if (InvokeRequired)
        {
            BeginInvoke(new Action(RefreshQueueIfVisibleOrRecordHistory));
            return;
        }

        RefreshQueueIfVisibleOrRecordHistory();
    }

    private void RefreshQueueIfVisibleOrRecordHistory()
    {
        CommandExecutionQueueSnapshot snapshot =
            ExecutionQueue.GetSnapshot();

        PersistQueueSnapshot(snapshot);
        RecordCompletedQueueItemsToHistory(snapshot);

        if (_viewState.IsExecutionQueue)
        {
            RefreshExecutionQueueView();
        }
    }
}
