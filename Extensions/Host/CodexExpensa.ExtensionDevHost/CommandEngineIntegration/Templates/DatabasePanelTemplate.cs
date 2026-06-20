namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration;

public partial class DatabasePanelTemplate : Form
{
    private readonly LinkLabel _databaseActionLink = new();
    private Action? _databaseAction;

    public DatabasePanelTemplate()
    {
        InitializeComponent();
        InitializeDatabaseActionLink();
    }

    private void InitializeDatabaseActionLink()
    {
        _databaseActionLink.AutoSize = true;
        _databaseActionLink.Location = new Point(405, 62);
        _databaseActionLink.Name = "linkDatabaseAction";
        _databaseActionLink.TabIndex = 6;
        _databaseActionLink.TabStop = true;
        _databaseActionLink.Visible = false;
        _databaseActionLink.LinkClicked += DatabaseActionLink_LinkClicked;

        panel1.Controls.Add(_databaseActionLink);
    }

    protected void ConfigureDatabaseActionLink(
        string linkText,
        Action action)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(linkText);
        ArgumentNullException.ThrowIfNull(action);

        _databaseAction = action;
        _databaseActionLink.Text = linkText;
        _databaseActionLink.Visible = true;
    }

    private void DatabaseActionLink_LinkClicked(
        object? sender,
        LinkLabelLinkClickedEventArgs e)
    {
        _databaseAction?.Invoke();
    }

    protected void ConfigureDatabaseContextMenus(
        Action reloadAction,
        Action openProdFolderAction,
        Action createOrReplaceDevAction,
        Action openDevFolderAction)
    {
        ArgumentNullException.ThrowIfNull(reloadAction);
        ArgumentNullException.ThrowIfNull(openProdFolderAction);
        ArgumentNullException.ThrowIfNull(createOrReplaceDevAction);
        ArgumentNullException.ThrowIfNull(openDevFolderAction);

        DatabasePanelContextMenuController.Configure(
            ctx_Prod,
            reloadAction,
            openProdFolderAction,
            "Open Prod database folder");

        DatabasePanelContextMenuController.ConfigureDev(
            ctx_Dev,
            reloadAction,
            createOrReplaceDevAction,
            openDevFolderAction);
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
        gridDataView.DataSource = dataSource;
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

    protected virtual void ReloadAddinDatabaseIntoMemory()
    {
        throw CreateUnsupportedOperationException(
            "reloading the add-in database into memory");
    }

    protected virtual void OpenProdDatabaseFolder()
    {
        throw CreateUnsupportedOperationException(
            "opening the Prod database folder");
    }

    protected virtual void CreateOrReplaceDevDatabaseFromAddinDatabase()
    {
        throw CreateUnsupportedOperationException(
            "creating the Dev database from the add-in database");
    }

    protected virtual void OpenDevDatabaseFolder()
    {
        throw CreateUnsupportedOperationException(
            "opening the Dev database folder");
    }

    private NotSupportedException CreateUnsupportedOperationException(
        string operation)
    {
        return new NotSupportedException(
            $"{GetType().Name} does not support {operation}.");
    }


    private void openFolderToolStripMenuItem_Click(
        object sender,
        EventArgs e)
    {
        OpenProdDatabaseFolder();
    }

    private void openFolderToolStripMenuItem1_Click(
        object sender,
        EventArgs e)
    {
        OpenDevDatabaseFolder();
    }

    private void linkDb_filename_LinkClicked(
        object sender,
        LinkLabelLinkClickedEventArgs e)
    {
        OnDatabasePathLinkClicked();
    }

    private void linkUpdate_from_Prod_LinkClicked(
        object sender,
        LinkLabelLinkClickedEventArgs e)
    {
        OnUpdateFromProdClicked();
    }

    private void linkUpdate_from_Dev_LinkClicked(
        object sender,
        LinkLabelLinkClickedEventArgs e)
    {
        OnUpdateFromDevClicked();
    }
}
