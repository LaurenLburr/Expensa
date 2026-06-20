using System.Diagnostics;

namespace CodexExpensa.ExtensionDevHost.Deployment;

public sealed class DeploymentInstructionPanelForm : Form
{
    private readonly DeploymentInstructionPaths _paths = new();
    private readonly DeploymentInstructionFileService _fileService = new();

    private readonly TextBox _instructionPathTextBox = new();
    private readonly TextBox _outputTextBox = new();

    public DeploymentInstructionPanelForm()
    {
        Text = "Deployment Instructions";
        FormBorderStyle = FormBorderStyle.None;
        Dock = DockStyle.Fill;

        DeploymentPathPolicy pathPolicy =
            new(_paths.RepositoryRoot);

        DeploymentInstructionValidator validator =
            new(pathPolicy);

        DeploymentPreviewBuilder previewBuilder =
            new(pathPolicy);

        DeploymentInstructionExecutor executor =
            new(
                _paths,
                pathPolicy);

        TableLayoutPanel layout =
            new()
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                Padding = new Padding(12)
            };

        layout.RowStyles.Add(
            new RowStyle(
                SizeType.AutoSize));

        layout.RowStyles.Add(
            new RowStyle(
                SizeType.AutoSize));

        layout.RowStyles.Add(
            new RowStyle(
                SizeType.Percent,
                100));

        Label heading =
            new()
            {
                AutoSize = true,
                Font =
                    new Font(
                        Font,
                        FontStyle.Bold),
                Text = "Controlled Deployment Instructions"
            };

        FlowLayoutPanel actions =
            new()
            {
                AutoSize = true,
                Dock = DockStyle.Fill,
                WrapContents = true
            };

        _instructionPathTextBox.Width = 620;
        _instructionPathTextBox.Text =
            _paths.DefaultInstructionFile;

        Button browseButton =
            CreateButton(
                "Browse",
                (_, _) => Browse());

        Button openButton =
            CreateButton(
                "Open File",
                (_, _) => OpenInstructionFile());

        Button restoreButton =
            CreateButton(
                "Restore Default Instructions",
                (_, _) => RestoreDefaultInstructions());

        Button validateButton =
            CreateButton(
                "Validate",
                (_, _) => ValidateFile(
                    validator));

        Button previewButton =
            CreateButton(
                "Preview",
                (_, _) => Preview(
                    validator,
                    previewBuilder));

        Button runButton =
            CreateButton(
                "Run Deployment",
                (_, _) => RunDeployment(
                    validator,
                    executor));

        Button reportButton =
            CreateButton(
                "Open Last Report",
                (_, _) => OpenLastReport());

        Button outputFolderButton =
            CreateButton(
                "Open Deployment Folder",
                (_, _) => OpenFolder(
                    _paths.DeploymentFolder));

        actions.Controls.Add(_instructionPathTextBox);
        actions.Controls.Add(browseButton);
        actions.Controls.Add(openButton);
        actions.Controls.Add(restoreButton);
        actions.Controls.Add(validateButton);
        actions.Controls.Add(previewButton);
        actions.Controls.Add(runButton);
        actions.Controls.Add(reportButton);
        actions.Controls.Add(outputFolderButton);

        _outputTextBox.Dock = DockStyle.Fill;
        _outputTextBox.Multiline = true;
        _outputTextBox.ReadOnly = true;
        _outputTextBox.ScrollBars = ScrollBars.Both;
        _outputTextBox.WordWrap = false;
        _outputTextBox.Font =
            new Font(
                FontFamily.GenericMonospace,
                10);

        layout.Controls.Add(
            heading,
            0,
            0);

        layout.Controls.Add(
            actions,
            0,
            1);

        layout.Controls.Add(
            _outputTextBox,
            0,
            2);

        Controls.Add(layout);

        EnsureStarterFileExists();
        ShowInstructions();
    }

    private static Button CreateButton(
        string text,
        EventHandler click)
    {
        Button button =
            new()
            {
                AutoSize = true,
                Text = text
            };

        button.Click += click;

        return button;
    }

    private void EnsureStarterFileExists()
    {
        Directory.CreateDirectory(
            _paths.DeploymentFolder);

        if (!File.Exists(
                _paths.DefaultInstructionFile))
        {
            WriteDefaultInstructionFile(
                createBackup: false);

            return;
        }

        if (IsLegacyStarterFile(
                _paths.DefaultInstructionFile))
        {
            WriteDefaultInstructionFile(
                createBackup: true);
        }
    }

    private void RestoreDefaultInstructions()
    {
        DialogResult confirmation =
            MessageBox.Show(
                this,
                "Replace the current deployment instruction file with the"
                + Environment.NewLine
                + "current default Expensa deployment instructions?"
                + Environment.NewLine
                + Environment.NewLine
                + "The existing file will be backed up first.",
                "Restore Default Instructions",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2);

        if (confirmation != DialogResult.Yes)
        {
            return;
        }

        WriteDefaultInstructionFile(
            createBackup: true);

        _instructionPathTextBox.Text =
            _paths.DefaultInstructionFile;

        _outputTextBox.Text =
            "Default deployment instructions restored."
            + Environment.NewLine
            + Environment.NewLine
            + $"File: {_paths.DefaultInstructionFile}"
            + Environment.NewLine
            + Environment.NewLine
            + "Use Validate and Preview before running deployment.";
    }

    private void WriteDefaultInstructionFile(
        bool createBackup)
    {
        if (createBackup &&
            File.Exists(
                _paths.DefaultInstructionFile))
        {
            string backupPath =
                _paths.DefaultInstructionFile
                + "."
                + DateTime.Now.ToString("yyyyMMdd_HHmmss")
                + ".bak";

            File.Copy(
                _paths.DefaultInstructionFile,
                backupPath,
                overwrite: false);
        }

        DeploymentInstructionDocument starter =
            DeploymentInstructionTemplate.Create();

        _fileService.Save(
            _paths.DefaultInstructionFile,
            starter);
    }

    private bool IsLegacyStarterFile(string path)
    {
        try
        {
            DeploymentInstructionDocument document =
                _fileService.Load(path);

            bool hasLegacyDestination =
                document.Steps.Any(
                    static step =>
                        !string.IsNullOrWhiteSpace(step.Destination)
                        &&
                        step.Destination.Contains(
                            "CodexExpensa.App.WinForms\\bin\\Debug\\net8.0-windows\\Modules",
                            StringComparison.OrdinalIgnoreCase));

            bool usesRelease =
                document.Steps.Any(
                    static step =>
                        string.Equals(
                            step.Action,
                            "BuildProject",
                            StringComparison.OrdinalIgnoreCase)
                        &&
                        string.Equals(
                            step.Configuration,
                            "Release",
                            StringComparison.OrdinalIgnoreCase));

            return hasLegacyDestination && usesRelease;
        }
        catch
        {
            return false;
        }
    }

    private void ShowInstructions()
    {
        _outputTextBox.Text =
            "The deployment file is validated before preview or execution."
            + Environment.NewLine
            + Environment.NewLine
            + "Approved actions:"
            + Environment.NewLine
            + "  EnsureDirectory"
            + Environment.NewLine
            + "  BuildProject"
            + Environment.NewLine
            + "  CopyFile"
            + Environment.NewLine
            + "  CopyFolder"
            + Environment.NewLine
            + "  VerifyFile"
            + Environment.NewLine
            + Environment.NewLine
            + "All paths must remain under the CodexExpensa repository root."
            + Environment.NewLine
            + "Replaced files are backed up and every run creates a report."
            + Environment.NewLine
            + Environment.NewLine
            + "Current default destination:"
            + Environment.NewLine
            + "  Expensa\\Extensions";
    }

    private void Browse()
    {
        using OpenFileDialog dialog =
            new()
            {
                Filter =
                    "Deployment JSON (*.json)|*.json|All files (*.*)|*.*",
                FileName =
                    _instructionPathTextBox.Text,
                CheckFileExists = true
            };

        if (dialog.ShowDialog(this) ==
            DialogResult.OK)
        {
            _instructionPathTextBox.Text =
                dialog.FileName;
        }
    }

    private void OpenInstructionFile()
    {
        string path =
            GetInstructionPath();

        if (!File.Exists(path))
        {
            throw new FileNotFoundException(
                "The deployment instruction file was not found.",
                path);
        }

        Process.Start(
            new ProcessStartInfo
            {
                FileName = path,
                UseShellExecute = true
            });
    }

    private void ValidateFile(
        DeploymentInstructionValidator validator)
    {
        try
        {
            DeploymentInstructionDocument document =
                LoadDocument();

            DeploymentValidationResult result =
                validator.Validate(document);

            _outputTextBox.Text =
                FormatValidation(result);
        }
        catch (Exception exception)
        {
            _outputTextBox.Text =
                exception.ToString();
        }
    }

    private void Preview(
        DeploymentInstructionValidator validator,
        DeploymentPreviewBuilder previewBuilder)
    {
        try
        {
            string path =
                GetInstructionPath();

            DeploymentInstructionDocument document =
                _fileService.Load(path);

            DeploymentValidationResult validation =
                validator.Validate(document);

            if (!validation.IsValid)
            {
                _outputTextBox.Text =
                    FormatValidation(validation);
                return;
            }

            _outputTextBox.Text =
                previewBuilder.Build(
                    document,
                    path);
        }
        catch (Exception exception)
        {
            _outputTextBox.Text =
                exception.ToString();
        }
    }

    private void RunDeployment(
        DeploymentInstructionValidator validator,
        DeploymentInstructionExecutor executor)
    {
        try
        {
            string path =
                GetInstructionPath();

            DeploymentInstructionDocument document =
                _fileService.Load(path);

            DeploymentValidationResult validation =
                validator.Validate(document);

            if (!validation.IsValid)
            {
                _outputTextBox.Text =
                    FormatValidation(validation);
                return;
            }

            DialogResult confirmation =
                MessageBox.Show(
                    this,
                    "Run the validated deployment instructions?"
                    + Environment.NewLine
                    + Environment.NewLine
                    + "A timestamped report will be created."
                    + Environment.NewLine
                    + "Existing destination files will be backed up when enabled.",
                    "Run Deployment",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button2);

            if (confirmation !=
                DialogResult.Yes)
            {
                return;
            }

            DeploymentExecutionResult result =
                executor.Execute(
                    document,
                    path);

            _outputTextBox.Text =
                File.ReadAllText(
                    result.ReportPath);

            MessageBox.Show(
                this,
                result.Succeeded
                    ? "Deployment completed successfully."
                    : "Deployment completed with one or more failures.",
                "Deployment Instructions",
                MessageBoxButtons.OK,
                result.Succeeded
                    ? MessageBoxIcon.Information
                    : MessageBoxIcon.Warning);
        }
        catch (Exception exception)
        {
            _outputTextBox.Text =
                exception.ToString();

            MessageBox.Show(
                this,
                exception.Message,
                "Deployment Failed",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }

    private DeploymentInstructionDocument LoadDocument()
    {
        return _fileService.Load(
            GetInstructionPath());
    }

    private string GetInstructionPath()
    {
        string path =
            _instructionPathTextBox.Text.Trim();

        if (string.IsNullOrWhiteSpace(path))
        {
            throw new InvalidOperationException(
                "Select a deployment instruction file.");
        }

        return Path.GetFullPath(path);
    }

    private static string FormatValidation(
        DeploymentValidationResult result)
    {
        List<string> lines = [];

        lines.Add(
            result.IsValid
                ? "VALID"
                : "INVALID");

        if (result.Errors.Count > 0)
        {
            lines.Add(string.Empty);
            lines.Add("Errors:");
            lines.AddRange(
                result.Errors.Select(
                    static error => $"  - {error}"));
        }

        if (result.Warnings.Count > 0)
        {
            lines.Add(string.Empty);
            lines.Add("Warnings:");
            lines.AddRange(
                result.Warnings.Select(
                    static warning => $"  - {warning}"));
        }

        return string.Join(
            Environment.NewLine,
            lines);
    }

    private void OpenLastReport()
    {
        if (!Directory.Exists(
                _paths.ReportsFolder))
        {
            MessageBox.Show(
                this,
                "No deployment reports have been created.",
                "Deployment Reports",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            return;
        }

        string? report =
            Directory.GetFiles(
                    _paths.ReportsFolder,
                    "Deployment_*.txt",
                    SearchOption.TopDirectoryOnly)
                .OrderByDescending(
                    static path => path,
                    StringComparer.OrdinalIgnoreCase)
                .FirstOrDefault();

        if (report is null)
        {
            MessageBox.Show(
                this,
                "No deployment reports have been created.",
                "Deployment Reports",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            return;
        }

        Process.Start(
            new ProcessStartInfo
            {
                FileName = report,
                UseShellExecute = true
            });
    }

    private static void OpenFolder(string path)
    {
        Directory.CreateDirectory(path);

        Process.Start(
            new ProcessStartInfo
            {
                FileName = path,
                UseShellExecute = true
            });
    }
}
