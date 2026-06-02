namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Websites;

public sealed class WebsitesDetailsPanel : UserControl
{
    private readonly Label titleLabel = new();
    private readonly Label websiteIdLabel = new();
    private readonly LinkLabel urlLinkLabel = new();
    private readonly Label tagLabel = new();
    private readonly TextBox detailTextBox = new();

    public WebsitesDetailsPanel()
    {
        TableLayoutPanel rootLayoutPanel = new()
        {
            ColumnCount = 1,
            RowCount = 5,
            Dock = DockStyle.Fill,
            Padding = new Padding(8)
        };

        rootLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F));
        rootLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 24F));
        rootLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 24F));
        rootLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 24F));
        rootLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

        titleLabel.Dock = DockStyle.Fill;
        titleLabel.Font = new Font(titleLabel.Font, FontStyle.Bold);
        titleLabel.TextAlign = ContentAlignment.MiddleLeft;

        websiteIdLabel.Dock = DockStyle.Fill;
        urlLinkLabel.Dock = DockStyle.Fill;
        tagLabel.Dock = DockStyle.Fill;

        urlLinkLabel.LinkClicked += urlLinkLabel_LinkClicked;

        detailTextBox.Dock = DockStyle.Fill;
        detailTextBox.Multiline = true;
        detailTextBox.ReadOnly = true;
        detailTextBox.ScrollBars = ScrollBars.Both;
        detailTextBox.WordWrap = false;

        rootLayoutPanel.Controls.Add(titleLabel, 0, 0);
        rootLayoutPanel.Controls.Add(websiteIdLabel, 0, 1);
        rootLayoutPanel.Controls.Add(urlLinkLabel, 0, 2);
        rootLayoutPanel.Controls.Add(tagLabel, 0, 3);
        rootLayoutPanel.Controls.Add(detailTextBox, 0, 4);

        Controls.Add(rootLayoutPanel);
        Dock = DockStyle.Fill;

        ShowEmpty();
    }

    public void ShowWebsite(HostWebsiteTreeNodePayload payload)
    {
        ArgumentNullException.ThrowIfNull(payload);

        titleLabel.Text = payload.DisplayText;
        websiteIdLabel.Text = $"WebsiteId: {payload.WebsiteId}";
        urlLinkLabel.Text = payload.Url;
        tagLabel.Text = $"Tag: {payload.TagName}";
        detailTextBox.Text =
            $"Name: {payload.DisplayText}{Environment.NewLine}" +
            $"WebsiteId: {payload.WebsiteId}{Environment.NewLine}" +
            $"Url: {payload.Url}{Environment.NewLine}" +
            $"Tag: {payload.TagName}{Environment.NewLine}" +
            $"Active: {payload.IsActive}";
    }

    public void ShowGroup(IHostWebsiteTreeNodePayload payload)
    {
        ArgumentNullException.ThrowIfNull(payload);

        titleLabel.Text = payload.DisplayText;
        websiteIdLabel.Text = $"Node type: {payload.NodeType}";
        urlLinkLabel.Text = string.Empty;
        tagLabel.Text = $"NodeId: {payload.NodeId}";
        detailTextBox.Text =
            $"Group: {payload.DisplayText}{Environment.NewLine}" +
            $"NodeId: {payload.NodeId}{Environment.NewLine}" +
            $"NodeType: {payload.NodeType}";
    }

    public void ShowEmpty()
    {
        titleLabel.Text = "No website selected";
        websiteIdLabel.Text = string.Empty;
        urlLinkLabel.Text = string.Empty;
        tagLabel.Text = string.Empty;
        detailTextBox.Text = "Select a website node to load details.";
    }

    private void urlLinkLabel_LinkClicked(object? sender, LinkLabelLinkClickedEventArgs e)
    {
        string url = urlLinkLabel.Text.Trim();

        if (!Uri.TryCreate(url, UriKind.Absolute, out Uri? uri))
        {
            return;
        }

        try
        {
            System.Diagnostics.Process.Start(
                new System.Diagnostics.ProcessStartInfo
                {
                    FileName = uri.ToString(),
                    UseShellExecute = true
                });
        }
        catch
        {
        }
    }
}
