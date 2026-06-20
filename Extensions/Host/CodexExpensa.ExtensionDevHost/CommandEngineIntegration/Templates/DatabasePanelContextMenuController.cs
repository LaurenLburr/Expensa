namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration;

internal static class DatabasePanelContextMenuController
{
    private const string ReloadText =
        "Reload add-in database into memory";

    private const string CreateDevText =
        "Create/replace Dev database from add-in database";

    public static void Configure(
        ContextMenuStrip contextMenu,
        Action reloadAction,
        Action openFolderAction,
        string openFolderText)
    {
        ArgumentNullException.ThrowIfNull(contextMenu);
        ArgumentNullException.ThrowIfNull(reloadAction);
        ArgumentNullException.ThrowIfNull(openFolderAction);
        ArgumentException.ThrowIfNullOrWhiteSpace(openFolderText);

        contextMenu.Items.Clear();
        contextMenu.Items.Add(CreateMenuItem(contextMenu, ReloadText, reloadAction));
        contextMenu.Items.Add(new ToolStripSeparator());
        contextMenu.Items.Add(CreateMenuItem(contextMenu, openFolderText, openFolderAction));
    }

    public static void ConfigureDev(
        ContextMenuStrip contextMenu,
        Action reloadAction,
        Action createOrReplaceDevAction,
        Action openFolderAction)
    {
        ArgumentNullException.ThrowIfNull(contextMenu);
        ArgumentNullException.ThrowIfNull(reloadAction);
        ArgumentNullException.ThrowIfNull(createOrReplaceDevAction);
        ArgumentNullException.ThrowIfNull(openFolderAction);

        contextMenu.Items.Clear();
        contextMenu.Items.Add(CreateMenuItem(contextMenu, ReloadText, reloadAction));
        contextMenu.Items.Add(new ToolStripSeparator());
        contextMenu.Items.Add(CreateMenuItem(contextMenu, CreateDevText, createOrReplaceDevAction));
        contextMenu.Items.Add(CreateMenuItem(contextMenu, "Open Dev database folder", openFolderAction));
    }

    private static ToolStripMenuItem CreateMenuItem(
        ContextMenuStrip owner,
        string text,
        Action action)
    {
        ToolStripMenuItem item = new(text);
        item.Click += (_, _) => Execute(owner, action);
        return item;
    }

    private static void Execute(ContextMenuStrip owner, Action action)
    {
        try
        {
            action();
        }
        catch (Exception exception)
        {
            MessageBox.Show(
                owner.SourceControl?.FindForm(),
                exception.Message,
                "Database",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }
}
