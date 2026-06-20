namespace CodexExpensa.ExtensionDevHost.Deployment;

public static class DeploymentInstructionTemplate
{
    public static DeploymentInstructionDocument Create()
    {
        DeploymentInstructionDocument document =
            new()
            {
                FormatVersion = 1,
                Name = "Deploy Expensa Extensions",
                TargetRoot = ".",
                StopOnError = true,
                CreateBackup = true
            };

        AddProject(document, "BudgetsAddin");
        AddProject(document, "PayeesAddin");
        AddProject(document, "WebsitesAddin");

        return document;
    }

    private static void AddProject(
        DeploymentInstructionDocument document,
        string projectName)
    {
        document.Steps.Add(
            new DeploymentInstructionStep
            {
                Action = "BuildProject",
                Name = $"Build {projectName}",
                Project =
                    $"Extensions\\Modules\\{projectName}\\{projectName}.csproj",
                Configuration = "Debug"
            });

        document.Steps.Add(
            new DeploymentInstructionStep
            {
                Action = "CopyFolder",
                Name = $"Deploy {projectName}",
                Source =
                    $"Extensions\\Modules\\{projectName}\\bin\\Debug\\net8.0-windows",
                Destination =
                    $"Expensa\\Extensions\\{projectName}",
                CleanDestination = true,
                Overwrite = true
            });

        document.Steps.Add(
            new DeploymentInstructionStep
            {
                Action = "VerifyFile",
                Name = $"Verify {projectName}",
                Path =
                    $"Expensa\\Extensions\\{projectName}\\{projectName}.dll"
            });
    }
}
