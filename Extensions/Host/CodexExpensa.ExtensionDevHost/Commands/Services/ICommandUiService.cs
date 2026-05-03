using System.Windows.Forms;

namespace CodexExpensa.ExtensionDevHost.Commands.Services;

public interface ICommandUiService
{
    Form Owner { get; }

    void Show(Form form);

    DialogResult ShowDialog(Form form);
}
