namespace Codex.CommandEngine.Core;

public interface IRuntimeCommandRegistrationProvider
{
    IReadOnlyList<RuntimeCommandRegistration> GetRegistrations();
}
