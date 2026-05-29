using Codex.CommandEngine.Core;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Websites;

public sealed partial class WebsitesRuntimeExplorerForm : Form
{
    private readonly HostWebsiteRuntimeModuleInvoker _invoker;

    public WebsitesRuntimeExplorerForm()
        : this(new HostWebsiteRuntimeModuleInvoker())
    {
    }

    public WebsitesRuntimeExplorerForm(
        HostWebsiteRuntimeModuleInvoker invoker)
    {
        ArgumentNullException.ThrowIfNull(invoker);

        _invoker = invoker;

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
            CommandExecutionResult result =
                await _invoker.ExecuteAsync(
                    new HostWebsiteRuntimeLoadRequest
                    {
                        SearchText = searchTextBox.Text.Trim(),
                        IncludeDisabled = includeDisabledCheckBox.Checked,
                        MaximumRows = (int)maximumRowsNumericUpDown.Value
                    }).ConfigureAwait(true);

            HostWebsiteLoadExecutionResultAdapter.RenderExecutionResult(
                websitesTreeView,
                result);

            HostWebsiteLoadResult loadResult =
                HostWebsiteLoadExecutionResultAdapter.FromExecutionResult(result);

            statusLabel.Text =
                $"{result.Status}: {loadResult.Message}";

            detailsTextBox.Text =
                result.OutputJson;

            websitesTreeView.ExpandAll();
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
        HostWebsiteTreeNode? node =
            HostWebsiteTreeViewRenderer.GetSelectedWebsiteNode(websitesTreeView);

        if (node is null)
        {
            return;
        }

        detailsTextBox.Text =
            $"NodeId: {node.NodeId}{Environment.NewLine}" +
            $"DisplayText: {node.DisplayText}{Environment.NewLine}" +
            $"Category: {node.Category}{Environment.NewLine}" +
            $"Url: {node.Url}{Environment.NewLine}" +
            $"Enabled: {node.IsEnabled}{Environment.NewLine}" +
            $"Children: {node.Children.Count}";
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
