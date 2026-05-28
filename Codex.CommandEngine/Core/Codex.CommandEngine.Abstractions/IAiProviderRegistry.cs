namespace Codex.CommandEngine.Abstractions;

public interface IAiProviderRegistry
{
    void Register(IAiProvider provider);

    bool TryResolve(string providerKey, out IAiProvider provider);

    IReadOnlyList<AiProviderDescriptor> ListProviders();
}
