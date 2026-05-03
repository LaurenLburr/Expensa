namespace CodexExpensa.ExtensionDevHost.Models;

public class TechSpec
{
    public string AddonName { get; set; } = string.Empty;
    public string ProjectFolder { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; } = DateTime.Now;

    // Conversation 1: Vision & Requirements
    public string VisionStatement { get; set; } = string.Empty;
    public List<TechSpecConversationMessage> Conversation1 { get; set; } = new();
    public string Section1_VisionAndRequirements { get; set; } = string.Empty;

    // Conversation 2: Implementation Strategy
    public List<TechSpecConversationMessage> Conversation2 { get; set; } = new();
    public string Section2_ImplementationStrategy { get; set; } = string.Empty;

    public string GenerateMarkdown()
    {
        var sb = new System.Text.StringBuilder();

        sb.AppendLine($"# {AddonName} - Technical Specification");
        sb.AppendLine();
        sb.AppendLine($"**Created:** {CreatedDate:yyyy-MM-dd HH:mm:ss}");
        sb.AppendLine($"**Addon:** {AddonName}");
        sb.AppendLine();

        sb.AppendLine("## Section 1: Vision & Requirements");
        sb.AppendLine();
        sb.AppendLine(Section1_VisionAndRequirements);
        sb.AppendLine();

        sb.AppendLine("## Section 2: Implementation Strategy");
        sb.AppendLine();
        sb.AppendLine(Section2_ImplementationStrategy);
        sb.AppendLine();

        sb.AppendLine("---");
        sb.AppendLine();
        sb.AppendLine("*Tech Spec built with CodexExpensa Tech Spec Builder*");

        return sb.ToString();
    }

    public void SaveToFile(string filePath)
    {
        string? folder = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrWhiteSpace(folder))
        {
            Directory.CreateDirectory(folder);
        }

        File.WriteAllText(filePath, GenerateMarkdown());
    }

    public static TechSpec CreateNew(string addonName, string projectFolder)
    {
        return new TechSpec
        {
            AddonName = addonName,
            ProjectFolder = projectFolder,
            CreatedDate = DateTime.Now
        };
    }
}

public class TechSpecConversationMessage
{
    public string Role { get; set; } = string.Empty; // "User" or "AI"
    public string Content { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.Now;
}
