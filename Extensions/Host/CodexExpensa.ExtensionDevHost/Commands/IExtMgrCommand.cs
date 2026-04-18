using System.Windows.Forms;

namespace CodexExpensa.ExtensionDevHost.Commands;

public interface IExtMgrCommand
{
    string CommandKey { get; }
    string TopLevelMenu { get; }
    string MenuText { get; }
    int MenuOrder { get; }
    int ItemOrder { get; }
    bool IsSeparator { get; }
    void Execute(Form owner);
}
