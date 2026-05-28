using Codex.CommandEngine.Core;

namespace WebsitesAddin;

public sealed class WebsitesAddinCommandProvider : IRuntimeCommandRegistrationProvider
{
    public IReadOnlyList<RuntimeCommandRegistration> GetRegistrations()
    {
        return
        [
            new RuntimeCommandRegistration
            {
                Handler = new WebsitesAddinSmokeCommandHandler(),
                DisplayName = "Websites Add-in Smoke Test",
                Description = "Verifies that the Websites add-in can register and execute through CommandEngine.",
                Category = "Websites",
                Version = 1,
                IsEnabled = true
            }
        ];
    }
}
