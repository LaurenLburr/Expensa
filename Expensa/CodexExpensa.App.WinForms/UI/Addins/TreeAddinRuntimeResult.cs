namespace CodexExpensa.App.WinForms.UI.Addins;

public sealed class TreeAddinRuntimeResult
{
    public required TreeAddinDefinition Definition { get; init; }

    public required string Status { get; init; }

    public required string Message { get; init; }

    public required string OutputJson { get; init; }

    public bool Succeeded =>
        string.Equals(Status, "Succeeded", StringComparison.OrdinalIgnoreCase) ||
        string.Equals(Status, "Success", StringComparison.OrdinalIgnoreCase) ||
        string.Equals(Status, "Completed", StringComparison.OrdinalIgnoreCase);
}
