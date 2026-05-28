using CodexExpensa.ExtensionDevHost.Commands.Services;
using CodexExpensa.ExtensionDevHost.Services.Ai;

namespace CodexExpensa.ExtensionDevHost.UI;

public partial class StandardAiDocsUpdateForm : Form
{
    private static readonly StandardDocDefinition[] Documents =
    [
        new(
            "General Coding Rules",
            "GeneralCodingRules.md",
            "Create a concise but complete General Coding Rules markdown document for Lauren's C#/.NET/WinForms/SQLite projects. Include rules for full-file delivery, no mocks, integration-style tests, guard clauses, SQLite SqlQuery catalog usage, secrets handling, Visual Studio Extensibility constraints, and AI-assisted design workflows."),

        new(
            "General Coding Spec",
            "GeneralCodingSpec.md",
            "Create a practical General Coding Spec markdown document for Lauren's C#/.NET/WinForms/SQLite projects. Include preferred stack, database standards, SqlQuery catalog schema, logging, file delivery standards, AI workflow standards, and preferred design spec sections."),

        new(
            "AI Add-in Design Workflow",
            "AiAddinDesignWorkflow.md",
            "Create a markdown AI Add-in Design Workflow document for ExtensionDevHost. It should describe using AI to maintain an ongoing design conversation, update curated design specs, maintain Expensa integration specs, and generate scaffolding only after architecture stabilizes."),

        new(
            "Add-in Design Spec Template",
            "AddinDesignSpecTemplate.md",
            "Create a reusable markdown Add-in Design Spec Template for Expensa add-ins. Include sections for Overview, Goals, Non-Goals, User Workflow, Tree Integration, Commands, Forms, Services, Data Model, SQLite Schema, Query Catalog, File Structure, AI Integration, Testing Plan, Open Questions, and Revision History."),

        new(
            "Expensa Integration Spec Template",
            "ExpensaIntegrationSpecTemplate.md",
            "Create a reusable markdown Expensa Integration Spec Template for add-ins. Include sections for integration overview, tree nodes, commands, workspace integration, menu integration, query catalog usage, database integration, shared services, extension registration, AI integration, future expansion, and open questions.")
    ];

    private readonly string _workspaceDocsFolder;

    public StandardAiDocsUpdateForm()
    {
        InitializeComponent();

        _workspaceDocsFolder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Expensa",
            "Extensions",
            "WorkspaceDocs");

        Directory.CreateDirectory(_workspaceDocsFolder);

        LoadDocumentList();
        SetStatus("Ready.");
    }

    private void LoadDocumentList()
    {
        documentListBox.Items.Clear();

        foreach (StandardDocDefinition document in Documents)
        {
            documentListBox.Items.Add(document);
        }

        if (documentListBox.Items.Count > 0)
        {
            documentListBox.SelectedIndex = 0;
        }
    }

    private void DocumentListBox_SelectedIndexChanged(object? sender, EventArgs e)
    {
        LoadSelectedDocument();
    }

    private void LoadSelectedDocument()
    {
        if (documentListBox.SelectedItem is not StandardDocDefinition document)
        {
            previewTextBox.Clear();
            pathTextBox.Clear();
            return;
        }

        string path = GetDocumentPath(document.FileName);
        pathTextBox.Text = path;

        previewTextBox.Text = File.Exists(path)
            ? File.ReadAllText(path)
            : string.Empty;

        promptTextBox.Text = document.Prompt;
    }

    private async void UpdateSelectedButton_Click(object? sender, EventArgs e)
    {
        if (documentListBox.SelectedItem is not StandardDocDefinition document)
        {
            return;
        }

        await UpdateDocumentAsync(document);
    }

    private async void UpdateAllButton_Click(object? sender, EventArgs e)
    {
        DialogResult result = MessageBox.Show(
            this,
            "Update all standard AI instruction documents from AI?",
            "Update Standard AI Docs",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (result != DialogResult.Yes)
        {
            return;
        }

        foreach (StandardDocDefinition document in Documents)
        {
            await UpdateDocumentAsync(document);
        }

        LoadSelectedDocument();
    }

    private async Task UpdateDocumentAsync(StandardDocDefinition document)
    {
        try
        {
            SetBusy(true);
            SetStatus($"Updating {document.DisplayName}...");

            string existing = previewTextBox.Text;
            string prompt = BuildPrompt(document, existing);

            OpenAiCommandAiService aiService = new(new OpenAiApiKeyStore());
            string response = await aiService.GenerateScaffoldAsync(prompt);

            string markdown = ExtractMarkdown(response);

            if (string.IsNullOrWhiteSpace(markdown))
            {
                markdown = response.Trim();
            }

            string path = GetDocumentPath(document.FileName);
            File.WriteAllText(path, markdown.Trim() + Environment.NewLine);

            if (documentListBox.SelectedItem == document)
            {
                previewTextBox.Text = File.ReadAllText(path);
            }

            SetStatus($"Updated {document.DisplayName}.");
        }
        catch (Exception ex)
        {
            SetStatus("Update failed.");

            MessageBox.Show(
                this,
                ex.Message,
                "Update Standard AI Docs",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
        finally
        {
            SetBusy(false);
        }
    }

    private string BuildPrompt(StandardDocDefinition document, string existing)
    {
        string userPrompt = string.IsNullOrWhiteSpace(promptTextBox.Text)
            ? document.Prompt
            : promptTextBox.Text.Trim();

        return $"""
You are updating a standard instruction document used as AI context for Lauren's ExtensionDevHost and Expensa add-in work.

Document:
{document.DisplayName}

File name:
{document.FileName}

Instructions:
{userPrompt}

Existing document:
```markdown
{existing}
```

Rules:
- Return ONLY markdown.
- Do not wrap the answer in explanatory prose.
- Keep it practical and implementation-oriented.
- Preserve useful existing rules.
- Remove obsolete or duplicate rules.
- Use clear headings.
- Include C#/.NET/WinForms/SQLite conventions where relevant.
- Include AI workflow rules where relevant.
""";
    }

    private static string ExtractMarkdown(string response)
    {
        if (string.IsNullOrWhiteSpace(response))
        {
            return string.Empty;
        }

        int firstHeading = response.IndexOf("# ", StringComparison.Ordinal);

        if (firstHeading >= 0)
        {
            return response[firstHeading..].Trim();
        }

        return response.Trim();
    }

    private void OpenFolderButton_Click(object? sender, EventArgs e)
    {
        Directory.CreateDirectory(_workspaceDocsFolder);

        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
        {
            FileName = _workspaceDocsFolder,
            UseShellExecute = true
        });
    }

    private string GetDocumentPath(string fileName)
    {
        return Path.Combine(_workspaceDocsFolder, fileName);
    }

    private void SetBusy(bool busy)
    {
        UseWaitCursor = busy;
        updateSelectedButton.Enabled = !busy;
        updateAllButton.Enabled = !busy;
        documentListBox.Enabled = !busy;
        promptTextBox.Enabled = !busy;
    }

    private void SetStatus(string message)
    {
        statusLabel.Text = message;
    }

    private sealed record StandardDocDefinition(
        string DisplayName,
        string FileName,
        string Prompt)
    {
        public override string ToString()
        {
            return DisplayName;
        }
    }
}
