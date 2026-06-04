namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.ExpensaLoader;

public sealed class ExpensaAddinLoaderRequestTemplate
{
    public string DatabasePath { get; init; } = string.Empty;

    public string SearchText { get; init; } = string.Empty;

    public bool IncludeInactive { get; init; }

    public int MaximumRows { get; init; } = 500;
}
