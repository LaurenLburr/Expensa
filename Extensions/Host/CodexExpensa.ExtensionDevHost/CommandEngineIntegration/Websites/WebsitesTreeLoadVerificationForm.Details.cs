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

        _websitesDetailsPanel = new WebsitesDetailsPanel();

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

        if (node is null)
        {
            _websitesDetailsPanel.ShowEmpty();
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
}
