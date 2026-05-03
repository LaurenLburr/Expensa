namespace CodexExpensa.ExtensionDevHost;

partial class MainForm
{
    private System.ComponentModel.IContainer components = null;

    private MenuStrip mainMenuStrip;
    private SplitContainer mainSplitContainer;
    private TreeView navigationTreeView;
    private Panel contentPanel;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components is not null))
        {
            components.Dispose();
        }

        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        mainMenuStrip = new MenuStrip();
        mainSplitContainer = new SplitContainer();
        navigationTreeView = new TreeView();
        contentPanel = new Panel();
        ((System.ComponentModel.ISupportInitialize)mainSplitContainer).BeginInit();
        mainSplitContainer.Panel1.SuspendLayout();
        mainSplitContainer.Panel2.SuspendLayout();
        mainSplitContainer.SuspendLayout();
        SuspendLayout();
        // 
        // mainMenuStrip
        // 
        mainMenuStrip.Location = new Point(0, 0);
        mainMenuStrip.Name = "mainMenuStrip";
        mainMenuStrip.Size = new Size(1216, 24);
        mainMenuStrip.TabIndex = 1;
        // 
        // mainSplitContainer
        // 
        mainSplitContainer.Dock = DockStyle.Fill;
        mainSplitContainer.FixedPanel = FixedPanel.Panel1;
        mainSplitContainer.Location = new Point(0, 24);
        mainSplitContainer.Name = "mainSplitContainer";
        // 
        // mainSplitContainer.Panel1
        // 
        mainSplitContainer.Panel1.Controls.Add(navigationTreeView);
        // 
        // mainSplitContainer.Panel2
        // 
        mainSplitContainer.Panel2.Controls.Add(contentPanel);
        mainSplitContainer.Size = new Size(1216, 688);
        mainSplitContainer.SplitterDistance = 165;
        mainSplitContainer.TabIndex = 0;
        // 
        // navigationTreeView
        // 
        navigationTreeView.Dock = DockStyle.Fill;
        navigationTreeView.HideSelection = false;
        navigationTreeView.Location = new Point(0, 0);
        navigationTreeView.Name = "navigationTreeView";
        navigationTreeView.Size = new Size(165, 688);
        navigationTreeView.TabIndex = 0;
        navigationTreeView.AfterSelect += NavigationTreeView_AfterSelect;
        navigationTreeView.NodeMouseDoubleClick += NavigationTreeView_NodeMouseDoubleClick;
        // 
        // contentPanel
        // 
        contentPanel.Dock = DockStyle.Fill;
        contentPanel.Location = new Point(0, 0);
        contentPanel.Name = "contentPanel";
        contentPanel.Size = new Size(1047, 688);
        contentPanel.TabIndex = 0;
        // 
        // MainForm
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1216, 712);
        Controls.Add(mainSplitContainer);
        Controls.Add(mainMenuStrip);
        MainMenuStrip = mainMenuStrip;
        Name = "MainForm";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Extension Dev Host";
        mainSplitContainer.Panel1.ResumeLayout(false);
        mainSplitContainer.Panel2.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)mainSplitContainer).EndInit();
        mainSplitContainer.ResumeLayout(false);
        ResumeLayout(false);
        PerformLayout();
    }
}
