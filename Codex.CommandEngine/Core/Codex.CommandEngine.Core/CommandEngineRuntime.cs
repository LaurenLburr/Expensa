namespace Codex.CommandEngine.Core;

public sealed class CommandEngineRuntime : ICommandEngineRuntime
{
    private readonly ICommandDispatcher _commandDispatcher;
    private readonly IWorkflowRunner _workflowRunner;
    private readonly Dictionary<string, RuntimeCommandDescriptor> _registeredCommands =
        new(StringComparer.OrdinalIgnoreCase);

    public CommandEngineRuntime()
        : this(new CommandDispatcher())
    {
    }

    public CommandEngineRuntime(ICommandDispatcher commandDispatcher)
    {
        ArgumentNullException.ThrowIfNull(commandDispatcher);

        _commandDispatcher = commandDispatcher;
        _workflowRunner = new SequentialWorkflowRunner(commandDispatcher);
    }

    public CommandEngineRuntime(
        ICommandDispatcher commandDispatcher,
        IWorkflowRunner workflowRunner)
    {
        ArgumentNullException.ThrowIfNull(commandDispatcher);
        ArgumentNullException.ThrowIfNull(workflowRunner);

        _commandDispatcher = commandDispatcher;
        _workflowRunner = workflowRunner;
    }

    public void RegisterCommand(ICommandHandler handler)
    {
        ArgumentNullException.ThrowIfNull(handler);

        RegisterCommand(new RuntimeCommandRegistration
        {
            Handler = handler
        });
    }

    public void RegisterCommand(RuntimeCommandRegistration registration)
    {
        ArgumentNullException.ThrowIfNull(registration);
        ArgumentNullException.ThrowIfNull(registration.Handler);
        ArgumentException.ThrowIfNullOrWhiteSpace(registration.Handler.CommandName);

        if (registration.Version <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(registration),
                registration.Version,
                "Command version must be greater than zero.");
        }

        RuntimeCommandDescriptor descriptor =
            registration.ToDescriptor();

        if (_registeredCommands.ContainsKey(descriptor.CommandName))
        {
            throw new InvalidOperationException(
                $"A command is already registered for '{descriptor.CommandName}'.");
        }

        if (descriptor.IsEnabled)
        {
            _commandDispatcher.Register(registration.Handler);
        }

        _registeredCommands.Add(descriptor.CommandName, descriptor);
    }

    public void RegisterCommands(IEnumerable<RuntimeCommandRegistration> registrations)
    {
        ArgumentNullException.ThrowIfNull(registrations);

        foreach (RuntimeCommandRegistration registration in registrations)
        {
            RegisterCommand(registration);
        }
    }

    public IReadOnlyList<string> ListRegisteredCommandNames()
    {
        return _registeredCommands.Values
            .OrderBy(static descriptor => descriptor.CommandName, StringComparer.OrdinalIgnoreCase)
            .Select(static descriptor => descriptor.CommandName)
            .ToList();
    }

    public IReadOnlyList<RuntimeCommandDescriptor> ListRegisteredCommands()
    {
        return _registeredCommands.Values
            .OrderBy(static descriptor => descriptor.Category, StringComparer.OrdinalIgnoreCase)
            .ThenBy(static descriptor => descriptor.CommandName, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    public bool TryGetRegisteredCommand(
        string commandName,
        out RuntimeCommandDescriptor descriptor)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(commandName);

        if (_registeredCommands.TryGetValue(commandName, out RuntimeCommandDescriptor? resolvedDescriptor))
        {
            descriptor = resolvedDescriptor;
            return true;
        }

        descriptor = null!;
        return false;
    }

    public Task<CommandExecutionResult> ExecuteCommandAsync(
        CommandExecutionRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        return _commandDispatcher.ExecuteAsync(request, cancellationToken);
    }

    public Task<WorkflowExecutionResult> ExecuteWorkflowAsync(
        WorkflowExecutionRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        return _workflowRunner.ExecuteAsync(request, cancellationToken);
    }
}
