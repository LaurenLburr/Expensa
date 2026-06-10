using CodexExpensa.ExtensionDevHost.CommandEngineIntegration.ExtensionManager;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration;

public partial class DatabasePanelTemplate : Form
{
    public DatabasePanelTemplate()
    {
        InitializeComponent();
        SafeDataGridViewBinding.Attach(gridDataView);
    }

    protected void ConfigureDatabasePanel(
        string addinName,
        string pageTitle,
        string databaseDisplayName,
        string databasePath)
    {
        Text = pageTitle;
        labelAdd_in_Name.Text = addinName;
        label1.Text = pageTitle;
        labelDatabase_file_Name.Text = databaseDisplayName;
        linkDb_filename.Text = databasePath;
    }

    protected ToolStrip DiagnosticsToolStrip =>
        diagnosticsToolStrip;

    protected void ClearDiagnosticsToolStrip()
    {
        diagnosticsToolStrip.Items.Clear();
    }

    protected void AddDiagnosticsToolStripItem(ToolStripItem item)
    {
        ArgumentNullException.ThrowIfNull(item);

        diagnosticsToolStrip.Items.Add(item);
    }

    protected void SetSummaryText(string text)
    {
        text_Data_.Text = text;
    }

    protected void SetGridDataSource(object? dataSource)
    {
        gridDataView.DataSource = SafeDataGridViewBinding.Sanitize(dataSource);
    }

    protected void SetRowStatus(string text)
    {
        labelNumRows.Text = text;
    }

    protected string DatabasePathLinkText =>
        linkDb_filename.Text;

    protected virtual void OnDatabasePathLinkClicked()
    {
    }

    protected virtual void OnUpdateFromProdClicked()
    {
    }

    protected virtual void OnUpdateFromDevClicked()
    {
    }

    private void linkDb_filename_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
        OnDatabasePathLinkClicked();
    }

    private void linkUpdate_from_Prod_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
        OnUpdateFromProdClicked();
    }

    private void linkUpdate_from_Dev_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
        OnUpdateFromDevClicked();
    }
}
