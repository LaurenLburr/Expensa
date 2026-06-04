using System.Reflection;
using System.Runtime.Loader;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.ExpensaLoader;

public sealed class ExpensaStyleAddinRuntimeInvoker
{
    public async Task<ExpensaAddinLoaderResult> ExecuteAsync(
        ExpensaAddinLoaderRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        ExpensaAddinRuntimeDescriptor descriptor =
            ExpensaAddinRuntimeDescriptorFactory.Create(request.Kind);

        string assemblyPath =
            ExpensaAddinAssemblyLocator.FindAssemblyPath(descriptor);

        string databasePath =
            string.IsNullOrWhiteSpace(request.DatabasePath)
                ? GetDefaultExpensaDatabasePath()
                : request.DatabasePath;

        DependencyLoadContext loadContext = new(assemblyPath);

        Assembly assembly =
            loadContext.LoadMainAssembly(assemblyPath);

        Type smokeRunnerType =
            assembly.GetType(descriptor.SmokeRunnerTypeName, throwOnError: true)!;

        Type requestType =
            assembly.GetType(descriptor.RequestTypeName, throwOnError: true)!;

        object smokeRunner =
            Activator.CreateInstance(smokeRunnerType)
            ?? throw new InvalidOperationException($"Could not create {descriptor.SmokeRunnerTypeName}.");

        object loadRequest =
            Activator.CreateInstance(requestType)
            ?? throw new InvalidOperationException($"Could not create {descriptor.RequestTypeName}.");

        SetProperty(loadRequest, "SearchText", request.SearchText);
        SetIncludeProperty(loadRequest, request);
        SetProperty(loadRequest, "MaximumRows", request.MaximumRows);
        SetProperty(loadRequest, "DatabasePath", databasePath);

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
            throw new InvalidOperationException("Add-in smoke runner did not return a Task.");
        }

        await task.ConfigureAwait(false);

        PropertyInfo resultProperty =
            task.GetType().GetProperty("Result")
            ?? throw new InvalidOperationException("Add-in smoke runner task did not expose Result.");

        object? result =
            resultProperty.GetValue(task);

        if (result is null)
        {
            throw new InvalidOperationException("Add-in smoke runner returned null.");
        }

        return new ExpensaAddinLoaderResult
        {
            AddinName = descriptor.AddinName,
            AssemblyPath = assemblyPath,
            DatabasePath = databasePath,
            CommandName = GetPropertyString(result, "CommandName", descriptor.DefaultCommandName),
            CorrelationId = GetPropertyString(result, "CorrelationId", Guid.NewGuid().ToString("N")),
            Status = GetPropertyString(result, "Status", "Unknown"),
            Message = GetPropertyString(result, "Message", string.Empty),
            OutputJson = GetPropertyString(result, "OutputJson", string.Empty)
        };
    }

    private static void SetIncludeProperty(
        object loadRequest,
        ExpensaAddinLoaderRequest request)
    {
        if (HasProperty(loadRequest, "IncludeDisabled"))
        {
            SetProperty(loadRequest, "IncludeDisabled", request.IncludeInactive);
            return;
        }

        if (HasProperty(loadRequest, "IncludeClosed"))
        {
            SetProperty(loadRequest, "IncludeClosed", request.IncludeInactive);
        }
    }

    private static bool HasProperty(
        object instance,
        string propertyName)
    {
        return instance.GetType().GetProperty(propertyName) is not null;
    }

    private static void SetProperty(
        object instance,
        string propertyName,
        object value)
    {
        PropertyInfo? property =
            instance.GetType().GetProperty(propertyName);

        if (property is null)
        {
            return;
        }

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

        string? text =
            Convert.ToString(property.GetValue(instance));

        return string.IsNullOrWhiteSpace(text)
            ? defaultValue
            : text;
    }

    private static string GetDefaultExpensaDatabasePath()
    {
        return Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "CodexExpensa",
            "db",
            "codexexpensa.db");
    }

    private sealed class DependencyLoadContext : AssemblyLoadContext
    {
        private readonly AssemblyDependencyResolver resolver;

        public DependencyLoadContext(
            string mainAssemblyPath)
            : base(isCollectible: false)
        {
            resolver = new AssemblyDependencyResolver(mainAssemblyPath);
        }

        public Assembly LoadMainAssembly(
            string assemblyPath)
        {
            return LoadFromAssemblyPath(assemblyPath);
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
