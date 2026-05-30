namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.ExtensionManager;

public sealed class TreeContributionCommandAdapterRegistry
{
    private readonly IReadOnlyList<ITreeContributionCommandAdapter> _adapters;

    public TreeContributionCommandAdapterRegistry(
        IReadOnlyList<ITreeContributionCommandAdapter> adapters)
    {
        ArgumentNullException.ThrowIfNull(adapters);

        _adapters = adapters;
    }

    public ITreeContributionCommandAdapter GetRequiredAdapter(
        string commandName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(commandName);

        ITreeContributionCommandAdapter? adapter =
            _adapters.FirstOrDefault(adapter =>
                string.Equals(
                    adapter.CommandName,
                    commandName,
                    StringComparison.OrdinalIgnoreCase));

        if (adapter is null)
        {
            throw new InvalidOperationException(
                $"No host tree contribution adapter is registered for command '{commandName}'.");
        }

        return adapter;
    }
}
