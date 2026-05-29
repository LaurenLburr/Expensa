using System.Text.Json;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Websites;

public static class HostWebsiteLoadResultParser
{
    public static HostWebsiteLoadResult Parse(
        string outputJson)
    {
        if (string.IsNullOrWhiteSpace(outputJson))
        {
            return new HostWebsiteLoadResult
            {
                Message = "No website output was returned."
            };
        }

        HostWebsiteLoadResult? result =
            JsonSerializer.Deserialize<HostWebsiteLoadResult>(
                outputJson,
                HostWebsiteJsonSerializerOptions.Default);

        return result ?? new HostWebsiteLoadResult
        {
            Message = "Website output could not be parsed."
        };
    }
}
