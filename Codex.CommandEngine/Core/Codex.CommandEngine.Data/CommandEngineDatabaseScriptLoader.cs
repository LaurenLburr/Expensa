namespace Codex.CommandEngine.Data;

public static class CommandEngineDatabaseScriptLoader
{
    public static string LoadRequiredScript(
        string scriptPath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(scriptPath);

        if (!File.Exists(scriptPath))
        {
            throw new FileNotFoundException(
                $"Database schema script was not found: {scriptPath}",
                scriptPath);
        }

        string sql =
            File.ReadAllText(scriptPath);

        if (string.IsNullOrWhiteSpace(sql))
        {
            throw new InvalidOperationException(
                $"Database schema script is empty: {scriptPath}");
        }

        return sql;
    }
}
