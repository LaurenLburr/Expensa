using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.ExtensionManager;

public sealed class DeploymentInstructionUpgradeTests
{
    [Fact]
    public void Panel_DetectsAndUpgradesLegacyStarterFile()
    {
        string text = TestPathHelper.ReadHostFile(
            "CodexExpensa.ExtensionDevHost",
            "Deployment",
            "DeploymentInstructionPanelForm.cs");

        Assert.Contains(
            "IsLegacyStarterFile(",
            text,
            StringComparison.Ordinal);

        Assert.Contains(
            "\"CodexExpensa.App.WinForms\\\\bin\\\\Debug\\\\net8.0-windows\\\\Modules\"",
            text,
            StringComparison.Ordinal);

        Assert.Contains(
            "\"Release\"",
            text,
            StringComparison.Ordinal);

        Assert.Contains(
            "WriteDefaultInstructionFile(",
            text,
            StringComparison.Ordinal);
    }

    [Fact]
    public void Panel_ProvidesRestoreDefaultInstructionsButton()
    {
        string text = TestPathHelper.ReadHostFile(
            "CodexExpensa.ExtensionDevHost",
            "Deployment",
            "DeploymentInstructionPanelForm.cs");

        Assert.Contains(
            "\"Restore Default Instructions\"",
            text,
            StringComparison.Ordinal);

        Assert.Contains(
            "RestoreDefaultInstructions()",
            text,
            StringComparison.Ordinal);

        Assert.Contains(
            ".bak",
            text,
            StringComparison.Ordinal);
    }

    [Fact]
    public void DefaultTemplate_UsesExpensaCompiledExtensionsFolder()
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
