namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.CommonTree;

public sealed class AddinTreeNode<TPayload>
    where TPayload : IAddinTreePayload
{
    public required TPayload Payload { get; init; }

    public IReadOnlyList<AddinTreeNode<TPayload>> Children { get; init; } = [];
}
