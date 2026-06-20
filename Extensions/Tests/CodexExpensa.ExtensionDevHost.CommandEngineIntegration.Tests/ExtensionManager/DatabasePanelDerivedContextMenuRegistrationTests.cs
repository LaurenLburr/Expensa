using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.ExtensionManager;

public sealed class DatabasePanelDerivedContextMenuRegistrationTests
{
    [Fact]
    public void Template_DoesNotRegisterContextMenusInBaseConstructor()
    {
        string text = TestPathHelper.ReadHostFile(
            "CodexExpensa.ExtensionDevHost",
            "CommandEngineIntegration",
            "Templates",
            "DatabasePanelTemplate.cs");

        string constructorText =
            text[
                text.IndexOf("public DatabasePanelTemplate()", StringComparison.Ordinal)
                ..
                text.IndexOf("protected void ConfigureDatabaseContextMenus", StringComparison.Ordinal)];

        Assert.DoesNotContain(
            "DatabasePanelContextMenuController.Configure",
            constructorText,
            StringComparison.Ordinal);

        Assert.Contains(
            "protected void ConfigureDatabaseContextMenus(",
            text,
            StringComparison.Ordinal);
    }

    [Theory]
    [InlineData("Budgets", "BudgetsDatabasePanelForm.cs")]
    [InlineData("Payees", "PayeesDatabasePanelForm.cs")]
    [InlineData("Websites", "WebsitesDatabasePanelForm.cs")]
    public void DerivedDatabaseForm_RegistersItsOwnContextMenuActions(
        string folder,
        string fileName)
    {
        string text = TestPathHelper.ReadHostFile(
            "CodexExpensa.ExtensionDevHost",
            "CommandEngineIntegration",
            folder,
            fileName);

        Assert.Contains(
            "ConfigureDatabaseContextMenus(",
            text,
            StringComparison.Ordinal);

        Assert.Contains(
            "() => ReloadAddinDatabaseIntoMemory()",
            text,
            StringComparison.Ordinal);

        Assert.Contains(
            "() => CreateOrReplaceDevDatabaseFromAddinDatabase()",
            text,
            StringComparison.Ordinal);
    }
}
