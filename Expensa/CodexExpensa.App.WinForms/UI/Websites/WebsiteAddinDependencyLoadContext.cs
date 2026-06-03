using System.Reflection;
using System.Runtime.Loader;

namespace CodexExpensa.App.WinForms.UI.Websites;

internal sealed class WebsiteAddinDependencyLoadContext : AssemblyLoadContext
{
    private readonly AssemblyDependencyResolver resolver;

    public WebsiteAddinDependencyLoadContext(string mainAssemblyPath)
        : base(isCollectible: false)
    {
        resolver = new AssemblyDependencyResolver(mainAssemblyPath);
    }

    public Assembly LoadMainAssembly(string assemblyPath)
    {
        return LoadFromAssemblyPath(assemblyPath);
    }

    protected override Assembly? Load(AssemblyName assemblyName)
    {
        string? assemblyPath = resolver.ResolveAssemblyToPath(assemblyName);

        return assemblyPath is null
            ? null
            : LoadFromAssemblyPath(assemblyPath);
    }

    protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
    {
        string? libraryPath = resolver.ResolveUnmanagedDllToPath(unmanagedDllName);

        return libraryPath is null
            ? IntPtr.Zero
            : LoadUnmanagedDllFromPath(libraryPath);
    }
}
