namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration;

partial class ExtensionRuntimeDashboardForm
{
    private System.ComponentModel.IContainer components = null;
    private MenuStrip mainMenuStrip;
    private ToolStripMenuItem runtimeMenuItem;
    private ToolStripMenuItem startSmokeMenuItem;
    private ToolStripMenuItem stopRuntimeMenuItem;
    private ToolStripMenuItem refreshSnapshotMenuItem;
    private ToolStripMenuItem discoveryMenuItem;
    private ToolStripMenuItem startLoadedAssembliesMenuItem;
    private ToolStripMenuItem reloadLoadedAssembliesMenuItem;
    private ToolStripMenuItem startDllFolderMenuItem;
    private ToolStripMenuItem reloadDllFolderMenuItem;
    private ToolStripMenuItem startManifestsMenuItem;
    private ToolStripMenuItem reloadManifestsMenuItem;
    private ToolStripMenuItem manifestMenuItem;
    private ToolStripMenuItem discoverRegistryMenuItem;
    private ToolStripMenuItem showDuplicateManifestRowsMenuItem;
    private ToolStripMenuItem enableSelectedManifestMenuItem;
    private ToolStripMenuItem disableSelectedManifestMenuItem;
    private ToolStripMenuItem setManifestDisplaySortMenuItem;
    private ToolStripMenuItem openSelectedManifestMenuItem;
    private ToolStripMenuItem openManifestFolderMenuItem;
    private ToolStripMenuItem copyManifestPathMenuItem;
    private ToolStripMenuItem copyManifestReportMenuItem;
    private ToolStripMenuItem settingsMenuItem;
    private ToolStripMenuItem saveFolderSettingsMenuItem;
    private ToolStripMenuItem reloadFolderSettingsMenuItem;
    private ToolStripMenuItem resetFolderToAppFolderMenuItem;
    private ToolStripMenuItem useModulesFolderMenuItem;
    private ToolStripMenuItem useWebsitesAddinDebugOutputMenuItem;
    private ToolStripMenuItem useWebsitesAddinReleaseOutputMenuItem;
    private ToolStripMenuItem useFirstExistingWebsitesAddinOutputMenuItem;
    private ToolStripMenuItem commandMenuItem;
    private ToolStripMenuItem commandPaletteMenuItem;
    private ToolStripMenuItem executeSelectedMenuItem;
    private ToolStripMenuItem executeSelectedWithParametersMenuItem;
    private ToolStripMenuItem showSelectedMetadataMenuItem;
    private ToolStripMenuItem copySelectedNameMenuItem;
    private ToolStripMenuItem copyDiagnosticsMenuItem;
    private ToolStripMenuItem queueMenuItem;
    private ToolStripMenuItem showExecutionQueueMenuItem;
    private ToolStripMenuItem queueSelectedCommandMenuItem;
    private ToolStripMenuItem queueSelectedCommandWithParametersMenuItem;
    private ToolStripMenuItem cancelSelectedQueueItemMenuItem;
    private ToolStripMenuItem clearCompletedQueueItemsMenuItem;
    private ToolStripMenuItem showSelectedQueueItemDetailsMenuItem;
    private ToolStripMenuItem copySelectedQueueParametersMenuItem;
    private ToolStripMenuItem copySelectedQueueOutputMenuItem;
    private ToolStripMenuItem copyQueueReportMenuItem;
    private ToolStripMenuItem websitesMenuItem;
    private ToolStripMenuItem openWebsitesRuntimeExplorerMenuItem;
    private ToolStripMenuItem persistenceMenuItem;
    private ToolStripMenuItem openPersistentExecutionExplorerMenuItem;
    private ToolStripMenuItem showPersistedExecutionCountsMenuItem;
    private ToolStripMenuItem showPersistedQueueRecordsMenuItem;
    private ToolStripMenuItem showPersistedHistoryRecordsMenuItem;
    private ToolStripMenuItem openExecutionDatabaseMenuItem;
    private ToolStripMenuItem openExecutionDatabaseFolderMenuItem;
    private ToolStripMenuItem copyExecutionDatabasePathMenuItem;
    private ToolStripMenuItem openPersistentExecutionRetentionDialogMenuItem;
    private ToolStripMenuItem purgeFailedPersistedExecutionsMenuItem;
    private ToolStripMenuItem purgePersistedHistoryOlderThan30DaysMenuItem;
    private ToolStripMenuItem vacuumExecutionDatabaseMenuItem;
    private ToolStripMenuItem replaySelectedPersistedExecutionMenuItem;
    private ToolStripMenuItem showSelectedPersistedExecutionDetailsMenuItem;
    private ToolStripMenuItem copySelectedPersistedExecutionParametersMenuItem;
    private ToolStripMenuItem copySelectedPersistedExecutionOutputMenuItem;
    private ToolStripMenuItem historyMenuItem;
    private ToolStripMenuItem showExecutionHistoryMenuItem;
    private ToolStripMenuItem replaySelectedHistoryMenuItem;
    private ToolStripMenuItem showSelectedHistoryDetailsMenuItem;
    private ToolStripMenuItem copySelectedParametersMenuItem;
    private ToolStripMenuItem copySelectedOutputMenuItem;
    private ToolStripMenuItem openHistoryFileMenuItem;
    private ToolStripMenuItem openHistoryFolderMenuItem;
    private ToolStripMenuItem clearExecutionHistoryMenuItem;
    private ToolStripMenuItem copyHistoryReportMenuItem;
    private TableLayoutPanel rootLayoutPanel;
    private TableLayoutPanel folderLayoutPanel;
    private Label folderLabel;
    private TextBox folderTextBox;
    private Button browseFolderButton;
    private CheckBox recursiveCheckBox;
    private Label summaryLabel;
    private Label statusDetailsLabel;
    private ListView commandListView;
    private TextBox diagnosticsTextBox;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }

        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        mainMenuStrip = new MenuStrip();
        runtimeMenuItem = new ToolStripMenuItem();
        startSmokeMenuItem = new ToolStripMenuItem();
        stopRuntimeMenuItem = new ToolStripMenuItem();
        refreshSnapshotMenuItem = new ToolStripMenuItem();
        discoveryMenuItem = new ToolStripMenuItem();
        startLoadedAssembliesMenuItem = new ToolStripMenuItem();
        reloadLoadedAssembliesMenuItem = new ToolStripMenuItem();
        startDllFolderMenuItem = new ToolStripMenuItem();
        reloadDllFolderMenuItem = new ToolStripMenuItem();
        startManifestsMenuItem = new ToolStripMenuItem();
        reloadManifestsMenuItem = new ToolStripMenuItem();
        manifestMenuItem = new ToolStripMenuItem();
        discoverRegistryMenuItem = new ToolStripMenuItem();
        showDuplicateManifestRowsMenuItem = new ToolStripMenuItem();
        enableSelectedManifestMenuItem = new ToolStripMenuItem();
        disableSelectedManifestMenuItem = new ToolStripMenuItem();
        setManifestDisplaySortMenuItem = new ToolStripMenuItem();
        openSelectedManifestMenuItem = new ToolStripMenuItem();
        openManifestFolderMenuItem = new ToolStripMenuItem();
        copyManifestPathMenuItem = new ToolStripMenuItem();
        copyManifestReportMenuItem = new ToolStripMenuItem();
        settingsMenuItem = new ToolStripMenuItem();
        saveFolderSettingsMenuItem = new ToolStripMenuItem();
        reloadFolderSettingsMenuItem = new ToolStripMenuItem();
        resetFolderToAppFolderMenuItem = new ToolStripMenuItem();
        useModulesFolderMenuItem = new ToolStripMenuItem();
        useWebsitesAddinDebugOutputMenuItem = new ToolStripMenuItem();
        useWebsitesAddinReleaseOutputMenuItem = new ToolStripMenuItem();
        useFirstExistingWebsitesAddinOutputMenuItem = new ToolStripMenuItem();
        commandMenuItem = new ToolStripMenuItem();
        commandPaletteMenuItem = new ToolStripMenuItem();
        executeSelectedMenuItem = new ToolStripMenuItem();
        executeSelectedWithParametersMenuItem = new ToolStripMenuItem();
        showSelectedMetadataMenuItem = new ToolStripMenuItem();
        copySelectedNameMenuItem = new ToolStripMenuItem();
        copyDiagnosticsMenuItem = new ToolStripMenuItem();
        queueMenuItem = new ToolStripMenuItem();
        showExecutionQueueMenuItem = new ToolStripMenuItem();
        queueSelectedCommandMenuItem = new ToolStripMenuItem();
        queueSelectedCommandWithParametersMenuItem = new ToolStripMenuItem();
        cancelSelectedQueueItemMenuItem = new ToolStripMenuItem();
        clearCompletedQueueItemsMenuItem = new ToolStripMenuItem();
        showSelectedQueueItemDetailsMenuItem = new ToolStripMenuItem();
        copySelectedQueueParametersMenuItem = new ToolStripMenuItem();
        copySelectedQueueOutputMenuItem = new ToolStripMenuItem();
        copyQueueReportMenuItem = new ToolStripMenuItem();
        persistenceMenuItem = new ToolStripMenuItem();
        openPersistentExecutionExplorerMenuItem = new ToolStripMenuItem();
        showPersistedExecutionCountsMenuItem = new ToolStripMenuItem();
        showPersistedQueueRecordsMenuItem = new ToolStripMenuItem();
        showPersistedHistoryRecordsMenuItem = new ToolStripMenuItem();
        openExecutionDatabaseMenuItem = new ToolStripMenuItem();
        openExecutionDatabaseFolderMenuItem = new ToolStripMenuItem();
        copyExecutionDatabasePathMenuItem = new ToolStripMenuItem();
        openPersistentExecutionRetentionDialogMenuItem = new ToolStripMenuItem();
        purgeFailedPersistedExecutionsMenuItem = new ToolStripMenuItem();
        purgePersistedHistoryOlderThan30DaysMenuItem = new ToolStripMenuItem();
        vacuumExecutionDatabaseMenuItem = new ToolStripMenuItem();
        replaySelectedPersistedExecutionMenuItem = new ToolStripMenuItem();
        showSelectedPersistedExecutionDetailsMenuItem = new ToolStripMenuItem();
        copySelectedPersistedExecutionParametersMenuItem = new ToolStripMenuItem();
        copySelectedPersistedExecutionOutputMenuItem = new ToolStripMenuItem();
        websitesMenuItem = new ToolStripMenuItem();
        openWebsitesRuntimeExplorerMenuItem = new ToolStripMenuItem();
        historyMenuItem = new ToolStripMenuItem();
        showExecutionHistoryMenuItem = new ToolStripMenuItem();
        replaySelectedHistoryMenuItem = new ToolStripMenuItem();
        showSelectedHistoryDetailsMenuItem = new ToolStripMenuItem();
        copySelectedParametersMenuItem = new ToolStripMenuItem();
        copySelectedOutputMenuItem = new ToolStripMenuItem();
        openHistoryFileMenuItem = new ToolStripMenuItem();
        openHistoryFolderMenuItem = new ToolStripMenuItem();
        clearExecutionHistoryMenuItem = new ToolStripMenuItem();
        copyHistoryReportMenuItem = new ToolStripMenuItem();
        rootLayoutPanel = new TableLayoutPanel();
        folderLayoutPanel = new TableLayoutPanel();
        folderLabel = new Label();
        folderTextBox = new TextBox();
        browseFolderButton = new Button();
        recursiveCheckBox = new CheckBox();
        summaryLabel = new Label();
        statusDetailsLabel = new Label();
        commandListView = new ListView();
        diagnosticsTextBox = new TextBox();

        mainMenuStrip.SuspendLayout();
        rootLayoutPanel.SuspendLayout();
        folderLayoutPanel.SuspendLayout();
        SuspendLayout();

        mainMenuStrip.Items.AddRange(new ToolStripItem[]
        {
            runtimeMenuItem,
            discoveryMenuItem,
            manifestMenuItem,
            settingsMenuItem,
            commandMenuItem,
            queueMenuItem,
            extensionManagerMenuItem,
            websitesMenuItem,
            persistenceMenuItem,
            historyMenuItem
        });
        mainMenuStrip.Location = new Point(0, 0);
        mainMenuStrip.Name = "mainMenuStrip";
        mainMenuStrip.Size = new Size(1300, 24);
        mainMenuStrip.TabIndex = 0;

        runtimeMenuItem.Text = "&Runtime";
        runtimeMenuItem.DropDownItems.AddRange(new ToolStripItem[]
        {
            startSmokeMenuItem,
            stopRuntimeMenuItem,
            new ToolStripSeparator(),
            refreshSnapshotMenuItem
        });
        startSmokeMenuItem.Text = "Start &Smoke";
        startSmokeMenuItem.Click += StartSmokeRuntime;
        stopRuntimeMenuItem.Text = "&Stop";
        stopRuntimeMenuItem.Click += StopRuntime;
        refreshSnapshotMenuItem.Text = "&Refresh Snapshot";
        refreshSnapshotMenuItem.Click += RefreshSnapshot;

        discoveryMenuItem.Text = "&Discovery";
        discoveryMenuItem.DropDownItems.AddRange(new ToolStripItem[]
        {
            startLoadedAssembliesMenuItem,
            reloadLoadedAssembliesMenuItem,
            new ToolStripSeparator(),
            startDllFolderMenuItem,
            reloadDllFolderMenuItem,
            new ToolStripSeparator(),
            startManifestsMenuItem,
            reloadManifestsMenuItem
        });
        startLoadedAssembliesMenuItem.Text = "Start &Loaded Assemblies";
        startLoadedAssembliesMenuItem.Click += StartFromLoadedAssemblies;
        reloadLoadedAssembliesMenuItem.Text = "Reload Loaded &Assemblies";
        reloadLoadedAssembliesMenuItem.Click += ReloadFromLoadedAssemblies;
        startDllFolderMenuItem.Text = "Start &DLL Folder";
        startDllFolderMenuItem.Click += StartFromFolder;
        reloadDllFolderMenuItem.Text = "Reload D&LL Folder";
        reloadDllFolderMenuItem.Click += ReloadFromFolder;
        startManifestsMenuItem.Text = "Start &Manifests";
        startManifestsMenuItem.Click += StartFromManifests;
        reloadManifestsMenuItem.Text = "Reload Mani&fests";
        reloadManifestsMenuItem.Click += ReloadFromManifests;

        manifestMenuItem.Text = "&Manifest";
        manifestMenuItem.DropDownItems.AddRange(new ToolStripItem[]
        {
            discoverRegistryMenuItem,
            showDuplicateManifestRowsMenuItem,
            new ToolStripSeparator(),
            enableSelectedManifestMenuItem,
            disableSelectedManifestMenuItem,
            setManifestDisplaySortMenuItem,
            new ToolStripSeparator(),
            openSelectedManifestMenuItem,
            openManifestFolderMenuItem,
            copyManifestPathMenuItem,
            new ToolStripSeparator(),
            copyManifestReportMenuItem
        });
        discoverRegistryMenuItem.Text = "&Discover Registry";
        discoverRegistryMenuItem.Click += DiscoverManifestRegistry;
        showDuplicateManifestRowsMenuItem.Text = "Show &Duplicate Manifest Rows";
        showDuplicateManifestRowsMenuItem.CheckOnClick = true;
        showDuplicateManifestRowsMenuItem.CheckedChanged += ShowDuplicateManifestRowsMenuItem_CheckedChanged;
        enableSelectedManifestMenuItem.Text = "&Enable Selected Manifest";
        enableSelectedManifestMenuItem.Click += EnableSelectedManifest;
        disableSelectedManifestMenuItem.Text = "&Disable Selected Manifest";
        disableSelectedManifestMenuItem.Click += DisableSelectedManifest;
        setManifestDisplaySortMenuItem.Text = "Set Display &Sort...";
        setManifestDisplaySortMenuItem.Click += SetSelectedManifestDisplaySort;
        openSelectedManifestMenuItem.Text = "&Open Selected Manifest";
        openSelectedManifestMenuItem.Click += OpenSelectedManifest;
        openManifestFolderMenuItem.Text = "Open Manifest &Folder";
        openManifestFolderMenuItem.Click += OpenSelectedManifestFolder;
        copyManifestPathMenuItem.Text = "Copy Manifest &Path";
        copyManifestPathMenuItem.Click += CopySelectedManifestPath;
        copyManifestReportMenuItem.Text = "Copy Manifest &Report";
        copyManifestReportMenuItem.Click += CopyDiagnostics;

        settingsMenuItem.Text = "&Settings";
        settingsMenuItem.DropDownItems.AddRange(new ToolStripItem[]
        {
            saveFolderSettingsMenuItem,
            reloadFolderSettingsMenuItem,
            resetFolderToAppFolderMenuItem,
            new ToolStripSeparator(),
            useModulesFolderMenuItem,
            useWebsitesAddinDebugOutputMenuItem,
            useWebsitesAddinReleaseOutputMenuItem,
            useFirstExistingWebsitesAddinOutputMenuItem
        });
        saveFolderSettingsMenuItem.Text = "&Save Folder Settings";
        saveFolderSettingsMenuItem.Click += SaveDashboardSettingsMenu;
        reloadFolderSettingsMenuItem.Text = "&Reload Folder Settings";
        reloadFolderSettingsMenuItem.Click += ReloadDashboardSettingsMenu;
        resetFolderToAppFolderMenuItem.Text = "&Reset Folder To App Folder";
        resetFolderToAppFolderMenuItem.Click += ResetFolderToAppFolder;
        useModulesFolderMenuItem.Text = "Use &Modules Folder";
        useModulesFolderMenuItem.Click += UseModulesFolder;
        useWebsitesAddinDebugOutputMenuItem.Text = "Use WebsitesAddin &Debug Output";
        useWebsitesAddinDebugOutputMenuItem.Click += UseWebsitesAddinDebugOutput;
        useWebsitesAddinReleaseOutputMenuItem.Text = "Use WebsitesAddin &Release Output";
        useWebsitesAddinReleaseOutputMenuItem.Click += UseWebsitesAddinReleaseOutput;
        useFirstExistingWebsitesAddinOutputMenuItem.Text = "Use &First Existing WebsitesAddin Output";
        useFirstExistingWebsitesAddinOutputMenuItem.Click += UseFirstExistingWebsitesAddinOutput;

        commandMenuItem.Text = "&Command";
        commandMenuItem.DropDownItems.AddRange(new ToolStripItem[]
        {
            commandPaletteMenuItem,
            new ToolStripSeparator(),
            executeSelectedMenuItem,
            executeSelectedWithParametersMenuItem,
            showSelectedMetadataMenuItem,
            copySelectedNameMenuItem,
            copyDiagnosticsMenuItem
        });
        commandPaletteMenuItem.Text = "Command &Palette";
        commandPaletteMenuItem.ShortcutKeys = Keys.Control | Keys.P;
        commandPaletteMenuItem.Click += ShowCommandPalette;
        executeSelectedMenuItem.Text = "&Execute Selected";
        executeSelectedMenuItem.Click += ExecuteSelectedCommandAsync;
        executeSelectedWithParametersMenuItem.Text = "Execute Selected With &Parameters";
        executeSelectedWithParametersMenuItem.Click += ExecuteSelectedCommandWithParameters;
        showSelectedMetadataMenuItem.Text = "Show Selected &Metadata";
        showSelectedMetadataMenuItem.Click += ShowSelectedCommandMetadata;
        copySelectedNameMenuItem.Text = "&Copy Selected Name";
        copySelectedNameMenuItem.Click += CopySelectedCommandName;
        copyDiagnosticsMenuItem.Text = "Copy &Diagnostics";
        copyDiagnosticsMenuItem.Click += CopyDiagnostics;

        queueMenuItem.Text = "&Queue";
        queueMenuItem.DropDownItems.AddRange(new ToolStripItem[]
        {
            showExecutionQueueMenuItem,
            new ToolStripSeparator(),
            queueSelectedCommandMenuItem,
            queueSelectedCommandWithParametersMenuItem,
            new ToolStripSeparator(),
            cancelSelectedQueueItemMenuItem,
            clearCompletedQueueItemsMenuItem,
            new ToolStripSeparator(),
            showSelectedQueueItemDetailsMenuItem,
            copySelectedQueueParametersMenuItem,
            copySelectedQueueOutputMenuItem,
            copyQueueReportMenuItem
        });
        showExecutionQueueMenuItem.Text = "&Show Execution Queue";
        showExecutionQueueMenuItem.Click += ShowExecutionQueue;
        queueSelectedCommandMenuItem.Text = "&Queue Selected Command";
        queueSelectedCommandMenuItem.Click += QueueSelectedCommand;
        queueSelectedCommandWithParametersMenuItem.Text = "Queue Selected Command With &Parameters";
        queueSelectedCommandWithParametersMenuItem.Click += QueueSelectedCommandWithParameters;
        cancelSelectedQueueItemMenuItem.Text = "&Cancel Selected Queue Item";
        cancelSelectedQueueItemMenuItem.Click += CancelSelectedQueueItem;
        clearCompletedQueueItemsMenuItem.Text = "Clear &Completed Queue Items";
        clearCompletedQueueItemsMenuItem.Click += ClearCompletedQueueItems;
        showSelectedQueueItemDetailsMenuItem.Text = "Show Selected Queue Item &Details";
        showSelectedQueueItemDetailsMenuItem.Click += ShowSelectedQueueItemDetails;
        copySelectedQueueParametersMenuItem.Text = "Copy Selected Queue &Parameters";
        copySelectedQueueParametersMenuItem.Click += CopySelectedQueueParameters;
        copySelectedQueueOutputMenuItem.Text = "Copy Selected Queue &Output";
        copySelectedQueueOutputMenuItem.Click += CopySelectedQueueOutput;
        copyQueueReportMenuItem.Text = "Copy Queue &Report";
        copyQueueReportMenuItem.Click += CopyQueueReport;

        persistenceMenuItem.Text = "&Persistence";
        persistenceMenuItem.DropDownItems.AddRange(new ToolStripItem[]
        {
            openPersistentExecutionExplorerMenuItem,
            new ToolStripSeparator(),
            showPersistedExecutionCountsMenuItem,
            showPersistedQueueRecordsMenuItem,
            showPersistedHistoryRecordsMenuItem,
            new ToolStripSeparator(),
            openExecutionDatabaseMenuItem,
            openExecutionDatabaseFolderMenuItem,
            copyExecutionDatabasePathMenuItem,
            new ToolStripSeparator(),
            replaySelectedPersistedExecutionMenuItem,
            showSelectedPersistedExecutionDetailsMenuItem,
            copySelectedPersistedExecutionParametersMenuItem,
            copySelectedPersistedExecutionOutputMenuItem
        });
        openPersistentExecutionExplorerMenuItem.Text = "Open Persistent Execution &Explorer";
        openPersistentExecutionExplorerMenuItem.Click += OpenPersistentExecutionExplorer;
        showPersistedExecutionCountsMenuItem.Text = "Show Persisted Execution &Counts";
        showPersistedExecutionCountsMenuItem.Click += ShowPersistedExecutionCounts;
        showPersistedQueueRecordsMenuItem.Text = "Show Persisted &Queue Records";
        showPersistedQueueRecordsMenuItem.Click += ShowPersistedQueueRecords;
        showPersistedHistoryRecordsMenuItem.Text = "Show Persisted &History Records";
        showPersistedHistoryRecordsMenuItem.Click += ShowPersistedHistoryRecords;
        openExecutionDatabaseMenuItem.Text = "Open Execution &Database";
        openExecutionDatabaseMenuItem.Click += OpenExecutionDatabase;
        openExecutionDatabaseFolderMenuItem.Text = "Open Execution Database &Folder";
        openExecutionDatabaseFolderMenuItem.Click += OpenExecutionDatabaseFolder;
        copyExecutionDatabasePathMenuItem.Text = "Copy Execution Database &Path";
        copyExecutionDatabasePathMenuItem.Click += CopyExecutionDatabasePath;
        replaySelectedPersistedExecutionMenuItem.Text = "Replay Selected Persisted &Execution";
        replaySelectedPersistedExecutionMenuItem.Click += ReplaySelectedPersistedExecution;
        showSelectedPersistedExecutionDetailsMenuItem.Text = "Show Selected Persisted Execution &Details";
        showSelectedPersistedExecutionDetailsMenuItem.Click += ShowSelectedPersistedExecutionDetails;
        copySelectedPersistedExecutionParametersMenuItem.Text = "Copy Selected Persisted &Parameters";
        copySelectedPersistedExecutionParametersMenuItem.Click += CopySelectedPersistedExecutionParameters;
        copySelectedPersistedExecutionOutputMenuItem.Text = "Copy Selected Persisted &Output";
        copySelectedPersistedExecutionOutputMenuItem.Click += CopySelectedPersistedExecutionOutput;
        openPersistentExecutionRetentionDialogMenuItem.Text = "Open Retention &Manager";
        openPersistentExecutionRetentionDialogMenuItem.Click += OpenPersistentExecutionRetentionDialog;
        purgeFailedPersistedExecutionsMenuItem.Text = "Purge &Failed Persisted Executions";
        purgeFailedPersistedExecutionsMenuItem.Click += PurgeFailedPersistedExecutions;
        purgePersistedHistoryOlderThan30DaysMenuItem.Text = "Purge History Older Than &30 Days";
        purgePersistedHistoryOlderThan30DaysMenuItem.Click += PurgePersistedHistoryOlderThan30Days;
        vacuumExecutionDatabaseMenuItem.Text = "&Vacuum Execution Database";
        vacuumExecutionDatabaseMenuItem.Click += VacuumExecutionDatabase;
        extensionManagerMenuItem.Text = "Extension &Mgr";
        extensionManagerMenuItem.DropDownItems.AddRange(new ToolStripItem[]
        {
            openExtensionManagerAddinTestSurfaceMenuItem
        });
        openExtensionManagerAddinTestSurfaceMenuItem.Text = "Open Add-in &Test Surface";
        openExtensionManagerAddinTestSurfaceMenuItem.Click += OpenExtensionManagerAddinTestSurface;

        websitesMenuItem.Text = "&Websites";
        websitesMenuItem.DropDownItems.AddRange(new ToolStripItem[]
        {
            openWebsitesRuntimeExplorerMenuItem
        });
        openWebsitesRuntimeExplorerMenuItem.Text = "Open Websites Runtime &Explorer";
        openWebsitesRuntimeExplorerMenuItem.Click += OpenWebsitesRuntimeExplorer;
        historyMenuItem.Text = "&History";
        historyMenuItem.DropDownItems.AddRange(new ToolStripItem[]
        {
            showExecutionHistoryMenuItem,
            replaySelectedHistoryMenuItem,
            showSelectedHistoryDetailsMenuItem,
            new ToolStripSeparator(),
            copySelectedParametersMenuItem,
            copySelectedOutputMenuItem,
            new ToolStripSeparator(),
            openHistoryFileMenuItem,
            openHistoryFolderMenuItem,
            new ToolStripSeparator(),
            clearExecutionHistoryMenuItem,
            copyHistoryReportMenuItem
        });
        showExecutionHistoryMenuItem.Text = "&Show Execution History";
        showExecutionHistoryMenuItem.Click += ShowExecutionHistory;
        replaySelectedHistoryMenuItem.Text = "&Replay Selected History";
        replaySelectedHistoryMenuItem.Click += ReplaySelectedHistory;
        showSelectedHistoryDetailsMenuItem.Text = "Show Selected History &Details";
        showSelectedHistoryDetailsMenuItem.Click += ShowSelectedHistoryDetails;
        copySelectedParametersMenuItem.Text = "Copy Selected &Parameters";
        copySelectedParametersMenuItem.Click += CopySelectedHistoryParameters;
        copySelectedOutputMenuItem.Text = "Copy Selected &Output";
        copySelectedOutputMenuItem.Click += CopySelectedHistoryOutput;
        openHistoryFileMenuItem.Text = "Open History &File";
        openHistoryFileMenuItem.Click += OpenHistoryFile;
        openHistoryFolderMenuItem.Text = "Open History F&older";
        openHistoryFolderMenuItem.Click += OpenHistoryFolder;
        clearExecutionHistoryMenuItem.Text = "&Clear Execution History";
        clearExecutionHistoryMenuItem.Click += ClearExecutionHistory;
        copyHistoryReportMenuItem.Text = "Copy History &Report";
        copyHistoryReportMenuItem.Click += CopyDiagnostics;

        rootLayoutPanel.ColumnCount = 1;
        rootLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        rootLayoutPanel.Controls.Add(folderLayoutPanel, 0, 0);
        rootLayoutPanel.Controls.Add(summaryLabel, 0, 1);
        rootLayoutPanel.Controls.Add(statusDetailsLabel, 0, 2);
        rootLayoutPanel.Controls.Add(commandListView, 0, 3);
        rootLayoutPanel.Controls.Add(diagnosticsTextBox, 0, 4);
        rootLayoutPanel.Dock = DockStyle.Fill;
        rootLayoutPanel.Location = new Point(8, 32);
        rootLayoutPanel.Name = "rootLayoutPanel";
        rootLayoutPanel.RowCount = 5;
        rootLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 36F));
        rootLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 28F));
        rootLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 24F));
        rootLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 55F));
        rootLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 45F));
        rootLayoutPanel.Size = new Size(1284, 760);
        rootLayoutPanel.TabIndex = 1;

        folderLayoutPanel.ColumnCount = 4;
        folderLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 90F));
        folderLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        folderLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 90F));
        folderLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100F));
        folderLayoutPanel.Controls.Add(folderLabel, 0, 0);
        folderLayoutPanel.Controls.Add(folderTextBox, 1, 0);
        folderLayoutPanel.Controls.Add(browseFolderButton, 2, 0);
        folderLayoutPanel.Controls.Add(recursiveCheckBox, 3, 0);
        folderLayoutPanel.Dock = DockStyle.Fill;
        folderLayoutPanel.Name = "folderLayoutPanel";
        folderLayoutPanel.RowCount = 1;
        folderLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

        folderLabel.Dock = DockStyle.Fill;
        folderLabel.Text = "Folder:";
        folderLabel.TextAlign = ContentAlignment.MiddleLeft;
        folderTextBox.Dock = DockStyle.Fill;
        folderTextBox.Margin = new Padding(3, 7, 3, 3);
        folderTextBox.Text = AppContext.BaseDirectory;
        folderTextBox.Leave += FolderTextBox_Leave;
        browseFolderButton.Dock = DockStyle.Fill;
        browseFolderButton.Text = "Browse...";
        browseFolderButton.Click += BrowseFolderButton_Click;
        recursiveCheckBox.Dock = DockStyle.Fill;
        recursiveCheckBox.Text = "Recursive";
        recursiveCheckBox.TextAlign = ContentAlignment.MiddleLeft;
        recursiveCheckBox.CheckedChanged += RecursiveCheckBox_CheckedChanged;

        summaryLabel.Dock = DockStyle.Fill;
        summaryLabel.Text = "Runtime not started.";
        summaryLabel.TextAlign = ContentAlignment.MiddleLeft;
        statusDetailsLabel.Dock = DockStyle.Fill;
        statusDetailsLabel.Text = "Mode: Runtime";
        statusDetailsLabel.TextAlign = ContentAlignment.MiddleLeft;

        commandListView.Dock = DockStyle.Fill;
        commandListView.FullRowSelect = true;
        commandListView.MultiSelect = false;
        commandListView.UseCompatibleStateImageBehavior = false;
        commandListView.View = View.Details;
        commandListView.DoubleClick += CommandList_DoubleClick;

        diagnosticsTextBox.Dock = DockStyle.Fill;
        diagnosticsTextBox.Multiline = true;
        diagnosticsTextBox.ReadOnly = true;
        diagnosticsTextBox.ScrollBars = ScrollBars.Both;
        diagnosticsTextBox.WordWrap = false;

        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1300, 800);
        Controls.Add(rootLayoutPanel);
        Controls.Add(mainMenuStrip);
        MainMenuStrip = mainMenuStrip;
        Name = "ExtensionRuntimeDashboardForm";
        Padding = new Padding(8);
        StartPosition = FormStartPosition.CenterParent;
        Text = "CommandEngine Runtime";
        FormClosing += ExtensionRuntimeDashboardForm_FormClosing;

        mainMenuStrip.ResumeLayout(false);
        mainMenuStrip.PerformLayout();
        rootLayoutPanel.ResumeLayout(false);
        rootLayoutPanel.PerformLayout();
        folderLayoutPanel.ResumeLayout(false);
        folderLayoutPanel.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }
}






