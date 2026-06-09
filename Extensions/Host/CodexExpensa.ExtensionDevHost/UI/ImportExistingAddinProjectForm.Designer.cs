namespace CodexExpensa.ExtensionDevHost.UI;

partial class ImportExistingAddinProjectForm
{
    private System.ComponentModel.IContainer components = null;
    private TableLayoutPanel rootTableLayoutPanel;
    private Label projectFileLabel;
    private TextBox projectFileTextBox;
    private Button browseButton;
    private TextBox previewTextBox;
    private TextBox resultTextBox;
    private FlowLayoutPanel buttonFlowLayoutPanel;
    private Button importButton;
    private Button cancelButton;

    protected override void Dispose(bool disposing)
    {
        if (disposing && components != null) components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        rootTableLayoutPanel = new TableLayoutPanel();
        projectFileLabel = new Label();
        projectFileTextBox = new TextBox();
        browseButton = new Button();
        previewTextBox = new TextBox();
        resultTextBox = new TextBox();
        buttonFlowLayoutPanel = new FlowLayoutPanel();
        importButton = new Button();
        cancelButton = new Button();
        rootTableLayoutPanel.SuspendLayout();
        buttonFlowLayoutPanel.SuspendLayout();
        SuspendLayout();
        rootTableLayoutPanel.ColumnCount = 3;
        rootTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 90F));
        rootTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        rootTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110F));
        rootTableLayoutPanel.Dock = DockStyle.Fill;
        rootTableLayoutPanel.Padding = new Padding(8);
        rootTableLayoutPanel.RowCount = 4;
        rootTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 34F));
        rootTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 45F));
        rootTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 55F));
        rootTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 42F));
        projectFileLabel.Dock = DockStyle.Fill;
        projectFileLabel.Text = "Project:";
        projectFileLabel.TextAlign = ContentAlignment.MiddleLeft;
        projectFileTextBox.Dock = DockStyle.Fill;
        browseButton.Dock = DockStyle.Fill;
        browseButton.Text = "Browse...";
        browseButton.UseVisualStyleBackColor = true;
        browseButton.Click += browseButton_Click;
        previewTextBox.Dock = DockStyle.Fill;
        previewTextBox.Multiline = true;
        previewTextBox.ReadOnly = true;
        previewTextBox.ScrollBars = ScrollBars.Both;
        previewTextBox.WordWrap = false;
        resultTextBox.Dock = DockStyle.Fill;
        resultTextBox.Multiline = true;
        resultTextBox.ReadOnly = true;
        resultTextBox.ScrollBars = ScrollBars.Both;
        resultTextBox.WordWrap = false;
        buttonFlowLayoutPanel.Dock = DockStyle.Fill;
        buttonFlowLayoutPanel.FlowDirection = FlowDirection.RightToLeft;
        importButton.Text = "Import";
        importButton.Size = new Size(90, 28);
        importButton.Click += importButton_Click;
        cancelButton.Text = "Cancel";
        cancelButton.Size = new Size(90, 28);
        cancelButton.Click += cancelButton_Click;
        rootTableLayoutPanel.Controls.Add(projectFileLabel, 0, 0);
        rootTableLayoutPanel.Controls.Add(projectFileTextBox, 1, 0);
        rootTableLayoutPanel.Controls.Add(browseButton, 2, 0);
        rootTableLayoutPanel.Controls.Add(previewTextBox, 0, 1);
        rootTableLayoutPanel.Controls.Add(resultTextBox, 0, 2);
        rootTableLayoutPanel.Controls.Add(buttonFlowLayoutPanel, 0, 3);
        rootTableLayoutPanel.SetColumnSpan(previewTextBox, 3);
        rootTableLayoutPanel.SetColumnSpan(resultTextBox, 3);
        rootTableLayoutPanel.SetColumnSpan(buttonFlowLayoutPanel, 3);
        buttonFlowLayoutPanel.Controls.Add(cancelButton);
        buttonFlowLayoutPanel.Controls.Add(importButton);
        AcceptButton = importButton;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        CancelButton = cancelButton;
        ClientSize = new Size(820, 460);
        Controls.Add(rootTableLayoutPanel);
        MinimumSize = new Size(720, 420);
        Name = "ImportExistingAddinProjectForm";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Import Existing Add-in Project";
        rootTableLayoutPanel.ResumeLayout(false);
        rootTableLayoutPanel.PerformLayout();
        buttonFlowLayoutPanel.ResumeLayout(false);
        ResumeLayout(false);
    }
}
