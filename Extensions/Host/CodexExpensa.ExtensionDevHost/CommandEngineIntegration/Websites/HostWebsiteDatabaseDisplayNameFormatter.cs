namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Websites;

public static class HostWebsiteDatabaseDisplayNameFormatter
{
    public static string Ellipsize(
        string value,
        int maximumLength)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        if (maximumLength < 4)
        {
            return value.Length <= maximumLength
                ? value
                : value[..maximumLength];
        }

        if (value.Length <= maximumLength)
        {
            return value;
        }

        int visibleLength =
            maximumLength - 3;

        return $"{value[..visibleLength]}...";
    }
}
