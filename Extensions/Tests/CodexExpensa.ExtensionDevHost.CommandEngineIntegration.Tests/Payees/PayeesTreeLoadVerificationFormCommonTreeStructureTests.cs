using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.Payees;

public sealed class PayeesTreeLoadVerificationFormCommonTreeStructureTests
{
    [Fact]
    public void PayeesTestPage_UsesExpensaAddinLoaderService()
    {
        string text = ReadFile(
            "Host",
            "CodexExpensa.ExtensionDevHost",
            "CommandEngineIntegration",
            "Payees",
            "PayeesTreeLoadVerificationFormCommonTree.cs");

        Assert.Contains("ExpensaAddinTreeViewLoaderService", text);
        Assert.Contains("ExpensaAddinLoaderKind.Payees", text);
        Assert.Contains("LoadIntoTreeViewAsync", text);
    }

    [Fact]
    public void PayeesTestPage_UsesDesignerPattern()
    {
        Assert.True(File.Exists(GetPath(
            "Host",
            "CodexExpensa.ExtensionDevHost",
            "CommandEngineIntegration",
            "Payees",
            "PayeesTreeLoadVerificationFormCommonTree.Designer.cs")));
    }

    private static string ReadFile(params string[] parts)
    {
        return File.ReadAllText(GetPath(parts));
    }

    private static string GetPath(params string[] parts)
    {
        string repositoryRoot = FindRepositoryRoot();
        return Path.Combine([repositoryRoot, .. parts]);
    }

    private static string FindRepositoryRoot()
    {
        return TestPathHelper.ExtensionsRoot;
    }
}
