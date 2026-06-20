using System.Text.Json;

namespace CodexExpensa.App.WinForms.UI.Addins;

public sealed class AddinDeploymentMetadata
{
    public string AddIn { get; init; } = string.Empty;

    public string Version { get; init; } = string.Empty;

    public DateTime? DeployedUtc { get; init; }

    public string SourceAssembly { get; init; } = string.Empty;

    public string DeployedAssembly { get; init; } = string.Empty;

    public static AddinDeploymentMetadata LoadForAssembly(
        string assemblyPath)
    {
        if (string.IsNullOrWhiteSpace(assemblyPath))
        {
            return new AddinDeploymentMetadata();
        }

        string? folder =
            Path.GetDirectoryName(assemblyPath);

        if (string.IsNullOrWhiteSpace(folder))
        {
            return new AddinDeploymentMetadata();
        }

        string manifestPath =
            Path.Combine(
                folder,
                "deployment.json");

        if (!File.Exists(manifestPath))
        {
            return new AddinDeploymentMetadata();
        }

        try
        {
            return JsonSerializer.Deserialize<AddinDeploymentMetadata>(
                       File.ReadAllText(manifestPath),
                       new JsonSerializerOptions
                       {
                           PropertyNameCaseInsensitive = true
                       })
                   ?? new AddinDeploymentMetadata();
        }
        catch
        {
            // Deployment metadata is diagnostic only. A malformed or old
            // manifest must never prevent the add-in itself from loading.
            return new AddinDeploymentMetadata();
        }
    }
}
