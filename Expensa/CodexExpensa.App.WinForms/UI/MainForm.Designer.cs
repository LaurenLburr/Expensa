namespace CodexExpensa.App.WinForms.UI
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.MenuStrip menuMain;
        private System.Windows.Forms.ToolStripMenuItem menuFile;
        private System.Windows.Forms.ToolStripMenuItem menuFileSave;
        private System.Windows.Forms.ToolStripMenuItem menuFileExit;

        private System.Windows.Forms.ToolStripMenuItem menuView;
        private System.Windows.Forms.ToolStripMenuItem menuViewRefresh;

        private System.Windows.Forms.ToolStripMenuItem menuTools;
        private System.Windows.Forms.ToolStripMenuItem menuToolsDbStatus;

        private System.Windows.Forms.ToolStripMenuItem menuHelp;
        private System.Windows.Forms.ToolStripMenuItem menuHelpAbout;

        private System.Windows.Forms.StatusStrip statusMain;
        private System.Windows.Forms.ToolStripStatusLabel statusText;
        private System.Windows.Forms.ToolStripStatusLabel statusDbMode;

        private System.Windows.Forms.SplitContainer splitMain;
        private System.Windows.Forms.TreeView treeNav;
        private System.Windows.Forms.Panel panelHost;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();

            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();

            this.menuMain = new System.Windows.Forms.MenuStrip();
            this.menuFile = new System.Windows.Forms.ToolStripMenuItem();
            this.menuFileSave = new System.Windows.Forms.ToolStripMenuItem();
            this.menuFileExit = new System.Windows.Forms.ToolStripMenuItem();

            this.menuView = new System.Windows.Forms.ToolStripMenuItem();
            this.menuViewRefresh = new System.Windows.Forms.ToolStripMenuItem();

            this.menuTools = new System.Windows.Forms.ToolStripMenuItem();
            this.menuToolsDbStatus = new System.Windows.Forms.ToolStripMenuItem();

            this.menuHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.menuHelpAbout = new System.Windows.Forms.ToolStripMenuItem();

            this.statusMain = new System.Windows.Forms.StatusStrip();
            this.statusText = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusDbMode = new System.Windows.Forms.ToolStripStatusLabel();

            this.splitMain = new System.Windows.Forms.SplitContainer();
            this.treeNav = new System.Windows.Forms.TreeView();
            this.panelHost = new System.Windows.Forms.Panel();

            this.menuMain.SuspendLayout();
            this.statusMain.SuspendLayout();

            ((System.ComponentModel.ISupportInitialize)(this.splitMain)).BeginInit();
            this.splitMain.Panel1.SuspendLayout();
            this.splitMain.Panel2.SuspendLayout();
            this.splitMain.SuspendLayout();

            this.SuspendLayout();

            // 
            // menuMain
            // 
            this.menuMain.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.menuFile,
                this.menuView,
                this.menuTools,
                this.menuHelp
            });
            this.menuMain.Location = new System.Drawing.Point(0, 0);
            this.menuMain.Name = "menuMain";
            this.menuMain.Size = new System.Drawing.Size(1100, 28);
            this.menuMain.TabIndex = 0;

            // 
            // menuFile
            // 
            this.menuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.menuFileSave,
                new System.Windows.Forms.ToolStripSeparator(),
                this.menuFileExit
            });
            this.menuFile.Name = "menuFile";
            this.menuFile.Size = new System.Drawing.Size(46, 24);
            this.menuFile.Text = "&File";

            // 
            // menuFileSave
            // 
            this.menuFileSave.Name = "menuFileSave";
            this.menuFileSave.Size = new System.Drawing.Size(224, 26);
            this.menuFileSave.Text = "&Save";

            // 
            // menuFileExit
            // 
            this.menuFileExit.Name = "menuFileExit";
            this.menuFileExit.Size = new System.Drawing.Size(224, 26);
            this.menuFileExit.Text = "E&xit";

            // 
            // menuView
            // 
            this.menuView.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.menuViewRefresh
            });
            this.menuView.Name = "menuView";
            this.menuView.Size = new System.Drawing.Size(55, 24);
            this.menuView.Text = "&View";

            // 
            // menuViewRefresh
            // 
            this.menuViewRefresh.Name = "menuViewRefresh";
            this.menuViewRefresh.Size = new System.Drawing.Size(224, 26);
            this.menuViewRefresh.Text = "&Refresh";

            // 
            // menuTools
            // 
            this.menuTools.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.menuToolsDbStatus
            });
            this.menuTools.Name = "menuTools";
            this.menuTools.Size = new System.Drawing.Size(58, 24);
            this.menuTools.Text = "&Tools";

            // 
            // menuToolsDbStatus
            // 
            this.menuToolsDbStatus.Name = "menuToolsDbStatus";
            this.menuToolsDbStatus.Size = new System.Drawing.Size(224, 26);
            this.menuToolsDbStatus.Text = "DB &Status...";

            // 
            // menuHelp
            // 
            this.menuHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.menuHelpAbout
            });
            this.menuHelp.Name = "menuHelp";
            this.menuHelp.Size = new System.Drawing.Size(55, 24);
            this.menuHelp.Text = "&Help";

            // 
            // menuHelpAbout
            // 
            this.menuHelpAbout.Name = "menuHelpAbout";
            this.menuHelpAbout.Size = new System.Drawing.Size(224, 26);
            this.menuHelpAbout.Text = "&About";

            // 
            // statusMain
            // 
            this.statusMain.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.statusText,
                this.statusDbMode
            });
            this.statusMain.Location = new System.Drawing.Point(0, 628);
            this.statusMain.Name = "statusMain";
            this.statusMain.Size = new System.Drawing.Size(1100, 22);
            this.statusMain.TabIndex = 2;

            // 
            // statusText
            // 
            this.statusText.Name = "statusText";
            this.statusText.Size = new System.Drawing.Size(1020, 16);
            this.statusText.Spring = true;
            this.statusText.Text = "Ready";

            // 
            // statusDbMode
            // 
            this.statusDbMode.Name = "statusDbMode";
            this.statusDbMode.Size = new System.Drawing.Size(65, 16);
            this.statusDbMode.Text = "DB: ???";

            // 
            // splitMain
            // 
            this.splitMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitMain.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitMain.Location = new System.Drawing.Point(0, 28);
            this.splitMain.Name = "splitMain";
            this.splitMain.Panel1.Controls.Add(this.treeNav);
            this.splitMain.Panel2.Controls.Add(this.panelHost);
            this.splitMain.Size = new System.Drawing.Size(1100, 600);
            this.splitMain.SplitterDistance = 280;
            this.splitMain.TabIndex = 1;

            // 
            // treeNav
            // 
            this.treeNav.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeNav.HideSelection = false;
            this.treeNav.Location = new System.Drawing.Point(0, 0);
            this.treeNav.Name = "treeNav";
            this.treeNav.Size = new System.Drawing.Size(280, 600);
            this.treeNav.TabIndex = 0;

            // 
            // panelHost
            // 
            this.panelHost.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelHost.Location = new System.Drawing.Point(0, 0);
            this.panelHost.Name = "panelHost";
            this.panelHost.Size = new System.Drawing.Size(816, 600);
            this.panelHost.TabIndex = 0;

            // 
            // MainForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1100, 650);
            this.Controls.Add(this.splitMain);
            this.Controls.Add(this.statusMain);
            this.Controls.Add(this.menuMain);
            this.MainMenuStrip = this.menuMain;
            this.Name = "MainForm";
            this.Text = "Expensa";

            this.menuMain.ResumeLayout(false);
            this.menuMain.PerformLayout();

            this.statusMain.ResumeLayout(false);
            this.statusMain.PerformLayout();

            this.splitMain.Panel1.ResumeLayout(false);
            this.splitMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitMain)).EndInit();
            this.splitMain.ResumeLayout(false);

            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion
    }
}