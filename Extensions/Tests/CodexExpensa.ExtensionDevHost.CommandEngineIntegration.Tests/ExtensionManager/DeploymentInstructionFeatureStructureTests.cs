using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.ExtensionManager;

public sealed class DeploymentInstructionFeatureStructureTests
{
    [Fact]
    public void MainForm_ExposesDeploymentMenu()
    {
        string text = TestPathHelper.ReadHostFile(
            "CodexExpensa.ExtensionDevHost",
            "MainForm.cs");

        Assert.Contains(
            "private void AddDeploymentMenu()",
            text,
            StringComparison.Ordinal);

        Assert.Contains(
            "ToolStripMenuItem deployMenu = new(\"Deploy\")",
            text,
            StringComparison.Ordinal);

        Assert.Contains(
            "Deploy All Enabled Add-ins to Expensa",
            text,
            StringComparison.Ordinal);

        Assert.Contains(
            "DeployRegisteredAddinToExpensa(registration)",
            text,
            StringComparison.Ordinal);

        Assert.Contains(
            "mainMenuStrip.Items.Add(deployMenu)",
            text,
            StringComparison.Ordinal);
    }

    [Fact]
    public void MainForm_DeploysBuiltOutputToExpensaExtensionsFolder()
    {
        string text = TestPathHelper.ReadHostFile(
            "CodexExpensa.ExtensionDevHost",
            "MainForm.cs");

        Assert.Contains(
            "BuildProject(projectFile);",
            text,
            StringComparison.Ordinal);

        Assert.Contains(
            "\"Expensa\",",
            text,
            StringComparison.Ordinal);

        Assert.Contains(
            "\"Extensions\",",
            text,
            StringComparison.Ordinal);
    }

    [Fact]
    public void DeploymentValidator_AllowsOnlyApprovedActions()
    {
        string text = TestPathHelper.ReadHostFile(
            "CodexExpensa.ExtensionDevHost",
            "Deployment",
            "DeploymentInstructionValidator.cs");

        Assert.Contains("\"EnsureDirectory\"", text);
        Assert.Contains("\"BuildProject\"", text);
        Assert.Contains("\"CopyFile\"", text);
        Assert.Contains("\"CopyFolder\"", text);
        Assert.Contains("\"VerifyFile\"", text);
        Assert.Contains("unsupported action", text);
    }

    [Fact]
    public void DeploymentPaths_AreRestrictedToRepository()
    {
        string text = TestPathHelper.ReadHostFile(
            "CodexExpensa.ExtensionDevHost",
            "Deployment",
            "DeploymentPathPolicy.cs");

        Assert.Contains("EnsureWithinRepository", text);
        Assert.Contains("outside the repository root", text);
    }

    [Fact]
    public void DeploymentExecutor_CreatesBackupsAndReports()
    {
        string text = TestPathHelper.ReadHostFile(
            "CodexExpensa.ExtensionDevHost",
            "Deployment",
            "DeploymentInstructionExecutor.cs");

        Assert.Contains("BackupFile(", text);
        Assert.Contains("Deployment_", text);
        Assert.Contains("BuildReport(result)", text);
        Assert.Contains("StopOnError", TestPathHelper.ReadHostFile(
            "CodexExpensa.ExtensionDevHost",
            "Deployment",
            "DeploymentInstructionDocument.cs"));
    }

    [Fact]
    public void StarterDeploymentFile_IsVersioned()
    {
        string text = TestPathHelper.ReadExtensionsFile(
            "Deployment",
            "ExpensaDeployment.json");

        Assert.Contains("\"formatVersion\": 1", text);
        Assert.Contains("\"BuildProject\"", text);
        Assert.Contains("\"CopyFolder\"", text);
        Assert.Contains("\"VerifyFile\"", text);
    }
}
