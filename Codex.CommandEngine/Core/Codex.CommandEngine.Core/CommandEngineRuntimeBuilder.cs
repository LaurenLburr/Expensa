namespace Codex.CommandEngine.Core;

public sealed class CommandEngineRuntimeBuilder
{
    private readonly List<RuntimeCommandRegistration> _commands = [];
    private bool _continueOnRegistrationError;

    public CommandEngineRuntimeBuilder AddCommand(
        RuntimeCommandRegistration registration)
    {
        ArgumentNullException.ThrowIfNull(registration);
        ArgumentNullException.ThrowIfNull(registration.Handler);

        _commands.Add(registration);
        return this;
    }

    public CommandEngineRuntimeBuilder AddCommand(
        ICommandHandler handler)
    {
        ArgumentNullException.ThrowIfNull(handler);

        return AddCommand(new RuntimeCommandRegistration
        {
            Handler = handler
        });
    }

    public CommandEngineRuntimeBuilder AddCommands(
        IEnumerable<RuntimeCommandRegistration> registrations)
    {
        ArgumentNullException.ThrowIfNull(registrations);

        foreach (RuntimeCommandRegistration registration in registrations)
        {
            AddCommand(registration);
        }

        return this;
    }

    public CommandEngineRuntimeBuilder ContinueOnRegistrationError()
    {
        _continueOnRegistrationError = true;
        return this;
    }

    public CommandEngineRuntimeBootstrapResult Build()
    {
        CommandEngineRuntimeBootstrapper bootstrapper = new();

        return bootstrapper.Bootstrap(new CommandEngineRuntimeBootstrapRequest
        {
            Commands = _commands,
            ContinueOnRegistrationError = _continueOnRegistrationError
        });
    }

    public CommandEngineRuntimeBootstrapResult BuildRequired()
    {
        CommandEngineRuntimeBootstrapper bootstrapper = new();

        return bootstrapper.Bootstrap(new CommandEngineRuntimeBootstrapRequest
        {
            Commands = _commands,
            ThrowIfNoCommands = true,
            ContinueOnRegistrationError = _continueOnRegistrationError
        });
    }
}
