using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace CodexExpensa.ExtensionDevHost.Services.Ai;

public sealed class OpenAiDesignConversationService
{
    private readonly HttpClient _httpClient;

    public OpenAiDesignConversationService(OpenAiApiKeyStore apiKeyStore)
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

    public async Task<string> SendAsync(
        string prompt,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(prompt);

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
                $"OpenAI design conversation request failed: {(int)response.StatusCode} {response.ReasonPhrase}{Environment.NewLine}{responseJson}");
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
