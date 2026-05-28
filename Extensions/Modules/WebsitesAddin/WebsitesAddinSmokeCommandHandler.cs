using Codex.CommandEngine.Core;

namespace WebsitesAddin;

public sealed class WebsitesAddinSmokeCommandHandler : ICommandHandler
{
    public const string RegisteredCommandName = "websites.smoke.test";

    public string CommandName => RegisteredCommandName;

    public Task<CommandExecutionResult> ExecuteAsync(
        CommandExecutionRequest request,
        ICommandExecutionContext context)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(context);

        context.WriteLog("Websites add-in smoke command executed.");

        return Task.FromResult(
            CommandExecutionResult.Succeeded(
                request.CommandName,
                request.CorrelationId,
                "Websites add-in smoke test completed.",
                """
                {"ok":true,"source":"WebsitesAddin"}
                """));
    }
}
