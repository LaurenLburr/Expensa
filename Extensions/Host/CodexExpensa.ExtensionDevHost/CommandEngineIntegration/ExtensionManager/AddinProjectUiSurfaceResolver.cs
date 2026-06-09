using System.Reflection;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.ExtensionManager;

public sealed class AddinProjectUiSurfaceResolver
{
    public bool TryCreateDatabaseForm(string projectName, string projectFolder, out Form? form)
    {
        return TryCreateForm(projectName, projectFolder, AddinProjectUiSurfaceKind.Database, out form);
    }

    public bool TryCreateTestForm(string projectName, string projectFolder, out Form? form)
    {
        return TryCreateForm(projectName, projectFolder, AddinProjectUiSurfaceKind.Test, out form);
    }

    private static bool TryCreateForm(string projectName, string projectFolder, AddinProjectUiSurfaceKind kind, out Form? form)
    {
        form = null;

        foreach (string typeName in GetCandidateTypeNames(projectName, kind))
        {
            Type? type = FindType(typeName);

            if (type is null || !typeof(Form).IsAssignableFrom(type))
            {
                continue;
            }

            object? instance = CreateFormInstance(type, projectName, projectFolder);

            if (instance is Form createdForm)
            {
                form = createdForm;
                return true;
            }
        }

        return false;
    }

    private static object? CreateFormInstance(Type type, string projectName, string projectFolder)
    {
        ConstructorInfo? twoArg = type.GetConstructor([typeof(string), typeof(string)]);

        if (twoArg is not null)
        {
            return twoArg.Invoke([projectName, projectFolder]);
        }

        ConstructorInfo? empty = type.GetConstructor(Type.EmptyTypes);
        return empty?.Invoke([]);
    }

    private static IEnumerable<string> GetCandidateTypeNames(string projectName, AddinProjectUiSurfaceKind kind)
    {
        string normalizedName = NormalizeAddinName(projectName);
        string pluralName = EnsurePlural(normalizedName);
        const string baseNamespace = "CodexExpensa.ExtensionDevHost.CommandEngineIntegration";

        if (kind == AddinProjectUiSurfaceKind.Database)
        {
            yield return $"{baseNamespace}.{pluralName}.{pluralName}DatabasePanelForm";
            yield return $"{baseNamespace}.{normalizedName}.{normalizedName}DatabasePanelForm";
            yield break;
        }

        yield return $"{baseNamespace}.{pluralName}.{pluralName}TreeLoadVerificationFormCommonTree";
        yield return $"{baseNamespace}.{pluralName}.{pluralName}TreeLoadVerificationForm";
        yield return $"{baseNamespace}.{normalizedName}.{normalizedName}TreeLoadVerificationFormCommonTree";
        yield return $"{baseNamespace}.{normalizedName}.{normalizedName}TreeLoadVerificationForm";
    }

    private static Type? FindType(string fullTypeName)
    {
        foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            Type? type = assembly.GetType(fullTypeName, throwOnError: false, ignoreCase: false);

            if (type is not null)
            {
                return type;
            }
        }

        return null;
    }

    private static string NormalizeAddinName(string projectName)
    {
        string value = projectName.Trim()
            .Replace(" ", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace("-", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace("_", string.Empty, StringComparison.OrdinalIgnoreCase);

        if (value.EndsWith("Addin", StringComparison.OrdinalIgnoreCase))
        {
            value = value[..^"Addin".Length];
        }

        if (value.EndsWith("AddIn", StringComparison.OrdinalIgnoreCase))
        {
            value = value[..^"AddIn".Length];
        }

        return value;
    }

    private static string EnsurePlural(string value)
    {
        if (value.EndsWith("s", StringComparison.OrdinalIgnoreCase))
        {
            return value;
        }

        if (value.EndsWith("y", StringComparison.OrdinalIgnoreCase) && value.Length > 1)
        {
            return value[..^1] + "ies";
        }

        return value + "s";
    }

    private enum AddinProjectUiSurfaceKind
    {
        Database,
        Test
    }
}
