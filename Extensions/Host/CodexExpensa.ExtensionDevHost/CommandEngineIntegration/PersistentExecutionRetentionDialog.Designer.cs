namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration;

partial class PersistentExecutionRetentionDialog
{
    private System.ComponentModel.IContainer components = null;
    private TableLayoutPanel rootLayoutPanel;
    private TableLayoutPanel filterLayoutPanel;
    private Label sourceFilterLabel;
    private ComboBox sourceFilterComboBox;
    private Label statusFilterLabel;
    private ComboBox statusFilterComboBox;
    private Label agePresetLabel;
    private ComboBox agePresetComboBox;
    private Label summaryLabel;
    private SplitContainer splitContainer;
    private ListView previewListView;
    private TextBox detailsTextBox;
    private FlowLayoutPanel buttonPanel;
    private Button previewButton;
    private Button deleteButton;
    private Button vacuumButton;
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
        filterLayoutPanel = new TableLayoutPanel();
        sourceFilterLabel = new Label();
        sourceFilterComboBox = new ComboBox();
        statusFilterLabel = new Label();
        statusFilterComboBox = new ComboBox();
        agePresetLabel = new Label();
        agePresetComboBox = new ComboBox();
        summaryLabel = new Label();
        splitContainer = new SplitContainer();
        previewListView = new ListView();
        detailsTextBox = new TextBox();
        buttonPanel = new FlowLayoutPanel();
        previewButton = new Button();
        deleteButton = new Button();
        vacuumButton = new Button();
        closeButton = new Button();

        rootLayoutPanel.SuspendLayout();
        filterLayoutPanel.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)splitContainer).BeginInit();
        splitContainer.Panel1.SuspendLayout();
        splitContainer.Panel2.SuspendLayout();
        splitContainer.SuspendLayout();
        buttonPanel.SuspendLayout();
        SuspendLayout();

        rootLayoutPanel.ColumnCount = 1;
        rootLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        rootLayoutPanel.Controls.Add(filterLayoutPanel, 0, 0);
        rootLayoutPanel.Controls.Add(summaryLabel, 0, 1);
        rootLayoutPanel.Controls.Add(splitContainer, 0, 2);
        rootLayoutPanel.Controls.Add(buttonPanel, 0, 3);
        rootLayoutPanel.Dock = DockStyle.Fill;
        rootLayoutPanel.RowCount = 4;
        rootLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 42F));
        rootLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 28F));
        rootLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        rootLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 44F));

        filterLayoutPanel.ColumnCount = 6;
        filterLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100F));
        filterLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33F));
        filterLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100F));
        filterLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33F));
        filterLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100F));
        filterLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 34F));
        filterLayoutPanel.Controls.Add(sourceFilterLabel, 0, 0);
        filterLayoutPanel.Controls.Add(sourceFilterComboBox, 1, 0);
        filterLayoutPanel.Controls.Add(statusFilterLabel, 2, 0);
        filterLayoutPanel.Controls.Add(statusFilterComboBox, 3, 0);
        filterLayoutPanel.Controls.Add(agePresetLabel, 4, 0);
        filterLayoutPanel.Controls.Add(agePresetComboBox, 5, 0);
        filterLayoutPanel.Dock = DockStyle.Fill;
        filterLayoutPanel.RowCount = 1;
        filterLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

        sourceFilterLabel.Dock = DockStyle.Fill;
        sourceFilterLabel.Text = "Source:";
        sourceFilterLabel.TextAlign = ContentAlignment.MiddleLeft;
        sourceFilterComboBox.Dock = DockStyle.Fill;
        sourceFilterComboBox.DropDownStyle = ComboBoxStyle.DropDownList;

        statusFilterLabel.Dock = DockStyle.Fill;
        statusFilterLabel.Text = "Status:";
        statusFilterLabel.TextAlign = ContentAlignment.MiddleLeft;
        statusFilterComboBox.Dock = DockStyle.Fill;
        statusFilterComboBox.DropDownStyle = ComboBoxStyle.DropDownList;

        agePresetLabel.Dock = DockStyle.Fill;
        agePresetLabel.Text = "Age:";
        agePresetLabel.TextAlign = ContentAlignment.MiddleLeft;
        agePresetComboBox.Dock = DockStyle.Fill;
        agePresetComboBox.DropDownStyle = ComboBoxStyle.DropDownList;

        summaryLabel.Dock = DockStyle.Fill;
        summaryLabel.Text = "Matching records: 0";
        summaryLabel.TextAlign = ContentAlignment.MiddleLeft;

        splitContainer.Dock = DockStyle.Fill;
        splitContainer.Orientation = Orientation.Horizontal;
        splitContainer.SplitterDistance = 360;
        splitContainer.Panel1.Controls.Add(previewListView);
        splitContainer.Panel2.Controls.Add(detailsTextBox);

        previewListView.Dock = DockStyle.Fill;
        previewListView.FullRowSelect = true;
        previewListView.MultiSelect = false;
        previewListView.UseCompatibleStateImageBehavior = false;
        previewListView.View = View.Details;

        detailsTextBox.Dock = DockStyle.Fill;
        detailsTextBox.Multiline = true;
        detailsTextBox.ReadOnly = true;
        detailsTextBox.ScrollBars = ScrollBars.Both;
        detailsTextBox.WordWrap = false;

        buttonPanel.Dock = DockStyle.Fill;
        buttonPanel.FlowDirection = FlowDirection.RightToLeft;
        buttonPanel.Controls.Add(closeButton);
        buttonPanel.Controls.Add(vacuumButton);
        buttonPanel.Controls.Add(deleteButton);
        buttonPanel.Controls.Add(previewButton);

        closeButton.Text = "Close";
        closeButton.Width = 100;
        closeButton.Click += closeButton_Click;

        vacuumButton.Text = "Vacuum";
        vacuumButton.Width = 100;
        vacuumButton.Click += vacuumButton_Click;

        deleteButton.Text = "Delete Matching";
        deleteButton.Width = 140;
        deleteButton.Click += deleteButton_Click;

        previewButton.Text = "Preview";
        previewButton.Width = 100;
        previewButton.Click += previewButton_Click;

        CancelButton = closeButton;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1000, 650);
        Controls.Add(rootLayoutPanel);
        MinimizeBox = false;
        MaximizeBox = false;
        Name = "PersistentExecutionRetentionDialog";
        Padding = new Padding(8);
        StartPosition = FormStartPosition.CenterParent;
        Text = "Persistent Execution Retention";

        rootLayoutPanel.ResumeLayout(false);
        filterLayoutPanel.ResumeLayout(false);
        filterLayoutPanel.PerformLayout();
        splitContainer.Panel1.ResumeLayout(false);
        splitContainer.Panel2.ResumeLayout(false);
        splitContainer.Panel2.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)splitContainer).EndInit();
        splitContainer.ResumeLayout(false);
        buttonPanel.ResumeLayout(false);
        ResumeLayout(false);
    }
}
