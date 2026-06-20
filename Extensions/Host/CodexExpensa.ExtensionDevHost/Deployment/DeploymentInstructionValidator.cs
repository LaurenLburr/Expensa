namespace CodexExpensa.ExtensionDevHost.Deployment;

public sealed class DeploymentInstructionValidator
{
    private static readonly HashSet<string> AllowedActions =
        new(
            [
                "EnsureDirectory",
                "BuildProject",
                "CopyFile",
                "CopyFolder",
                "VerifyFile"
            ],
            StringComparer.OrdinalIgnoreCase);

    private readonly DeploymentPathPolicy _pathPolicy;

    public DeploymentInstructionValidator(
        DeploymentPathPolicy pathPolicy)
    {
        _pathPolicy =
            pathPolicy
            ?? throw new ArgumentNullException(nameof(pathPolicy));
    }

    public DeploymentValidationResult Validate(
        DeploymentInstructionDocument document)
    {
        ArgumentNullException.ThrowIfNull(document);

        DeploymentValidationResult result = new();

        if (document.FormatVersion != 1)
        {
            result.Errors.Add(
                $"Unsupported formatVersion {document.FormatVersion}. Expected 1.");
        }

        if (string.IsNullOrWhiteSpace(document.Name))
        {
            result.Errors.Add("The deployment name is required.");
        }

        if (string.IsNullOrWhiteSpace(document.TargetRoot))
        {
            result.Errors.Add("targetRoot is required.");
        }
        else
        {
            TryValidatePath(
                result,
                "targetRoot",
                () => _pathPolicy.ResolveRepositoryPath(document.TargetRoot));
        }

        if (document.Steps.Count == 0)
        {
            result.Errors.Add("At least one deployment step is required.");
        }

        for (int index = 0;
             index < document.Steps.Count;
             index++)
        {
            ValidateStep(
                document,
                document.Steps[index],
                index + 1,
                result);
        }

        return result;
    }

    private void ValidateStep(
        DeploymentInstructionDocument document,
        DeploymentInstructionStep step,
        int stepNumber,
        DeploymentValidationResult result)
    {
        string prefix =
            $"Step {stepNumber}";

        if (string.IsNullOrWhiteSpace(step.Action))
        {
            result.Errors.Add($"{prefix}: action is required.");
            return;
        }

        if (!AllowedActions.Contains(step.Action))
        {
            result.Errors.Add(
                $"{prefix}: unsupported action '{step.Action}'.");
            return;
        }

        switch (step.Action.ToUpperInvariant())
        {
            case "ENSUREDIRECTORY":
                RequireTargetPath(
                    document,
                    step.Path,
                    prefix,
                    result);
                break;

            case "BUILDPROJECT":
                RequireRepositoryPath(
                    step.Project,
                    "project",
                    prefix,
                    result);
                break;

            case "COPYFILE":
                RequireRepositoryPath(
                    step.Source,
                    "source",
                    prefix,
                    result);
                RequireTargetPath(
                    document,
                    step.Destination,
                    prefix,
                    result);
                break;

            case "COPYFOLDER":
                RequireRepositoryPath(
                    step.Source,
                    "source",
                    prefix,
                    result);
                RequireTargetPath(
                    document,
                    step.Destination,
                    prefix,
                    result);
                break;

            case "VERIFYFILE":
                RequireTargetPath(
                    document,
                    step.Path,
                    prefix,
                    result);
                break;
        }
    }

    private void RequireRepositoryPath(
        string? path,
        string propertyName,
        string prefix,
        DeploymentValidationResult result)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            result.Errors.Add(
                $"{prefix}: {propertyName} is required.");
            return;
        }

        TryValidatePath(
            result,
            $"{prefix}: {propertyName}",
            () => _pathPolicy.ResolveRepositoryPath(path));
    }

    private void RequireTargetPath(
        DeploymentInstructionDocument document,
        string? path,
        string prefix,
        DeploymentValidationResult result)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            result.Errors.Add(
                $"{prefix}: path/destination is required.");
            return;
        }

        TryValidatePath(
            result,
            $"{prefix}: target path",
            () => _pathPolicy.ResolveTargetPath(
                document.TargetRoot,
                path));
    }

    private static void TryValidatePath(
        DeploymentValidationResult result,
        string description,
        Func<string> resolve)
    {
        try
        {
            _ = resolve();
        }
        catch (Exception exception)
        {
            result.Errors.Add(
                $"{description}: {exception.Message}");
        }
    }
}
