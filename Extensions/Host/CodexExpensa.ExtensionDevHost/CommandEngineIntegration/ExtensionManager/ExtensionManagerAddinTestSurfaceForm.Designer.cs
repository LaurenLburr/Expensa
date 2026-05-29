namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.ExtensionManager;

partial class ExtensionManagerAddinTestSurfaceForm
{
    private System.ComponentModel.IContainer components = null;
    private TableLayoutPanel rootLayoutPanel;
    private Label titleLabel;
    private Label statusLabel;
    private SplitContainer splitContainer;
    private TreeView addinTreeView;
    private TextBox detailsTextBox;
    private FlowLayoutPanel buttonPanel;
    private Button refreshButton;
    private Button openSelectedButton;
    private Button closeButton;

    protected override void Dispose(bool disposing)
    {
        if (disposing && components is not null)
        {
            components.Dispose();
        }

        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        rootLayoutPanel = new TableLayoutPanel();
        titleLabel = new Label();
        statusLabel = new Label();
        splitContainer = new SplitContainer();
        addinTreeView = new TreeView();
        detailsTextBox = new TextBox();
        buttonPanel = new FlowLayoutPanel();
        refreshButton = new Button();
        openSelectedButton = new Button();
        closeButton = new Button();

        rootLayoutPanel.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)splitContainer).BeginInit();
        splitContainer.Panel1.SuspendLayout();
        splitContainer.Panel2.SuspendLayout();
        splitContainer.SuspendLayout();
        buttonPanel.SuspendLayout();
        SuspendLayout();

        rootLayoutPanel.ColumnCount = 1;
        rootLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        rootLayoutPanel.Controls.Add(titleLabel, 0, 0);
        rootLayoutPanel.Controls.Add(statusLabel, 0, 1);
        rootLayoutPanel.Controls.Add(splitContainer, 0, 2);
        rootLayoutPanel.Controls.Add(buttonPanel, 0, 3);
        rootLayoutPanel.Dock = DockStyle.Fill;
        rootLayoutPanel.RowCount = 4;
        rootLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 36F));
        rootLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 28F));
        rootLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        rootLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 44F));

        titleLabel.Dock = DockStyle.Fill;
        titleLabel.Text = "Extension Manager Add-in Test Surface";
        titleLabel.TextAlign = ContentAlignment.MiddleLeft;

        statusLabel.Dock = DockStyle.Fill;
        statusLabel.Text = "Ready.";
        statusLabel.TextAlign = ContentAlignment.MiddleLeft;

        splitContainer.Dock = DockStyle.Fill;
        splitContainer.SplitterDistance = 360;
        splitContainer.Panel1.Controls.Add(addinTreeView);
        splitContainer.Panel2.Controls.Add(detailsTextBox);

        addinTreeView.Dock = DockStyle.Fill;
        addinTreeView.HideSelection = false;
        addinTreeView.AfterSelect += addinTreeView_AfterSelect;
        addinTreeView.DoubleClick += addinTreeView_DoubleClick;

        detailsTextBox.Dock = DockStyle.Fill;
        detailsTextBox.Multiline = true;
        detailsTextBox.ReadOnly = true;
        detailsTextBox.ScrollBars = ScrollBars.Both;
        detailsTextBox.WordWrap = false;

        buttonPanel.Dock = DockStyle.Fill;
        buttonPanel.FlowDirection = FlowDirection.RightToLeft;
        buttonPanel.Controls.Add(closeButton);
        buttonPanel.Controls.Add(openSelectedButton);
        buttonPanel.Controls.Add(refreshButton);

        closeButton.Text = "Close";
        closeButton.Width = 100;
        closeButton.Click += closeButton_Click;

        openSelectedButton.Text = "Open Selected";
        openSelectedButton.Width = 130;
        openSelectedButton.Click += openSelectedButton_Click;

        refreshButton.Text = "Refresh";
        refreshButton.Width = 100;
        refreshButton.Click += refreshButton_Click;

        CancelButton = closeButton;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(900, 600);
        Controls.Add(rootLayoutPanel);
        MinimizeBox = false;
        MaximizeBox = false;
        Name = "ExtensionManagerAddinTestSurfaceForm";
        Padding = new Padding(8);
        StartPosition = FormStartPosition.CenterParent;
        Text = "Extension Manager Add-in Test Surface";

        rootLayoutPanel.ResumeLayout(false);
        splitContainer.Panel1.ResumeLayout(false);
        splitContainer.Panel2.ResumeLayout(false);
        splitContainer.Panel2.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)splitContainer).EndInit();
        splitContainer.ResumeLayout(false);
        buttonPanel.ResumeLayout(false);
        ResumeLayout(false);
    }
}
