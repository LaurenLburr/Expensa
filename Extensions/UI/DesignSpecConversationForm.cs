using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using CodexExpensa.ExtensionDevHost.Services;

namespace CodexExpensa.ExtensionDevHost.UI;

public partial class DesignSpecConversationForm : Form
{
    private readonly string _projectName;
    private readonly string _designSpecPath;
    private readonly string _conversationPath;
    private readonly AiInstructionContextService _instructionContextService = new();

    private readonly System.Windows.Forms.Timer _previewRenderTimer = new()
    {
        Interval = 450
    };

    private bool _isRendering;

    public DesignSpecConversationForm(string projectName, string designSpecPath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(projectName);
        ArgumentException.ThrowIfNullOrWhiteSpace(designSpecPath);

        _projectName = projectName.Trim();
        _designSpecPath = designSpecPath;
        _conversationPath = Path.Combine(
            Path.GetDirectoryName(_designSpecPath) ?? AppContext.BaseDirectory,
            "DesignConversation.md");

        InitializeComponent();
        WirePreviewRenderTimer();
        WireRuntimeEvents();
        LoadWorkspace();
    }


    private void WirePreviewRenderTimer()
    {
        _previewRenderTimer.Tick += (_, _) =>
        {
            _previewRenderTimer.Stop();
            RenderDesignSpecPreviewPreservingEditorState();
        };
    }

    private void WireRuntimeEvents()
    {
        designSpecEditor.TextChanged += (_, _) =>
        {
            if (_isRendering)
            {
                return;
            }

            ScheduleDesignSpecPreviewRender();
        };
    }

    private void LoadWorkspace()
    {
        EnsureFiles();

        projectNameTextBox.Text = _projectName;
        designSpecPathTextBox.Text = _designSpecPath;
        conversationPathTextBox.Text = _conversationPath;

        designSpecEditor.Text = File.ReadAllText(_designSpecPath);
        conversationEditor.Text = File.ReadAllText(_conversationPath);

        RenderDesignSpecPreviewPreservingEditorState();

        SetStatus("Loaded design spec conversation workspace.");
    }

    private void EnsureFiles()
    {
        string? folder = Path.GetDirectoryName(_designSpecPath);
        if (!string.IsNullOrWhiteSpace(folder))
        {
            Directory.CreateDirectory(folder);
        }

        if (!File.Exists(_designSpecPath))
        {
            File.WriteAllText(_designSpecPath, BuildDefaultDesignSpec());
        }

        if (!File.Exists(_conversationPath) || string.IsNullOrWhiteSpace(File.ReadAllText(_conversationPath)))
        {
            File.WriteAllText(_conversationPath, BuildDefaultConversation());
        }
    }

    private string BuildDefaultDesignSpec()
    {
        return $"""
# {_projectName} – Add-in Design Spec

## Purpose

Describe what this add-in is intended to do.

## User Workflow

1. 
2. 
3. 

## Navigation / Tree Structure

```text
Root
└── Child
```

## Commands

| Command | Purpose |
|---|---|
| | |

## Forms

| Form | Purpose |
|---|---|
| | |

## Data Model

| Entity | Key Fields |
|---|---|
| | |

## Database Design

| Table | Purpose |
|---|---|
| | |

## Revision History

| Date | Change |
|---|---|
| | |

""";
    }

    private string BuildDefaultConversation()
    {
        return $"""
# {_projectName} – Design Conversation

## Purpose

This file stores the ongoing AI design conversation for the add-in design spec.

## Context

- Project: {_projectName}
- Design Spec: {_designSpecPath}

## Conversation

""";
    }

    private void SaveButton_Click(object? sender, EventArgs e)
    {
        SaveWorkspace();
        SetStatus("Saved design spec and conversation.");
    }

    private void ReloadButton_Click(object? sender, EventArgs e)
    {
        LoadWorkspace();
    }

    private void AddMessageButton_Click(object? sender, EventArgs e)
    {
        string message = promptTextBox.Text.Trim();

        if (string.IsNullOrWhiteSpace(message))
        {
            MessageBox.Show(
                this,
                "Enter a message before adding it to the conversation.",
                "Design Conversation",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            return;
        }

        AppendConversationEntry("User", message);

        promptTextBox.Clear();
        SaveWorkspace();

        SetStatus("Added message to the ongoing design conversation. Live AI response wiring is the next step.");
    }

    private void AddAiPlaceholderButton_Click(object? sender, EventArgs e)
    {
        AppendConversationEntry(
            "AI",
            "Live AI response wiring is pending. Use this section to capture design notes until the OpenAI conversation service is connected.");

        SaveWorkspace();
        SetStatus("Added AI placeholder response.");
    }

    private void AppendConversationEntry(string speaker, string message)
    {
        string entry =
            $"{Environment.NewLine}### {speaker} – {DateTime.Now:yyyy-MM-dd HH:mm}{Environment.NewLine}{Environment.NewLine}" +
            $"{message.Trim()}{Environment.NewLine}";

        conversationEditor.AppendText(entry);
        conversationEditor.SelectionStart = conversationEditor.TextLength;
        conversationEditor.ScrollToCaret();
    }

    private void SaveWorkspace()
    {
        File.WriteAllText(_designSpecPath, designSpecEditor.Text);
        File.WriteAllText(_conversationPath, conversationEditor.Text);
        RenderDesignSpecPreviewPreservingEditorState();
    }

    private void OpenDocsFolderButton_Click(object? sender, EventArgs e)
    {
        string? folder = Path.GetDirectoryName(_designSpecPath);

        if (string.IsNullOrWhiteSpace(folder))
        {
            return;
        }

        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
        {
            FileName = folder,
            UseShellExecute = true
        });
    }


    private void ScheduleDesignSpecPreviewRender()
    {
        if (_isRendering)
        {
            return;
        }

        _previewRenderTimer.Stop();
        _previewRenderTimer.Start();
    }

    private void RenderDesignSpecPreviewPreservingEditorState()
    {
        bool editorHadFocus = designSpecEditor.Focused;
        int selectionStart = designSpecEditor.SelectionStart;
        int selectionLength = designSpecEditor.SelectionLength;

        try
        {
            RenderDesignSpecPreview();
        }
        finally
        {
            if (!designSpecEditor.IsDisposed && selectionStart <= designSpecEditor.TextLength)
            {
                designSpecEditor.SelectionStart = selectionStart;
                designSpecEditor.SelectionLength = Math.Min(
                    selectionLength,
                    designSpecEditor.TextLength - selectionStart);
            }

            if (editorHadFocus && !designSpecEditor.IsDisposed)
            {
                designSpecEditor.Focus();
            }
        }
    }

    private void RenderDesignSpecPreview()
    {
        string html = BuildHtmlDocument(designSpecEditor.Text);

        _isRendering = true;

        try
        {
            if (!designSpecPreviewBrowser.IsHandleCreated)
            {
                designSpecPreviewBrowser.CreateControl();
            }

            designSpecPreviewBrowser.DocumentText = html;
            designSpecPreviewBrowser.Refresh();
        }
        finally
        {
            _isRendering = false;
        }
    }

    private static string BuildHtmlDocument(string markdown)
    {
        string body = MarkdownToHtml(markdown);

        return $$"""
<!DOCTYPE html>
<html>
<head>
<meta http-equiv="X-UA-Compatible" content="IE=edge" />
<meta charset="utf-8" />
<style>
body { font-family: "Segoe UI", Arial, sans-serif; font-size: 14px; line-height: 1.5; padding: 16px; color: #222; }
h1 { font-size: 24px; border-bottom: 1px solid #ddd; padding-bottom: 8px; }
h2 { font-size: 20px; margin-top: 22px; }
h3 { font-size: 16px; margin-top: 16px; }
p { margin: 8px 0; }
code { font-family: Consolas, monospace; background: #f3f3f3; padding: 2px 4px; }
pre { background: #f3f3f3; padding: 10px; overflow-x: auto; }
blockquote { border-left: 4px solid #bbb; margin-left: 0; padding-left: 12px; color: #555; }
table { border-collapse: collapse; margin: 10px 0; }
td, th { border: 1px solid #ccc; padding: 6px 10px; }
</style>
</head>
<body>
{{body}}
</body>
</html>
""";
    }

    private static string MarkdownToHtml(string markdown)
    {
        string normalized = markdown
            .Replace("\r\n", "\n", StringComparison.Ordinal)
            .Replace("\r", "\n", StringComparison.Ordinal);

        string[] lines = normalized.Split('\n');
        StringBuilder html = new();

        bool inCodeBlock = false;
        bool inList = false;
        bool inOrderedList = false;
        bool inTable = false;

        foreach (string rawLine in lines)
        {
            string line = rawLine.TrimEnd('\r');
            string trimmed = line.Trim();

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
                html.AppendLine(WebUtility.HtmlEncode(line));
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

                html.Append("<tr>");

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

            if (trimmed.StartsWith("# ", StringComparison.Ordinal))
            {
                CloseList(html, ref inList, ref inOrderedList);
                html.AppendLine($"<h1>{InlineMarkdownToHtml(trimmed[2..])}</h1>");
                continue;
            }

            if (trimmed.StartsWith("## ", StringComparison.Ordinal))
            {
                CloseList(html, ref inList, ref inOrderedList);
                html.AppendLine($"<h2>{InlineMarkdownToHtml(trimmed[3..])}</h2>");
                continue;
            }

            if (trimmed.StartsWith("### ", StringComparison.Ordinal))
            {
                CloseList(html, ref inList, ref inOrderedList);
                html.AppendLine($"<h3>{InlineMarkdownToHtml(trimmed[4..])}</h3>");
                continue;
            }

            if (trimmed.StartsWith("> ", StringComparison.Ordinal))
            {
                CloseList(html, ref inList, ref inOrderedList);
                html.AppendLine($"<blockquote>{InlineMarkdownToHtml(trimmed[2..])}</blockquote>");
                continue;
            }

            if (trimmed.StartsWith("- ", StringComparison.Ordinal))
            {
                if (!inList || inOrderedList)
                {
                    CloseList(html, ref inList, ref inOrderedList);
                    html.AppendLine("<ul>");
                    inList = true;
                    inOrderedList = false;
                }

                html.AppendLine($"<li>{InlineMarkdownToHtml(trimmed[2..])}</li>");
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

                html.AppendLine($"<li>{InlineMarkdownToHtml(orderedMatch.Groups[1].Value)}</li>");
                continue;
            }

            CloseList(html, ref inList, ref inOrderedList);
            html.AppendLine($"<p>{InlineMarkdownToHtml(trimmed)}</p>");
        }

        CloseList(html, ref inList, ref inOrderedList);
        CloseTable(html, ref inTable);

        if (inCodeBlock)
        {
            html.AppendLine("</code></pre>");
        }

        return html.ToString();
    }

    private static void CloseList(StringBuilder html, ref bool inList, ref bool inOrderedList)
    {
        if (!inList)
        {
            return;
        }

        html.AppendLine(inOrderedList ? "</ol>" : "</ul>");
        inList = false;
        inOrderedList = false;
    }

    private static void CloseTable(StringBuilder html, ref bool inTable)
    {
        if (!inTable)
        {
            return;
        }

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
        if (!LooksLikeTableRow(text))
        {
            return false;
        }

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

    private void SetStatus(string message)
    {
        statusLabel.Text = message;
    }
}
    private void AcceptAiUpdateButton_Click(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(_pendingAiSpecUpdate))
        {
            return;
        }

        designSpecEditor.Text = _pendingAiSpecUpdate;

        SaveWorkspace();
        RenderDesignSpecPreviewWhenReady();

        _pendingAiSpecUpdate = string.Empty;
        aiSpecPreviewTextBox.Clear();

        acceptAiUpdateButton.Enabled = false;
        rejectAiUpdateButton.Enabled = false;

        SetStatus("Applied AI Design Spec update.");
    }

    private void RejectAiUpdateButton_Click(object? sender, EventArgs e)
    {
        _pendingAiSpecUpdate = string.Empty;
        aiSpecPreviewTextBox.Clear();

        acceptAiUpdateButton.Enabled = false;
        rejectAiUpdateButton.Enabled = false;

        SetStatus("Rejected AI Design Spec update.");
    }


