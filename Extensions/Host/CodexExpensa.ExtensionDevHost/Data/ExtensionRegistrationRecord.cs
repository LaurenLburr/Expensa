namespace CodexExpensa.ExtensionDevHost.Data;

public sealed class ExtensionRegistrationRecord
{
    public required string ProjectName { get; init; }

    public string? RelativeBinPath { get; init; }

    public required string AssemblyName { get; init; }

    public bool IsEnabled { get; init; }

    public int SortOrder { get; init; }
}
