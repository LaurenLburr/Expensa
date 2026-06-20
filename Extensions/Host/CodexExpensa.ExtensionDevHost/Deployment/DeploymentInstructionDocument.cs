using System.Text.Json.Serialization;

namespace CodexExpensa.ExtensionDevHost.Deployment;

public sealed class DeploymentInstructionDocument
{
    public int FormatVersion { get; set; } = 1;

    public string Name { get; set; } = "Deployment";

    public string TargetRoot { get; set; } = ".";

    public bool StopOnError { get; set; } = true;

    public bool CreateBackup { get; set; } = true;

    public List<DeploymentInstructionStep> Steps { get; set; } = [];
}
