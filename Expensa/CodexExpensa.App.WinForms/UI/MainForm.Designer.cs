namespace CodexExpensa.App.WinForms.UI;

partial class MainForm
{
    private System.ComponentModel.IContainer components = null;

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
        menuStrip1 = new System.Windows.Forms.MenuStrip();
        menuFile = new System.Windows.Forms.ToolStripMenuItem();
        menuFileSave = new System.Windows.Forms.ToolStripMenuItem();
        menuFileExit = new System.Windows.Forms.ToolStripMenuItem();
        menuView = new System.Windows.Forms.ToolStripMenuItem();
        menuViewRefresh = new System.Windows.Forms.ToolStripMenuItem();
        menuTools = new System.Windows.Forms.ToolStripMenuItem();
        menuToolsDbStatus = new System.Windows.Forms.ToolStripMenuItem();
        diagnosticsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        queryCatalogToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        budgetMonthsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        menuHelp = new System.Windows.Forms.ToolStripMenuItem();
        menuHelpAbout = new System.Windows.Forms.ToolStripMenuItem();
        splitContainer1 = new System.Windows.Forms.SplitContainer();
        treeNav = new System.Windows.Forms.TreeView();
        panelHost = new System.Windows.Forms.Panel();
        statusStrip1 = new System.Windows.Forms.StatusStrip();
        statusText = new System.Windows.Forms.ToolStripStatusLabel();
        statusDbMode = new System.Windows.Forms.ToolStripStatusLabel();
        menuStrip1.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
        splitContainer1.Panel1.SuspendLayout();
        splitContainer1.Panel2.SuspendLayout();
        splitContainer1.SuspendLayout();
        statusStrip1.SuspendLayout();
        SuspendLayout();
        // 
        // menuStrip1
        // 
        menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { menuFile, menuView, menuTools, menuHelp });
        menuStrip1.Location = new System.Drawing.Point(0, 0);
        menuStrip1.Name = "menuStrip1";
        menuStrip1.Size = new System.Drawing.Size(1264, 24);
        menuStrip1.TabIndex = 0;
        menuStrip1.Text = "menuStrip1";
        // 
        // menuFile
        // 
        menuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { menuFileSave, menuFileExit });
        menuFile.Name = "menuFile";
        menuFile.Size = new System.Drawing.Size(37, 20);
        menuFile.Text = "&File";
        // 
        // menuFileSave
        // 
        menuFileSave.Name = "menuFileSave";
        menuFileSave.Size = new System.Drawing.Size(98, 22);
        menuFileSave.Text = "&Save";
        // 
        // menuFileExit
        // 
        menuFileExit.Name = "menuFileExit";
        menuFileExit.Size = new System.Drawing.Size(98, 22);
        menuFileExit.Text = "E&xit";
        // 
        // menuView
        // 
        menuView.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { menuViewRefresh });
        menuView.Name = "menuView";
        menuView.Size = new System.Drawing.Size(44, 20);
        menuView.Text = "&View";
        // 
        // menuViewRefresh
        // 
        menuViewRefresh.Name = "menuViewRefresh";
        menuViewRefresh.Size = new System.Drawing.Size(113, 22);
        menuViewRefresh.Text = "&Refresh";
        // 
        // menuTools
        // 
        menuTools.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { menuToolsDbStatus, diagnosticsToolStripMenuItem, queryCatalogToolStripMenuItem, budgetMonthsToolStripMenuItem });
        menuTools.Name = "menuTools";
        menuTools.Size = new System.Drawing.Size(46, 20);
        menuTools.Text = "&Tools";
        // 
        // menuToolsDbStatus
        // 
        menuToolsDbStatus.Name = "menuToolsDbStatus";
        menuToolsDbStatus.Size = new System.Drawing.Size(161, 22);
        menuToolsDbStatus.Text = "&DB Status";
        // 
        // diagnosticsToolStripMenuItem
        // 
        diagnosticsToolStripMenuItem.Name = "diagnosticsToolStripMenuItem";
        diagnosticsToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
        diagnosticsToolStripMenuItem.Text = "&Diagnostics";
        diagnosticsToolStripMenuItem.Click += diagnosticsToolStripMenuItem_Click;
        // 
        // queryCatalogToolStripMenuItem
        // 
        queryCatalogToolStripMenuItem.Name = "queryCatalogToolStripMenuItem";
        queryCatalogToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
        queryCatalogToolStripMenuItem.Text = "&Query Catalog";
        queryCatalogToolStripMenuItem.Click += queryCatalogToolStripMenuItem_Click;
        // 
        // budgetMonthsToolStripMenuItem
        // 
        budgetMonthsToolStripMenuItem.Name = "budgetMonthsToolStripMenuItem";
        budgetMonthsToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
        budgetMonthsToolStripMenuItem.Text = "&Budget Months";
        budgetMonthsToolStripMenuItem.Click += budgetMonthsToolStripMenuItem_Click;
        // 
        // menuHelp
        // 
        menuHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { menuHelpAbout });
        menuHelp.Name = "menuHelp";
        menuHelp.Size = new System.Drawing.Size(44, 20);
        menuHelp.Text = "&Help";
        // 
        // menuHelpAbout
        // 
        menuHelpAbout.Name = "menuHelpAbout";
        menuHelpAbout.Size = new System.Drawing.Size(107, 22);
        menuHelpAbout.Text = "&About";
        menuHelpAbout.Click += menuHelpAbout_Click_1;
        // 
        // splitContainer1
        // 
        splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
        splitContainer1.Location = new System.Drawing.Point(0, 24);
        splitContainer1.Name = "splitContainer1";
        // 
        // splitContainer1.Panel1
        // 
        splitContainer1.Panel1.Controls.Add(treeNav);
        // 
        // splitContainer1.Panel2
        // 
        splitContainer1.Panel2.Controls.Add(panelHost);
        splitContainer1.Size = new System.Drawing.Size(1264, 659);
        splitContainer1.SplitterDistance = 280;
        splitContainer1.TabIndex = 1;
        // 
        // treeNav
        // 
        treeNav.Dock = System.Windows.Forms.DockStyle.Fill;
        treeNav.HideSelection = false;
        treeNav.Location = new System.Drawing.Point(0, 0);
        treeNav.Name = "treeNav";
        treeNav.Size = new System.Drawing.Size(280, 659);
        treeNav.TabIndex = 0;
        // 
        // panelHost
        // 
        panelHost.Dock = System.Windows.Forms.DockStyle.Fill;
        panelHost.Location = new System.Drawing.Point(0, 0);
        panelHost.Name = "panelHost";
        panelHost.Size = new System.Drawing.Size(980, 659);
        panelHost.TabIndex = 0;
        // 
        // statusStrip1
        // 
        statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { statusText, statusDbMode });
        statusStrip1.Location = new System.Drawing.Point(0, 683);
        statusStrip1.Name = "statusStrip1";
        statusStrip1.Size = new System.Drawing.Size(1264, 22);
        statusStrip1.TabIndex = 2;
        statusStrip1.Text = "statusStrip1";
        // 
        // statusText
        // 
        statusText.Name = "statusText";
        statusText.Size = new System.Drawing.Size(39, 17);
        statusText.Text = "Ready";
        statusText.Spring = true;
        statusText.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
        // 
        // statusDbMode
        // 
        statusDbMode.Name = "statusDbMode";
        statusDbMode.Size = new System.Drawing.Size(55, 17);
        statusDbMode.Text = "DB: ???";
        // 
        // MainForm
        // 
        AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
        AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        ClientSize = new System.Drawing.Size(1264, 705);
        Controls.Add(splitContainer1);
        Controls.Add(statusStrip1);
        Controls.Add(menuStrip1);
        MainMenuStrip = menuStrip1;
        Name = "MainForm";
        StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
        Text = "CodexExpensa";
        menuStrip1.ResumeLayout(false);
        menuStrip1.PerformLayout();
        splitContainer1.Panel1.ResumeLayout(false);
        splitContainer1.Panel2.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
        splitContainer1.ResumeLayout(false);
        statusStrip1.ResumeLayout(false);
        statusStrip1.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }

    private System.Windows.Forms.MenuStrip menuStrip1;
    private System.Windows.Forms.ToolStripMenuItem menuFile;
    private System.Windows.Forms.ToolStripMenuItem menuFileSave;
    private System.Windows.Forms.ToolStripMenuItem menuFileExit;
    private System.Windows.Forms.ToolStripMenuItem menuView;
    private System.Windows.Forms.ToolStripMenuItem menuViewRefresh;
    private System.Windows.Forms.ToolStripMenuItem menuTools;
    private System.Windows.Forms.ToolStripMenuItem menuToolsDbStatus;
    private System.Windows.Forms.ToolStripMenuItem diagnosticsToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem queryCatalogToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem budgetMonthsToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem menuHelp;
    private System.Windows.Forms.ToolStripMenuItem menuHelpAbout;
    private System.Windows.Forms.SplitContainer splitContainer1;
    private System.Windows.Forms.TreeView treeNav;
    private System.Windows.Forms.Panel panelHost;
    private System.Windows.Forms.StatusStrip statusStrip1;
    private System.Windows.Forms.ToolStripStatusLabel statusText;
    private System.Windows.Forms.ToolStripStatusLabel statusDbMode;
}
