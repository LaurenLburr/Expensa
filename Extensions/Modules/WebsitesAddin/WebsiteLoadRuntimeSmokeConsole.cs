using Codex.CommandEngine.Core;

namespace WebsitesAddin;

public static class WebsiteLoadRuntimeSmokeConsole
{
    public static async Task<int> RunAsync(
        string[] args,
        CancellationToken cancellationToken = default)
    {
        string outputPath =
            args.Length > 0 && !string.IsNullOrWhiteSpace(args[0])
                ? args[0]
                : Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "Expensa",
                    "Extensions",
                    "WebsitesAddin",
                    "websites-load-smoke.json");

        string searchText =
            args.Length > 1
                ? args[1]
                : string.Empty;

        WebsiteLoadRuntimeSmokeRunner runner = new();

        CommandExecutionResult result =
            await runner.ExecuteAndSaveAsync(
                new WebsiteLoadRequest
                {
                    SearchText = searchText,
                    MaximumRows = 500
                },
                outputPath,
                cancellationToken).ConfigureAwait(false);

        return result.Status == CommandExecutionStatus.Succeeded
            ? 0
            : 1;
    }
}
