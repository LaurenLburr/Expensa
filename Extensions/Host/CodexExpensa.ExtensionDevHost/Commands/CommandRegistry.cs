using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace CodexExpensa.ExtensionDevHost.Commands;

public sealed class CommandRegistry
{
    private readonly Dictionary<string, IExtMgrCommand> _commands = new(StringComparer.OrdinalIgnoreCase);
    private readonly ICommandMenuConfigStore? _configStore;

    public CommandRegistry(ICommandMenuConfigStore? configStore = null)
    {
        _configStore = configStore;
    }

    public event EventHandler? MenuDefinitionChanged;

    public void Register(IExtMgrCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);

        if (_commands.ContainsKey(command.CommandKey))
        {
            throw new InvalidOperationException($"Command '{command.CommandKey}' is already registered.");
        }

        _commands.Add(command.CommandKey, command);
    }

    public IReadOnlyCollection<IExtMgrCommand> GetCommands() => _commands.Values;

    public void Invoke(string commandKey, Form owner)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(commandKey);
        ArgumentNullException.ThrowIfNull(owner);

        if (!_commands.TryGetValue(commandKey, out IExtMgrCommand? command))
        {
            throw new InvalidOperationException($"Command '{commandKey}' is not registered.");
        }

        command.Execute(owner);
    }

    public void LoadConfig()
    {
        if (_configStore is null)
        {
            return;
        }

        ApplyConfig(_configStore.Load(), raiseChanged: false);
    }

    public void SaveConfig()
    {
        if (_configStore is null)
        {
            return;
        }

        _configStore.Save(BuildConfigDocument());
    }

    public string? GetConfigFilePath() => _configStore?.FilePath;

    public void UpdateMetadata(string commandKey, string menuText, int menuOrder, int itemOrder)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(commandKey);

        if (_commands.TryGetValue(commandKey, out IExtMgrCommand? command) &&
            command is ExtMgrCommandBase editable)
        {
            editable.SetMetadata(menuText, menuOrder, itemOrder);
            SaveConfig();
            OnMenuDefinitionChanged();
        }
    }

    public void PopulateMenu(MenuStrip menuStrip, Form owner)
    {
        ArgumentNullException.ThrowIfNull(menuStrip);
        ArgumentNullException.ThrowIfNull(owner);

        menuStrip.Items.Clear();

        var groupedCommands = _commands.Values
            .GroupBy(static x => x.TopLevelMenu)
            .OrderBy(group => group.Min(static x => x.MenuOrder))
            .ThenBy(static group => group.Key, StringComparer.OrdinalIgnoreCase);

        foreach (var group in groupedCommands)
        {
            ToolStripMenuItem topLevel = new(group.Key);
            bool lastWasSeparator = false;

            foreach (IExtMgrCommand command in group
                .OrderBy(static x => x.ItemOrder)
                .ThenBy(static x => x.MenuText, StringComparer.OrdinalIgnoreCase))
            {
                if (command.IsSeparator)
                {
                    if (topLevel.DropDownItems.Count == 0 || lastWasSeparator)
                    {
                        continue;
                    }

                    topLevel.DropDownItems.Add(new ToolStripSeparator());
                    lastWasSeparator = true;
                    continue;
                }

                ToolStripMenuItem item = new(command.MenuText);
                item.Click += (_, _) => Invoke(command.CommandKey, owner);
                topLevel.DropDownItems.Add(item);
                lastWasSeparator = false;
            }

            if (topLevel.DropDownItems.Count > 0 &&
                topLevel.DropDownItems[topLevel.DropDownItems.Count - 1] is ToolStripSeparator)
            {
                topLevel.DropDownItems.RemoveAt(topLevel.DropDownItems.Count - 1);
            }

            if (topLevel.DropDownItems.Count > 0)
            {
                menuStrip.Items.Add(topLevel);
            }
        }
    }

    private void ApplyConfig(CommandMenuConfigDocument document, bool raiseChanged)
    {
        foreach (CommandMenuConfigEntry entry in document.Commands)
        {
            if (_commands.TryGetValue(entry.CommandKey, out IExtMgrCommand? command) &&
                command is ExtMgrCommandBase editable)
            {
                editable.SetMetadata(entry.MenuText, entry.MenuOrder, entry.ItemOrder);
            }
        }

        if (raiseChanged)
        {
            OnMenuDefinitionChanged();
        }
    }

    private CommandMenuConfigDocument BuildConfigDocument()
    {
        return new CommandMenuConfigDocument
        {
            Commands = _commands.Values
                .OfType<ExtMgrCommandBase>()
                .Select(static x => x.ToConfigEntry())
                .OrderBy(static x => x.MenuOrder)
                .ThenBy(static x => x.ItemOrder)
                .ThenBy(static x => x.CommandKey, StringComparer.OrdinalIgnoreCase)
                .ToList()
        };
    }

    private void OnMenuDefinitionChanged()
    {
        MenuDefinitionChanged?.Invoke(this, EventArgs.Empty);
    }
}
