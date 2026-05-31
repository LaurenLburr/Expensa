namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Websites;

public sealed partial class WebsitesTreeLoadVerificationForm : Form
{
    private readonly HostWebsiteTreeContributionLoader _loader;
    private bool _hasAutoLoaded;

    public WebsitesTreeLoadVerificationForm()
        : this(new HostWebsiteTreeContributionLoader())
    {
    }

    public WebsitesTreeLoadVerificationForm(
        HostWebsiteTreeContributionLoader loader)
    {
        ArgumentNullException.ThrowIfNull(loader);

        _loader = loader;

        InitializeComponent();
    }

    protected override async void OnShown(
        EventArgs e)
    {
        base.OnShown(e);

        if (_hasAutoLoaded)
        {
            return;
        }

        _hasAutoLoaded = true;

        await LoadWebsitesTreeAsync().ConfigureAwait(true);
    }

    private async void loadButton_Click(
        object? sender,
        EventArgs e)
    {
        await LoadWebsitesTreeAsync().ConfigureAwait(true);
    }

    private async Task LoadWebsitesTreeAsync()
    {
        loadButton.Enabled = false;
        statusLabel.Text = "Loading Websites tree...";
        detailsTextBox.Clear();

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
                $"{result.ExecutionResult.Status}: {result.RootNodeCount} root node(s), {result.WebsiteResult.TotalCount} website row(s).";

            detailsTextBox.Text =
                $"Message: {result.WebsiteResult.Message}{Environment.NewLine}" +
                $"Root nodes: {result.RootNodeCount}{Environment.NewLine}" +
                $"Website rows: {result.WebsiteResult.TotalCount}{Environment.NewLine}{Environment.NewLine}" +
                result.ExecutionResult.OutputJson;
        }
        catch (Exception exception)
        {
            statusLabel.Text = "Failed.";
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

    private void websitesTreeView_AfterSelect(
        object? sender,
        TreeViewEventArgs e)
    {
        detailsTextBox.Text =
            e.Node?.Tag?.ToString() ?? e.Node?.Text ?? string.Empty;
    }

    private void closeButton_Click(
        object? sender,
        EventArgs e)
    {
        Close();
    }
}
