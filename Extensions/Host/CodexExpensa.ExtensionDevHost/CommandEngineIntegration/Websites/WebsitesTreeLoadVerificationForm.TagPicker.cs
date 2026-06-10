namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Websites;

public sealed partial class WebsitesTreeLoadVerificationForm
{
    private readonly HostWebsiteTagAssignmentService _tagAssignmentService = new();
    private readonly HostWebsiteRuntimeDatabaseSelectionService _runtimeDatabaseSelectionService = new();

    private ToolStripMenuItem? _tagPickerMenuItem;
    private Form? _tagPickerPopupForm;
    private WebsitesTreeTagPickerPanel? _tagPickerPanel;
    private Point _lastTreeContextMenuScreenLocation;

    private void AddTagPickerMenuItem(ContextMenuStrip menuStrip)
    {
        ArgumentNullException.ThrowIfNull(menuStrip);

        if (_tagPickerMenuItem is not null)
        {
            return;
        }

        _tagPickerMenuItem = new ToolStripMenuItem("________");
        _tagPickerMenuItem.Click += tagPickerMenuItem_Click;

        menuStrip.Items.Add(_tagPickerMenuItem);
    }

    private void RememberTreeContextMenuLocation(Point clientLocation)
    {
        _lastTreeContextMenuScreenLocation = websitesTreeView.PointToScreen(clientLocation);
    }

    private void tagPickerMenuItem_Click(object? sender, EventArgs e)
    {
        ShowTagPickerPopup();
    }

    private void ShowTagPickerPopup()
    {
        TreeNode? selectedNode = websitesTreeView.SelectedNode;

        if (selectedNode is null || !IsWebsiteTreeNode(selectedNode))
        {
            SetStatus("Select an individual website node before assigning a tag.");
            return;
        }

        string websiteId = HostWebsiteTreeNodeTagReader.GetWebsiteId(selectedNode);

        CloseTagPickerPopup();

        _tagPickerPanel = new WebsitesTreeTagPickerPanel();
        _tagPickerPanel.SetTags(GetAvailableTreeTagNamesForWebsite(websiteId));
        _tagPickerPanel.TypedTagAccepted += tagPickerPanel_TypedTagAccepted;
        _tagPickerPanel.ListedTagAccepted += tagPickerPanel_ListedTagAccepted;

        int popupHeight = Math.Max(300, websitesTreeView.ClientSize.Height - 12);

        _tagPickerPopupForm = new Form
        {
            FormBorderStyle = FormBorderStyle.FixedSingle,
            ShowInTaskbar = false,
            StartPosition = FormStartPosition.Manual,
            Size = new Size(300, popupHeight),
            Text = "Tag",
            TopMost = true
        };

        _tagPickerPanel.Dock = DockStyle.Fill;
        _tagPickerPopupForm.Controls.Add(_tagPickerPanel);

        Point treeTopLeft = websitesTreeView.PointToScreen(Point.Empty);
        Point popupLocation = _lastTreeContextMenuScreenLocation == Point.Empty
            ? websitesTreeView.PointToScreen(new Point(20, 20))
            : _lastTreeContextMenuScreenLocation;

        _tagPickerPopupForm.Location = new Point(popupLocation.X + 12, treeTopLeft.Y + 6);

        _tagPickerPopupForm.Deactivate += tagPickerPopupForm_Deactivate;
        _tagPickerPopupForm.FormClosed += tagPickerPopupForm_FormClosed;

        _tagPickerPopupForm.Show(this);
        _tagPickerPanel.FocusTextBox();
    }

    private void tagPickerPopupForm_Deactivate(object? sender, EventArgs e)
    {
        CloseTagPickerPopup();
    }

    private void tagPickerPopupForm_FormClosed(object? sender, FormClosedEventArgs e)
    {
        if (_tagPickerPopupForm is not null)
        {
            _tagPickerPopupForm.Deactivate -= tagPickerPopupForm_Deactivate;
            _tagPickerPopupForm.FormClosed -= tagPickerPopupForm_FormClosed;
        }

        _tagPickerPopupForm = null;
        _tagPickerPanel = null;
    }

    private void CloseTagPickerPopup()
    {
        if (_tagPickerPopupForm is null)
        {
            return;
        }

        Form popupForm = _tagPickerPopupForm;

        _tagPickerPopupForm = null;
        _tagPickerPanel = null;

        popupForm.Close();
        popupForm.Dispose();
    }

    private async void tagPickerPanel_TypedTagAccepted(object? sender, string tagName)
    {
        await AssignTagToSelectedWebsiteAsync(tagName).ConfigureAwait(true);
    }

    private async void tagPickerPanel_ListedTagAccepted(object? sender, string tagName)
    {
        await AssignTagToSelectedWebsiteAsync(tagName).ConfigureAwait(true);
    }

    private async Task AssignTagToSelectedWebsiteAsync(string tagName)
    {
        TreeNode? selectedNode = websitesTreeView.SelectedNode;

        if (selectedNode is null)
        {
            SetStatus("Select a website before assigning a tag.");
            return;
        }

        string websiteId = HostWebsiteTreeNodeTagReader.GetWebsiteId(selectedNode);

        if (string.IsNullOrWhiteSpace(websiteId))
        {
            SetStatus("Select an individual website node before assigning a tag.");
            return;
        }

        HostWebsiteDatabaseLocation databaseLocation =
            _runtimeDatabaseSelectionService.GetActiveRuntimeDatabaseLocation();

        string databasePath = databaseLocation.DatabasePath;

        if (string.IsNullOrWhiteSpace(databasePath))
        {
            SetStatus("No active Websites runtime database is selected.");
            return;
        }

        try
        {
            HostWebsiteTagAssignmentResult result =
                _tagAssignmentService.AddOrAssignTagToWebsite(databasePath, websiteId, tagName);

            SetStatus(result.AssignmentCreated
                ? $"Assigned tag '{result.TagName}' to selected website."
                : $"Selected website already has tag '{result.TagName}'.");

            CloseTagPickerPopup();

            await LoadWebsitesTreeAsync().ConfigureAwait(true);

            SelectFirstTagNodeByName(result.TagName);
        }
        catch (Exception exception)
        {
            SetStatus("Tag assignment failed.");

            MessageBox.Show(
                this,
                exception.Message,
                "Assign Website Tag",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }

    private IReadOnlyList<string> GetAvailableTreeTagNamesForWebsite(string websiteId)
    {
        HashSet<string> assignedTags =
            new(GetAssignedTagNamesForWebsite(websiteId), StringComparer.CurrentCultureIgnoreCase);

        return GetCurrentTreeTagNames()
            .Where(tagName => !assignedTags.Contains(tagName))
            .ToList();
    }

    private IReadOnlyList<string> GetAssignedTagNamesForWebsite(string websiteId)
    {
        HostWebsiteDatabaseLocation databaseLocation =
            _runtimeDatabaseSelectionService.GetActiveRuntimeDatabaseLocation();

        string databasePath = databaseLocation.DatabasePath;

        if (string.IsNullOrWhiteSpace(databasePath))
        {
            return [];
        }

        return _tagAssignmentService.GetAssignedTagNames(databasePath, websiteId);
    }

    private IReadOnlyList<string> GetCurrentTreeTagNames()
    {
        return websitesTreeView.Nodes
            .Cast<TreeNode>()
            .Select(static node => node.Text)
            .Where(static text => !string.IsNullOrWhiteSpace(text))
            .Distinct(StringComparer.CurrentCultureIgnoreCase)
            .OrderBy(static text => text, StringComparer.CurrentCultureIgnoreCase)
            .ToList();
    }

    private void SelectFirstTagNodeByName(string tagName)
    {
        if (string.IsNullOrWhiteSpace(tagName))
        {
            return;
        }

        TreeNode? matchingNode = websitesTreeView.Nodes
            .Cast<TreeNode>()
            .FirstOrDefault(node => string.Equals(node.Text, tagName, StringComparison.CurrentCultureIgnoreCase));

        if (matchingNode is null)
        {
            SetStatus($"Tag not found: {tagName}");
            return;
        }

        websitesTreeView.SelectedNode = matchingNode;
        matchingNode.EnsureVisible();
    }
}
