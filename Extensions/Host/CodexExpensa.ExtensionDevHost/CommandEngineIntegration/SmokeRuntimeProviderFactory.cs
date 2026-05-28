using Codex.CommandEngine.Core;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration;

public static class SmokeRuntimeProviderFactory
{
    public static IReadOnlyList<IRuntimeCommandRegistrationProvider> Create()
    {
        return
        [
            new ExtensionSmokeTestRegistrationProvider()
        ];
    }
}
