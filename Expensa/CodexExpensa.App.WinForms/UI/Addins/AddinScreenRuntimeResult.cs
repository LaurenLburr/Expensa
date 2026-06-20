namespace CodexExpensa.App.WinForms.UI.Addins;

public sealed class AddinScreenRuntimeResult
{
    public required TreeAddinDefinition Definition { get; init; }

    public required string Status { get; init; }

    public required string Message { get; init; }

    public required AddinScreenModel Screen { get; init; }

    public required string AssemblyPath { get; init; }

    public DateTime? AssemblyLastWriteTimeUtc { get; init; }

    public string AssemblyVersion { get; init; } = string.Empty;

    public DateTime? DeployedUtc { get; init; }

    public bool Succeeded =>
        string.Equals(Status, "Succeeded", StringComparison.OrdinalIgnoreCase) ||
        string.Equals(Status, "Success", StringComparison.OrdinalIgnoreCase) ||
        string.Equals(Status, "Completed", StringComparison.OrdinalIgnoreCase);
}
