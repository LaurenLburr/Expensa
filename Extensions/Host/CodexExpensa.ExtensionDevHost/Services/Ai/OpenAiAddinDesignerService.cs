using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using CodexExpensa.ExtensionDevHost.Models;

namespace CodexExpensa.ExtensionDevHost.Services.Ai;

public sealed class OpenAiAddinDesignerService
{
    private readonly HttpClient _httpClient;

    public OpenAiAddinDesignerService(OpenAiApiKeyStore apiKeyStore)
    {
        ArgumentNullException.ThrowIfNull(apiKeyStore);

        string apiKey = apiKeyStore.GetApiKey();
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            throw new InvalidOperationException("OpenAI API key is not configured. Use Project > OpenAI API Key.");
        }

        _httpClient = new HttpClient { BaseAddress = new Uri("https://api.openai.com/") };
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
    }

    public async Task<AiAddinDesignerResponse> ContinueDesignAsync(
        AiAddinDesignSession session,
        string userMessage,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(session);
        ArgumentException.ThrowIfNullOrWhiteSpace(userMessage);

        string conversation = string.Join(
            Environment.NewLine + Environment.NewLine,
            session.Messages.Select(static x => $"{x.Role}: {x.Content}"));

        string prompt = $"""
You are helping design a C# WinForms add-in project for CodexExpensa.

Important rules:
- The extension workflow is project-based. Do not assume runtime DLL loading.
- Design first. Generate code later.
- Ask clarifying questions when needed.
- Maintain the add-in design spec and Expensa integration spec.
- Preserve user decisions.
- Do not produce code unless the user explicitly asks for generation.
- Return JSON only.

Current project name:
{session.ProjectName}

Conversation:
{conversation}

Current Add-in Design Spec:
{session.CurrentSpecMarkdown}

Current Expensa Integration Spec:
{session.CurrentIntegrationSpecMarkdown}

New user message:
{userMessage}

Return JSON with exactly these fields:
assistantMessage
projectName
updatedSpecMarkdown
updatedIntegrationSpecMarkdown
""";

        var payload = new
        {
            model = "gpt-5-mini",
            input = prompt,
            text = new
            {
                format = new
                {
                    type = "json_schema",
                    name = "addin_designer_response",
                    schema = new
                    {
                        type = "object",
                        additionalProperties = false,
                        properties = new
                        {
                            assistantMessage = new { type = "string" },
                            projectName = new { type = "string" },
                            updatedSpecMarkdown = new { type = "string" },
                            updatedIntegrationSpecMarkdown = new { type = "string" }
                        },
                        required = new[]
                        {
                            "assistantMessage",
                            "projectName",
                            "updatedSpecMarkdown",
                            "updatedIntegrationSpecMarkdown"
                        }
                    }
                }
            }
        };

        string json = JsonSerializer.Serialize(payload);
        using StringContent content = new(json, Encoding.UTF8, "application/json");
        using HttpResponseMessage response = await _httpClient.PostAsync("v1/responses", content, cancellationToken);

        string responseJson = await response.Content.ReadAsStringAsync(cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException(
                $"OpenAI design request failed: {(int)response.StatusCode} {response.ReasonPhrase}{Environment.NewLine}{responseJson}");
        }

        using JsonDocument document = JsonDocument.Parse(responseJson);
        string outputText = ExtractOutputText(document.RootElement);

        AiAddinDesignerResponse? result = JsonSerializer.Deserialize<AiAddinDesignerResponse>(
            outputText,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        return result ?? new AiAddinDesignerResponse
        {
            AssistantMessage = "I could not parse the AI response.",
            ProjectName = session.ProjectName,
            UpdatedSpecMarkdown = session.CurrentSpecMarkdown,
            UpdatedIntegrationSpecMarkdown = session.CurrentIntegrationSpecMarkdown
        };
    }

    private static string ExtractOutputText(JsonElement root)
    {
        if (root.TryGetProperty("output_text", out JsonElement outputTextElement) &&
            outputTextElement.ValueKind == JsonValueKind.String)
        {
            return outputTextElement.GetString() ?? "{}";
        }

        if (root.TryGetProperty("output", out JsonElement outputArray) &&
            outputArray.ValueKind == JsonValueKind.Array)
        {
            foreach (JsonElement item in outputArray.EnumerateArray())
            {
                if (!item.TryGetProperty("content", out JsonElement contentArray) ||
                    contentArray.ValueKind != JsonValueKind.Array)
                {
                    continue;
                }

                foreach (JsonElement contentItem in contentArray.EnumerateArray())
                {
                    if (contentItem.TryGetProperty("text", out JsonElement textElement) &&
                        textElement.ValueKind == JsonValueKind.String)
                    {
                        return textElement.GetString() ?? "{}";
                    }
                }
            }
        }

        throw new InvalidOperationException("OpenAI response did not contain output text.");
    }
}

public sealed class AiAddinDesignerResponse
{
    public string AssistantMessage { get; set; } = string.Empty;
    public string ProjectName { get; set; } = string.Empty;
    public string UpdatedSpecMarkdown { get; set; } = string.Empty;
    public string UpdatedIntegrationSpecMarkdown { get; set; } = string.Empty;
}
