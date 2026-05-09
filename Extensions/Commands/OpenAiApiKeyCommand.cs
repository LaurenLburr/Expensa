using CodexExpensa.ExtensionDevHost.Commands.Abstractions;
using CodexExpensa.ExtensionDevHost.UI;

namespace CodexExpensa.ExtensionDevHost.Commands;

public sealed class OpenAiApiKeyCommand : ExtMgrCommandBase
{
    public override string CommandKey => "Project.OpenAiApiKey";

    public override string TopLevelMenu => "Project";

    protected override string GetDefaultMenuText() => "OpenAI API Key";

    protected override int GetDefaultMenuOrder() => 100;

    protected override int GetDefaultItemOrder() => 20;

    public override void Execute(ICommandContext context)
    {
        var ui = GetUi(context);

        using OpenAiApiKeyForm dialog = new();
        ui.ShowDialog(dialog);
    }
}
