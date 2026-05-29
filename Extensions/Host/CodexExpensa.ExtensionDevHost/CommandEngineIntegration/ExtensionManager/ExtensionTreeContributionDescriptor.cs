namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.ExtensionManager;

public sealed class ExtensionTreeContributionDescriptor
{
    public required string AddinId { get; init; }

    public required string DisplayName { get; init; }

    public int SortOrder { get; init; }

    public required string CommandName { get; init; }

    public required string RootNodeName { get; init; }

    public required string RootDisplayText { get; init; }
}
