using System;
using System.IO;
using System.Windows.Forms;
using CodexExpensa.ExtensionDevHost.Commands;

namespace CodexExpensa.ExtensionDevHost;

public class MainForm : Form
{
    private readonly CommandRegistry _commandRegistry;
    private MenuStrip? _menuStrip;
    private readonly string _commandConfigPath;

    public MainForm()
    {
        Text = "Extension Dev Host";
        Width = 900;
        Height = 600;

        _commandConfigPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Expensa",
            "Extensions",
            "CommandMenuConfig.json");

        _commandRegistry = new CommandRegistry(new JsonCommandMenuConfigStore(_commandConfigPath));
        _commandRegistry.MenuDefinitionChanged += (_, _) => RebuildMenu();

        RegisterCommands();
        _commandRegistry.LoadConfig();
        InitializeUi();
    }

    public CommandRegistry CommandRegistry => _commandRegistry;

    public string CommandConfigPath => _commandConfigPath;

    private void RegisterCommands()
    {
        _commandRegistry.Register(new NewProjectSpaceCommand());
        _commandRegistry.Register(new QueryCatalogMenuSpacerCommand());
        _commandRegistry.Register(new OpenQueryCatalogCommand());
        _commandRegistry.Register(new OpenCommandCatalogCommand());
        _commandRegistry.Register(new ExitApplicationCommand());
    }

    private void InitializeUi()
    {
        _menuStrip = new MenuStrip();
        Controls.Add(_menuStrip);

        RebuildMenu();
    }

    public void RebuildMenu()
    {
        if (_menuStrip is null)
        {
            return;
        }

        _commandRegistry.PopulateMenu(_menuStrip, this);
        MainMenuStrip = _menuStrip;
        PerformLayout();
        Refresh();
    }
}
