using System.Reflection;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.ExtensionManager;

public sealed class ExtensionTreeNodeLoaderProviderDiscovery
{
    public IReadOnlyList<IExtensionTreeNodeLoaderProvider> DiscoverProviders(
        IReadOnlyList<Assembly> assemblies)
    {
        ArgumentNullException.ThrowIfNull(assemblies);

        List<IExtensionTreeNodeLoaderProvider> providers = [];

        foreach (Assembly assembly in assemblies)
        {
            foreach (Type providerType in GetLoadableTypes(assembly))
            {
                if (!typeof(IExtensionTreeNodeLoaderProvider).IsAssignableFrom(providerType))
                {
                    continue;
                }

                if (providerType.IsAbstract || providerType.IsInterface)
                {
                    continue;
                }

                if (providerType.GetConstructor(Type.EmptyTypes) is null)
                {
                    continue;
                }

                object? instance =
                    Activator.CreateInstance(providerType);

                if (instance is IExtensionTreeNodeLoaderProvider provider)
                {
                    providers.Add(provider);
                }
            }
        }

        return providers;
    }

    private static IReadOnlyList<Type> GetLoadableTypes(
        Assembly assembly)
    {
        try
        {
            return assembly.GetTypes();
        }
        catch (ReflectionTypeLoadException exception)
        {
            return exception.Types
                .Where(static type => type is not null)
                .Cast<Type>()
                .ToList();
        }
    }
}
