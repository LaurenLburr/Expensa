using Codex.CommandEngine.Core;
using System.Reflection;
using System.Runtime.Loader;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Budgets;

public sealed class HostBudgetRuntimeModuleInvoker
{
    private readonly HostBudgetRuntimeDatabaseSelectionService databaseSelectionService;

    private const string BudgetsAddinAssemblyName = "BudgetsAddin.dll";
    private const string SmokeRunnerTypeName = "BudgetsAddin.BudgetLoadRuntimeSmokeRunner";
    private const string RequestTypeName = "BudgetsAddin.BudgetLoadRequest";
    private const string DefaultCommandName = "Budgets.LoadTree";

    public HostBudgetRuntimeModuleInvoker()
        : this(new HostBudgetRuntimeDatabaseSelectionService())
    {
    }

    public HostBudgetRuntimeModuleInvoker(
        HostBudgetRuntimeDatabaseSelectionService databaseSelectionService)
    {
        ArgumentNullException.ThrowIfNull(databaseSelectionService);

        this.databaseSelectionService = databaseSelectionService;
    }

    public async Task<CommandExecutionResult> ExecuteAsync(
        HostBudgetRuntimeLoadRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        string assemblyPath = FindBudgetsAddinAssemblyPath();

        BudgetAddinDependencyLoadContext loadContext = new(assemblyPath);
        Assembly assembly = loadContext.LoadMainAssembly(assemblyPath);

        Type smokeRunnerType = assembly.GetType(SmokeRunnerTypeName, throwOnError: true)!;
        Type requestType = assembly.GetType(RequestTypeName, throwOnError: true)!;

        object smokeRunner =
            Activator.CreateInstance(smokeRunnerType)
            ?? throw new InvalidOperationException("Could not create BudgetLoadRuntimeSmokeRunner.");

        object loadRequest =
            Activator.CreateInstance(requestType)
            ?? throw new InvalidOperationException("Could not create BudgetLoadRequest.");

        HostBudgetDatabaseLocation activeDatabaseLocation =
            databaseSelectionService.GetActiveRuntimeDatabaseLocation();

        SetProperty(loadRequest, "SearchText", request.SearchText);
        SetProperty(loadRequest, "IncludeClosed", request.IncludeClosed);
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

        object? taskObject = executeMethod.Invoke(smokeRunner, [loadRequest, cancellationToken]);

        if (taskObject is not Task task)
        {
            throw new InvalidOperationException("Budget smoke runner did not return a Task.");
        }

        await task.ConfigureAwait(false);

        PropertyInfo resultProperty =
            task.GetType().GetProperty("Result")
            ?? throw new InvalidOperationException("Budget smoke runner task did not expose Result.");

        object? result = resultProperty.GetValue(task);

        if (result is null)
        {
            throw new InvalidOperationException("Budget smoke runner returned null.");
        }

        return MapCommandExecutionResult(result);
    }

    public static string FindBudgetsAddinAssemblyPath()
    {
        foreach (string candidatePath in GetBudgetsAddinAssemblyCandidatePaths())
        {
            string fullPath = Path.GetFullPath(candidatePath);

            if (File.Exists(fullPath))
            {
                return fullPath;
            }
        }

        string candidates = string.Join(Environment.NewLine, GetBudgetsAddinAssemblyCandidatePaths().Select(Path.GetFullPath));

        throw new FileNotFoundException(
            $"Could not locate {BudgetsAddinAssemblyName}. Build BudgetsAddin first.{Environment.NewLine}{candidates}");
    }

    private static CommandExecutionResult MapCommandExecutionResult(object result)
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

    private static IReadOnlyList<string> GetBudgetsAddinAssemblyCandidatePaths()
    {
        List<string> candidatePaths =
        [
            Path.Combine(AppContext.BaseDirectory, "Modules", "BudgetsAddin", BudgetsAddinAssemblyName),
            Path.Combine(Directory.GetCurrentDirectory(), "Modules", "BudgetsAddin", BudgetsAddinAssemblyName)
        ];

        foreach (string root in EnumerateAncestorFolders(AppContext.BaseDirectory))
        {
            candidatePaths.Add(Path.Combine(root, "Modules", "BudgetsAddin", BudgetsAddinAssemblyName));
            candidatePaths.Add(Path.Combine(root, "Modules", "BudgetsAddin", "bin", "Debug", "net8.0-windows", BudgetsAddinAssemblyName));
            candidatePaths.Add(Path.Combine(root, "Modules", "BudgetsAddin", "bin", "Release", "net8.0-windows", BudgetsAddinAssemblyName));
        }

        return candidatePaths.Distinct(StringComparer.OrdinalIgnoreCase).ToList();
    }

    private static IEnumerable<string> EnumerateAncestorFolders(string startPath)
    {
        DirectoryInfo? directory = new(startPath);

        while (directory is not null)
        {
            yield return directory.FullName;
            directory = directory.Parent;
        }
    }

    private static void SetProperty(object instance, string propertyName, object value)
    {
        PropertyInfo property =
            instance.GetType().GetProperty(propertyName)
            ?? throw new MissingMemberException(instance.GetType().FullName, propertyName);

        property.SetValue(instance, value);
    }

    private static string GetPropertyString(object instance, string propertyName, string defaultValue)
    {
        PropertyInfo? property = instance.GetType().GetProperty(propertyName);
        if (property is null) return defaultValue;

        string? text = Convert.ToString(property.GetValue(instance));
        return string.IsNullOrWhiteSpace(text) ? defaultValue : text;
    }

    private static CommandExecutionStatus GetPropertyEnum(object instance, string propertyName, CommandExecutionStatus defaultValue)
    {
        PropertyInfo? property = instance.GetType().GetProperty(propertyName);
        if (property is null) return defaultValue;

        object? value = property.GetValue(instance);
        if (value is CommandExecutionStatus status) return status;

        string? text = Convert.ToString(value);
        return Enum.TryParse(text, ignoreCase: true, out CommandExecutionStatus parsedStatus)
            ? parsedStatus
            : defaultValue;
    }

    private sealed class BudgetAddinDependencyLoadContext : AssemblyLoadContext
    {
        private readonly AssemblyDependencyResolver resolver;

        public BudgetAddinDependencyLoadContext(string mainAssemblyPath)
            : base(isCollectible: false)
        {
            resolver = new AssemblyDependencyResolver(mainAssemblyPath);
        }

        public Assembly LoadMainAssembly(string assemblyPath) => LoadFromAssemblyPath(assemblyPath);

        protected override Assembly? Load(AssemblyName assemblyName)
        {
            string? assemblyPath = resolver.ResolveAssemblyToPath(assemblyName);
            return assemblyPath is null ? null : LoadFromAssemblyPath(assemblyPath);
        }

        protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
        {
            string? libraryPath = resolver.ResolveUnmanagedDllToPath(unmanagedDllName);
            return libraryPath is null ? IntPtr.Zero : LoadUnmanagedDllFromPath(libraryPath);
        }
    }
}
