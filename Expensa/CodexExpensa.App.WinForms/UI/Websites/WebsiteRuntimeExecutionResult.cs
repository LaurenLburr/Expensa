namespace CodexExpensa.App.WinForms.UI.Websites;

public sealed class WebsiteRuntimeExecutionResult
{
    public string Status { get; init; } = string.Empty;

    public string Message { get; init; } = string.Empty;

    public string OutputJson { get; init; } = string.Empty;

    public bool Succeeded =>
        string.Equals(Status, "Succeeded", StringComparison.OrdinalIgnoreCase);
}
