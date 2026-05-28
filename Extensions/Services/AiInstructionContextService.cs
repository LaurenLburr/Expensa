using System.Text;

namespace CodexExpensa.ExtensionDevHost.Services;

public sealed class AiInstructionContextService
{
    private const int MaxDocumentCharacters = 20_000;

    public string BuildContextForDesignSpec(string designSpecPath)
    {
        StringBuilder builder = new();

        builder.AppendLine("# Standard Instructions and Coding Rules");
        builder.AppendLine();

        AppendWorkspaceDocument(builder, "GeneralCodingSpec.md", "General Coding Spec");
        AppendWorkspaceDocument(builder, "GeneralCodingRules.md", "General Coding Rules");
        AppendWorkspaceDocument(builder, "AiAddinDesignWorkflow.md", "AI Add-in Design Workflow");
        AppendWorkspaceDocument(builder, "AddinDesignSpecTemplate.md", "Add-in Design Spec Template");
        AppendWorkspaceDocument(builder, "ExpensaIntegrationSpecTemplate.md", "Expensa Integration Spec Template");

        AppendProjectSiblingDocument(builder, designSpecPath, "CatchUp.md", "Project Catch-Up Doc");
        AppendProjectSiblingDocument(builder, designSpecPath, "Expensa_Integration_Design_Spec.md", "Project Expensa Integration Spec");

        if (builder.ToString().Trim() == "# Standard Instructions and Coding Rules")
        {
            builder.AppendLine("No standard instruction documents were found.");
        }

        return builder.ToString();
    }

    private static void AppendWorkspaceDocument(StringBuilder builder, string fileName, string title)
    {
        string path = Path.Combine(GetWorkspaceDocsFolder(), fileName);
        AppendDocument(builder, title, path);
    }

    private static void AppendProjectSiblingDocument(
        StringBuilder builder,
        string designSpecPath,
        string fileName,
        string title)
    {
        string? docsFolder = Path.GetDirectoryName(designSpecPath);
        if (string.IsNullOrWhiteSpace(docsFolder))
        {
            return;
        }

        string path = Path.Combine(docsFolder, fileName);
        AppendDocument(builder, title, path);
    }

    private static void AppendDocument(StringBuilder builder, string title, string path)
    {
        if (!File.Exists(path))
        {
            return;
        }

        string content = File.ReadAllText(path);

        if (string.IsNullOrWhiteSpace(content))
        {
            return;
        }

        if (content.Length > MaxDocumentCharacters)
        {
            content = content[..MaxDocumentCharacters] + Environment.NewLine + Environment.NewLine + "[Document truncated for AI context.]";
        }

        builder.AppendLine($"## {title}");
        builder.AppendLine();
        builder.AppendLine($"Path: {path}");
        builder.AppendLine();
        builder.AppendLine("```markdown");
        builder.AppendLine(content.Trim());
        builder.AppendLine("```");
        builder.AppendLine();
    }

    private static string GetWorkspaceDocsFolder()
    {
        return Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Expensa",
            "Extensions",
            "WorkspaceDocs");
    }
}
