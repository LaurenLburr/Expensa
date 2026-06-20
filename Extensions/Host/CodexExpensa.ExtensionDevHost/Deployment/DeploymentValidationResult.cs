namespace CodexExpensa.ExtensionDevHost.Deployment;

public sealed class DeploymentValidationResult
{
    public List<string> Errors { get; } = [];

    public List<string> Warnings { get; } = [];

    public bool IsValid => Errors.Count == 0;
}
