namespace Codex.CommandEngine.Core;

public sealed class ExtensionRuntimeHost : IExtensionRuntimeHost
{
    private readonly IExtensionCommandRuntimeBootstrapper _bootstrapper;
    private IExtensionCommandRuntime? _runtime;

    public ExtensionRuntimeHost()
        : this(new ExtensionCommandRuntimeBootstrapper())
    {
    }

    public ExtensionRuntimeHost(
        IExtensionCommandRuntimeBootstrapper bootstrapper)
    {
        ArgumentNullException.ThrowIfNull(bootstrapper);

        _bootstrapper = bootstrapper;
    }

    public ExtensionRuntimeHostState State { get; private set; } =
        ExtensionRuntimeHostState.NotStarted;

    public ExtensionCommandRuntimeBootstrapResult? LastBootstrapResult { get; private set; }

    public ExtensionCommandRuntimeBootstrapResult Start(
        ExtensionCommandRuntimeBootstrapRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        return BootstrapInternal(request);
    }

    public ExtensionCommandRuntimeBootstrapResult Reload(
        ExtensionCommandRuntimeBootstrapRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        ClearRuntime(ExtensionRuntimeHostState.NotStarted);

        return BootstrapInternal(request);
    }

    public void Stop()
    {
        ClearRuntime(ExtensionRuntimeHostState.Stopped);
    }

    public ExtensionRuntimeHostSnapshot GetSnapshot()
    {
        ExtensionCommandRuntimeBootstrapResult? result =
            LastBootstrapResult;

        if (result is null)
        {
            return new ExtensionRuntimeHostSnapshot
            {
                State = State,
                RegisteredCommandCount = 0,
                Diagnostics = RuntimeBootstrapDiagnostics.Empty(),
                Commands = []
            };
        }

        return new ExtensionRuntimeHostSnapshot
        {
            State = State,
            RegisteredCommandCount = result.RegisteredCommands.Count,
            Diagnostics = result.Diagnostics,
            Commands = result.RegisteredCommands
        };
    }

    public IReadOnlyList<RuntimeCommandDescriptor> ListCommands()
    {
        return RequireRuntime().ListCommands();
    }

    public Task<CommandExecutionResult> ExecuteCommandAsync(
        ExtensionRuntimeCommandRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        return RequireRuntime().ExecuteCommandAsync(
            request,
            cancellationToken);
    }

    public Task<WorkflowExecutionResult> ExecuteWorkflowAsync(
        ExtensionRuntimeWorkflowRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        return RequireRuntime().ExecuteWorkflowAsync(
            request,
            cancellationToken);
    }

    private ExtensionCommandRuntimeBootstrapResult BootstrapInternal(
        ExtensionCommandRuntimeBootstrapRequest request)
    {
        try
        {
            ExtensionCommandRuntimeBootstrapResult result =
                _bootstrapper.Bootstrap(request);

            _runtime = result.Runtime;
            LastBootstrapResult = result;

            State = result.HasErrors
                ? ExtensionRuntimeHostState.Failed
                : ExtensionRuntimeHostState.Started;

            return result;
        }
        catch
        {
            ClearRuntime(ExtensionRuntimeHostState.Failed);
            throw;
        }
    }

    private void ClearRuntime(
        ExtensionRuntimeHostState state)
    {
        _runtime = null;
        LastBootstrapResult = null;
        State = state;
    }

    private IExtensionCommandRuntime RequireRuntime()
    {
        if (_runtime is null)
        {
            throw new InvalidOperationException(
                "Extension runtime host has not been started.");
        }

        return _runtime;
    }
}
