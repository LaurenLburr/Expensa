using Codex.CommandEngine.Core;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration;

public sealed partial class ExtensionRuntimeDashboardForm
{
    private void ShowCommandPalette(
        object? sender,
        EventArgs e)
    {
        if (!_viewState.IsRuntimeCommands)
        {
            MessageBox.Show(
                this,
                "Switch back to runtime commands before opening the command palette.",
                "Command Palette",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            return;
        }

        IReadOnlyList<CommandPaletteItem> commands =
            commandListView.Items
                .Cast<ListViewItem>()
                .Select(item =>
                {
                    string commandName = item.Text;
                    CommandMetadataRecord? metadata =
                        _controller.FindCommandMetadata(commandName);

                    return new CommandPaletteItem
                    {
                        CommandName = commandName,
                        DisplayName = item.SubItems.Count > 1
                            ? item.SubItems[1].Text
                            : metadata?.DisplayName ?? string.Empty,
                        Category = item.SubItems.Count > 2
                            ? item.SubItems[2].Text
                            : metadata?.Category ?? string.Empty,
                        IsEnabled = item.SubItems.Count <= 3 ||
                            item.SubItems[3].Text.Equals("Yes", StringComparison.OrdinalIgnoreCase),
                        Description = metadata?.Description ?? string.Empty,
                        ParameterTemplateJson = metadata?.ParameterTemplateJson ?? "{}",
                        Notes = metadata?.Notes ?? string.Empty
                    };
                })
                .ToList();

        if (commands.Count == 0)
        {
            MessageBox.Show(
                this,
                "No runtime commands are currently visible.",
                "Command Palette",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            return;
        }

        using CommandPaletteDialog dialog = new(commands);

        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        if (dialog.ExecuteWithParameters)
        {
            ExecuteCommandWithParametersFromPalette(dialog.SelectedCommandName);
            return;
        }

        ExecuteCommandFromPalette(dialog.SelectedCommandName);
    }

    private async void ExecuteCommandFromPalette(
        string commandName)
    {
        await ExecuteCommandAndRecordAsync(commandName, "{}").ConfigureAwait(true);
    }

    private async void ExecuteCommandWithParametersFromPalette(
        string commandName)
    {
        using CommandParameterEditorDialog parameterDialog = new()
        {
            ParameterJson = _controller.GetParameterTemplateJson(commandName)
        };

        if (parameterDialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        await ExecuteCommandAndRecordAsync(commandName, parameterDialog.ParameterJson).ConfigureAwait(true);
    }
}
