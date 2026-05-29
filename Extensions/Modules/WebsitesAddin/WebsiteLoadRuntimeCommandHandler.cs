using System.Text.Json;
using Codex.CommandEngine.Core;

namespace WebsitesAddin;

public sealed class WebsiteLoadRuntimeCommandHandler : ICommandHandler
{
    private readonly WebsiteLoadCommand _command;

    public WebsiteLoadRuntimeCommandHandler()
        : this(new WebsiteLoadCommand())
    {
    }

    public WebsiteLoadRuntimeCommandHandler(
        WebsiteLoadCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);

        _command = command;
    }

    public string CommandName =>
        "websites.load";

    public Task<CommandExecutionResult> ExecuteAsync(
        CommandExecutionRequest request,
        ICommandExecutionContext context)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(context);

        context.CancellationToken.ThrowIfCancellationRequested();

        if (!string.Equals(request.CommandName, CommandName, StringComparison.OrdinalIgnoreCase))
        {
            return Task.FromResult(
                CommandExecutionResult.Failed(
                    request.CommandName,
                    request.CorrelationId,
                    $"Unsupported command '{request.CommandName}'."));
        }

        WebsiteLoadRequest loadRequest =
            WebsiteLoadRequestParser.Parse(request.Parameters);

        WebsiteLoadResult result =
            _command.Execute(loadRequest);

        string outputJson =
            JsonSerializer.Serialize(
                result,
                WebsiteJsonSerializerOptions.Default);

        context.WriteLog($"Loaded {result.TotalCount} website root node(s).");

        return Task.FromResult(
            CommandExecutionResult.Succeeded(
                CommandName,
                request.CorrelationId,
                result.Message,
                outputJson));
    }
}
