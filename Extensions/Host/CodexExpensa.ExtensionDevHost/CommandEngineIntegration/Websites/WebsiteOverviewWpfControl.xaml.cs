using System.Collections.ObjectModel;
using System.Windows.Input;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Websites;

public sealed partial class WebsiteOverviewWpfControl : System.Windows.Controls.UserControl
{
    private readonly ObservableCollection<WebsiteOverviewGridRow> _rows = [];

    public event EventHandler<string>? WebsiteRowDoubleClicked;

    public WebsiteOverviewWpfControl()
    {
        InitializeComponent();
        WebsitesDataGrid.ItemsSource = _rows;
    }

    public void SetWebsites(IEnumerable<HostWebsiteTreeNodePayload> payloads)
    {
        ArgumentNullException.ThrowIfNull(payloads);

        _rows.Clear();

        foreach (HostWebsiteTreeNodePayload payload in payloads)
        {
            _rows.Add(new WebsiteOverviewGridRow(
                payload.WebsiteId,
                payload.DisplayText,
                payload.Url,
                payload.TagName));
        }
    }

    private void WebsitesDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (WebsitesDataGrid.SelectedItem is not WebsiteOverviewGridRow row)
        {
            return;
        }

        WebsiteRowDoubleClicked?.Invoke(this, row.WebsiteId);
    }
}

public sealed record WebsiteOverviewGridRow(
    string WebsiteId,
    string Name,
    string Url,
    string TagName);
