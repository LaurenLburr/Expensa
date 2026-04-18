
using System.Windows.Forms;

namespace CodexExpensa.ExtensionDevHost;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();
        Application.Run(new MainForm());
    }
}
