namespace CodexExpensa.ExtensionDevHost.UI;

partial class DocsEditorForm
{
    private System.ComponentModel.IContainer components = null;

    private ToolStripContainer toolStripContainer;
    private ToolStrip fileToolStrip;
    private ToolStrip editToolStrip;
    private ToolStripButton saveButton;
    private ToolStripButton reloadButton;
    private ToolStripButton aiReviewButton;
    private ToolStripButton openFolderButton;
    private ToolStripButton h1Button;
    private ToolStripButton h2Button;
    private ToolStripButton h3Button;
    private ToolStripButton boldButton;
    private ToolStripButton italicButton;
    private ToolStripButton bulletButton;
    private ToolStripButton numberButton;
    private ToolStripButton codeButton;
    private ToolStripButton codeBlockButton;
    private ToolStripButton quoteButton;
    private ToolStripButton linkButton;
    private ToolStripButton tableButton;
    private ToolStripButton normalizeButton;
    private ToolStripButton showSourceButton;
    private TableLayoutPanel contentLayout;
    private TextBox pathTextBox;
    private SplitContainer editorSplit;
    private WebBrowser previewBrowser;
    private TextBox sourceEditor;

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
        toolStripContainer = new ToolStripContainer();
        contentLayout = new TableLayoutPanel();
        pathTextBox = new TextBox();
        editorSplit = new SplitContainer();
        previewBrowser = new WebBrowser();
        sourceEditor = new TextBox();
        fileToolStrip = new ToolStrip();
        saveButton = new ToolStripButton();
        reloadButton = new ToolStripButton();
        aiReviewButton = new ToolStripButton();
        openFolderButton = new ToolStripButton();
        editToolStrip = new ToolStrip();
        h1Button = new ToolStripButton();
        h2Button = new ToolStripButton();
        h3Button = new ToolStripButton();
        boldButton = new ToolStripButton();
        italicButton = new ToolStripButton();
        bulletButton = new ToolStripButton();
        numberButton = new ToolStripButton();
        codeButton = new ToolStripButton();
        codeBlockButton = new ToolStripButton();
        quoteButton = new ToolStripButton();
        linkButton = new ToolStripButton();
        tableButton = new ToolStripButton();
        normalizeButton = new ToolStripButton();
        showSourceButton = new ToolStripButton();
        toolStripContainer.ContentPanel.SuspendLayout();
        toolStripContainer.TopToolStripPanel.SuspendLayout();
        toolStripContainer.SuspendLayout();
        contentLayout.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)editorSplit).BeginInit();
        editorSplit.Panel1.SuspendLayout();
        editorSplit.Panel2.SuspendLayout();
        editorSplit.SuspendLayout();
        fileToolStrip.SuspendLayout();
        editToolStrip.SuspendLayout();
        SuspendLayout();
        // 
        // toolStripContainer
        // 
        // 
        // toolStripContainer.ContentPanel
        // 
        toolStripContainer.ContentPanel.Controls.Add(contentLayout);
        toolStripContainer.ContentPanel.Size = new Size(1100, 642);
        toolStripContainer.Dock = DockStyle.Fill;
        toolStripContainer.Location = new Point(0, 0);
        toolStripContainer.Name = "toolStripContainer";
        toolStripContainer.Size = new Size(1100, 700);
        toolStripContainer.TabIndex = 0;
        // 
        // toolStripContainer.TopToolStripPanel
        // 
        toolStripContainer.TopToolStripPanel.Controls.Add(fileToolStrip);
        toolStripContainer.TopToolStripPanel.Controls.Add(editToolStrip);
        toolStripContainer.TopToolStripPanel.MinimumSize = new Size(0, 58);
        toolStripContainer.TopToolStripPanel.Padding = new Padding(0, 2, 0, 2);
        // 
        // contentLayout
        // 
        contentLayout.ColumnCount = 1;
        contentLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        contentLayout.Controls.Add(pathTextBox, 0, 0);
        contentLayout.Controls.Add(editorSplit, 0, 1);
        contentLayout.Dock = DockStyle.Fill;
        contentLayout.Location = new Point(0, 0);
        contentLayout.Name = "contentLayout";
        contentLayout.Padding = new Padding(8, 4, 8, 8);
        contentLayout.RowCount = 2;
        contentLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 34F));
        contentLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        contentLayout.Size = new Size(1100, 642);
        contentLayout.TabIndex = 0;
        // 
        // pathTextBox
        // 
        pathTextBox.Dock = DockStyle.Fill;
        pathTextBox.Location = new Point(8, 8);
        pathTextBox.Margin = new Padding(0, 4, 0, 4);
        pathTextBox.Name = "pathTextBox";
        pathTextBox.ReadOnly = true;
        pathTextBox.Size = new Size(1084, 23);
        pathTextBox.TabIndex = 0;
        // 
        // editorSplit
        // 
        editorSplit.Dock = DockStyle.Fill;
        editorSplit.FixedPanel = FixedPanel.Panel2;
        editorSplit.Location = new Point(11, 41);
        editorSplit.Name = "editorSplit";
        editorSplit.Orientation = Orientation.Horizontal;
        // 
        // editorSplit.Panel1
        // 
        editorSplit.Panel1.Controls.Add(previewBrowser);
        // 
        // editorSplit.Panel2
        // 
        editorSplit.Panel2.Controls.Add(sourceEditor);
        editorSplit.Panel2Collapsed = true;
        editorSplit.Size = new Size(1078, 590);
        editorSplit.TabIndex = 1;
        // 
        // previewBrowser
        // 
        previewBrowser.AllowWebBrowserDrop = false;
        previewBrowser.Dock = DockStyle.Fill;
        previewBrowser.Location = new Point(0, 0);
        previewBrowser.Name = "previewBrowser";
        previewBrowser.ScriptErrorsSuppressed = true;
        previewBrowser.Size = new Size(1078, 590);
        previewBrowser.TabIndex = 0;
        // 
        // sourceEditor
        // 
        sourceEditor.AcceptsReturn = true;
        sourceEditor.AcceptsTab = true;
        sourceEditor.Dock = DockStyle.Fill;
        sourceEditor.Font = new Font("Consolas", 10F);
        sourceEditor.Location = new Point(0, 0);
        sourceEditor.Multiline = true;
        sourceEditor.Name = "sourceEditor";
        sourceEditor.ScrollBars = ScrollBars.Both;
        sourceEditor.Size = new Size(150, 46);
        sourceEditor.TabIndex = 0;
        sourceEditor.WordWrap = false;
        // 
        // fileToolStrip
        // 
        fileToolStrip.Dock = DockStyle.None;
        fileToolStrip.Items.AddRange(new ToolStripItem[] { saveButton, reloadButton, aiReviewButton, openFolderButton });
        fileToolStrip.Location = new Point(46, 0);
        fileToolStrip.Name = "fileToolStrip";
        fileToolStrip.Size = new Size(104, 25);
        fileToolStrip.TabIndex = 0;
        // 
        // saveButton
        // 
        saveButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
        saveButton.Name = "saveButton";
        saveButton.Size = new Size(23, 22);
        saveButton.Text = "Save";
        saveButton.ToolTipText = "Save document";
        // 
        // reloadButton
        // 
        reloadButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
        reloadButton.Name = "reloadButton";
        reloadButton.Size = new Size(23, 22);
        reloadButton.Text = "Reload";
        reloadButton.ToolTipText = "Reload document";
        // 
        // aiReviewButton
        // 
        aiReviewButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
        aiReviewButton.Name = "aiReviewButton";
        aiReviewButton.Size = new Size(23, 22);
        aiReviewButton.Text = "AI Review";
        aiReviewButton.ToolTipText = "AI Review";
        // 
        // openFolderButton
        // 
        openFolderButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
        openFolderButton.Name = "openFolderButton";
        openFolderButton.Size = new Size(23, 22);
        openFolderButton.Text = "Open Folder";
        openFolderButton.ToolTipText = "Open Folder";
        // 
        // editToolStrip
        // 
        editToolStrip.Dock = DockStyle.None;
        editToolStrip.Items.AddRange(new ToolStripItem[] { h1Button, h2Button, h3Button, boldButton, italicButton, bulletButton, numberButton, codeButton, codeBlockButton, quoteButton, linkButton, tableButton, normalizeButton, showSourceButton });
        editToolStrip.Location = new Point(3, 25);
        editToolStrip.Name = "editToolStrip";
        editToolStrip.Size = new Size(334, 25);
        editToolStrip.TabIndex = 1;
        // 
        // h1Button
        // 
        h1Button.DisplayStyle = ToolStripItemDisplayStyle.Image;
        h1Button.Name = "h1Button";
        h1Button.Size = new Size(23, 22);
        h1Button.Text = "H1";
        h1Button.ToolTipText = "Heading 1";
        // 
        // h2Button
        // 
        h2Button.DisplayStyle = ToolStripItemDisplayStyle.Image;
        h2Button.Name = "h2Button";
        h2Button.Size = new Size(23, 22);
        h2Button.Text = "H2";
        h2Button.ToolTipText = "Heading 2";
        // 
        // h3Button
        // 
        h3Button.DisplayStyle = ToolStripItemDisplayStyle.Image;
        h3Button.Name = "h3Button";
        h3Button.Size = new Size(23, 22);
        h3Button.Text = "H3";
        h3Button.ToolTipText = "Heading 3";
        // 
        // boldButton
        // 
        boldButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
        boldButton.Name = "boldButton";
        boldButton.Size = new Size(23, 22);
        boldButton.Text = "Bold";
        boldButton.ToolTipText = "Bold";
        // 
        // italicButton
        // 
        italicButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
        italicButton.Name = "italicButton";
        italicButton.Size = new Size(23, 22);
        italicButton.Text = "Italic";
        italicButton.ToolTipText = "Italic";
        // 
        // bulletButton
        // 
        bulletButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
        bulletButton.Name = "bulletButton";
        bulletButton.Size = new Size(23, 22);
        bulletButton.Text = "Bullet";
        bulletButton.ToolTipText = "Bulleted list";
        // 
        // numberButton
        // 
        numberButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
        numberButton.Name = "numberButton";
        numberButton.Size = new Size(23, 22);
        numberButton.Text = "Number";
        numberButton.ToolTipText = "Numbered list";
        // 
        // codeButton
        // 
        codeButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
        codeButton.Name = "codeButton";
        codeButton.Size = new Size(23, 22);
        codeButton.Text = "Code";
        codeButton.ToolTipText = "Inline code";
        // 
        // codeBlockButton
        // 
        codeBlockButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
        codeBlockButton.Name = "codeBlockButton";
        codeBlockButton.Size = new Size(23, 22);
        codeBlockButton.Text = "Code Block";
        codeBlockButton.ToolTipText = "Code block";
        // 
        // quoteButton
        // 
        quoteButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
        quoteButton.Name = "quoteButton";
        quoteButton.Size = new Size(23, 22);
        quoteButton.Text = "Quote";
        quoteButton.ToolTipText = "Quote";
        // 
        // linkButton
        // 
        linkButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
        linkButton.Name = "linkButton";
        linkButton.Size = new Size(23, 22);
        linkButton.Text = "Link";
        linkButton.ToolTipText = "Link";
        // 
        // tableButton
        // 
        tableButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
        tableButton.Name = "tableButton";
        tableButton.Size = new Size(23, 22);
        tableButton.Text = "Table";
        tableButton.ToolTipText = "Table";
        // 
        // normalizeButton
        // 
        normalizeButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
        normalizeButton.Name = "normalizeButton";
        normalizeButton.Size = new Size(23, 22);
        normalizeButton.Text = "Normalize";
        normalizeButton.ToolTipText = "Normalize markdown";
        // 
        // showSourceButton
        // 
        showSourceButton.CheckOnClick = true;
        showSourceButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
        showSourceButton.Name = "showSourceButton";
        showSourceButton.Size = new Size(23, 22);
        showSourceButton.Text = "Show Source";
        showSourceButton.ToolTipText = "Show or hide Markdown source";
        // 
        // DocsEditorForm
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1100, 700);
        Controls.Add(toolStripContainer);
        Name = "DocsEditorForm";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Documentation Editor";
        toolStripContainer.ContentPanel.ResumeLayout(false);
        toolStripContainer.TopToolStripPanel.ResumeLayout(false);
        toolStripContainer.TopToolStripPanel.PerformLayout();
        toolStripContainer.ResumeLayout(false);
        toolStripContainer.PerformLayout();
        contentLayout.ResumeLayout(false);
        contentLayout.PerformLayout();
        editorSplit.Panel1.ResumeLayout(false);
        editorSplit.Panel2.ResumeLayout(false);
        editorSplit.Panel2.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)editorSplit).EndInit();
        editorSplit.ResumeLayout(false);
        fileToolStrip.ResumeLayout(false);
        fileToolStrip.PerformLayout();
        editToolStrip.ResumeLayout(false);
        editToolStrip.PerformLayout();
        ResumeLayout(false);
    }

    private static void ConfigureDesignerToolStrip(ToolStrip toolStrip)
    {
        toolStrip.AutoSize = true;
        toolStrip.CanOverflow = true;
        toolStrip.GripStyle = ToolStripGripStyle.Hidden;
        toolStrip.ImageScalingSize = new Size(16, 16);
        toolStrip.LayoutStyle = ToolStripLayoutStyle.HorizontalStackWithOverflow;
        toolStrip.Padding = new Padding(2);
        toolStrip.RenderMode = ToolStripRenderMode.System;
        toolStrip.Stretch = true;
    }
}
