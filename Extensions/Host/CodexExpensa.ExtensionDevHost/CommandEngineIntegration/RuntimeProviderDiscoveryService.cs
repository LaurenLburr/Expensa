using Codex.CommandEngine.Core;
using System.Reflection;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration;

public sealed class RuntimeProviderDiscoveryService : IRuntimeProviderDiscoveryService
{
    public RuntimeProviderDiscoveryResult DiscoverFromLoadedAssemblies()
    {
        List<IRuntimeCommandRegistrationProvider> providers = [];
        List<RuntimeProviderDiscoveryIssue> issues = [];

        foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies().OrderBy(static x => x.FullName))
        {
            RuntimeProviderDiscoveryResult result =
                DiscoverFromAssembly(assembly);

            providers.AddRange(result.Providers);
            issues.AddRange(result.Issues);
        }

        return new RuntimeProviderDiscoveryResult
        {
            Providers = providers,
            Issues = issues
        };
    }

    public RuntimeProviderDiscoveryResult DiscoverFromAssembly(
        Assembly assembly)
    {
        ArgumentNullException.ThrowIfNull(assembly);

        List<IRuntimeCommandRegistrationProvider> providers = [];
        List<RuntimeProviderDiscoveryIssue> issues = [];

        foreach (Type type in GetCandidateTypes(assembly, issues))
        {
            if (!typeof(IRuntimeCommandRegistrationProvider).IsAssignableFrom(type))
            {
                continue;
            }

            if (type.IsAbstract || type.IsInterface)
            {
                continue;
            }

            if (type.GetConstructor(Type.EmptyTypes) is null)
            {
                issues.Add(new RuntimeProviderDiscoveryIssue
                {
                    Source = type.FullName ?? type.Name,
                    Message = "Runtime provider type does not have a public parameterless constructor."
                });

                continue;
            }

            try
            {
                object? instance =
                    Activator.CreateInstance(type);

                if (instance is IRuntimeCommandRegistrationProvider provider)
                {
                    providers.Add(provider);
                }
            }
            catch (Exception exception)
            {
                issues.Add(new RuntimeProviderDiscoveryIssue
                {
                    Source = type.FullName ?? type.Name,
                    Message = exception.Message,
                    Exception = exception
                });
            }
        }

        return new RuntimeProviderDiscoveryResult
        {
            Providers = providers,
            Issues = issues
        };
    }

    public RuntimeProviderDiscoveryResult DiscoverFromAssemblyFile(
        string assemblyPath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(assemblyPath);

        if (!File.Exists(assemblyPath))
        {
            return new RuntimeProviderDiscoveryResult
            {
                Issues =
                [
                    new RuntimeProviderDiscoveryIssue
                    {
                        Source = assemblyPath,
                        Message = "Assembly file was not found."
                    }
                ]
            };
        }

        try
        {
            Assembly assembly =
                Assembly.LoadFrom(assemblyPath);

            return DiscoverFromAssembly(assembly);
        }
        catch (Exception exception)
        {
            return new RuntimeProviderDiscoveryResult
            {
                Issues =
                [
                    new RuntimeProviderDiscoveryIssue
                    {
                        Source = assemblyPath,
                        Message = exception.Message,
                        Exception = exception
                    }
                ]
            };
        }
    }

    public RuntimeProviderDiscoveryResult DiscoverFromFolder(
        string folderPath,
        bool recursive = false)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(folderPath);

        if (!Directory.Exists(folderPath))
        {
            return new RuntimeProviderDiscoveryResult
            {
                Issues =
                [
                    new RuntimeProviderDiscoveryIssue
                    {
                        Source = folderPath,
                        Message = "Folder was not found."
                    }
                ]
            };
        }

        SearchOption searchOption =
            recursive
                ? SearchOption.AllDirectories
                : SearchOption.TopDirectoryOnly;

        List<IRuntimeCommandRegistrationProvider> providers = [];
        List<RuntimeProviderDiscoveryIssue> issues = [];

        foreach (string assemblyPath in Directory.GetFiles(folderPath, "*.dll", searchOption).OrderBy(static x => x))
        {
            RuntimeProviderDiscoveryResult result =
                DiscoverFromAssemblyFile(assemblyPath);

            providers.AddRange(result.Providers);
            issues.AddRange(result.Issues);
        }

        return new RuntimeProviderDiscoveryResult
        {
            Providers = providers,
            Issues = issues
        };
    }

    private static IReadOnlyList<Type> GetCandidateTypes(
        Assembly assembly,
        List<RuntimeProviderDiscoveryIssue> issues)
    {
        try
        {
            return assembly.GetTypes();
        }
        catch (ReflectionTypeLoadException exception)
        {
            foreach (Exception? loaderException in exception.LoaderExceptions)
            {
                if (loaderException is null)
                {
                    continue;
                }

                issues.Add(new RuntimeProviderDiscoveryIssue
                {
                    Source = assembly.FullName ?? assembly.GetName().Name ?? "Unknown assembly",
                    Message = loaderException.Message,
                    Exception = loaderException
                });
            }

            return exception.Types
                .Where(static type => type is not null)
                .Cast<Type>()
                .ToList();
        }
        catch (Exception exception)
        {
            issues.Add(new RuntimeProviderDiscoveryIssue
            {
                Source = assembly.FullName ?? assembly.GetName().Name ?? "Unknown assembly",
                Message = exception.Message,
                Exception = exception
            });

            return [];
        }
    }
}
