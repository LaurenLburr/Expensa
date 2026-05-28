namespace Codex.CommandEngine.Core;

public static class ExtensionRuntimeSmokeTest
{
    public static IExtensionCommandRuntime CreateRuntime()
    {
        return ExtensionCommandRuntimeFactory.CreateRequiredFromProviders(
        [
            new ExtensionSmokeTestRegistrationProvider()
        ]);
    }

    public static Task<CommandExecutionResult> ExecuteAsync(
        CancellationToken cancellationToken = default)
    {
        IExtensionCommandRuntime runtime =
            CreateRuntime();

        return runtime.ExecuteCommandAsync(
            new ExtensionRuntimeCommandRequest
            {
                CommandName = ExtensionSmokeTestCommandHandler.RegisteredCommandName,
                CorrelationId = Guid.NewGuid().ToString("N"),
                ContextJson = "{}"
            },
            cancellationToken);
    }
}
