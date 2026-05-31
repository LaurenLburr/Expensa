using Codex.CommandEngine.Core;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Websites;

public sealed partial class WebsitesDatabasePanelForm : Form
{
    private readonly HostWebsiteRuntimeModuleInvoker _invoker;
    private readonly HostWebsiteDatabasePathService _databasePathService;
    private readonly HostWebsiteDatabaseCopyService _databaseCopyService;
    private readonly HostWebsiteDatabaseCopyHistory _copyHistory = new();
    private readonly HostWebsiteRuntimeDatabaseSelectionService _runtimeDatabaseSelectionService = new();
    private readonly ControlDisplayNameOverlayService _displayNameOverlayService = new();
    private bool _displayControlNames;

    public WebsitesDatabasePanelForm()
        : this(
            new HostWebsiteRuntimeModuleInvoker(),
            new HostWebsiteDatabasePathService(),
            new HostWebsiteDatabaseCopyService())
    {
    }

    public WebsitesDatabasePanelForm(
        HostWebsiteRuntimeModuleInvoker invoker,
        HostWebsiteDatabasePathService databasePathService,
        HostWebsiteDatabaseCopyService databaseCopyService)
    {
        ArgumentNullException.ThrowIfNull(invoker);
        ArgumentNullException.ThrowIfNull(databasePathService);
        ArgumentNullException.ThrowIfNull(databaseCopyService);

        _invoker = invoker;
        _databasePathService = databasePathService;
        _databaseCopyService = databaseCopyService;

        InitializeComponent();
        LoadDatabaseLocation();
        RefreshCopyHistoryList();
    }

    private void LoadDatabaseLocation()
    {
        HostWebsiteDatabaseLocation location =
            _runtimeDatabaseSelectionService.GetActiveRuntimeDatabaseLocation();

        databaseNameLinkLabel.Text = location.DatabaseName;
        databasePathTextBox.Text = location.DatabasePath;
    }

    private void toggleControlNamesButton_Click(
        object? sender,
        EventArgs e)
    {
        _displayControlNames = !_displayControlNames;

        if (_displayControlNames)
        {
            _displayNameOverlayService.ShowControlNames(this);
            toggleControlNamesButton.Text = "×";
            toggleControlNamesButton.ToolTipText = "Hide control names";
            return;
        }

        _displayNameOverlayService.HideControlNames(this);
        toggleControlNamesButton.Text = "?";
        toggleControlNamesButton.ToolTipText = "Show control names";
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

    private void databaseNameLinkLabel_LinkClicked(
        object? sender,
        LinkLabelLinkClickedEventArgs e)
    {
        HostWebsiteDatabaseLocation location =
            _databasePathService.GetRuntimeDatabaseLocation();

        Directory.CreateDirectory(location.DatabaseFolder);

        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
        {
            FileName = location.DatabaseFolder,
            UseShellExecute = true
        });
    }

    private void copyFromDevTemplateLinkLabel_LinkClicked(
        object? sender,
        LinkLabelLinkClickedEventArgs e)
    {
        CopyDatabase(
            "dev template",
            _databaseCopyService.CopyNewFromDevTemplate);
    }

    private void copyFromSandboxLinkLabel_LinkClicked(
        object? sender,
        LinkLabelLinkClickedEventArgs e)
    {
        CopyDatabase(
            "Sandbox database",
            _databaseCopyService.CopyNewFromSandbox);
    }

    private void CopyDatabase(
        string sourceDescription,
        Func<string> copyAction)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sourceDescription);
        ArgumentNullException.ThrowIfNull(copyAction);

        try
        {
            string targetPath =
                copyAction();

            _copyHistory.Add(
                new HostWebsiteDatabaseCopyRecord
                {
                    SourceLabel = sourceDescription,
                    DatabasePath = targetPath
                });

            RefreshCopyHistoryList();

            copiedDatabasePathTextBox.Text =
                targetPath;

            MessageBox.Show(
                this,
                $"Copied new database from {sourceDescription}.{Environment.NewLine}{Environment.NewLine}{targetPath}",
                "Websites Database",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
        catch (Exception exception)
        {
            MessageBox.Show(
                this,
                exception.Message,
                "Websites Database Copy Failed",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }

    private void RefreshCopyHistoryList()
    {
        copiedDatabasesListView.BeginUpdate();

        try
        {
            copiedDatabasesListView.Items.Clear();

            foreach (HostWebsiteDatabaseCopyRecord record in _copyHistory.Records)
            {
                ListViewItem item = new(record.DatabaseName)
                {
                    Tag = record
                };

                item.SubItems.Add(record.SourceLabel);
                item.SubItems.Add(record.CreatedLocal.ToString("g"));
                item.SubItems.Add(record.DatabasePath);

                copiedDatabasesListView.Items.Add(item);
            }
        }
        finally
        {
            copiedDatabasesListView.EndUpdate();
        }
    }


    private void activateSelectedDatabaseButton_Click(
        object? sender,
        EventArgs e)
    {
        ActivateSelectedDatabase();
    }

    private void ActivateSelectedDatabase()
    {
        if (copiedDatabasesListView.SelectedItems.Count == 0)
        {
            MessageBox.Show(
                this,
                "Select a copied database first.",
                "Activate Runtime Database",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            return;
        }

        if (copiedDatabasesListView.SelectedItems[0].Tag is not HostWebsiteDatabaseCopyRecord record)
        {
            return;
        }

        _runtimeDatabaseSelectionService.SetActiveRuntimeDatabase(
            record.DatabasePath);

        LoadDatabaseLocation();

        MessageBox.Show(
            this,
            $"Active runtime database set to:{Environment.NewLine}{Environment.NewLine}{record.DatabasePath}",
            "Runtime Database Updated",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
    }

    private void copiedDatabasesListView_SelectedIndexChanged(
        object? sender,
        EventArgs e)
    {
        if (copiedDatabasesListView.SelectedItems.Count == 0)
        {
            return;
        }

        if (copiedDatabasesListView.SelectedItems[0].Tag is not HostWebsiteDatabaseCopyRecord record)
        {
            return;
        }

        copiedDatabasePathTextBox.Text =
            record.DatabasePath;
    }

    private void openCopiedDatabaseFolderLinkLabel_LinkClicked(
        object? sender,
        LinkLabelLinkClickedEventArgs e)
    {
        string databasePath =
            copiedDatabasePathTextBox.Text.Trim();

        if (string.IsNullOrWhiteSpace(databasePath))
        {
            return;
        }

        string? folder =
            Path.GetDirectoryName(databasePath);

        if (string.IsNullOrWhiteSpace(folder))
        {
            return;
        }

        Directory.CreateDirectory(folder);

        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
        {
            FileName = folder,
            UseShellExecute = true
        });
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
