using System.Text.Json;

namespace CodexExpensa.App.WinForms.UI.Websites;

public static class WebsiteLoadResultParser
{
    private static readonly JsonSerializerOptions SerializerOptions =
        new()
        {
            PropertyNameCaseInsensitive = true
        };

    public static WebsiteLoadResult Parse(
        string outputJson)
    {
        if (string.IsNullOrWhiteSpace(outputJson))
        {
            return new WebsiteLoadResult
            {
                Message = "No website output was returned."
            };
        }

        WebsiteLoadResult? result =
            JsonSerializer.Deserialize<WebsiteLoadResult>(
                outputJson,
                SerializerOptions);

        return result ?? new WebsiteLoadResult
        {
            Message = "Website output could not be parsed."
        };
    }
}
