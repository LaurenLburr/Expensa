using Codex.CommandEngine.Core;
using CodexExpensa.ExtensionDevHost.CommandEngineIntegration.CommonTree;
using System.Text.Json;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Websites;

public sealed class WebsiteTreeProvider : AddinTreeProviderBase<WebsiteTreePayload>
{
    private readonly HostWebsiteRuntimeModuleInvoker invoker;

    public WebsiteTreeProvider()
        : this(new HostWebsiteRuntimeModuleInvoker())
    {
    }

    public WebsiteTreeProvider(HostWebsiteRuntimeModuleInvoker invoker)
        : base(WebsiteTreePayload.Addin)
    {
        ArgumentNullException.ThrowIfNull(invoker);
        this.invoker = invoker;
    }

    public override async Task<IReadOnlyList<AddinTreeNode<WebsiteTreePayload>>> LoadAsync(
        AddinTreeLoadRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        CommandExecutionResult executionResult =
            await invoker.ExecuteAsync(
                new HostWebsiteRuntimeLoadRequest
                {
                    SearchText = request.SearchText,
                    IncludeDisabled = request.IncludeInactive,
                    MaximumRows = request.MaximumRows
                },
                cancellationToken).ConfigureAwait(true);

        return ParseWebsiteTreeNodes(executionResult.OutputJson);
    }

    public override Task<AddinTreeOperationResult> FillAsync(
        WebsiteTreePayload payload,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(payload);

        return Task.FromResult(
            AddinTreeOperationResult.Success(
                $"Loaded {payload.DisplayText}.",
                payload.NodeId));
    }

    public override Task<AddinTreeOperationResult> ModifyAsync(
        WebsiteTreePayload payload,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(payload);

        return Task.FromResult(
            AddinTreeOperationResult.Failure(
                "Website modify is not implemented through CommonTree yet.",
                payload.NodeId));
    }

    public override Task<AddinTreeOperationResult> DeleteAsync(
        WebsiteTreePayload payload,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(payload);

        return Task.FromResult(
            AddinTreeOperationResult.Failure(
                "Website delete is not implemented through CommonTree yet.",
                payload.NodeId));
    }

    private static IReadOnlyList<AddinTreeNode<WebsiteTreePayload>> ParseWebsiteTreeNodes(string outputJson)
    {
        if (string.IsNullOrWhiteSpace(outputJson))
        {
            return [];
        }

        using JsonDocument document = JsonDocument.Parse(outputJson);

        if (!document.RootElement.TryGetProperty("nodes", out JsonElement nodesElement) ||
            nodesElement.ValueKind != JsonValueKind.Array)
        {
            return [];
        }

        List<AddinTreeNode<WebsiteTreePayload>> nodes = [];

        foreach (JsonElement nodeElement in nodesElement.EnumerateArray())
        {
            nodes.Add(ReadWebsiteNode(nodeElement));
        }

        return nodes;
    }

    private static AddinTreeNode<WebsiteTreePayload> ReadWebsiteNode(JsonElement element)
    {
        List<AddinTreeNode<WebsiteTreePayload>> children = [];

        if (element.TryGetProperty("children", out JsonElement childrenElement) &&
            childrenElement.ValueKind == JsonValueKind.Array)
        {
            foreach (JsonElement childElement in childrenElement.EnumerateArray())
            {
                children.Add(ReadWebsiteNode(childElement));
            }
        }

        bool isWebsite = children.Count == 0;

        WebsiteTreePayload payload =
            new(
                isWebsite ? AddinTreeNodeType.Website : AddinTreeNodeType.Group,
                GetString(element, "nodeId"),
                GetString(element, "displayText"))
            {
                WebsiteId = isWebsite ? GetString(element, "nodeId") : string.Empty,
                Url = GetString(element, "url"),
                TagName = GetString(element, "category"),
                IsActive = GetBoolean(element, "isEnabled", defaultValue: true)
            };

        return CreateNode(payload, children);
    }

    private static string GetString(JsonElement element, string propertyName)
    {
        return element.TryGetProperty(propertyName, out JsonElement property) &&
            property.ValueKind == JsonValueKind.String
                ? property.GetString() ?? string.Empty
                : string.Empty;
    }

    private static bool GetBoolean(JsonElement element, string propertyName, bool defaultValue)
    {
        if (!element.TryGetProperty(propertyName, out JsonElement property))
        {
            return defaultValue;
        }

        return property.ValueKind switch
        {
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            _ => defaultValue
        };
    }
}
