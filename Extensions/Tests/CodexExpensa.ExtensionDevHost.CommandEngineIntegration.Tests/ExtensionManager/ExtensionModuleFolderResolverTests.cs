using CodexExpensa.ExtensionDevHost.CommandEngineIntegration.ExtensionManager;
using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.ExtensionManager;

public sealed class ExtensionModuleFolderResolverTests
{
    [Fact]
    public void ResolveDefaultModulesFolder_ReturnsModulesUnderBaseDirectory()
    {
        string folder =
            ExtensionModuleFolderResolver.ResolveDefaultModulesFolder();

        Assert.EndsWith("Modules", folder, StringComparison.OrdinalIgnoreCase);
    }
}
