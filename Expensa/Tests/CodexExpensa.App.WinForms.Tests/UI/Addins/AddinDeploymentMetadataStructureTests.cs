using Xunit;

namespace CodexExpensa.App.WinForms.Tests.UI.Addins;

public sealed class AddinDeploymentMetadataStructureTests
{
    [Fact]
    public void RuntimeInvoker_ReadsDeploymentManifestAndAssemblyVersion()
    {
        string text = TestRepositoryPath.ReadExpensaFile(
            "UI",
            "Addins",
            "AddinScreenRuntimeInvoker.cs");

        Assert.Contains(
            "AddinDeploymentMetadata.LoadForAssembly(",
            text,
            StringComparison.Ordinal);

        Assert.Contains(
            "GetCustomAttribute<AssemblyInformationalVersionAttribute>()",
            text,
            StringComparison.Ordinal);

        Assert.Contains(
            "AssemblyVersion =",
            text,
            StringComparison.Ordinal);

        Assert.Contains(
            "DeployedUtc =",
            text,
            StringComparison.Ordinal);
    }

    [Fact]
    public void ScreenHeader_DisplaysVersionAndDeploymentTime()
    {
        string text = TestRepositoryPath.ReadExpensaFile(
            "UI",
            "Addins",
            "AddinScreenForm.cs");

        Assert.Contains(
            "Add-in version:",
            text,
            StringComparison.Ordinal);

        Assert.Contains(
            "Deployed:",
            text,
            StringComparison.Ordinal);

        Assert.Contains(
            "Provider DLL modified:",
            text,
            StringComparison.Ordinal);
    }

    [Fact]
    public void DeploymentMetadata_IsDiagnosticAndNonBlocking()
    {
        string text = TestRepositoryPath.ReadExpensaFile(
            "UI",
            "Addins",
            "AddinDeploymentMetadata.cs");

        Assert.Contains(
            "deployment.json",
            text,
            StringComparison.Ordinal);

        Assert.Contains(
            "catch",
            text,
            StringComparison.Ordinal);

        Assert.Contains(
            "must never prevent the add-in itself from loading",
            text,
            StringComparison.Ordinal);
    }
}
