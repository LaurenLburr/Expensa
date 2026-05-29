namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.ExtensionManager;

public static class ExtensionModuleFolderResolver
{
    public static string ResolveDefaultModulesFolder()
    {
        return Path.Combine(
            AppContext.BaseDirectory,
            "Modules");
    }
}
