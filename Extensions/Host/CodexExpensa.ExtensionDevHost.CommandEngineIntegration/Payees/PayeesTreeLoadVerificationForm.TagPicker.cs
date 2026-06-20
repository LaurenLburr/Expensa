using CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Websites;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Payees;

public sealed partial class PayeesTreeLoadVerificationForm
{
    private readonly HostPayeeTagAssignmentService _payeeTagAssignmentService = new();

    private ToolStripMenuItem? _payeeTagPickerMenuItem;
    private Form? _payeeTagPickerPopupForm;
    private WebsitesTreeTagPickerPanel? _payeeTagPickerPanel;
    private Point _lastPayeeTreeContextMenuScreenLocation;

    private void AddPayeeTagPickerMenuItem(ContextMenuStrip menuStrip)
    {
        ArgumentNullException.ThrowIfNull(menuStrip);

        if (_payeeTagPickerMenuItem is not null)
        {
            return;
        }

        _payeeTagPickerMenuItem = new ToolStripMenuItem("Add Tag...");
        _payeeTagPickerMenuItem.Click += payeeTagPickerMenuItem_Click;

        menuStrip.Items.Add(_payeeTagPickerMenuItem);
    }

    private void RememberPayeeTreeContextMenuLocation(Point clientLocation)
    {
        _lastPayeeTreeContextMenuScreenLocation = TestTreeView.PointToScreen(clientLocation);
    }

    private void payeeTagPickerMenuItem_Click(object? sender, EventArgs e)
    {
        ShowPayeeTagPickerPopup();
    }

    private void ShowPayeeTagPickerPopup()
    {
        TreeNode? selectedNode = TestTreeView.SelectedNode;

        if (selectedNode is null || !IsPayeeTreeNode(selectedNode))
        {
            SetNotes("Select an individual payee node before assigning a tag.");
            return;
        }

        string payeeId = TryGetPayeeId(selectedNode);

        ClosePayeeTagPickerPopup();

        _payeeTagPickerPanel = new WebsitesTreeTagPickerPanel();
        _payeeTagPickerPanel.SetTags(GetAvailableTreeTagNamesForPayee(payeeId));
        _payeeTagPickerPanel.TypedTagAccepted += payeeTagPickerPanel_TypedTagAccepted;
        _payeeTagPickerPanel.ListedTagAccepted += payeeTagPickerPanel_ListedTagAccepted;

        int popupHeight = Math.Max(300, TestTreeView.ClientSize.Height - 12);

        _payeeTagPickerPopupForm = new Form
        {
            FormBorderStyle = FormBorderStyle.FixedSingle,
            ShowInTaskbar = false,
            StartPosition = FormStartPosition.Manual,
            Size = new Size(300, popupHeight),
            Text = "Payee Tag",
            TopMost = true
        };

        _payeeTagPickerPanel.Dock = DockStyle.Fill;
        _payeeTagPickerPopupForm.Controls.Add(_payeeTagPickerPanel);

        Point treeTopLeft = TestTreeView.PointToScreen(Point.Empty);
        Point popupLocation = _lastPayeeTreeContextMenuScreenLocation == Point.Empty
            ? TestTreeView.PointToScreen(new Point(20, 20))
            : _lastPayeeTreeContextMenuScreenLocation;

        _payeeTagPickerPopupForm.Location = new Point(popupLocation.X + 12, treeTopLeft.Y + 6);

        _payeeTagPickerPopupForm.Deactivate += payeeTagPickerPopupForm_Deactivate;
        _payeeTagPickerPopupForm.FormClosed += payeeTagPickerPopupForm_FormClosed;

        _payeeTagPickerPopupForm.Show(this);
        _payeeTagPickerPanel.FocusTextBox();
    }

    private void payeeTagPickerPopupForm_Deactivate(object? sender, EventArgs e)
    {
        ClosePayeeTagPickerPopup();
    }

    private void payeeTagPickerPopupForm_FormClosed(object? sender, FormClosedEventArgs e)
    {
        if (_payeeTagPickerPopupForm is not null)
        {
            _payeeTagPickerPopupForm.Deactivate -= payeeTagPickerPopupForm_Deactivate;
            _payeeTagPickerPopupForm.FormClosed -= payeeTagPickerPopupForm_FormClosed;
        }

        _payeeTagPickerPopupForm = null;
        _payeeTagPickerPanel = null;
    }

    private void ClosePayeeTagPickerPopup()
    {
        if (_payeeTagPickerPopupForm is null)
        {
            return;
        }

        Form popupForm = _payeeTagPickerPopupForm;

        _payeeTagPickerPopupForm = null;
        _payeeTagPickerPanel = null;

        popupForm.Close();
        popupForm.Dispose();
    }

    private async void payeeTagPickerPanel_TypedTagAccepted(object? sender, string tagName)
    {
        await AssignTagToSelectedPayeeAsync(tagName).ConfigureAwait(true);
    }

    private async void payeeTagPickerPanel_ListedTagAccepted(object? sender, string tagName)
    {
        await AssignTagToSelectedPayeeAsync(tagName).ConfigureAwait(true);
    }

    private async Task AssignTagToSelectedPayeeAsync(string tagName)
    {
        TreeNode? selectedNode = TestTreeView.SelectedNode;

        if (selectedNode is null)
        {
            SetNotes("Select a payee before assigning a tag.");
            return;
        }

        string payeeId = TryGetPayeeId(selectedNode);

        if (string.IsNullOrWhiteSpace(payeeId))
        {
            SetNotes("Select an individual payee node before assigning a tag.");
            return;
        }

        if (string.IsNullOrWhiteSpace(_databasePath))
        {
            SetNotes("No active Payees runtime database is selected.");
            return;
        }

        try
        {
            HostPayeeTagAssignmentResult result =
                _payeeTagAssignmentService.AddOrAssignTagToPayee(_databasePath, payeeId, tagName);

            SetNotes(result.AssignmentCreated
                ? $"Assigned tag '{result.TagName}' to selected payee."
                : $"Selected payee already has tag '{result.TagName}'.");

            ClosePayeeTagPickerPopup();

            await LoadPayeesTreeAsync().ConfigureAwait(true);

            SelectFirstTagNodeByName(result.TagName);
        }
        catch (Exception exception)
        {
            SetNotes("Payee tag assignment failed." + Environment.NewLine + Environment.NewLine + exception);

            MessageBox.Show(
                this,
                exception.Message,
                "Assign Payee Tag",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }

    private IReadOnlyList<string> GetAvailableTreeTagNamesForPayee(string payeeId)
    {
        HashSet<string> assignedTags =
            new(GetAssignedTagNamesForPayee(payeeId), StringComparer.CurrentCultureIgnoreCase);

        return GetCurrentTreeTagNames()
            .Where(tagName => !assignedTags.Contains(tagName))
            .ToList();
    }

    private IReadOnlyList<string> GetAssignedTagNamesForPayee(string payeeId)
    {
        if (string.IsNullOrWhiteSpace(_databasePath))
        {
            return [];
        }

        return _payeeTagAssignmentService.GetAssignedTagNames(_databasePath, payeeId);
    }

    private IReadOnlyList<string> GetCurrentTreeTagNames()
    {
        TreeNodeCollection groupNodes = TestTreeView.Nodes.Count == 1
            ? TestTreeView.Nodes[0].Nodes
            : TestTreeView.Nodes;

        return groupNodes
            .Cast<TreeNode>()
            .Select(static node => node.Text)
            .Where(static text => !string.IsNullOrWhiteSpace(text))
            .Where(static text => !string.Equals(text, "By Name", StringComparison.CurrentCultureIgnoreCase))
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

        TreeNodeCollection groupNodes = TestTreeView.Nodes.Count == 1
            ? TestTreeView.Nodes[0].Nodes
            : TestTreeView.Nodes;

        TreeNode? matchingNode = groupNodes
            .Cast<TreeNode>()
            .FirstOrDefault(node => string.Equals(node.Text, tagName, StringComparison.CurrentCultureIgnoreCase));

        if (matchingNode is null)
        {
            SetNotes($"Tag not found after reload: {tagName}");
            return;
        }

        TestTreeView.SelectedNode = matchingNode;
        matchingNode.EnsureVisible();
    }
}
