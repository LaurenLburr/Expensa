using Codex.CommandEngine.Core;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration;

public sealed class ExtensionRuntimeManager : IExtensionRuntimeManager
{
    private readonly ExtensionRuntimeManagerOptions _options;
    private readonly IRuntimeProviderDiscoveryService _discoveryService;
    private readonly RuntimeProviderManifestDiscoveryService _manifestDiscoveryService;
    private readonly ExtensionRuntimeHost _host = new();
    private RuntimeProviderDiscoveryResult _lastDiscoveryResult = new();

    public ExtensionRuntimeManager()
        : this(new ExtensionRuntimeManagerOptions())
    {
    }

    public ExtensionRuntimeManager(ExtensionRuntimeManagerOptions options)
        : this(options, new RuntimeProviderDiscoveryService(), new RuntimeProviderManifestDiscoveryService())
    {
    }

    public ExtensionRuntimeManager(
        ExtensionRuntimeManagerOptions options,
        IRuntimeProviderDiscoveryService discoveryService)
        : this(options, discoveryService, new RuntimeProviderManifestDiscoveryService())
    {
    }

    public ExtensionRuntimeManager(
        ExtensionRuntimeManagerOptions options,
        IRuntimeProviderDiscoveryService discoveryService,
        RuntimeProviderManifestDiscoveryService manifestDiscoveryService)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(discoveryService);
        ArgumentNullException.ThrowIfNull(manifestDiscoveryService);

        _options = options;
        _discoveryService = discoveryService;
        _manifestDiscoveryService = manifestDiscoveryService;
    }

    public ExtensionRuntimeManagerStatus Status => MapStatus(_host.State);

    public void Start(IEnumerable<IRuntimeCommandRegistrationProvider> providers)
    {
        ArgumentNullException.ThrowIfNull(providers);

        _lastDiscoveryResult = new RuntimeProviderDiscoveryResult
        {
            Providers = providers.ToList()
        };

        _host.Start(CreateBootstrapRequest(_lastDiscoveryResult.Providers));
    }

    public void StartFromLoadedAssemblies()
    {
        _lastDiscoveryResult = _discoveryService.DiscoverFromLoadedAssemblies();
        _host.Start(CreateBootstrapRequest(_lastDiscoveryResult.Providers));
    }

    public void StartFromFolder(string folderPath, bool recursive = false)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(folderPath);

        _lastDiscoveryResult = _discoveryService.DiscoverFromFolder(folderPath, recursive);
        _host.Start(CreateBootstrapRequest(_lastDiscoveryResult.Providers));
    }

    public void StartFromManifests(string folderPath, bool recursive = false)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(folderPath);

        _lastDiscoveryResult = _manifestDiscoveryService.DiscoverProvidersFromFolder(folderPath, recursive);
        _host.Start(CreateBootstrapRequest(_lastDiscoveryResult.Providers));
    }

    public void Reload(IEnumerable<IRuntimeCommandRegistrationProvider> providers)
    {
        ArgumentNullException.ThrowIfNull(providers);

        _lastDiscoveryResult = new RuntimeProviderDiscoveryResult
        {
            Providers = providers.ToList()
        };

        _host.Reload(CreateBootstrapRequest(_lastDiscoveryResult.Providers));
    }

    public void ReloadFromLoadedAssemblies()
    {
        _lastDiscoveryResult = _discoveryService.DiscoverFromLoadedAssemblies();
        _host.Reload(CreateBootstrapRequest(_lastDiscoveryResult.Providers));
    }

    public void ReloadFromFolder(string folderPath, bool recursive = false)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(folderPath);

        _lastDiscoveryResult = _discoveryService.DiscoverFromFolder(folderPath, recursive);
        _host.Reload(CreateBootstrapRequest(_lastDiscoveryResult.Providers));
    }

    public void ReloadFromManifests(string folderPath, bool recursive = false)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(folderPath);

        _lastDiscoveryResult = _manifestDiscoveryService.DiscoverProvidersFromFolder(folderPath, recursive);
        _host.Reload(CreateBootstrapRequest(_lastDiscoveryResult.Providers));
    }

    public void Stop()
    {
        _host.Stop();
    }

    public ExtensionRuntimeManagerSnapshot GetSnapshot()
    {
        ExtensionRuntimeHostSnapshot snapshot = _host.GetSnapshot();
        ExtensionRuntimeHostViewModel viewModel = ExtensionRuntimeHostViewModelFactory.Create(snapshot);

        string diagnosticText = ExtensionRuntimeHostSnapshotTextFormatter.Format(snapshot);

        if (_lastDiscoveryResult.HasIssues)
        {
            diagnosticText += Environment.NewLine + Environment.NewLine;
            diagnosticText += RuntimeProviderDiscoveryTextFormatter.Format(_lastDiscoveryResult);
        }

        return new ExtensionRuntimeManagerSnapshot
        {
            Status = MapStatus(snapshot.State),
            Host = viewModel,
            DiagnosticText = diagnosticText
        };
    }

    public IReadOnlyList<RuntimeCommandDescriptor> ListCommands()
    {
        return _host.ListCommands();
    }

    public Task<CommandExecutionResult> ExecuteCommandAsync(
        string commandName,
        IReadOnlyDictionary<string, object?>? parameters = null,
        string contextJson = "{}",
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(commandName);

        return _host.ExecuteCommandAsync(
            new ExtensionRuntimeCommandRequest
            {
                CommandName = commandName,
                CorrelationId = Guid.NewGuid().ToString("N"),
                ContextJson = contextJson,
                Parameters = parameters ?? new Dictionary<string, object?>()
            },
            cancellationToken);
    }

    private ExtensionCommandRuntimeBootstrapRequest CreateBootstrapRequest(IEnumerable<IRuntimeCommandRegistrationProvider> providers)
    {
        return new ExtensionCommandRuntimeBootstrapRequest
        {
            Providers = providers.ToList(),
            ThrowIfNoCommands = _options.ThrowIfNoCommands,
            ContinueOnRegistrationError = _options.ContinueOnRegistrationError
        };
    }

    private static ExtensionRuntimeManagerStatus MapStatus(ExtensionRuntimeHostState state)
    {
        return state switch
        {
            ExtensionRuntimeHostState.NotStarted => ExtensionRuntimeManagerStatus.NotStarted,
            ExtensionRuntimeHostState.Started => ExtensionRuntimeManagerStatus.Started,
            ExtensionRuntimeHostState.Failed => ExtensionRuntimeManagerStatus.Failed,
            ExtensionRuntimeHostState.Stopped => ExtensionRuntimeManagerStatus.Stopped,
            _ => ExtensionRuntimeManagerStatus.Failed
        };
    }
}
