using System.Reflection;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.ExtensionManager;

public sealed class ExtensionTreeContributionProviderDiscovery
{
    public IReadOnlyList<ExtensionTreeContributionDescriptor> DiscoverDescriptors(
        IReadOnlyList<Assembly> assemblies)
    {
        ArgumentNullException.ThrowIfNull(assemblies);

        List<ExtensionTreeContributionDescriptor> descriptors = [];

        foreach (Assembly assembly in assemblies)
        {
            foreach (Type type in GetLoadableTypes(assembly))
            {
                if (type.IsAbstract ||
                    type.IsInterface ||
                    !type.Name.EndsWith("TreeContributionProvider", StringComparison.Ordinal) ||
                    type.GetConstructor(Type.EmptyTypes) is null)
                {
                    continue;
                }

                object? instance =
                    Activator.CreateInstance(type);

                if (instance is null)
                {
                    continue;
                }

                ExtensionTreeContributionDescriptor? descriptor =
                    TryReadDescriptor(instance);

                if (descriptor is not null)
                {
                    descriptors.Add(descriptor);
                }
            }
        }

        return descriptors
            .OrderBy(static descriptor => descriptor.SortOrder)
            .ThenBy(static descriptor => descriptor.AddinId, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private static ExtensionTreeContributionDescriptor? TryReadDescriptor(
        object instance)
    {
        string addinId = GetStringProperty(instance, "AddinId");
        string displayName = GetStringProperty(instance, "DisplayName");
        string commandName = GetStringProperty(instance, "CommandName");
        string rootNodeName = GetStringProperty(instance, "RootNodeName");
        string rootDisplayText = GetStringProperty(instance, "RootDisplayText");
        int sortOrder = GetInt32Property(instance, "SortOrder");

        if (string.IsNullOrWhiteSpace(addinId) ||
            string.IsNullOrWhiteSpace(displayName) ||
            string.IsNullOrWhiteSpace(commandName) ||
            string.IsNullOrWhiteSpace(rootNodeName) ||
            string.IsNullOrWhiteSpace(rootDisplayText))
        {
            return null;
        }

        return new ExtensionTreeContributionDescriptor
        {
            AddinId = addinId,
            DisplayName = displayName,
            CommandName = commandName,
            RootNodeName = rootNodeName,
            RootDisplayText = rootDisplayText,
            SortOrder = sortOrder
        };
    }

    private static string GetStringProperty(object instance, string propertyName)
    {
        object? value =
            instance.GetType().GetProperty(propertyName)?.GetValue(instance);

        return Convert.ToString(value) ?? string.Empty;
    }

    private static int GetInt32Property(object instance, string propertyName)
    {
        object? value =
            instance.GetType().GetProperty(propertyName)?.GetValue(instance);

        return value is null ? 0 : Convert.ToInt32(value);
    }

    private static IReadOnlyList<Type> GetLoadableTypes(Assembly assembly)
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
