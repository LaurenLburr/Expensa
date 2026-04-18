using System.IO;

namespace CodexExpensa.Navigation.Abstractions.Models;

public sealed class ExtensionProjectRegistration
{
    public required string ProjectName { get; init; }

    public string? RelativeBinPath { get; init; }

    public required string AssemblyName { get; init; }

    public bool IsEnabled { get; init; }

    public int SortOrder { get; init; }

    public string ResolveAssemblyPath(string repoRoot, string configuration, string targetFramework)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(repoRoot);
        ArgumentException.ThrowIfNullOrWhiteSpace(configuration);
        ArgumentException.ThrowIfNullOrWhiteSpace(targetFramework);

        if (!string.IsNullOrWhiteSpace(RelativeBinPath))
        {
            return Path.Combine(repoRoot, RelativeBinPath.Trim(), AssemblyName + ".dll");
        }

        return Path.Combine(
            repoRoot,
            "BuildOutput",
            ProjectName,
            configuration,
            targetFramework,
            AssemblyName + ".dll");
    }
}
