using System.Text.Json;
using System.Text.Json.Serialization;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Websites;

public static class HostWebsiteJsonSerializerOptions
{
    public static JsonSerializerOptions Default { get; } =
        Create();

    private static JsonSerializerOptions Create()
    {
        JsonSerializerOptions options = new()
        {
            PropertyNameCaseInsensitive = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
            AllowTrailingCommas = true
        };

        options.Converters.Add(
            new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));

        return options;
    }
}
