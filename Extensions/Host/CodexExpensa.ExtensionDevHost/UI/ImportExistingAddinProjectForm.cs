using CodexExpensa.ExtensionDevHost.Services;

namespace CodexExpensa.ExtensionDevHost.UI;

public sealed partial class ImportExistingAddinProjectForm : Form
{
    private readonly ExistingAddinProjectImportService importService = new();

    public ImportExistingAddinProjectForm()
    {
        InitializeComponent();
    }

    public ExistingAddinProjectImportResult? ImportResult { get; private set; }

    private void browseButton_Click(object? sender, EventArgs e)
    {
        using OpenFileDialog dialog = new()
        {
            Title = "Select existing add-in project",
            Filter = "C# project (*.csproj)|*.csproj|All files (*.*)|*.*",
            CheckFileExists = true,
            Multiselect = false
        };
        if (dialog.ShowDialog(this) != DialogResult.OK) return;
        projectFileTextBox.Text = dialog.FileName;
        previewTextBox.Text = $"Project file:{Environment.NewLine}{dialog.FileName}{Environment.NewLine}{Environment.NewLine}Click Import to register this add-in.";
    }

    private void importButton_Click(object? sender, EventArgs e)
    {
        try
        {
            ExistingAddinProjectImportResult result = importService.ImportProject(projectFileTextBox.Text.Trim());
            ImportResult = result;
            resultTextBox.Text = $"Imported: {result.ProjectName}{Environment.NewLine}Target framework: {result.TargetFramework}{Environment.NewLine}Assembly: {result.AssemblyName}{Environment.NewLine}Relative bin path: {result.RelativeBinPath}{Environment.NewLine}Project file: {result.ProjectFilePath}";
            DialogResult = DialogResult.OK;
            Close();
        }
        catch (Exception exception)
        {
            resultTextBox.Text = exception.ToString();
            MessageBox.Show(this, exception.Message, "Import Existing Add-in Project", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void cancelButton_Click(object? sender, EventArgs e)
    {
        DialogResult = DialogResult.Cancel;
        Close();
    }
}
