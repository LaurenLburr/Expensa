namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration;

public static class CommandExecutionPersistentRecordListViewBuilder
{
    public static void ConfigureColumns(
        ListView listView)
    {
        ArgumentNullException.ThrowIfNull(listView);

        listView.Columns.Clear();
        listView.Columns.Add("Created UTC", 160);
        listView.Columns.Add("Completed UTC", 160);
        listView.Columns.Add("Source", 90);
        listView.Columns.Add("Command", 280);
        listView.Columns.Add("Status", 100);
        listView.Columns.Add("Message", 420);
    }

    public static void Populate(
        ListView listView,
        IReadOnlyList<CommandExecutionPersistentRecord> records)
    {
        ArgumentNullException.ThrowIfNull(listView);
        ArgumentNullException.ThrowIfNull(records);

        listView.Items.Clear();

        foreach (CommandExecutionPersistentRecord record in records)
        {
            ListViewItem item = new(record.CreatedUtc.ToString("u"))
            {
                Tag = record
            };

            item.SubItems.Add(record.CompletedUtc?.ToString("u") ?? string.Empty);
            item.SubItems.Add(record.SourceKind);
            item.SubItems.Add(record.CommandName);
            item.SubItems.Add(record.Status);
            item.SubItems.Add(record.Message);

            listView.Items.Add(item);
        }
    }
}
