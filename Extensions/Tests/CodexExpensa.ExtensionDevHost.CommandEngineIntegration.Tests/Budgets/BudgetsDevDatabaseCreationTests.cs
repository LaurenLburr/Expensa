using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.Budgets;

public sealed class BudgetsDevDatabaseCreationTests
{
    [Fact]
    public void BudgetsDatabasePanel_UsesSharedDevDatabaseCreationCoordinator()
    {
        string text = TestPathHelper.ReadHostFile(
            "CodexExpensa.ExtensionDevHost",
            "CommandEngineIntegration",
            "Budgets",
            "BudgetsDatabasePanelForm.cs");

        Assert.Contains(
            "AddinDevDatabaseUiCoordinator",
            text,
            StringComparison.Ordinal);

        Assert.Contains(
            "_devDatabaseUi.EnsureDevDatabaseExists(",
            text,
            StringComparison.Ordinal);

        Assert.Contains(
            "GetDevCurrentDatabasePath(AddinId)",
            text,
            StringComparison.Ordinal);
    }

    [Fact]
    public void SharedCoordinator_OwnsMissingDevDatabaseCreationWorkflow()
    {
        string text = TestPathHelper.ReadHostFile(
            "CodexExpensa.ExtensionDevHost",
            "CommandEngineIntegration",
            "ExtensionManager",
            "AddinDevDatabaseUiCoordinator.cs");

        Assert.Contains(
            "EnsureDevDatabaseExists",
            text,
            StringComparison.Ordinal);

        Assert.Contains(
            "MessageBox.Show",
            text,
            StringComparison.Ordinal);

        Assert.Contains(
            "Dev database",
            text,
            StringComparison.OrdinalIgnoreCase);

        Assert.Contains(
            "Create",
            text,
            StringComparison.OrdinalIgnoreCase);
    }
}
