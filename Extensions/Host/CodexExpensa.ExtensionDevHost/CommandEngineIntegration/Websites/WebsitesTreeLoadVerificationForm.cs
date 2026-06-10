using CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Templates;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Websites;

public sealed partial class WebsitesTreeLoadVerificationForm : TreeTestTemplate
{
    private readonly HostWebsiteTreeContributionLoader _loader;
    private bool _hasAutoLoaded;

    private TreeView websitesTreeView => TestTreeView;

    private TextBox detailsTextBox => NotesTextBox;

    private SplitContainer splitContainer => ContentSplitContainer;

    public WebsitesTreeLoadVerificationForm()
        : this(new HostWebsiteTreeContributionLoader())
    {
    }

    public WebsitesTreeLoadVerificationForm(HostWebsiteTreeContributionLoader loader)
    {
        ArgumentNullException.ThrowIfNull(loader);

        _loader = loader;

        InitializeComponent();

        ConfigureTreeTestTemplate(
            "Websites Tree Load Verification",
            "Load the Websites tree to verify the add-in contribution that Extension Manager will display.");

        websitesTreeView.Name = "websitesTreeView";
        detailsTextBox.Name = "detailsTextBox";

        websitesTreeView.AfterSelect += websitesTreeView_AfterSelect;
        websitesTreeView.NodeMouseClick += websitesTreeView_NodeMouseClick;

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

    private async Task LoadWebsitesTreeAsync()
    {
        SetStatus("Loading Websites tree...");
        ShowSelectedTreeNodeDetails(null);

        try
        {
            HostWebsiteTreeLoadResult result =
                await _loader.LoadContributionAsync(
                    websitesTreeView,
                    new HostWebsiteTreeLoadOptions()).ConfigureAwait(true);

            SetStatus(
                $"{result.ExecutionResult.Status}: {result.RootNodeCount} root node(s), {result.WebsiteResult.TotalCount} website row(s). {result.WebsiteResult.Message}");
        }
        catch (Exception exception)
        {
            SetStatus("Failed.");

            detailsTextBox.Visible = true;
            detailsTextBox.Text = exception.ToString();

            MessageBox.Show(
                this,
                exception.Message,
                "Websites Tree Load Verification",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }

    private void SetStatus(string message)
    {
        detailsTextBox.Visible = true;
        detailsTextBox.Text = message;
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
}
