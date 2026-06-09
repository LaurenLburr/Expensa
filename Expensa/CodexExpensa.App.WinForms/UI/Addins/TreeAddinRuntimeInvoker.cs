using System.Reflection;
using System.Runtime.Loader;
using CodexExpensa.App.WinForms.Composition;

namespace CodexExpensa.App.WinForms.UI.Addins;

public sealed class TreeAddinRuntimeInvoker
{
    private readonly TreeAddinAssemblyLocator assemblyLocator;

    public TreeAddinRuntimeInvoker()
        : this(new TreeAddinAssemblyLocator())
    {
    }

    public TreeAddinRuntimeInvoker(TreeAddinAssemblyLocator assemblyLocator)
    {
        ArgumentNullException.ThrowIfNull(assemblyLocator);

        this.assemblyLocator = assemblyLocator;
    }

    public async Task<TreeAddinRuntimeResult> ExecuteAsync(
        TreeAddinDefinition definition,
        TreeAddinLoadOptions options,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(definition);
        ArgumentNullException.ThrowIfNull(options);

        string assemblyPath =
            assemblyLocator.FindAssemblyPath(definition);

        TreeAddinDependencyLoadContext loadContext =
            new(assemblyPath);

        Assembly assembly =
            loadContext.LoadMainAssembly(assemblyPath);

        Type smokeRunnerType =
            assembly.GetType(definition.SmokeRunnerTypeName, throwOnError: true)!;

        Type requestType =
            assembly.GetType(definition.RequestTypeName, throwOnError: true)!;

        object smokeRunner =
            Activator.CreateInstance(smokeRunnerType)
            ?? throw new InvalidOperationException($"Could not create {definition.SmokeRunnerTypeName}.");

        object loadRequest =
            Activator.CreateInstance(requestType)
            ?? throw new InvalidOperationException($"Could not create {definition.RequestTypeName}.");

        SetPropertyIfExists(loadRequest, "SearchText", options.SearchText);
        SetPropertyIfExists(loadRequest, "IncludeDisabled", options.IncludeInactive);
        SetPropertyIfExists(loadRequest, "IncludeClosed", options.IncludeInactive);
        SetPropertyIfExists(loadRequest, "MaximumRows", options.MaximumRows);
        SetPropertyIfExists(loadRequest, "DatabasePath", AppPaths.DatabaseFilePath());

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
            throw new InvalidOperationException($"{definition.AddinName} runner did not return a Task.");
        }

        await task.ConfigureAwait(true);

        PropertyInfo resultProperty =
            task.GetType().GetProperty("Result")
            ?? throw new InvalidOperationException($"{definition.AddinName} runner task did not expose Result.");

        object? result =
            resultProperty.GetValue(task);

        if (result is null)
        {
            throw new InvalidOperationException($"{definition.AddinName} runner returned null.");
        }

        return new TreeAddinRuntimeResult
        {
            Definition = definition,
            Status = GetPropertyString(result, "Status"),
            Message = GetPropertyString(result, "Message"),
            OutputJson = GetPropertyString(result, "OutputJson")
        };
    }

    private static void SetPropertyIfExists(object instance, string propertyName, object value)
    {
        PropertyInfo? property =
            instance.GetType().GetProperty(propertyName);

        property?.SetValue(instance, value);
    }

    private static string GetPropertyString(object instance, string propertyName)
    {
        PropertyInfo? property =
            instance.GetType().GetProperty(propertyName);

        if (property is null)
        {
            return string.Empty;
        }

        object? value =
            property.GetValue(instance);

        return Convert.ToString(value) ?? string.Empty;
    }

    private sealed class TreeAddinDependencyLoadContext : AssemblyLoadContext
    {
        private readonly AssemblyDependencyResolver resolver;

        public TreeAddinDependencyLoadContext(string mainAssemblyPath)
            : base(isCollectible: false)
        {
            resolver =
                new AssemblyDependencyResolver(mainAssemblyPath);
        }

        public Assembly LoadMainAssembly(string assemblyPath)
        {
            return LoadFromAssemblyPath(assemblyPath);
        }

        protected override Assembly? Load(AssemblyName assemblyName)
        {
            string? assemblyPath =
                resolver.ResolveAssemblyToPath(assemblyName);

            return assemblyPath is null
                ? null
                : LoadFromAssemblyPath(assemblyPath);
        }

        protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
        {
            string? libraryPath =
                resolver.ResolveUnmanagedDllToPath(unmanagedDllName);

            return libraryPath is null
                ? IntPtr.Zero
                : LoadUnmanagedDllFromPath(libraryPath);
        }
    }
}
