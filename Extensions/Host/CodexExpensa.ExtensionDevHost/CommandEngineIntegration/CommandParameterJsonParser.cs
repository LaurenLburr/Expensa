using System.Text.Json;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration;

public static class CommandParameterJsonParser
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true
    };

    public static IReadOnlyDictionary<string, object?> Parse(
        string json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return new Dictionary<string, object?>();
        }

        using JsonDocument document =
            JsonDocument.Parse(
                json,
                new JsonDocumentOptions
                {
                    CommentHandling = JsonCommentHandling.Skip,
                    AllowTrailingCommas = true
                });

        if (document.RootElement.ValueKind != JsonValueKind.Object)
        {
            throw new InvalidOperationException("Command parameters must be a JSON object.");
        }

        Dictionary<string, object?> parameters =
            new(StringComparer.OrdinalIgnoreCase);

        foreach (JsonProperty property in document.RootElement.EnumerateObject())
        {
            parameters[property.Name] =
                ConvertElement(property.Value);
        }

        return parameters;
    }

    private static object? ConvertElement(
        JsonElement element)
    {
        return element.ValueKind switch
        {
            JsonValueKind.String => element.GetString(),
            JsonValueKind.Number => ConvertNumber(element),
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            JsonValueKind.Null => null,
            JsonValueKind.Object => JsonSerializer.Deserialize<Dictionary<string, object?>>(element.GetRawText(), JsonOptions),
            JsonValueKind.Array => JsonSerializer.Deserialize<List<object?>>(element.GetRawText(), JsonOptions),
            _ => element.GetRawText()
        };
    }

    private static object ConvertNumber(
        JsonElement element)
    {
        if (element.TryGetInt32(out int intValue))
        {
            return intValue;
        }

        if (element.TryGetInt64(out long longValue))
        {
            return longValue;
        }

        if (element.TryGetDecimal(out decimal decimalValue))
        {
            return decimalValue;
        }

        return element.GetDouble();
    }
}
