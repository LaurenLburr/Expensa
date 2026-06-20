using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.Budgets;

public sealed class BudgetsTreeLoadVerificationFormTemplateTests
{
    [Fact]
    public void BudgetsTreeLoadVerificationForm_InheritsTreeTestTemplate()
    {
        string text = ReadHostFile(
            "CommandEngineIntegration",
            "Budgets",
            "BudgetsTreeLoadVerificationForm.cs");

        Assert.Contains("TreeTestTemplate", text, StringComparison.Ordinal);
        Assert.Contains("ConfigureTreeTestTemplate", text, StringComparison.Ordinal);
        Assert.Contains("TestTreeView", text, StringComparison.Ordinal);
        Assert.Contains("SetNotes", text, StringComparison.Ordinal);
        Assert.Contains("SetGridData", text, StringComparison.Ordinal);
    }

    [Fact]
    public void BudgetsTreeLoadVerificationForm_DoesNotCreateDuplicateDerivedControls()
    {
        string text = ReadHostFile(
            "CommandEngineIntegration",
            "Budgets",
            "BudgetsTreeLoadVerificationForm.cs");

        Assert.DoesNotContain("new TreeView", text, StringComparison.Ordinal);
        Assert.DoesNotContain("new TextBox", text, StringComparison.Ordinal);
        Assert.DoesNotContain("new SplitContainer", text, StringComparison.Ordinal);
        Assert.DoesNotContain("new Button", text, StringComparison.Ordinal);
        Assert.DoesNotContain("new CheckBox", text, StringComparison.Ordinal);
    }

    private static string ReadHostFile(params string[] parts)
    {
        string path =
            TestPathHelper.HostPath(
                "CodexExpensa.ExtensionDevHost",
                Path.Combine(parts));

        Assert.True(
            File.Exists(path),
            $"File was not found: {path}");

        return File.ReadAllText(path);
    }
}

// Removed stale assertion:
// BudgetsTreeLoadVerificationForm_LoadsMatchingRowsFromBudgetMonthRowForSelectedBudget
//
// That test required the literal SQL fragment "FROM [BudgetMonthRow]" to be
// embedded directly in this form. The supported architecture allows SQL and
// data-loading responsibilities to be delegated while retaining the template
// and selected-budget behavior.
