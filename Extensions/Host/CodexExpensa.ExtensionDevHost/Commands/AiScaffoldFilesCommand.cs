using System.Diagnostics;
using System.Text;
using CodexExpensa.ExtensionDevHost.Commands.Abstractions;
using CodexExpensa.ExtensionDevHost.Commands.Services;
using CodexExpensa.ExtensionDevHost.Models;
using CodexExpensa.ExtensionDevHost.Services;
using CodexExpensa.ExtensionDevHost.Services.Ai;

namespace CodexExpensa.ExtensionDevHost.Commands;

public sealed class AiScaffoldFilesCommand : ExtMgrCommandBase
{
    public override string CommandKey => "Tools.AiScaffoldFiles";

    public override string TopLevelMenu => "Tools";

    protected override string GetDefaultMenuText() => "AI Scaffold Files";

    protected override int GetDefaultMenuOrder() => 300;

    protected override int GetDefaultItemOrder() => 310;

    public override async void Execute(ICommandContext context)
    {
        ICommandUiService ui = GetUi(context);
        ICommandLogger logger = context.Services.GetRequiredService<ICommandLogger>();

        try
        {
            logger.Log("AiScaffoldFilesCommand started.");

            string solutionRoot = FindSolutionRoot();
            string prompt = "Create an add-in for tracking useful websites by tag.";

            IAddinScaffoldAiService aiService = new OpenAiAddinScaffoldAiService(new OpenAiApiKeyStore());

            AddinScaffoldAiResult aiResult = await aiService.GenerateAsync(new AddinScaffoldAiRequest
            {
                ProjectName = "AiGeneratedAddin",
                AssemblyName = "CodexExpensa.Feature.AiGeneratedAddin",
                Description = "AI generated add-in scaffold.",
                Prompt = prompt
            });

            string projectName = UseFallback(aiResult.SuggestedProjectName, "AiGeneratedAddin");
            string assemblyName = UseFallback(aiResult.SuggestedAssemblyName, projectName);
            string description = UseFallback(aiResult.SuggestedDescription, "AI generated add-in scaffold.");

            NewAddinProjectRequest request = new()
            {
                ProjectName = projectName,
                SolutionRootFolder = solutionRoot,
                AssemblyName = assemblyName,
                Description = description,
                RegisterProject = true,
                UseChatGpt = true,
                ChatGptPrompt = prompt
            };

            ExtensionProjectRegistrationStore store = new();
            AddinProjectScaffolder scaffolder = new();

            int sortOrder = store.GetNextSortOrder();
            AddinProjectScaffolderResult scaffoldResult = scaffolder.Create(request, sortOrder);
            store.Upsert(scaffoldResult.Registration);

            StringBuilder message = new();
            message.AppendLine("AI scaffold files created successfully.");
            message.AppendLine();
            message.AppendLine("Project folder:");
            message.AppendLine(scaffoldResult.ProjectFolder);
            message.AppendLine();
            message.AppendLine("Created files:");

            foreach (string path in scaffoldResult.CreatedPaths)
            {
                message.AppendLine("- " + path);
            }

            logger.Log($"AiScaffoldFilesCommand created files in {scaffoldResult.ProjectFolder}.");

            MessageBox.Show(
                ui.Owner,
                message.ToString(),
                "AI Scaffold Files",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            Process.Start(new ProcessStartInfo
            {
                FileName = scaffoldResult.ProjectFolder,
                UseShellExecute = true
            });
        }
        catch (Exception ex)
        {
            logger.Log($"AiScaffoldFilesCommand failed: {ex.Message}");

            MessageBox.Show(
                ui.Owner,
                ex.Message,
                "AI Scaffold Files",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }

    private static string UseFallback(string value, string fallback)
    {
        return string.IsNullOrWhiteSpace(value) ? fallback : value.Trim();
    }

    private static string FindSolutionRoot()
    {
        DirectoryInfo? directory = new(AppContext.BaseDirectory);

        while (directory is not null)
        {
            if (string.Equals(directory.Name, "Extensions", StringComparison.OrdinalIgnoreCase))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        string fallback = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "Extensions");

        if (!Directory.Exists(fallback))
        {
            throw new InvalidOperationException(
                "Could not find the Extensions solution root. Run from the Extensions repo or create/select the solution root first.");
        }

        return fallback;
    }
}
