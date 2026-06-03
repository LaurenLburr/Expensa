namespace CodexExpensa.App.WinForms.UI.Websites;

partial class WebsiteDetailsForm
{
    private System.ComponentModel.IContainer components = null;

    private TableLayoutPanel mainLayoutPanel;
    private Label titleLabel;
    private Label nodeTypeLabel;
    private Label nodeTypeValueLabel;
    private Label nodeIdLabel;
    private TextBox nodeIdValueTextBox;
    private Label websiteIdLabel;
    private TextBox websiteIdValueTextBox;
    private Label categoryLabel;
    private TextBox categoryValueTextBox;
    private Label urlLabel;
    private TextBox urlValueTextBox;
    private Label enabledLabel;
    private Label enabledValueLabel;
    private FlowLayoutPanel buttonPanel;
    private Button openWebsiteButton;

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
        mainLayoutPanel = new TableLayoutPanel();
        titleLabel = new Label();
        nodeTypeLabel = new Label();
        nodeTypeValueLabel = new Label();
        nodeIdLabel = new Label();
        nodeIdValueTextBox = new TextBox();
        websiteIdLabel = new Label();
        websiteIdValueTextBox = new TextBox();
        categoryLabel = new Label();
        categoryValueTextBox = new TextBox();
        urlLabel = new Label();
        urlValueTextBox = new TextBox();
        enabledLabel = new Label();
        enabledValueLabel = new Label();
        buttonPanel = new FlowLayoutPanel();
        openWebsiteButton = new Button();
        mainLayoutPanel.SuspendLayout();
        buttonPanel.SuspendLayout();
        SuspendLayout();
        // 
        // mainLayoutPanel
        // 
        mainLayoutPanel.ColumnCount = 2;
        mainLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120F));
        mainLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        mainLayoutPanel.Controls.Add(titleLabel, 0, 0);
        mainLayoutPanel.Controls.Add(nodeTypeLabel, 0, 1);
        mainLayoutPanel.Controls.Add(nodeTypeValueLabel, 1, 1);
        mainLayoutPanel.Controls.Add(nodeIdLabel, 0, 2);
        mainLayoutPanel.Controls.Add(nodeIdValueTextBox, 1, 2);
        mainLayoutPanel.Controls.Add(websiteIdLabel, 0, 3);
        mainLayoutPanel.Controls.Add(websiteIdValueTextBox, 1, 3);
        mainLayoutPanel.Controls.Add(categoryLabel, 0, 4);
        mainLayoutPanel.Controls.Add(categoryValueTextBox, 1, 4);
        mainLayoutPanel.Controls.Add(urlLabel, 0, 5);
        mainLayoutPanel.Controls.Add(urlValueTextBox, 1, 5);
        mainLayoutPanel.Controls.Add(enabledLabel, 0, 6);
        mainLayoutPanel.Controls.Add(enabledValueLabel, 1, 6);
        mainLayoutPanel.Controls.Add(buttonPanel, 0, 7);
        mainLayoutPanel.Dock = DockStyle.Fill;
        mainLayoutPanel.Location = new Point(0, 0);
        mainLayoutPanel.Name = "mainLayoutPanel";
        mainLayoutPanel.Padding = new Padding(12);
        mainLayoutPanel.RowCount = 8;
        mainLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 42F));
        mainLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F));
        mainLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F));
        mainLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F));
        mainLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F));
        mainLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F));
        mainLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F));
        mainLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        mainLayoutPanel.Size = new Size(720, 420);
        mainLayoutPanel.TabIndex = 0;
        // 
        // titleLabel
        // 
        titleLabel.AutoSize = true;
        mainLayoutPanel.SetColumnSpan(titleLabel, 2);
        titleLabel.Dock = DockStyle.Fill;
        titleLabel.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
        titleLabel.Location = new Point(15, 12);
        titleLabel.Name = "titleLabel";
        titleLabel.Size = new Size(690, 42);
        titleLabel.TabIndex = 0;
        titleLabel.Text = "Website";
        titleLabel.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // nodeTypeLabel
        // 
        nodeTypeLabel.AutoSize = true;
        nodeTypeLabel.Dock = DockStyle.Fill;
        nodeTypeLabel.Location = new Point(15, 54);
        nodeTypeLabel.Name = "nodeTypeLabel";
        nodeTypeLabel.Size = new Size(114, 32);
        nodeTypeLabel.TabIndex = 1;
        nodeTypeLabel.Text = "Node Type";
        nodeTypeLabel.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // nodeTypeValueLabel
        // 
        nodeTypeValueLabel.AutoSize = true;
        nodeTypeValueLabel.Dock = DockStyle.Fill;
        nodeTypeValueLabel.Location = new Point(135, 54);
        nodeTypeValueLabel.Name = "nodeTypeValueLabel";
        nodeTypeValueLabel.Size = new Size(570, 32);
        nodeTypeValueLabel.TabIndex = 2;
        nodeTypeValueLabel.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // nodeIdLabel
        // 
        nodeIdLabel.AutoSize = true;
        nodeIdLabel.Dock = DockStyle.Fill;
        nodeIdLabel.Location = new Point(15, 86);
        nodeIdLabel.Name = "nodeIdLabel";
        nodeIdLabel.Size = new Size(114, 32);
        nodeIdLabel.TabIndex = 3;
        nodeIdLabel.Text = "Node Id";
        nodeIdLabel.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // nodeIdValueTextBox
        // 
        nodeIdValueTextBox.Dock = DockStyle.Fill;
        nodeIdValueTextBox.Location = new Point(135, 89);
        nodeIdValueTextBox.Name = "nodeIdValueTextBox";
        nodeIdValueTextBox.ReadOnly = true;
        nodeIdValueTextBox.Size = new Size(570, 23);
        nodeIdValueTextBox.TabIndex = 4;
        // 
        // websiteIdLabel
        // 
        websiteIdLabel.AutoSize = true;
        websiteIdLabel.Dock = DockStyle.Fill;
        websiteIdLabel.Location = new Point(15, 118);
        websiteIdLabel.Name = "websiteIdLabel";
        websiteIdLabel.Size = new Size(114, 32);
        websiteIdLabel.TabIndex = 5;
        websiteIdLabel.Text = "Website Id";
        websiteIdLabel.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // websiteIdValueTextBox
        // 
        websiteIdValueTextBox.Dock = DockStyle.Fill;
        websiteIdValueTextBox.Location = new Point(135, 121);
        websiteIdValueTextBox.Name = "websiteIdValueTextBox";
        websiteIdValueTextBox.ReadOnly = true;
        websiteIdValueTextBox.Size = new Size(570, 23);
        websiteIdValueTextBox.TabIndex = 6;
        // 
        // categoryLabel
        // 
        categoryLabel.AutoSize = true;
        categoryLabel.Dock = DockStyle.Fill;
        categoryLabel.Location = new Point(15, 150);
        categoryLabel.Name = "categoryLabel";
        categoryLabel.Size = new Size(114, 32);
        categoryLabel.TabIndex = 7;
        categoryLabel.Text = "Category";
        categoryLabel.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // categoryValueTextBox
        // 
        categoryValueTextBox.Dock = DockStyle.Fill;
        categoryValueTextBox.Location = new Point(135, 153);
        categoryValueTextBox.Name = "categoryValueTextBox";
        categoryValueTextBox.ReadOnly = true;
        categoryValueTextBox.Size = new Size(570, 23);
        categoryValueTextBox.TabIndex = 8;
        // 
        // urlLabel
        // 
        urlLabel.AutoSize = true;
        urlLabel.Dock = DockStyle.Fill;
        urlLabel.Location = new Point(15, 182);
        urlLabel.Name = "urlLabel";
        urlLabel.Size = new Size(114, 32);
        urlLabel.TabIndex = 9;
        urlLabel.Text = "Url";
        urlLabel.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // urlValueTextBox
        // 
        urlValueTextBox.Dock = DockStyle.Fill;
        urlValueTextBox.Location = new Point(135, 185);
        urlValueTextBox.Name = "urlValueTextBox";
        urlValueTextBox.ReadOnly = true;
        urlValueTextBox.Size = new Size(570, 23);
        urlValueTextBox.TabIndex = 10;
        // 
        // enabledLabel
        // 
        enabledLabel.AutoSize = true;
        enabledLabel.Dock = DockStyle.Fill;
        enabledLabel.Location = new Point(15, 214);
        enabledLabel.Name = "enabledLabel";
        enabledLabel.Size = new Size(114, 32);
        enabledLabel.TabIndex = 11;
        enabledLabel.Text = "Enabled";
        enabledLabel.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // enabledValueLabel
        // 
        enabledValueLabel.AutoSize = true;
        enabledValueLabel.Dock = DockStyle.Fill;
        enabledValueLabel.Location = new Point(135, 214);
        enabledValueLabel.Name = "enabledValueLabel";
        enabledValueLabel.Size = new Size(570, 32);
        enabledValueLabel.TabIndex = 12;
        enabledValueLabel.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // buttonPanel
        // 
        mainLayoutPanel.SetColumnSpan(buttonPanel, 2);
        buttonPanel.Controls.Add(openWebsiteButton);
        buttonPanel.Dock = DockStyle.Fill;
        buttonPanel.FlowDirection = FlowDirection.LeftToRight;
        buttonPanel.Location = new Point(15, 249);
        buttonPanel.Name = "buttonPanel";
        buttonPanel.Size = new Size(690, 156);
        buttonPanel.TabIndex = 13;
        // 
        // openWebsiteButton
        // 
        openWebsiteButton.AutoSize = true;
        openWebsiteButton.Location = new Point(3, 3);
        openWebsiteButton.Name = "openWebsiteButton";
        openWebsiteButton.Size = new Size(102, 25);
        openWebsiteButton.TabIndex = 0;
        openWebsiteButton.Text = "Open Website";
        openWebsiteButton.UseVisualStyleBackColor = true;
        openWebsiteButton.Click += openWebsiteButton_Click;
        // 
        // WebsiteDetailsForm
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(720, 420);
        Controls.Add(mainLayoutPanel);
        Name = "WebsiteDetailsForm";
        Text = "Website Details";
        mainLayoutPanel.ResumeLayout(false);
        mainLayoutPanel.PerformLayout();
        buttonPanel.ResumeLayout(false);
        buttonPanel.PerformLayout();
        ResumeLayout(false);
    }
}
