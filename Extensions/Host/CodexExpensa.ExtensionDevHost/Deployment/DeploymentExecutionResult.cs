namespace CodexExpensa.ExtensionDevHost.Deployment;

public sealed class DeploymentExecutionResult
{
    public required string DeploymentName { get; init; }

    public required DateTime StartedUtc { get; init; }

    public required DateTime CompletedUtc { get; init; }

    public required string InstructionFilePath { get; init; }

    public required string ReportPath { get; init; }

    public string? BackupFolder { get; init; }

    public List<DeploymentStepResult> Steps { get; init; } = [];

    public bool Succeeded =>
        Steps.All(static step => step.Succeeded || step.Skipped);
}
