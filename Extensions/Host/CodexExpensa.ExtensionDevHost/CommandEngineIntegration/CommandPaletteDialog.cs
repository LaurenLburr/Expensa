namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration;

public sealed partial class CommandPaletteDialog : Form
{
    private readonly IReadOnlyList<CommandPaletteItem> _commands;

    public CommandPaletteDialog(
        IReadOnlyList<CommandPaletteItem> commands)
    {
        ArgumentNullException.ThrowIfNull(commands);

        _commands = commands
            .OrderBy(static command => command.Category, StringComparer.OrdinalIgnoreCase)
            .ThenBy(static command => command.DisplayName, StringComparer.OrdinalIgnoreCase)
            .ThenBy(static command => command.CommandName, StringComparer.OrdinalIgnoreCase)
            .ToList();

        InitializeComponent();
        PopulateCommandList(_commands);
        UpdatePreviewFromSelection();
    }

    public string SelectedCommandName { get; private set; } = string.Empty;

    public bool ExecuteWithParameters =>
        executeWithParametersCheckBox.Checked;

    private void searchTextBox_TextChanged(
        object? sender,
        EventArgs e)
    {
        string filter =
            searchTextBox.Text.Trim();

        IReadOnlyList<CommandPaletteItem> filtered =
            string.IsNullOrWhiteSpace(filter)
                ? _commands
                : _commands
                    .Where(command => command.SearchText.Contains(filter, StringComparison.OrdinalIgnoreCase))
                    .ToList();

        PopulateCommandList(filtered);
        UpdatePreviewFromSelection();
    }

    private void commandListView_SelectedIndexChanged(
        object? sender,
        EventArgs e)
    {
        UpdatePreviewFromSelection();
    }

    private void commandListView_DoubleClick(
        object? sender,
        EventArgs e)
    {
        AcceptSelection();
    }

    private void executeButton_Click(
        object? sender,
        EventArgs e)
    {
        AcceptSelection();
    }

    private void cancelButton_Click(
        object? sender,
        EventArgs e)
    {
        DialogResult = DialogResult.Cancel;
        Close();
    }

    private void PopulateCommandList(
        IReadOnlyList<CommandPaletteItem> commands)
    {
        commandListView.BeginUpdate();

        try
        {
            commandListView.Items.Clear();

            foreach (CommandPaletteItem command in commands)
            {
                ListViewItem item = new(command.CommandName)
                {
                    Tag = command
                };

                item.SubItems.Add(command.DisplayName);
                item.SubItems.Add(command.Category);
                item.SubItems.Add(command.IsEnabled ? "Yes" : "No");

                commandListView.Items.Add(item);
            }

            if (commandListView.Items.Count > 0)
            {
                commandListView.Items[0].Selected = true;
                commandListView.Items[0].Focused = true;
            }
        }
        finally
        {
            commandListView.EndUpdate();
        }
    }

    private void UpdatePreviewFromSelection()
    {
        commandPreviewTextBox.Text =
            CommandPalettePreviewTextFormatter.Format(GetSelectedCommand());
    }

    private CommandPaletteItem? GetSelectedCommand()
    {
        if (commandListView.SelectedItems.Count == 0)
        {
            return null;
        }

        return commandListView.SelectedItems[0].Tag as CommandPaletteItem;
    }

    private void AcceptSelection()
    {
        CommandPaletteItem? command =
            GetSelectedCommand();

        if (command is null)
        {
            MessageBox.Show(
                this,
                "Select a command first.",
                "Command Palette",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            return;
        }

        if (!command.IsEnabled)
        {
            DialogResult response =
                MessageBox.Show(
                    this,
                    "The selected command is disabled. Execute it anyway?",
                    "Command Palette",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

            if (response != DialogResult.Yes)
            {
                return;
            }
        }

        SelectedCommandName = command.CommandName;
        DialogResult = DialogResult.OK;
        Close();
    }
}
