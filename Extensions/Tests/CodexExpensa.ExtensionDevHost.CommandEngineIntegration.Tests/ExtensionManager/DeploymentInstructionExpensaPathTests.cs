using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.ExtensionManager;

public sealed class DeploymentInstructionExpensaPathTests
{
    [Fact]
    public void StarterDeployment_UsesExpensaCompiledExtensionsFolder()
    {
        string text = TestPathHelper.ReadExtensionsFile(
            "Deployment",
            "ExpensaDeployment.json");

        Assert.Contains(
            "Expensa\\\\Extensions",
            text,
            StringComparison.Ordinal);

        Assert.DoesNotContain(
            "Expensa\\\\CodexExpensa.App.WinForms\\\\bin\\\\Debug\\\\net8.0-windows\\\\Modules",
            text,
            StringComparison.Ordinal);
    }

    [Fact]
    public void StarterDeployment_UsesDebugBuildOutput()
    {
        string text = TestPathHelper.ReadExtensionsFile(
            "Deployment",
            "ExpensaDeployment.json");

        Assert.Contains(
            "\"configuration\": \"Debug\"",
            text,
            StringComparison.Ordinal);

        Assert.Contains(
            "\\\\bin\\\\Debug\\\\net8.0-windows",
            text,
            StringComparison.Ordinal);

        Assert.DoesNotContain(
            "\\\\bin\\\\Release\\\\net8.0-windows",
            text,
            StringComparison.Ordinal);
    }

    [Fact]
    public void GeneratedTemplate_MatchesActualExpensaDeploymentPath()
    {
        string text = TestPathHelper.ReadHostFile(
            "CodexExpensa.ExtensionDevHost",
            "Deployment",
            "DeploymentInstructionTemplate.cs");

        Assert.Contains(
            "Expensa\\\\Extensions",
            text,
            StringComparison.Ordinal);

        Assert.Contains(
            "Configuration = \"Debug\"",
            text,
            StringComparison.Ordinal);
    }
}
