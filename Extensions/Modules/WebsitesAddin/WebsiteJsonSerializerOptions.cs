using System.Text.Json;
using System.Text.Json.Serialization;

namespace WebsitesAddin;

public static class WebsiteJsonSerializerOptions
{
    public static JsonSerializerOptions Default { get; } =
        Create();

    private static JsonSerializerOptions Create()
    {
        JsonSerializerOptions options = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
            AllowTrailingCommas = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = true
        };

        options.Converters.Add(
            new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));

        return options;
    }
}
