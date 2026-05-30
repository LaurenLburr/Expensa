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
            }
        ];
    }
}
