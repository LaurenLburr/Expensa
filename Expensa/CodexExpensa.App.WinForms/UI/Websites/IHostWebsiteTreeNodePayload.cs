namespace CodexExpensa.App.WinForms.UI.Websites;

public interface IHostWebsiteTreeNodePayload
{
    string NodeId { get; }

    string WebsiteId { get; }

    string DisplayText { get; }

    string Url { get; }

    string TagName { get; }

    HostWebsiteTreeNodeType NodeType { get; }

    bool IsActive { get; }
}
