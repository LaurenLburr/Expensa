using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using CodexExpensa.ExtensionDevHost.Models;

namespace CodexExpensa.ExtensionDevHost.Services.Ai;

public sealed class OpenAiAddinScaffoldAiService : IAddinScaffoldAiService
{
    private readonly HttpClient _httpClient;

    public OpenAiAddinScaffoldAiService(OpenAiApiKeyStore apiKeyStore)
    {
        ArgumentNullException.ThrowIfNull(apiKeyStore);

        string apiKey = apiKeyStore.GetApiKey();
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            throw new InvalidOperationException("OpenAI API key is not configured. Use Project > OpenAI API Key.");
        }

        _httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://api.openai.com/")
        };

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
    }

    public async Task<AddinScaffoldAiResult> GenerateAsync(AddinScaffoldAiRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        string prompt = BuildPrompt(request);

        var payload = new
        {
            model = "gpt-5-mini",
            input = prompt,
            text = new
            {
                format = new
                {
                    type = "json_schema",
                    name = "addin_scaffold",
                    schema = new
                    {
                        type = "object",
                        additionalProperties = false,
                        properties = new
                        {
                            SuggestedProjectName = new { type = "string" },
                            SuggestedAssemblyName = new { type = "string" },
                            SuggestedDescription = new { type = "string" },
                            SuggestedClassName = new { type = "string" },
                            SuggestedRootNodeName = new { type = "string" }
                        },
                        required = new[]
                        {
                            "SuggestedProjectName",
                            "SuggestedAssemblyName",
                            "SuggestedDescription",
                            "SuggestedClassName",
                            "SuggestedRootNodeName"
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
            throw new InvalidOperationException($"OpenAI request failed: {(int)response.StatusCode} {response.ReasonPhrase}{Environment.NewLine}{responseJson}");
        }

        using JsonDocument document = JsonDocument.Parse(responseJson);
        string outputText = ExtractOutputText(document.RootElement);

        AddinScaffoldAiResult? result = JsonSerializer.Deserialize<AddinScaffoldAiResult>(
            outputText,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        return result ?? new AddinScaffoldAiResult();
    }

    private static string BuildPrompt(AddinScaffoldAiRequest request)
    {
        return $"""
Return JSON only with these properties:
- SuggestedProjectName
- SuggestedAssemblyName
- SuggestedDescription
- SuggestedClassName
- SuggestedRootNodeName

Constraints:
- Keep names concise.
- Use identifier-friendly names.
- No markdown fences.
- No explanation text.

ProjectName: {request.ProjectName}
AssemblyName: {request.AssemblyName}
Description: {request.Description}
UserPrompt: {request.Prompt}
""";
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
