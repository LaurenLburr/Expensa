using CodexExpensa.App.WinForms.Composition;

namespace CodexExpensa.App.WinForms;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();

        try
        {
            var bootstrapper = new AppBootstrapper();
            var mainForm = bootstrapper.Initialize();

            Application.Run(mainForm);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Startup failed:\n\n{ex}",
                "Codex Expensa",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }
}