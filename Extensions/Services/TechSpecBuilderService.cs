using CodexExpensa.ExtensionDevHost.Models;

namespace CodexExpensa.ExtensionDevHost.Services;

public class TechSpecBuilderService
{
    private TechSpec? _currentSpec;

    public TechSpec CurrentSpec => _currentSpec ?? throw new InvalidOperationException("No tech spec initialized");

    public void Initialize(string addonName, string projectFolder)
    {
        _currentSpec = TechSpec.CreateNew(addonName, projectFolder);
    }

    public void SetVisionStatement(string visionStatement)
    {
        CurrentSpec.VisionStatement = visionStatement;
    }

    public void AddConversation1Message(string role, string content)
    {
        CurrentSpec.Conversation1.Add(new TechSpecConversationMessage
        {
            Role = role,
            Content = content,
            Timestamp = DateTime.Now
        });
    }

    public void AddConversation2Message(string role, string content)
    {
        CurrentSpec.Conversation2.Add(new TechSpecConversationMessage
        {
            Role = role,
            Content = content,
            Timestamp = DateTime.Now
        });
    }

    public void SetSection1(string content)
    {
        CurrentSpec.Section1_VisionAndRequirements = content;
    }

    public void SetSection2(string content)
    {
        CurrentSpec.Section2_ImplementationStrategy = content;
    }

    public void SaveTechSpec()
    {
        string docsFolder = Path.Combine(CurrentSpec.ProjectFolder, "Docs");
        string techSpecPath = Path.Combine(docsFolder, "TechSpec.md");
        CurrentSpec.SaveToFile(techSpecPath);
    }

    public string GetMarkdownPreview()
    {
        return CurrentSpec.GenerateMarkdown();
    }

    public void Reset()
    {
        _currentSpec = null;
    }
}
