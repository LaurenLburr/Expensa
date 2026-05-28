namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration;

public sealed class ExtensionRuntimeDashboardSettings
{
    public string FolderPath { get; set; } = string.Empty;

    public bool Recursive { get; set; }

    public bool ShowDuplicateManifests { get; set; } = true;
}
