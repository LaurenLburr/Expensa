using CodexExpensa.ExtensionDevHost.CommandEngineIntegration.CommonTree;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Websites;

public sealed class WebsiteTreePayload : AddinTreePayloadBase
{
    public const string Addin = "WebsitesAddin";

    public WebsiteTreePayload(
        AddinTreeNodeType nodeType,
        string nodeId,
        string displayText)
        : base(Addin, nodeType, nodeId, displayText)
    {
    }

    public string WebsiteId { get; init; } = string.Empty;

    public string Url { get; init; } = string.Empty;

    public string TagName { get; init; } = string.Empty;

    public bool IsActive { get; init; } = true;
}
