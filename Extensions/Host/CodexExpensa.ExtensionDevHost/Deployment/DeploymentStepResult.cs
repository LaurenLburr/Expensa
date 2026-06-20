namespace CodexExpensa.ExtensionDevHost.Deployment;

public sealed class DeploymentStepResult
{
    public required int StepNumber { get; init; }

    public required string Action { get; init; }

    public required string Description { get; init; }

    public bool Succeeded { get; init; }

    public bool Skipped { get; init; }

    public string Message { get; init; } = string.Empty;
}
