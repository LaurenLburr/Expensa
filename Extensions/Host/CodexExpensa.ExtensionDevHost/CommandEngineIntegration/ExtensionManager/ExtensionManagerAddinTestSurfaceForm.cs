namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.ExtensionManager;

public sealed partial class ExtensionManagerAddinTestSurfaceForm : Form
{
    private readonly IExtensionManagerAddinTestCatalog _catalog;

    public ExtensionManagerAddinTestSurfaceForm()
        : this(new ExtensionManagerAddinTestCatalog())
    {
    }

    public ExtensionManagerAddinTestSurfaceForm(IExtensionManagerAddinTestCatalog catalog)
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

        ExtensionManagerAddinTestTreeBuilder.Populate(addinTreeView, addins);

        statusLabel.Text =
            $"Loaded {addins.Count} add-in test node(s).";
    }

    private void addinTreeView_AfterSelect(object? sender, TreeViewEventArgs e)
    {
        detailsTextBox.Text = FormatSelectedNode(e.Node);
    }

    private void addinTreeView_DoubleClick(object? sender, EventArgs e)
    {
        OpenSelectedTestForm();
    }

    private void openSelectedButton_Click(object? sender, EventArgs e)
    {
        OpenSelectedTestForm();
    }

    private void refreshButton_Click(object? sender, EventArgs e)
    {
        LoadAddins();
    }

    private void closeButton_Click(object? sender, EventArgs e)
    {
        Close();
    }

    private void OpenSelectedTestForm()
    {
        if (addinTreeView.SelectedNode?.Tag is not ExtensionManagerAddinTestAction action)
        {
            MessageBox.Show(
                this,
                "Select an add-in test action node first.",
                "Extension Manager Test Surface",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            return;
        }

        if (!string.Equals(
                action.ActionKey,
                ExtensionManagerAddinTestTreeBuilder.LoadTestFormActionKey,
                StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        if (string.Equals(action.Addin.AddinId, "websites", StringComparison.OrdinalIgnoreCase))
        {
            using ExtensionTreeLoadTestForm form = new();
            form.ShowDialog(this);
            return;
        }

        MessageBox.Show(
            this,
            $"No test form is registered for add-in '{action.Addin.DisplayName}'.",
            "Extension Manager Test Surface",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
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
                $"TestForm: {addin.TestFormName}";
        }

        if (node.Tag is ExtensionManagerAddinTestAction action)
        {
            return
                $"Action: {action.ActionKey}{Environment.NewLine}" +
                $"Add-in: {action.Addin.DisplayName}{Environment.NewLine}" +
                $"AddinId: {action.Addin.AddinId}{Environment.NewLine}" +
                $"TestForm: {action.Addin.TestFormName}";
        }

        return node.Text;
    }
}
