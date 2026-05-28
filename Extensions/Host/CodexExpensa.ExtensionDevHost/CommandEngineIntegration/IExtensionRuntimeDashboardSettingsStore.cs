namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration;

public interface IExtensionRuntimeDashboardSettingsStore
{
    ExtensionRuntimeDashboardSettings Load();

    void Save(
        ExtensionRuntimeDashboardSettings settings);
}
