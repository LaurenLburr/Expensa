namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.ExtensionManager;

public sealed class ExtensionManagerAddinTestNode
{
    public required string AddinId { get; init; }
    public required string DisplayName { get; init; }
    public int SortOrder { get; init; }
    public string TestFormName { get; init; } = string.Empty;
}
