namespace CodexExpensa.ExtensionDevHost.Deployment;

public sealed class DeploymentInstructionStep
{
    public string Action { get; set; } = string.Empty;

    public string? Name { get; set; }

    public string? Project { get; set; }

    public string? Configuration { get; set; }

    public string? Source { get; set; }

    public string? Destination { get; set; }

    public string? Path { get; set; }

    public bool CleanDestination { get; set; }

    public bool Overwrite { get; set; } = true;

    public bool Required { get; set; } = true;
}
