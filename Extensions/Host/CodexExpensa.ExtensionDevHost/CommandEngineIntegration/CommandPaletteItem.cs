namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration;

public sealed class CommandPaletteItem
{
    public required string CommandName { get; init; }

    public string DisplayName { get; init; } = string.Empty;

    public string Category { get; init; } = string.Empty;

    public bool IsEnabled { get; init; }

    public string Description { get; init; } = string.Empty;

    public string ParameterTemplateJson { get; init; } = "{}";

    public string Notes { get; init; } = string.Empty;

    public string SearchText =>
        $"{CommandName} {DisplayName} {Category} {Description} {Notes}";
}
