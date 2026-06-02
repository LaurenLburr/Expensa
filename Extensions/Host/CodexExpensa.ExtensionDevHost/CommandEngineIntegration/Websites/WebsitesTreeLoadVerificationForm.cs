namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Websites;

public sealed partial class WebsitesTreeLoadVerificationForm : Form
{
    private readonly HostWebsiteTreeContributionLoader _loader;
    private bool _hasAutoLoaded;

    public WebsitesTreeLoadVerificationForm()
        : this(new HostWebsiteTreeContributionLoader())
    {
    }

    public WebsitesTreeLoadVerificationForm(HostWebsiteTreeContributionLoader loader)
    {
        ArgumentNullException.ThrowIfNull(loader);

        _loader = loader;

        InitializeComponent();
        EnsureWebsiteDetailsPanel();
    }

    protected override async void OnShown(EventArgs e)
    {
        base.OnShown(e);

        if (_hasAutoLoaded)
        {
            return;
        }

        _hasAutoLoaded = true;

        await LoadWebsitesTreeAsync().ConfigureAwait(true);
    }

    private async void loadButton_Click(object? sender, EventArgs e)
    {
        await LoadWebsitesTreeAsync().ConfigureAwait(true);
    }

    private async Task LoadWebsitesTreeAsync()
    {
        loadButton.Enabled = false;
        statusLabel.Text = "Loading Websites tree...";
        ShowSelectedTreeNodeDetails(null);

        try
        {
            HostWebsiteTreeLoadResult result =
                await _loader.LoadContributionAsync(
                    websitesTreeView,
                    new HostWebsiteTreeLoadOptions
                    {
                        SearchText = searchTextBox.Text.Trim(),
                        IncludeDisabled = includeDisabledCheckBox.Checked,
                        MaximumRows = (int)maximumRowsNumericUpDown.Value,
                        ExpandAll = expandAllCheckBox.Checked
                    }).ConfigureAwait(true);

            statusLabel.Text =
                $"{result.ExecutionResult.Status}: {result.RootNodeCount} root node(s), {result.WebsiteResult.TotalCount} website row(s). {result.WebsiteResult.Message}";

            detailsTextBox.Visible = true;
            detailsTextBox.Text = result.ExecutionResult.OutputJson;
        }
        catch (Exception exception)
        {
            statusLabel.Text = "Failed.";
            detailsTextBox.Visible = true;
            detailsTextBox.Text = exception.ToString();

            MessageBox.Show(
                this,
                exception.Message,
                "Websites Tree Load Verification",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
        finally
        {
            loadButton.Enabled = true;
        }
    }

    private void websitesTreeView_NodeMouseClick(object? sender, TreeNodeMouseClickEventArgs e)
    {
        if (e.Button == MouseButtons.Right)
        {
            websitesTreeView.SelectedNode = e.Node;
        }
    }

    private void sortAscendingToolStripMenuItem_Click(object? sender, EventArgs e)
    {
        SortTreeNodesFromContextMenu(ascending: true);
    }

    private void sortDescendingToolStripMenuItem_Click(object? sender, EventArgs e)
    {
        SortTreeNodesFromContextMenu(ascending: false);
    }

    private void websitesTreeView_AfterSelect(object? sender, TreeViewEventArgs e)
    {
        ShowSelectedTreeNodeDetails(e.Node);
    }

    private void closeButton_Click(object? sender, EventArgs e)
    {
        Close();
    }
}
