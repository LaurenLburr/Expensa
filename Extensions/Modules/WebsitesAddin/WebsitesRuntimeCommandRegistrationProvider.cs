using Codex.CommandEngine.Core;

namespace WebsitesAddin;

public sealed class WebsitesRuntimeCommandRegistrationProvider : IRuntimeCommandRegistrationProvider
{
    public IReadOnlyList<RuntimeCommandRegistration> GetRegistrations()
    {
        return
        [
            new RuntimeCommandRegistration
            {
                Handler = new WebsiteLoadRuntimeCommandHandler(),
                DisplayName = "Load Websites",
                Description = "Loads Websites into a UI-neutral tree structure.",
                Category = "Websites",
                Version = 1,
                IsEnabled = true
            }
        ];
    }
}
