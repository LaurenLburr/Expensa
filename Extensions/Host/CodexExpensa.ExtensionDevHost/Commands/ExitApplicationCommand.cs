using CodexExpensa.ExtensionDevHost.Commands.Abstractions;

namespace CodexExpensa.ExtensionDevHost.Commands;

public sealed class ExitApplicationCommand : ExtMgrCommandBase
{
    public override string CommandKey => "File.Exit";

    public override string TopLevelMenu => "File";

    protected override string GetDefaultMenuText() => "Exit";

    protected override int GetDefaultMenuOrder() => 900;

    protected override int GetDefaultItemOrder() => 1000;

    public override void Execute(ICommandContext context)
    {
        GetOwner(context).Close();
    }
}
