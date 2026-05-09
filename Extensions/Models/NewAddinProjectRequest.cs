namespace CodexExpensa.ExtensionDevHost.Models;

public sealed class NewAddinProjectRequest
{
    public string ProjectName { get; init; } = string.Empty;
    public string SolutionRootFolder { get; init; } = string.Empty;
    public string AssemblyName { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public bool RegisterProject { get; init; }
    public bool UseChatGpt { get; init; }
    public string ChatGptPrompt { get; init; } = string.Empty;
}
