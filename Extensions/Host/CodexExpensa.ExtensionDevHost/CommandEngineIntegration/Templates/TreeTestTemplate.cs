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

    protected DataGridView ResultGrid => gridData;

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
        gridData.DataSource = dataSource;
    }

    protected void SetGridData(
        DataTable table)
    {
        ArgumentNullException.ThrowIfNull(table);

        gridData.DataSource = table;
    }

    protected void SetNotes(
        string text)
    {
        textNotes.Text = text;
    }

    protected void ClearTemplate()
    {
        tree.Nodes.Clear();
        gridData.DataSource = null;
        textNotes.Clear();
    }

    private void ConfigureTemplateDefaults()
    {
        tree.HideSelection = false;

        gridData.AllowUserToAddRows = false;
        gridData.AllowUserToDeleteRows = false;
        gridData.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
        gridData.ReadOnly = true;
        gridData.RowHeadersVisible = false;

        textNotes.ReadOnly = true;
        textNotes.ScrollBars = ScrollBars.Both;
        textNotes.WordWrap = false;
    }
}
