using System.Diagnostics;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration;

public sealed partial class ExtensionRuntimeDashboardForm
{
    private ICommandExecutionPersistenceStore? _persistenceStore;

    private ICommandExecutionPersistenceStore PersistenceStore
    {
        get
        {
            if (_persistenceStore is not null)
            {
                return _persistenceStore;
            }

            _persistenceStore =
                new SqliteCommandExecutionPersistenceStore(
                    new CommandExecutionPersistenceOptions());

            _persistenceStore.EnsureCreated();

            return _persistenceStore;
        }
    }

    private void PersistHistoryRecord(
        CommandExecutionHistoryRecord record)
    {
        try
        {
            PersistenceStore.UpsertHistoryRecord(record);
        }
        catch (Exception exception)
        {
            diagnosticsTextBox.Text =
                $"Failed to persist history record:{Environment.NewLine}{exception}";
        }
    }

    private void PersistQueueSnapshot(
        CommandExecutionQueueSnapshot snapshot)
    {
        try
        {
            foreach (CommandExecutionQueueItem item in snapshot.Items)
            {
                PersistenceStore.UpsertQueueItem(item);
            }
        }
        catch (Exception exception)
        {
            diagnosticsTextBox.Text =
                $"Failed to persist queue snapshot:{Environment.NewLine}{exception}";
        }
    }

    private void OpenExecutionDatabase(
        object? sender,
        EventArgs e)
    {
        if (PersistenceStore is not ICommandExecutionPersistenceFileStore fileStore)
        {
            MessageBox.Show(
                this,
                "The active persistence store does not expose a database file.",
                "Command Execution Persistence",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            return;
        }

        PersistenceStore.EnsureCreated();
        OpenPath(fileStore.DatabasePath);
    }

    private void OpenExecutionDatabaseFolder(
        object? sender,
        EventArgs e)
    {
        if (PersistenceStore is not ICommandExecutionPersistenceFileStore fileStore)
        {
            MessageBox.Show(
                this,
                "The active persistence store does not expose a database file.",
                "Command Execution Persistence",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            return;
        }

        PersistenceStore.EnsureCreated();

        string? folder =
            Path.GetDirectoryName(fileStore.DatabasePath);

        if (string.IsNullOrWhiteSpace(folder))
        {
            return;
        }

        Process.Start(new ProcessStartInfo
        {
            FileName = folder,
            UseShellExecute = true
        });
    }

    private void CopyExecutionDatabasePath(
        object? sender,
        EventArgs e)
    {
        if (PersistenceStore is ICommandExecutionPersistenceFileStore fileStore)
        {
            Clipboard.SetText(fileStore.DatabasePath);
        }
    }
}
