using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.Payees;

public sealed class PayeesTreeLoadVerificationFormRuntimeDatabaseTests
{
    [Fact]
    public void PayeesTreeLoadVerificationForm_UsesActiveRuntimeDatabaseInsteadOfProdPath()
    {
        string text = TestPathHelper.ReadHostFile(
            "CodexExpensa.ExtensionDevHost",
            "CommandEngineIntegration",
            "Payees",
            "PayeesTreeLoadVerificationForm.cs");

        Assert.Contains("GetActiveRuntimePathFile(AddinId)", text);
        Assert.Contains("GetRuntimeDatabaseLocation(AddinId)", text);
        Assert.DoesNotContain("codexexpensa.db", text);
        Assert.DoesNotContain("LocalApplicationData", text);
    }
}
