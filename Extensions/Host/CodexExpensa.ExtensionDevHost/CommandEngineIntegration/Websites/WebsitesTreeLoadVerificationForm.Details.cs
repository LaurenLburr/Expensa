namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Websites;

public sealed partial class WebsitesTreeLoadVerificationForm
{
    private WebsitesDetailsPanel? _websitesDetailsPanel;

    private void EnsureWebsiteDetailsPanel()
    {
        if (_websitesDetailsPanel is not null)
        {
            return;
        }

        splitContainer.FixedPanel = FixedPanel.Panel1;
        splitContainer.Panel1MinSize = 240;
        splitContainer.Panel2MinSize = 420;

        _websitesDetailsPanel = new WebsitesDetailsPanel();
        _websitesDetailsPanel.WebsiteOverviewRowDoubleClicked += websitesDetailsPanel_WebsiteOverviewRowDoubleClicked;

        splitContainer.Panel2.Controls.Add(_websitesDetailsPanel);
        _websitesDetailsPanel.BringToFront();
    }

    private void ShowSelectedTreeNodeDetails(TreeNode? node)
    {
        EnsureWebsiteDetailsPanel();

        if (_websitesDetailsPanel is null)
        {
            return;
        }

        _websitesDetailsPanel.SaveCurrentCredential();

        if (node is null)
        {
            _websitesDetailsPanel.ShowEmpty();
            return;
        }

        if (IsWebsitesRootNode(node))
        {
            _websitesDetailsPanel.ShowOverview(GetWebsitePayloadsFromTree());
            return;
        }

        if (node.Tag is HostWebsiteTreeNodePayload websitePayload)
        {
            _websitesDetailsPanel.ShowWebsite(websitePayload);
            return;
        }

        if (node.Tag is IHostWebsiteTreeNodePayload payload)
        {
            _websitesDetailsPanel.ShowGroup(payload);
            return;
        }

        _websitesDetailsPanel.ShowEmpty();
    }

    private void websitesDetailsPanel_WebsiteOverviewRowDoubleClicked(object? sender, string websiteId)
    {
        TreeNode? matchingNode =
            FindWebsiteTreeNodeByWebsiteId(websitesTreeView.Nodes, websiteId);

        if (matchingNode is null)
        {
            return;
        }

        websitesTreeView.SelectedNode = matchingNode;
        matchingNode.EnsureVisible();
    }

    private static bool IsWebsitesRootNode(TreeNode node)
    {
        return node.Parent is null &&
            (string.Equals(node.Name, HostWebsiteTreeContributionRenderer.WebsitesRootNodeName, StringComparison.OrdinalIgnoreCase) ||
             string.Equals(node.Text, "Website", StringComparison.OrdinalIgnoreCase) ||
             string.Equals(node.Text, "Websites", StringComparison.OrdinalIgnoreCase));
    }

    private IReadOnlyList<HostWebsiteTreeNodePayload> GetWebsitePayloadsFromTree()
    {
        List<HostWebsiteTreeNodePayload> payloads = [];

        CollectWebsitePayloads(websitesTreeView.Nodes, payloads);

        return payloads
            .OrderBy(static payload => payload.DisplayText, StringComparer.CurrentCultureIgnoreCase)
            .ToList();
    }

    private static void CollectWebsitePayloads(
        TreeNodeCollection nodes,
        List<HostWebsiteTreeNodePayload> payloads)
    {
        foreach (TreeNode node in nodes)
        {
            if (node.Tag is HostWebsiteTreeNodePayload payload)
            {
                payloads.Add(payload);
            }

            CollectWebsitePayloads(node.Nodes, payloads);
        }
    }

    private static TreeNode? FindWebsiteTreeNodeByWebsiteId(
        TreeNodeCollection nodes,
        string websiteId)
    {
        foreach (TreeNode node in nodes)
        {
            if (node.Tag is HostWebsiteTreeNodePayload payload &&
                string.Equals(payload.WebsiteId, websiteId, StringComparison.OrdinalIgnoreCase))
            {
                return node;
            }

            TreeNode? matchingChild =
                FindWebsiteTreeNodeByWebsiteId(node.Nodes, websiteId);

            if (matchingChild is not null)
            {
                return matchingChild;
            }
        }

        return null;
    }
}
