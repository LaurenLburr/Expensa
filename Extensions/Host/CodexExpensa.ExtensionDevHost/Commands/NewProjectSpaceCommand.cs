using System.Windows.Forms;

namespace CodexExpensa.ExtensionDevHost.Commands;

public sealed class NewProjectSpaceCommand : ExtMgrCommandBase
{
    public override string CommandKey => "ProjectSpace.New";

    public override string TopLevelMenu => "Project Space";

    protected override string GetDefaultMenuText() => "New Project Space";

    protected override int GetDefaultMenuOrder() => 100;

    protected override int GetDefaultItemOrder() => 100;

    public override void Execute(Form owner)
    {
        using Form? dialog = CreateFormIfAvailable(owner, "CodexExpensa.ExtensionDevHost.ProjectSpaceForm");
        if (dialog is null)
        {
            ShowNotAvailable(owner, "Project Space Setup");
            return;
        }

        dialog.ShowDialog(owner);
    }
}
