using System.Text;

namespace CodexExpensa.ExtensionDevHost.Deployment;

public sealed class DeploymentPreviewBuilder
{
    private readonly DeploymentPathPolicy _pathPolicy;

    public DeploymentPreviewBuilder(
        DeploymentPathPolicy pathPolicy)
    {
        _pathPolicy =
            pathPolicy
            ?? throw new ArgumentNullException(nameof(pathPolicy));
    }

    public string Build(
        DeploymentInstructionDocument document,
        string instructionFilePath)
    {
        ArgumentNullException.ThrowIfNull(document);

        StringBuilder builder = new();

        builder.AppendLine(document.Name);
        builder.AppendLine(
            new string('=', document.Name.Length));
        builder.AppendLine();
        builder.AppendLine($"Instruction file: {instructionFilePath}");
        builder.AppendLine($"Target root: {_pathPolicy.ResolveRepositoryPath(document.TargetRoot)}");
        builder.AppendLine($"Stop on error: {document.StopOnError}");
        builder.AppendLine($"Create backup: {document.CreateBackup}");
        builder.AppendLine();

        for (int index = 0;
             index < document.Steps.Count;
             index++)
        {
            DeploymentInstructionStep step =
                document.Steps[index];

            builder.AppendLine(
                $"{index + 1}. {Describe(document, step)}");
        }

        return builder.ToString();
    }

    private string Describe(
        DeploymentInstructionDocument document,
        DeploymentInstructionStep step)
    {
        string name =
            string.IsNullOrWhiteSpace(step.Name)
                ? step.Action
                : step.Name;

        return step.Action.ToUpperInvariant() switch
        {
            "ENSUREDIRECTORY" =>
                $"{name}: ensure directory {_pathPolicy.ResolveTargetPath(document.TargetRoot, step.Path!)}",

            "BUILDPROJECT" =>
                $"{name}: dotnet build {_pathPolicy.ResolveRepositoryPath(step.Project!)} --configuration {step.Configuration ?? "Release"}",

            "COPYFILE" =>
                $"{name}: copy {_pathPolicy.ResolveRepositoryPath(step.Source!)} -> {_pathPolicy.ResolveTargetPath(document.TargetRoot, step.Destination!)}",

            "COPYFOLDER" =>
                $"{name}: copy folder {_pathPolicy.ResolveRepositoryPath(step.Source!)} -> {_pathPolicy.ResolveTargetPath(document.TargetRoot, step.Destination!)}; clean={step.CleanDestination}",

            "VERIFYFILE" =>
                $"{name}: verify {_pathPolicy.ResolveTargetPath(document.TargetRoot, step.Path!)}",

            _ => $"{name}: unsupported"
        };
    }
}
