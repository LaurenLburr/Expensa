using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.Payees;

public sealed class PayeesSurfaceAndLoaderPhase2StructureTests
{
    [Fact]
    public void Payees_HasConventionDatabaseAndTestForms()
    {
        Assert.True(File.Exists(GetPath( "Host", "CodexExpensa.ExtensionDevHost", "CommandEngineIntegration", "Payees", "PayeesDatabasePanelForm.cs")));
        Assert.True(File.Exists(GetPath("Host", "CodexExpensa.ExtensionDevHost", "CommandEngineIntegration", "Payees", "PayeesTreeLoadVerificationFormCommonTree.cs")));
    }

    [Fact]
    public void MainForm_UsesGenericAddinSurfaceResolver()
    {
        string text = ReadFile("Host", "CodexExpensa.ExtensionDevHost", "MainForm.cs");

        Assert.Contains("AddinProjectUiSurfaceResolver", text);
        Assert.Contains("GenericAddinDatabasePanelForm", text);
        Assert.Contains("GenericAddinTreeLoadVerificationForm", text);
        Assert.DoesNotContain("IsWebsitesAddinProject", text);
        Assert.DoesNotContain("IsBudgetsAddinProject", text);
    }

    [Fact]
    public void RuntimeConvention_CanDerivePayeesRunnerNames()
    {
        string text = ReadFile( "Host", "CodexExpensa.ExtensionDevHost", "CommandEngineIntegration", "ExpensaLoader", "ExpensaAddinRuntimeConvention.cs");

        Assert.Contains("Singularize", text);
        Assert.Contains("Payees", text);
        Assert.Contains("Payee", text);
        Assert.Contains("LoadRuntimeSmokeRunner", text);
        Assert.Contains("LoadRequest", text);
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
