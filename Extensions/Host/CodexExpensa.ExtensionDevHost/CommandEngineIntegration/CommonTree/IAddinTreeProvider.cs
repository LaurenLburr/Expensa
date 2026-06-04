namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.CommonTree;

public interface IAddinTreeProvider<TPayload>
    where TPayload : IAddinTreePayload
{
    string AddinName { get; }

    Task<IReadOnlyList<AddinTreeNode<TPayload>>> LoadAsync(
        AddinTreeLoadRequest request,
        CancellationToken cancellationToken = default);

    Task<AddinTreeOperationResult> FillAsync(
        TPayload payload,
        CancellationToken cancellationToken = default);

    Task<AddinTreeOperationResult> ModifyAsync(
        TPayload payload,
        CancellationToken cancellationToken = default);

    Task<AddinTreeOperationResult> DeleteAsync(
        TPayload payload,
        CancellationToken cancellationToken = default);
}
