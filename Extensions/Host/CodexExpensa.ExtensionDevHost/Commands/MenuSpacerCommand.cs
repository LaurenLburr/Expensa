using System.Windows.Forms;

namespace CodexExpensa.ExtensionDevHost.Commands;

public abstract class MenuSpacerCommand : ExtMgrCommandBase
{
    protected override string GetDefaultMenuText() => "-";

    public override bool IsSeparator => true;

    public override void Execute(Form owner)
    {
    }
}
