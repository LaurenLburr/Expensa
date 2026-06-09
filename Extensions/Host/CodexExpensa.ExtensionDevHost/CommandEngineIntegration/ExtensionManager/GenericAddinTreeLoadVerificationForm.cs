namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.ExtensionManager;

public  class GenericAddinTreeLoadVerificationForm : Form
{
    public GenericAddinTreeLoadVerificationForm(string projectName, string projectFolder)
    {
        Text = $"{projectName} Tree Test";
        Dock = DockStyle.Fill;

        TextBox textBox = new()
        {
            Dock = DockStyle.Fill,
            Multiline = true,
            ReadOnly = true,
            ScrollBars = ScrollBars.Both,
            WordWrap = false,
            Text =
                $"{projectName} Tree Test{Environment.NewLine}{Environment.NewLine}" +
                $"Project folder:{Environment.NewLine}{projectFolder}{Environment.NewLine}{Environment.NewLine}" +
                "This add-in does not have a custom tree test panel yet." + Environment.NewLine +
                "Use Tools > Expensa Add-in Loader Test to verify aggregate tree loading."
        };

        Controls.Add(textBox);
    }
}
