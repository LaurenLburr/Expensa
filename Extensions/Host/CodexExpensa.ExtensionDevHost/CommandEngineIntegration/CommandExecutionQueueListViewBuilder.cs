namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration;

public static class CommandExecutionQueueListViewBuilder
{
    public static void ConfigureColumns(
        ListView listView)
    {
        ArgumentNullException.ThrowIfNull(listView);

        listView.Columns.Clear();
        listView.Columns.Add("Created UTC", 160);
        listView.Columns.Add("Command", 280);
        listView.Columns.Add("Status", 100);
        listView.Columns.Add("Duration ms", 100);
        listView.Columns.Add("Message", 420);
    }

    public static void Populate(
        ListView listView,
        CommandExecutionQueueSnapshot snapshot)
    {
        ArgumentNullException.ThrowIfNull(listView);
        ArgumentNullException.ThrowIfNull(snapshot);

        listView.Items.Clear();

        foreach (CommandExecutionQueueItem item in snapshot.Items)
        {
            ListViewItem listItem = new(item.CreatedUtc.ToString("u"))
            {
                Tag = item
            };

            listItem.SubItems.Add(item.CommandName);
            listItem.SubItems.Add(item.Status.ToString());
            listItem.SubItems.Add(item.Duration?.TotalMilliseconds.ToString("0") ?? string.Empty);
            listItem.SubItems.Add(item.Message);

            listView.Items.Add(listItem);
        }
    }
}
