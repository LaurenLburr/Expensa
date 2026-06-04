using CodexExpensa.ExtensionDevHost.CommandEngineIntegration.CommonTree;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Websites;

public sealed class WebsitesTreeLoadVerificationFormCommonTree : Form
{
    private readonly CommonWebsiteTreeContributionLoader loader = new();
    private readonly TreeView treeView = new();
    private readonly TextBox detailsTextBox = new();
    private readonly Button loadButton = new();
    private readonly CheckBox expandAllCheckBox = new();
    private readonly CheckBox includeDisabledCheckBox = new();
    private readonly TextBox searchTextBox = new();
    private readonly NumericUpDown maximumRowsNumericUpDown = new();
    private readonly Label statusLabel = new();

    public WebsitesTreeLoadVerificationFormCommonTree()
    {
        Text = "Websites Tree Load Verification - CommonTree";
        Width = 1100;
        Height = 700;
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

        includeDisabledCheckBox.Text = "Include disabled";
        includeDisabledCheckBox.AutoSize = true;

        searchTextBox.Width = 220;
        searchTextBox.PlaceholderText = "Search...";

        maximumRowsNumericUpDown.Minimum = 1;
        maximumRowsNumericUpDown.Maximum = 10000;
        maximumRowsNumericUpDown.Value = 500;
        maximumRowsNumericUpDown.Width = 80;

        toolbar.Controls.Add(loadButton);
        toolbar.Controls.Add(new Label { Text = "Search:", AutoSize = true, TextAlign = ContentAlignment.MiddleLeft });
        toolbar.Controls.Add(searchTextBox);
        toolbar.Controls.Add(includeDisabledCheckBox);
        toolbar.Controls.Add(expandAllCheckBox);
        toolbar.Controls.Add(new Label { Text = "Max:", AutoSize = true, TextAlign = ContentAlignment.MiddleLeft });
        toolbar.Controls.Add(maximumRowsNumericUpDown);

        SplitContainer split = new()
        {
            Dock = DockStyle.Fill,
            SplitterDistance = 400
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
        await LoadWebsitesTreeAsync().ConfigureAwait(true);
    }

    private async void loadButton_Click(object? sender, EventArgs e)
    {
        await LoadWebsitesTreeAsync().ConfigureAwait(true);
    }

    private async Task LoadWebsitesTreeAsync()
    {
        loadButton.Enabled = false;
        statusLabel.Text = "Loading Websites tree through CommonTree...";

        try
        {
            int rootNodeCount =
                await loader.LoadContributionAsync(
                    treeView,
                    new HostWebsiteTreeLoadOptions
                    {
                        SearchText = searchTextBox.Text.Trim(),
                        IncludeDisabled = includeDisabledCheckBox.Checked,
                        ExpandAll = expandAllCheckBox.Checked,
                        MaximumRows = (int)maximumRowsNumericUpDown.Value
                    }).ConfigureAwait(true);

            statusLabel.Text = $"Loaded {rootNodeCount} root node(s) through CommonTree.";
        }
        catch (Exception exception)
        {
            statusLabel.Text = "Failed.";
            detailsTextBox.Text = exception.ToString();

            MessageBox.Show(
                this,
                exception.Message,
                "Websites CommonTree Load Verification",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
        finally
        {
            loadButton.Enabled = true;
        }
    }

    private void treeView_AfterSelect(object? sender, TreeViewEventArgs e)
    {
        detailsTextBox.Text = AddinTreeSelectionFormatter.FormatSelectedNode(treeView);
    }
}
