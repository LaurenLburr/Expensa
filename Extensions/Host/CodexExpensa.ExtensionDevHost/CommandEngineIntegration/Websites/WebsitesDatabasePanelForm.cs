using Codex.CommandEngine.Core;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Websites;

public sealed partial class WebsitesDatabasePanelForm : Form
{
    private readonly HostWebsiteRuntimeModuleInvoker _invoker;

    public WebsitesDatabasePanelForm()
        : this(new HostWebsiteRuntimeModuleInvoker())
    {
    }

    public WebsitesDatabasePanelForm(
        HostWebsiteRuntimeModuleInvoker invoker)
    {
        ArgumentNullException.ThrowIfNull(invoker);

        _invoker = invoker;

        InitializeComponent();
    }

    private async void refreshButton_Click(object? sender, EventArgs e)
    {
        await RefreshWebsitesAsync().ConfigureAwait(true);
    }

    private async Task RefreshWebsitesAsync()
    {
        refreshButton.Enabled = false;
        statusLabel.Text = "Loading Websites database view...";
        detailsTextBox.Clear();

        try
        {
            CommandExecutionResult executionResult =
                await _invoker.ExecuteAsync(
                    new HostWebsiteRuntimeLoadRequest
                    {
                        SearchText = searchTextBox.Text.Trim(),
                        IncludeDisabled = includeDisabledCheckBox.Checked,
                        MaximumRows = (int)maximumRowsNumericUpDown.Value
                    }).ConfigureAwait(true);

            HostWebsiteLoadResult loadResult =
                HostWebsiteLoadExecutionResultAdapter.FromExecutionResult(executionResult);

            IReadOnlyList<HostWebsiteFlatRow> rows =
                HostWebsiteFlatRowBuilder.BuildRows(loadResult);

            PopulateListView(rows);

            statusLabel.Text =
                $"{executionResult.Status}: {rows.Count} website row(s). {loadResult.Message}";

            detailsTextBox.Text =
                executionResult.OutputJson;
        }
        catch (Exception exception)
        {
            statusLabel.Text = "Failed.";
            detailsTextBox.Text = exception.ToString();

            MessageBox.Show(
                this,
                exception.Message,
                "Websites Database",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
        finally
        {
            refreshButton.Enabled = true;
        }
    }

    private void PopulateListView(IReadOnlyList<HostWebsiteFlatRow> rows)
    {
        websitesListView.BeginUpdate();

        try
        {
            websitesListView.Items.Clear();

            foreach (HostWebsiteFlatRow row in rows)
            {
                ListViewItem item = new(row.DisplayText)
                {
                    Tag = row
                };

                item.SubItems.Add(row.Category);
                item.SubItems.Add(row.Url);
                item.SubItems.Add(row.IsEnabled ? "Yes" : "No");
                item.SubItems.Add(row.NodeId);

                websitesListView.Items.Add(item);
            }
        }
        finally
        {
            websitesListView.EndUpdate();
        }
    }

    private void websitesListView_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (websitesListView.SelectedItems.Count == 0)
        {
            return;
        }

        if (websitesListView.SelectedItems[0].Tag is not HostWebsiteFlatRow row)
        {
            return;
        }

        detailsTextBox.Text =
            $"NodeId: {row.NodeId}{Environment.NewLine}" +
            $"DisplayText: {row.DisplayText}{Environment.NewLine}" +
            $"Category: {row.Category}{Environment.NewLine}" +
            $"Url: {row.Url}{Environment.NewLine}" +
            $"Enabled: {row.IsEnabled}";
    }

    private void websitesListView_DoubleClick(object? sender, EventArgs e)
    {
        if (websitesListView.SelectedItems.Count == 0)
        {
            return;
        }

        if (websitesListView.SelectedItems[0].Tag is not HostWebsiteFlatRow row)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(row.Url))
        {
            return;
        }

        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
        {
            FileName = row.Url,
            UseShellExecute = true
        });
    }

    private void closeButton_Click(object? sender, EventArgs e)
    {
        Close();
    }
}
