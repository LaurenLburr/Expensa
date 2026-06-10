using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.ExtensionManager;

public sealed class AddinProjectUiSurfaceResolverStructureTests
{
    [Fact]
    public void Resolver_PrefersTemplateBasedTreeLoadVerificationFormBeforeCommonTreeFallback()
    {
        string text = TestPathHelper.ReadHostFile(
            "CodexExpensa.ExtensionDevHost",
            "CommandEngineIntegration",
            "ExtensionManager",
            "AddinProjectUiSurfaceResolver.cs");

        int templateFormIndex = text.IndexOf(
            "{pluralName}.{pluralName}TreeLoadVerificationForm\"",
            StringComparison.Ordinal);

        int commonTreeIndex = text.IndexOf(
            "{pluralName}.{pluralName}TreeLoadVerificationFormCommonTree\"",
            StringComparison.Ordinal);

        Assert.True(templateFormIndex >= 0, "The resolver should include the template-based tree load verification form convention.");
        Assert.True(commonTreeIndex >= 0, "The resolver should keep CommonTree as a fallback convention.");
        Assert.True(templateFormIndex < commonTreeIndex, "The resolver should prefer the template-based test form before the CommonTree fallback.");
    }
}
