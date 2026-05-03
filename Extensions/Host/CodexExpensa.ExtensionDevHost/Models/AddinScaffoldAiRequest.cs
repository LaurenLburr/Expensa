namespace CodexExpensa.ExtensionDevHost.Models;

public sealed class AddinScaffoldAiRequest
{
    public required string ProjectName { get; init; }
    public required string AssemblyName { get; init; }
    public required string Description { get; init; }
    public required string Prompt { get; init; }
}
