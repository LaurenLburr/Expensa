namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Websites;

public sealed partial class WebsitesRuntimeExplorerForm : Form
{
    private readonly IHostWebsiteTreeLoader _treeLoader;

    public WebsitesRuntimeExplorerForm()
        : this(new HostWebsiteTreeLoader())
    {
    }

    public WebsitesRuntimeExplorerForm(
        IHostWebsiteTreeLoader treeLoader)
    {
        ArgumentNullException.ThrowIfNull(treeLoader);

        _treeLoader = treeLoader;

        InitializeComponent();
    }

    private async void loadButton_Click(
        object? sender,
        EventArgs e)
    {
        await LoadWebsitesAsync().ConfigureAwait(true);
    }

    private async Task LoadWebsitesAsync()
    {
        loadButton.Enabled = false;
        statusLabel.Text = "Loading websites...";
        detailsTextBox.Clear();

        try
        {
            HostWebsiteTreeLoadResult result =
                await _treeLoader.LoadIntoTreeViewAsync(
                    websitesTreeView,
                    new HostWebsiteTreeLoadOptions
                    {
                        SearchText = searchTextBox.Text.Trim(),
                        IncludeDisabled = includeDisabledCheckBox.Checked,
                        MaximumRows = (int)maximumRowsNumericUpDown.Value,
                        ExpandAll = true
                    }).ConfigureAwait(true);

            statusLabel.Text =
                $"{result.ExecutionResult.Status}: {result.WebsiteResult.Message}";

            detailsTextBox.Text =
                result.ExecutionResult.OutputJson;
        }
        catch (Exception exception)
        {
            statusLabel.Text = "Failed.";
            detailsTextBox.Text = exception.ToString();

            MessageBox.Show(
                this,
                exception.Message,
                "Load Websites",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
        finally
        {
            loadButton.Enabled = true;
        }
    }

    private void websitesTreeView_AfterSelect(
        object? sender,
        TreeViewEventArgs e)
    {
        string details =
            HostWebsiteTreeSelectionFormatter.FormatSelectedNode(websitesTreeView);

        if (!string.IsNullOrWhiteSpace(details))
        {
            detailsTextBox.Text = details;
        }
    }

    private void websitesTreeView_DoubleClick(
        object? sender,
        EventArgs e)
    {
        string url =
            HostWebsiteTreeViewRenderer.GetSelectedUrl(websitesTreeView);

        if (string.IsNullOrWhiteSpace(url))
        {
            return;
        }

        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
        {
            FileName = url,
            UseShellExecute = true
        });
    }

    private void closeButton_Click(
        object? sender,
        EventArgs e)
    {
        Close();
    }
}
