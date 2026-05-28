namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration;

public static class CommandExecutionHistoryListViewBuilder
{
    public static void ConfigureColumns(
        ListView listView)
    {
        ArgumentNullException.ThrowIfNull(listView);

        listView.Columns.Clear();
        listView.Columns.Add("Started UTC", 160);
        listView.Columns.Add("Command", 280);
        listView.Columns.Add("Status", 100);
        listView.Columns.Add("Duration ms", 100);
        listView.Columns.Add("Message", 420);
    }

    public static void Populate(
        ListView listView,
        IReadOnlyList<CommandExecutionHistoryRecord> records)
    {
        ArgumentNullException.ThrowIfNull(listView);
        ArgumentNullException.ThrowIfNull(records);

        listView.Items.Clear();

        foreach (CommandExecutionHistoryRecord record in records)
        {
            ListViewItem item = new(record.StartedUtc.ToString("u"))
            {
                Tag = record
            };

            item.SubItems.Add(record.CommandName);
            item.SubItems.Add(record.Status);
            item.SubItems.Add(record.Duration.TotalMilliseconds.ToString("0"));
            item.SubItems.Add(record.Message);

            listView.Items.Add(item);
        }
    }
}
