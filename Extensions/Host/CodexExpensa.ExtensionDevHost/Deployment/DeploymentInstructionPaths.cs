namespace CodexExpensa.ExtensionDevHost.Deployment;

public sealed class DeploymentInstructionPaths
{
    public string RepositoryRoot { get; }

    public string DeploymentFolder { get; }

    public string DefaultInstructionFile { get; }

    public string ReportsFolder { get; }

    public string BackupsFolder { get; }

    public DeploymentInstructionPaths()
    {
        RepositoryRoot = FindRepositoryRoot();
        DeploymentFolder =
            Path.Combine(
                RepositoryRoot,
                "Extensions",
                "Deployment");

        DefaultInstructionFile =
            Path.Combine(
                DeploymentFolder,
                "ExpensaDeployment.json");

        ReportsFolder =
            Path.Combine(
                DeploymentFolder,
                "Reports");

        BackupsFolder =
            Path.Combine(
                DeploymentFolder,
                "Backups");
    }

    private static string FindRepositoryRoot()
    {
        DirectoryInfo? directory =
            new(AppContext.BaseDirectory);

        while (directory is not null)
        {
            string extensions =
                Path.Combine(
                    directory.FullName,
                    "Extensions");

            if (Directory.Exists(
                    Path.Combine(extensions, "Host"))
                &&
                Directory.Exists(
                    Path.Combine(extensions, "Modules")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException(
            "Could not find the CodexExpensa repository root.");
    }
}
