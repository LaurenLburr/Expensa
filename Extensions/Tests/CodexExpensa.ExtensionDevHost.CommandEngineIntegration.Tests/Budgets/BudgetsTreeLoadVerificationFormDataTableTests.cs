using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.Budgets;

public sealed class BudgetsTreeLoadVerificationFormDataTableTests
{
    [Fact]
    public void HierarchyLoader_DoesNotUseDataTableLoadWhichInfersSourceConstraints()
    {
        string text = TestPathHelper.ReadHostFile(
            "CodexExpensa.ExtensionDevHost",
            "CommandEngineIntegration",
            "Budgets",
            "BudgetsTreeLoadVerificationForm.cs");

        Assert.DoesNotContain("table.Load(reader)", text, StringComparison.Ordinal);
        Assert.Contains("table.Columns.Add(reader.GetName(ordinal), typeof(object))", text, StringComparison.Ordinal);
        Assert.Contains("table.NewRow()", text, StringComparison.Ordinal);
        Assert.Contains("reader.GetValue(ordinal)", text, StringComparison.Ordinal);
    }

    [Fact]
    public void HierarchyLoader_AllowsRepeatedParentIdsFromCatalogResult()
    {
        string text = TestPathHelper.ReadHostFile(
            "CodexExpensa.ExtensionDevHost",
            "CommandEngineIntegration",
            "Budgets",
            "BudgetsTreeLoadVerificationForm.cs");

        Assert.Contains("table.Columns.Add(reader.GetName(ordinal), typeof(object))", text, StringComparison.Ordinal);
        Assert.DoesNotContain("PrimaryKey", text, StringComparison.Ordinal);
        Assert.DoesNotContain("Unique = true", text, StringComparison.Ordinal);
    }
}
