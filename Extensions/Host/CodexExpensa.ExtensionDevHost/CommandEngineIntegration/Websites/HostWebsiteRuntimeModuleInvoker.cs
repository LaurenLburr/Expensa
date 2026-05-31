using Codex.CommandEngine.Core;
using System.Reflection;

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

    public async Task<CommandExecutionResult> ExecuteAsync(
        HostWebsiteRuntimeLoadRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        string assemblyPath =
            FindWebsitesAddinAssemblyPath();

        Assembly assembly =
            Assembly.LoadFrom(assemblyPath);

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

        return result as CommandExecutionResult
            ?? throw new InvalidOperationException("Website smoke runner did not return CommandExecutionResult.");
    }

    public static string FindWebsitesAddinAssemblyPath()
    {
        IReadOnlyList<string> candidatePaths =
        [
            Path.Combine(
                AppContext.BaseDirectory,
                "Modules",
                "WebsitesAddin",
                WebsitesAddinAssemblyName),

            Path.Combine(
                AppContext.BaseDirectory,
                "..",
                "..",
                "..",
                "..",
                "..",
                "Modules",
                "WebsitesAddin",
                "bin",
                "Debug",
                "net8.0",
                WebsitesAddinAssemblyName),

            Path.Combine(
                AppContext.BaseDirectory,
                "..",
                "..",
                "..",
                "..",
                "..",
                "Modules",
                "WebsitesAddin",
                "bin",
                "Release",
                "net8.0",
                WebsitesAddinAssemblyName),

            Path.Combine(
                Directory.GetCurrentDirectory(),
                "Modules",
                "WebsitesAddin",
                WebsitesAddinAssemblyName)
        ];

        foreach (string candidatePath in candidatePaths)
        {
            string fullPath =
                Path.GetFullPath(candidatePath);

            if (File.Exists(fullPath))
            {
                return fullPath;
            }
        }

        throw new FileNotFoundException(
            $"Could not locate {WebsitesAddinAssemblyName}. Build WebsitesAddin first or copy it under the Modules folder.");
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
}
