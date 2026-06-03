using Codex.CommandEngine.Core;
using System.Reflection;
using System.Runtime.Loader;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Websites;

public sealed class HostWebsiteRuntimeModuleInvoker
{
    private readonly HostWebsiteRuntimeDatabaseSelectionService _databaseSelectionService;

    public HostWebsiteRuntimeModuleInvoker()
        : this(new HostWebsiteRuntimeDatabaseSelectionService())
    {
    }

    public HostWebsiteRuntimeModuleInvoker(
        HostWebsiteRuntimeDatabaseSelectionService databaseSelectionService)
    {
        ArgumentNullException.ThrowIfNull(databaseSelectionService);

        _databaseSelectionService = databaseSelectionService;
    }

    private const string WebsitesAddinAssemblyName = "WebsitesAddin.dll";
    private const string SmokeRunnerTypeName = "WebsitesAddin.WebsiteLoadRuntimeSmokeRunner";
    private const string RequestTypeName = "WebsitesAddin.WebsiteLoadRequest";
    private const string DefaultCommandName = "Websites.Load";

    public async Task<CommandExecutionResult> ExecuteAsync(
        HostWebsiteRuntimeLoadRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        string assemblyPath =
            FindWebsitesAddinAssemblyPath();

        WebsiteAddinDependencyLoadContext loadContext =
            new(assemblyPath);

        Assembly assembly =
            loadContext.LoadMainAssembly(assemblyPath);

        Type smokeRunnerType =
            assembly.GetType(SmokeRunnerTypeName, throwOnError: true)!;

        Type requestType =
            assembly.GetType(RequestTypeName, throwOnError: true)!;

        object smokeRunner =
            Activator.CreateInstance(smokeRunnerType)
            ?? throw new InvalidOperationException("Could not create WebsiteLoadRuntimeSmokeRunner.");

        object loadRequest =
            Activator.CreateInstance(requestType)
            ?? throw new InvalidOperationException("Could not create WebsiteLoadRequest.");

        HostWebsiteDatabaseLocation activeDatabaseLocation =
            _databaseSelectionService.GetActiveRuntimeDatabaseLocation();

        SetProperty(loadRequest, "SearchText", request.SearchText);
        SetProperty(loadRequest, "IncludeDisabled", request.IncludeDisabled);
        SetProperty(loadRequest, "MaximumRows", request.MaximumRows);
        SetProperty(loadRequest, "DatabasePath", activeDatabaseLocation.DatabasePath);

        MethodInfo executeMethod =
            smokeRunnerType.GetMethod(
                "ExecuteAsync",
                BindingFlags.Public | BindingFlags.Instance,
                binder: null,
                types: [requestType, typeof(CancellationToken)],
                modifiers: null)
            ?? throw new MissingMethodException(smokeRunnerType.FullName, "ExecuteAsync");

        object? taskObject =
            executeMethod.Invoke(smokeRunner, [loadRequest, cancellationToken]);

        if (taskObject is not Task task)
        {
            throw new InvalidOperationException("Website smoke runner did not return a Task.");
        }

        await task.ConfigureAwait(false);

        PropertyInfo resultProperty =
            task.GetType().GetProperty("Result")
            ?? throw new InvalidOperationException("Website smoke runner task did not expose Result.");

        object? result =
            resultProperty.GetValue(task);

        if (result is null)
        {
            throw new InvalidOperationException("Website smoke runner returned null.");
        }

        return MapCommandExecutionResult(result);
    }

    public static string FindWebsitesAddinAssemblyPath()
    {
        foreach (string candidatePath in GetWebsitesAddinAssemblyCandidatePaths())
        {
            string fullPath =
                Path.GetFullPath(candidatePath);

            if (File.Exists(fullPath))
            {
                return fullPath;
            }
        }

        string candidates =
            string.Join(
                Environment.NewLine,
                GetWebsitesAddinAssemblyCandidatePaths()
                    .Select(Path.GetFullPath));

        throw new FileNotFoundException(
            $"Could not locate {WebsitesAddinAssemblyName}. Build WebsitesAddin first or copy it under a Modules folder.{Environment.NewLine}{Environment.NewLine}Searched:{Environment.NewLine}{candidates}");
    }

    private static CommandExecutionResult MapCommandExecutionResult(
        object result)
    {
        if (result is CommandExecutionResult sameContextResult)
        {
            return sameContextResult;
        }

        return new CommandExecutionResult
        {
            CorrelationId = GetPropertyString(result, "CorrelationId", Guid.NewGuid().ToString("N")),
            CommandName = GetPropertyString(result, "CommandName", DefaultCommandName),
            Status = GetPropertyEnum(result, "Status", CommandExecutionStatus.Failed),
            Message = GetPropertyString(result, "Message", string.Empty),
            OutputJson = GetPropertyString(result, "OutputJson", string.Empty)
        };
    }

    private static IReadOnlyList<string> GetWebsitesAddinAssemblyCandidatePaths()
    {
        List<string> candidatePaths =
        [
            Path.Combine(
                AppContext.BaseDirectory,
                "Modules",
                "WebsitesAddin",
                WebsitesAddinAssemblyName),

            Path.Combine(
                Directory.GetCurrentDirectory(),
                "Modules",
                "WebsitesAddin",
                WebsitesAddinAssemblyName)
        ];

        foreach (string root in EnumerateAncestorFolders(AppContext.BaseDirectory))
        {
            candidatePaths.Add(
                Path.Combine(
                    root,
                    "Modules",
                    "WebsitesAddin",
                    WebsitesAddinAssemblyName));

            candidatePaths.Add(
                Path.Combine(
                    root,
                    "Modules",
                    "WebsitesAddin",
                    "bin",
                    "Debug",
                    "net8.0-windows",
                    WebsitesAddinAssemblyName));

            candidatePaths.Add(
                Path.Combine(
                    root,
                    "Modules",
                    "WebsitesAddin",
                    "bin",
                    "Debug",
                    "net8.0",
                    WebsitesAddinAssemblyName));

            candidatePaths.Add(
                Path.Combine(
                    root,
                    "Modules",
                    "WebsitesAddin",
                    "bin",
                    "Release",
                    "net8.0-windows",
                    WebsitesAddinAssemblyName));

            candidatePaths.Add(
                Path.Combine(
                    root,
                    "Modules",
                    "WebsitesAddin",
                    "bin",
                    "Release",
                    "net8.0",
                    WebsitesAddinAssemblyName));
        }

        return candidatePaths
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private static IEnumerable<string> EnumerateAncestorFolders(
        string startPath)
    {
        DirectoryInfo? directory =
            new(startPath);

        while (directory is not null)
        {
            yield return directory.FullName;

            directory = directory.Parent;
        }
    }

    private static void SetProperty(
        object instance,
        string propertyName,
        object value)
    {
        PropertyInfo property =
            instance.GetType().GetProperty(propertyName)
            ?? throw new MissingMemberException(instance.GetType().FullName, propertyName);

        property.SetValue(instance, value);
    }

    private static string GetPropertyString(
        object instance,
        string propertyName,
        string defaultValue)
    {
        PropertyInfo? property =
            instance.GetType().GetProperty(propertyName);

        if (property is null)
        {
            return defaultValue;
        }

        object? value =
            property.GetValue(instance);

        string? text =
            Convert.ToString(value);

        return string.IsNullOrWhiteSpace(text)
            ? defaultValue
            : text;
    }

    private static CommandExecutionStatus GetPropertyEnum(
        object instance,
        string propertyName,
        CommandExecutionStatus defaultValue)
    {
        PropertyInfo? property =
            instance.GetType().GetProperty(propertyName);

        if (property is null)
        {
            return defaultValue;
        }

        object? value =
            property.GetValue(instance);

        if (value is CommandExecutionStatus status)
        {
            return status;
        }

        string? text =
            Convert.ToString(value);

        return Enum.TryParse(
            text,
            ignoreCase: true,
            out CommandExecutionStatus parsedStatus)
                ? parsedStatus
                : defaultValue;
    }

    private sealed class WebsiteAddinDependencyLoadContext : AssemblyLoadContext
    {
        private readonly AssemblyDependencyResolver resolver;

        public WebsiteAddinDependencyLoadContext(
            string mainAssemblyPath)
            : base(isCollectible: false)
        {
            resolver =
                new AssemblyDependencyResolver(mainAssemblyPath);
        }

        public Assembly LoadMainAssembly(
            string assemblyPath)
        {
            return LoadFromAssemblyPath(
                assemblyPath);
        }

        protected override Assembly? Load(
            AssemblyName assemblyName)
        {
            string? assemblyPath =
                resolver.ResolveAssemblyToPath(assemblyName);

            return assemblyPath is null
                ? null
                : LoadFromAssemblyPath(assemblyPath);
        }

        protected override IntPtr LoadUnmanagedDll(
            string unmanagedDllName)
        {
            string? libraryPath =
                resolver.ResolveUnmanagedDllToPath(unmanagedDllName);

            return libraryPath is null
                ? IntPtr.Zero
                : LoadUnmanagedDllFromPath(libraryPath);
        }
    }
}
