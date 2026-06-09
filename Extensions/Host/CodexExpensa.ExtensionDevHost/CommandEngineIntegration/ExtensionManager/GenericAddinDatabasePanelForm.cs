namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.ExtensionManager;

public  class GenericAddinDatabasePanelForm : Form
{
    public GenericAddinDatabasePanelForm(string projectName, string projectFolder)
    {
        Text = $"{projectName} Database";
        Dock = DockStyle.Fill;

        TextBox textBox = new()
        {
            Dock = DockStyle.Fill,
            Multiline = true,
            ReadOnly = true,
            ScrollBars = ScrollBars.Both,
            WordWrap = false,
            Text =
                $"{projectName} Database{Environment.NewLine}{Environment.NewLine}" +
                $"Project folder:{Environment.NewLine}{projectFolder}{Environment.NewLine}{Environment.NewLine}" +
                "This add-in does not have a custom database panel yet. The generic add-in database panel is being shown."
        };

        Controls.Add(textBox);
    }
}
