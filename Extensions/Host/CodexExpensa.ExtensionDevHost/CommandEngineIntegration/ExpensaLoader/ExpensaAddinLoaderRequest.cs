namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.ExpensaLoader;

public sealed class ExpensaAddinLoaderRequest
{
    public required ExpensaAddinLoaderKind Kind { get; init; }

    public string SearchText { get; init; } = string.Empty;

    public bool IncludeInactive { get; init; }

    public int MaximumRows { get; init; } = 500;

    public string DatabasePath { get; init; } = string.Empty;
}
