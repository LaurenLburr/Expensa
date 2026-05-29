using System.Text.Json;
using Codex.CommandEngine.Abstractions;

namespace WebsitesAddin;

public sealed class WebsiteLoadCommandHandler : ICommandHandler
{
    private readonly WebsiteLoadCommand _command;

    public WebsiteLoadCommandHandler()
        : this(new WebsiteLoadCommand())
    {
    }

    public WebsiteLoadCommandHandler(
        WebsiteLoadCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);

        _command = command;
    }

    public CommandDefinition Definition =>
        new()
        {
            CommandName = "websites.load",
            DisplayName = "Load Websites",
            Description = "Loads Websites into a UI-neutral tree structure."
        };

    public Task<CommandExecutionResult> ValidateAsync(
        CommandRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        cancellationToken.ThrowIfCancellationRequested();

        if (!string.Equals(
                request.CommandName,
                Definition.CommandName,
                StringComparison.OrdinalIgnoreCase))
        {
            return Task.FromResult(
                CommandExecutionResult.Failure(
                    $"Unsupported command '{request.CommandName}'."));
        }

        return Task.FromResult(
            CommandExecutionResult.Success(
                "Validation succeeded."));
    }

    public Task<CommandExecutionResult> ExecuteAsync(
        CommandRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        cancellationToken.ThrowIfCancellationRequested();

        WebsiteLoadRequest loadRequest =
            WebsiteLoadRequestParser.Parse(request.Parameters);

        WebsiteLoadResult result =
            _command.Execute(loadRequest);

        string outputJson =
            JsonSerializer.Serialize(
                result,
                WebsiteJsonSerializerOptions.Default);

        return Task.FromResult(
            CommandExecutionResult.Success(outputJson));
    }
}
