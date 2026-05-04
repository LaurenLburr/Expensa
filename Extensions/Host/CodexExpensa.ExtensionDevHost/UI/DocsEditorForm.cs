using System.Diagnostics;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using CodexExpensa.ExtensionDevHost.Services.Ai;

namespace CodexExpensa.ExtensionDevHost.UI;

public partial class DocsEditorForm : Form
{
    private const int DefaultSourcePanelHeight = 240;
    private const int MinimumPreviewPanelHeight = 180;
    private const int MinimumSourcePanelHeight = 120;

    private readonly string _settingsPath;

    private string? _currentFilePath;
    private bool _isRendering;
    private bool _suppressPreviewHighlightClear;
    private bool _isApplyingAiReviewResult;
    private bool _isWaitingForAiReply;
    private string? _pendingPreviewHtml;
    private HashSet<int> _highlightedPreviewLineIndexes = new();

    private Image? _aiReviewReadyImage;
    private Image? _aiReviewWaitingImage;

    public DocsEditorForm()
    {
        InitializeComponent();

        _settingsPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Expensa",
            "Extensions",
            "DocsEditorSettings.json");

        ApplyToolbarIcons();
        WireRuntimeEvents();
    }

    public void FocusEditor()
    {
        if (!IsHandleCreated)
        {
            CreateControl();
        }

        PerformLayout();
        Refresh();

        if (sourceEditor.Visible && !editorSplit.Panel2Collapsed)
        {
            sourceEditor.Focus();
        }
    }

    public void SelectDocumentByTag(string docTag)
    {
        string? fileName = docTag switch
        {
            "Docs.ExtensionManagerCatchUp" => "ExtensionManager_CatchUp_Latest.md",
            "Docs.GeneralCodingSpec" => "General_Coding_Spec.md",
            "Docs.GeneralCodingRules" => "General_Coding_Rules.md",
            "Docs.AddinDesignSpecTemplate" => "Addin_Design_Spec_Template.md",
            "Docs.ExpensaIntegrationSpecTemplate" => "Expensa_Integration_Design_Spec_Template.md",
            "Docs.AiAddinDesignWorkflow" => "AI_Addin_Design_Workflow.md",
            _ => null
        };

        if (fileName is null)
        {
            return;
        }

        string path = Path.Combine(FindSolutionRoot(), "Docs", fileName);
        EnsureTemplate(path);
        LoadDocumentPath(path);
    }

    public void LoadDocumentPath(string filePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        if (!IsHandleCreated)
        {
            CreateControl();
        }

        string? folder = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrWhiteSpace(folder))
        {
            Directory.CreateDirectory(folder);
        }

        if (!File.Exists(filePath))
        {
            File.WriteAllText(filePath, $"# {Path.GetFileNameWithoutExtension(filePath)}{Environment.NewLine}");
        }

        string loadedText = NormalizeMarkdownLineEndings(File.ReadAllText(filePath));

        _currentFilePath = filePath;
        pathTextBox.Text = filePath;

        _suppressPreviewHighlightClear = true;
        try
        {
            sourceEditor.Text = loadedText;
            sourceEditor.SelectionStart = 0;
            sourceEditor.SelectionLength = 0;
        }
        finally
        {
            _suppressPreviewHighlightClear = false;
        }

        _highlightedPreviewLineIndexes.Clear();

        // Important: keep the source pane visible after loading.
        // This proves the selected file actually loaded and avoids the preview browser
        // looking blank while WinForms/IE finishes rendering.
        showSourceButton.Checked = true;
        showSourceButton.Text = "Hide Source";
        editorSplit.Panel2Collapsed = false;
        ApplySavedSplitterLayout();

        PerformLayout();

        if (previewBrowser.Document is null)
        {
            previewBrowser.Navigate("about:blank");
        }

        RenderPreview();
        Refresh();
    }

    private void WireRuntimeEvents()
    {
        saveButton.Click += (_, _) => SaveCurrentDocument();
        reloadButton.Click += (_, _) => ReloadCurrentDocument();
        aiReviewButton.Click += async (_, _) => await ReviewCurrentDocumentAsync();
        openFolderButton.Click += (_, _) => OpenDocsFolder();

        h1Button.Click += (_, _) => ApplyLinePrefix("# ");
        h2Button.Click += (_, _) => ApplyLinePrefix("## ");
        h3Button.Click += (_, _) => ApplyLinePrefix("### ");
        boldButton.Click += (_, _) => WrapSelection("**", "**", "bold text");
        italicButton.Click += (_, _) => WrapSelection("*", "*", "italic text");
        bulletButton.Click += (_, _) => ApplyLinePrefix("- ");
        numberButton.Click += (_, _) => ApplyLinePrefix("1. ");
        codeButton.Click += (_, _) => WrapSelection("`", "`", "code");
        codeBlockButton.Click += (_, _) => WrapSelection("```" + Environment.NewLine, Environment.NewLine + "```", "code block");
        quoteButton.Click += (_, _) => ApplyLinePrefix("> ");
        linkButton.Click += (_, _) => WrapSelection("[", "](url)", "link text");
        tableButton.Click += (_, _) => InsertTextAtCursor("| Column 1 | Column 2 |" + Environment.NewLine + "|---|---|" + Environment.NewLine + "|  |  |" + Environment.NewLine);
        normalizeButton.Click += (_, _) => NormalizeCurrentMarkdown();

        showSourceButton.CheckedChanged += (_, _) =>
        {
            editorSplit.Panel2Collapsed = !showSourceButton.Checked;
            showSourceButton.Text = showSourceButton.Checked ? "Hide Source" : "Show Source";

            if (showSourceButton.Checked)
            {
                ApplySavedSplitterLayout();
            }
        };

        sourceEditor.TextChanged += (_, _) =>
        {
            if (_isRendering)
            {
                return;
            }

            if (!_suppressPreviewHighlightClear && !_isApplyingAiReviewResult)
            {
                _highlightedPreviewLineIndexes.Clear();
            }

            RenderPreview();
        };

        editorSplit.SplitterMoved += (_, _) => SaveSplitterLocation();
        previewBrowser.DocumentCompleted += PreviewBrowser_DocumentCompleted;

        Shown += (_, _) => ApplySavedSplitterLayout();
        Resize += (_, _) =>
        {
            if (!editorSplit.Panel2Collapsed)
            {
                ApplySafeSourceHeight();
            }
        };
    }

    private void ApplyToolbarIcons()
    {
        _aiReviewReadyImage = CreateTextIcon("AI", Color.FromArgb(128, 80, 190));
        _aiReviewWaitingImage = CreateHourglassIcon();

        saveButton.Image = CreateTextIcon("S", Color.FromArgb(34, 139, 230));
        reloadButton.Image = CreateTextIcon("R", Color.FromArgb(91, 154, 76));
        aiReviewButton.Image = _aiReviewReadyImage;
        openFolderButton.Image = CreateFolderIcon();

        h1Button.Image = CreateTextIcon("H1", Color.FromArgb(80, 80, 80));
        h2Button.Image = CreateTextIcon("H2", Color.FromArgb(80, 80, 80));
        h3Button.Image = CreateTextIcon("H3", Color.FromArgb(80, 80, 80));
        boldButton.Image = CreateTextIcon("B", Color.FromArgb(30, 30, 30), FontStyle.Bold);
        italicButton.Image = CreateTextIcon("I", Color.FromArgb(30, 30, 30), FontStyle.Italic);
        bulletButton.Image = CreateTextIcon("•", Color.FromArgb(80, 80, 80));
        numberButton.Image = CreateTextIcon("1", Color.FromArgb(80, 80, 80));
        codeButton.Image = CreateTextIcon("<>", Color.FromArgb(80, 80, 80));
        codeBlockButton.Image = CreateTextIcon("{ }", Color.FromArgb(80, 80, 80));
        quoteButton.Image = CreateTextIcon("“", Color.FromArgb(80, 80, 80));
        linkButton.Image = CreateTextIcon("@", Color.FromArgb(80, 80, 80));
        tableButton.Image = CreateTableIcon();
        normalizeButton.Image = CreateTextIcon("N", Color.FromArgb(230, 126, 34));
        showSourceButton.Image = CreateTextIcon("</>", Color.FromArgb(80, 80, 80));

        foreach (ToolStripItem item in fileToolStrip.Items)
        {
            if (item is ToolStripButton button)
            {
                button.DisplayStyle = ToolStripItemDisplayStyle.Image;
            }
        }

        foreach (ToolStripItem item in editToolStrip.Items)
        {
            if (item is ToolStripButton button)
            {
                button.DisplayStyle = ToolStripItemDisplayStyle.Image;
            }
        }
    }

    private static Bitmap CreateTextIcon(string text, Color color, FontStyle style = FontStyle.Bold)
    {
        Bitmap bitmap = new(16, 16);

        using Graphics graphics = Graphics.FromImage(bitmap);
        graphics.Clear(Color.Transparent);
        graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

        using Font font = new("Segoe UI", text.Length > 2 ? 6.5f : 8f, style, GraphicsUnit.Point);
        using Brush brush = new SolidBrush(color);

        SizeF size = graphics.MeasureString(text, font);
        float x = Math.Max(0, (16 - size.Width) / 2);
        float y = Math.Max(0, (16 - size.Height) / 2);

        graphics.DrawString(text, font, brush, x, y);
        return bitmap;
    }

    private static Bitmap CreateFolderIcon()
    {
        Bitmap bitmap = new(16, 16);

        using Graphics graphics = Graphics.FromImage(bitmap);
        graphics.Clear(Color.Transparent);

        using Brush tabBrush = new SolidBrush(Color.FromArgb(245, 190, 80));
        using Brush folderBrush = new SolidBrush(Color.FromArgb(241, 196, 92));
        using Pen outline = new(Color.FromArgb(170, 120, 30));

        graphics.FillRectangle(tabBrush, 2, 3, 6, 3);
        graphics.FillRectangle(folderBrush, 1, 5, 14, 9);
        graphics.DrawRectangle(outline, 1, 5, 14, 9);

        return bitmap;
    }

    private static Bitmap CreateTableIcon()
    {
        Bitmap bitmap = new(16, 16);

        using Graphics graphics = Graphics.FromImage(bitmap);
        graphics.Clear(Color.Transparent);

        using Pen pen = new(Color.FromArgb(70, 70, 70));

        graphics.DrawRectangle(pen, 2, 3, 12, 10);
        graphics.DrawLine(pen, 2, 7, 14, 7);
        graphics.DrawLine(pen, 2, 10, 14, 10);
        graphics.DrawLine(pen, 6, 3, 6, 13);
        graphics.DrawLine(pen, 10, 3, 10, 13);

        return bitmap;
    }

    private static Bitmap CreateHourglassIcon()
    {
        Bitmap bitmap = new(16, 16);

        using Graphics graphics = Graphics.FromImage(bitmap);
        graphics.Clear(Color.Transparent);
        graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

        using Pen outline = new(Color.FromArgb(100, 100, 100), 1.4f);
        using Brush sand = new SolidBrush(Color.FromArgb(230, 170, 40));

        Point[] top = { new(4, 3), new(12, 3), new(8, 8) };
        Point[] bottom = { new(4, 13), new(12, 13), new(8, 8) };

        graphics.DrawLine(outline, 4, 3, 12, 3);
        graphics.DrawLine(outline, 4, 13, 12, 13);
        graphics.DrawLine(outline, 4, 3, 8, 8);
        graphics.DrawLine(outline, 12, 3, 8, 8);
        graphics.DrawLine(outline, 4, 13, 8, 8);
        graphics.DrawLine(outline, 12, 13, 8, 8);

        graphics.FillPolygon(sand, top);
        graphics.FillPolygon(sand, bottom);

        return bitmap;
    }

    private async Task ReviewCurrentDocumentAsync()
    {
        if (string.IsNullOrWhiteSpace(_currentFilePath))
        {
            MessageBox.Show(this, "Select a document first.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        if (string.IsNullOrWhiteSpace(sourceEditor.Text))
        {
            MessageBox.Show(this, "The selected document is empty.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        DialogResult confirm = MessageBox.Show(
            this,
            "AI Review will rewrite the current document for spelling, grammar, clarity, and professional formatting. It will not save automatically.\r\n\r\nContinue?",
            "AI Review",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (confirm != DialogResult.Yes)
        {
            return;
        }

        string originalText = sourceEditor.Text;

        try
        {
            SetAiReviewWaitingState(isWaiting: true);

            OpenAiDocumentReviewService reviewService = new(new OpenAiApiKeyStore());
            string reviewedText = await reviewService.ReviewMarkdownAsync(
                Path.GetFileName(_currentFilePath),
                originalText);

            string normalizedReviewedText = NormalizeMarkdownLineEndings(reviewedText);
            _highlightedPreviewLineIndexes = FindChangedLineIndexes(originalText, normalizedReviewedText);

            _isApplyingAiReviewResult = true;
            _suppressPreviewHighlightClear = true;
            try
            {
                sourceEditor.Text = normalizedReviewedText;
                sourceEditor.SelectionStart = 0;
                sourceEditor.SelectionLength = 0;
            }
            finally
            {
                _suppressPreviewHighlightClear = false;
                _isApplyingAiReviewResult = false;
            }

            showSourceButton.Checked = false;
            RenderPreview();

            DialogResult saveResult = MessageBox.Show(
                this,
                "AI review complete. Changed lines are highlighted in yellow in the preview.\r\n\r\nSave AI-reviewed changes to disk?\r\n\r\nYes = Save changes\r\nNo = Keep changes in editor only\r\nCancel = Revert to the original text",
                "Save AI Review?",
                MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Question);

            switch (saveResult)
            {
                case DialogResult.Yes:
                    File.WriteAllText(_currentFilePath, sourceEditor.Text);

                    MessageBox.Show(
                        this,
                        "AI-reviewed document saved. Highlighting will remain until you click the toolbar Save button.",
                        "AI Review",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    break;

                case DialogResult.No:
                    MessageBox.Show(
                        this,
                        "AI-reviewed changes are still in the editor but have not been saved. Highlighting will remain until you click the toolbar Save button.",
                        "AI Review",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    break;

                case DialogResult.Cancel:
                    _suppressPreviewHighlightClear = true;
                    try
                    {
                        sourceEditor.Text = originalText;
                    }
                    finally
                    {
                        _suppressPreviewHighlightClear = false;
                    }

                    _highlightedPreviewLineIndexes.Clear();
                    RenderPreview();

                    MessageBox.Show(
                        this,
                        "AI-reviewed changes were discarded.",
                        "AI Review",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    break;
            }
        }
        catch (Exception ex)
        {
            _suppressPreviewHighlightClear = true;
            try
            {
                sourceEditor.Text = originalText;
            }
            finally
            {
                _suppressPreviewHighlightClear = false;
            }

            _highlightedPreviewLineIndexes.Clear();
            RenderPreview();

            MessageBox.Show(this, ex.Message, "AI Review Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            SetAiReviewWaitingState(isWaiting: false);
        }
    }

    private void SetAiReviewWaitingState(bool isWaiting)
    {
        _isWaitingForAiReply = isWaiting;

        UseWaitCursor = isWaiting;
        Cursor.Current = isWaiting ? Cursors.WaitCursor : Cursors.Default;

        aiReviewButton.Enabled = !isWaiting;
        aiReviewButton.Image = isWaiting
            ? _aiReviewWaitingImage
            : _aiReviewReadyImage;

        aiReviewButton.ToolTipText = isWaiting
            ? "Waiting for AI reply..."
            : "AI Review";

        RenderPreview();
        Application.DoEvents();
    }

    private void SaveCurrentDocument()
    {
        if (string.IsNullOrWhiteSpace(_currentFilePath))
        {
            MessageBox.Show(this, "Select a document first.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        File.WriteAllText(_currentFilePath, sourceEditor.Text);

        _highlightedPreviewLineIndexes.Clear();
        RenderPreview();

        MessageBox.Show(this, "Document saved.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void ReloadCurrentDocument()
    {
        if (string.IsNullOrWhiteSpace(_currentFilePath) || !File.Exists(_currentFilePath))
        {
            MessageBox.Show(this, "Select a document first.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        _suppressPreviewHighlightClear = true;
        try
        {
            sourceEditor.Text = NormalizeMarkdownLineEndings(File.ReadAllText(_currentFilePath));
        }
        finally
        {
            _suppressPreviewHighlightClear = false;
        }

        showSourceButton.Checked = false;
        _highlightedPreviewLineIndexes.Clear();
        RenderPreview();
    }

    private void OpenDocsFolder()
    {
        string docsRoot = string.IsNullOrWhiteSpace(_currentFilePath)
            ? Path.Combine(FindSolutionRoot(), "Docs")
            : Path.GetDirectoryName(_currentFilePath) ?? Path.Combine(FindSolutionRoot(), "Docs");

        Directory.CreateDirectory(docsRoot);
        Process.Start(new ProcessStartInfo { FileName = docsRoot, UseShellExecute = true });
    }

    private void NormalizeCurrentMarkdown()
    {
        if (string.IsNullOrWhiteSpace(sourceEditor.Text))
        {
            return;
        }

        sourceEditor.Text = NormalizeMarkdownStructure(sourceEditor.Text);
        _highlightedPreviewLineIndexes.Clear();
        RenderPreview();
    }

    private void WrapSelection(string prefix, string suffix, string placeholder)
    {
        EnsureSourceVisible();

        string selected = sourceEditor.SelectedText;
        if (string.IsNullOrEmpty(selected))
        {
            selected = placeholder;
        }

        int start = sourceEditor.SelectionStart;
        sourceEditor.SelectedText = prefix + selected + suffix;
        sourceEditor.SelectionStart = start + prefix.Length;
        sourceEditor.SelectionLength = selected.Length;
        sourceEditor.Focus();

        _highlightedPreviewLineIndexes.Clear();
        RenderPreview();
    }

    private void ApplyLinePrefix(string prefix)
    {
        EnsureSourceVisible();

        int selectionStart = sourceEditor.SelectionStart;
        int lineIndex = sourceEditor.GetLineFromCharIndex(selectionStart);
        int lineStart = sourceEditor.GetFirstCharIndexFromLine(lineIndex);

        sourceEditor.SelectionStart = lineStart;
        sourceEditor.SelectionLength = 0;
        sourceEditor.SelectedText = prefix;
        sourceEditor.SelectionStart = selectionStart + prefix.Length;
        sourceEditor.Focus();

        _highlightedPreviewLineIndexes.Clear();
        RenderPreview();
    }

    private void InsertTextAtCursor(string text)
    {
        EnsureSourceVisible();

        int start = sourceEditor.SelectionStart;
        sourceEditor.SelectedText = text;
        sourceEditor.SelectionStart = start + text.Length;
        sourceEditor.Focus();

        _highlightedPreviewLineIndexes.Clear();
        RenderPreview();
    }

    private void EnsureSourceVisible()
    {
        if (!showSourceButton.Checked)
        {
            showSourceButton.Checked = true;
        }
    }

    private void ApplySavedSplitterLayout()
    {
        if (editorSplit.Panel2Collapsed || editorSplit.Height <= 0)
        {
            return;
        }

        DocsEditorSettings settings = LoadSettings();
        int requestedDistance = settings.SourceSplitterDistance;

        if (requestedDistance <= 0)
        {
            requestedDistance = editorSplit.Height - DefaultSourcePanelHeight;
        }

        editorSplit.SplitterDistance = GetSafeSplitterDistance(requestedDistance);
    }

    private void ApplySafeSourceHeight()
    {
        if (editorSplit.Panel2Collapsed || editorSplit.Height <= 0)
        {
            return;
        }

        editorSplit.SplitterDistance = GetSafeSplitterDistance(editorSplit.SplitterDistance);
    }

    private int GetSafeSplitterDistance(int requestedDistance)
    {
        if (editorSplit.Height <= 0)
        {
            return requestedDistance;
        }

        int minimumDistance = Math.Max(editorSplit.Panel1MinSize, MinimumPreviewPanelHeight);
        int maximumDistance = editorSplit.Height - Math.Max(editorSplit.Panel2MinSize, MinimumSourcePanelHeight);

        if (maximumDistance < minimumDistance)
        {
            return Math.Max(editorSplit.Panel1MinSize, Math.Min(requestedDistance, editorSplit.Height - editorSplit.Panel2MinSize));
        }

        return Math.Max(minimumDistance, Math.Min(requestedDistance, maximumDistance));
    }

    private void SaveSplitterLocation()
    {
        if (editorSplit.Panel2Collapsed || editorSplit.Height <= 0)
        {
            return;
        }

        try
        {
            DocsEditorSettings settings = LoadSettings();
            settings.SourceSplitterDistance = editorSplit.SplitterDistance;
            SaveSettings(settings);
        }
        catch
        {
            // Splitter persistence should never break the editor.
        }
    }

    private DocsEditorSettings LoadSettings()
    {
        try
        {
            if (!File.Exists(_settingsPath))
            {
                return new DocsEditorSettings();
            }

            string json = File.ReadAllText(_settingsPath);
            return JsonSerializer.Deserialize<DocsEditorSettings>(json) ?? new DocsEditorSettings();
        }
        catch
        {
            return new DocsEditorSettings();
        }
    }

    private void SaveSettings(DocsEditorSettings settings)
    {
        string? folder = Path.GetDirectoryName(_settingsPath);
        if (!string.IsNullOrWhiteSpace(folder))
        {
            Directory.CreateDirectory(folder);
        }

        string json = JsonSerializer.Serialize(settings, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        File.WriteAllText(_settingsPath, json);
    }

    private void RenderPreview()
    {
        string html = BuildHtmlDocument(
            sourceEditor.Text,
            _highlightedPreviewLineIndexes,
            _isWaitingForAiReply);

        _pendingPreviewHtml = html;

        if (!previewBrowser.IsHandleCreated)
        {
            previewBrowser.CreateControl();
        }

        if (previewBrowser.Document is null)
        {
            previewBrowser.Navigate("about:blank");
            return;
        }

        WritePreviewHtml(html);
    }

    private void PreviewBrowser_DocumentCompleted(object? sender, WebBrowserDocumentCompletedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(_pendingPreviewHtml))
        {
            return;
        }

        WritePreviewHtml(_pendingPreviewHtml);
    }

    private void WritePreviewHtml(string html)
    {
        _isRendering = true;

        try
        {
            HtmlDocument? document = previewBrowser.Document;

            if (document is null)
            {
                _pendingPreviewHtml = html;
                previewBrowser.Navigate("about:blank");
                return;
            }

            document.OpenNew(replaceInHistory: true);
            document.Write(html);
            //document.Close();

            _pendingPreviewHtml = null;
            previewBrowser.Refresh();
        }
        finally
        {
            _isRendering = false;
        }
    }

    private static HashSet<int> FindChangedLineIndexes(string originalText, string revisedText)
    {
        string[] originalLines = SplitNormalizedLines(originalText);
        string[] revisedLines = SplitNormalizedLines(revisedText);

        HashSet<int> changedIndexes = new();

        int max = Math.Max(originalLines.Length, revisedLines.Length);
        for (int index = 0; index < max; index++)
        {
            string original = index < originalLines.Length ? originalLines[index] : string.Empty;
            string revised = index < revisedLines.Length ? revisedLines[index] : string.Empty;

            if (!string.Equals(original, revised, StringComparison.Ordinal) &&
                index < revisedLines.Length &&
                !string.IsNullOrWhiteSpace(revised))
            {
                changedIndexes.Add(index);
            }
        }

        return changedIndexes;
    }

    private static string[] SplitNormalizedLines(string text)
    {
        return NormalizeMarkdownLineEndings(text)
            .Split(new[] { Environment.NewLine }, StringSplitOptions.None);
    }

    private static string BuildHtmlDocument(string markdown, IReadOnlySet<int> highlightedLineIndexes, bool isWaitingForAiReply)
    {
        string waitBanner = isWaitingForAiReply
            ? "<div class=\"ai-wait\"><span class=\"ai-wait-icon\">⏳</span> Waiting for AI Review reply...</div>"
            : string.Empty;

        string body = waitBanner + MarkdownToHtml(markdown, highlightedLineIndexes);

        return $$"""
<!DOCTYPE html>
<html>
<head>
<meta http-equiv="X-UA-Compatible" content="IE=edge" />
<meta charset="utf-8" />
<style>
body { font-family: "Segoe UI", Arial, sans-serif; font-size: 14px; line-height: 1.5; padding: 18px; color: #222; }
h1 { font-size: 26px; border-bottom: 1px solid #ddd; padding-bottom: 8px; }
h2 { font-size: 21px; margin-top: 24px; }
h3 { font-size: 17px; margin-top: 18px; }
p { margin: 8px 0; }
code { font-family: Consolas, monospace; background: #f3f3f3; padding: 2px 4px; }
pre { background: #f3f3f3; padding: 10px; overflow-x: auto; }
blockquote { border-left: 4px solid #bbb; margin-left: 0; padding-left: 12px; color: #555; }
table { border-collapse: collapse; margin: 10px 0; }
td, th { border: 1px solid #ccc; padding: 6px 10px; }
.ai-change { background: #fff3a3; border-left: 4px solid #e0b400; padding-left: 8px; }
.ai-change-inline { background: #fff3a3; padding: 2px 4px; }
.ai-wait { position: sticky; top: 0; z-index: 10; margin: 0 0 12px 0; padding: 10px 12px; background: #fff3cd; border: 1px solid #e0b400; font-weight: 600; }
.ai-wait-icon { margin-right: 8px; }
</style>
</head>
<body>
{{body}}
</body>
</html>
""";
    }

    private static string MarkdownToHtml(string markdown, IReadOnlySet<int> highlightedLineIndexes)
    {
        string normalized = NormalizeMarkdownStructure(markdown);
        string[] lines = normalized.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

        StringBuilder html = new();
        bool inCodeBlock = false;
        bool inList = false;
        bool inOrderedList = false;
        bool inTable = false;

        for (int lineIndex = 0; lineIndex < lines.Length; lineIndex++)
        {
            string rawLine = lines[lineIndex];
            string line = rawLine.TrimEnd('\r');
            string trimmed = line.Trim();
            bool highlight = highlightedLineIndexes.Contains(lineIndex);

            if (trimmed.StartsWith("```", StringComparison.Ordinal))
            {
                CloseList(html, ref inList, ref inOrderedList);
                CloseTable(html, ref inTable);
                html.AppendLine(inCodeBlock ? "</code></pre>" : "<pre><code>");
                inCodeBlock = !inCodeBlock;
                continue;
            }

            if (inCodeBlock)
            {
                string encodedCodeLine = WebUtility.HtmlEncode(line);

                html.AppendLine(highlight
                    ? $"<span class=\"ai-change-inline\">{encodedCodeLine}</span>"
                    : encodedCodeLine);

                continue;
            }

            if (string.IsNullOrWhiteSpace(trimmed))
            {
                CloseList(html, ref inList, ref inOrderedList);
                CloseTable(html, ref inTable);
                continue;
            }

            if (IsMarkdownTableSeparator(trimmed))
            {
                continue;
            }

            if (LooksLikeTableRow(trimmed))
            {
                CloseList(html, ref inList, ref inOrderedList);

                if (!inTable)
                {
                    html.AppendLine("<table>");
                    inTable = true;
                }

                html.Append(highlight ? "<tr class=\"ai-change\">" : "<tr>");
                foreach (string cell in trimmed.Trim('|').Split('|'))
                {
                    html.Append("<td>");
                    html.Append(InlineMarkdownToHtml(cell.Trim()));
                    html.Append("</td>");
                }

                html.AppendLine("</tr>");
                continue;
            }

            CloseTable(html, ref inTable);

            if (trimmed.StartsWith("# "))
            {
                CloseList(html, ref inList, ref inOrderedList);
                html.AppendLine(WrapBlock($"<h1>{InlineMarkdownToHtml(trimmed[2..])}</h1>", highlight));
                continue;
            }

            if (trimmed.StartsWith("## "))
            {
                CloseList(html, ref inList, ref inOrderedList);
                html.AppendLine(WrapBlock($"<h2>{InlineMarkdownToHtml(trimmed[3..])}</h2>", highlight));
                continue;
            }

            if (trimmed.StartsWith("### "))
            {
                CloseList(html, ref inList, ref inOrderedList);
                html.AppendLine(WrapBlock($"<h3>{InlineMarkdownToHtml(trimmed[4..])}</h3>", highlight));
                continue;
            }

            if (trimmed.StartsWith("> "))
            {
                CloseList(html, ref inList, ref inOrderedList);
                html.AppendLine(WrapBlock($"<blockquote>{InlineMarkdownToHtml(trimmed[2..])}</blockquote>", highlight));
                continue;
            }

            if (trimmed.StartsWith("- "))
            {
                if (!inList || inOrderedList)
                {
                    CloseList(html, ref inList, ref inOrderedList);
                    html.AppendLine("<ul>");
                    inList = true;
                    inOrderedList = false;
                }

                html.AppendLine(highlight
                    ? $"<li class=\"ai-change\">{InlineMarkdownToHtml(trimmed[2..])}</li>"
                    : $"<li>{InlineMarkdownToHtml(trimmed[2..])}</li>");
                continue;
            }

            Match orderedMatch = Regex.Match(trimmed, @"^\d+\.\s+(.*)$");
            if (orderedMatch.Success)
            {
                if (!inList || !inOrderedList)
                {
                    CloseList(html, ref inList, ref inOrderedList);
                    html.AppendLine("<ol>");
                    inList = true;
                    inOrderedList = true;
                }

                html.AppendLine(highlight
                    ? $"<li class=\"ai-change\">{InlineMarkdownToHtml(orderedMatch.Groups[1].Value)}</li>"
                    : $"<li>{InlineMarkdownToHtml(orderedMatch.Groups[1].Value)}</li>");
                continue;
            }

            CloseList(html, ref inList, ref inOrderedList);

            html.AppendLine(highlight
                ? $"<p class=\"ai-change\">{InlineMarkdownToHtml(trimmed)}</p>"
                : $"<p>{InlineMarkdownToHtml(trimmed)}</p>");
        }

        CloseList(html, ref inList, ref inOrderedList);
        CloseTable(html, ref inTable);

        if (inCodeBlock)
        {
            html.AppendLine("</code></pre>");
        }

        return html.ToString();
    }

    private static string WrapBlock(string html, bool highlight)
    {
        return highlight
            ? $"<div class=\"ai-change\">{html}</div>"
            : html;
    }

    private static void CloseList(StringBuilder html, ref bool inList, ref bool inOrderedList)
    {
        if (!inList) return;
        html.AppendLine(inOrderedList ? "</ol>" : "</ul>");
        inList = false;
        inOrderedList = false;
    }

    private static void CloseTable(StringBuilder html, ref bool inTable)
    {
        if (!inTable) return;
        html.AppendLine("</table>");
        inTable = false;
    }

    private static bool LooksLikeTableRow(string text)
    {
        return text.StartsWith("|", StringComparison.Ordinal) &&
               text.EndsWith("|", StringComparison.Ordinal) &&
               text.Count(static c => c == '|') >= 2;
    }

    private static bool IsMarkdownTableSeparator(string text)
    {
        if (!LooksLikeTableRow(text)) return false;

        string inner = text.Trim('|')
            .Replace("|", string.Empty, StringComparison.Ordinal)
            .Replace(":", string.Empty, StringComparison.Ordinal)
            .Replace("-", string.Empty, StringComparison.Ordinal)
            .Trim();

        return inner.Length == 0;
    }

    private static string InlineMarkdownToHtml(string text)
    {
        string encoded = WebUtility.HtmlEncode(text);

        encoded = Regex.Replace(encoded, @"`([^`]+)`", "<code>$1</code>");
        encoded = Regex.Replace(encoded, @"\*\*([^*]+)\*\*", "<strong>$1</strong>");
        encoded = Regex.Replace(encoded, @"\*([^*]+)\*", "<em>$1</em>");
        encoded = Regex.Replace(
            encoded,
            @"\[([^\]]+)\]\(([^)]+)\)",
            "<a href=\"$2\">$1</a>");

        return encoded;
    }

    private static string NormalizeMarkdownStructure(string text)
    {
        string value = NormalizeMarkdownLineEndings(text);

        value = Regex.Replace(value, @"(?<!\r?\n)(#{1,6}\s+)", Environment.NewLine + "$1");
        value = Regex.Replace(value, @"(?<!\r?\n)(\|[^|\r\n]+(?:\|[^|\r\n]+)+\|)", Environment.NewLine + "$1");
        value = Regex.Replace(value, @"\r?\n{3,}", Environment.NewLine + Environment.NewLine);

        return value.Trim() + Environment.NewLine;
    }

    private static string NormalizeMarkdownLineEndings(string text)
    {
        return text
            .Replace("\r\n", "\n", StringComparison.Ordinal)
            .Replace("\r", "\n", StringComparison.Ordinal)
            .Replace("\n", Environment.NewLine, StringComparison.Ordinal);
    }

    private static void EnsureTemplate(string path)
    {
        string? folder = Path.GetDirectoryName(path);
        if (!string.IsNullOrWhiteSpace(folder))
        {
            Directory.CreateDirectory(folder);
        }

        if (File.Exists(path)) return;

        string fileName = Path.GetFileName(path);
        string content = fileName switch
        {
            "General_Coding_Spec.md" => "# General Coding Spec\r\n",
            "General_Coding_Rules.md" => "# General Coding Rules\r\n",
            "Addin_Design_Spec_Template.md" => "# Add-in Design Spec Template\r\n",
            "Expensa_Integration_Design_Spec_Template.md" => "# Expensa Integration Design Spec Template\r\n",
            "AI_Addin_Design_Workflow.md" => "# AI Add-in Design Workflow\r\n",
            "ExtensionManager_CatchUp_Latest.md" => "# Extension Manager Catch-Up\r\n",
            _ => $"# {Path.GetFileNameWithoutExtension(path)}\r\n"
        };

        File.WriteAllText(path, content);
    }

    private static string FindSolutionRoot()
    {
        DirectoryInfo? directory = new(AppContext.BaseDirectory);

        while (directory is not null)
        {
            if (string.Equals(directory.Name, "Extensions", StringComparison.OrdinalIgnoreCase))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Extensions");
    }

    private sealed class DocsEditorSettings
    {
        public int SourceSplitterDistance { get; set; }
    }
}
