using System.Text.Json;
using CodexExpensa.ExtensionDevHost.Models;

namespace CodexExpensa.ExtensionDevHost.Services.Ai;

public sealed class AiAddinDesignSessionStore
{
    private readonly string _rootFolder;

    public AiAddinDesignSessionStore()
    {
        _rootFolder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Expensa",
            "Extensions",
            "AiDesignSessions");

        Directory.CreateDirectory(_rootFolder);
    }

    public IReadOnlyList<AiAddinDesignSession> GetAll()
    {
        List<AiAddinDesignSession> sessions = new();

        foreach (string file in Directory.GetFiles(_rootFolder, "*.json").OrderBy(static x => x))
        {
            AiAddinDesignSession? session = LoadFromFile(file);
            if (session is not null)
            {
                sessions.Add(session);
            }
        }

        return sessions.OrderByDescending(static x => x.UpdatedUtc).ToList();
    }

    public AiAddinDesignSession CreateNew(string projectName = "NewAddin")
    {
        string cleanName = string.IsNullOrWhiteSpace(projectName) ? "NewAddin" : projectName.Trim();

        AiAddinDesignSession session = new()
        {
            ProjectName = cleanName,
            CurrentSpecMarkdown = AiAddinDesignSession.GetDefaultSpec(cleanName),
            CurrentIntegrationSpecMarkdown = AiAddinDesignSession.GetDefaultIntegrationSpec(cleanName)
        };

        Save(session);
        return session;
    }

    public void Save(AiAddinDesignSession session)
    {
        ArgumentNullException.ThrowIfNull(session);
        session.UpdatedUtc = DateTime.UtcNow;

        string json = JsonSerializer.Serialize(session, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        File.WriteAllText(GetPath(session.SessionId), json);
    }

    private AiAddinDesignSession? LoadFromFile(string filePath)
    {
        try
        {
            string json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<AiAddinDesignSession>(json);
        }
        catch
        {
            return null;
        }
    }

    private string GetPath(string sessionId) => Path.Combine(_rootFolder, sessionId + ".json");
}
