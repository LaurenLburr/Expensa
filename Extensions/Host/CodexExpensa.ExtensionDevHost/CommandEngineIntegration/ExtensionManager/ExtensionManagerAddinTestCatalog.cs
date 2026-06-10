namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.ExtensionManager;

public sealed class ExtensionManagerAddinTestCatalog : IExtensionManagerAddinTestCatalog
{
    public IReadOnlyList<ExtensionManagerAddinTestNode> GetAddins()
    {
        return
        [
            new ExtensionManagerAddinTestNode
            {
                AddinId = "websites",
                DisplayName = "Websites Add-in",
                SortOrder = 100,
                DatabaseDisplayName = "Websites Database",
                TestFormName = "Websites Tree Test"
            },
            new ExtensionManagerAddinTestNode
            {
                AddinId = "payees",
                DisplayName = "Payees Add-in",
                SortOrder = 200,
                DatabaseDisplayName = "Payees Database",
                TestFormName = "Payees Tree Test"
            },
            new ExtensionManagerAddinTestNode
            {
                AddinId = "budgets",
                DisplayName = "Budgets Add-in",
                SortOrder = 300,
                DatabaseDisplayName = "Budgets Database",
                TestFormName = "Budgets Tree Test"
            }
        ];
    }
}
