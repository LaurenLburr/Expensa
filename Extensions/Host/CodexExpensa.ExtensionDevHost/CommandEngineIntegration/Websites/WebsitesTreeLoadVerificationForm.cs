using CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Templates;
using System.Text.Json;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Websites;

public sealed partial class WebsitesTreeLoadVerificationForm : TreeTestTemplate
{
    private readonly HostWebsiteTreeContributionLoader _loader;
    private readonly string _settingsPath;
    private bool _hasAutoLoaded;

    private TreeView websitesTreeView => TestTreeView;

    private TextBox detailsTextBox => NotesTextBox;

    private SplitContainer splitContainer => ContentSplitContainer;

    public WebsitesTreeLoadVerificationForm()
        : this(new HostWebsiteTreeContributionLoader())
    {
    }

    public WebsitesTreeLoadVerificationForm(HostWebsiteTreeContributionLoader loader)
    {
        ArgumentNullException.ThrowIfNull(loader);

        _loader = loader;
        _settingsPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Expensa",
            "Extensions",
            "WebsitesTreeLoadVerificationSettings.json");

        InitializeComponent();

        ConfigureTreeTestTemplate(
            "Websites Tree Load Verification",
            "Load the Websites tree to verify the add-in contribution that Extension Manager will display.");

        websitesTreeView.Name = "websitesTreeView";
        detailsTextBox.Name = "detailsTextBox";

        websitesTreeView.AfterSelect += websitesTreeView_AfterSelect;
        websitesTreeView.NodeMouseClick += websitesTreeView_NodeMouseClick;
        splitContainer.SplitterMoved += splitContainer_SplitterMoved;
        FormClosing += websitesTreeLoadVerificationForm_FormClosing;

        EnsureWebsiteDetailsPanel();
        RestoreSplitterDistance();
    }

    protected override async void OnShown(EventArgs e)
    {
        base.OnShown(e);

        if (_hasAutoLoaded)
        {
            return;
        }

        _hasAutoLoaded = true;

        await LoadWebsitesTreeAsync().ConfigureAwait(true);
    }

    private async Task LoadWebsitesTreeAsync()
    {
        SetStatus("Loading Websites tree...");
        ShowSelectedTreeNodeDetails(null);

        try
        {
            HostWebsiteTreeLoadResult result =
                await _loader.LoadContributionAsync(
                    websitesTreeView,
                    new HostWebsiteTreeLoadOptions()).ConfigureAwait(true);

            SetStatus(
                $"{result.ExecutionResult.Status}: {result.RootNodeCount} root node(s), {result.WebsiteResult.TotalCount} website row(s). {result.WebsiteResult.Message}");
        }
        catch (Exception exception)
        {
            SetStatus("Failed.");

            detailsTextBox.Visible = true;
            detailsTextBox.Text = exception.ToString();

            MessageBox.Show(
                this,
                exception.Message,
                "Websites Tree Load Verification",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }

    private void SetStatus(string message)
    {
        detailsTextBox.Visible = true;
        detailsTextBox.Text = message;
    }

    private void websitesTreeView_NodeMouseClick(object? sender, TreeNodeMouseClickEventArgs e)
    {
        if (e.Button == MouseButtons.Right)
        {
            websitesTreeView.SelectedNode = e.Node;
        }
    }

    private void sortAscendingToolStripMenuItem_Click(object? sender, EventArgs e)
    {
        SortTreeNodesFromContextMenu(ascending: true);
    }

    private void sortDescendingToolStripMenuItem_Click(object? sender, EventArgs e)
    {
        SortTreeNodesFromContextMenu(ascending: false);
    }

    private void websitesTreeView_AfterSelect(object? sender, TreeViewEventArgs e)
    {
        ShowSelectedTreeNodeDetails(e.Node);
    }

    private void splitContainer_SplitterMoved(object? sender, SplitterEventArgs e)
    {
        SaveSplitterDistance();
    }

    private void websitesTreeLoadVerificationForm_FormClosing(object? sender, FormClosingEventArgs e)
    {
        SaveSplitterDistance();
    }

    private void RestoreSplitterDistance()
    {
        WebsitesTreeLoadVerificationSettings settings =
            LoadSettings();

        if (settings.SplitterDistance <= 0)
        {
            return;
        }

        splitContainer.SplitterDistance =
            GetSafeSplitterDistance(settings.SplitterDistance);
    }

    private void SaveSplitterDistance()
    {
        try
        {
            WebsitesTreeLoadVerificationSettings settings =
                LoadSettings();

            settings.SplitterDistance = splitContainer.SplitterDistance;
            SaveSettings(settings);
        }
        catch
        {
        }
    }

    private int GetSafeSplitterDistance(int requestedDistance)
    {
        int maxDistance =
            Math.Max(
                splitContainer.Panel1MinSize,
                splitContainer.Width - splitContainer.Panel2MinSize - splitContainer.SplitterWidth);

        return Math.Clamp(
            requestedDistance,
            splitContainer.Panel1MinSize,
            maxDistance);
    }

    private WebsitesTreeLoadVerificationSettings LoadSettings()
    {
        try
        {
            if (!File.Exists(_settingsPath))
            {
                return new WebsitesTreeLoadVerificationSettings();
            }

            string json = File.ReadAllText(_settingsPath);

            return JsonSerializer.Deserialize<WebsitesTreeLoadVerificationSettings>(json)
                ?? new WebsitesTreeLoadVerificationSettings();
        }
        catch
        {
            return new WebsitesTreeLoadVerificationSettings();
        }
    }

    private void SaveSettings(WebsitesTreeLoadVerificationSettings settings)
    {
        string? folder =
            Path.GetDirectoryName(_settingsPath);

        if (!string.IsNullOrWhiteSpace(folder))
        {
            Directory.CreateDirectory(folder);
        }

        string json =
            JsonSerializer.Serialize(
                settings,
                new JsonSerializerOptions
                {
                    WriteIndented = true
                });

        File.WriteAllText(_settingsPath, json);
    }

    private sealed class WebsitesTreeLoadVerificationSettings
    {
        public int SplitterDistance { get; set; }
    }
}
