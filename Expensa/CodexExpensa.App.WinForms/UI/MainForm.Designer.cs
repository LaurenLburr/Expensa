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
            menuMain = new MenuStrip();
            menuFile = new ToolStripMenuItem();
            menuFileSave = new ToolStripMenuItem();
            menuFileExit = new ToolStripMenuItem();
            menuView = new ToolStripMenuItem();
            menuViewRefresh = new ToolStripMenuItem();
            menuTools = new ToolStripMenuItem();
            menuToolsDbStatus = new ToolStripMenuItem();
            menuHelp = new ToolStripMenuItem();
            menuHelpAbout = new ToolStripMenuItem();
            statusMain = new StatusStrip();
            statusText = new ToolStripStatusLabel();
            statusDbMode = new ToolStripStatusLabel();
            splitMain = new SplitContainer();
            treeNav = new TreeView();
            panelHost = new Panel();
            menuMain.SuspendLayout();
            statusMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitMain).BeginInit();
            splitMain.Panel1.SuspendLayout();
            splitMain.Panel2.SuspendLayout();
            splitMain.SuspendLayout();
            SuspendLayout();
            // 
            // menuMain
            // 
            menuMain.ImageScalingSize = new Size(20, 20);
            menuMain.Items.AddRange(new ToolStripItem[] { menuFile, menuView, menuTools, menuHelp });
            menuMain.Location = new Point(0, 0);
            menuMain.Name = "menuMain";
            menuMain.Size = new Size(1100, 24);
            menuMain.TabIndex = 0;
            // 
            // menuFile
            // 
            menuFile.DropDownItems.AddRange(new ToolStripItem[] { menuFileSave, menuFileExit });
            menuFile.Name = "menuFile";
            menuFile.Size = new Size(37, 20);
            menuFile.Text = "&File";
            // 
            // menuFileSave
            // 
            menuFileSave.Name = "menuFileSave";
            menuFileSave.Size = new Size(98, 22);
            menuFileSave.Text = "&Save";
            // 
            // menuFileExit
            // 
            menuFileExit.Name = "menuFileExit";
            menuFileExit.Size = new Size(98, 22);
            menuFileExit.Text = "E&xit";
            // 
            // menuView
            // 
            menuView.DropDownItems.AddRange(new ToolStripItem[] { menuViewRefresh });
            menuView.Name = "menuView";
            menuView.Size = new Size(44, 20);
            menuView.Text = "&View";
            // 
            // menuViewRefresh
            // 
            menuViewRefresh.Name = "menuViewRefresh";
            menuViewRefresh.Size = new Size(113, 22);
            menuViewRefresh.Text = "&Refresh";
            // 
            // menuTools
            // 
            menuTools.DropDownItems.AddRange(new ToolStripItem[] { menuToolsDbStatus });
            menuTools.Name = "menuTools";
            menuTools.Size = new Size(46, 20);
            menuTools.Text = "&Tools";
            // 
            // menuToolsDbStatus
            // 
            menuToolsDbStatus.Name = "menuToolsDbStatus";
            menuToolsDbStatus.Size = new Size(133, 22);
            menuToolsDbStatus.Text = "DB &Status...";
            // 
            // menuHelp
            // 
            menuHelp.DropDownItems.AddRange(new ToolStripItem[] { menuHelpAbout });
            menuHelp.Name = "menuHelp";
            menuHelp.Size = new Size(44, 20);
            menuHelp.Text = "&Help";
            // 
            // menuHelpAbout
            // 
            menuHelpAbout.Name = "menuHelpAbout";
            menuHelpAbout.Size = new Size(180, 22);
            menuHelpAbout.Text = "&About";
            menuHelpAbout.Click += menuHelpAbout_Click_1;
            // 
            // statusMain
            // 
            statusMain.ImageScalingSize = new Size(20, 20);
            statusMain.Items.AddRange(new ToolStripItem[] { statusText, statusDbMode });
            statusMain.Location = new Point(0, 628);
            statusMain.Name = "statusMain";
            statusMain.Size = new Size(1100, 22);
            statusMain.TabIndex = 2;
            // 
            // statusText
            // 
            statusText.Name = "statusText";
            statusText.Size = new Size(1042, 17);
            statusText.Spring = true;
            statusText.Text = "Ready";
            // 
            // statusDbMode
            // 
            statusDbMode.Name = "statusDbMode";
            statusDbMode.Size = new Size(43, 17);
            statusDbMode.Text = "DB: ???";
            // 
            // splitMain
            // 
            splitMain.Dock = DockStyle.Fill;
            splitMain.FixedPanel = FixedPanel.Panel1;
            splitMain.Location = new Point(0, 24);
            splitMain.Name = "splitMain";
            // 
            // splitMain.Panel1
            // 
            splitMain.Panel1.Controls.Add(treeNav);
            // 
            // splitMain.Panel2
            // 
            splitMain.Panel2.Controls.Add(panelHost);
            splitMain.Size = new Size(1100, 604);
            splitMain.SplitterDistance = 280;
            splitMain.TabIndex = 1;
            // 
            // treeNav
            // 
            treeNav.Dock = DockStyle.Fill;
            treeNav.HideSelection = false;
            treeNav.Location = new Point(0, 0);
            treeNav.Name = "treeNav";
            treeNav.Size = new Size(280, 604);
            treeNav.TabIndex = 0;
            // 
            // panelHost
            // 
            panelHost.Dock = DockStyle.Fill;
            panelHost.Location = new Point(0, 0);
            panelHost.Name = "panelHost";
            panelHost.Size = new Size(816, 604);
            panelHost.TabIndex = 0;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1100, 650);
            Controls.Add(splitMain);
            Controls.Add(statusMain);
            Controls.Add(menuMain);
            MainMenuStrip = menuMain;
            Name = "MainForm";
            Text = "Expensa";
            menuMain.ResumeLayout(false);
            menuMain.PerformLayout();
            statusMain.ResumeLayout(false);
            statusMain.PerformLayout();
            splitMain.Panel1.ResumeLayout(false);
            splitMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitMain).EndInit();
            splitMain.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
    }
}