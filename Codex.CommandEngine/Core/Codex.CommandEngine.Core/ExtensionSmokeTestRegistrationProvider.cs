namespace Codex.CommandEngine.Core;

public sealed class ExtensionSmokeTestRegistrationProvider : IRuntimeCommandRegistrationProvider
{
    public IReadOnlyList<RuntimeCommandRegistration> GetRegistrations()
    {
        return
        [
            new RuntimeCommandRegistration
            {
                Handler = new ExtensionSmokeTestCommandHandler(),
                DisplayName = "Extension Smoke Test",
                Description = "Verifies that ExtensionMgr can bootstrap and execute CommandEngine commands.",
                Category = "Extension Diagnostics",
                Version = 1,
                IsEnabled = true
            }
        ];
    }
}
