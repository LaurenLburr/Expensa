using CodexExpensa.ExtensionDevHost.CommandEngineIntegration.ExpensaLoader;
using System.Text.Json;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Payees;

public sealed partial class PayeesTreeLoadVerificationFormCommonTree : Form
{
    private readonly ExpensaAddinTreeViewLoaderService loaderService = new();

    private bool hasAutoLoaded;

    public PayeesTreeLoadVerificationFormCommonTree()
    {
        InitializeComponent();

        databasePathTextBox.Text = GetDefaultExpensaDatabasePath();
        maximumRowsNumericUpDown.Value = 500;
        expandAllCheckBox.Checked = true;
    }

    protected override async void OnShown(EventArgs e)
    {
        base.OnShown(e);

        if (hasAutoLoaded)
        {
            return;
        }

        hasAutoLoaded = true;

        await LoadPayeesTreeAsync().ConfigureAwait(true);
    }

    private async void loadButton_Click(object? sender, EventArgs e)
    {
        await LoadPayeesTreeAsync().ConfigureAwait(true);
    }

    private void browseButton_Click(object? sender, EventArgs e)
    {
        using OpenFileDialog dialog =
            new()
            {
                Title = "Select Expensa database",
                Filter = "SQLite database (*.db)|*.db|All files (*.*)|*.*",
                FileName = databasePathTextBox.Text,
                CheckFileExists = true
            };

        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        databasePathTextBox.Text =
            dialog.FileName;
    }

    private void payeesTreeView_AfterSelect(object? sender, TreeViewEventArgs e)
    {
        if (e.Node is null)
        {
            return;
        }

        selectedNodeTextBox.Text =
            FormatSelectedNode(e.Node);
    }

    private async Task LoadPayeesTreeAsync()
    {
        SetLoadingState(isLoading: true, "Loading Payees add-in tree...");

        try
        {
            string databasePath =
                databasePathTextBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(databasePath))
            {
                throw new InvalidOperationException("Database path is required.");
            }

            if (!File.Exists(databasePath))
            {
                throw new FileNotFoundException(
                    "Database file was not found.",
                    databasePath);
            }

            IReadOnlyList<ExpensaAddinLoaderResult> results =
                await loaderService.LoadIntoTreeViewAsync(
                    payeesTreeView,
                    new ExpensaAddinLoaderRequest
                    {
                        Kind = ExpensaAddinLoaderKind.Payees,
                        DatabasePath = databasePath,
                        SearchText = searchTextBox.Text.Trim(),
                        IncludeInactive = includeInactiveCheckBox.Checked,
                        MaximumRows = (int)maximumRowsNumericUpDown.Value
                    },
                    expandAllCheckBox.Checked).ConfigureAwait(true);

            ShowResultSummary(results);
            SelectFirstNode();
        }
        catch (Exception exception)
        {
            payeesTreeView.Nodes.Clear();

            resultsTextBox.Text =
                exception.ToString();

            statusLabel.Text =
                "Payees tree load failed.";

            MessageBox.Show(
                this,
                exception.Message,
                "Payees Tree Verification",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
        finally
        {
            SetLoadingState(isLoading: false);
        }
    }

    private void ShowResultSummary(IReadOnlyList<ExpensaAddinLoaderResult> results)
    {
        if (results.Count == 0)
        {
            resultsTextBox.Text =
                "No loader results were returned.";

            statusLabel.Text =
                "No results.";

            return;
        }

        ExpensaAddinLoaderResult result =
            results[0];

        resultsTextBox.Text =
            $"Add-in: {result.AddinName}{Environment.NewLine}" +
            $"Status: {result.Status}{Environment.NewLine}" +
            $"Message: {result.Message}{Environment.NewLine}" +
            $"Tree root count: {payeesTreeView.Nodes.Count}{Environment.NewLine}{Environment.NewLine}" +
            "Output JSON:" + Environment.NewLine +
            FormatJson(result.OutputJson);

        statusLabel.Text =
            $"{result.AddinName}: {result.Status} - {result.Message}";
    }

    private void SelectFirstNode()
    {
        if (payeesTreeView.Nodes.Count == 0)
        {
            selectedNodeTextBox.Clear();
            return;
        }

        payeesTreeView.SelectedNode =
            payeesTreeView.Nodes[0];

        payeesTreeView.SelectedNode.EnsureVisible();

        selectedNodeTextBox.Text =
            FormatSelectedNode(payeesTreeView.SelectedNode);
    }

    private void SetLoadingState(bool isLoading, string? message = null)
    {
        loadButton.Enabled = !isLoading;
        browseButton.Enabled = !isLoading;

        if (!string.IsNullOrWhiteSpace(message))
        {
            statusLabel.Text = message;
        }
    }

    private static string FormatSelectedNode(TreeNode node)
    {
        return
            $"Text: {node.Text}{Environment.NewLine}" +
            $"Name: {node.Name}{Environment.NewLine}" +
            $"Tag type: {node.Tag?.GetType().FullName ?? "(null)"}{Environment.NewLine}" +
            $"Tag:{Environment.NewLine}{node.Tag}";
    }

    private static string FormatJson(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return string.Empty;
        }

        try
        {
            using JsonDocument document =
                JsonDocument.Parse(json);

            return JsonSerializer.Serialize(
                document.RootElement,
                new JsonSerializerOptions
                {
                    WriteIndented = true
                });
        }
        catch
        {
            return json;
        }
    }

    private static string GetDefaultExpensaDatabasePath()
    {
        return Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "CodexExpensa",
            "db",
            "codexexpensa.db");
    }
}
