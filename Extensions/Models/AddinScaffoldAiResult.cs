namespace CodexExpensa.ExtensionDevHost.Models;

public sealed class AddinScaffoldAiResult
{
    public string SuggestedProjectName { get; init; } = string.Empty;
    public string SuggestedAssemblyName { get; init; } = string.Empty;
    public string SuggestedDescription { get; init; } = string.Empty;
    public string SuggestedClassName { get; init; } = string.Empty;
    public string SuggestedRootNodeName { get; init; } = string.Empty;
}
