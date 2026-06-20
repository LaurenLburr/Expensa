using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.ExtensionManager;

public sealed class DatabasePanelVirtualContextMenuDispatchTests
{
    [Fact]
    public void DatabasePanelTemplate_DoesNotRegisterDerivedActionsInBaseConstructor()
    {
        string text = TestPathHelper.ReadHostFile(
            "CodexExpensa.ExtensionDevHost",
            "CommandEngineIntegration",
            "Templates",
            "DatabasePanelTemplate.cs");

        int constructorStart =
            text.IndexOf(
                "public DatabasePanelTemplate()",
                StringComparison.Ordinal);

        int configurationStart =
            text.IndexOf(
                "protected void ConfigureDatabaseContextMenus",
                StringComparison.Ordinal);

        Assert.True(constructorStart >= 0);
        Assert.True(configurationStart > constructorStart);

        string constructorText =
            text[constructorStart..configurationStart];

        Assert.DoesNotContain(
            "ReloadAddinDatabaseIntoMemory",
            constructorText,
            StringComparison.Ordinal);

        Assert.DoesNotContain(
            "CreateOrReplaceDevDatabaseFromAddinDatabase",
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
    public void DerivedDatabasePanel_RegistersItsOwnActions(
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
