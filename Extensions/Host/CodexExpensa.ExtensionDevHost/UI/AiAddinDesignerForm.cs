using CodexExpensa.ExtensionDevHost.Models;
using CodexExpensa.ExtensionDevHost.Services.Ai;

namespace CodexExpensa.ExtensionDevHost.UI;

public partial class AiAddinDesignerForm : Form
{
    private readonly AiAddinDesignSessionStore _sessionStore = new();
    private AiAddinDesignSession? _currentSession;

    public AiAddinDesignerForm()
    {
        InitializeComponent();
        WireRuntimeEvents();
        LoadSessions();
    }

    private void WireRuntimeEvents()
    {
        newSessionButton.Click += (_, _) => CreateNewSession();
        saveSessionButton.Click += (_, _) => SaveCurrentSession();
        sendButton.Click += async (_, _) => await SendToAiAsync();
        exportDocsButton.Click += (_, _) => ExportCurrentSessionDocs();
        sessionsListBox.SelectedIndexChanged += (_, _) => SelectSession();
    }

    private void LoadSessions()
    {
        sessionsListBox.Items.Clear();

        foreach (AiAddinDesignSession session in _sessionStore.GetAll())
        {
            sessionsListBox.Items.Add(new SessionListItem(session));
        }

        if (sessionsListBox.Items.Count > 0 && sessionsListBox.SelectedIndex < 0)
        {
            sessionsListBox.SelectedIndex = 0;
        }
    }

    private void CreateNewSession()
    {
        string projectName = projectNameTextBox.Text.Trim();
        _currentSession = _sessionStore.CreateNew(string.IsNullOrWhiteSpace(projectName) ? "NewAddin" : projectName);
        LoadSessions();
        SelectSessionById(_currentSession.SessionId);
    }

    private void SelectSession()
    {
        if (sessionsListBox.SelectedItem is not SessionListItem item)
        {
            return;
        }

        _currentSession = item.Session;
        RefreshEditor();
    }

    private void SelectSessionById(string sessionId)
    {
        for (int i = 0; i < sessionsListBox.Items.Count; i++)
        {
            if (sessionsListBox.Items[i] is SessionListItem item &&
                string.Equals(item.Session.SessionId, sessionId, StringComparison.OrdinalIgnoreCase))
            {
                sessionsListBox.SelectedIndex = i;
                return;
            }
        }
    }

    private void RefreshEditor()
    {
        if (_currentSession is null)
        {
            return;
        }

        projectNameTextBox.Text = _currentSession.ProjectName;

        conversationTextBox.Text = string.Join(
            Environment.NewLine + Environment.NewLine,
            _currentSession.Messages.Select(static x =>
                $"{x.Role} [{x.CreatedUtc:g}]{Environment.NewLine}{x.Content}"));

        designSpecTextBox.Text = _currentSession.CurrentSpecMarkdown;
        integrationSpecTextBox.Text = _currentSession.CurrentIntegrationSpecMarkdown;
    }

    private void SaveCurrentSession()
    {
        if (_currentSession is null)
        {
            return;
        }

        _currentSession.ProjectName = string.IsNullOrWhiteSpace(projectNameTextBox.Text)
            ? _currentSession.ProjectName
            : projectNameTextBox.Text.Trim();

        _currentSession.CurrentSpecMarkdown = designSpecTextBox.Text;
        _currentSession.CurrentIntegrationSpecMarkdown = integrationSpecTextBox.Text;

        _sessionStore.Save(_currentSession);
        LoadSessions();
        SelectSessionById(_currentSession.SessionId);
    }

    private async Task SendToAiAsync()
    {
        string message = userMessageTextBox.Text.Trim();
        if (string.IsNullOrWhiteSpace(message))
        {
            MessageBox.Show(this, "Enter a message first.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        if (_currentSession is null)
        {
            _currentSession = _sessionStore.CreateNew(
                string.IsNullOrWhiteSpace(projectNameTextBox.Text) ? "NewAddin" : projectNameTextBox.Text.Trim());
        }

        try
        {
            UseWaitCursor = true;
            sendButton.Enabled = false;

            _currentSession.ProjectName = string.IsNullOrWhiteSpace(projectNameTextBox.Text)
                ? _currentSession.ProjectName
                : projectNameTextBox.Text.Trim();

            _currentSession.CurrentSpecMarkdown = designSpecTextBox.Text;
            _currentSession.CurrentIntegrationSpecMarkdown = integrationSpecTextBox.Text;

            _currentSession.Messages.Add(new AiAddinDesignMessage
            {
                Role = "User",
                Content = message
            });

            OpenAiAddinDesignerService aiService = new(new OpenAiApiKeyStore());
            AiAddinDesignerResponse response = await aiService.ContinueDesignAsync(_currentSession, message);

            _currentSession.Messages.Add(new AiAddinDesignMessage
            {
                Role = "AI",
                Content = response.AssistantMessage
            });

            if (!string.IsNullOrWhiteSpace(response.ProjectName))
            {
                _currentSession.ProjectName = response.ProjectName.Trim();
            }

            _currentSession.CurrentSpecMarkdown = response.UpdatedSpecMarkdown;
            _currentSession.CurrentIntegrationSpecMarkdown = response.UpdatedIntegrationSpecMarkdown;

            _sessionStore.Save(_currentSession);

            userMessageTextBox.Clear();
            RefreshEditor();
            LoadSessions();
            SelectSessionById(_currentSession.SessionId);
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "AI Add-in Designer", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            sendButton.Enabled = true;
            UseWaitCursor = false;
        }
    }

    private void ExportCurrentSessionDocs()
    {
        if (_currentSession is null)
        {
            MessageBox.Show(this, "Select or create a session first.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        SaveCurrentSession();

        string projectFolder = Path.Combine(FindSolutionRoot(), "Modules", _currentSession.ProjectName);
        string docsFolder = Path.Combine(projectFolder, "Docs");
        Directory.CreateDirectory(docsFolder);

        File.WriteAllText(Path.Combine(docsFolder, "Addin_Design_Spec.md"), _currentSession.CurrentSpecMarkdown);
        File.WriteAllText(Path.Combine(docsFolder, "Expensa_Integration_Design_Spec.md"), _currentSession.CurrentIntegrationSpecMarkdown);
        File.WriteAllText(Path.Combine(docsFolder, "AI_Conversation_Summary.md"), conversationTextBox.Text);

        MessageBox.Show(this, $"Docs exported to:{Environment.NewLine}{docsFolder}", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
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

    private sealed class SessionListItem
    {
        public SessionListItem(AiAddinDesignSession session)
        {
            Session = session;
        }

        public AiAddinDesignSession Session { get; }

        public override string ToString() => $"{Session.ProjectName} – {Session.UpdatedUtc:g}";
    }
}
