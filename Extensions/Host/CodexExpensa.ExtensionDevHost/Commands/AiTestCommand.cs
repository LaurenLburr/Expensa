using System.Windows.Forms;
using CodexExpensa.ExtensionDevHost.Commands.Abstractions;
using CodexExpensa.ExtensionDevHost.Commands.Services;

namespace CodexExpensa.ExtensionDevHost.Commands;

public sealed class AiTestCommand : ExtMgrCommandBase
{
    public override string CommandKey => "Tools.AiTest";
    public override string TopLevelMenu => "Tools";
    protected override string GetDefaultMenuText() => "AI Test";
    protected override int GetDefaultMenuOrder() => 300;
    protected override int GetDefaultItemOrder() => 300;

    public override async void Execute(ICommandContext context)
    {
        var ui = GetUi(context);
        var logger = context.Services.GetRequiredService<ICommandLogger>();
        var ai = context.Services.GetRequiredService<ICommandAiService>();

        try
        {
            logger.Log("AiTestCommand started real AI request.");

            string result = await ai.GenerateScaffoldAsync("Create a sample extension for tracking useful websites by tag.");

            logger.Log("AiTestCommand completed real AI request.");

            MessageBox.Show(
                ui.Owner,
                result,
                "AI Result",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            logger.Log($"AiTestCommand failed: {ex.Message}");

            MessageBox.Show(
                ui.Owner,
                ex.Message,
                "AI Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }
}
