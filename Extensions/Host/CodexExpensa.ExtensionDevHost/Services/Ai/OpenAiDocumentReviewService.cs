using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace CodexExpensa.ExtensionDevHost.Services.Ai;

public sealed class OpenAiDocumentReviewService
{
    private readonly HttpClient _httpClient;

    public OpenAiDocumentReviewService(OpenAiApiKeyStore apiKeyStore)
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

    public async Task<string> ReviewMarkdownAsync(
        string documentName,
        string markdown,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(documentName);
        ArgumentException.ThrowIfNullOrWhiteSpace(markdown);

        string prompt = $"""
You are reviewing project documentation.

Document name:
{documentName}

Task:
- Correct spelling, grammar, and punctuation.
- Improve clarity and wording.
- Format the document professionally in Markdown.
- Preserve technical meaning.
- Preserve headings where practical.
- Preserve code blocks exactly unless they contain obvious prose comments.
- Return only the revised Markdown.
- Do not wrap the response in markdown fences.

Document:
{markdown}
""";

        var payload = new
        {
            model = "gpt-5-mini",
            input = prompt
        };

        string json = JsonSerializer.Serialize(payload);

        using StringContent content = new(json, Encoding.UTF8, "application/json");
        using HttpResponseMessage response = await _httpClient.PostAsync("v1/responses", content, cancellationToken);

        string responseJson = await response.Content.ReadAsStringAsync(cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException(
                $"OpenAI document review failed: {(int)response.StatusCode} {response.ReasonPhrase}{Environment.NewLine}{responseJson}");
        }

        using JsonDocument document = JsonDocument.Parse(responseJson);
        return ExtractOutputText(document.RootElement).Trim();
    }

    private static string ExtractOutputText(JsonElement root)
    {
        if (root.TryGetProperty("output_text", out JsonElement outputTextElement) &&
            outputTextElement.ValueKind == JsonValueKind.String)
        {
            return outputTextElement.GetString() ?? string.Empty;
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
                        return textElement.GetString() ?? string.Empty;
                    }
                }
            }
        }

        throw new InvalidOperationException("OpenAI response did not contain output text.");
    }
}
