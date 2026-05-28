namespace Codex.CommandEngine.Core;

public sealed class RuntimeCommandRegistrationProviderCollection
{
    private readonly List<IRuntimeCommandRegistrationProvider> _providers = [];

    public RuntimeCommandRegistrationProviderCollection AddProvider(
        IRuntimeCommandRegistrationProvider provider)
    {
        ArgumentNullException.ThrowIfNull(provider);

        _providers.Add(provider);
        return this;
    }

    public RuntimeCommandRegistrationProviderCollection AddProviders(
        IEnumerable<IRuntimeCommandRegistrationProvider> providers)
    {
        ArgumentNullException.ThrowIfNull(providers);

        foreach (IRuntimeCommandRegistrationProvider provider in providers)
        {
            AddProvider(provider);
        }

        return this;
    }

    public IReadOnlyList<RuntimeCommandRegistration> GetRegistrations()
    {
        List<RuntimeCommandRegistration> registrations = [];

        foreach (IRuntimeCommandRegistrationProvider provider in _providers)
        {
            IReadOnlyList<RuntimeCommandRegistration> providedRegistrations =
                provider.GetRegistrations();

            foreach (RuntimeCommandRegistration registration in providedRegistrations)
            {
                registrations.Add(registration);
            }
        }

        return registrations;
    }
}
