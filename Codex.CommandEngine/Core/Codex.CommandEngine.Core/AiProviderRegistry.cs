using Codex.CommandEngine.Abstractions;

namespace Codex.CommandEngine.Core;

public sealed class AiProviderRegistry : IAiProviderRegistry
{
    private readonly Dictionary<string, IAiProvider> _providers = new(StringComparer.OrdinalIgnoreCase);

    public void Register(IAiProvider provider)
    {
        ArgumentNullException.ThrowIfNull(provider);
        ArgumentException.ThrowIfNullOrWhiteSpace(provider.Descriptor.ProviderKey);

        if (_providers.ContainsKey(provider.Descriptor.ProviderKey))
        {
            throw new InvalidOperationException($"AI provider '{provider.Descriptor.ProviderKey}' is already registered.");
        }

        _providers.Add(provider.Descriptor.ProviderKey, provider);
    }

    public bool TryResolve(string providerKey, out IAiProvider provider)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(providerKey);

        return _providers.TryGetValue(providerKey, out provider!);
    }

    public IReadOnlyList<AiProviderDescriptor> ListProviders()
    {
        return _providers.Values
            .Select(provider => provider.Descriptor)
            .OrderBy(descriptor => descriptor.ProviderKind, StringComparer.OrdinalIgnoreCase)
            .ThenBy(descriptor => descriptor.DisplayName, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }
}
