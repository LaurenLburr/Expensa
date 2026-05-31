using Codex.CommandEngine.Core;

namespace WebsitesAddin;

public sealed class WebsiteLoadRuntimeSmokeRunner
{
    private readonly WebsiteLoadRuntimeCommandHandler _handler;

    public WebsiteLoadRuntimeSmokeRunner()
        : this(new WebsiteLoadRuntimeCommandHandler())
    {
    }

    public WebsiteLoadRuntimeSmokeRunner(
        WebsiteLoadRuntimeCommandHandler handler)
    {
        ArgumentNullException.ThrowIfNull(handler);

        _handler = handler;
    }

    public async Task<CommandExecutionResult> ExecuteAsync(
        WebsiteLoadRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        CommandExecutionRequest commandRequest = new()
        {
            CommandName = _handler.CommandName,
            Parameters = new Dictionary<string, object?>
            {
                ["searchText"] = request.SearchText,
                ["includeDisabled"] = request.IncludeDisabled,
                ["maximumRows"] = request.MaximumRows,
                ["databasePath"] = request.DatabasePath
            }
        };

        SmokeCommandExecutionContext context =
            new(commandRequest.CorrelationId, cancellationToken);

        return await _handler.ExecuteAsync(commandRequest, context)
            .ConfigureAwait(false);
    }

    public async Task<CommandExecutionResult> ExecuteAndSaveAsync(
        WebsiteLoadRequest request,
        string outputPath,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrWhiteSpace(outputPath);

        CommandExecutionResult result =
            await ExecuteAsync(request, cancellationToken).ConfigureAwait(false);

        string? folder =
            Path.GetDirectoryName(outputPath);

        if (!string.IsNullOrWhiteSpace(folder))
        {
            Directory.CreateDirectory(folder);
        }

        File.WriteAllText(outputPath, result.OutputJson);

        return result;
    }

    private sealed class SmokeCommandExecutionContext : ICommandExecutionContext
    {
        public SmokeCommandExecutionContext(
            string correlationId,
            CancellationToken cancellationToken)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(correlationId);

            CorrelationId = correlationId;
            CancellationToken = cancellationToken;
        }

        public string CorrelationId { get; }

        public string ContextJson { get; } = "{}";

        public CancellationToken CancellationToken { get; }

        public List<string> LogMessages { get; } = [];

        public void WriteLog(
            string message)
        {
            LogMessages.Add(message);
        }
    }
}
