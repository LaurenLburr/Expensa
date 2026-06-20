namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.ExtensionManager;

public sealed partial class ExtensionManagerAddinTestSurfaceForm : Form
{
    private readonly IExtensionManagerAddinTestCatalog _catalog;
    private readonly AddinProjectUiSurfaceResolver _surfaceResolver = new();
    private readonly AddinFolderStructureInitializer _folderStructureInitializer = new();

    public ExtensionManagerAddinTestSurfaceForm()
        : this(new ExtensionManagerAddinTestCatalog())
    {
    }

    public ExtensionManagerAddinTestSurfaceForm(
        IExtensionManagerAddinTestCatalog catalog)
    {
        ArgumentNullException.ThrowIfNull(catalog);

        _catalog = catalog;

        InitializeComponent();
        LoadAddins();
    }

    private void LoadAddins()
    {
        IReadOnlyList<ExtensionManagerAddinTestNode> addins =
            _catalog.GetAddins()
                .OrderBy(static item => item.SortOrder)
                .ToList();

        _folderStructureInitializer.EnsureCreated(
            addins.Select(static addin => addin.AddinId));

        ExtensionManagerAddinTestTreeBuilder.Populate(addinTreeView, addins);

        statusLabel.Text =
            $"Loaded {addins.Count} add-in test node(s).";
    }

    private void addinTreeView_AfterSelect(object? sender, TreeViewEventArgs e)
    {
        detailsTextBox.Text =
            FormatSelectedNode(e.Node);
    }

    private void addinTreeView_DoubleClick(object? sender, EventArgs e)
    {
        OpenSelectedNode();
    }

    private void openSelectedButton_Click(object? sender, EventArgs e)
    {
        OpenSelectedNode();
    }

    private void refreshButton_Click(object? sender, EventArgs e)
    {
        LoadAddins();
    }

    private void closeButton_Click(object? sender, EventArgs e)
    {
        Close();
    }

    private void OpenSelectedNode()
    {
        if (addinTreeView.SelectedNode?.Tag is not ExtensionManagerAddinTestAction action)
        {
            MessageBox.Show(
                this,
                "Select an add-in child node first.",
                "Extension Manager Test Surface",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            return;
        }

        Form? form;

        bool created =
            action.ActionKind == ExtensionManagerAddinTestActionKind.Database
                ? _surfaceResolver.TryCreateDatabaseForm(action.Addin.DisplayName, string.Empty, out form)
                : _surfaceResolver.TryCreateTestForm(action.Addin.DisplayName, string.Empty, out form);

        if (!created || form is null)
        {
            MessageBox.Show(
                this,
                $"No {action.ActionKind} form is registered for add-in '{action.Addin.DisplayName}'.",
                "Extension Manager Test Surface",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            return;
        }

        using (form)
        {
            form.StartPosition = FormStartPosition.CenterParent;
            form.ShowDialog(this);
        }
    }

    private static string FormatSelectedNode(TreeNode? node)
    {
        if (node is null)
        {
            return string.Empty;
        }

        if (node.Tag is ExtensionManagerAddinTestNode addin)
        {
            return
                $"Add-in: {addin.DisplayName}{Environment.NewLine}" +
                $"AddinId: {addin.AddinId}{Environment.NewLine}" +
                $"SortOrder: {addin.SortOrder}{Environment.NewLine}" +
                $"Database: {addin.DatabaseDisplayName}{Environment.NewLine}" +
                $"TestForm: {addin.TestFormName}";
        }

        if (node.Tag is ExtensionManagerAddinTestAction action)
        {
            return
                $"Action: {action.ActionKind}{Environment.NewLine}" +
                $"Add-in: {action.Addin.DisplayName}{Environment.NewLine}" +
                $"AddinId: {action.Addin.AddinId}{Environment.NewLine}" +
                $"SortOrder: {action.Addin.SortOrder}{Environment.NewLine}" +
                $"Database: {action.Addin.DatabaseDisplayName}{Environment.NewLine}" +
                $"TestForm: {action.Addin.TestFormName}";
        }

        return node.Text;
    }
}
