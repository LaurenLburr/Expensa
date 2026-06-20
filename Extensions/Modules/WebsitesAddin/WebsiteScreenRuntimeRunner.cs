using Codex.CommandEngine.Core;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace WebsitesAddin;

public sealed class WebsiteScreenRuntimeRunner
{
    private static readonly JsonSerializerOptions JsonOptions =
        new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition =
                JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = true
        };

    public Task<CommandExecutionResult> ExecuteAsync(
        WebsiteScreenRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        cancellationToken.ThrowIfCancellationRequested();

        List<IReadOnlyDictionary<string, object?>> rows =
        [
            new Dictionary<string, object?>
            {
                ["Property"] = "Node type",
                ["Value"] = request.NodeType
            },
            new Dictionary<string, object?>
            {
                ["Property"] = "Website ID",
                ["Value"] = request.EntityId
            },
            new Dictionary<string, object?>
            {
                ["Property"] = "Category",
                ["Value"] = request.Category
            },
            new Dictionary<string, object?>
            {
                ["Property"] = "URL",
                ["Value"] = request.Url
            },
            new Dictionary<string, object?>
            {
                ["Property"] = "Active",
                ["Value"] = request.IsActive
            }
        ];

        WebsiteScreenDocument document =
            new()
            {
                Title = string.IsNullOrWhiteSpace(request.DisplayText)
                    ? "Websites"
                    : request.DisplayText,
                Subtitle = "Website details supplied by WebsitesAddin.",
                Columns = ["Property", "Value"],
                Rows = rows
            };

        return Task.FromResult(
            new CommandExecutionResult
            {
                CorrelationId = Guid.NewGuid().ToString("N"),
                CommandName = "Websites.LoadScreen",
                Status = CommandExecutionStatus.Succeeded,
                Message = $"Loaded screen: {document.Title}",
                OutputJson =
                    JsonSerializer.Serialize(
                        document,
                        JsonOptions)
            });
    }
}
