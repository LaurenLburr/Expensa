namespace Codex.CommandEngine.Core;

public sealed class CommandEngineRuntimeBootstrapper : ICommandEngineRuntimeBootstrapper
{
    public CommandEngineRuntimeBootstrapResult Bootstrap(
        CommandEngineRuntimeBootstrapRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request.ThrowIfNoCommands && request.Commands.Count == 0)
        {
            throw new InvalidOperationException("At least one command registration is required.");
        }

        CommandEngineRuntime runtime = new();
        List<RuntimeBootstrapIssue> issues = [];

        foreach (RuntimeCommandRegistration registration in request.Commands)
        {
            try
            {
                runtime.RegisterCommand(registration);
            }
            catch (Exception exception) when (request.ContinueOnRegistrationError)
            {
                issues.Add(new RuntimeBootstrapIssue
                {
                    Severity = RuntimeBootstrapIssueSeverity.Error,
                    CommandName = registration.Handler?.CommandName ?? string.Empty,
                    Message = exception.Message,
                    Exception = exception
                });
            }
        }

        IReadOnlyList<RuntimeCommandDescriptor> registeredCommands =
            runtime.ListRegisteredCommands();

        if (registeredCommands.Count == 0 && request.Commands.Count > 0)
        {
            issues.Add(new RuntimeBootstrapIssue
            {
                Severity = RuntimeBootstrapIssueSeverity.Warning,
                Message = "No commands were registered."
            });
        }

        return new CommandEngineRuntimeBootstrapResult
        {
            Runtime = runtime,
            RegisteredCommands = registeredCommands,
            Diagnostics = new RuntimeBootstrapDiagnostics
            {
                Issues = issues
            }
        };
    }
}
