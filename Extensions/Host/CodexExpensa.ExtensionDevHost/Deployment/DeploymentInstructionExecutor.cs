using System.Diagnostics;
using System.Text;

namespace CodexExpensa.ExtensionDevHost.Deployment;

public sealed class DeploymentInstructionExecutor
{
    private readonly DeploymentInstructionPaths _paths;
    private readonly DeploymentPathPolicy _pathPolicy;

    public DeploymentInstructionExecutor(
        DeploymentInstructionPaths paths,
        DeploymentPathPolicy pathPolicy)
    {
        _paths =
            paths
            ?? throw new ArgumentNullException(nameof(paths));

        _pathPolicy =
            pathPolicy
            ?? throw new ArgumentNullException(nameof(pathPolicy));
    }

    public DeploymentExecutionResult Execute(
        DeploymentInstructionDocument document,
        string instructionFilePath)
    {
        ArgumentNullException.ThrowIfNull(document);

        DateTime startedUtc =
            DateTime.UtcNow;

        string stamp =
            startedUtc.ToString("yyyyMMdd_HHmmss");

        Directory.CreateDirectory(_paths.ReportsFolder);

        string? backupFolder =
            document.CreateBackup
                ? Path.Combine(
                    _paths.BackupsFolder,
                    stamp)
                : null;

        if (backupFolder is not null)
        {
            Directory.CreateDirectory(backupFolder);
        }

        List<DeploymentStepResult> results = [];

        for (int index = 0;
             index < document.Steps.Count;
             index++)
        {
            DeploymentInstructionStep step =
                document.Steps[index];

            try
            {
                string message =
                    ExecuteStep(
                        document,
                        step,
                        backupFolder);

                results.Add(
                    new DeploymentStepResult
                    {
                        StepNumber = index + 1,
                        Action = step.Action,
                        Description = step.Name ?? step.Action,
                        Succeeded = true,
                        Message = message
                    });
            }
            catch (Exception exception)
            {
                bool skipped =
                    !step.Required;

                results.Add(
                    new DeploymentStepResult
                    {
                        StepNumber = index + 1,
                        Action = step.Action,
                        Description = step.Name ?? step.Action,
                        Succeeded = false,
                        Skipped = skipped,
                        Message = exception.ToString()
                    });

                if (step.Required && document.StopOnError)
                {
                    break;
                }
            }
        }

        DateTime completedUtc =
            DateTime.UtcNow;

        string reportPath =
            Path.Combine(
                _paths.ReportsFolder,
                $"Deployment_{stamp}.txt");

        DeploymentExecutionResult result =
            new()
            {
                DeploymentName = document.Name,
                StartedUtc = startedUtc,
                CompletedUtc = completedUtc,
                InstructionFilePath = instructionFilePath,
                ReportPath = reportPath,
                BackupFolder = backupFolder,
                Steps = results
            };

        File.WriteAllText(
            reportPath,
            BuildReport(result));

        return result;
    }

    private string ExecuteStep(
        DeploymentInstructionDocument document,
        DeploymentInstructionStep step,
        string? backupFolder)
    {
        return step.Action.ToUpperInvariant() switch
        {
            "ENSUREDIRECTORY" =>
                EnsureDirectory(
                    document,
                    step),

            "BUILDPROJECT" =>
                BuildProject(step),

            "COPYFILE" =>
                CopyFile(
                    document,
                    step,
                    backupFolder),

            "COPYFOLDER" =>
                CopyFolder(
                    document,
                    step,
                    backupFolder),

            "VERIFYFILE" =>
                VerifyFile(
                    document,
                    step),

            _ => throw new InvalidOperationException(
                $"Unsupported deployment action: {step.Action}")
        };
    }

    private string EnsureDirectory(
        DeploymentInstructionDocument document,
        DeploymentInstructionStep step)
    {
        string path =
            _pathPolicy.ResolveTargetPath(
                document.TargetRoot,
                step.Path!);

        Directory.CreateDirectory(path);

        return $"Ensured directory: {path}";
    }

    private string BuildProject(
        DeploymentInstructionStep step)
    {
        string project =
            _pathPolicy.ResolveRepositoryPath(
                step.Project!);

        if (!File.Exists(project))
        {
            throw new FileNotFoundException(
                "The project file was not found.",
                project);
        }

        string configuration =
            string.IsNullOrWhiteSpace(step.Configuration)
                ? "Release"
                : step.Configuration;

        ProcessStartInfo startInfo =
            new()
            {
                FileName = "dotnet",
                Arguments =
                    $"build \"{project}\" --configuration \"{configuration}\"",
                WorkingDirectory =
                    Path.GetDirectoryName(project)!,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

        using Process process =
            Process.Start(startInfo)
            ?? throw new InvalidOperationException(
                "Could not start dotnet build.");

        string output =
            process.StandardOutput.ReadToEnd();

        string error =
            process.StandardError.ReadToEnd();

        process.WaitForExit();

        if (process.ExitCode != 0)
        {
            throw new InvalidOperationException(
                $"dotnet build failed with exit code {process.ExitCode}.{Environment.NewLine}{output}{Environment.NewLine}{error}");
        }

        return $"Built project: {project}{Environment.NewLine}{output}";
    }

    private string CopyFile(
        DeploymentInstructionDocument document,
        DeploymentInstructionStep step,
        string? backupFolder)
    {
        string source =
            _pathPolicy.ResolveRepositoryPath(
                step.Source!);

        string destination =
            _pathPolicy.ResolveTargetPath(
                document.TargetRoot,
                step.Destination!);

        if (!File.Exists(source))
        {
            throw new FileNotFoundException(
                "Deployment source file was not found.",
                source);
        }

        string? destinationFolder =
            Path.GetDirectoryName(destination);

        if (!string.IsNullOrWhiteSpace(destinationFolder))
        {
            Directory.CreateDirectory(destinationFolder);
        }

        if (File.Exists(destination))
        {
            if (!step.Overwrite)
            {
                return $"Skipped existing file: {destination}";
            }

            BackupFile(
                destination,
                backupFolder);
        }

        File.Copy(
            source,
            destination,
            overwrite: step.Overwrite);

        return $"Copied file: {source} -> {destination}";
    }

    private string CopyFolder(
        DeploymentInstructionDocument document,
        DeploymentInstructionStep step,
        string? backupFolder)
    {
        string source =
            _pathPolicy.ResolveRepositoryPath(
                step.Source!);

        string destination =
            _pathPolicy.ResolveTargetPath(
                document.TargetRoot,
                step.Destination!);

        if (!Directory.Exists(source))
        {
            throw new DirectoryNotFoundException(
                $"Deployment source folder was not found:{Environment.NewLine}{source}");
        }

        if (step.CleanDestination &&
            Directory.Exists(destination))
        {
            BackupFolder(
                destination,
                backupFolder);

            Directory.Delete(
                destination,
                recursive: true);
        }

        Directory.CreateDirectory(destination);

        int copied =
            CopyDirectory(
                source,
                destination,
                step.Overwrite,
                backupFolder);

        return $"Copied {copied} files: {source} -> {destination}";
    }

    private string VerifyFile(
        DeploymentInstructionDocument document,
        DeploymentInstructionStep step)
    {
        string path =
            _pathPolicy.ResolveTargetPath(
                document.TargetRoot,
                step.Path!);

        if (!File.Exists(path))
        {
            throw new FileNotFoundException(
                "Required deployed file was not found.",
                path);
        }

        return $"Verified file: {path}";
    }

    private static int CopyDirectory(
        string source,
        string destination,
        bool overwrite,
        string? backupFolder)
    {
        int count = 0;

        foreach (string sourceFile in
                 Directory.GetFiles(
                     source,
                     "*",
                     SearchOption.AllDirectories))
        {
            string relative =
                Path.GetRelativePath(
                    source,
                    sourceFile);

            string destinationFile =
                Path.Combine(
                    destination,
                    relative);

            string? destinationFolder =
                Path.GetDirectoryName(
                    destinationFile);

            if (!string.IsNullOrWhiteSpace(destinationFolder))
            {
                Directory.CreateDirectory(destinationFolder);
            }

            if (File.Exists(destinationFile))
            {
                if (!overwrite)
                {
                    continue;
                }

                BackupFile(
                    destinationFile,
                    backupFolder);
            }

            File.Copy(
                sourceFile,
                destinationFile,
                overwrite);

            count++;
        }

        return count;
    }

    private static void BackupFolder(
        string folder,
        string? backupFolder)
    {
        if (backupFolder is null)
        {
            return;
        }

        foreach (string file in
                 Directory.GetFiles(
                     folder,
                     "*",
                     SearchOption.AllDirectories))
        {
            BackupFile(
                file,
                backupFolder);
        }
    }

    private static void BackupFile(
        string file,
        string? backupFolder)
    {
        if (backupFolder is null ||
            !File.Exists(file))
        {
            return;
        }

        string root =
            Path.GetPathRoot(file)
            ?? string.Empty;

        string relative =
            Path.GetRelativePath(
                root,
                file);

        string backupFile =
            Path.Combine(
                backupFolder,
                relative);

        string? folder =
            Path.GetDirectoryName(
                backupFile);

        if (!string.IsNullOrWhiteSpace(folder))
        {
            Directory.CreateDirectory(folder);
        }

        File.Copy(
            file,
            backupFile,
            overwrite: true);
    }

    private static string BuildReport(
        DeploymentExecutionResult result)
    {
        StringBuilder builder = new();

        builder.AppendLine(result.DeploymentName);
        builder.AppendLine(new string('=', result.DeploymentName.Length));
        builder.AppendLine();
        builder.AppendLine($"Succeeded: {result.Succeeded}");
        builder.AppendLine($"Started UTC: {result.StartedUtc:O}");
        builder.AppendLine($"Completed UTC: {result.CompletedUtc:O}");
        builder.AppendLine($"Instruction file: {result.InstructionFilePath}");
        builder.AppendLine($"Backup folder: {result.BackupFolder ?? "(none)"}");
        builder.AppendLine();

        foreach (DeploymentStepResult step in result.Steps)
        {
            string status =
                step.Succeeded
                    ? "SUCCEEDED"
                    : step.Skipped
                        ? "OPTIONAL FAILURE"
                        : "FAILED";

            builder.AppendLine(
                $"{step.StepNumber}. [{status}] {step.Description} ({step.Action})");
            builder.AppendLine(step.Message);
            builder.AppendLine();
        }

        return builder.ToString();
    }
}
