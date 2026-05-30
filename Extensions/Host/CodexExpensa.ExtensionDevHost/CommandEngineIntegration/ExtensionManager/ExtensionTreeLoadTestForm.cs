namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.ExtensionManager;

public sealed partial class ExtensionTreeLoadTestForm : Form
{
    private readonly ExtensionTreeLoadOrchestrator _orchestrator;

    private IReadOnlySet<string> _lastExpandedNodeNames =
        new HashSet<string>(StringComparer.OrdinalIgnoreCase);

    public ExtensionTreeLoadTestForm()
        : this(new ExtensionTreeLoadOrchestrator(new ExtensionTreeNodeLoaderCatalog().GetLoaders()))
    {
    }

    public ExtensionTreeLoadTestForm(ExtensionTreeLoadOrchestrator orchestrator)
    {
        ArgumentNullException.ThrowIfNull(orchestrator);

        _orchestrator = orchestrator;

        InitializeComponent();
    }

    private async void loadTreeButton_Click(object? sender, EventArgs e)
    {
        await LoadTreeAsync().ConfigureAwait(true);
    }

    private async Task LoadTreeAsync()
    {
        loadTreeButton.Enabled = false;
        statusLabel.Text = "Loading extension tree...";
        detailsTextBox.Clear();

        IReadOnlySet<string> expandedBeforeReload =
            TreeViewExpansionStateService.CaptureExpandedNodeNames(extensionTreeView);

        try
        {
            ExtensionTreeLoadSummary summary =
                await _orchestrator.LoadAsync(extensionTreeView).ConfigureAwait(true);

            IReadOnlySet<string> stateToRestore =
                expandedBeforeReload.Count > 0
                    ? expandedBeforeReload
                    : _lastExpandedNodeNames;

            TreeViewExpansionStateService.RestoreExpandedNodeNames(extensionTreeView, stateToRestore);

            _lastExpandedNodeNames =
                TreeViewExpansionStateService.CaptureExpandedNodeNames(extensionTreeView);

            statusLabel.Text = summary.ToDisplayText();
            detailsTextBox.Text = ExtensionTreeLoadSummaryFormatter.Format(summary);
        }
        catch (Exception exception)
        {
            statusLabel.Text = "Failed.";
            detailsTextBox.Text = exception.ToString();

            MessageBox.Show(
                this,
                exception.Message,
                "Extension Tree Load Test",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
        finally
        {
            loadTreeButton.Enabled = true;
        }
    }

    private void extensionTreeView_AfterExpand(object? sender, TreeViewEventArgs e)
    {
        _lastExpandedNodeNames =
            TreeViewExpansionStateService.CaptureExpandedNodeNames(extensionTreeView);
    }

    private void extensionTreeView_AfterCollapse(object? sender, TreeViewEventArgs e)
    {
        _lastExpandedNodeNames =
            TreeViewExpansionStateService.CaptureExpandedNodeNames(extensionTreeView);
    }

    private void extensionTreeView_AfterSelect(object? sender, TreeViewEventArgs e)
    {
        detailsTextBox.Text =
            e.Node?.Tag?.ToString() ?? e.Node?.Text ?? string.Empty;
    }

    private void closeButton_Click(object? sender, EventArgs e)
    {
        Close();
    }
}
