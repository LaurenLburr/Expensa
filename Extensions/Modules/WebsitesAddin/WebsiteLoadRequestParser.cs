namespace WebsitesAddin;

public static class WebsiteLoadRequestParser
{
    public static WebsiteLoadRequest Parse(
        IReadOnlyDictionary<string, string> parameters)
    {
        ArgumentNullException.ThrowIfNull(parameters);

        return new WebsiteLoadRequest
        {
            SearchText = GetString(parameters, "searchText"),
            IncludeDisabled = GetBool(parameters, "includeDisabled"),
            MaximumRows = GetInt32(parameters, "maximumRows", 500),
            DatabasePath = GetString(parameters, "databasePath")
        };
    }

    public static WebsiteLoadRequest Parse(
        IReadOnlyDictionary<string, object?> parameters)
    {
        ArgumentNullException.ThrowIfNull(parameters);

        Dictionary<string, string> converted =
            parameters.ToDictionary(
                static pair => pair.Key,
                static pair => Convert.ToString(pair.Value) ?? string.Empty,
                StringComparer.OrdinalIgnoreCase);

        return Parse(converted);
    }

    private static string GetString(
        IReadOnlyDictionary<string, string> parameters,
        string key)
    {
        return parameters.TryGetValue(key, out string? value)
            ? value
            : string.Empty;
    }

    private static bool GetBool(
        IReadOnlyDictionary<string, string> parameters,
        string key)
    {
        return parameters.TryGetValue(key, out string? value) &&
            bool.TryParse(value, out bool parsed) &&
            parsed;
    }

    private static int GetInt32(
        IReadOnlyDictionary<string, string> parameters,
        string key,
        int defaultValue)
    {
        return parameters.TryGetValue(key, out string? value) &&
            int.TryParse(value, out int parsed)
                ? parsed
                : defaultValue;
    }
}
