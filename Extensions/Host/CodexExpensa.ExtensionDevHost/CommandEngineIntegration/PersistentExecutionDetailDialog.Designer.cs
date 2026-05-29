namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration;

partial class PersistentExecutionDetailDialog
{
    private System.ComponentModel.IContainer components = null;
    private TableLayoutPanel rootLayoutPanel;
    private Label titleLabel;
    private TabControl detailTabControl;
    private TabPage summaryTabPage;
    private TabPage parametersTabPage;
    private TabPage outputTabPage;
    private TextBox summaryTextBox;
    private TextBox parameterTextBox;
    private TextBox outputTextBox;
    private FlowLayoutPanel buttonPanel;
    private Button replayButton;
    private Button copySummaryButton;
    private Button copyParametersButton;
    private Button copyOutputButton;
    private Button closeButton;

    protected override void Dispose(
        bool disposing)
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
        detailTabControl = new TabControl();
        summaryTabPage = new TabPage();
        parametersTabPage = new TabPage();
        outputTabPage = new TabPage();
        summaryTextBox = new TextBox();
        parameterTextBox = new TextBox();
        outputTextBox = new TextBox();
        buttonPanel = new FlowLayoutPanel();
        replayButton = new Button();
        copySummaryButton = new Button();
        copyParametersButton = new Button();
        copyOutputButton = new Button();
        closeButton = new Button();

        rootLayoutPanel.SuspendLayout();
        detailTabControl.SuspendLayout();
        summaryTabPage.SuspendLayout();
        parametersTabPage.SuspendLayout();
        outputTabPage.SuspendLayout();
        buttonPanel.SuspendLayout();
        SuspendLayout();

        rootLayoutPanel.ColumnCount = 1;
        rootLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        rootLayoutPanel.Controls.Add(titleLabel, 0, 0);
        rootLayoutPanel.Controls.Add(detailTabControl, 0, 1);
        rootLayoutPanel.Controls.Add(buttonPanel, 0, 2);
        rootLayoutPanel.Dock = DockStyle.Fill;
        rootLayoutPanel.RowCount = 3;
        rootLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 36F));
        rootLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        rootLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 44F));

        titleLabel.Dock = DockStyle.Fill;
        titleLabel.Text = "Persisted Execution";
        titleLabel.TextAlign = ContentAlignment.MiddleLeft;

        detailTabControl.Controls.Add(summaryTabPage);
        detailTabControl.Controls.Add(parametersTabPage);
        detailTabControl.Controls.Add(outputTabPage);
        detailTabControl.Dock = DockStyle.Fill;

        summaryTabPage.Text = "Summary";
        summaryTabPage.Controls.Add(summaryTextBox);

        parametersTabPage.Text = "Parameters";
        parametersTabPage.Controls.Add(parameterTextBox);

        outputTabPage.Text = "Output";
        outputTabPage.Controls.Add(outputTextBox);

        summaryTextBox.Dock = DockStyle.Fill;
        summaryTextBox.Multiline = true;
        summaryTextBox.ReadOnly = true;
        summaryTextBox.ScrollBars = ScrollBars.Both;
        summaryTextBox.WordWrap = false;

        parameterTextBox.Dock = DockStyle.Fill;
        parameterTextBox.Multiline = true;
        parameterTextBox.ReadOnly = true;
        parameterTextBox.ScrollBars = ScrollBars.Both;
        parameterTextBox.WordWrap = false;

        outputTextBox.Dock = DockStyle.Fill;
        outputTextBox.Multiline = true;
        outputTextBox.ReadOnly = true;
        outputTextBox.ScrollBars = ScrollBars.Both;
        outputTextBox.WordWrap = false;

        buttonPanel.Dock = DockStyle.Fill;
        buttonPanel.FlowDirection = FlowDirection.RightToLeft;
        buttonPanel.Controls.Add(closeButton);
        buttonPanel.Controls.Add(replayButton);
        buttonPanel.Controls.Add(copyOutputButton);
        buttonPanel.Controls.Add(copyParametersButton);
        buttonPanel.Controls.Add(copySummaryButton);

        closeButton.Text = "Close";
        closeButton.Width = 100;
        closeButton.Click += closeButton_Click;

        replayButton.Text = "Replay";
        replayButton.Width = 100;
        replayButton.Click += replayButton_Click;

        copyOutputButton.Text = "Copy Output";
        copyOutputButton.Width = 120;
        copyOutputButton.Click += copyOutputButton_Click;

        copyParametersButton.Text = "Copy Parameters";
        copyParametersButton.Width = 140;
        copyParametersButton.Click += copyParametersButton_Click;

        copySummaryButton.Text = "Copy Summary";
        copySummaryButton.Width = 130;
        copySummaryButton.Click += copySummaryButton_Click;

        CancelButton = closeButton;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(900, 650);
        Controls.Add(rootLayoutPanel);
        MinimizeBox = false;
        MaximizeBox = false;
        Name = "PersistentExecutionDetailDialog";
        Padding = new Padding(8);
        StartPosition = FormStartPosition.CenterParent;
        Text = "Persisted Execution Detail";

        rootLayoutPanel.ResumeLayout(false);
        detailTabControl.ResumeLayout(false);
        summaryTabPage.ResumeLayout(false);
        summaryTabPage.PerformLayout();
        parametersTabPage.ResumeLayout(false);
        parametersTabPage.PerformLayout();
        outputTabPage.ResumeLayout(false);
        outputTabPage.PerformLayout();
        buttonPanel.ResumeLayout(false);
        ResumeLayout(false);
    }
}
