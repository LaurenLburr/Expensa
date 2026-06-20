using Krypton.Toolkit.Suite.Extended.TreeGridView;
using System.Data;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Templates;

public partial class TreeTestTemplate : Form
{
    public TreeTestTemplate()
    {
        InitializeComponent();
        ConfigureTemplateDefaults();
    }

    protected TreeView TestTreeView => tree;

    // Keep the base DataGridView surface for existing derived forms.
    protected DataGridView ResultGrid => gridData;

    // Expose the hierarchical control to derived forms that need tree features.
    protected KryptonTreeGridView HierarchyGrid => gridData;

    protected TextBox NotesTextBox => textNotes;

    protected SplitContainer MainSplitContainer => splitContainer1;

    protected SplitContainer ContentSplitContainer => splitContainer2;

    protected void ConfigureTreeTestTemplate(
        string title,
        string notesText)
    {
        Text = title;
        labelAdd_in_Name.Text = title;
        textNotes.Text = notesText;
    }

    protected void SetTreeNodes(
        IEnumerable<TreeNode> nodes,
        bool expandAll)
    {
        ArgumentNullException.ThrowIfNull(nodes);

        tree.BeginUpdate();

        try
        {
            tree.Nodes.Clear();

            foreach (TreeNode node in nodes)
            {
                tree.Nodes.Add(node);
            }

            if (expandAll)
            {
                tree.ExpandAll();
            }
        }
        finally
        {
            tree.EndUpdate();
        }
    }

    protected void SetGridData(
        object? dataSource)
    {
        switch (dataSource)
        {
            case null:
                ClearGridData();
                return;

            case DataTable table:
                SetGridData(table);
                return;

            default:
                throw new ArgumentException(
                    "TreeTestTemplate grids require a DataTable or null.",
                    nameof(dataSource));
        }
    }

    protected void SetGridData(
        DataTable table)
    {
        ArgumentNullException.ThrowIfNull(table);

        ConfigureFlatGridMode();

        // KryptonTreeGridView owns and disposes its DataTable, so bind a copy.
        gridData.DataSource = table.Copy();
        gridData.ExpandAll();
    }

    /// <summary>
    /// Displays a DataTable using explicit parent/child columns.
    /// The identifier columns should contain values that the Krypton
    /// TreeGridView can compare when building the hierarchy.
    /// </summary>
    protected void SetHierarchicalGridData(
        DataTable table,
        string idColumnName,
        string parentIdColumnName,
        bool expandAll = true)
    {
        ArgumentNullException.ThrowIfNull(table);

        if (string.IsNullOrWhiteSpace(idColumnName))
        {
            throw new ArgumentException(
                "An ID column name is required.",
                nameof(idColumnName));
        }

        if (string.IsNullOrWhiteSpace(parentIdColumnName))
        {
            throw new ArgumentException(
                "A parent ID column name is required.",
                nameof(parentIdColumnName));
        }

        if (!table.Columns.Contains(idColumnName))
        {
            throw new ArgumentException(
                $"The table does not contain ID column '{idColumnName}'.",
                nameof(idColumnName));
        }

        if (!table.Columns.Contains(parentIdColumnName))
        {
            throw new ArgumentException(
                $"The table does not contain parent ID column '{parentIdColumnName}'.",
                nameof(parentIdColumnName));
        }

        gridData.UseParentRelationship = true;
        gridData.IdColumnName = idColumnName;
        gridData.ParentIdColumnName = parentIdColumnName;
        gridData.IsOneLevel = false;

        // The control takes ownership of its DataSource.
        gridData.DataSource = table.Copy();

        if (expandAll)
        {
            gridData.ExpandAll();
        }
        else
        {
            gridData.CollapseAll();
        }
    }

    protected void ExpandAllGridNodes()
    {
        gridData.ExpandAll();
    }

    protected void CollapseAllGridNodes()
    {
        gridData.CollapseAll();
    }

    protected void SetNotes(
        string text)
    {
        textNotes.Text = text;
    }

    protected void ClearTemplate()
    {
        tree.Nodes.Clear();
        ClearGridData();
        textNotes.Clear();
    }

    private void ConfigureTemplateDefaults()
    {
        tree.HideSelection = false;

        gridData.AllowUserToAddRows = false;
        gridData.AllowUserToDeleteRows = false;
        gridData.AutoSizeColumnsMode =
            DataGridViewAutoSizeColumnsMode.DisplayedCells;
        gridData.ReadOnly = true;
        gridData.RowHeadersVisible = false;
        gridData.SelectionMode =
            DataGridViewSelectionMode.FullRowSelect;
        gridData.ShowLines = true;
        gridData.FontParentBold = true;

        textNotes.ReadOnly = true;
        textNotes.ScrollBars = ScrollBars.Both;
        textNotes.WordWrap = false;
    }

    private void ConfigureFlatGridMode()
    {
        gridData.UseParentRelationship = false;
        gridData.IsOneLevel = true;
    }

    private void ClearGridData()
    {
        gridData.GridNodes.Clear();
        gridData.Columns.Clear();

        // KryptonTreeGridView assumes DataSource contains at least one column.
        // A zero-column DataTable causes its DataSource setter to access
        // Columns[0] and throw IndexOutOfRangeException.
        DataTable placeholder =
            new("EmptyTreeGrid");

        placeholder.Columns.Add(
            "__Empty",
            typeof(string));

        gridData.DataSource = placeholder;

        if (gridData.Columns.Contains("__Empty"))
        {
            gridData.Columns["__Empty"]!.Visible = false;
        }
    }
}
