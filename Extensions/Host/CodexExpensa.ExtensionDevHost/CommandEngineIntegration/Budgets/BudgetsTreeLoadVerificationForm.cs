namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Budgets;

public sealed class BudgetsTreeLoadVerificationForm : Form
{
    private readonly HostBudgetTreeContributionLoader loader = new();
    private readonly TreeView treeView = new();
    private readonly TextBox detailsTextBox = new();
    private readonly Button loadButton = new();
    private readonly CheckBox expandAllCheckBox = new();
    private readonly Label statusLabel = new();

    public BudgetsTreeLoadVerificationForm()
    {
        Text = "Budgets Tree Load Verification";
        Width = 1000;
        Height = 650;
        StartPosition = FormStartPosition.CenterParent;

        TableLayoutPanel root = new()
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 3,
            Padding = new Padding(8)
        };

        FlowLayoutPanel toolbar = new()
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight
        };

        loadButton.Text = "Load";
        loadButton.Click += loadButton_Click;

        expandAllCheckBox.Text = "Expand";
        expandAllCheckBox.AutoSize = true;

        toolbar.Controls.Add(loadButton);
        toolbar.Controls.Add(expandAllCheckBox);

        SplitContainer split = new()
        {
            Dock = DockStyle.Fill,
            SplitterDistance = 380
        };

        treeView.Dock = DockStyle.Fill;
        treeView.AfterSelect += treeView_AfterSelect;

        detailsTextBox.Dock = DockStyle.Fill;
        detailsTextBox.Multiline = true;
        detailsTextBox.ReadOnly = true;
        detailsTextBox.ScrollBars = ScrollBars.Both;
        detailsTextBox.WordWrap = false;

        split.Panel1.Controls.Add(treeView);
        split.Panel2.Controls.Add(detailsTextBox);

        statusLabel.Dock = DockStyle.Fill;
        statusLabel.Text = "Ready.";

        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 36F));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 28F));

        root.Controls.Add(toolbar, 0, 0);
        root.Controls.Add(split, 0, 1);
        root.Controls.Add(statusLabel, 0, 2);

        Controls.Add(root);
    }

    protected override async void OnShown(EventArgs e)
    {
        base.OnShown(e);
        await LoadBudgetsTreeAsync().ConfigureAwait(true);
    }

    private async void loadButton_Click(object? sender, EventArgs e)
    {
        await LoadBudgetsTreeAsync().ConfigureAwait(true);
    }

    private async Task LoadBudgetsTreeAsync()
    {
        loadButton.Enabled = false;
        statusLabel.Text = "Loading Budgets tree...";

        try
        {
            HostBudgetTreeLoadResult result =
                await loader.LoadContributionAsync(
                    treeView,
                    new HostBudgetTreeLoadOptions
                    {
                        ExpandAll = expandAllCheckBox.Checked,
                        MaximumRows = 500
                    }).ConfigureAwait(true);

            statusLabel.Text = $"{result.ExecutionResult.Status}: {result.RootNodeCount} root node(s).";
        }
        catch (Exception exception)
        {
            statusLabel.Text = "Failed.";
            detailsTextBox.Text = exception.ToString();

            MessageBox.Show(this, exception.Message, "Budgets Tree Load Verification", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            loadButton.Enabled = true;
        }
    }

    private void treeView_AfterSelect(object? sender, TreeViewEventArgs e)
    {
        detailsTextBox.Text = e.Node?.Tag?.ToString() ?? e.Node?.Text ?? string.Empty;
    }
}
