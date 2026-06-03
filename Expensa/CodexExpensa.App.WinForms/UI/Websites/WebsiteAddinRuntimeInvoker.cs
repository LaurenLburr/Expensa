using System.Reflection;
using System.Runtime.Loader;
using CodexExpensa.App.WinForms.Composition;

namespace CodexExpensa.App.WinForms.UI.Websites;

public sealed class WebsiteAddinRuntimeInvoker
{
    private const string SmokeRunnerTypeName = "WebsitesAddin.WebsiteLoadRuntimeSmokeRunner";
    private const string RequestTypeName = "WebsitesAddin.WebsiteLoadRequest";

    public async Task<WebsiteRuntimeExecutionResult> ExecuteAsync(
        WebsiteTreeLoadOptions options,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(options);

        string assemblyPath =
            WebsiteAddinAssemblyLocator.FindWebsitesAddinAssemblyPath();

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

        SetProperty(loadRequest, "SearchText", options.SearchText);
        SetProperty(loadRequest, "IncludeDisabled", options.IncludeDisabled);
        SetProperty(loadRequest, "MaximumRows", options.MaximumRows);
        SetProperty(loadRequest, "DatabasePath", AppPaths.DatabaseFilePath());

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
            throw new InvalidOperationException("Website add-in runner did not return a Task.");
        }

        await task.ConfigureAwait(true);

        PropertyInfo resultProperty =
            task.GetType().GetProperty("Result")
            ?? throw new InvalidOperationException("Website add-in runner task did not expose Result.");

        object? result =
            resultProperty.GetValue(task);

        if (result is null)
        {
            throw new InvalidOperationException("Website add-in runner returned null.");
        }

        return new WebsiteRuntimeExecutionResult
        {
            Status = GetPropertyString(result, "Status"),
            Message = GetPropertyString(result, "Message"),
            OutputJson = GetPropertyString(result, "OutputJson")
        };
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
        string propertyName)
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

    private sealed class WebsiteAddinDependencyLoadContext
        : AssemblyLoadContext
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