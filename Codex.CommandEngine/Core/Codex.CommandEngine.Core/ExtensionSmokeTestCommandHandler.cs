namespace Codex.CommandEngine.Core;

public sealed class ExtensionSmokeTestCommandHandler : ICommandHandler
{
    public const string RegisteredCommandName = "extension.smoke.test";

    public string CommandName => RegisteredCommandName;

    public Task<CommandExecutionResult> ExecuteAsync(
        CommandExecutionRequest request,
        ICommandExecutionContext context)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(context);

        context.WriteLog("Extension smoke-test command executed.");

        return Task.FromResult(
            CommandExecutionResult.Succeeded(
                request.CommandName,
                request.CorrelationId,
                "Extension runtime smoke test completed.",
                """
                {"ok":true,"source":"CommandEngine"}
                """));
    }
}
