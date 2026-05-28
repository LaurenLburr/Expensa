namespace Codex.CommandEngine.Core;

public sealed class StaticRuntimeCommandRegistrationProvider : IRuntimeCommandRegistrationProvider
{
    private readonly IReadOnlyList<RuntimeCommandRegistration> _registrations;

    public StaticRuntimeCommandRegistrationProvider(
        IEnumerable<RuntimeCommandRegistration> registrations)
    {
        ArgumentNullException.ThrowIfNull(registrations);

        _registrations = registrations.ToList();
    }

    public IReadOnlyList<RuntimeCommandRegistration> GetRegistrations()
    {
        return _registrations;
    }
}
