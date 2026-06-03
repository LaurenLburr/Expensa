namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Websites;

public interface IHostWebsiteTreeNodePayload
{
    HostWebsiteTreeNodeType NodeType { get; }

    string NodeId { get; }

    string DisplayText { get; }

    string WebsiteId { get; }

    string TagName { get; }

    string Url { get; }

    bool IsActive { get; }
}
