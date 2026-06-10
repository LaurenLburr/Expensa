using CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Templates;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.ExtensionManager;

public sealed partial class ExtensionTreeLoadTestForm : TreeTestTemplate
{
    private readonly ExtensionTreeLoadOrchestrator _orchestrator;
    private bool _hasAutoLoaded;

    private IReadOnlySet<string> _lastExpandedNodeNames =
        new HashSet<string>(StringComparer.OrdinalIgnoreCase);

    private TreeView extensionTreeView => TestTreeView;

    private TextBox detailsTextBox => NotesTextBox;

    public ExtensionTreeLoadTestForm()
        : this(new ExtensionTreeLoadOrchestrator(new ExtensionTreeNodeLoaderCatalog().GetLoaders()))
    {
    }

    public ExtensionTreeLoadTestForm(ExtensionTreeLoadOrchestrator orchestrator)
    {
        ArgumentNullException.ThrowIfNull(orchestrator);

        _orchestrator = orchestrator;

        InitializeComponent();

        ConfigureTreeTestTemplate(
            "Extension Tree Load Test",
            "Load the extension tree to verify the add-in tree nodes that Extension Manager will display.");

        extensionTreeView.Name = "extensionTreeView";
        detailsTextBox.Name = "detailsTextBox";

        extensionTreeView.AfterSelect += extensionTreeView_AfterSelect;
        extensionTreeView.AfterExpand += extensionTreeView_AfterExpand;
        extensionTreeView.AfterCollapse += extensionTreeView_AfterCollapse;
    }

    protected override async void OnShown(EventArgs e)
    {
        base.OnShown(e);

        if (_hasAutoLoaded)
        {
            return;
        }

        _hasAutoLoaded = true;

        await LoadTreeAsync().ConfigureAwait(true);
    }

    private async Task LoadTreeAsync()
    {
        detailsTextBox.Text = "Loading extension tree...";

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

            detailsTextBox.Text =
                ExtensionTreeLoadSummaryFormatter.Format(summary);
        }
        catch (Exception exception)
        {
            detailsTextBox.Text = exception.ToString();

            MessageBox.Show(
                this,
                exception.Message,
                "Extension Tree Load Test",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
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
}
