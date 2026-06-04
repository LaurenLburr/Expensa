using Codex.CommandEngine.Core;
using CodexExpensa.ExtensionDevHost.CommandEngineIntegration.CommonTree;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Budgets;

public sealed class BudgetsDatabasePanelForm : Form
{
    private readonly HostBudgetRuntimeModuleInvoker invoker = new();
    private readonly HostBudgetDatabasePathService databasePathService = new();
    private readonly HostBudgetDatabaseCopyService databaseCopyService = new();
    private readonly HostBudgetDatabaseCopyHistory copyHistory = new();
    private readonly HostBudgetExpensaProdDatabaseCopyService expensaProdCopyService = new();

    private readonly LinkLabel databaseNameLinkLabel = new();
    private readonly TextBox databasePathTextBox = new();
    private readonly TextBox copiedDatabasePathTextBox = new();
    private readonly ListView copiedDatabasesListView = new();
    private readonly TextBox searchTextBox = new();
    private readonly CheckBox includeClosedCheckBox = new();
    private readonly NumericUpDown maximumRowsNumericUpDown = new();
    private readonly Button refreshButton = new();
    private readonly Label statusLabel = new();
    private readonly ListView budgetsListView = new();
    private readonly TextBox detailsTextBox = new();

    public BudgetsDatabasePanelForm()
    {
        Text = "Budgets Database";
        Width = 1150;
        Height = 720;
        StartPosition = FormStartPosition.CenterParent;

        BuildLayout();
        LoadDatabaseLocation();
        RefreshCopyHistoryList();
    }

    private void BuildLayout()
    {
        TableLayoutPanel root = new()
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 5,
            Padding = new Padding(8)
        };

        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 180F));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 36F));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 26F));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 42F));

        root.Controls.Add(BuildDatabaseGroup(), 0, 0);
        root.Controls.Add(BuildFilterPanel(), 0, 1);

        statusLabel.Dock = DockStyle.Fill;
        statusLabel.Text = "Ready.";
        statusLabel.TextAlign = ContentAlignment.MiddleLeft;
        root.Controls.Add(statusLabel, 0, 2);

        SplitContainer split = new()
        {
            Dock = DockStyle.Fill,
            SplitterDistance = 560
        };

        budgetsListView.Dock = DockStyle.Fill;
        budgetsListView.FullRowSelect = true;
        budgetsListView.HideSelection = false;
        budgetsListView.MultiSelect = false;
        budgetsListView.UseCompatibleStateImageBehavior = false;
        budgetsListView.View = View.Details;
        budgetsListView.Columns.Add("Month", 140);
        budgetsListView.Columns.Add("Year", 90);
        budgetsListView.Columns.Add("Month #", 90);
        budgetsListView.Columns.Add("NodeId", 240);
        budgetsListView.Columns.Add("MonthKey", 180);
        budgetsListView.SelectedIndexChanged += budgetsListView_SelectedIndexChanged;

        detailsTextBox.Dock = DockStyle.Fill;
        detailsTextBox.Multiline = true;
        detailsTextBox.ReadOnly = true;
        detailsTextBox.ScrollBars = ScrollBars.Both;
        detailsTextBox.WordWrap = false;

        split.Panel1.Controls.Add(budgetsListView);
        split.Panel2.Controls.Add(detailsTextBox);

        root.Controls.Add(split, 0, 3);

        FlowLayoutPanel buttons = new()
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.RightToLeft
        };

        Button closeButton = new()
        {
            Text = "Close",
            Width = 100
        };

        closeButton.Click += (_, _) => Close();

        buttons.Controls.Add(closeButton);
        root.Controls.Add(buttons, 0, 4);

        Controls.Add(root);
    }

    private Control BuildDatabaseGroup()
    {
        GroupBox groupBox = new()
        {
            Dock = DockStyle.Fill,
            Text = "Add-in Database"
        };

        TableLayoutPanel layout = new()
        {
            Dock = DockStyle.Fill,
            ColumnCount = 4,
            RowCount = 4,
            Padding = new Padding(8)
        };

        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110F));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 10F));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 230F));

        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

        layout.Controls.Add(new Label { Text = "Database:", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 0, 0);

        databaseNameLinkLabel.AutoEllipsis = true;
        databaseNameLinkLabel.Dock = DockStyle.Fill;
        databaseNameLinkLabel.MaximumSize = new Size(0, 22);
        databaseNameLinkLabel.TextAlign = ContentAlignment.MiddleLeft;
        databaseNameLinkLabel.LinkClicked += databaseNameLinkLabel_LinkClicked;
        layout.Controls.Add(databaseNameLinkLabel, 1, 0);

        databasePathTextBox.Dock = DockStyle.Fill;
        databasePathTextBox.ReadOnly = true;
        layout.Controls.Add(databasePathTextBox, 1, 1);

        LinkLabel copyProd = new()
        {
            Dock = DockStyle.Fill,
            Text = "Copy from Expensa Prod",
            TextAlign = ContentAlignment.MiddleLeft
        };
        copyProd.LinkClicked += copyFromExpensaProdLinkLabel_LinkClicked;
        layout.Controls.Add(copyProd, 3, 0);

        LinkLabel copyDev = new()
        {
            Dock = DockStyle.Fill,
            Text = "Copy new from dev template",
            TextAlign = ContentAlignment.MiddleLeft
        };
        copyDev.LinkClicked += copyFromDevTemplateLinkLabel_LinkClicked;
        layout.Controls.Add(copyDev, 3, 1);

        LinkLabel copySandbox = new()
        {
            Dock = DockStyle.Fill,
            Text = "Copy new from Sandbox DB",
            TextAlign = ContentAlignment.MiddleLeft
        };
        copySandbox.LinkClicked += copyFromSandboxLinkLabel_LinkClicked;
        layout.Controls.Add(copySandbox, 3, 2);

        layout.Controls.Add(new Label { Text = "Copied DB:", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 0, 2);

        copiedDatabasePathTextBox.Dock = DockStyle.Fill;
        copiedDatabasePathTextBox.ReadOnly = true;
        layout.Controls.Add(copiedDatabasePathTextBox, 1, 2);

        LinkLabel openCopiedFolder = new()
        {
            Dock = DockStyle.Fill,
            Text = "Open copied DB folder",
            TextAlign = ContentAlignment.MiddleLeft
        };
        openCopiedFolder.LinkClicked += openCopiedDatabaseFolderLinkLabel_LinkClicked;
        layout.Controls.Add(openCopiedFolder, 3, 2);

        copiedDatabasesListView.Dock = DockStyle.Fill;
        copiedDatabasesListView.FullRowSelect = true;
        copiedDatabasesListView.HideSelection = false;
        copiedDatabasesListView.MultiSelect = false;
        copiedDatabasesListView.UseCompatibleStateImageBehavior = false;
        copiedDatabasesListView.View = View.Details;
        copiedDatabasesListView.Columns.Add("Database", 260);
        copiedDatabasesListView.Columns.Add("Source", 160);
        copiedDatabasesListView.Columns.Add("Created", 160);
        copiedDatabasesListView.Columns.Add("Path", 520);
        copiedDatabasesListView.SelectedIndexChanged += copiedDatabasesListView_SelectedIndexChanged;

        layout.SetColumnSpan(copiedDatabasesListView, 4);
        layout.Controls.Add(copiedDatabasesListView, 0, 3);

        groupBox.Controls.Add(layout);

        return groupBox;
    }

    private Control BuildFilterPanel()
    {
        TableLayoutPanel layout = new()
        {
            Dock = DockStyle.Fill,
            ColumnCount = 6,
            RowCount = 1
        };

        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80F));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 140F));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100F));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110F));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100F));

        layout.Controls.Add(new Label { Text = "Search:", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 0, 0);

        searchTextBox.Dock = DockStyle.Fill;
        layout.Controls.Add(searchTextBox, 1, 0);

        includeClosedCheckBox.Dock = DockStyle.Fill;
        includeClosedCheckBox.Text = "Include closed";
        includeClosedCheckBox.TextAlign = ContentAlignment.MiddleLeft;
        layout.Controls.Add(includeClosedCheckBox, 2, 0);

        layout.Controls.Add(new Label { Text = "Max rows:", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 3, 0);

        maximumRowsNumericUpDown.Dock = DockStyle.Left;
        maximumRowsNumericUpDown.Minimum = 1;
        maximumRowsNumericUpDown.Maximum = 100000;
        maximumRowsNumericUpDown.Value = 500;
        maximumRowsNumericUpDown.Width = 90;
        layout.Controls.Add(maximumRowsNumericUpDown, 4, 0);

        refreshButton.Dock = DockStyle.Fill;
        refreshButton.Text = "Refresh";
        refreshButton.Click += refreshButton_Click;
        layout.Controls.Add(refreshButton, 5, 0);

        return layout;
    }

    private void LoadDatabaseLocation()
    {
        HostBudgetDatabaseLocation location =
            new HostBudgetRuntimeDatabaseSelectionService().GetActiveRuntimeDatabaseLocation();

        databaseNameLinkLabel.Text = location.DatabaseName;
        databasePathTextBox.Text = location.DatabasePath;
    }

    private async void refreshButton_Click(object? sender, EventArgs e)
    {
        await RefreshBudgetsAsync().ConfigureAwait(true);
    }

    private async Task RefreshBudgetsAsync()
    {
        refreshButton.Enabled = false;
        statusLabel.Text = "Loading Budgets database view...";
        detailsTextBox.Clear();

        try
        {
            CommandExecutionResult executionResult =
                await invoker.ExecuteAsync(
                    new HostBudgetRuntimeLoadRequest
                    {
                        SearchText = searchTextBox.Text.Trim(),
                        IncludeClosed = includeClosedCheckBox.Checked,
                        MaximumRows = (int)maximumRowsNumericUpDown.Value
                    }).ConfigureAwait(true);

            IReadOnlyList<BudgetFlatRow> rows =
                BudgetFlatRowBuilder.BuildRows(executionResult.OutputJson);

            PopulateListView(rows);

            statusLabel.Text =
                $"{executionResult.Status}: {rows.Count} budget month row(s). {executionResult.Message}";

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
                "Budgets Database",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
        finally
        {
            refreshButton.Enabled = true;
        }
    }

    private void PopulateListView(IReadOnlyList<BudgetFlatRow> rows)
    {
        budgetsListView.BeginUpdate();

        try
        {
            budgetsListView.Items.Clear();

            foreach (BudgetFlatRow row in rows)
            {
                ListViewItem item = new(row.DisplayText)
                {
                    Tag = row
                };

                item.SubItems.Add(row.BudgetYear?.ToString() ?? string.Empty);
                item.SubItems.Add(row.BudgetMonth?.ToString() ?? string.Empty);
                item.SubItems.Add(row.NodeId);
                item.SubItems.Add(row.MonthKey);

                budgetsListView.Items.Add(item);
            }
        }
        finally
        {
            budgetsListView.EndUpdate();
        }
    }

    private void databaseNameLinkLabel_LinkClicked(object? sender, LinkLabelLinkClickedEventArgs e)
    {
        HostBudgetDatabaseLocation location =
            new HostBudgetRuntimeDatabaseSelectionService().GetActiveRuntimeDatabaseLocation();

        Directory.CreateDirectory(location.DatabaseFolder);

        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
        {
            FileName = location.DatabaseFolder,
            UseShellExecute = true
        });
    }

    private void copyFromExpensaProdLinkLabel_LinkClicked(object? sender, LinkLabelLinkClickedEventArgs e)
    {
        try
        {
            HostBudgetExpensaProdDatabaseCopyResult result =
                expensaProdCopyService.CopyToDevAndRuntime();

            copyHistory.Add(
                new HostBudgetDatabaseCopyRecord
                {
                    SourceLabel = "Expensa production",
                    DatabasePath = result.RuntimeDatabasePath
                });

            RefreshCopyHistoryList();
            copiedDatabasePathTextBox.Text = result.RuntimeDatabasePath;
            LoadDatabaseLocation();

            string warningText =
                result.Warnings.Count == 0
                    ? string.Empty
                    : $"{Environment.NewLine}{Environment.NewLine}Warnings:{Environment.NewLine}{string.Join(Environment.NewLine, result.Warnings)}";

            MessageBox.Show(
                this,
                $"Copied Expensa production budget data to:{Environment.NewLine}{Environment.NewLine}{result.RuntimeDatabasePath}{warningText}",
                "Budgets Database",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
        catch (Exception exception)
        {
            MessageBox.Show(
                this,
                exception.Message,
                "Copy From Expensa Production Failed",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }

    private void copyFromDevTemplateLinkLabel_LinkClicked(object? sender, LinkLabelLinkClickedEventArgs e)
    {
        CopyDatabase("dev template", databaseCopyService.CopyNewFromDevTemplate);
    }

    private void copyFromSandboxLinkLabel_LinkClicked(object? sender, LinkLabelLinkClickedEventArgs e)
    {
        CopyDatabase("Sandbox database", databaseCopyService.CopyNewFromSandbox);
    }

    private void CopyDatabase(string sourceDescription, Func<string> copyAction)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sourceDescription);
        ArgumentNullException.ThrowIfNull(copyAction);

        try
        {
            string targetPath = copyAction();

            copyHistory.Add(
                new HostBudgetDatabaseCopyRecord
                {
                    SourceLabel = sourceDescription,
                    DatabasePath = targetPath
                });

            RefreshCopyHistoryList();
            copiedDatabasePathTextBox.Text = targetPath;

            MessageBox.Show(
                this,
                $"Copied new database from {sourceDescription}.{Environment.NewLine}{Environment.NewLine}{targetPath}",
                "Budgets Database",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
        catch (Exception exception)
        {
            MessageBox.Show(
                this,
                exception.Message,
                "Budgets Database Copy Failed",
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

            foreach (HostBudgetDatabaseCopyRecord record in copyHistory.Records)
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

    private void copiedDatabasesListView_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (copiedDatabasesListView.SelectedItems.Count == 0)
        {
            return;
        }

        if (copiedDatabasesListView.SelectedItems[0].Tag is not HostBudgetDatabaseCopyRecord record)
        {
            return;
        }

        copiedDatabasePathTextBox.Text = record.DatabasePath;
    }

    private void openCopiedDatabaseFolderLinkLabel_LinkClicked(object? sender, LinkLabelLinkClickedEventArgs e)
    {
        string databasePath = copiedDatabasePathTextBox.Text.Trim();

        if (string.IsNullOrWhiteSpace(databasePath))
        {
            return;
        }

        string? folder = Path.GetDirectoryName(databasePath);

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

    private void budgetsListView_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (budgetsListView.SelectedItems.Count == 0)
        {
            return;
        }

        if (budgetsListView.SelectedItems[0].Tag is not BudgetFlatRow row)
        {
            return;
        }

        detailsTextBox.Text =
            $"NodeId: {row.NodeId}{Environment.NewLine}" +
            $"DisplayText: {row.DisplayText}{Environment.NewLine}" +
            $"Year: {row.BudgetYear}{Environment.NewLine}" +
            $"Month: {row.BudgetMonth}{Environment.NewLine}" +
            $"MonthKey: {row.MonthKey}";
    }
}
