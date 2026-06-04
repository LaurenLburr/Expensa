using CodexExpensa.ExtensionDevHost.CommandEngineIntegration.CommonTree;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.ExpensaLoader;

public sealed partial class ExpensaAddinLoaderTestForm : Form
{
    private readonly ExpensaAddinTreeViewLoaderService treeLoader = new();

    private bool hasAutoLoaded;

    public ExpensaAddinLoaderTestForm()
    {
        InitializeComponent();
        LoadDefaults();

        AcceptButton = loadAllButton;
    }

    protected override async void OnShown(EventArgs e)
    {
        base.OnShown(e);

        if (hasAutoLoaded)
        {
            return;
        }

        hasAutoLoaded = true;

        await LoadAllAsync().ConfigureAwait(true);
    }

    private void LoadDefaults()
    {
        addinComboBox.SelectedIndex = 0;

        databasePathTextBox.Text =
            Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "CodexExpensa",
                "db",
                "codexexpensa.db");
    }

    private void browseDatabaseButton_Click(object? sender, EventArgs e)
    {
        using OpenFileDialog dialog =
            new()
            {
                Title = "Select Expensa database",
                Filter = "SQLite database (*.db)|*.db|All files (*.*)|*.*",
                FileName = databasePathTextBox.Text
            };

        if (dialog.ShowDialog(this) == DialogResult.OK)
        {
            databasePathTextBox.Text = dialog.FileName;
        }
    }

    private async void loadSelectedButton_Click(object? sender, EventArgs e)
    {
        await LoadSelectedAsync().ConfigureAwait(true);
    }

    private async void loadAllButton_Click(object? sender, EventArgs e)
    {
        await LoadAllAsync().ConfigureAwait(true);
    }

    private async Task LoadSelectedAsync()
    {
        SetLoadingState(
            isLoading: true,
            "Loading selected add-in through Expensa-style aggregate tree loader...");

        try
        {
            IReadOnlyList<ExpensaAddinLoaderResult> results =
                await treeLoader.LoadIntoTreeViewAsync(
                    treeView,
                    new ExpensaAddinLoaderRequest
                    {
                        Kind = GetSelectedKind(),
                        SearchText = searchTextBox.Text.Trim(),
                        IncludeInactive = includeInactiveCheckBox.Checked,
                        MaximumRows = (int)maximumRowsNumericUpDown.Value,
                        DatabasePath = databasePathTextBox.Text.Trim()
                    },
                    expandAllCheckBox.Checked).ConfigureAwait(true);

            ShowResultStatus(results);
            SelectFirstMeaningfulNode();
        }
        catch (Exception exception)
        {
            ShowFailure(exception);
        }
        finally
        {
            SetLoadingState(isLoading: false);
        }
    }

    private async Task LoadAllAsync()
    {
        SetLoadingState(
            isLoading: true,
            "Loading Websites and Budgets under one Expensa Add-ins root...");

        try
        {
            IReadOnlyList<ExpensaAddinLoaderResult> results =
                await treeLoader.LoadAllIntoTreeViewAsync(
                    treeView,
                    new ExpensaAddinLoaderRequestTemplate
                    {
                        DatabasePath = databasePathTextBox.Text.Trim(),
                        SearchText = searchTextBox.Text.Trim(),
                        IncludeInactive = includeInactiveCheckBox.Checked,
                        MaximumRows = (int)maximumRowsNumericUpDown.Value
                    },
                    expandAllCheckBox.Checked).ConfigureAwait(true);

            ShowResultStatus(results);
            SelectFirstMeaningfulNode();
        }
        catch (Exception exception)
        {
            ShowFailure(exception);
        }
        finally
        {
            SetLoadingState(isLoading: false);
        }
    }

    private void SelectFirstMeaningfulNode()
    {
        if (treeView.Nodes.Count == 0)
        {
            return;
        }

        TreeNode rootNode =
            treeView.Nodes[0];

        rootNode.Expand();

        if (rootNode.Nodes.Count > 0)
        {
            treeView.SelectedNode = rootNode.Nodes[0];
            treeView.SelectedNode.EnsureVisible();
            return;
        }

        treeView.SelectedNode = rootNode;
    }

    private void ShowResultStatus(IReadOnlyList<ExpensaAddinLoaderResult> results)
    {
        string loadedText =
            string.Join(
                " | ",
                results.Select(static result => $"{result.AddinName}: {result.Status} - {result.Message}"));

        statusLabel.Text =
            $"Loaded {results.Count} add-in(s): {loadedText}";
    }

    private void ShowFailure(Exception exception)
    {
        statusLabel.Text = "Failed.";
        detailsTextBox.Text = exception.ToString();

        MessageBox.Show(
            this,
            exception.Message,
            "Expensa Add-in Loader Tree Test",
            MessageBoxButtons.OK,
            MessageBoxIcon.Error);
    }

    private void SetLoadingState(bool isLoading, string? message = null)
    {
        loadSelectedButton.Enabled = !isLoading;
        loadAllButton.Enabled = !isLoading;
        browseDatabaseButton.Enabled = !isLoading;

        if (!string.IsNullOrWhiteSpace(message))
        {
            statusLabel.Text = message;
        }
    }

    private ExpensaAddinLoaderKind GetSelectedKind()
    {
        return addinComboBox.SelectedItem?.ToString() switch
        {
            "Budgets" => ExpensaAddinLoaderKind.Budgets,
            _ => ExpensaAddinLoaderKind.Websites
        };
    }

    private void treeView_AfterSelect(object? sender, TreeViewEventArgs e)
    {
        detailsTextBox.Text =
            AddinTreeSelectionFormatter.FormatSelectedNode(treeView);
    }
}
