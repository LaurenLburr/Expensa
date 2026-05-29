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
                TestFormName = "Extension Tree Load Test"
            }
        ];
    }
}
