namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Payees;

public sealed partial class PayeesTreeLoadVerificationForm
{
    private ContextMenuStrip? _payeesTreeContextMenuStrip;

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        AttachPayeesTreeContextMenu();
    }

    private void AttachPayeesTreeContextMenu()
    {
        if (_payeesTreeContextMenuStrip is not null)
        {
            return;
        }

        _payeesTreeContextMenuStrip = new ContextMenuStrip();

        AddPayeeTagPickerMenuItem(_payeesTreeContextMenuStrip);

        TestTreeView.ContextMenuStrip = null;

        TestTreeView.MouseUp -= TestTreeView_RuntimeMouseUp;
        TestTreeView.MouseUp += TestTreeView_RuntimeMouseUp;
    }

    private void TestTreeView_RuntimeMouseUp(object? sender, MouseEventArgs e)
    {
        if (e.Button != MouseButtons.Right)
        {
            return;
        }

        TreeNode? clickedNode = TestTreeView.GetNodeAt(e.Location);

        if (clickedNode is null)
        {
            return;
        }

        TestTreeView.SelectedNode = clickedNode;

        if (!IsPayeeTreeNode(clickedNode))
        {
            SetNotes("Context menu is available only for individual payee nodes.");
            return;
        }

        RememberPayeeTreeContextMenuLocation(e.Location);

        _payeesTreeContextMenuStrip?.Show(TestTreeView, e.Location);
    }

    private static bool IsPayeeTreeNode(TreeNode node)
    {
        return !string.IsNullOrWhiteSpace(TryGetPayeeId(node));
    }
}
