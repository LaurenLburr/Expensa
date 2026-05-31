namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Websites;

public static class WebsitesDatabasePanelCopyMessageFormatter
{
    public static string FormatCopyFromExpensaProdMessage(
        HostWebsiteExpensaProdDatabaseCopyResult result)
    {
        ArgumentNullException.ThrowIfNull(result);

        string message =
            $"Copied Expensa production website data to:{Environment.NewLine}{Environment.NewLine}" +
            $"Runtime: {result.RuntimeDatabasePath}{Environment.NewLine}" +
            $"Dev: {result.DevDatabasePath}";

        if (result.Warnings.Count == 0)
        {
            return message;
        }

        return message +
            $"{Environment.NewLine}{Environment.NewLine}Warnings:{Environment.NewLine}" +
            string.Join(Environment.NewLine + Environment.NewLine, result.Warnings);
    }
}
