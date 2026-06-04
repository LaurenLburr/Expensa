namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.CommonTree;

public abstract class AddinTreeProviderBase<TPayload> : IAddinTreeProvider<TPayload>
    where TPayload : IAddinTreePayload
{
    protected AddinTreeProviderBase(
        string addinName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(addinName);

        AddinName = addinName;
    }

    public string AddinName { get; }

    public abstract Task<IReadOnlyList<AddinTreeNode<TPayload>>> LoadAsync(
        AddinTreeLoadRequest request,
        CancellationToken cancellationToken = default);

    public virtual Task<AddinTreeOperationResult> FillAsync(
        TPayload payload,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(payload);

        return Task.FromResult(
            AddinTreeOperationResult.Success(
                $"{payload.DisplayText} loaded.",
                payload.NodeId));
    }

    public virtual Task<AddinTreeOperationResult> ModifyAsync(
        TPayload payload,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(payload);

        return Task.FromResult(
            AddinTreeOperationResult.Failure(
                $"{AddinName} does not support modify yet.",
                payload.NodeId));
    }

    public virtual Task<AddinTreeOperationResult> DeleteAsync(
        TPayload payload,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(payload);

        return Task.FromResult(
            AddinTreeOperationResult.Failure(
                $"{AddinName} does not support delete yet.",
                payload.NodeId));
    }

    protected static AddinTreeNode<TPayload> CreateNode(
        TPayload payload,
        IReadOnlyList<AddinTreeNode<TPayload>>? children = null)
    {
        ArgumentNullException.ThrowIfNull(payload);

        return new AddinTreeNode<TPayload>
        {
            Payload = payload,
            Children = children ?? []
        };
    }
}
