namespace CodexExpensa.ExtensionDevHost.Services;

public sealed class ExistingAddinProjectImportResult
{
    public required string ProjectName { get; init; }
    public required string ProjectFilePath { get; init; }
    public required string ProjectFolder { get; init; }
    public required string AssemblyName { get; init; }
    public required string TargetFramework { get; init; }
    public required string RelativeBinPath { get; init; }
}
