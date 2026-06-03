using System.Diagnostics;

namespace CodexExpensa.App.WinForms.UI.Websites;

public partial class WebsiteDetailsForm : Form
{
    private readonly IHostWebsiteTreeNodePayload _payload;

    public WebsiteDetailsForm(
        IHostWebsiteTreeNodePayload payload)
    {
        ArgumentNullException.ThrowIfNull(payload);

        _payload = payload;

        InitializeComponent();
        LoadPayload();
    }

    private void LoadPayload()
    {
        titleLabel.Text = _payload.DisplayText;
        nodeTypeValueLabel.Text = _payload.NodeType.ToString();
        nodeIdValueTextBox.Text = _payload.NodeId;
        websiteIdValueTextBox.Text = _payload.WebsiteId;
        categoryValueTextBox.Text = _payload.TagName;
        urlValueTextBox.Text = _payload.Url;
        enabledValueLabel.Text = _payload.IsActive ? "Yes" : "No";

        openWebsiteButton.Enabled =
            _payload.NodeType == HostWebsiteTreeNodeType.Website &&
            !string.IsNullOrWhiteSpace(_payload.Url);
    }

    private void openWebsiteButton_Click(
        object? sender,
        EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(_payload.Url))
        {
            return;
        }

        Process.Start(
            new ProcessStartInfo
            {
                FileName = _payload.Url,
                UseShellExecute = true
            });
    }
}
