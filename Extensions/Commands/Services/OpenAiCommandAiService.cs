using System.Text;
using CodexExpensa.ExtensionDevHost.Models;
using CodexExpensa.ExtensionDevHost.Services.Ai;

namespace CodexExpensa.ExtensionDevHost.Commands.Services;

public sealed class OpenAiCommandAiService : ICommandAiService
{
    private readonly OpenAiApiKeyStore _apiKeyStore;

    public OpenAiCommandAiService(OpenAiApiKeyStore apiKeyStore)
    {
        _apiKeyStore = apiKeyStore ?? throw new ArgumentNullException(nameof(apiKeyStore));
    }

    public async Task<string> GenerateScaffoldAsync(string prompt, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(prompt);

        OpenAiAddinScaffoldAiService aiService = new(_apiKeyStore);

        AddinScaffoldAiRequest request = new()
        {
            ProjectName = "AiGeneratedAddin",
            AssemblyName = "CodexExpensa.Feature.AiGeneratedAddin",
            Description = "AI generated add-in scaffold test.",
            Prompt = prompt
        };

        AddinScaffoldAiResult result = await aiService.GenerateAsync(request, cancellationToken);

        StringBuilder builder = new();
        builder.AppendLine("AI scaffold suggestion:");
        builder.AppendLine();
        builder.AppendLine($"Project: {result.SuggestedProjectName}");
        builder.AppendLine($"Assembly: {result.SuggestedAssemblyName}");
        builder.AppendLine($"Class: {result.SuggestedClassName}");
        builder.AppendLine($"Root node: {result.SuggestedRootNodeName}");
        builder.AppendLine();
        builder.AppendLine(result.SuggestedDescription);

        return builder.ToString();
    }
}
