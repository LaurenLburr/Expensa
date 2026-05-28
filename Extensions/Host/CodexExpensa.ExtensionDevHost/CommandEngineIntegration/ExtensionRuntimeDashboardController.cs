using Codex.CommandEngine.Core;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration;

public sealed partial class ExtensionRuntimeDashboardController
{
    private readonly IExtensionRuntimeManager _manager;
    private readonly IExtensionManifestRegistryService _manifestRegistryService;
    private readonly IExtensionManifestEditorService _manifestEditorService;
    private readonly CommandMetadataRegistry _commandMetadataRegistry = new();

    public ExtensionRuntimeDashboardController(IExtensionRuntimeManager manager)
        : this(manager, new ExtensionManifestRegistryService(), new ExtensionManifestEditorService())
    {
    }

    public ExtensionRuntimeDashboardController(
        IExtensionRuntimeManager manager,
        IExtensionManifestRegistryService manifestRegistryService)
        : this(manager, manifestRegistryService, new ExtensionManifestEditorService())
    {
    }

    public ExtensionRuntimeDashboardController(
        IExtensionRuntimeManager manager,
        IExtensionManifestRegistryService manifestRegistryService,
        IExtensionManifestEditorService manifestEditorService)
    {
        ArgumentNullException.ThrowIfNull(manager);
        ArgumentNullException.ThrowIfNull(manifestRegistryService);
        ArgumentNullException.ThrowIfNull(manifestEditorService);

        _manager = manager;
        _manifestRegistryService = manifestRegistryService;
        _manifestEditorService = manifestEditorService;
    }

    public ExtensionRuntimeManagerSnapshot StartSmokeRuntime()
    {
        _manager.Start(SmokeRuntimeProviderFactory.Create());
        RefreshMetadataFromLoadedAssemblies();
        return _manager.GetSnapshot();
    }

    public ExtensionRuntimeManagerSnapshot StartFromLoadedAssemblies()
    {
        _manager.StartFromLoadedAssemblies();
        RefreshMetadataFromLoadedAssemblies();
        return _manager.GetSnapshot();
    }

    public ExtensionRuntimeManagerSnapshot StartFromFolder(string folderPath, bool recursive)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(folderPath);

        _manager.StartFromFolder(folderPath, recursive);
        RefreshMetadataFromFolder(folderPath, recursive);
        return _manager.GetSnapshot();
    }

    public ExtensionRuntimeManagerSnapshot StartFromManifests(string folderPath, bool recursive)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(folderPath);

        _manager.StartFromManifests(folderPath, recursive);
        RefreshMetadataFromFolder(folderPath, recursive);
        return _manager.GetSnapshot();
    }

    public ExtensionRuntimeManagerSnapshot ReloadSmokeRuntime()
    {
        _manager.Reload(SmokeRuntimeProviderFactory.Create());
        RefreshMetadataFromLoadedAssemblies();
        return _manager.GetSnapshot();
    }

    public ExtensionRuntimeManagerSnapshot ReloadFromLoadedAssemblies()
    {
        _manager.ReloadFromLoadedAssemblies();
        RefreshMetadataFromLoadedAssemblies();
        return _manager.GetSnapshot();
    }

    public ExtensionRuntimeManagerSnapshot ReloadFromFolder(string folderPath, bool recursive)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(folderPath);

        _manager.ReloadFromFolder(folderPath, recursive);
        RefreshMetadataFromFolder(folderPath, recursive);
        return _manager.GetSnapshot();
    }

    public ExtensionRuntimeManagerSnapshot ReloadFromManifests(string folderPath, bool recursive)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(folderPath);

        _manager.ReloadFromManifests(folderPath, recursive);
        RefreshMetadataFromFolder(folderPath, recursive);
        return _manager.GetSnapshot();
    }

    public ExtensionManifestRegistryViewModel DiscoverManifestRegistry(string folderPath, bool recursive)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(folderPath);

        ExtensionManifestRegistrySnapshot snapshot =
            _manifestRegistryService.Discover(folderPath, recursive);

        return ExtensionManifestRegistryViewModelFactory.Create(snapshot);
    }

    public ExtensionManifestUpdateResult SetManifestEnabled(string manifestPath, bool enabled)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(manifestPath);

        return _manifestEditorService.SetEnabled(manifestPath, enabled);
    }

    public CommandMetadataRecord? FindCommandMetadata(string commandName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(commandName);

        return _commandMetadataRegistry.Find(commandName);
    }

    public string GetParameterTemplateJson(string commandName)
    {
        return FindCommandMetadata(commandName)?.ParameterTemplateJson ?? "{}";
    }

    public ExtensionRuntimeManagerSnapshot StopRuntime()
    {
        _manager.Stop();
        return _manager.GetSnapshot();
    }

    public ExtensionRuntimeManagerSnapshot GetSnapshot()
    {
        return _manager.GetSnapshot();
    }

    public async Task<CommandExecutionResult> ExecuteCommandAsync(
        string commandName,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(commandName);

        return await _manager.ExecuteCommandAsync(
            commandName,
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    public async Task<CommandExecutionResult> ExecuteCommandAsync(
        string commandName,
        IReadOnlyDictionary<string, object?> parameters,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(commandName);
        ArgumentNullException.ThrowIfNull(parameters);

        return await _manager.ExecuteCommandAsync(
            commandName,
            parameters,
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    private void RefreshMetadataFromLoadedAssemblies()
    {
        foreach (ICommandMetadataProvider provider in AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(GetSafeTypes)
            .Where(static type => typeof(ICommandMetadataProvider).IsAssignableFrom(type))
            .Where(static type => !type.IsAbstract && !type.IsInterface)
            .Where(static type => type.GetConstructor(Type.EmptyTypes) is not null)
            .Select(static type => Activator.CreateInstance(type))
            .OfType<ICommandMetadataProvider>())
        {
            _commandMetadataRegistry.AddRange(provider.GetCommandMetadata());
        }
    }

    private void RefreshMetadataFromFolder(string folderPath, bool recursive)
    {
        if (!Directory.Exists(folderPath))
        {
            return;
        }

        SearchOption option = recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

        foreach (string assemblyPath in Directory.GetFiles(folderPath, "*.dll", option))
        {
            try
            {
                System.Reflection.Assembly assembly = System.Reflection.Assembly.LoadFrom(assemblyPath);

                foreach (ICommandMetadataProvider provider in GetSafeTypes(assembly)
                    .Where(static type => typeof(ICommandMetadataProvider).IsAssignableFrom(type))
                    .Where(static type => !type.IsAbstract && !type.IsInterface)
                    .Where(static type => type.GetConstructor(Type.EmptyTypes) is not null)
                    .Select(static type => Activator.CreateInstance(type))
                    .OfType<ICommandMetadataProvider>())
                {
                    _commandMetadataRegistry.AddRange(provider.GetCommandMetadata());
                }
            }
            catch
            {
                // Metadata discovery must not break runtime execution.
            }
        }
    }

    private static IReadOnlyList<Type> GetSafeTypes(System.Reflection.Assembly assembly)
    {
        try
        {
            return assembly.GetTypes();
        }
        catch (System.Reflection.ReflectionTypeLoadException exception)
        {
            return exception.Types
                .Where(static type => type is not null)
                .Cast<Type>()
                .ToList();
        }
        catch
        {
            return [];
        }
    }
}
